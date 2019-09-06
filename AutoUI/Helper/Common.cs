using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Threading;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Web.Mvc;

namespace AutoUI
{ 
    public class Common
    {
        /// <summary>
        /// 获取客户端Ip地址
        /// </summary>
        /// <returns>客户端Ip地址</returns>
        public static string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //win7
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }

        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="page">对应的page</param>
        /// <param name="height">高</param>
        /// <param name="width">宽</param>
        /// <param name="url">url地址</param>
        /// <param name="control">如果是在updatePanel内调用，则需要传入改对象</param>
        public static void ShowWebDialog(Page page, int height,int width, string url, Control control = null)
        {
            string script = string.Format("window.showModalDialog('{0}',window,'toolbar:0;menubar:0;status:0;resizeable :1;dialogHeight={1}px;dialogWidth={2}px;');", url, height, width);
            if (control == null)
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "", "<script language=javascript>" + script + "</script>");
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(control, page.GetType(), "unReport", script, true);
            }
            //page.Response.Write(script);
        }

        /// <summary>
        /// 字符串省略
        /// </summary>
        /// <param name="oldStr">原字符串</param>
        /// <param name="maxLength">最大长度限定</param>
        /// <param name="endWith">超出部分的显示字符，如...</param>
        /// <returns>处理后的字符串</returns>
        public static string StringTruncat(string oldStr, int maxLength, string endWith)
        {
            if (string.IsNullOrEmpty(oldStr))
                return oldStr + endWith;
            if (maxLength < 1)
                throw new Exception("返回的字符串长度必须大于[0] ");
            if (oldStr.Length > maxLength)
            {
                string strTmp = oldStr.Substring(0, maxLength);
                if (string.IsNullOrEmpty(endWith))
                    return strTmp;
                else
                    return strTmp + endWith;
            }
            return oldStr;
        }

        /// <summary> 
        /// 分割IP
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <returns></returns>
        private static long GetIpNum(String ipAddress)
        {
            String[] ip = ipAddress.Split(new char[] { '.' });
            if (ip.Length < 4)
            {
                return -1;//error
            }
            long a = int.Parse(ip[0]);
            long b = int.Parse(ip[1]);
            long c = int.Parse(ip[2]);
            long d = int.Parse(ip[3]);
            return a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d;
        }

        /// <summary>     
        /// 判断客户端的IP是否在某个ip段中
        /// </summary>   
        /// <param name="clientIp">客户端的IP</param>  
        /// <param name="begin">开始IP</param> 
        /// <param name="end">结束IP</param>  
        /// <returns>是/否</returns>  
        public static bool IsInner(string strClientIp, string strBegin, string strEnd)
        {
            long clientIp = GetIpNum(strClientIp);
            long begin = GetIpNum(strBegin);
            long end = GetIpNum(strEnd);
            return (clientIp >= begin) && (clientIp <= end);
        }

        /// <summary>
        /// 执行客户端脚本语言
        /// </summary>
        /// <param name="page">page对象</param>
        /// <param name="strScript">脚本函数，不需要<script></script></param>
        public static void StartJavaScript(Page page, string strScript)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "", "<script language=javascript>" + strScript + "</script>");
        }
        
        /// <summary>
        /// 重新load page
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="info">alert内容</param>
        /// <param name="strId">id</param>
        /// <param name="path">另外指定跳转页面</param>
        public static void ReloadThePage(Page page, string info = "", string strId = "", string path = "")
        {
            string strScript = "";
            if (path != "")
            {
                strScript = "window.location='http://' + location.host + '" + path;
            }
            else
            {
                strScript = "window.location='http://' + location.host + '" + page.Request.Path;
            }

            if (strId != "")
            {
                strScript = strScript + "?" + strId;
            }
            strScript += "'";
            if (info != "")
            {
                strScript = "alert('" + info + "')" + ";" + strScript;
            }
            page.ClientScript.RegisterStartupScript(page.GetType(), "", "<script language=javascript>" + strScript + "</script>");
        }


        public static void ShowOkFormDialog(Page page, int toId, int fromId, string title = "")
        {
            string url1 = HttpUtility.UrlEncode(page.Request.RawUrl);
            title = HttpUtility.UrlEncode(title);
            string script = string.Format("<script language=javascript>window.showModalDialog('../okForm.aspx?url={0}&id={1}&title={2}&fromId={3}','Details','toolbar:0;menubar:0;status:0;resizeable :1;dialogHeight=265px;')</script>", url1, toId.ToString(), title, fromId);
            page.Response.Write(script);
        }

        public static void GoOnOrBack(Page page, string describ, string url)
        {          
            string script = "if (confirm('" + describ + "'))";
            script += "{ window.location.href='" + page.Request.Path + "';}";
            script += "else";          
            script += "{ window.location.href= '" + url + "' }";
            Common.StartJavaScript(page, script);
        }

        public static void RedoOrStay(Page page, string describ)
        {
            string script = "if (confirm('" + describ + "'))";
            script += "{ window.location.href='" + page.Request.Path + "';}";
            script += "else";
            script += "{   }";
            Common.StartJavaScript(page, script);
        }

        public static void RepairMultiTextBoxMaxLength(Page page, TextBox txtBox)
        {
            string
            lengthFunction = "function isMaxLength(txtBox) {";
            lengthFunction += " if(txtBox) { ";
            lengthFunction += " return ( txtBox.value.length <=" + txtBox.MaxLength + ");";
            lengthFunction += " }";
            lengthFunction += "}";

            txtBox.Attributes.Add("onkeypress", "return isMaxLength(this);");
            page.ClientScript.RegisterClientScriptBlock(
            page.GetType(),
            "txtLength",
            lengthFunction, true);

        }

        public static bool ContractRegCheck(string contractNum)
        {
            Regex reg = new Regex(@"^\d{4}-[0-9A-Z]{1}\d{4}$");//
            return reg.IsMatch(contractNum);
        }
        public static bool ProjectRegCheck(string projectNum)
        {
            Regex reg = new Regex(@"^\d{4}-[0-9A-Z]{1}\d{4}-\d{2}-\d{2}$");//
            return reg.IsMatch(projectNum);
        }

        public static string ReplaceAmp_nbsp(string str)
        {
            str = str.Replace("&", "");
            str = str.Replace("amp;", "");
            str = str.Replace("nbsp;", "");
            str.Trim();
            return str;
        }        

        /// <summary>
        /// 禁用或启用Column
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="colIndex"></param>
        /// <param name="bb"></param>
        public static void ChangeEnableGridColumn(GridView gv, int[] colIndex, bool bb)
        {
            for (int i = 0; i < colIndex.Length; i++)
            {
                if (gv.Columns.Count <= colIndex[i])
                {
                    continue;//error
                }

               // gv.Columns[colIndex[i]].Visible = bb;
                foreach (GridViewRow row in gv.Rows)
                {
                    row.Cells[colIndex[i]].Enabled = bb;
                }         
            }
            
        }

        /// <summary>
        /// 合并GridView中某列相同信息的行（单元格）
        /// </summary>
        /// <param name="GridView1"></param>
        /// <param name="cellNum"></param>
        public static void GroupCol(GridView GridView1, bool bShowTotal, params int[] cols)
        {
            if (cols.Length < 0)
            {
                return;
            }

            if (GridView1.Rows.Count < 1)
            {
                return;
            }

            List<TableCell> oldTcs = GetCells(GridView1, 0, cols);
           
            for (int i = 1; i < GridView1.Rows.Count; i++)
            {
                List<TableCell> tcs = GetCells(GridView1, i, cols);
                for (int j = 0; j < tcs.Count; j++)
                {
                    if (tcs[j].Text == oldTcs[j].Text)
                    {
                        tcs[j].Visible = false;
                        if (oldTcs[j].RowSpan == 0)
                        {
                            oldTcs[j].RowSpan = 1;
                        }

                        oldTcs[j].RowSpan++;
                        
                        oldTcs[j].VerticalAlign = VerticalAlign.Middle;

                        if (i == GridView1.Rows.Count - 1)
                        {
                            if (bShowTotal)
                                oldTcs[j].Text += "(" + oldTcs[j].RowSpan + ")";
                        }
                    }
                    else
                    {
                        if (bShowTotal)
                            oldTcs[j].Text += "(" + oldTcs[j].RowSpan + ")";
                        oldTcs[j] = tcs[j];
                    }
                }               
            }
        } 

        private static List<TableCell> GetCells(GridView GridView1,int rowIndex,params int[] cols)
        {
            List<TableCell> cells = new List<TableCell>();//GridView1.Rows[0].Cells[cols];
            for (int i = 0; i < cols.Length; i++)
            {
                int col = cols[i];
                cells.Add(GridView1.Rows[rowIndex].Cells[col]);
            }
            return cells;
        }
        public static string FileName(int id, string fileName)
        {
            return id + "_" + fileName;
        }
        public static string GetMIME(string fileExtend)
        {
            switch (fileExtend)
            {
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                    return "image/jpeg";
                default:
                    Debug.Assert(false, "找不到" + fileExtend + "的MIME类型,返回空");
                    return "";
            }
        }
        /// <summary>
        /// 导出报表专用
        /// </summary>
        /// <param name="type">文档类型</param>
        /// <returns>返回类型为string</returns>
        public static string mimeType(string type)
        {
            switch (type)
            {
                case "Word":
                    return "application/msword";
                case "Excel":
                    return "application/vnd.ms-excel";
                case "PDF":
                    return "application/pdf";
                case "Image":
                    return "image/TIF";
                default:
                    Debug.Assert(false, "找不到" + type + "的MIME类型,返回空");
                    return "";
            }
        }

        ///// <summary>
        ///// RDLC报表,数据类型为Datatable
        ///// </summary>
        ///// <param name="type">文档类型</param>
        ///// <param name="ReportPath">报表路径</param>
        ///// <param name="DataName">数据集名称</param>
        ///// <param name="list">筛选之后的数据</param>
        ///// <returns>返回类型为字符串</returns>
        //public static byte[] RdlcReport(string type, string ReportPath, string DataName, object list)
        //{
        //    LocalReport localReport = new LocalReport();
        //    localReport.ReportPath = ReportPath;
        //    ReportDataSource reportDataSource = new ReportDataSource(DataName, list);
        //    localReport.DataSources.Add(reportDataSource);
        //    string reportType = type;
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo =                              //deviceInfo限制输出文档的长宽高等参数
        //        "<DeviceInfo>" +
        //        "<OutPutFormat>" + type + "</OutPutFormat>" +
        //        "<PageWidth>11in</PageWidth>" +
        //        "<PageHeight>11in</PageHeight>" +
        //        "<MarginTop>0.5in</MarginTop>" +
        //        "<MarginLeft>1in</MarginLeft>" +
        //        "<MarginRight>1in</MarginRight>" +
        //        "<MarginBottom>0.5in</MarginBottom>" +
        //        "</DeviceInfo>";
        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderedBytes;

        //    renderedBytes = localReport.Render(
        //        reportType,
        //        deviceInfo,
        //        out mimeType,
        //        out encoding,
        //        out fileNameExtension,
        //        out streams,
        //        out warnings
        //        );
        //    return renderedBytes;
        //}
        ///// <summary>
        ///// RDLC报表,数据类型为List转Datatable
        ///// </summary>
        ///// <param name="type">文档类型</param>
        ///// <param name="ReportPath">报表路径</param>
        ///// <param name="DataName">数据集名称</param>
        ///// <param name="reportDataSource">筛选之后的数据</param>
        ///// <returns>返回类型为字符串</returns>
        //public static byte[] RdlcReportNullList(string type, string ReportPath, ReportDataSource reportDataSource)
        //{
        //    LocalReport localReport = new LocalReport();
        //    localReport.ReportPath = ReportPath;
        //    localReport.DataSources.Add(reportDataSource);
        //    string reportType = type;
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo =                              //deviceInfo限制输出文档的长宽高等参数
        //        "<DeviceInfo>" +
        //        "<OutPutFormat>" + type + "</OutPutFormat>" +
        //        "<PageWidth>11in</PageWidth>" +
        //        "<PageHeight>11in</PageHeight>" +
        //        "<MarginTop>0.5in</MarginTop>" +
        //        "<MarginLeft>1in</MarginLeft>" +
        //        "<MarginRight>1in</MarginRight>" +
        //        "<MarginBottom>0.5in</MarginBottom>" +
        //        "</DeviceInfo>";
        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderedBytes;

        //    renderedBytes = localReport.Render(
        //        reportType,
        //        deviceInfo,
        //        out mimeType,
        //        out encoding,
        //        out fileNameExtension,
        //        out streams,
        //        out warnings
        //        );
        //    return renderedBytes;
        //}
    }
}