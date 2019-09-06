using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF_Base.Model
{
    [Description("数据源")]
    public class SQLDataSource : Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string DBName { get; set; }
        public string SQL { get; set; }
        public string Remark { get; set; }

        /// <summary>
        /// 获取返回的所有列名
        /// </summary>
        /// <returns></returns>
        public List<string> GetFields()
        {
            SqlHelper sqlHelper = new SqlHelper(DBName, true);
            sqlHelper.CheckNotNull("无法创建DBName为{0}的sqlHelper".ReplaceArg(DBName));
            SQL.CheckNotNullOrEmpty("SQL语句为空");
            var dt = sqlHelper.ExcuteTable("select top 1 * from ({0}) tmp".ReplaceArg(SQL));

            List<string> resFields = new List<string>();
            foreach(DataColumn dc in dt.Columns)
            {
                resFields.Add(dc.ColumnName);
            }
            return resFields;
        }

        public IEnumerable<string> GetFieldValueList(string field)
        {
            SqlHelper sqlHelper = new SqlHelper(DBName, true);
            sqlHelper.CheckNotNull("无法创建DBName为{0}的sqlHelper".ReplaceArg(DBName));
            SQL.CheckNotNullOrEmpty("SQL语句为空");
            var dt = sqlHelper.ExcuteTable("select {0} from ({1}) tmp".ReplaceArg(field, SQL));
            var resList = dt.AsEnumerable().Select(a => a[field].ToString());
            return resList;
        }

        public DataTable GetDataTable()
        {
            SqlHelper sqlHelper = new SqlHelper(DBName, true);
            sqlHelper.CheckNotNull("无法创建DBName为{0}的sqlHelper".ReplaceArg(DBName));
            SQL.CheckNotNullOrEmpty("SQL语句为空");
            return sqlHelper.ExcuteTable(SQL);
        }        
    }

    
}
