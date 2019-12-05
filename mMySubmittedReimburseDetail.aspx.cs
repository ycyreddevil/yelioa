using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mMySubmittedReimburseDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMySubmittedReimburseDetail",
           "v5afj_CYpboe-JWNOrCU0Cy-xP5krFq6cWYM9KZfe4o",
           "1000020",
           "http://yelioa.top/mMySubmittedReimburseDetail.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        string action = Request.Params["action"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getList")
            {
                Response.Write(getList());
            }
            else if (action == "getDetail")
            {
                Response.Write(getDetail());
            }
            else if (action == "reEdit")
            {
                Response.Write(reEdit());
            }
            Response.End();
        }
    }

    private string getList()
    {
        UserInfo user = (UserInfo)Session["user"];
        string date = Request.Form["date"];

        string sql = string.Format("select distinct t1.BatchNo, t1.CreateTime, t1.Code,t1.ReceiptAmount,t1.Status,t1.Opinion,t3.avatar from (select BatchNo, " +
            "CreateTime,CODE,sum(ReceiptAmount) ReceiptAmount,STATUS,Opinion from yl_reimburse_detail group by BatchNo) t1 left join yl_reimburse t2 " +
           "on t1.code like concat('%', t2.code, '%') left join users t3 on t2.name = t3.userName " +
           " where t2.name = '{0}' and t1.status != '草稿' and DATE_FORMAT( createTime, '%Y%m' ) = DATE_FORMAT( CURDATE( ) , '%Y%m' ) group by t1.batchNo order by t1.createTime", user.userName);

        if (!string.IsNullOrEmpty(date))
        {
            sql = string.Format("select distinct t1.BatchNo, t1.CreateTime, t1.Code,t1.ReceiptAmount,t1.Opinion,t1.Status,t3.avatar from (select BatchNo, " +
            "CreateTime,CODE,sum(ReceiptAmount) ReceiptAmount,STATUS,Opinion from yl_reimburse_detail group by BatchNo) t1 left join yl_reimburse t2 " +
            "on t1.code like concat('%', t2.code, '%') left join users t3 on t2.name = t3.userName " +
            " where t2.name = '{0}' and t1.status != '草稿' and DATE_FORMAT( createTime, '%Y%m' ) = DATE_FORMAT( '{1}' , '%Y%m' ) group by t1.batchNo order by t1.createTime", user.userName, date);
        }

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string getDetail()
    {
        string batchNo = Request.Form["batchNo"];
        string sql = string.Format("select * from yl_reimburse_detail where batchNo = '{0}'", batchNo);
        DataTable dt = SqlHelper.Find(sql).Tables[0];
        return JsonHelper.DataTable2Json(dt);
    }


    private string reEdit()
    {
        string[] codeArray = Request.Form["code"].Split(',');
        string batchNo = Request.Form["batchNo"];

        double totalReceiptAmount = double.Parse(SqlHelper.Find(string.Format("select sum(receiptAmount) from yl_reimburse_detail " +
            "where batchNo = '{0}'", batchNo)).Tables[0].Rows[0][0].ToString());

        string sql = string.Format("select 1 from yl_reimburse t1 left join yl_reimburse_detail_relevance t2 on t1.code = t2.reimburseCode " +
                "where t2.batchNo = '{0}'", batchNo);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        string _sql = "";

        foreach (string tempCode in codeArray)
        {
            if (string.IsNullOrEmpty(tempCode))
                continue;

            if (dt.Rows.Count > 0)
            {
                _sql += string.Format("update yl_reimburse t1 left join yl_reimburse_detail_relevance t2 on t1.code = t2.reimburseCode set t1.remain_fee_amount = " +
                   "t1.remain_fee_amount + t2.amount where t2.batchNo = '{0}' and t2.reimburseCode = '{1}';", batchNo, tempCode);
            }
            else
            {
                DataTable tempDt = SqlHelper.Find(string.Format("select fee_amount, remain_fee_amount from yl_reimburse where code = '{0}'", tempCode)).Tables[0];

                double fee_amount = double.Parse(tempDt.Rows[0][0].ToString());
                double remain_fee_amount = double.Parse(tempDt.Rows[0][1].ToString());

                if ((fee_amount - remain_fee_amount) >= totalReceiptAmount)
                {
                    _sql += string.Format("update yl_reimburse set remain_fee_amount = remain_fee_amount + {0} where code = '{1}';", totalReceiptAmount, tempCode);
                    break;
                }
                else
                {
                    _sql += string.Format("update yl_reimburse set remain_fee_amount = fee_amount where code = '{0}';", tempCode);
                    totalReceiptAmount -= fee_amount - remain_fee_amount;
                }
            }
        }

        _sql += string.Format("update yl_reimburse_detail set status = '草稿' where batchNo = '{0}';", batchNo);
        _sql += string.Format("delete from yl_reimburse_detail_relevance where batchNo = '{0}'", batchNo);

        string msg = SqlHelper.Exce(_sql);

        JObject result = new JObject();

        if (msg.Contains("操作成功"))
        {
            result.Add("msg", "成功");
            result.Add("code", "200");
        }
        else
        {
            result.Add("msg", "失败");
            result.Add("code", "500");
        }
        return result.ToString();
    }
}