using IFilterTextReader;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MFTool;
using PanGu;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentIndex
{  
    public sealed class SearchIndexManager
    {        
        public static SearchIndexManager GetInstance()
        {
            return searchIndexManager;
        }
        public Action<IndexOperation> WriteSuccessHandler;
        public Action<Exception> ExceptionHandler;
        public void AddOpreation(IndexOperation indexContent)
        {
            queue.Enqueue(indexContent);
        }

        /// <summary>
        /// 开启线程，扫描队列，从队列中获取数据
        /// </summary>
        public void StartThread(string indexPath)
        {
            _indexDIR = indexPath;
            Thread myThread = new Thread(WriteIndexContent);
            myThread.IsBackground = true;
            myThread.Start();
        }

        public List<IndexOperation> SearchAll(String keywords)
        {
            IndexSearcher search = new IndexSearcher(_indexDIR);
            keywords = GetKeyWordsSplitBySpace(keywords, new PanGuTokenizer());
            QueryParser queryParser = new QueryParser("contents", new PanGuAnalyzer(true));

            Query query = queryParser.Parse(keywords);

            Hits hits = search.Search(query);

            List<IndexOperation> result = new List<IndexOperation>();

            for (int i = 0; i < hits.Length(); i++)
            {
                IndexOperation fileP = IndexOperation.GetReadOpera();
                try
                {
                    fileP.Title = hits.Doc(i).Get("title");
                    //fileP.FilePath = hits.Doc(i).Get("path");
                    //fileP.FileContent = hits.Doc(i).Get("contents");
                    fileP.Id = hits.Doc(i).Get("fileID");
                    String strTime = hits.Doc(i).Get("time");
                    fileP.CreateTime = DateTime.ParseExact(strTime, "yyyyMMdd", null);
                }
                catch (Exception e)
                {
                    Debug.Print("输出查找数据失败,遗漏一条查找结果,Exception：" + e.Message);
                }
                finally
                {
                    result.Add(fileP);
                }
            }

            search.Close();
            return result;
        }
        public List<IndexOperation> SearchByPage(String keywords, int pageIndex, int pageCount, out int recCount)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(_indexDIR));
            IndexSearcher search = new IndexSearcher(directory, true);
            keywords = GetKeyWordsSplitBySpace(keywords, new PanGuTokenizer());
            QueryParser queryParser = new QueryParser(new Lucene.Net.Util.Version("sllVersion", 1), "contents", new PanGuAnalyzer(true));

            Query query = queryParser.Parse(keywords);

            Hits hits = search.Search(query);

            List<IndexOperation> result = new List<IndexOperation>();

            recCount = hits.Length();
            int i = (pageIndex - 1) * pageCount;

            while (i < recCount && result.Count < pageCount)
            {
                IndexOperation fileP = IndexOperation.GetReadOpera();
                try
                {                   
                    fileP.Title = hits.Doc(i).Get("title");
                    //fileP.FilePath = hits.Doc(i).Get("path");
                    //fileP = hits.Doc(i).Get("contents");
                    fileP.Id = hits.Doc(i).Get("fileID");
                    String strTime = hits.Doc(i).Get("time");
                    fileP.CreateTime = DateTime.ParseExact(strTime, "yyyyMMdd", null);
                }
                catch (Exception e)
                {
                    Debug.Print("输出查找数据失败,遗漏一条查找结果,Exception：" + e.Message);
                }
                finally
                {
                    result.Add(fileP);
                    i++;
                }
            }

            search.Close();
            directory.Close();
            return result;
        }

        #region private
        private Queue<IndexOperation> queue = new Queue<IndexOperation>();
        private static readonly SearchIndexManager searchIndexManager = new SearchIndexManager();
        private SearchIndexManager()
        {
        }
        private void WriteIndexContent()
        {
            while (true)
            {
                if (queue.Count > 0)
                {
                    CreateIndexContent();
                }
                else
                {
                    Thread.Sleep(5000);//避免造成CPU空转
                }
            }
        }

        private IndexWriter CreateWriter(FSDirectory directory)
        {
            bool isExist = IndexReader.IndexExists(directory);
            if (isExist)
            {
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }

            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);

            string minMergeDocs = ConfigurationManager.AppSettings["MinMergeDocs"] ?? "10";
            string maxMergeDocs = ConfigurationManager.AppSettings["MaxMergeDocs"] ?? int.MaxValue.ToString();
            string maxMergeFactor = ConfigurationManager.AppSettings["MaxMergeFactor"] ?? "10";
            writer.SetMaxBufferedDocs(Convert.ToInt32(minMergeDocs));
            writer.SetMaxMergeDocs(Convert.ToInt32(maxMergeDocs));
            writer.SetMergeFactor(Convert.ToInt32(maxMergeFactor));
            return writer;
        }

        private void CreateIndexContent()
        {
            string indexPath = _indexDIR;
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());//指定索引文件(打开索引目录) FS指的是就是FileSystem
            IndexWriter writer = CreateWriter(directory);
            string fileID = "";
            try
            {
                while (queue.Count > 0)
                {
                    IndexOperation indexContent = queue.Dequeue();//将队列中的数据出队
                    fileID = indexContent.Id;                   
                    writer.DeleteDocuments(new Term("fileID", indexContent.Id.ToString()));
                    if (indexContent.LuceneEnum == LuceneEnum.DeleType)
                    {
                        continue;
                    }
                    AddFileContent(writer, indexContent);

                    if (WriteSuccessHandler != null)
                    {
                        WriteSuccessHandler(indexContent);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print("id【" + fileID + "】生成索引失败,Exception:" + ex.Message);
                Logger.GetCurMethodLog().Info("id【" + fileID + "】生成索引失败", ex);
                if (ExceptionHandler != null)
                {
                    ExceptionHandler(ex);
                }
                throw new Exception("id【" + fileID + "】生成索引失败", ex);
            }
            finally
            {
                writer.Close();
                directory.Close();
            }
        }

        /// <summary>
        /// 增加索引的文件
        /// </summary>
        /// <param name="indexContent"></param>
        /// <returns></returns>
        private int AddFileContent(IndexWriter writer, IndexOperation indexContent)
        {
            int num = 0;
            string fileContent = "";
            if (indexContent.AddByPathOrBytes)
            {
                fileContent = GetContent(indexContent.FilePath);                
            }
            else
            {
                fileContent = GetContent(indexContent.SourceFileBytes, indexContent.Extension);
            }

            Document doc = new Document();
            Field field = new Field("fileID", indexContent.Id ?? "", Field.Store.YES, Field.Index.NO);
            doc.Add(field);
            field = new Field("path", indexContent.FilePath ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);
            field = new Field("title", indexContent.Title ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);
            field = new Field("time", indexContent.CreateTime.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);
            field = new Field("contents", fileContent ?? "", Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);
            writer.AddDocument(doc);
            num = writer.MaxDoc();
            writer.Optimize();

            return num;
        }

        private string GetContent(string filtPath)
        {
            var options = new FilterReaderOptions()
            {
                DisableEmbeddedContent = false,
                IncludeProperties = false,
                ReadIntoMemory = false,
                ReaderTimeout = FilterReaderTimeout.NoTimeout,
                Timeout = 0
            };
            string result = "";
            string line;
            var reader = new FilterReader(filtPath, string.Empty, options);
            while ((line = reader.ReadLine()) != null)
            {
                result += (line + Environment.NewLine);
            }

            return result;
        }

        private string GetContent(byte[] bytes, string extension)
        {
            var options = new FilterReaderOptions()
            {
                DisableEmbeddedContent = false,
                IncludeProperties = false,
                ReadIntoMemory = false,
                ReaderTimeout = FilterReaderTimeout.NoTimeout,
                Timeout = 0
            };
            string result = "";
            string line;
            Stream stream = new MemoryStream(bytes);
            var reader = new FilterReader(stream, extension, options);
            while ((line = reader.ReadLine()) != null)
            {
                result += (line + Environment.NewLine);
            }

            return result;
        }

        private string GetKeyWordsSplitBySpace(string keywords, PanGuTokenizer ktTokenizer)
        {
            StringBuilder result = new StringBuilder();
            ICollection<WordInfo> words = ktTokenizer.SegmentToWordInfos(keywords);

            foreach (WordInfo word in words)
            {
                if (word == null)
                {
                    continue;
                }

                result.AppendFormat("{0}^{1}.0 ", word.Word, (int)Math.Pow(3, word.Rank));
            }

            return result.ToString().Trim();
        }

        private string _indexDIR="";
        #endregion
    }
}
