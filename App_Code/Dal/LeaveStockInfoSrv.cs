using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

/// <summary>
/// LeaveStockInfoSrv 的摘要说明
/// </summary>
public class LeaveStockInfoSrv
{
    public LeaveStockInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetInfos(DateTime start, DateTime end, string type)
    {
        string sql = "select * from leave_stock where 1=1 ";
        if (!"".Equals(type))
        {
            sql += " and type = '{2}' ";
        }
        sql += " and (date between '{0}' and '{1}') order by date";
        sql = string.Format(sql, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"), type);
            
        return SqlHelper.Find(sql);
    }

    public static string ImportInfos(Dictionary<string, string> dict,string type)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        List<string> listSql = new List<string>();

        string sql = "";
        if(dict.Keys.Contains("productCode") && !string.IsNullOrEmpty(dict["productCode"]))
        {
            sql = string.Format("select  p.Id,p.code,p.name  from products p inner join product_alias a"
                + " on a.productCode=p.code where a.aliasCode = '{0}' and a.type = '{1}' union all "
                + " select Id,code,name from products where code = '{2}'"
                , dict["productCode"], type, dict["productCode"]);
        }
        else if(!string.IsNullOrEmpty(dict["productName"]))
        {
            sql = string.Format("select  p.Id,p.code,p.name  from products p inner join product_alias a"
                + " on a.productCode=p.code where a.alias = '{0}' and a.type = '{1}' union all "
                + " select Id,code,name from products where name = '{2}'"
                , dict["productName"], type, dict["productName"]);
            if (dict.Keys.Contains("specification") && !string.IsNullOrEmpty(dict["specification"]))
                sql += string.Format(" and specification = '{0}'", dict["specification"]);
        }
        else
        {
            return "无产品信息";
        }
        listSql.Add(sql);

        if (dict.Keys.Contains("terminalClient") && !string.IsNullOrEmpty(dict["terminalClient"]))
        {
            sql = string.Format("select  p.Id,p.code,p.name  from organization p inner join organization_alias a"
                + " on a.code=p.code where a.alias = '{0}' and a.type = '{1}' union all "
                + " select Id,code, name from organization where name = '{2}'"
                , dict["terminalClient"], type, dict["terminalClient"]);
        }
        else
        {
            return "无终端客户信息";
        }
        listSql.Add(sql);

        if (dict.Keys.Contains("accountUnit") && !string.IsNullOrEmpty(dict["accountUnit"]))
        {
            sql = string.Format("select  p.Id,p.code,p.name  from organization p inner join organization_alias a"
                + " on a.code=p.code where a.alias = '{0}' and a.type = '{1}' union all "
                + " select Id,code, name from organization where name = '{2}'"
                , dict["accountUnit"], type, dict["accountUnit"]);
        }
        else
        {
            return "无结算单位信息";
        }
        listSql.Add(sql);

        DataSet ds = SqlHelper.Find(listSql.ToArray());
        if(ds==null)
            return "数据库信息查询错误";
        else if(ds.Tables[0].Rows.Count == 0)
            return "未找到产品信息";
        else if (ds.Tables[1].Rows.Count == 0)
            return "未找到终端客户信息";
        else if (ds.Tables[2].Rows.Count == 0)
            return "未找到结算单位信息";

        dict = Common.ChangeDictionaryValue(dict, "ProductId", ds.Tables[0].Rows[0]["Id"].ToString());
        dict = Common.ChangeDictionaryValue(dict, "productCode", ds.Tables[0].Rows[0]["code"].ToString());
        dict = Common.ChangeDictionaryValue(dict, "productName", ds.Tables[0].Rows[0]["name"].ToString());
        dict = Common.ChangeDictionaryValue(dict, "terminalClientCode", ds.Tables[1].Rows[0]["code"].ToString());
        dict = Common.ChangeDictionaryValue(dict, "terminalClient", ds.Tables[1].Rows[0]["name"].ToString());
        dict = Common.ChangeDictionaryValue(dict, "terminalClientId", ds.Tables[1].Rows[0]["Id"].ToString());
        dict = Common.ChangeDictionaryValue(dict, "accountUnitCode", ds.Tables[2].Rows[0]["code"].ToString());
        dict = Common.ChangeDictionaryValue(dict, "accountUnit", ds.Tables[2].Rows[0]["name"].ToString());
        dict = Common.ChangeDictionaryValue(dict, "accountUnitId", ds.Tables[2].Rows[0]["Id"].ToString());

        //listSql.Clear();
        if (!dict.Keys.Contains("documentNumber") || string.IsNullOrEmpty(dict["documentNumber"]))
        {
            sql = SqlHelper.GetInsertString(dict, "leave_stock");
        }
        else
        {
            sql = SqlHelper.GetUpdateString(dict, "leave_stock"
                , string.Format(" where documentNumber='{0} and productCode='{1}' and"
                +" terminalClientCode='{2}'", dict["documentNumber"], dict["productCode"], dict["terminalClientCode"]));
            sql += SqlHelper.GetInsertIgnoreString(dict, "leave_stock");//避免重复插入数据
        }

            
        return SqlHelper.Exce(sql);
    }

    public static string InsertInfos(ArrayList list)
    {
        string sql = "";
        foreach (Dictionary<string, string> dict in list)
        {
            string cmd = SqlHelper.GetInsertIgnoreString(dict, "leave_stock");//避免重复插入数据
            sql += cmd;
        }

        return SqlHelper.Exce(sql);
    }

    public static DataSet GetTemplateInfo()
    {
        string sql = "select COLUMN_NAME as field,column_comment as name from INFORMATION_SCHEMA.Columns"
            + " where table_name='leave_stock' and table_schema='yelioa'";
        return SqlHelper.Find(sql);
    }

    public static DataSet GetDatalistInfo()
    {
        string sql = "select distinct type from leave_stock_template";
        return SqlHelper.Find(sql);
    }

    public static DataSet CheckInfo(string type)
    {
        string sql = string.Format("select distinct type from leave_stock_template where type='{0}'", type);
        return SqlHelper.Find(sql);
    }

    public static DataSet GetFormDetail(string type)
    {
        string sql = string.Format("select * from leave_stock_template where type='{0}'", type);
        return SqlHelper.Find(sql);
    }

    public static string InsertTemplate(ArrayList list)
    {
        string sql = "";
        foreach (Dictionary<string, string> dict in list)
        {
            string cmd = SqlHelper.GetInsertIgnoreString(dict, "leave_stock_template");//避免重复插入数据
            sql += cmd;
        }
        return SqlHelper.Exce(sql);
    }

    public static string UpdateTemplate(ArrayList list, string type)
    {
        string sql = string.Format("delete from leave_stock_template where type = '{0}'\r\n;", type);
        foreach (Dictionary<string, string> dict in list)
        {
            string cmd = SqlHelper.GetInsertIgnoreString(dict, "leave_stock_template");//避免重复插入数据
            sql += cmd;
        }
        return SqlHelper.Exce(sql);
    }

    public static string DeleteTemplate(string type)
    {
        string sql = string.Format("delete from leave_stock_template where type = '{0}'\r\n;", type);
        return SqlHelper.Exce(sql);
    }

    public static string DeleteData(string id)
    {
        string sql = string.Format("delete from leave_stock where Id = {0}", id);
        if (!StringTools.IsInt(id))
        {
            id = id.Substring(0, id.Length - 1);
            sql = string.Format("delete from leave_stock where Id in ({0})", id);
        }

        return SqlHelper.Exce(sql);
    }
}