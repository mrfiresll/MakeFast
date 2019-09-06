using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentIndex
{
    public class FileIndexOperation : ICloneable
    {
        private FileIndexOperation()
        {
            CreateTime = new DateTime();
            EnumIndexAction = EnumIndexAction.None;
            FileID = Guid.NewGuid().ToString();
        }

        public static FileIndexOperation GetAddOrUpdateOpera(string filePath, bool AddOrUpdate)
        {
            FileIndexOperation opera = new FileIndexOperation();
            opera.FilePath = filePath;
            opera.EnumIndexAction = AddOrUpdate ? EnumIndexAction.Create : EnumIndexAction.Update;
            return opera;
        }

        public static FileIndexOperation GetAddOrUpdateOpera(byte[] bytes, string extension, bool AddOrUpdate)
        {
            FileIndexOperation opera = new FileIndexOperation();
            opera.EnumIndexAction = AddOrUpdate ? EnumIndexAction.Create : EnumIndexAction.Update;
            opera.SourceFileBytes = bytes;
            opera.Extension = extension;
            return opera;
        }

        public static FileIndexOperation GetReadOpera()
        {
            return new FileIndexOperation();
        }


        public string FileID { get; set; }
        public string Title { get; set; }        
        public DateTime CreateTime { get; set; }
        public string FilePath { get; private set; }
        public byte[] SourceFileBytes { get; private set; }
        public string Extension { get; private set; }
        public EnumIndexAction EnumIndexAction { get; private set; }

        public object Clone()
        {
            FileIndexOperation opera = new FileIndexOperation();
            opera.FileID = FileID;
            opera.Title = Title;
            opera.FilePath = FilePath;
            if (SourceFileBytes != null)
                opera.SourceFileBytes = (byte[])SourceFileBytes.Clone();
            opera.Extension = Extension;
            opera.EnumIndexAction = EnumIndexAction;
            return opera;
        }
    }
}
