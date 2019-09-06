using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFTool;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MF_Base.Model
{
    [Description("配置的列表")]
    public class ListConfig : Entity
    {
        public string MainTypeFullId { get; set; }
        public string UICode { get; set; }
        public string DBName { get; set; }
        public string EntityFullName { get; set; }//列表绑定的实体
        public string Name { get; set; }
        public string TableName { get; set; }
        public string SQL { get; set; }
        public string OrderBy { get; set; }
        public string Script { get; set; }
        public string PropertySetting { get; set; }
        public string ColumnSetting { get; set; }
        public string ButtonSetting { get; set; }

        public void UpdateColumn(List<Dictionary<string, object>> dicList)
        {
            ColumnSetting = dicList.Where(a => a.GetValue("state") != "delete").ToJson();
        }

        public IEnumerable<Dictionary<string, object>> GetColumnList()
        {
            return ColumnSetting.JsonToDictionaryList().OrderBy(a => Convert.ToInt32(
                        string.IsNullOrEmpty(a.GetValue("OrderIndex")) ? 0 : Convert.ToInt32(a.GetValue("OrderIndex")))
                        );
        }

        public IEnumerable<string> GetQuickSearchColumnFieldList()
        {
            return ColumnSetting.JsonToDictionaryList().Where(a => !string.IsNullOrEmpty(a.GetValue("quickSearch")) && a.GetValue("quickSearch").ToLower() == "true")
                .Select(a=>a.GetValue("field"));
        }

        public IEnumerable<string> GetQuickSearchColumnTitleList()
        {
            return ColumnSetting.JsonToDictionaryList().Where(a => !string.IsNullOrEmpty(a.GetValue("quickSearch")) && a.GetValue("quickSearch").ToLower() == "true")
                .Select(a => a.GetValue("title"));
        }

        public void UpdateButton(List<Dictionary<string, object>> dicList)
        {
            ButtonSetting = dicList.Where(a => a.GetValue("state") != "delete").ToJson();
        }

        public IEnumerable<Dictionary<string, object>> GetButtonList()
        {
            return ButtonSetting.JsonToDictionaryList().OrderBy(a => Convert.ToInt32(
                        string.IsNullOrEmpty(a.GetValue("OrderIndex")) ? 0 : Convert.ToInt32(a.GetValue("OrderIndex")))
                        );
        }

        public List<string> GetScriptFunctionList()
        {
            List<string> res = new List<string>();
            string RegexStr = @"function\s+([^\(]+)";
            Match mat = Regex.Match(Script, RegexStr);
            for (int i = 0; i < mat.Groups.Count; i++)
            {
                res.Add(mat.Groups[i].Value);
            }
            return res;
        }
    }
}
