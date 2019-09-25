using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

/// <summary>
/// CostSharingApprovalManage 的摘要说明
/// </summary>
public class CostSharingApprovalManage
{
    public CostSharingApprovalManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string queryPendingApprovalList(UserInfo userInfo)
    {
        DataSet ds = CostSharingApprovalSrv.queryPendingApprovalList(userInfo);

        JObject resJObject = new JObject();

        if (ds == null)
        {
            resJObject.Add("ErrCode", 2);
            resJObject.Add("ErrMsg", "数据库查询错误");
            return resJObject.ToString();
        }

        DataTable dt1 = ds.Tables[0];
        DataTable dt2 = ds.Tables[1];

        resJObject.Add("ErrCode", 0);
        
        resJObject.Add("add", merge(dt1));

        resJObject.Add("update1", merge(dt2));

        return resJObject.ToString();
    }

    private static JArray merge(DataTable dt)
    {
        List<string> users = new List<string>();
        string productName = "";
        JObject jObject = new JObject();
        JArray jArray = new JArray();
        JObject innerJObject = new JObject();
        JArray innerJArray = new JArray();
        foreach (DataRow dr in dt.Rows)
        {
            string sales = dr["代表"].ToString();
            string code = dr["code"].ToString();
            if (users.Contains(sales))
            {
                string hospitalCode = dr["newValue"].ToString();
                string hospitalName = SqlHelper.Find(string.Format("select name from fee_branch_dict where id='{0}'", hospitalCode)).Tables[0].Rows[0][0].ToString();
                string info = hospitalName + "|" + productName;
                innerJObject = new JObject();
                innerJObject.Add("info", info);
                innerJObject.Add("id", code);
                innerJArray.Add(innerJObject);
                jObject.Remove("网点信息");
                jObject.Add("网点信息", innerJArray);
                jObject.Remove("avatar");
                jObject.Add("avatar", dr["avatar"].ToString());
                jArray.Add(jObject);
            }
            else
            {
                jObject = new JObject();
                jObject.Add("代表", sales);
                innerJArray = new JArray();

                if (dr["newValue"] != null)
                {
                    string productCode = dr["newValue"].ToString();
                    productName = SqlHelper.Find(string.Format("select name from jb_product where id='{0}'", productCode)).Tables[0].Rows[0][0].ToString();
                }
            }

            users.Add(sales);
        }

        return jArray;
    }
}