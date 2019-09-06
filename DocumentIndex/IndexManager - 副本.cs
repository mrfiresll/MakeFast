using IFilterTextReader;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PanGu;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentIndex
{
    /// <summary>
    /// 文件内容索引管理类
    /// </summary>
    public class IndexManager
    {
        public static readonly IndexManager SingManager = new IndexManager();
        //索引操作队列
        private static ConcurrentQueue<FileIndexOperation> fileIndexQueue = new ConcurrentQueue<FileIndexOperation>();
        //private static readonly IndexWriter _indexWriter = CreateWriter();

        public Action<FileIndexOperation> WriteSuccessHandler;
        public Action<Exception> ExceptionHandler;

        public void StartNewThread()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(QueueToIndex));
        }

        public void PushOperationToQueue(FileIndexOperation opera)
        {
            fileIndexQueue.Enqueue(opera);
        }
        public List<FileIndexOperation> SearchAll(String keywords)
        {
            IndexSearcher search = new IndexSearcher(_indexDIR);
            keywords = GetKeyWordsSplitBySpace(keywords, new PanGuTokenizer());
            QueryParser queryParser = new QueryParser("contents", new PanGuAnalyzer(true));

            Query query = queryParser.Parse(keywords);

            Hits hits = search.Search(query);

            List<FileIndexOperation> result = new List<FileIndexOperation>();

            for (int i = 0; i < hits.Length(); i++)
            {
                FileIndexOperation fileP = FileIndexOperation.GetReadOpera();
                try
                {
                    fileP.Title = hits.Doc(i).Get("title");
                    //fileP.FilePath = hits.Doc(i).Get("path");
                    //fileP.FileContent = hits.Doc(i).Get("contents");
                    fileP.FileID = hits.Doc(i).Get("fileID");
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
        public List<FileIndexOperation> SearchByPage(String keywords, int pageIndex, int pageCount, out int recCount)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(_indexDIR));
            IndexSearcher search = new IndexSearcher(directory, true);
            keywords = GetKeyWordsSplitBySpace(keywords, new PanGuTokenizer());
            QueryParser queryParser = new QueryParser(new Lucene.Net.Util.Version("sllVersion", 1), "contents", new PanGuAnalyzer(true));

            Query query = queryParser.Parse(keywords);

            Hits hits = search.Search(query);

            List<FileIndexOperation> result = new List<FileIndexOperation>();

            recCount = hits.Length();
            int i = (pageIndex - 1) * pageCount;

            while (i < recCount && result.Count < pageCount)
            {
                FileIndexOperation fileP = null;
                try
                {
                    fileP = FileIndexOperation.GetReadOpera();
                    fileP.Title = hits.Doc(i).Get("title");
                    //fileP.FilePath = hits.Doc(i).Get("path");
                    //fileP = hits.Doc(i).Get("contents");
                    fileP.FileID = hits.Doc(i).Get("fileID");
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
        private IndexManager()
        {

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

        private string _indexDIR
        {
            get
            {
                return PanGu.Framework.Path.GetAssemblyPath() + @"NewIndex";
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
      
        //定义一个线程 将队列中的数据取出来 插入索引库中
        private void QueueToIndex(object para)
        {
            while (true)
            {
                FileIndexOperation fileIndex = null;
                if (fileIndexQueue.TryDequeue(out fileIndex))
                {
                     CRUDIndex((FileIndexOperation)fileIndex.Clone());
                }
                else
                {
                    Thread.Sleep(3000);
                }
            }
        }
        /// <summary>
        /// 更新索引库操作
        /// </summary>
        public void CRUDIndex(FileIndexOperation fileIndex)
        {
            string folder = ConfigurationManager.AppSettings["IndexFolder"] ?? @"NewIndex";
            string indexPath = PanGu.Framework.Path.GetAssemblyPath() + folder;
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
            string fileID = "";
            FileIndexOperation fileIndex1 = FileIndexOperation.GetAddOrUpdateOpera(new byte[] { }, ".doc", true);
            IndexWriter indexWriter = CreateWriter(directory);
            try
            {
                fileID = fileIndex1.FileID;
                if (fileIndex1.EnumIndexAction == EnumIndexAction.Create)
                {
                    AddFileContent(indexWriter, fileIndex1);
                }
                else if (fileIndex1.EnumIndexAction == EnumIndexAction.Delete)
                {
                    //indexWriter.DeleteDocuments(new Term("id", fileIndex.FileID.ToString()));
                }
                else if (fileIndex1.EnumIndexAction == EnumIndexAction.Update)
                {
                    //先删除 再新增
                    //indexWriter.DeleteDocuments(new Term("id", fileIndex.FileID.ToString()));
                    //AddFileContent(indexWriter, fileIndex);
                }

                if (WriteSuccessHandler != null)
                {
                    WriteSuccessHandler(fileIndex1);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("id【" + fileID + "】生成索引失败,Exception:" + ex.Message);
                throw new Exception("id【" + fileID + "】生成索引失败", ex);
            }
            finally
            {
                indexWriter.Close();
                directory.Close();
            }
        }
        /// <summary>
        /// 增加索引的文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private int AddFileContent(IndexWriter writer, FileIndexOperation file)
        {           
            int num = 0;
            string fileContent = "";
            if (file.SourceFileBytes != null && file.SourceFileBytes.Length > 0)
            {
                fileContent = GetContent(file.SourceFileBytes, file.Extension);
            }
            else
            {
                fileContent = GetContent(file.FilePath);
            }

            Document doc = new Document();
            Field field = new Field("fileID", file.FileID ?? "", Field.Store.YES, Field.Index.NO);
            doc.Add(field);
            field = new Field("path", file.FilePath ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);
            field = new Field("title", file.Title ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);
            field = new Field("time", file.CreateTime.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED);
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
        #endregion
    }
}
