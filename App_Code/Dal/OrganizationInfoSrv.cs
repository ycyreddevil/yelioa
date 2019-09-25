using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// OrganizationInfoSrv 的摘要说明
/// </summary>
public class OrganizationInfoSrv
{
    public OrganizationInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetData()
    {
        string sql = string.Format("select * from organization order by code");
        return SqlHelper.Find(sql);
    }

    public static DataSet GetAliasData(string code)
    {
        string sql = string.Format("select * from product_alias where organization_alias='{0}' ", code);
        return SqlHelper.Find(sql);
    }

    public static string SaveAliasData(Dictionary<string, string> dict, string code, string type)
    {
        string sql = SqlHelper.GetSaveString(dict, "organization_alias",
            string.Format(" where code='{0}' and type='{1}'", code,type));
        return SqlHelper.Exce(sql);
    }

    public static string GetAliasId(string code, string type)
    {
        string sql = string.Format("select Id from organization_alias  "
            + " where code='{0}' and type='{1}'", code, type);
        object id = SqlHelper.Scalar(sql);
        string res = "";
        if (id != null)
            res = id.ToString();
        return res;
    }

    public static DataSet GetAliasData()
    {
        string sql = string.Format("select a.*,o.name as name, o.fullName as fullName "
            + "from organization_alias a inner join organization o "
            + "on a.code=o.code  ");
        return SqlHelper.Find(sql);
    }

    public static string InsertAliasData(DataTable dt)
    {
        string sql = "";
        foreach (DataRow row in dt.Rows)
        {
            string id = GetAliasId(row["code"].ToString(), row["type"].ToString());
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataColumn clm in dt.Columns)
            {
                dict.Add(clm.ColumnName, row[clm.ColumnName].ToString());
            }
            if (string.IsNullOrEmpty(id))
            {
                sql += SqlHelper.GetInsertIgnoreString(dict, "organization_alias");
            }
            else
            {
                sql += SqlHelper.GetUpdateString(dict, "organization_alias", string.Format("where Id = {0}", id));
            }
        }
        return SqlHelper.Exce(sql);
    }

    public static string InsertAliasData(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertIgnoreString(dict, "organization_alias");
        return SqlHelper.Exce(sql);
    }

    public static string UpdateAliasData(Dictionary<string, string> dict, string id)
    {
        string sql = SqlHelper.GetUpdateString(dict, "organization_alias", string.Format("where Id = {0}", id));
        return SqlHelper.Exce(sql);
    }

    public static string DeleteAliasData(string id)
    {
        string sql = string.Format("delete from organization_alias where Id = {0}", id);
        if (!StringTools.IsInt(id))
        {
            id = id.Substring(0, id.Length - 1);
            sql = string.Format("delete from organization_alias where Id in ({0})", id);
        }
        return SqlHelper.Exce(sql);
    }

    public static string InsertInfo(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertIgnoreString(dict, "organization");
        return SqlHelper.Exce(sql);
    }

    public static string UpdateInfo(Dictionary<string, string> dict, string id)
    {
        string sql = SqlHelper.GetUpdateString(dict, "organization",
            string.Format(" where Id={0}",id));
        return SqlHelper.Exce(sql);
    }

    public static string DeleteInfo(string id)
    {
        string sql = string.Format("delete from organization where Id={0}", id);
        return SqlHelper.Exce(sql);
    }

    public static string checkCode(string code)
    {
        string sql = string.Format("select code from organization where code='{0}'",  code);
        return SqlHelper.Scalar(sql).ToString();
    }

    public static string SaveInfos(DataTable dt)
    {
        List<string> sqlList = new List<string>();
        foreach (DataRow row in dt.Rows)
        {
            string sql = SqlHelper.GetUpdateString(row, "organization"
                , " where code='" + row["code"].ToString() + "'");
            sqlList.Add(sql);
            sql = SqlHelper.GetInsertIgnoreString(row, "organization");
            sqlList.Add(sql);
        }

        return SqlHelper.Exce(sqlList.ToArray());
    }
}