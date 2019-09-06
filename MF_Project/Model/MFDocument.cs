using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF_Project.Model
{
    [Description("文档")]
    public class MFDocument : Entity
    {
        /// <summary>文件名称</summary>	
        [Description("文件名称")]
        public string Name { get; set; } // Name
        /// <summary>文件编号</summary>	
        [Description("文件编号")]
        public string SerialNumber { get; set; } // SerialNumber
        /// <summary>纸质文档名称</summary>	
        [Description("纸质文档名称")]
        public string PaperName { get; set; } // PaperName
        /// <summary>纸质文档编号</summary>	
        [Description("纸质文档编号")]
        public string PaperCode { get; set; } // PaperCode
        /// <summary>密级</summary>	
        [Description("密级")]
        public string SecrectLevel { get; set; } // SecrectLevel
        /// <summary>关键字</summary>	
        [Description("关键字")]
        public string KeyWords { get; set; } // KeyWords
        /// <summary>文件分类</summary>	
        [Description("文件分类")]
        public string FileCategory { get; set; } // FileCategory
        /// <summary>编制人</summary>	
        [Description("编制人")]
        public string Author { get; set; } // Author
        /// <summary>编制人名称</summary>	
        [Description("编制人名称")]
        public string AuthorName { get; set; } // AuthorName
        /// <summary>编制单位</summary>	
        [Description("编制单位")]
        public string AuthorOrg { get; set; } // AuthorOrg
        /// <summary></summary>	
        [Description("")]
        public string AuthorOrgName { get; set; } // AuthorOrgName
        /// <summary>存放位置</summary>	
        [Description("存放位置")]
        public string Storage { get; set; } // Storage
        /// <summary>当前版本</summary>	
        [Description("当前版本")]
        public decimal? CurrentVersion { get; set; } // CurrentVersion
        [Description("过期日期")]
        public DateTime? ExpiredDate { get; set; } // ExpiredDate
        /// <summary>发布日期</summary>	
        [Description("发布日期")]
        public DateTime? PublishDate { get; set; } // PublishDate
        [Description("浏览文件")]
        public string BrowsFile { get; set; } // BrowsFile
        [Description("")]
        public string MainFileType { get; set; } // MainFileType
        [Description("状态")]
        public string State { get; set; } // State
        /// <summary>存储类型</summary>	
        [Description("存储类型")]
        public string StorageMedia { get; set; } // StorageMedia
        [Description("备注")]
        public string Remark { get; set; } // Remark
    }
}
