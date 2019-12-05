using System;
using System.Web;
using System.Data;
using System.IO;
using System.Text;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.Util;
using System.Collections.Generic;

/// <summary>
/// ExcelHelperV2_0 的摘要说明
/// </summary>
public class ExcelHelperV2_0
{
    public ExcelHelperV2_0()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable Import(HttpRequest request)
    {
        DataTable dt = null;
        HttpFileCollection httpFileCollection = request.Files;
        HttpPostedFile file = null;
        if (httpFileCollection.Count > 0)
        {
            file = httpFileCollection[0];
            //Excel读取
            dt = ExcelHelperV2_0.Import(file.InputStream, 0, 0);
        }
        return dt;
        //    return null;
    }

    public static DataTable Import(HttpRequest request, int headerIndex)
    {
        DataTable dt = null;
        HttpFileCollection httpFileCollection = request.Files;
        HttpPostedFile file = null;
        if (httpFileCollection.Count > 0)
        {
            file = httpFileCollection[0];
            //Excel读取
            dt = ExcelHelperV2_0.Import(file.InputStream, 0, headerIndex);
        }
        return dt;
        //    return null;
    }

    public static DataTable Import(Stream fileStream)
    {
        return Import(fileStream, "0", 0);
    }

    public static DataTable Import(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
    {
        XSSFWorkbook wbXssf = null;
        HSSFWorkbook wbHssf = null;
        ISheet sheet = null;
        DataTable dt = null;
        try
        {
            wbXssf = new XSSFWorkbook(ExcelFileStream);
            sheet = wbXssf.GetSheet(SheetName);
        }
        catch (Exception ex)
        {
            if (ex.ToString().Contains("Wrong Local header signature"))
            {
                wbHssf = new HSSFWorkbook(ExcelFileStream);
                sheet = wbHssf.GetSheet(SheetName);
            }
        }
        finally
        {
            dt = Import(sheet, HeaderRowIndex);
            wbXssf = null;
            wbHssf = null;
            sheet = null;
            ExcelFileStream.Close();
        }
        return dt;
    }

    public static DataTable Import(ISheet sheet, int HeaderRowIndex)
    {
        if (sheet == null)
            return null;

        DataTable table = new DataTable();

        IRow headerRow = sheet.GetRow(HeaderRowIndex);
        IRow headerRow1 = sheet.GetRow(HeaderRowIndex - 1); // 记录上一行表头，用以处理多行表头情况

        int index = 1;  int tempI = 0;
        foreach (ICell cell in headerRow.Cells)
        {
            DataColumn column = new DataColumn(cell.ToString().Trim());

            if (string.IsNullOrEmpty(cell.ToString().Trim()))
            {
                column = new DataColumn(headerRow1.Cells[tempI].ToString().Trim());
            }

            if (!table.Columns.Contains(column.ColumnName))
            {
                table.Columns.Add(column);
            }
            else
            {
                column.ColumnName = column.ColumnName + index;
                table.Columns.Add(column);

                index++;
            }
            tempI++;
        }

        int cellCount = table.Columns.Count;

        int rowCount = sheet.LastRowNum;

        for (int i = (sheet.FirstRowNum + HeaderRowIndex + 1); i <= rowCount; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null || row.FirstCellNum < 0)
            {
                continue;
            }
            DataRow dataRow = table.NewRow();
            bool AllCellIsBlank = true;
            for (int j = row.FirstCellNum; j < cellCount; j++)
            {
                ICell cell = row.GetCell(j);
                if (cell != null)
                {
                    //读取Excel格式，根据格式读取数据类型
                    switch (cell.CellType)
                    {
                        case CellType.Blank: //空数据类型处理
                            dataRow[j] = "";
                            break;
                        case CellType.String: //字符串类型
                            dataRow[j] = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(cell.StringCellValue))
                            {
                                dataRow[j] = cell.StringCellValue.Trim();
                                AllCellIsBlank = false;
                            }
                            else
                                dataRow[j] = null;
                            break;
                        case CellType.Numeric: //数字类型                                   
                            dataRow[j] = cell.NumericCellValue;
                            AllCellIsBlank = false;
                            break;
                        case CellType.Formula:
                            switch (row.GetCell(j).CachedFormulaResultType)
                            {
                                case CellType.String:
                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                    {
                                        dataRow[j] = strFORMULA.ToString();
                                    }
                                    else
                                    {
                                        dataRow[j] = null;
                                    }
                                    break;
                                case CellType.Numeric:
                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                    break;
                                case CellType.Boolean:
                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                    break;
                                default:
                                    dataRow[j] = "";
                                    break;
                            }
                            break;
                        default:
                            dataRow[j] = "";
                            break;
                    }
                }
            }
            if (!AllCellIsBlank)//如果每个cell都是空值则不保存到dataTable
            {
                table.Rows.Add(dataRow);
            }

        }        
        
        return table;
    }
    
    public static DataTable Import(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
    {
        XSSFWorkbook wbXssf = null;
        HSSFWorkbook wbHssf = null;
        ISheet sheet = null;
        DataTable dt = null;
        try
        {
            wbXssf = new XSSFWorkbook(ExcelFileStream);
            sheet = wbXssf.GetSheetAt(SheetIndex);
        }
        catch (Exception ex)
        {
            if(ex.ToString().Contains("Wrong Local header signature"))
            {
                wbHssf = new HSSFWorkbook(ExcelFileStream);
                sheet = wbHssf.GetSheetAt(SheetIndex);
            }            
        }
        finally
        {
            //CostSharingHelper.importCostSharing(sheet, HeaderRowIndex, true);
            dt = Import(sheet, HeaderRowIndex);
            //dt = CostSharingHelper.importCostSharing(dt, true);
            wbXssf = null;
            wbHssf = null;
            sheet = null;
            ExcelFileStream.Close();
        }

        return dt;
    }

    /// <summary>
    /// 用于Web导出
    /// </summary>
    /// <param name="dtSource"></param>
    /// <param name="strHeaderText"></param>
    /// <param name="strFileName"></param>
    public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName,string[] chineseHeaders)
    {

        HttpContext curContext = HttpContext.Current;

        // 设置编码和附件格式
        curContext.Response.ContentType = "application/vnd.ms-excel";
        curContext.Response.ContentEncoding = Encoding.UTF8;
        curContext.Response.Charset = "";
        curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

        curContext.Response.BinaryWrite(Export(dtSource, strHeaderText,chineseHeaders).GetBuffer());
        curContext.Response.End();
    }

    public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName, string title)
    {

        HttpContext curContext = HttpContext.Current;

        // 设置编码和附件格式
        curContext.Response.ContentType = "application/vnd.ms-excel";
        curContext.Response.ContentEncoding = Encoding.UTF8;
        curContext.Response.Charset = "";
        curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

        curContext.Response.BinaryWrite(Export(dtSource, strHeaderText, title).GetBuffer());
        curContext.Response.End();
    }


    /// <summary>
    /// DataTable导出到Excel的MemoryStream
    /// </summary>
    /// <param name="dtSource">源DataTable</param>
    /// <param name="strHeaderText">表头文本</param>
    /// <returns></returns>
    public static MemoryStream Export(DataTable dtSource, string strHeaderText,string[] chineseHeaders)
    {
        HSSFWorkbook workbook = new HSSFWorkbook();
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

        #region 右击文件 属性信息
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Yeli";
            workbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Author = "yelioa"; //填加xls文件作者信息
            si.ApplicationName = strHeaderText; //填加xls文件创建程序信息
            si.LastAuthor = "yelioa"; //填加xls文件最后保存者信息
            si.Comments = "说明信息"; //填加xls文件作者信息
            si.Title = ""; //填加xls文件标题信息
            si.Subject = "";//填加文件主题信息
            si.CreateDateTime = DateTime.Now;
            workbook.SummaryInformation = si;
        }
        #endregion

        HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
        HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
        //dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");

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
                    arrColWidth[j] = intTemp;
                }
            }
        }

        int rowIndex = 0;

        foreach (DataRow row in dtSource.Rows)
        {
            #region 新建表，填充表头，填充列头，样式
            if (rowIndex == 65535 || rowIndex == 0)
            {
                if (rowIndex != 0)
                {
                    sheet = (HSSFSheet)workbook.CreateSheet();
                }

                #region 表头及样式
                {
                    HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
                    headerRow.HeightInPoints = 25;
                    headerRow.CreateCell(0).SetCellValue(strHeaderText);

                    HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                    headStyle.Alignment = HorizontalAlignment.Center;
                    //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                    HSSFFont font = (HSSFFont)workbook.CreateFont();
                    font.FontHeightInPoints = 20;
                    font.Boldweight = 700;
                    headStyle.SetFont(font);

                    headerRow.GetCell(0).CellStyle = headStyle;

                    sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                    //headerRow.Dispose();
                }
                #endregion

                #region 列头及样式
                {
                    HSSFRow headerRow = (HSSFRow)sheet.CreateRow(1);
                    HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                    //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                    HSSFFont font = (HSSFFont)workbook.CreateFont();
                    font.FontHeightInPoints = 10;
                    font.Boldweight = 700;
                    headStyle.SetFont(font);
                    //string[] chineseHeaders;
                    //if(strHeaderText=="部门积分信息")
                    //{
                    //    chineseHeaders =new string[] { "姓名", "部门", "月度积分", "季度积分", "年度积分", "总积分" };
                    //}
                    //else
                    // chineseHeaders = new string[] {"序号", "编号", "提交日期", "审批日期", "财务审批日期",
                    //    "提交人", "部门", "费用归属部门", "产品", "费用明细", "金额", "实报金额", "状态", "审批人", "抄送人", "备注", "审批意见", "审批结果"};

                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        int colWidth = sheet.GetColumnWidth(i) * 2;
                        if (colWidth < 255 * 256)
                        {
                            sheet.SetColumnWidth(i, colWidth < 3000 ? 3000 : colWidth);
                        }
                        else
                        {
                            sheet.SetColumnWidth(i, 6000);
                        }
                        DataColumn column = dtSource.Columns[i];
                        if(chineseHeaders.Length==0 || chineseHeaders.Length!= dtSource.Columns.Count)
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                        else
                            headerRow.CreateCell(column.Ordinal).SetCellValue(chineseHeaders[i]);
                        headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                        //设置列宽
                        //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 255);

                    }
                   // headerRow.Dispose();
                }
                #endregion

                rowIndex = 2;
            }
            #endregion

            HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);

            #region 填充内容
            int rowIndex_y = 0;
            foreach (DataColumn column in dtSource.Columns)
            {
                HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);

                string drValue = row[column].ToString();

                switch (column.DataType.ToString())
                {
                    case "System.String"://字符串类型
                        newCell.SetCellValue(drValue);
                        break;
                    case "System.DateTime"://日期类型
                        if ("".Equals(drValue))
                        {
                            newCell.SetCellValue(drValue);
                            break;
                        }
                        DateTime date = Convert.ToDateTime(drValue);
                        drValue = date.ToString("yyyy-MM-dd HH:mm:ss");
                        //DateTime dateV;
                        //DateTime.TryParse(drValue, out dateV);
                        newCell.SetCellValue(drValue);

                        //newCell.SetCellType(HSSFCellType.FORMULA);
                        //newCell.CellStyle = dateStyle;//格式化显示
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

                // 插入批注
                //if (row.Table.Columns.Contains(column + "_addition")) {
                //    HSSFPatriarch patr = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
                //    HSSFComment comment = (HSSFComment)patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, rowIndex, rowIndex_y, rowIndex, rowIndex_y));
                //    comment.String = new HSSFRichTextString(row[column + "_addition"].ToString());
                //    comment.Author = @"黄慧";
                //    newCell.CellComment = comment;
                //}
                //rowIndex_y++;
            }
            #endregion

            rowIndex++;
        }


        using (MemoryStream ms = new MemoryStream())
        {
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            //sheet.Dispose();
            //workbook.Dispose();

            return ms;
        }

    }


    public static MemoryStream Export(DataTable dtSource, string strHeaderText,string title)
    {
        HSSFWorkbook workbook = new HSSFWorkbook();
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

        #region 右击文件 属性信息
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Yeli";
            workbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Author = "yelioa"; //填加xls文件作者信息
            si.ApplicationName = title; //填加xls文件创建程序信息
            si.LastAuthor = "yelioa"; //填加xls文件最后保存者信息
            si.Comments = "说明信息"; //填加xls文件作者信息
            si.Title = title; //填加xls文件标题信息
            si.Subject = "";//填加文件主题信息
            si.CreateDateTime = DateTime.Now;
            workbook.SummaryInformation = si;
        }
        #endregion

        HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
        HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
        //dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");

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
                    arrColWidth[j] = intTemp;
                }
            }
        }

        int rowIndex = 0;

        foreach (DataRow row in dtSource.Rows)
        {
            #region 新建表，填充表头，填充列头，样式
            if (rowIndex == 65535 || rowIndex == 0)
            {
                if (rowIndex != 0)
                {
                    sheet = (HSSFSheet)workbook.CreateSheet();
                }

                #region 表头及样式
                {
                    HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
                    headerRow.HeightInPoints = 25;
                    headerRow.CreateCell(0).SetCellValue(strHeaderText);

                    HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                    headStyle.Alignment = HorizontalAlignment.Center;
                    //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                    HSSFFont font = (HSSFFont)workbook.CreateFont();
                    font.FontHeightInPoints = 20;
                    font.Boldweight = 700;
                    headStyle.SetFont(font);

                    headerRow.GetCell(0).CellStyle = headStyle;

                    sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                    //headerRow.Dispose();
                }
                #endregion

                #region 列头及样式
                {
                    HSSFRow headerRow = (HSSFRow)sheet.CreateRow(1);
                    HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                    //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                    HSSFFont font = (HSSFFont)workbook.CreateFont();
                    font.FontHeightInPoints = 10;
                    font.Boldweight = 700;
                    headStyle.SetFont(font);
                    //string[] chineseHeaders;
                    //if(strHeaderText=="部门积分信息")
                    //{
                    //    chineseHeaders =new string[] { "姓名", "部门", "月度积分", "季度积分", "年度积分", "总积分" };
                    //}
                    //else
                    // chineseHeaders = new string[] {"序号", "编号", "提交日期", "审批日期", "财务审批日期",
                    //    "提交人", "部门", "费用归属部门", "产品", "费用明细", "金额", "实报金额", "状态", "审批人", "抄送人", "备注", "审批意见", "审批结果"};

                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        int colWidth = sheet.GetColumnWidth(i) * 2;
                        if (colWidth < 255 * 256)
                        {
                            sheet.SetColumnWidth(i, colWidth < 3000 ? 3000 : colWidth);
                        }
                        else
                        {
                            sheet.SetColumnWidth(i, 6000);
                        }
                        DataColumn column = dtSource.Columns[i];
                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                        headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                        //设置列宽
                        //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 255);

                    }
                    // headerRow.Dispose();
                }
                #endregion

                rowIndex = 2;
            }
            #endregion

            HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);

            #region 填充内容
            foreach (DataColumn column in dtSource.Columns)
            {

                HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);

                string drValue = row[column].ToString();

                switch (column.DataType.ToString())
                {
                    case "System.String"://字符串类型
                        newCell.SetCellValue(drValue);
                        break;
                    case "System.DateTime"://日期类型
                        if ("".Equals(drValue))
                        {
                            newCell.SetCellValue(drValue);
                            break;
                        }
                        DateTime date = Convert.ToDateTime(drValue);
                        drValue = date.ToString("yyyy-MM-dd HH:mm:ss");
                        //DateTime dateV;
                        //DateTime.TryParse(drValue, out dateV);
                        newCell.SetCellValue(drValue);

                        //newCell.SetCellType(HSSFCellType.FORMULA);
                        //newCell.CellStyle = dateStyle;//格式化显示
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
            #endregion

            rowIndex++;
        }


        using (MemoryStream ms = new MemoryStream())
        {
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            //sheet.Dispose();
            //workbook.Dispose();

            return ms;
        }

    }

    //private static DataTable  InitReimbursementSlipCoverDetail()
    //{
        

    //    return dtVal;
    //}
    public static byte[] CreateVoucher(DataTable dt,string path, ref string msg)
    {
        msg = "";
        byte[] data = null;
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            HSSFWorkbook wbHssf = new HSSFWorkbook(fs);
            ISheet sheet = wbHssf.GetSheetAt(0);
            sheet.ForceFormulaRecalculation = true;//
            int index = 1;
            foreach(DataRow r in dt.Rows)
            {
                IRow row = sheet.CreateRow(index);
                foreach(DataColumn c in dt.Columns)
                {
                    if (string.IsNullOrEmpty(r[c.ColumnName].ToString()))
                        continue;
                    int cIndex = Convert.ToInt32(c.ColumnName)-1;
                    ICell cell = row.CreateCell(cIndex);
                    cell.SetCellValue(r[c.ColumnName].ToString());
                }
                index++;
            }



            //填完数据后，统一更新公式计算
            wbHssf.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
            using (MemoryStream ms = new MemoryStream())
            {
                wbHssf.Write(ms);
                ms.Flush();
                ms.Position = 0;
                data = ms.GetBuffer();
            }
            wbHssf = null;
            sheet = null;
        }
        return data;
    }

    public static byte[] EditSalesmanExcel(DataTable dt, ref string msg,string path, Dictionary<string, object> dictInfo)
    {
        msg = "";
        byte[] data = null;
        DateTime dateStart = (DateTime)dictInfo["dateStart"];
        DateTime dateEnd = (DateTime)dictInfo["dateEnd"];
        UserInfo user = (UserInfo)dictInfo["user"];
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            HSSFWorkbook wbHssf = new HSSFWorkbook(fs);
            ISheet sheet = wbHssf.GetSheetAt(1);
            sheet.ForceFormulaRecalculation = true;//
            //表头信息
            IRow row = sheet.GetRow(4);
            ICell cell = row.GetCell(0);
            cell.SetCellValue(user.userName);
            //row = sheet.GetRow(5);
            cell = row.GetCell(4);
            cell.SetCellValue(dictInfo["depart"].ToString());
            cell = row.GetCell(12);
            cell.SetCellValue(string.Format("从{0}到{1}", dateStart.ToString("yyyy年MM月dd日"), dateEnd.AddDays(-1).ToString("yyyy年MM月dd日")));
            cell = row.GetCell(20);
            cell.SetCellValue(user.mobilePhone);
            cell = row.GetCell(25);
            cell.SetCellValue(dictInfo["depart"].ToString());

            //当月岗位补贴
            if (dictInfo.ContainsKey("餐费"))
            {
                row = sheet.GetRow(6);
                cell = row.GetCell(2);
                cell.SetCellValue(dateStart.Month + "月");
                row = sheet.GetRow(13);
                cell = row.GetCell(2);
                cell.SetCellValue((double)dictInfo["餐费"]);
                row = sheet.GetRow(14);
                cell = row.GetCell(2);
                cell.SetCellValue((double)dictInfo["市内交通费"]);
            }

            //核销预付款
            if (dictInfo.ContainsKey("核销预付款"))
            {
                row = sheet.GetRow(39);
                cell = row.GetCell(32);              
                cell.SetCellValue((double)dictInfo["核销预付款"]);
            }
            else
            {
                if (dictInfo.ContainsKey("核销预付款2"))
                {
                    row = sheet.GetRow(39);
                    cell = row.GetCell(32);
                    cell.SetCellValue((double)dictInfo["核销预付款2"]);
                }
            }

            //税金
            if (dictInfo.ContainsKey("税金"))
            {
                row = sheet.GetRow(38);
                cell = row.GetCell(26);
                cell.SetCellValue((double)dictInfo["税金"]);
            }
            else
            {
                if (dictInfo.ContainsKey("税金2"))
                {
                    row = sheet.GetRow(38);
                    cell = row.GetCell(26);
                    cell.SetCellValue((double)dictInfo["税金2"]);
                }
            }
            if(dt.Rows.Count>0)
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();
                dict.Add("出差车船费", 9);
                dict.Add("住宿费", 10);
                dict.Add("出差补贴", 11);
                dict.Add("餐费", 13);
                dict.Add("市内交通费", 14);
                dict.Add("会议费", 15);
                dict.Add("培训费", 16);
                dict.Add("办公用品", 17);
                dict.Add("工作餐", 18);
                dict.Add("场地费", 25);
                dict.Add("招待餐费", 26);
                dict.Add("纪念品", 27);
                dict.Add("外协劳务", 28);
                dict.Add("外部人员机票/火车票", 29);
                dict.Add("外部人员住宿费", 30);
                dict.Add("外部人员交通费", 31);
                dict.Add("学术会费", 32);
                dict.Add("营销办公费", 33);

                int count = 0;
                foreach (DataRow rowVal in dt.Rows)
                {
                    int CellIndex = 4 + count * 2;
                    string val = rowVal["date"].ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        row = sheet.GetRow(6);
                        cell = row.GetCell(CellIndex);
                        cell.SetCellValue(val);
                    }

                    val = rowVal["地点"].ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        row = sheet.GetRow(7);
                        cell = row.GetCell(CellIndex);
                        cell.SetCellValue(val);
                    }

                    val = rowVal["出差内容描述"].ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        row = sheet.GetRow(8);
                        cell = row.GetCell(CellIndex);
                        cell.SetCellValue(val);
                    }

                    val = rowVal["活动申请编号"].ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        row = sheet.GetRow(24);
                        cell = row.GetCell(CellIndex);
                        cell.SetCellValue(val);
                    }

                    val = rowVal["活动内容描述"].ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        row = sheet.GetRow(23);
                        cell = row.GetCell(CellIndex);
                        cell.SetCellValue(val);
                    }

                    foreach (DataColumn c in dt.Columns)
                    {
                        if (c.DataType == typeof(double))
                        {
                            double tempVal = Convert.ToDouble(rowVal[c.ColumnName]);
                            if (tempVal != 0)
                            {
                                row = sheet.GetRow(dict[c.ColumnName]);
                                cell = row.GetCell(CellIndex);
                                cell.SetCellValue(tempVal);
                            }
                        }
                    }
                    count++;
                }
            }    

            

            //填完数据后，统一更新公式计算
            wbHssf.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
            using (MemoryStream ms = new MemoryStream())
            {
                wbHssf.Write(ms);
                ms.Flush();
                ms.Position = 0;
                data = ms.GetBuffer();
            }
            wbHssf = null;
            sheet = null;
        }
            
        return data;
    }

    public static byte[] EditNoSalesmanExcel(DataTable dt, ref string msg, string path, Dictionary<string, object> dictInfo)
    {
        msg = "";
        byte[] data = null;
        DateTime dateStart = (DateTime)dictInfo["dateStart"];
        DateTime dateEnd = (DateTime)dictInfo["dateEnd"];
        UserInfo user = (UserInfo)dictInfo["user"];
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            HSSFWorkbook wbHssf = new HSSFWorkbook(fs);
            ISheet sheet = wbHssf.GetSheetAt(0);
            sheet.ForceFormulaRecalculation = true;//
            //表头信息
            IRow row = sheet.GetRow(4);
            ICell cell = row.GetCell(0);
            cell.SetCellValue(user.userName);
            //row = sheet.GetRow(5);
            cell = row.GetCell(4);
            cell.SetCellValue(dictInfo["depart"].ToString());
            cell = row.GetCell(12);
            cell.SetCellValue(string.Format("从{0}到{1}", dateStart.ToString("yyyy年MM月dd日"), dateEnd.AddDays(-1).ToString("yyyy年MM月dd日")));
            cell = row.GetCell(20);
            cell.SetCellValue(user.mobilePhone);
            cell = row.GetCell(25);
            cell.SetCellValue(dictInfo["depart"].ToString());

            //当月岗位补贴
            if (dictInfo.ContainsKey("交通费"))
            {
                row = sheet.GetRow(6);
                cell = row.GetCell(2);
                cell.SetCellValue(dateStart.Month + "月");              
                row = sheet.GetRow(14);
                cell = row.GetCell(2);
                cell.SetCellValue((double)dictInfo["交通费"]);
            }
            if (dictInfo.ContainsKey("业务招待费"))
            {
                row = sheet.GetRow(16);
                cell = row.GetCell(2);
                cell.SetCellValue((double)dictInfo["业务招待费"]);
            }

            //核销预付款
            if (dictInfo.ContainsKey("核销预付款"))
            {
                row = sheet.GetRow(36);
                cell = row.GetCell(32);
                cell.SetCellValue((double)dictInfo["核销预付款"]);
            }
            else
            {
                if (dictInfo.ContainsKey("核销预付款2"))
                {
                    row = sheet.GetRow(36);
                    cell = row.GetCell(32);
                    cell.SetCellValue((double)dictInfo["核销预付款2"]);
                }
            }

            //税金
            if (dictInfo.ContainsKey("税金"))
            {
                row = sheet.GetRow(35);
                cell = row.GetCell(26);
                cell.SetCellValue((double)dictInfo["税金"]);
            }
            else
            {
                if (dictInfo.ContainsKey("税金2"))
                {
                    row = sheet.GetRow(35);
                    cell = row.GetCell(26);
                    cell.SetCellValue((double)dictInfo["税金2"]);
                }
            }
            int count = 6;
            Dictionary<string, int> dict = new Dictionary<string, int>();        
            dict.Add("date", count++);
            dict.Add("地点", count++);
            dict.Add("出差项目编号", count++);            
            dict.Add("出差车船费", count++);
            dict.Add("住宿费", count++);
            dict.Add("出差补贴", count++);
            count++;
            dict.Add("项目编号", count++);            
            dict.Add("交通费", count++);
            dict.Add("汽车使用费", count++);
            dict.Add("业务招待费", count++);
            dict.Add("培训费", count++);
            dict.Add("办公费", count++);
            dict.Add("租赁费", count++);
            dict.Add("劳保费", count++);
            dict.Add("通讯费", count++);
            dict.Add("福利费", count++);
            dict.Add("物业费", count++);
            dict.Add("水电费", count++);
            dict.Add("招聘费", count++);
            dict.Add("运输费", count++);
            dict.Add("材料费", count++);
            dict.Add("试验费", count++);
            dict.Add("检测费", count++);
            dict.Add("专利费", count++);
            dict.Add("注册费", count++);
            dict.Add("服务费", count++);
            dict.Add("其他", count++);

            count = 0;
            foreach (DataRow rowVal in dt.Rows)
            {
                int CellIndex = 4 + count * 2;
                //row = sheet.GetRow(6);
                //cell = row.GetCell(CellIndex);
                //cell.SetCellValue(rowVal["date"].ToString());

                //row = sheet.GetRow(7);
                //cell = row.GetCell(CellIndex);
                //cell.SetCellValue(rowVal["地点"].ToString());

                //row = sheet.GetRow(8);
                //cell = row.GetCell(CellIndex);
                //cell.SetCellValue(rowVal["出差内容描述"].ToString());

                //row = sheet.GetRow(24);
                //cell = row.GetCell(CellIndex);
                //cell.SetCellValue(rowVal["活动申请编号"].ToString());

                //row = sheet.GetRow(23);
                //cell = row.GetCell(CellIndex);
                //cell.SetCellValue(rowVal["活动内容描述"].ToString());

                foreach (DataColumn c in dt.Columns)
                {
                    if (c.DataType == typeof(double))
                    {
                        row = sheet.GetRow(dict[c.ColumnName]);
                        cell = row.GetCell(CellIndex);
                        cell.SetCellValue(Convert.ToDouble(rowVal[c.ColumnName]));
                    }
                    else
                    {
                        row = sheet.GetRow(dict[c.ColumnName]);
                        cell = row.GetCell(CellIndex);
                        cell.SetCellValue(rowVal[c.ColumnName].ToString());
                    }
                }
                count++;
            }

            //填完数据后，统一更新公式计算
            wbHssf.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
            using (MemoryStream ms = new MemoryStream())
            {
                wbHssf.Write(ms);
                ms.Flush();
                ms.Position = 0;
                data = ms.GetBuffer();
            }
            wbHssf = null;
            sheet = null;
        }

        return data;
    }
}