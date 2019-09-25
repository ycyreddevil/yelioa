using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// SalesTaskInfoSrv 的摘要说明
/// </summary>
public class SalesTaskInfoSrv
{
    public SalesTaskInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet getInfos()
    {
        string sql = string.Format("select * from sales_task");
        return SqlHelper.Find(sql);
    }

    public static string ImportInfos(Dictionary<string, string> dict)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        List<string> list = new List<string>();
        list.Add(ImportUpdateInfos(dict));
        list.Add(ImportInsertInfos(dict));
        return SqlHelper.Exce(list.ToArray());
    }

    public static string ImportUpdateInfos(Dictionary<string, string> dict)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string sql = string.Format("Update sales_task set ");
        foreach (string key in dict.Keys)
        {
            //不包含中文列名及excel缺省列名ColumnXXX
            if (!StringTools.HasChinese(key) && !key.Contains("Column"))
            {
                string value = "";
                if (key.Contains("Department") || key.Contains("Sector") ||
                    key.Contains("Supervisor") || key.Contains("Specification") ||
                    key.Contains("Manager") || key.Contains("Director") ||
                    key.Contains("Hospital") ||
                    key.Contains("Product") || key.Contains("Sales"))
                {
                    value = dict[key];
                }
                else
                {
                    double val = 0.0;
                    try
                    {
                        val = Convert.ToDouble(dict[key]);
                    }
                    catch
                    {
                        val = 0.0;
                    }
                    if (key.Contains("Ratio"))
                    {
                        value = (val * 100).ToString();
                    }
                    else
                    {
                        value = val.ToString();
                    }
                }


                if (!string.IsNullOrEmpty(value))
                {
                    sql += string.Format("{0}='{1}', ", (key), value);
                }
                else
                {
                    //return "人员未找到！";
                }
            }

        }
        sql = sql.Substring(0, sql.Length - 2);
        sql += string.Format(" where Hospital = '{0}' and Product='{1}' and Sales='{2}' and Year={3}"
            , dict["Hospital"], dict["Product"], dict["Sales"], dict["Year"]);
        return sql;
    }

    public static string ImportInsertInfos(Dictionary<string, string> dict)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string fileds = "";
        string values = "";
        foreach (string key in dict.Keys)
        {
            //不包含中文列名及excel缺省列名ColumnXXX
            if (!StringTools.HasChinese(key) && !key.Contains("Column"))
            {
                string value = "";
                if (key.Contains("Department") || key.Contains("Sector") ||
                    key.Contains("Supervisor") || key.Contains("Specification") ||
                    key.Contains("Manager") || key.Contains("Director") ||
                    key.Contains("Hospital") ||
                    key.Contains("Product") || key.Contains("Sales"))
                {
                    value = dict[key];
                }
                else
                {
                    double val = 0.0;
                    try
                    {
                        val = Convert.ToDouble(dict[key]);
                    }
                    catch
                    {
                        val = 0.0;
                    }
                    if (key.Contains("Ratio"))
                    {
                        value = (val * 100).ToString();
                    }
                    else
                    {
                        value = val.ToString();
                    }
                }
                if (!string.IsNullOrEmpty(value))
                {
                    fileds += string.Format("{0},", (key));
                    values += string.Format("'{0}',", value);
                }
                else
                {
                    //return "人员未找到！";
                }
            }
        }
        if (!string.IsNullOrEmpty(fileds))
        {
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            //fileds += string.Format("HospitalId, ProductId, SalesId");
            //values += string.Format("{0},{1},{2}", HospitalId, ProductId, SalesId);
        }
        else
        {
            return "";
        }

        string sql = string.Format("Insert into {0} ({1}) values ({2}) ", "sales_task", fileds, values);
        return sql;
    }
}