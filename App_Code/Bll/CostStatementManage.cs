using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;

/// <summary>
/// CostStatementManage 的摘要说明
/// </summary>
public class CostStatementManage
{
    public CostStatementManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string GetPieChart(string departmentId,string year,string month)
    {
        if (departmentId == null)
            departmentId = "1";
        

        JObject res = new JObject();
        string msg = "";
        List<string> departmentList = new List<string>();
        DataSet ds = CostStatementSrv.GetPieChart(departmentId,ref msg,ref departmentList);
        if(ds==null)
        {
            res.Add("ErrCode", "1");
            res.Add("ErrMsg", msg);
        }
        else
        {
            res.Add("ErrCode", "0");
            res.Add("ErrMsg", "操作成功");
            DataTable dt = new DataTable();
            dt.Columns.Add("department", Type.GetType("System.String"));
            dt.Columns.Add("fee", Type.GetType("System.String"));//费用
            dt.Columns.Add("balance", Type.GetType("System.String"));//预算-费用
            for (int i=4;i<ds.Tables.Count;i++)
            {
                
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (departmentList[i - 4] == row["name"].ToString())
                    {
                        dt.Rows.Add(departmentList[i - 4], ds.Tables[i].Rows[0][0].ToString(),(Convert.ToDouble(row["fee"])- Convert.ToDouble(ds.Tables[i].Rows[0][0])).ToString());
                        break;
                    }
                }
                        
            }
            res.Add("ReimbursementAndBalance", JsonHelper.DataTable2Json(dt));
            res.Add("ExpenseAccountRatio", JsonHelper.DataTable2Json(ds.Tables[1]));//费用明细占比

            //计算费用率
            double sum = 0;
            foreach(DataRow row in ds.Tables[2].Rows)
            {
                JObject temp = JObject.Parse(row["DataJson"].ToString());
                sum += Convert.ToDouble(temp["当月流向金额"]);
            }

            foreach (DataRow row in ds.Tables[3].Rows)
            {
                if (sum <= 0)
                {                    
                    row["fee"] = (Convert.ToDouble(row["fee"]) - sum).ToString();
                }
                else
                {
                    row["fee"] = (Convert.ToDouble(row["fee"])/sum).ToString();
                }
            }
            res.Add("CostRate", JsonHelper.DataTable2Json(ds.Tables[3]));
        }
        return res.ToString();
    }

    public static string TheCostOfObtainingTheSubsidiaryExpenses(string feeDetail,string departmentId)
    {
        JObject res = new JObject();
        if(string.IsNullOrEmpty(feeDetail)|| string.IsNullOrEmpty(departmentId))
        {
            res.Add("ErrCode", "1");
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string msg = "";
            DataSet ds = CostStatementSrv.TheCostOfObtainingTheSubsidiaryExpenses(departmentId, feeDetail,ref msg);
            if(ds==null)
            {
                res.Add("ErrCode", "2");
                res.Add("ErrMsg", msg);
            }
            else
            {
                res.Add("ErrCode", "0");
                res.Add("ErrMsg", "操作成功");
                res.Add("data", JsonHelper.DataTable2Json(ds.Tables[0]));
            }
        }
        return res.ToString();
    }
}