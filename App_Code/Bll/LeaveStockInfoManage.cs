using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

/// <summary>
/// LeaveStockInfoManage 的摘要说明
/// </summary>
public class LeaveStockInfoManage
{
    public LeaveStockInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    private static DataTable GetDatagridFooter(DataTable dt)
    {
        DataTable footer = new DataTable();
        footer.Columns.Add("dateValidity");
        footer.Columns.Add("amountSend");
        footer.Columns.Add("IsFooter");
        DataRow row = footer.NewRow();
        row["dateValidity"] = "合计";
        row["IsFooter"] = true;
        int FlowSales = 0;
        foreach (DataRow r in dt.Rows)
        {
            FlowSales += Convert.ToInt32(r["amountSend"]);
        }
        row["amountSend"] = FlowSales;
        footer.Rows.Add(row);
        return footer;
    }

    public static DataSet GetInfos(DateTime start, DateTime end, string type)
    {
        DataSet ds = LeaveStockInfoSrv.GetInfos(start, end, type);
        DataTable dt = ds.Tables[0];
        DataSet dsRes = new DataSet();
        dsRes.Tables.Add(dt.Copy());
        dsRes.Tables.Add(GetDatagridFooter(dt));
        return dsRes;
    }

    public static DataTable FillBlankCell(DataTable dt, string colunm)
    {
        ArrayList columnsList = new ArrayList();
        //排除第一行为空的列
        foreach (DataColumn c in dt.Columns)
        {
            if (!string.IsNullOrEmpty(dt.Rows[0][c.ColumnName].ToString()) && colunm != null && !(c.ColumnName.ToString()).Equals(colunm))
            {
                columnsList.Add(c.ColumnName);
            }
        }
        for (int i = 1; i < dt.Rows.Count; i++)
        {
            foreach(string column in columnsList)
            {
                if (string.IsNullOrEmpty(dt.Rows[i][column].ToString()) && colunm != null && !(column.ToString()).Equals(colunm))
                {
                    dt.Rows[i][column] = dt.Rows[i - 1][column];
                }
            }            
        }
        return dt;
    }

    public static Dictionary<string, string> ImportInfos(Dictionary<string, string> dict, string type)
    {
        string res = "";
        if (string.IsNullOrEmpty(type))
        {
            res =  "数据类型为空，请选择正确的数据类型！";
        }
        else
        {
            DataSet ds = LeaveStockInfoSrv.GetFormDetail(type);
            if(ds==null || ds.Tables[0].Rows.Count == 0)
            {
                res = "数据类型无数据！";
            }
            else
            {
                bool needToImport = true;
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                foreach (DataRow field in ds.Tables[0].Rows)
                {
                    string alias = field["alias"].ToString();
                    if (string.IsNullOrEmpty(alias))
                    {
                        continue;
                    }
                    else if (alias == "进发生数&出发生数")//针对“实发数量”字段的进发生数&出发生数的特殊情况做的处理
                    {
                        string number = "";
                        if (dict["单据类型"].ToString().Contains("退"))//退货类型
                        {
                            number = "-" + dict["进发生数"].ToString();
                        }
                        else if (dict["单据类型"].ToString() == "采购")//采购类型，直接忽略
                        {
                            needToImport = false;
                            res = "非销售数据无需导入";
                            break;
                        }
                        else//正常销售类型
                        {
                            number = dict["出发生数"].ToString();
                        }
                        newDict.Add(field["field"].ToString(), number);
                    }
                    else if (alias.IndexOf("DirectStorage") == 0)//列名以“DirectStorage”开头的直接存储别名
                    {
                        string val = alias.Substring("DirectStorage".Length);
                        newDict.Add(field["field"].ToString(), val);
                    }
                    else if (!dict.Keys.Contains(alias))//列名不存在说明数据类型不对
                    {
                        res = "导入文件数据类型与所选择的不符，请选择正确的数据类型！";
                        needToImport = false;
                        break;
                    }
                    else if (field["field"].ToString() == "date" || field["field"].ToString() == "dateValidity")
                    {
                        DateTime date = new DateTime(1900,1,1);
                        if (StringTools.IsInt(dict[alias].ToString()))
                            date = date.AddDays(Convert.ToInt32(dict[alias].ToString()));
                        else
                            date = Convert.ToDateTime(dict[alias].ToString());
                        newDict.Add(field["field"].ToString(), date.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        newDict.Add(field["field"].ToString(), dict[alias].ToString());
                    }

                }
                if(needToImport)
                {
                    newDict.Add("type", type);
                    res = LeaveStockInfoSrv.ImportInfos(newDict,type);
                }
            }
        }
        if (!string.IsNullOrEmpty(res))
        {
            if (res.Contains("操作成功"))
                dict["状态"] = "已导入";
            else
                dict["状态"] = res;
        }
        return dict;
    }

    public static string InsertInfos(ref DataTable dt,string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            return "error:数据类型为空，请选择正确的数据类型！";
        }
        DataSet ds = LeaveStockInfoSrv.GetFormDetail(type);

        //补齐空缺的或省略的cell，第一行为空除外
        dt = FillBlankCell(dt, null);

        ArrayList list = new ArrayList();
             

        foreach (DataRow row in dt.Rows)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("type", type);
            bool DocumentTypeIsPurchase = false;
            foreach (DataRow field in ds.Tables[0].Rows)
            {
                string alias = field["alias"].ToString();
                if(string.IsNullOrEmpty(alias))
                {
                    continue;
                }
                else if(alias== "进发生数&出发生数")//针对“实发数量”字段的进发生数&出发生数的特殊情况做的处理
                {
                    string number = "";
                    if (row["单据类型"].ToString().Contains("退"))//退货类型
                    {
                        number = "-" + row["进发生数"].ToString();
                    }
                    else if (row["单据类型"].ToString() == "采购")//采购类型，直接忽略
                    {
                        DocumentTypeIsPurchase = true;
                        break;
                    }
                    else//正常销售类型
                    {
                        number = row["出发生数"].ToString();
                    }
                    dict.Add(field["field"].ToString(), number); 
                }
                else if (alias.IndexOf("DirectStorage")==0)//列名以“DirectStorage”开头的直接存储别名
                {
                    string val = alias.Substring("DirectStorage".Length);
                    dict.Add(field["field"].ToString(), val);
                }
                else if(!dt.Columns.Contains(alias))//列名不存在说明数据类型不对
                {
                    return "error:导入文件数据类型与所选择的不符，请选择正确的数据类型！";
                }
                else if(field["field"].ToString() == "date" || field["field"].ToString() == "dateValidity")
                {
                    DateTime date = Convert.ToDateTime(row[alias].ToString());
                    dict.Add(field["field"].ToString(), date.ToString("yyyy-MM-dd"));
                }
                else
                {
                    dict.Add(field["field"].ToString(), row[alias].ToString());
                }                
            }
            if(DocumentTypeIsPurchase)//采购类型，直接忽略
            {
                continue;
            }
            list.Add(dict);
        }

        string[] res = LeaveStockInfoSrv.InsertInfos(list).Split(';');
        dt.Columns.Add("状态");
        dt.Columns["状态"].SetOrdinal(0);//调整到第一列
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            SqlExceRes sqlRes = new SqlExceRes(res[i]);
            if (sqlRes.Result == SqlExceRes.ResState.Success)
            {
                dt.Rows[i]["状态"] = "已导入";
            }
            else if (sqlRes.Result == SqlExceRes.ResState.Repetition)
            {
                dt.Rows[i]["状态"] = "有重复";
            }
            else
            {
                dt.Rows[i]["状态"] = sqlRes.ExceMsg;
            }
        }
        return "插入成功！";
    }
    
    public static DataSet GetTemplateInfo()
    {
        DataSet ds = LeaveStockInfoSrv.GetTemplateInfo();
        if (ds != null)
        {
            //删掉Id和type字段
            for(int i= ds.Tables[0].Rows.Count-1;i>=0;i--)
            {
                DataRow row = ds.Tables[0].Rows[i];
                if (row["field"].ToString().Contains("Id") || row["field"].ToString().Contains("type"))
                {
                    ds.Tables[0].Rows.Remove(row);
                }
            }
        }
        return ds;
    }

    public static DataSet GetDatalistInfo()
    {
        return LeaveStockInfoSrv.GetDatalistInfo();
    }

    public static DataSet CheckInfo(string type)
    {
        return LeaveStockInfoSrv.CheckInfo(type);
    }

    public static Dictionary<string, string> GetFormDetail(string type)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        DataSet ds = LeaveStockInfoSrv.GetFormDetail(type);
        dict.Add("type", type);
        if(ds != null)
        {
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                dict.Add(row["field"].ToString(), row["alias"].ToString());
            }
        }
        return dict;
    }

    public static string InsertTemplate(ArrayList list)
    {
        string res = "";
        string[] res1 = LeaveStockInfoSrv.InsertTemplate(list).Split(';');
        for (int i=0;i<list.Count;i++)
        {
            SqlExceRes sqlRes = new SqlExceRes(res1[i]);
            if(sqlRes.Result == SqlExceRes.ResState.Error)
            {
                res += sqlRes.ExceMsg;
            }
        }
        if (string.IsNullOrEmpty(res))
        {
            return "提交成功！";
        }
        else
        {
            return res;
        }        
    }

    public static string UpdateTemplate(ArrayList list, string type)
    {
        string res = "";
        string[] res1 = LeaveStockInfoSrv.UpdateTemplate(list, type).Split(';');
        for (int i = 0; i < list.Count; i++)
        {
            SqlExceRes sqlRes = new SqlExceRes(res1[i]);
            if (sqlRes.Result == SqlExceRes.ResState.Error)
            {
                res += sqlRes.ExceMsg;
            }
        }
        if (string.IsNullOrEmpty(res))
        {
            return "提交成功！";
        }
        else
        {
            return res;
        }
    }

    public static string DeleteTemplate(string type)
    {
        string res = LeaveStockInfoSrv.DeleteTemplate(type);
        SqlExceRes sqlRes = new SqlExceRes(res);
        return sqlRes.GetResultString("删除成功！", "");
    }

    public static string DeleteData(string id)
    {
        return LeaveStockInfoSrv.DeleteData(id);
    }

    public static Dictionary<string, string> GetTemplate(string type)
    {
        DataSet ds = LeaveStockInfoSrv.GetFormDetail(type);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            if(!string.IsNullOrEmpty(row["alias"].ToString()))
                dict.Add(row["field"].ToString(), row["alias"].ToString());
        }
        return dict;
    }
}