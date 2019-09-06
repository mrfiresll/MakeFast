using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JJTZZXDB
{
    [Serializable]
    public abstract class Entity
    {
        /// <summary>
        /// 数据实体基类
        /// </summary>
        protected Entity()
        {
            //Id = DomainHelper.CreateTimeOrderID();
            CreateTime = DateTime.Now;
        }

        ///// <summary>
        ///// 获取或设置 版本控制标识，用于处理并发
        ///// 在做更新操作的时候模型中的Timestamp一定要与取出时的一致
        ///// [Timestamp]——整条数据控制  [ConcurrencyCheck]——字段控制
        ///// </summary>        
        //[Timestamp]
        //public byte[] RowVersion { get; set; }

        [MaxLength(50)]
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual string Id { get; set; }

        #region Web赋值
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreateUserID { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改人ID
        /// </summary>
        public string ModifyUserID { get; set; }
        /// <summary>
        /// 修改人名称
        /// </summary>
        public string ModifyUserName { get; set; }
        #endregion

        #region EFBase赋值
        /// <summary>
        /// 排序字段
        /// </summary>
        public double OrderIndex { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        #endregion

        public string GetCSProjName()
        {
            return Assembly.GetCallingAssembly().GetName().Name;
        }
    }
}
