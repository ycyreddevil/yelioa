using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.IO;
using System.Web.Security;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// SqlHelper 的摘要说明
/// </summary>
public class SqlHelper
{
    public SqlHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    //private static string connectString =
    //string.Format("server=yelioa.51vip.biz;Port=11268;user id=yelioa;password=yelioa123;database=yelioa;"
    //    + " pooling=true;Convert Zero Datetime=True");

    //private static string connectString =
    //string.Format("server=192.168.0.68;Port=3306;user id=yelioa;password=yelioa123;database=yelioa;"
    //    + " pooling=true;Convert Zero Datetime=True");

    private static string connectString = System.Configuration.ConfigurationManager.AppSettings["connectionstring"];

    private static string database = System.Configuration.ConfigurationManager.AppSettings["databaseName"];

    public static string ConnectString
    {
        get
        {
            return connectString;
        }

        set
        {
            connectString = value;
        }
    }

    public static DataSet Find(string sql)
    {
        string str = "";
        return Find(sql, ref str);
    }

    public static DataSet Find(string[] sql)
    {
        string str = "";
        return Find(sql, ref str);
    }

    public static DataSet Find(string[] sql, ref string msg)
    {
        DataSet ds = new DataSet();
        msg = "";
        using (MySqlConnection connection = new MySqlConnection(ConnectString))
        {
            try
            {
                connection.Open();
                foreach (string strCmd in sql)
                {
                    if (string.IsNullOrEmpty(strCmd))
                    {
                        continue;
                    }
                    DataTable dt = new DataTable();

                    MySqlDataAdapter adapter = new MySqlDataAdapter(strCmd, connection);
                    adapter.Fill(dt);


                    ds.Tables.Add(dt);
                }
            }
            catch (Exception ex)
            {
                ds = null;
                //throw new Exception(ex.Message);
                msg = ex.Message;
            }
            finally
            {
                connection.Close();
            }
        }
        return ds;
    }


    public static DataSet Find(string sql, ref string msg)
    {
        DataSet ds = new DataSet();
        msg = "";
        using (MySqlConnection connection = new MySqlConnection(ConnectString))
        {            
            string[] strs = sql.Split(';');
            try
            {
                connection.Open();
                foreach (string strCmd in strs)
                {
                    if (string.IsNullOrEmpty(strCmd))
                    {
                        continue;
                    }
                    DataTable dt = new DataTable();

                    MySqlDataAdapter adapter = new MySqlDataAdapter(strCmd, connection);
                    adapter.Fill(dt);


                    ds.Tables.Add(dt);
                }
            }
            catch (Exception ex)
            {
                ds = null;
                //throw new Exception(ex.Message);
                msg = ex.Message;
            }
            finally
            {
                connection.Close();
            }    
        }
        return ds;
    }

    public static string Exce(string sql)
    {
        string ret = "";
        using (MySqlConnection connection = new MySqlConnection(ConnectString))
        {
            int cnt = 0;
            int cmdCount = 1;
            
            string[] strs = sql.Split(';');
            connection.Open();
            foreach(string strCmd in strs)
            {
                if (string.IsNullOrEmpty(strCmd))
                {
                    continue;
                }
                using (MySqlCommand cmd = new MySqlCommand(strCmd, connection))
                {
                    try
                    {
                        cnt = cmd.ExecuteNonQuery();
                        ret += string.Format("{0}:{1},操作成功;\r\n", cmdCount, cnt);
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        ret += string.Format("{0}:{1},{2};\r\n", cmdCount,  cnt, e.ToString());
                    }
                }
                cmdCount++;
            }
            connection.Close();
        }
        return ret;
    }

    public static string Exce(string[] sql)
    {
        string ret = "";
        using (MySqlConnection connection = new MySqlConnection(ConnectString))
        {
            int cnt = 0;
            int cmdCount = 1;
            
            connection.Open();
            foreach (string strCmd in sql)
            {
                if (string.IsNullOrEmpty(strCmd))
                {
                    continue;
                }
                using (MySqlCommand cmd = new MySqlCommand(strCmd, connection))
                {
                    try
                    {
                        cnt = cmd.ExecuteNonQuery();
                        ret += string.Format("{0}:{1},操作成功;\r\n", cmdCount, cnt);
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        ret += string.Format("{0}:{1},{2};\r\n", cmdCount, cnt, e.ToString());
                    }
                }
                cmdCount++;
            }
            connection.Close();
        }
        return ret;
    }

    public static string InsertAndGetLastId(string sql)
    {
        JObject res = new JObject();
        using (MySqlConnection connection = new MySqlConnection(ConnectString))
        {
            connection.Open();
            int cnt = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {                
                try
                {
                    cnt = cmd.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    //sql执行报错
                    res.Add("Success", "-1");
                    res.Add("message", e.ToString());
                    return res.ToString();
                }                
            }
            sql = "SELECT LAST_INSERT_ID()";
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (cnt > 0)//已插入成功
                {                    
                    object obj = null;
                    try
                    {
                        obj = cmd.ExecuteScalar();
                        res.Add("Success", "1");
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            res.Add("Id", "-1");
                        else
                            res.Add("Id", obj.ToString());
                    }
                    catch(Exception e)
                    {
                        res.Add("Success", "-2");
                        res.Add("message", e.ToString());
                        return res.ToString();
                    }
                }
                else//sql 无错误，但有重复，插入不成功
                {
                    res.Add("Success", "0");
                    res.Add("message", "插入数据失败，有重复索引!");
                }
            }                
            connection.Close();
        }
        return res.ToString();
    }

    public static object Scalar(string sql)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectString))
        {
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }

    public static string[] Scalar(string[] sqls)
    {
        List<string> list = new List<string>();
        using (MySqlConnection connection = new MySqlConnection(ConnectString))
        {
            connection.Open();            
            foreach (string sql in sqls)
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    try
                    {
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            list.Add("");
                        }
                        else
                        {
                            list.Add(obj.ToString());
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {                        
                        //throw e;
                        list.Clear();
                        break;
                    }
                }
            }
            connection.Close();
        }
        return list.ToArray();
    }


    public static int[] RunProcedure(cProcedure[] listProcedure)
    {
        List<int> listRes = new List<int>();
        foreach(cProcedure p in listProcedure)
        {
            int res = RunProcedure(p.StoredProcName, p.ListParam);
            listRes.Add(res);
        }
        return listRes.ToArray();
    }
    /// <summary>
    /// 执行存储过程，返回影响的行数        
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="listParam">参数</param>
    /// <returns>影响的行数</returns>
    public static int RunProcedure(string storedProcName, List<MySqlParameter> listParam)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectString))
        {
            int result;
            
            MySqlCommand command = new MySqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach(MySqlParameter param in listParam)
            {
                command.Parameters.Add(param);
            }
            //command.Parameters.Add("_table",table);
            //command.Parameters["_table"].Direction = ParameterDirection.Input;
            //command.Parameters.Add("_field", field);
            //command.Parameters["_field"].Direction = ParameterDirection.Input;
            connection.Open();
            
            result = command.ExecuteNonQuery();
            //result = (int)command.Parameters["ReturnValue"].Value;
            connection.Close();
            return result;
        }
    }

    private static string VerifyString(string str)
    {
        if(!string.IsNullOrEmpty(str))
            str = str.Replace(";", "；");
        return str;
    }

    public static string GetSaveString(Dictionary<string, string> dict, string tableName, string condition)
    {
        string sql = string.Format("select * from {0} {1}", tableName, condition);
        DataSet ds = Find(sql);
        if(ds == null || ds.Tables[0].Rows.Count == 0)
            sql = GetInsertIgnoreString(dict, tableName);
        else
            sql = GetUpdateString(dict, tableName, condition);        
        return sql;
    }

    public static string GetInsertIgnoreString(DataRow row, string table)
    {
        string res = GetInsertString(row, table);
        res = res.Replace("Insert", "Insert ignore ");//避免重复插入数据
        return res;
    }

    public static string GetInsertIgnoreString(Dictionary<string, string> dict, string table)
    {
        string res = GetInsertString(dict, table);
        res = res.Replace("Insert", "Insert ignore ");//避免重复插入数据
        return res;
    }

    public static string GetInsertIgnoreString(DataTable dt, string table)
    {
        string res = GetInsertString(dt, table);
        res = res.Replace("Insert", "Insert ignore ");//避免重复插入数据
        return res;
    }

    public static string GetSaveString(DataTable dt, string table,string IdColumn)
    {
        if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0)
        {
            return "";
        }
        string sql = "";

        foreach (DataRow row in dt.Rows)
        {
            string insert = GetInsertString(row, table);
            string update = GetUpdateString(row, table, string.Format(" where {0}='{1}'", IdColumn, row[IdColumn].ToString()));
            sql += string.Format("if not exists (select {0} from {1} where {2}='{3}')"
                , IdColumn, table, IdColumn, row[IdColumn].ToString());
            sql += " begin " + insert + " end ";
            sql += " else begin " + update + " end\r\n;";            
        }
        return sql;
    }

    public static string GetInsertString(DataRow row, string table)
    {
        if(row == null || row.ItemArray.Length==0)
        {
            return "";
        }
        string fileds = "";
        string values = "";
        foreach (DataColumn clm in row.Table.Columns)
        {
            fileds += string.Format("{0},", VerifyString(clm.ColumnName));
            values += string.Format("'{0}',", VerifyString(row[clm.ColumnName].ToString()));
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        values = values.Substring(0, values.Length - 1);
        string sql = string.Format("Insert into {0} ({1}) values ({2})", table, fileds, values);
        return sql;
    }

    public static string GetInsertString(ArrayList list, string table)
    {
        if (list == null || list.Count == 0)
        {
            return "";
        }

        string sql = "";
        int len = list.Count;

        string fileds = "";
        string values = "";
        Dictionary<string, string> dict = (Dictionary<string, string>)list[0];
        if (dict.Keys.Count == 0)
            return "";
        foreach (string key in dict.Keys)
        {
            fileds += string.Format("{0},", VerifyString(key));
            values += string.Format("'{0}',", VerifyString(dict[key]));
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        values = values.Substring(0, values.Length - 1);
        sql += string.Format("Insert into {0} ({1}) values ({2})", table, fileds, values);

        for (int i = 1; i < len; i++)
        {
            dict = (Dictionary<string, string>)list[i];
            values = "";
            foreach (string key in dict.Keys)
            {
                values += string.Format("'{0}',", VerifyString(dict[key]));
            }
            values = values.Substring(0, values.Length - 1);
            sql += string.Format(", ({0})", values);
        }
        sql += "\r\n;";
        return sql;
    }

    public static string GetInsertString(List<JObject> list, string table)
    {
        if (list == null || list.Count == 0)
        {
            return "";
        }

        string sql = "";
        int len = list.Count;

        string fileds = "";
        string values = "";
        JObject dict = (JObject)list[0];
        foreach (var jp in dict)
        {
            fileds += string.Format("{0},", VerifyString(jp.Key));
            values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        values = values.Substring(0, values.Length - 1);
        sql += string.Format("Insert into {0} ({1}) values ({2})", table, fileds, values);

        for (int i = 1; i < len; i++)
        {
            dict = (JObject)list[i];
            values = "";
            foreach (var jp in dict)
            {
                values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
            }
            values = values.Substring(0, values.Length - 1);
            sql += string.Format(", ({0})", values);
        }
        sql += "\r\n;";
        return sql;
    }

    public static string GetInsertString(Dictionary<string, string> dict, string table)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string fileds = "";
        string values = "";
        foreach (string key in dict.Keys)
        {
            fileds += string.Format("{0},", VerifyString(key));
            //if(key == "Id")
            //    values += string.Format("{0},", VerifyString(dict[key]));
            //else
                values += string.Format("'{0}',", VerifyString(dict[key]));
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        values = values.Substring(0, values.Length - 1);
        string sql = string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
        return sql;
    }

    public static string GetInsertString(JObject dict, string table)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string fileds = "";
        string values = "";
        foreach (var jp in dict)
        {
            fileds += string.Format("{0},", VerifyString(jp.Key));
            values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        values = values.Substring(0, values.Length - 1);
        string sql = string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
        return sql;
    }

    public static string GetInsertStringAllColumns(JObject dict, string table)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string fileds = "";
        string values = "";
        foreach (var jp in dict)
        {
            fileds += string.Format("{0},", VerifyString(jp.Key));
            values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        values = values.Substring(0, values.Length - 1);
        string sql = string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
        return sql;
    }

    public static string GetInsertStringWithoutQuotes(JObject dict, string table, List<string> columns)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string fileds = "";
        string values = "";
        foreach (var jp in dict)
        {
            fileds += string.Format("{0},", VerifyString(jp.Key));
            if (!columns.Contains(jp.Key))
            {
                values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
            }
            else
            {
                values += string.Format("{0},", VerifyString(jp.Value.ToString()));
            }
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        values = values.Substring(0, values.Length - 1);
        string sql = string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
        return sql;
    }

    public static string GetInsertStringForImportCostSharing(List<JObject> list, string table,int max)
    {
//        if (dict.Count == 0)
//        {
//            return "";
//        }
//        string fileds = "";
//        string values = "";
//        foreach (var jp in dict)
//        {
//            fileds += string.Format("{0},", VerifyString(jp.Key));
//            if (jp.Key.ToString().Equals("DataJson") || jp.Key.ToString().Equals("CreateTime") || 
//                jp.Key.ToString().Equals("FeeDetail") || "".Equals(jp.Value.ToString()))
//            {
//                values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
//            }
//            else
//            {
//                values += string.Format("{0},", VerifyString(jp.Value.ToString()));
//            }
//        }
//        fileds = fileds.Substring(0, fileds.Length - 1);
//        values = values.Substring(0, values.Length - 1);
//        string sql = string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
//        return sql;

        if (list == null || list.Count == 0)
        {
            return "";
        }

        string sql = "";
        int len = list.Count;

        string fileds = "";
        string values = "";
        JObject dict = (JObject)list[0];
        foreach (var jp in dict)
        {
            fileds += string.Format("{0},", VerifyString(jp.Key));
            //if (jp.Key.ToString().Equals("DataJson") || jp.Key.ToString().Equals("CreateTime") ||
            //    jp.Key.ToString().Equals("FeeDetail") || "".Equals(jp.Value.ToString()))
            //{
            //    values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
            //}
            //else
            //{
            //    values += string.Format("{0},", VerifyString(jp.Value.ToString()));
            //}
            //values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        //values = values.Substring(0, values.Length - 1);


        for (int i = 0; i < len; i++)
        {
            if (i == 0 || (i != 1 && i % max == 1))
            {
                sql += string.Format("Insert into {0} ({1}) values ", table, fileds);
            }
            dict = (JObject)list[i];
            values = "";
            foreach (var jp in dict)
            {
                if (jp.Key.ToString().Equals("DataJson") || jp.Key.ToString().Equals("CreateTime") ||
                    jp.Key.ToString().Equals("FeeDetail") || "".Equals(jp.Value.ToString()))
                {
                    values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
                }
                else
                {
                    values += string.Format("{0},", VerifyString(jp.Value.ToString()));
                }
                //values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
            }
            values = values.Substring(0, values.Length - 1);
            sql += string.Format("({0})", values);
            if (i % max == 0 && i > 0)
            {
                sql += ";";
            }
            else
            {
                sql += ",";
            }
        }

        sql = sql.Substring(0, sql.Length - 1);
        return sql;

    }
    public static string GetInsertStringForDistribute(List<JObject> list, string table, int max)
    {
       

        if (list == null || list.Count == 0)
        {
            return "";
        }

        string sql = "";
        int len = list.Count;

        string fileds = "";
        string values = "";
        JObject dict = (JObject)list[0];
        foreach (var jp in dict)
        {
            fileds += string.Format("{0},", VerifyString(jp.Key));
            
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
      

        for (int i = 0; i < len; i++)
        {
            if (i == 0 || (i != 1 && i % max == 1))
            {
                sql += string.Format("Insert into {0} ({1}) values ", table, fileds);
            }
            dict = (JObject)list[i];
            values = "";
            foreach (var jp in dict)
            {
                if (jp.Key.ToString().Equals("HospitalRecordId"))
                {
                    values += string.Format("{0},", VerifyString(jp.Value.ToString()));
                }
                else
                {
                    values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
                }
                
            }
            values = values.Substring(0, values.Length - 1);
            sql += string.Format("({0})", values);
            if (i % max == 0 && i > 0)
            {
                sql += ";";
            }
            else
            {
                sql += ",";
            }
        }

        sql = sql.Substring(0, sql.Length - 1);
        return sql;

    }

    public static string GetInsertString(DataTable dt, string table)
    {
        if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0)
        {
            return "";
        }
        string sql = "";
        int len = dt.Rows.Count;

        string fileds = "";
        string values = "";
        DataRow row = dt.Rows[0];
        foreach (DataColumn clm in dt.Columns)
        {
            fileds += string.Format("{0},", VerifyString(clm.ColumnName));
            values += string.Format("'{0}',", VerifyString(row[clm.ColumnName].ToString()));
        }
        fileds = fileds.Substring(0, fileds.Length - 1);
        values = values.Substring(0, values.Length - 1);
        sql += string.Format("Insert into {0} ({1}) values ({2})", table, fileds, values);

        for (int i=1;i<len;i++)
        {
            row = dt.Rows[i];
            values = "";
            foreach (DataColumn clm in dt.Columns)
            {
                values += string.Format("'{0}',", VerifyString(row[clm.ColumnName].ToString()));
            }
            values = values.Substring(0, values.Length - 1);
            sql += string.Format(", ({0})", values);
        }
        sql += "\r\n;";
        //foreach (DataRow row in dt.Rows)
        //{
        //    string fileds = "";
        //    string values = "";
        //    foreach (DataColumn clm in dt.Columns)
        //    {
        //        fileds += string.Format("{0},", VerifyString(clm.ColumnName));
        //        values += string.Format("'{0}',", VerifyString(row[clm.ColumnName].ToString()));
        //    }
        //    if(dt.Columns.Count>0)
        //    {
        //        fileds = fileds.Substring(0, fileds.Length - 1);
        //        values = values.Substring(0, values.Length - 1);
        //        sql += string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
        //    }            
        //}       

        return sql;
    }

    public static string GetMassUpdateString(ArrayList list,string table, string condition)
    {
        //JObject jobj = new JObject();
        //jobj.Add("Index", 0);
        //jobj.Add("ErrMsg", "无数据");
        if (list.Count == 0)
        {
            return "";
        }
        string values = "";
        string fileds = "";
        foreach(string key in ((Dictionary<string, string>)list[0]).Keys)
        {
            fileds += key + ", ";
        }
        fileds = fileds.Substring(0, fileds.Length - 2);
        string sql = string.Format("replace into {0} ({1}) values ", table,fileds); 
        foreach(Dictionary<string, string> dict in list)
        {
            values = "(";
            foreach(string key in dict.Keys)
            {
                values += dict[key] + ", ";
            }
            values = values.Substring(0, fileds.Length - 2);
            values += "),";
            sql += values;
        }
        sql = sql.Substring(0, fileds.Length - 1);
        return sql;
    }

    public static string GetUpdateString(ArrayList list, string tableName, string condition)
    {
        if (list.Count == 0 )
        {
            return "";
        }
        string sql = "";
        foreach (Dictionary<string, string> dict in list)
        {
            sql += string.Format("Update {0} set ", tableName);
            foreach (string key in dict.Keys)
            {
                sql += string.Format("{0}='{1}', ", VerifyString(key)
                    , VerifyString(dict[key]));
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += string.Format(" {0}\r\n;", condition);
        }
        return sql;
    }

    public static string GetUpdateString(ArrayList list, string tableName, List<string> condition)
    {
        if (list.Count == 0)
        {
            return "";
        }
        string sql = "";
        int index = 0;
        foreach (Dictionary<string, string> dict in list)
        {
            sql += string.Format("Update {0} set ", tableName);
            foreach (string key in dict.Keys)
            {
                sql += string.Format("{0}='{1}', ", VerifyString(key)
                    , VerifyString(dict[key]));
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += string.Format(" {0}\r\n;", condition[index]);
            index++;
        }
        return sql;
    }
    public static string GetUpdateString(DataTable dt, string tableName, string condition)
    {
        if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0)
        {
            return "";
        }
        string sql = "";
        foreach (DataRow row in dt.Rows)
        {
            sql += string.Format("Update {0} set ", tableName);
            foreach (DataColumn clm in dt.Columns)
            {
                sql += string.Format("{0}='{1}', ", VerifyString(clm.ColumnName)
                    , VerifyString(row[clm.ColumnName].ToString()));
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += string.Format(" {0}\r\n;", condition);
        }        
        return sql;
    }

    public static string GetUpdateString(DataRow row, string tableName, string condition)
    {
        if (row == null || row.ItemArray.Length == 0)
        {
            return "";
        }
        string sql = string.Format("Update {0} set ", tableName);
        foreach (DataColumn clm in row.Table.Columns)
        {
            sql += string.Format("{0}='{1}', ", VerifyString(clm.ColumnName)
                    , VerifyString(row[clm.ColumnName].ToString()));
        }
        sql = sql.Substring(0, sql.Length - 2);
        sql += string.Format(" {0}", condition);
        return sql;
    }

    public static string GetUpdateString(Dictionary<string, string> dict, string tableName, string condition)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string sql = string.Format("Update {0} set ", tableName);
        foreach (string key in dict.Keys)
        {
            sql += string.Format("{0}='{1}', ", VerifyString(key), VerifyString(dict[key]));
        }
        sql = sql.Substring(0, sql.Length - 2);
        sql += string.Format(" {0}\r\n;", condition);
        return sql;
    }

    public static string GetUpdateString(JObject dict, string tableName, string condition)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string sql = string.Format("Update {0} set ", tableName);
        foreach (var jp in dict)
        {
            sql += string.Format("{0}='{1}', ", VerifyString(jp.Key.ToString()), VerifyString(jp.Value.ToString()));
        }
        sql = sql.Substring(0, sql.Length - 2);
        sql += string.Format(" {0}\r\n;", condition);
        return sql;
    }

    public static string GetUpdateStringForCostSharing(JObject dict, string tableName, string condition)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string sql = string.Format("Update {0} set ", tableName);
        foreach (var jp in dict)
        {
            if (jp.Key.ToString() != "LimitNumber")
            {
                sql += string.Format("{0}='{1}', ", VerifyString(jp.Key.ToString()), VerifyString(jp.Value.ToString()));
            }
            else
            {
                sql += string.Format("{0}={1}, ", VerifyString(jp.Key.ToString()), VerifyString(jp.Value.ToString()));
            }
        }
        sql = sql.Substring(0, sql.Length - 2);
        sql += string.Format(" {0}\r\n;", condition);
        return sql;
    }
    public static DataSet GetFiledNameAndComment(string[] tableNames)
    {
        string sql = "";
        foreach(string name in tableNames)
        {
            sql += string.Format("select COLUMN_NAME as field,column_comment as comment from INFORMATION_SCHEMA.Columns"
            + " where table_name='{0}' and table_schema='{1}'", name, database);
        }
        return SqlHelper.Find(sql);
    }

    public static DataSet GetFiledNameAndComment(string tableName)
    {
        string sql = string.Format("select COLUMN_NAME as field,column_comment as comment from INFORMATION_SCHEMA.Columns"
            + " where table_name='{0}' and table_schema='{1}'", tableName, database);
        return SqlHelper.Find(sql);
    }


    public static string ToMultiData(string[] datas)
    {
        string res = "";
        if (datas == null || datas.Length == 0)
            return res;
        foreach (string data in datas)
        {
            res += string.Format("({0}),", data);
        }
        res = res.Substring(0, res.Length - 1);
        return res;
    }

    public static string ToMultiData(object JsonObj)
    {
        JArray listSrc = (JArray)(JsonObj);
        string res = "";
        foreach (int val in listSrc)
        {
            res += string.Format("({0}),", val.ToString());
        }
        res = res.Substring(0, res.Length - 1);
        return res;
    }

    public static string[] FromMultiData(string data)
    {
        List<string> list = new List<string>();
        string[] datas = data.Split(',');
        foreach(string d in datas)
        {
            string res =d.Replace('(', (char)0);
            res = res.Replace(')', (char)0);
            list.Add(res);
        }
        return list.ToArray();
    }


    //<summary>
    /// MD5加密
    /// </summary>
    /// <param name="toCryString">被加密字符串</param>
    /// <returns>加密后的字符串</returns>
    public static string MD5(string toCryString)
    {
        string res = FormsAuthentication.HashPasswordForStoringInConfigFile(toCryString, "MD5");
        res = FormsAuthentication.HashPasswordForStoringInConfigFile(res, "SHA1");
        res = FormsAuthentication.HashPasswordForStoringInConfigFile(res, "MD5");
        return res;
    }

    /// <summary>
    /// 防止SQL注入方法
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    public static string Split(string inputString)
    {
        if(string.IsNullOrEmpty(inputString))
        {
            return inputString;
        }
        inputString = inputString.Trim();
        inputString = inputString.Replace("'", "");
        inputString = inputString.Replace(";--", "");
        inputString = inputString.Replace("--", "");
        inputString = inputString.Replace("=", "");
        //and|exec|insert|select|delete|update|chr|mid|master|or|truncate|char|declare|join|count|*|%|union 等待关键字过滤
        //不要忘记为你的用户名框，密码框设定 允许输入的最多字符长度 maxlength的值哦，这样他们就无法编写太长的东西来再次拼成第一次过滤掉的关键字 如 oorr一次replace过滤后又成了 or 喔。
        inputString = inputString.Replace("and", "");
        inputString = inputString.Replace("exec", "");
        inputString = inputString.Replace("insert", "");
        inputString = inputString.Replace("select", "");
        inputString = inputString.Replace("delete", "");
        inputString = inputString.Replace("update", "");
        inputString = inputString.Replace("chr", "");
        inputString = inputString.Replace("mid", "");
        inputString = inputString.Replace("master", "");
        inputString = inputString.Replace("or", "");
        inputString = inputString.Replace("truncate", "");
        inputString = inputString.Replace("char", "");
        inputString = inputString.Replace("declare", "");
        inputString = inputString.Replace("join", "");
        inputString = inputString.Replace("count", "");
        inputString = inputString.Replace("*", "");
        inputString = inputString.Replace("%", "");
        inputString = inputString.Replace("union", "");
        return inputString;
    }

    /// <summary>
    /// DES 加密 注意:密钥必须为８位
    /// </summary>
    /// <param name="inputString">待加密字符串</param>
    /// <param name="encryptKey">密钥</param>
    /// <returns>加密后的字符串</returns>
    private static string DesEncrypt(string inputString, string encryptKey, byte[] IV)
    {
        if (string.IsNullOrEmpty(inputString)) return null;
        byte[] byKey = null;
        //byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        byKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray = Encoding.UTF8.GetBytes(inputString);
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write))
            {
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    /// <summary>
    /// DES 解密 注意:密钥必须为８位
    /// </summary>
    /// <param name="inputString">待解密字符串</param>
    /// <param name="decryptKey">密钥</param>
    /// <returns>解密后的字符串</returns>
    private static string DesDecrypt(string inputString, string decryptKey, byte[] IV)
    {
        if (string.IsNullOrEmpty(inputString)) return null;
        byte[] byKey = null;
        //byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        byte[] inputByteArray = new Byte[inputString.Length];
        byKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        inputByteArray = Convert.FromBase64String(inputString);
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write))
            {
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

    }

    public static string DesEncrypt(string txt)
    {
        if (string.IsNullOrEmpty(txt)) return null;
        byte[] IV = { 0x12, 0x12, 0x56, 0x78, 0x12, 0xAB, 0x12, 0xEF };
        string res = DesEncrypt(txt, "j89zip90", IV);
        IV = new byte[] { 0xCD, 0x34, 0xCD, 0x78, 0xCD, 0xAB, 0xCD, 0xEF };
        res = DesEncrypt(res, "jaixnscd", IV);
        IV = new byte[] { 0xEF, 0x34, 0xEF, 0x78, 0xEF, 0xAB, 0xCD, 0xEF };
        res = DesEncrypt(res, "ghsi95xml", IV);
        return res;
    }

    public static string DesDecrypt(string txt)
    {
        if (string.IsNullOrEmpty(txt)) return null;
        byte[] IV = { 0xEF, 0x34, 0xEF, 0x78, 0xEF, 0xAB, 0xCD, 0xEF };
        string res = DesDecrypt(txt, "ghsi95xml", IV);
        IV = new byte[] { 0xCD, 0x34, 0xCD, 0x78, 0xCD, 0xAB, 0xCD, 0xEF };
        res = DesDecrypt(res, "jaixnscd", IV);
        IV = new byte[] { 0x12, 0x12, 0x56, 0x78, 0x12, 0xAB, 0x12, 0xEF };
        res = DesDecrypt(res, "j89zip90", IV);
        return res;
    }
}

public class SqlExceRes
{
    public int CmdIndex { get; set; }
    public int ModifiedLine { get; set; }
    public string ExceMsg { get; set; }

    //public string RepetitionMsg { get; set; }
    //public string SuccessMsg { get; set; }

    public enum ResState { Success,Repetition,Error}
    public ResState Result { get; set; }

    public SqlExceRes(string res)
    {
        if ("".Equals(res))
        {
            Result = ResState.Error;
        }
        else
        {
            string[] strs1 = res.Split(':');
            string[] strs2 = strs1[1].Split(',');
            CmdIndex = Convert.ToInt32(strs1[0]);
            ModifiedLine = Convert.ToInt32(strs2[0]);
            ExceMsg = strs2[1];
            if (ExceMsg.Contains("操作成功") && ModifiedLine > 0)
            {
                Result = ResState.Success;
            }
            else if (ExceMsg.Contains("操作成功") && ModifiedLine == 0)
            {
                Result = ResState.Repetition;
            }
            else
            {
                Result = ResState.Error;
            }
        }
    }
    
    public string GetResultString(string SuccessMsg, string RepetitionMsg)
    {
        string res = "";
        if (Result == SqlExceRes.ResState.Success)
        {
            res = SuccessMsg;
        }
        else if (Result == SqlExceRes.ResState.Repetition)
        {
            res = RepetitionMsg;
        }
        else
        {
            res = ExceMsg;
        }
        return res;
    }

    public string GetResultString(string SuccessMsg, string RepetitionMsg, string ErrorMsg)
    {
        string res = "";
        if (Result == SqlExceRes.ResState.Success)
        {
            res = SuccessMsg;
        }
        else if (Result == SqlExceRes.ResState.Repetition)
        {
            res = RepetitionMsg;
        }
        else
        {
            res = ErrorMsg + ExceMsg;
        }
        return res;
    }
}

public class cProcedure
{
    public string StoredProcName { get; set; }
    public List<MySqlParameter> ListParam { get; set; }

    public cProcedure(string name, List<MySqlParameter> list)
    {
        StoredProcName = name;
        ListParam = list;
    }
}