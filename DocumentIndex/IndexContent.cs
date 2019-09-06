using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentIndex
{
    public enum LuceneEnum
    {
        /// <summary>
        /// 添加
        /// </summary>
        AddType,
        /// <summary>
        /// 删除
        /// </summary>
        DeleType,
        /// <summary>
        /// 读取
        /// </summary>
        ReadType
    }
    public class IndexOperation
    {
        private IndexOperation()
        {
            CreateTime = DateTime.Now;
            Id = Guid.NewGuid().ToString();
        }

        public static IndexOperation GetAddOpera(string filePath)
        {
            IndexOperation opera = new IndexOperation();
            opera.FilePath = filePath;
            opera.LuceneEnum = LuceneEnum.AddType;
            opera.AddByPathOrBytes = true;
            return opera;
        }

        public static IndexOperation GetAddOpera(byte[] bytes, string extension)
        {
            IndexOperation opera = new IndexOperation();
            opera.SourceFileBytes = bytes;
            opera.Extension = extension;
            opera.LuceneEnum = LuceneEnum.AddType;
            opera.AddByPathOrBytes = false;
            return opera;
        }

        public static IndexOperation GetReadOpera()
        {
            return new IndexOperation() { LuceneEnum = LuceneEnum.ReadType };
        }

        public static IndexOperation GetDelOpera(string id)
        {
            return new IndexOperation() { Id = id, LuceneEnum = LuceneEnum.DeleType };
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }
        public string FilePath { get; private set; }
        public string Extension { get; private set; }
        public byte[] SourceFileBytes { get; private set; }        
        public LuceneEnum LuceneEnum { get; private set; }
        public bool AddByPathOrBytes { get; private set; }
    }
}
