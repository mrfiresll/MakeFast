using Newtonsoft.Json;
using MFTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data;

namespace AutoUI.Areas.ConfigUI.Controllers
{
    public class NPOIController : Controller
    {
        [ValidateInput(false)]
        public ActionResult ExportExcel(string gridUrl,
            string referUrl,
            string pageNumber,
            string pageSize,
            string queryParams,
            string title,
            string sortName,
            string sortOrder,
            string columns)
        {
            var postData = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(queryParams))
            {
                postData = queryParams.JsonToDictionary();
            }

            var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Headers["Host"]);
            var requestUrl = gridUrl;

            var dicList = GetDicList(serverUrl, requestUrl, referUrl, postData);

            DataTable dt = null;
            if (!string.IsNullOrEmpty(columns))
            {
                dt = new DataTable();
                var columnDic = columns.JsonToDictionaryList();
                foreach (var dic in columnDic)
                {
                    if (!dt.Columns.Contains(dic.GetValue("title")))
                        dt.Columns.Add(dic.GetValue("title"));
                }

                foreach (var dic in dicList)
                {
                    DataRow dr = dt.NewRow();
                    foreach (var item in dic)
                    {
                        var column = columnDic.FirstOrDefault(a => a.GetValue("field") == item.Key);
                        if (column != null)
                        {
                            dr[column.GetValue("title")] = item.Value;
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }  
            else
            {
                dt = DictionaryExtend.ConvertDicToTable(dicList.ToList());
            }
            
            var buffer = NPOIExcelHelper.Export(dt, "").GetBuffer();
            buffer.CheckNotNull("导出excel出现异常");
            return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
        }

        private IEnumerable<Dictionary<string, object>> GetDicList(string serverUrl, string requestUrl, string referUrl, IDictionary<string, object> data = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrl + requestUrl);

            //cookie
            StringBuilder sbCookie = new StringBuilder();
            for (int i = 0; i < Request.Cookies.Count; i++)
            {
                var cookie = Request.Cookies[i];
                sbCookie.Append("&{0}={1}".ReplaceArg(cookie.Name, cookie.Value));
            }
            if (sbCookie.Length > 0)
            {
                string param = sbCookie.ToString().TrimStart('&');
                request.Headers.Add("Cookie", param);
            }

            request.Timeout = 5000;
            request.Referer = referUrl;
            request.Method = "Post";
            request.ContentType = "application/x-www-form-urlencoded";
            //不加会操作超时
            request.Timeout = System.Threading.Timeout.Infinite;
            request.KeepAlive = true;


            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.Append("&{0}={1}".ReplaceArg(item.Key, item.Value));
            }

            if (sb.Length > 0)
            {
                string param = sb.ToString().TrimStart('&');
                using (Stream newStream = request.GetRequestStream())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(param);
                    newStream.Write(bytes, 0, param.Length);
                }
            }


            HttpWebResponse res = null;
            try
            {
                res = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                throw new Exception("在GetResponse的时候产生了异常", ex);                
            }
            StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            string content = reader.ReadToEnd();//得到结果

            var result = new List<Dictionary<string, object>>();
            if (content.StartsWith("[") && content.EndsWith("]"))
            {
                result = content.JsonToDictionaryList();
            }
            else
            {
                var jsonData = content.JsonToDictionary();
                if (jsonData["rows"] != null)
                {
                    var exportdata = jsonData["rows"].ToString();
                    result = exportdata.JsonToDictionaryList();
                }
            }
            return result;
        }
    }
}