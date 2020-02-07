using MCS.Library.Data.DataObjects;
using MCS.Library.Office.OpenXml.Excel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Seagull2.YuanXin.AppApi.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using NPOI.XSSF.UserModel;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class ExcelHelp<T, TCollection> where T : new() where TCollection : List<T>, new()
    {
        /// <summary>
        /// 读取 Excel 数据到 DataTable（第一行必须是列名）
        /// </summary>
        /// <param name="fileFormName">表单字段名</param>
        public static DataTable GetExcelData(string fileFormName)
        {
            DataTable dataTable = new DataTable();
            if (HttpContext.Current.Request.Files.Count == 0) throw new Exception ("附件不能为空");
            HttpPostedFile postFile = HttpContext.Current.Request.Files[fileFormName];
            string uploadFileName = postFile.FileName;
            string path = HttpContext.Current.Server.MapPath("~/Resources/" + uploadFileName);
            if(Path.GetExtension(uploadFileName)!=".xlsx" && Path.GetExtension(uploadFileName)!=".xls") throw new Exception("请上传Excel文件");
            postFile.SaveAs(path);
            IWorkbook workbook = null;
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (Path.GetExtension(path).IndexOf(".xlsx") >= 0) // 2007版本
            {
                workbook = new XSSFWorkbook(fileStream);
            }
            else if (Path.GetExtension(path).IndexOf(".xls") >= 0) // 2003版本
            {
                workbook = new HSSFWorkbook(fileStream);
            }
           
            ISheet sheet = workbook.GetSheetAt(0);
            IRow row;
            ICell cell;

            // 列名
            row = sheet.GetRow(0);
            for (int i = 0; i < row.LastCellNum; i++)
            {
                cell = row.GetCell(i);
                if (cell != null)
                {
                    dataTable.Columns.Add(new DataColumn(cell.ToString()));
                }
            }

            // 数据
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row != null)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int j = 0; j < dataTable.Columns.Count; ++j)
                    {
                        cell = row.GetCell(j);
                        if (cell != null)
                        {
                            dataRow[j] = cell.ToString();
                        }
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            fileStream.Close();
            workbook.Close();

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return dataTable;
        }

        /// <summary>
        /// 将数据导出EXCEL
        /// </summary>
        /// <param name="displayAndColumnName">显示名和列名字典集合</param>
        /// <param name="tlist">数据源</param>
        /// <param name="exportFileName">导出文件名</param>
        public static HttpResponseMessage ExportExcelData(Dictionary<string, string> displayAndColumnName, TCollection tlist, string exportFileName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");
            IRow headRow = sheet1.CreateRow(0);
            IFont font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            ICellStyle headCellStyle = workbook.CreateCellStyle();
            headCellStyle.SetFont(font);
            headCellStyle.Alignment = HorizontalAlignment.Center;
            List<string> displayNameList = displayAndColumnName.Keys.ToList();
            for (var i = 0; i < displayAndColumnName.Keys.Count; i++)
            {
                ICell cell = headRow.CreateCell(i);
                cell.SetCellValue(displayNameList[i].ToString());
                cell.CellStyle = headCellStyle;
            }
            List<string> columnNameList = displayAndColumnName.Values.ToList();
            ICellStyle contentCellStyle = workbook.CreateCellStyle();
            contentCellStyle.Alignment = HorizontalAlignment.Left;
            contentCellStyle.WrapText = false;
            for (var i = 0; i < tlist.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (var j = 0; j < columnNameList.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    cell.SetCellValue(getPropertyValue(tlist[i], columnNameList[j]));
                    cell.CellStyle = contentCellStyle;
                }
            }
            for (int i = 0; i < displayNameList.Count; i++)
            {
                sheet1.AutoSizeColumn(i);
            }
            MemoryStream file = new MemoryStream();
            workbook.Write(file);
            file.Seek(0, SeekOrigin.Begin);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(file);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = exportFileName + ".xls";
            return result;
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        public static string getPropertyValue(T t, string propertyName)
        {
            PropertyInfo info = t.GetType().GetProperty(propertyName);
            //获取属性值转换暂设置如下字段，可根据实际情况添加
            if (info.PropertyType == typeof(DateTime))
            {
                return Convert.ToDateTime(info.GetValue(t)).ToString("yyyy-MM-dd HH:mm");
            }
            return info.GetValue(t) == null ? "" : info.GetValue(t).ToString();
        }
    }
}