using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;

/// <summary>
/// AccessHelper 的摘要说明
/// </summary>
public class AccessHelper
{
    public AccessHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    private static DataSet Find(string sql, string DbPath)
    {
        string connectionStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DbPath + ";Jet OLEDB:System Database=system.mdw;";
        OleDbConnection con = new OleDbConnection(connectionStr);
        con.Open();
        DataSet ds = new DataSet();
        string[] strs = sql.Split(';');
        OleDbCommand cmd = null;
        foreach (string strCmd in strs)
        {
            if (string.IsNullOrEmpty(strCmd))
            {
                continue;
            }
            DataTable dt = new DataTable();
            try
            {
                cmd = new OleDbCommand(strCmd, con);
                System.Data.OleDb.OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception e)
            {
                ds = null;
                //MessageBox.Show(e.ToString());
                //LogHelper.WriteLine(sql);
                //LogHelper.WriteLine(e.ToString());
                break;
            }
            ds.Tables.Add(dt);
        }
        cmd.Dispose();
        con.Close();
        return ds;
    }

    private static string Exce(string sql, string DbPath)
    {
        string connectionStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DbPath + ";Jet OLEDB:System Database=system.mdw;";
        OleDbConnection conn = new OleDbConnection(connectionStr);
        conn.Open();
        int cnt = 0;
        int cmdCount = 1;
        string ret = "";
        string[] strs = sql.Split(';');
        OleDbCommand cmd = null;
        foreach (string strCmd in strs)
        {
            if (string.IsNullOrEmpty(strCmd))
            {
                continue;
            }
            try
            {
                cmd = new OleDbCommand(strCmd, conn);
                //cmd.CommandTimeout = 20;
                cnt = cmd.ExecuteNonQuery();
                ret += string.Format("{0}:操作成功,{1}\r\n", cmdCount, cnt);
            }
            catch (Exception e)
            {
                ret += string.Format("{0}:{1},{2}\r\n", cmdCount, e.ToString(), cnt);
                //ret = ret.Replace("操作成功", "");
                //ret += e.ToString();
                //LogHelper.WriteLine(sql);
                //LogHelper.WriteLine(e.ToString());
            }
            cmdCount++;
        }
        cmd.Dispose();
        conn.Close();
        return ret;
    }

    private static string Scalar(string sql, string DbPath)
    {
        string connectionStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DbPath + ";Jet OLEDB:System Database=system.mdw;";
        OleDbConnection conn = new OleDbConnection(connectionStr);
        conn.Open();
        string ret = null;
        OleDbCommand cmd = new OleDbCommand(sql, conn);
        try
        {
            ret = cmd.ExecuteScalar().ToString();
        }
        catch (Exception e)
        {
            //LogHelper.WriteLine(sql);
            //LogHelper.WriteLine(e.ToString());
        }
        cmd.Dispose();
        conn.Close();
        return ret;
    }
}