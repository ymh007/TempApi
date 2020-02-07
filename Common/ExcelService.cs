using log4net;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Common
{
    /// <summary>
    /// 导出Excel服务
    /// </summary>
    public class ExcelService
    {
        static HSSFWorkbook hssfworkbook;

        /// <summary>
        /// 实例化日志
        /// </summary>
        /// 
        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 投票专用Excel导出

        #region 导出Excel  
        /// <summary>  
        /// 使用DataTable导出数据  
        /// </summary>  
        /// <param name="dt">数据</param>
        /// <param name="fileName">文件名</param>
        /// <param name="title"></param>
        public static void DtToExcel(DataTable dt, string fileName, string title)
        {
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (fileName.Trim() == " ")//验证strFileName是否为空或值无效  
                    { throw new Exception("验证strFileName是否为空或值无效"); }
                    hssfworkbook = new HSSFWorkbook();
                    ISheet sheet = hssfworkbook.CreateSheet();
                    int colscount = dt.Columns.Count;//定义表格内数据的列数                 
                    ArrayList arr = new ArrayList();//储存头信息  
                    HSSFPalette palette = hssfworkbook.GetCustomPalette();
                    ICellStyle cstyle = hssfworkbook.CreateCellStyle();
                    ICellStyle cstyleTitle = hssfworkbook.CreateCellStyle();
                    palette.SetColorAtIndex(8, 218, 238, 243);
                    palette.SetColorAtIndex(9, 146, 205, 220);
                    HSSFColor hssFColor = palette.FindColor(218, 238, 243);
                    HSSFColor hssFTitleColor = palette.FindColor(146, 205, 220);

                    cstyle.FillForegroundColor = hssFColor.Indexed;
                    cstyle.FillPattern = FillPattern.SolidForeground;
                    cstyleTitle.VerticalAlignment = VerticalAlignment.Center;
                    cstyleTitle.Alignment = HorizontalAlignment.Center;
                    cstyleTitle.VerticalAlignment = VerticalAlignment.Center;
                    cstyleTitle.FillForegroundColor = hssFTitleColor.Indexed;
                    cstyleTitle.FillPattern = FillPattern.SolidForeground;

                    int displayColumnsCount = 0;//用于处理隐藏列头不被显示   

                    IRow rowTitle = sheet.CreateRow(0);//创建列头数据  
                    rowTitle.Height = 25 * 20;

                    IRow rowHeader = sheet.CreateRow(1);//创建列头数据  
                    rowHeader.Height = 25 * 20;



                    HSSFFont hssffont = hssfworkbook.CreateFont() as HSSFFont;
                    hssffont.FontHeightInPoints = 11;
                    CellRangeAddress c = CellRangeAddress.ValueOf(string.Format("A2:{0}2", ToName(dt.Columns.Count - 1)));//增加筛选
                    sheet.SetAutoFilter(c);

                    ICell cellTitle = rowTitle.CreateCell(0);
                    cellTitle.CellStyle = cstyleTitle;
                    sheet.SetColumnWidth(displayColumnsCount, 17 * 256);
                    //设置单元格内容
                    cellTitle.SetCellValue(title);
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dt.Columns.Count - 1));
                    cellTitle.CellStyle = cstyleTitle;

                    //设置单元格边框   
                    for (int i = 0; i < dt.Columns.Count; i++)//生成表头信息  
                    {
                        ICell cell = rowHeader.CreateCell(displayColumnsCount);
                        cell.CellStyle = cstyle;
                        cell.SetCellValue(dt.Columns[i].ToString());
                        sheet.SetColumnWidth(displayColumnsCount, 17 * 256);
                        displayColumnsCount++;
                        arr.Add(dt.Columns[i].ToString());
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        IRow irow = sheet.CreateRow(i + 2);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ICell cell = irow.CreateCell(j);
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                    var folder = fileName.Substring(0, fileName.LastIndexOf('\\'));
                    if (!Directory.Exists(folder))// 判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(folder);//不存在则创建文件夹 
                    }
                    using (Stream stream = File.OpenWrite(fileName))
                    {
                        hssfworkbook.Write(stream);
                    }
                }
                else
                {
                    throw new Exception("没有可导出的数据");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion



        #region 工作汇报 导出Excel  
        /// <summary>  
        /// 工作汇报 导出Excel  
        /// </summary>  
        /// <param name="dt">数据</param>
        /// <param name="fileName">文件名</param>
        /// <param name="title"></param>
        public static void GenerateExcel(DataTable dt, string fileName, string title, List<string> columns)
        {
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(fileName))//验证strFileName是否为空或值无效  
                    { throw new Exception("附件路径不能为空！"); }
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook();
                    ISheet myReceiveSheet = xssfworkbook.CreateSheet("我收到的");
                    ISheet mySendSheet = xssfworkbook.CreateSheet("我发送的");
                    ICellStyle headerStyle = xssfworkbook.CreateCellStyle();
                    headerStyle.FillForegroundColor =HSSFColor.Grey40Percent.Index;
                    headerStyle.FillPattern = FillPattern.SolidForeground;
                    ICellStyle dataStyle = xssfworkbook.CreateCellStyle();
                    ICellStyle data_border_Style = xssfworkbook.CreateCellStyle();
                    data_border_Style.BorderBottom = BorderStyle.Thin;
                    data_border_Style.BorderLeft = BorderStyle.Thin;
                    data_border_Style.BorderRight = BorderStyle.Thin;
                    data_border_Style.BorderTop = BorderStyle.Thin;
                    dataStyle.BorderBottom = BorderStyle.Thin;
                    dataStyle.BorderLeft = BorderStyle.Thin;
                    dataStyle.BorderRight = BorderStyle.Thin;
                    dataStyle.BorderTop = BorderStyle.Thin;
                    dataStyle.VerticalAlignment = VerticalAlignment.Center;
                    dataStyle.Alignment = HorizontalAlignment.Center;
                    dataStyle.VerticalAlignment = VerticalAlignment.Center;
                    IRow receiveHeader = myReceiveSheet.CreateRow(0);//创建列头数据  
                    IRow sendHeader = mySendSheet.CreateRow(0);//创建列头数据  
                    XSSFFont headerFont = xssfworkbook.CreateFont() as XSSFFont;
                    //headerFont.FontName = "宋体";
                    headerFont.IsBold = true;
                    headerFont.FontHeightInPoints = 12;

                    XSSFFont dataFont = xssfworkbook.CreateFont() as XSSFFont;
                    //headerFont.FontName = "宋体";
                    headerFont.FontHeightInPoints = 12;

                    CellRangeAddress c = CellRangeAddress.ValueOf(string.Format("A1:{0}1", ToName(columns.Count - 1)));//增加筛选
                    myReceiveSheet.SetAutoFilter(c);
                    mySendSheet.SetAutoFilter(c);
                    //设置单元格边框   
                    for (int i = 0; i < columns.Count; i++)//生成表头信息  
                    {
                        ICell reveive = receiveHeader.CreateCell(i);
                        ICell send = sendHeader.CreateCell(i);
                        reveive.CellStyle = headerStyle;
                        reveive.CellStyle.SetFont(headerFont);
                        reveive.SetCellValue(columns[i]);
                        send.CellStyle = headerStyle;
                        send.CellStyle.SetFont(headerFont);
                        send.SetCellValue(columns[i]);
                        if (i == 4)
                        {
                            myReceiveSheet.SetColumnWidth(i, 40 * 256);
                            mySendSheet.SetColumnWidth(i, 40 * 256);
                        }
                        else
                        {
                            myReceiveSheet.SetColumnWidth(i, 17 * 256);
                            mySendSheet.SetColumnWidth(i, 17 * 256);
                        }
                    }
                    int s = 1, r = 1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        IRow irow = null;
                        if ((bool)dt.Rows[i]["IsSender"])   // 我发送的
                        {
                            irow = mySendSheet.CreateRow(s);
                            s++;
                        }
                        else  // 我收到的
                        {
                            irow = myReceiveSheet.CreateRow(r);
                            r++;
                        }
                        for (int j = 0; j < columns.Count; j++)
                        {
                            ICell cell = irow.CreateCell(j);
                            cell.SetCellValue(dt.Rows[i][j + 3].ToString());
                            cell.CellStyle.SetFont(dataFont);
                            if (j != 4)
                            {
                                cell.CellStyle = dataStyle;
                            }
                            else
                            {
                                cell.CellStyle = data_border_Style;
                            }
                        }
                    }
                    var folder = fileName.Substring(0, fileName.LastIndexOf('\\'));
                    if (!Directory.Exists(folder))// 判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(folder);//不存在则创建文件夹 
                    }
                    using (Stream stream = File.OpenWrite(fileName))
                    {
                        xssfworkbook.Write(stream);
                    }
                }
                else
                {
                    throw new Exception("没有可导出的数据");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion

        #region - 由数字转换为Excel中的列字母 -

        public static int ToIndex(string columnName)
        {
            if (!Regex.IsMatch(columnName.ToUpper(), @"[A-Z]+")) { throw new Exception("invalid parameter"); }

            int index = 0;
            char[] chars = columnName.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index - 1;
        }


        public static string ToName(int index)
        {
            if (index < 0) { throw new Exception("invalid parameter"); }

            List<string> chars = new List<string>();
            do
            {
                if (chars.Count > 0) index--;
                chars.Insert(0, ((char)(index % 26 + (int)'A')).ToString());
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return String.Join(string.Empty, chars.ToArray());
        }
        #endregion

        #endregion


        #region - 客研数据Excel导入 -
        /// <summary>
        /// 读取Excel数据到DataTable(公共类)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheets"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DataTable GetExcelData(string fileName, int sheets, int rows, int columns)
        {
            DataTable dataTable = new DataTable();
            HttpPostedFile postFile = HttpContext.Current.Request.Files[fileName];
            if (postFile == null)
            {
                throw new Exception("无效的数据源");
            }
            string uploadFileName = postFile.FileName;
            string path = HttpContext.Current.Server.MapPath("~/Resources/" + uploadFileName);
            postFile.SaveAs(path);
            IWorkbook workbook = null;
            ISheet sheet = null;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                if (Path.GetExtension(path).IndexOf(".xlsx") >= 0) // 2007版本
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (Path.GetExtension(path).IndexOf(".xls") >= 0) // 2003版本
                {
                    workbook = new HSSFWorkbook(fileStream);
                }
                sheet = workbook.GetSheetAt(sheets);
                workbook.Close();
            }
            IRow row;
            ICell cell;
            // 列名
            row = sheet.GetRow(rows);
            for (int i = columns; i < row.LastCellNum; i++)
            {
                cell = row.GetCell(i);
                if (cell != null)
                {
                    dataTable.Columns.Add(new DataColumn(cell.ToString()));
                }
            }
            // 数据
            for (int i = rows + 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row != null)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int j = columns; j < dataTable.Columns.Count; ++j)
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
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return dataTable;
        }
        #endregion

    }
}