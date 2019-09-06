using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AutoUI
{
    public class NPOIExcelHelper
    {
        #region 导出
        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// 用于Web导出
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="colNameMap">更改列名<键,值>如果colNameMap没有指定的列则不显示</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">文件名</param>
        public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName, bool AddNumCol = true)
        {
            HttpContext curContext = HttpContext.Current;

            // 设置编码和附件格式
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

            curContext.Response.BinaryWrite(Export(dtSource, strHeaderText, AddNumCol).GetBuffer());
            curContext.Response.End();
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        public static MemoryStream Export(DataTable dtSource, string strHeaderText, bool AddNumCol = true)
        {
            if (AddNumCol)
            {
                DataColumn numCol = dtSource.Columns.Add();
                numCol.SetOrdinal(0);
                numCol.ColumnName = "序号";
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    dtSource.Rows[i][0] = i + 1;
                }
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "文件作者信息"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息

                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            #region 全局属性
            //int maxRowIndex = 65535;
            int lastRowNum = 0;
            int colMaxWidth = 255;//超过255会报错

            //表格统一样式
            HSSFCellStyle allCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            allCellStyle.BorderTop = CellBorderType.THIN;
            allCellStyle.BorderBottom = CellBorderType.THIN;
            allCellStyle.BorderLeft = CellBorderType.THIN;
            allCellStyle.BorderRight = CellBorderType.THIN;

            //时间样式
            HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            dateStyle.CloneStyleFrom(allCellStyle);
            HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //表头、列头样式
            HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            headStyle.CloneStyleFrom(allCellStyle);
            headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            HSSFFont font = (HSSFFont)workbook.CreateFont();
            font.Boldweight = 700;
            font.FontHeightInPoints = 15;
            headStyle.SetFont(font);

            // 列头样式
            HSSFCellStyle columnHeadStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            columnHeadStyle.CloneStyleFrom(allCellStyle);
            columnHeadStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            HSSFFont font2 = (HSSFFont)workbook.CreateFont();
            font2.Boldweight = 700;
            columnHeadStyle.SetFont(font2);
            #endregion

            #region 新建表，设置表头、整体样式
            //表头
            if (strHeaderText != "")
            {
                HSSFRow headerRow = (HSSFRow)sheet.CreateRow(lastRowNum);
                lastRowNum++;
                headerRow.HeightInPoints = 20;
                headerRow.CreateCell(0).SetCellValue(strHeaderText);

                headerRow.GetCell(0).CellStyle = headStyle;
                headerRow.GetCell(0).SetCellValue(strHeaderText);
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
            }

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp > colMaxWidth ? colMaxWidth : intTemp;
                    }
                }
            }

            //设置列宽、填充列头
            HSSFRow columRow = (HSSFRow)sheet.CreateRow(lastRowNum);
            lastRowNum++;
            foreach (DataColumn column in dtSource.Columns)
            {
                //songliangliang 2014.7.1
                //原  sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                HSSFCell newCell = (HSSFCell)columRow.CreateCell(column.Ordinal);
                newCell.CellStyle = columnHeadStyle;
                ConfigCell(ref newCell, column.ColumnName);
                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal]) * 256);
            }
            #endregion

            #region 填充表格内容
            //int rowIndex = sheet.LastRowNum;
            foreach (DataRow row in dtSource.Rows)
            {
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(lastRowNum);
                lastRowNum++;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);
                    ConfigCell(ref newCell, row[column], dateStyle);
                    newCell.CellStyle = allCellStyle;
                }
                //rowIndex++;
            }
            #endregion

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                sheet.Dispose();
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet              
                return ms;
            }
        }

        #endregion 导入
        #region 导入
        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable Import(string strFileName)
        {
            DataTable dt = new DataTable();

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                HSSFCell cell = (HSSFCell)headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = (HSSFRow)sheet.GetRow(i);
                if (row == null)
                {
                    continue;//error
                }

                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        dataRow[j] = parseExcel((HSSFCell)row.GetCell(j));
                    }
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        #endregion
        #region 私有

        /// <summary>
        /// 日期格式
        /// </summary>
        private string[] dtFormat = new string[] 
        {


        };

        private static void ConfigCell(ref HSSFCell newCell, object obj, HSSFCellStyle dateStyle = null)
        {
            //加边框
            //newCell.CellStyle.BorderTop = CellBorderType.THIN;
            //newCell.CellStyle.BorderBottom = CellBorderType.THIN;
            //newCell.CellStyle.BorderLeft = CellBorderType.THIN;
            //newCell.CellStyle.BorderRight = CellBorderType.THIN;
            string drValue = obj.ToString();
            switch (obj.GetType().ToString())
            {
                case "System.String"://字符串类型
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型
                    DateTime dateV;
                    //如果为空直接输出空值
                    if (string.IsNullOrEmpty(drValue))
                    {
                        newCell.SetCellValue("");
                    }
                    else
                    {
                        DateTime.TryParse(drValue, out dateV);
                        newCell.SetCellValue(dateV);
                    }

                    newCell.CellStyle = dateStyle;//格式化显示
                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    double doubV = 0;
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
                    break;
                case "System.DBNull"://空值处理
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }

        private static string parseExcel(HSSFCell cell)
        {
            string result = "";
            switch (cell.CellType)
            {
                case CellType.NUMERIC:// 数字类型   
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        // 处理日期格式、时间格式   
                        result = cell.DateCellValue.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        double value = cell.NumericCellValue;
                        //HSSFCellStyle style = (HSSFCellStyle)cell.CellStyle;
                        //DecimalFormat format = new DecimalFormat();
                        //String temp = style.GetDataFormatString();
                        // 单元格设置成常规   
                        //if (temp.Equals("General"))
                        //{
                        //    format = new DecimalFormat("#");
                        //}
                        result = value.ToString();
                    }
                    break;
                case CellType.STRING:// String类型   
                    result = cell.StringCellValue;
                    break;
                case CellType.BLANK:
                    result = "";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
        }
        #endregion
    }
}