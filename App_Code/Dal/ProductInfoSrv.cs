using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;

/// <summary>
/// ProductInfoSrv 的摘要说明
/// </summary>
public class ProductInfoSrv
{
    public ProductInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetInfos( string companyId)
    {
        string sql = string.Format("select * from products where companyId={0} order by code", companyId);
        return SqlHelper.Find(sql);
    }

    public static DataSet GetAliasData(string code)
    {
        string sql = string.Format("select * from product_alias  "
            + " where productCode='{0}' ", code);
        return SqlHelper.Find(sql);
    }

    public static string GetAliasId(string code,string type)
    {
        string sql = string.Format("select Id from product_alias  "
            + " where productCode='{0}' and type='{1}'", code,type);
        object id = SqlHelper.Scalar(sql);
        string res = "";
        if (id != null)
            res = id.ToString();
        return res;
    }

    public static DataSet GetAliasData()
    {
        string sql = string.Format("select a.*,p.name as name, p.specification as specification "
            +"from product_alias a inner join products p "
            + "on a.productCode=p.code  ");
        return SqlHelper.Find(sql);
    }

    public static string SaveAliasData(Dictionary<string, string> dict,string pCode, string type)
    {
        string sql = SqlHelper.GetSaveString(dict, "product_alias", 
            string.Format(" where productCode='{0}'  and type='{1}'", pCode,type));
        return SqlHelper.Exce(sql);
    }

    public static string InsertAliasData(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertIgnoreString(dict, "product_alias");
        return SqlHelper.Exce(sql);
    }

    public static string InsertAliasData(DataTable dt)
    {
        string sql = "";
        foreach (DataRow row in dt.Rows)
        {
            string id = GetAliasId(row["productCode"].ToString(), row["type"].ToString());
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataColumn clm in dt.Columns)
            {
                dict.Add(clm.ColumnName, row[clm.ColumnName].ToString());
            }
            if(string.IsNullOrEmpty(id))
            {
                sql += SqlHelper.GetInsertIgnoreString(dict, "product_alias");
            }
            else
            {
                sql += SqlHelper.GetUpdateString(dict, "product_alias", string.Format("where Id = {0}", id));
            }
        }
        return SqlHelper.Exce(sql);
    }

    public static string UpdateAliasData(Dictionary<string, string> dict, string id)
    {
        string sql = SqlHelper.GetUpdateString(dict, "product_alias",string.Format("where Id = {0}",id));
        return SqlHelper.Exce(sql);
    }

    public static string DeleteAliasData( string id)
    {
        string sql = string.Format("delete from product_alias where Id = {0}", id);
        if (!StringTools.IsInt(id))
        {
            id = id.Substring(0, id.Length - 1);
            sql = string.Format("delete from product_alias where Id in ({0})", id);
        }
        
        return SqlHelper.Exce(sql);
    }

    public static object ValidateProductCode(string companyId, string code)
    {
        string sql = string.Format("select Id from products where companyId={0} and code='{1}'", companyId, code);
        return SqlHelper.Scalar(sql);
    }

    public static string InsertInfos(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertIgnoreString(dict, "products");
        return SqlHelper.Exce(sql);
    }

    public static string SaveInfos(DataTable dt)
    {
        List<string> sqlList = new List<string>();
        foreach(DataRow row in dt.Rows)
        {
            string sql = SqlHelper.GetUpdateString(row, "products"
                , " where code='" + row["code"].ToString() + "'");
            sqlList.Add(sql);
            sql = SqlHelper.GetInsertIgnoreString(row, "products");
            sqlList.Add(sql);
        }

        return SqlHelper.Exce(sqlList.ToArray());
    }

    //public static string SaveInfos(DataTable dt)
    //{
    //    List<cProcedure> list = new List<cProcedure>();

    //    foreach (DataRow row in dt.Rows)
    //    {
    //        List<MySqlParameter> listParam = new List<MySqlParameter>();
    //        listParam.Add(table);
    //        listParam.Add(field);

    //        MySqlParameter value = new MySqlParameter("_value", MySqlDbType.VarChar);
    //        value.Value = row["code"].ToString();
    //        value.Direction = ParameterDirection.Input;
    //        listParam.Add(value);

    //        MySqlParameter insert = new MySqlParameter("inserCmd", MySqlDbType.VarChar);
    //        insert.Value = SqlHelper.GetInsertString(row, "products");
    //        insert.Direction = ParameterDirection.Input;
    //        listParam.Add(insert);

    //        MySqlParameter update = new MySqlParameter("updateCmd", MySqlDbType.VarChar);
    //        update.Value = SqlHelper.GetUpdateString(row, "products",string.Format(" where code='{0}'", row["code"].ToString()));
    //        update.Direction = ParameterDirection.Input;
    //        listParam.Add(update);

    //        list.Add(new cProcedure("SaveData", listParam));
    //    }        
    //    int [] res = SqlHelper.RunProcedure(list.ToArray());
    //    return "";
    //}

    public static string UpdateInfos(Dictionary<string, string> dict, string id)
    {
        string sql = SqlHelper.GetUpdateString(dict, "products", "where Id=" + id);
        return SqlHelper.Exce(sql);
    }

    public static string Delete(string id)
    {
        string sql = string.Format("DELETE FROM products WHERE Id = {0}", id);
        return SqlHelper.Exce(sql);
    }
}