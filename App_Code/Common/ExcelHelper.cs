using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using NPOI;
//using NPOI.HPSF;
//using NPOI.HSSF;
//using NPOI.HSSF.UserModel;
//using NPOI.HSSF.Util;
//using NPOI.POIFS;
using NPOI.Util;


/// <summary>
/// ExcelHelper 的摘要说明
/// </summary>
public class ExcelHelper
{
    ///// <summary>
    ///// NPOI简单Demo，快速入门代码
    ///// </summary>
    ///// <param name="dtSource"></param>
    ///// <param name="strFileName"></param>
    ///// <remarks>NPOI认为Excel的第一个单元格是：(0，0)</remarks>
    //public static void ExportEasy(DataTable dtSource, string strFileName)
    //{
    //    HSSFWorkbook workbook = new HSSFWorkbook();
    //    HSSFSheet sheet = workbook.CreateSheet();

    //    //填充表头
    //    HSSFRow dataRow = sheet.CreateRow(0);
    //    foreach (DataColumn column in dtSource.Columns)
    //    {
    //        dataRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
    //    }


    //    //填充内容
    //    for (int i = 0; i < dtSource.Rows.Count; i++)
    //    {
    //        dataRow = sheet.CreateRow(i + 1);
    //        for (int j = 0; j < dtSource.Columns.Count; j++)
    //        {
    //            dataRow.CreateCell(j).SetCellValue(dtSource.Rows[i][j].ToString());
    //        }
    //    }


    //    //保存
    //    using (MemoryStream ms = new MemoryStream())
    //    {
    //        using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
    //        {
    //            workbook.Write(ms);
    //            ms.Flush();
    //            ms.Position = 0;
    //            byte[] data = ms.ToArray();
    //            fs.Write(data, 0, data.Length);
    //            fs.Flush();
    //        }
    //    }
    //    sheet.Dispose();
    //    workbook.Dispose();
    //}

    ///// <summary>
    ///// 用于Web导出
    ///// </summary>
    ///// <param name="dtSource"></param>
    ///// <param name="strHeaderText"></param>
    ///// <param name="strFileName"></param>
    //public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName)
    //{

    //    HttpContext curContext = HttpContext.Current;

    //    // 设置编码和附件格式
    //    curContext.Response.ContentType = "application/vnd.ms-excel";
    //    curContext.Response.ContentEncoding = Encoding.UTF8;
    //    curContext.Response.Charset = "";
    //    curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

    //    curContext.Response.BinaryWrite(Export(dtSource, strHeaderText).GetBuffer());
    //    curContext.Response.End();

    //}


    ///// <summary>
    ///// DataTable导出到Excel的MemoryStream
    ///// </summary>
    ///// <param name="dtSource">源DataTable</param>
    ///// <param name="strHeaderText">表头文本</param>
    ///// <returns></returns>
    //public static MemoryStream Export(DataTable dtSource, string strHeaderText)
    //{
    //    HSSFWorkbook workbook = new HSSFWorkbook();
    //    HSSFSheet sheet = workbook.CreateSheet();

    //    #region 右击文件 属性信息
    //    {
    //        DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
    //        dsi.Company = "yuyaoyi";
    //        workbook.DocumentSummaryInformation = dsi;

    //        SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
    //        si.Author = "yuyaoyi"; //填加xls文件作者信息
    //        si.ApplicationName = "带尔康在线检测系统"; //填加xls文件创建程序信息
    //        si.LastAuthor = "yuyaoyi"; //填加xls文件最后保存者信息
    //        si.Comments = "说明信息"; //填加xls文件作者信息
    //        si.Title = ""; //填加xls文件标题信息
    //        si.Subject = "";//填加文件主题信息
    //        si.CreateDateTime = DateTime.Now;
    //        workbook.SummaryInformation = si;
    //    }
    //    #endregion

    //    HSSFCellStyle dateStyle = workbook.CreateCellStyle();
    //    HSSFDataFormat format = workbook.CreateDataFormat();
    //    //dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");

    //    //取得列宽
    //    int[] arrColWidth = new int[dtSource.Columns.Count];
    //    foreach (DataColumn item in dtSource.Columns)
    //    {
    //        arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
    //    }
    //    for (int i = 0; i < dtSource.Rows.Count; i++)
    //    {
    //        for (int j = 0; j < dtSource.Columns.Count; j++)
    //        {
    //            int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
    //            if (intTemp > arrColWidth[j])
    //            {
    //                arrColWidth[j] = intTemp;
    //            }
    //        }
    //    }



    //    int rowIndex = 0;

    //    foreach (DataRow row in dtSource.Rows)
    //    {
    //        #region 新建表，填充表头，填充列头，样式
    //        if (rowIndex == 65535 || rowIndex == 0)
    //        {
    //            if (rowIndex != 0)
    //            {
    //                sheet = workbook.CreateSheet();
    //            }

    //            #region 表头及样式
    //            {
    //                HSSFRow headerRow = sheet.CreateRow(0);
    //                headerRow.HeightInPoints = 25;
    //                headerRow.CreateCell(0).SetCellValue(strHeaderText);

    //                HSSFCellStyle headStyle = workbook.CreateCellStyle();
    //                headStyle.Alignment = CellHorizontalAlignment.CENTER;
    //                HSSFFont font = workbook.CreateFont();
    //                font.FontHeightInPoints = 20;
    //                font.Boldweight = 700;
    //                headStyle.SetFont(font);

    //                headerRow.GetCell(0).CellStyle = headStyle;

    //                sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
    //                headerRow.Dispose();
    //            }
    //            #endregion


    //            #region 列头及样式
    //            {
    //                HSSFRow headerRow = sheet.CreateRow(1);


    //                HSSFCellStyle headStyle = workbook.CreateCellStyle();
    //                headStyle.Alignment = CellHorizontalAlignment.CENTER;
    //                HSSFFont font = workbook.CreateFont();
    //                font.FontHeightInPoints = 10;
    //                font.Boldweight = 700;
    //                headStyle.SetFont(font);


    //                foreach (DataColumn column in dtSource.Columns)
    //                {
    //                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
    //                    headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

    //                    //设置列宽
    //                    sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

    //                }
    //                headerRow.Dispose();
    //            }
    //            #endregion

    //            rowIndex = 2;
    //        }
    //        #endregion


    //        #region 填充内容
    //        foreach (DataColumn column in dtSource.Columns)
    //        {
    //            HSSFRow dataRow = sheet.CreateRow(rowIndex);
    //            HSSFCell newCell = dataRow.CreateCell(column.Ordinal);

    //            string drValue = row[column].ToString();

    //            switch (column.DataType.ToString())
    //            {
    //                case "System.String"://字符串类型
    //                    newCell.SetCellValue(drValue);
    //                    break;
    //                case "System.DateTime"://日期类型
    //                    DateTime date = Convert.ToDateTime(drValue);
    //                    drValue = date.ToString("yyyy-MM-dd HH:mm:ss");
    //                    //DateTime dateV;
    //                    //DateTime.TryParse(drValue, out dateV);
    //                    newCell.SetCellValue(drValue);

    //                    //newCell.SetCellType(HSSFCellType.FORMULA);
    //                    //newCell.CellStyle = dateStyle;//格式化显示
    //                    break;
    //                case "System.Boolean"://布尔型
    //                    bool boolV = false;
    //                    bool.TryParse(drValue, out boolV);
    //                    newCell.SetCellValue(boolV);
    //                    break;
    //                case "System.Int16"://整型
    //                case "System.Int32":
    //                case "System.Int64":
    //                case "System.Byte":
    //                    int intV = 0;
    //                    int.TryParse(drValue, out intV);
    //                    newCell.SetCellValue(intV);
    //                    break;
    //                case "System.Decimal"://浮点型
    //                case "System.Double":
    //                    double doubV = 0;
    //                    double.TryParse(drValue, out doubV);
    //                    newCell.SetCellValue(doubV);
    //                    break;
    //                case "System.DBNull"://空值处理
    //                    newCell.SetCellValue("");
    //                    break;
    //                default:
    //                    newCell.SetCellValue("");
    //                    break;
    //            }

    //        }
    //        #endregion

    //        rowIndex++;
    //    }


    //    using (MemoryStream ms = new MemoryStream())
    //    {
    //        workbook.Write(ms);
    //        ms.Flush();
    //        ms.Position = 0;

    //        sheet.Dispose();
    //        workbook.Dispose();

    //        return ms;
    //    }



    //}


    ///// <summary>
    ///// DataTable导出到Excel文件
    ///// </summary>
    ///// <param name="dtSource">源DataTable</param>
    ///// <param name="strHeaderText">表头文本</param>
    ///// <param name="strFileName">保存位置</param>
    //public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
    //{
    //    using (MemoryStream ms = Export(dtSource, strHeaderText))
    //    {
    //        using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
    //        {
    //            byte[] data = ms.ToArray();
    //            fs.Write(data, 0, data.Length);
    //            fs.Flush();
    //        }
    //    }
    //}

    //public static DataTable Import(Stream fileStream)
    //{
    //    return Import(fileStream, 0, 0);
    //    //DataTable dt = new DataTable();

    //    //HSSFWorkbook hssfworkbook;
    //    //hssfworkbook = new HSSFWorkbook(fileStream);
    //    //HSSFSheet sheet = hssfworkbook.GetSheetAt(0);
    //    //System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

    //    //HSSFRow headerRow = sheet.GetRow(0);
    //    //int cellCount = headerRow.LastCellNum;

    //    //for (int j = 0; j < cellCount; j++)
    //    //{
    //    //    HSSFCell cell = headerRow.GetCell(j);
    //    //    dt.Columns.Add(cell.ToString());
    //    //}

    //    //for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
    //    //{
    //    //    HSSFRow row = sheet.GetRow(i);
    //    //    DataRow dataRow = dt.NewRow();

    //    //    for (int j = row.FirstCellNum; j < cellCount; j++)
    //    //    {
    //    //        if (row.GetCell(j) != null)
    //    //            dataRow[j] = row.GetCell(j).ToString();
    //    //    }

    //    //    dt.Rows.Add(dataRow);
    //    //}

    //    //return dt;
    //}

    ///// <summary>读取excel
    ///// 默认第一行为标头
    ///// </summary>
    ///// <param name="strFileName">excel文档路径</param>
    ///// <returns></returns>
    //public static DataTable Import(string strFileName)
    //{
    //    DataTable dt = new DataTable();

    //    HSSFWorkbook hssfworkbook;
    //    using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
    //    {
    //        hssfworkbook = new HSSFWorkbook(file);
    //    }
    //    HSSFSheet sheet = hssfworkbook.GetSheetAt(0);
    //    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

    //    HSSFRow headerRow = sheet.GetRow(0);
    //    int cellCount = headerRow.Cells.Count;

    //    for (int j = 0; j < cellCount; j++)
    //    {
    //        HSSFCell cell = headerRow.GetCell(j);
    //        dt.Columns.Add(cell.ToString());
    //    }

    //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
    //    {
    //        HSSFRow row = sheet.GetRow(i);
    //        DataRow dataRow = dt.NewRow();

    //        for (int j = row.FirstCellNum; j < cellCount; j++)
    //        {
    //            if (row.GetCell(j) != null)
    //                dataRow[j] = row.GetCell(j).ToString().Trim();
    //        }

    //        dt.Rows.Add(dataRow);
    //    }

    //    //while (rows.MoveNext())
    //    //{
    //    //    HSSFRow row = (HSSFRow)rows.Current;
    //    //    DataRow dr = dt.NewRow();

    //    //    for (int i = 0; i < row.LastCellNum; i++)
    //    //    {
    //    //        HSSFCell cell = row.GetCell(i);


    //    //        if (cell == null)
    //    //        {
    //    //            dr[i] = null;
    //    //        }
    //    //        else
    //    //        {
    //    //            dr[i] = cell.ToString();
    //    //        }
    //    //    }
    //    //    dt.Rows.Add(dr);
    //    //}

    //    return dt;
    //}

    //public static DataTable Import(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
    //{
    //    HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
    //    HSSFSheet sheet = workbook.GetSheet(SheetName);

    //    DataTable table = new DataTable();

    //    HSSFRow headerRow = sheet.GetRow(HeaderRowIndex);
    //    int cellCount = headerRow.LastCellNum;

    //    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
    //    {
    //        DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
    //        table.Columns.Add(column);
    //    }

    //    int rowCount = sheet.LastRowNum;

    //    for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
    //    {
    //        HSSFRow row = sheet.GetRow(i);
    //        DataRow dataRow = table.NewRow();

    //        for (int j = row.FirstCellNum; j < cellCount; j++)
    //            dataRow[j] = row.GetCell(j).ToString();
    //    }

    //    ExcelFileStream.Close();
    //    workbook = null;
    //    sheet = null;
    //    return table;
    //}

    //public static DataTable Import(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
    //{

    //     workbook = new XSSFWorkbook(ExcelFileStream);
    //    XSSFSheet sheet = workbook.GetSheetAt(SheetIndex);

    //    DataTable table = new DataTable();

    //    HSSFRow headerRow = sheet.GetRow(HeaderRowIndex);
        

    //    //for (int i = headerRow.FirstCellNum; i < cellCount; i++)
    //    //{
    //    //    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
    //    //    table.Columns.Add(column);
    //    //}
    //    foreach(HSSFCell cell in headerRow.Cells)
    //    {
    //        DataColumn column = new DataColumn(cell.StringCellValue.Trim());
    //        if(!table.Columns.Contains(column.ColumnName))
    //        {
    //            table.Columns.Add(column);
    //        }
                
    //    }
    //    int cellCount = table.Columns.Count;

    //    int rowCount = sheet.LastRowNum;

    //    for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
    //    {
    //        HSSFRow row = sheet.GetRow(i);
    //        if(row==null)
    //        {
    //            continue;
    //        }
    //        DataRow dataRow = table.NewRow();
    //        bool AllCellIsBlank = true;
    //        for (int j = row.FirstCellNum; j < cellCount; j++)
    //        {
    //            HSSFCell cell = row.GetCell(j);
    //            if (cell != null)
    //            {
    //                //读取Excel格式，根据格式读取数据类型
    //                switch (cell.CellType)
    //                {
    //                    case HSSFCellType.BLANK: //空数据类型处理
    //                        dataRow[j] = "";
    //                        break;
    //                    case HSSFCellType.STRING: //字符串类型
    //                        dataRow[j] = cell.StringCellValue;
    //                        if (!string.IsNullOrEmpty(cell.StringCellValue))
    //                        {
    //                            dataRow[j] = cell.StringCellValue.Trim();
    //                            AllCellIsBlank = false;
    //                        }
    //                        else
    //                            dataRow[j] = null;
    //                        break;
    //                    case HSSFCellType.NUMERIC: //数字类型                                   
    //                        //if (HSSFDateUtil.IsValidExcelDate(cell.NumericCellValue))
    //                        //{
    //                        //    dataRow[j] = cell.DateCellValue;
    //                        //}
    //                        //else
    //                        //{
    //                        //    dataRow[j] = cell.NumericCellValue;
    //                        //}
    //                        short format = cell.CellStyle.DataFormat;
    //                        if (format == 14 || format == 22 || format == 31 || format == 57 || format == 58)//日期格式
    //                        {
    //                            DateTime date = cell.DateCellValue;
    //                            dataRow[j] = date.ToString("yyy-MM-dd HH:mm:ss");
    //                        }
    //                        else if (format == 20 || format == 32)//时间格式
    //                        {
    //                            DateTime date = cell.DateCellValue;
    //                            dataRow[j] = date.ToString("HH:mm:ss");
    //                        }
    //                        else
    //                        {
    //                            dataRow[j] = cell.NumericCellValue;
    //                        }
    //                        AllCellIsBlank = false;
    //                        break;
    //                    case HSSFCellType.FORMULA:
    //                        //HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(workbook);
    //                        //dataRow[j] = e.Evaluate(cell).StringValue;

    //                        //if (!string.IsNullOrEmpty(e.Evaluate(cell).StringValue))
    //                        //{
    //                        //    AllCellIsBlank = false;
    //                        //}
    //                        switch (row.GetCell(j).CachedFormulaResultType)
    //                        {
    //                            case HSSFCellType.STRING:
    //                                string strFORMULA = row.GetCell(j).StringCellValue;
    //                                if (strFORMULA != null && strFORMULA.Length > 0)
    //                                {
    //                                    dataRow[j] = strFORMULA.ToString();
    //                                }
    //                                else
    //                                {
    //                                    dataRow[j] = null;
    //                                }
    //                                break;
    //                            case HSSFCellType.NUMERIC:
    //                                dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
    //                                break;
    //                            case HSSFCellType.BOOLEAN:
    //                                dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
    //                                break;
    //                            //case HSSFCellType.ERROR:
    //                            //    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
    //                            //    break;
    //                            default:
    //                                dataRow[j] = "";
    //                                break;
    //                        }
    //                        break;
    //                    default:
    //                        dataRow[j] = "";
    //                        break;
    //                }                    
    //            }
    //        }
    //        if (!AllCellIsBlank)//如果每个cell都是空值则不保存到dataTable
    //        {
    //            table.Rows.Add(dataRow);
    //        }
            
    //    }

    //    ExcelFileStream.Close();
    //    workbook = null;
    //    sheet = null;
    //    return table;
    //}
}