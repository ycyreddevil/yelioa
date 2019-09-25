using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// EnterStockInfoManage 的摘要说明
/// </summary>
public class EnterStockInfoManage
{
    public EnterStockInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetInfos(DateTime start, DateTime end, string company)
    {
        DataSet ds = EnterStockInfoSrv.GetInfos(start,end,company);
        return ds;
    }

    public static DataTable InsertInfos(DataTable dt, string companyId)
    {
        //补齐空缺的单据编号
        for(int i=0;i<dt.Rows.Count;i++)
        {
            if(string.IsNullOrEmpty(dt.Rows[i]["单据编号"].ToString()))
            {
                dt.Rows[i]["单据编号"] = dt.Rows[i - 1]["单据编号"];
                dt.Rows[i]["日期"] = dt.Rows[i - 1]["日期"];
            }
        }
        DataTable newDt = dt.Copy();
        string[] res = EnterStockInfoSrv.InsertInfos(dt, companyId).Split(';');
        newDt.Columns.Add("状态");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string[] strs1 = res[i].Split(':');
            string[] strs2 = strs1[1].Split(',');
            if (strs2[1].Contains("操作成功") && Convert.ToInt32(strs2[0]) >0)
            {
                newDt.Rows[i]["状态"] = "操作成功";
            }
            else if (strs2[1].Contains("操作成功") && Convert.ToInt32(strs2[0]) == 0)
            {
                newDt.Rows[i]["状态"] = "有重复";
            }
            else
            {
                string[] strs = res[i].Split(',');
                newDt.Rows[i]["状态"] = strs[1];
            }
        }
        return newDt;
    }

    public static string DeleteData(string id)
    {
        return EnterStockInfoSrv.DeleteData(id);
    }
}