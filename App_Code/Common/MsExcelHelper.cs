using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;

/// <summary>
/// MsExcelHelper 的摘要说明
/// </summary>
public class MsExcelHelper
{
    public MsExcelHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet ExcelToDS(string Path)
    {
        string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
        OleDbConnection conn = new OleDbConnection(strConn);
        conn.Open();
        string strExcel = "";
        OleDbDataAdapter myCommand = null;
        DataSet ds = null;
        strExcel = "select * from [sheet1$]";
        myCommand = new OleDbDataAdapter(strExcel, strConn);
        ds = new DataSet();
        myCommand.Fill(ds, "table1");
        return ds;
    }
}