using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mApprovalReimburseDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mApprovalReimburseDetail",
           "v5afj_CYpboe-JWNOrCU0Cy-xP5krFq6cWYM9KZfe4o",
           "1000020",
           "http://yelioa.top/mApprovalReimburseDetail.aspx");
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
            else if (action == "agree")
            {
                Response.Write(agree());
            }
            else if (action == "disagree") {
                Response.Write(disagree());
            }
            Response.End();
        }
    }

    private string getList()
    {
        UserInfo user = (UserInfo)Session["user"];

        if (user.userName != "黄慧" && user.userName != "舒婷")
        {
            return JsonHelper.DataTable2Json(new DataTable());
        }

        string date = Request.Form["date"];
        string sql = string.Format("select distinct t1.BatchNo, t1.CreateTime, t1.Code,t1.ReceiptAmount,t1.Status,t3.avatar,t3.userName from (select BatchNo, " +
            "CreateTime,CODE,sum(ReceiptAmount) ReceiptAmount,STATUS from yl_reimburse_detail group by BatchNo) t1 left join yl_reimburse t2 " +
           "on t1.code like concat('%', t2.code, '%') left join users t3 on t2.name = t3.userName " +
           " where t1.status = '已提交' and DATE_FORMAT( createTime, '%Y%m' ) = DATE_FORMAT( CURDATE( ) , '%Y%m' ) group by t1.batchNo", user.userName);

        if (!string.IsNullOrEmpty(date))
        {
            sql = string.Format("select distinct t1.BatchNo, t1.CreateTime, t1.Code,t1.ReceiptAmount,t1.Status,t3.avatar,t3.userName from (select BatchNo, " +
            "CreateTime,CODE,sum(ReceiptAmount) ReceiptAmount,STATUS from yl_reimburse_detail group by BatchNo) t1 left join yl_reimburse t2 " +
            "on t1.code like concat('%', t2.code, '%') left join users t3 on t2.name = t3.userName " +
            " where t1.status = '已提交' and to_days(t1.CreateTime) = to_days('{1}') group by t1.batchNo", user.userName, date);
        }

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string getDetail()
    {
        string batchNo = Request.Form["batchNo"];
        string code = Request.Form["code"];
        List<string> sqls = new List<string>();

        string sql = string.Format("select * from yl_reimburse_detail where batchNo = '{0}'", batchNo);
        sqls.Add(sql);

        sql = string.Format("select * from yl_reimburse where '{0}' like concat('%', code, '%')", code);
        sqls.Add(sql);

        DataSet ds = SqlHelper.Find(sqls.ToArray());

        string result1 = JsonHelper.DataTable2Json(ds.Tables[0]);

        DataTable dt = ds.Tables[1];

        foreach (DataRow dr in dt.Rows)
        {
            dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
        }

        string result2 = JsonHelper.DataTable2Json(dt);

        JObject jObject = new JObject {
            { "result1", result1},
            { "result2", result2},
        };

        return jObject.ToString();
    }

    private string agree()
    {
        string batchNo = Request.Form["batchNo"];
        string code = Request.Form["code"];
        UserInfo user = (UserInfo)Session["user"];
        List<JObject> receiptList = JsonHelper.DeserializeJsonToList<JObject>(Request.Form["receiptList"]);

        JObject result = new JObject(); // 返回结果jobject

        // 判断单据是否已经被审批
        string sql = string.Format("select * from yl_reimburse_detail where batchNo = '{0}' and status != '已提交'", batchNo);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        if (dt.Rows.Count > 0)
        {
            result.Add("code", 400);
            result.Add("msg", "该单据已被审批，请勿重复审批！");

            return result.ToString();
        }

        // 删除并新增修改后的单据列表
        sql = string.Format("delete from yl_reimburse_detail where batchNo = '{0}';", batchNo);
        sql += SqlHelper.GetInsertString(receiptList, "yl_reimburse_detail");
        SqlHelper.Exce(sql);

        sql = string.Format("update yl_reimburse_detail set status = '同意', opinion = '同意', approvalTime = now() where batchNo = '{0}'", batchNo);

        string msg = SqlHelper.Exce(sql);

        if (msg.Contains("操作成功"))
        {
            result.Add("code", 200);
            result.Add("msg", "操作成功");

            WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper("v5afj_CYpboe-JWNOrCU0Cy-xP5krFq6cWYM9KZfe4o", "发票上报", "1000020");

            dt = SqlHelper.Find(string.Format("select t2.wechatUserId, t1.code, t1.remain_fee_amount from yl_reimburse t1 left join users t2 on t1.name = t2.userName " +
                "where '{0}' like concat('%', t1.code, '%') limit 1", code)).Tables[0];
            //// 给提交人发送消息
            wxNetSalesHelper.GetJsonAndSendWxMsg(dt.Rows[0][0].ToString(), "您的发票信息财务已审批通过。审批人为:" + user.userName, "http://yelioa.top//mMySubmittedReimburseDetail.aspx", "1000020");

            sql = "";
            // 如果remian_fee_amount为0 单据改成财务已审批
            foreach (DataRow dr in dt.Rows)
            {
                string tempCode = dt.Rows[0][1].ToString();
                string remain_fee_amount = dt.Rows[0][2].ToString();

                if (remain_fee_amount == "0")
                {
                    sql += string.Format("update yl_reimburse set account_approval_time = now(), account_result = '同意', account_approver = '{1}' where code = '{0}';", tempCode, user.userName);
                }
            }
            SqlHelper.Exce(sql);
        }
        else
        {
            result.Add("code", 500);
            result.Add("msg", "操作失败");
        }

        return result.ToString();
    }

    private string disagree()
    {
        string code = Request.Form["code"];
        string batchNo = Request.Form["batchNo"];
        string opinion = Request.Form["opinion"];

        UserInfo user = (UserInfo)Session["user"];

        JObject result = new JObject(); // 返回结果jobject

        // 判断单据是否已经被审批
        string sql = string.Format("select 1 from yl_reimburse_detail where batchNo = '{0}' and status != '已提交'", batchNo);

        if (SqlHelper.Find(sql).Tables[0].Rows.Count > 0)
        {
            result.Add("code", 400);
            result.Add("msg", "该单据已被审批，请勿重复审批！");

            return result.ToString();
        }

        sql = string.Format("update yl_reimburse_detail set status = '拒绝', opinion = '{0}', approvalTime = now() " +
            "where batchNo = '{1}';", opinion, batchNo);

        string msg = SqlHelper.Exce(sql);

        if (msg.Contains("操作成功"))
        {
            result.Add("code", 200);
            result.Add("msg", "操作成功");

            // 还原消费记录的可用额度
            string[] codeArray = code.Split(',');

            sql = "";
            foreach (string tempCode in codeArray)
            {
                if (string.IsNullOrEmpty(tempCode))
                    continue;

                sql += string.Format("update yl_reimburse set remain_fee_amount = (fee_amount - (select ifnull(sum(receiptAmount), 0) from yl_reimburse_detail " +
                    "where code like '%{0}%' and status = '同意')) where code = '{0}';", tempCode);
            }

            SqlHelper.Exce(sql);

            WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper("v5afj_CYpboe-JWNOrCU0Cy-xP5krFq6cWYM9KZfe4o", "发票上报", "1000020");

            DataTable dt = SqlHelper.Find(string.Format("select t2.wechatUserId from yl_reimburse t1 left join users t2 on t1.name = t2.userName " +
                "where '{0}' like concat('%', t1.code, '%') limit 1", code)).Tables[0];
            //// 给提交人发送消息
            wxNetSalesHelper.GetJsonAndSendWxMsg(dt.Rows[0][0].ToString(), "您的发票信息已被审批拒绝，审批人为: " + user.userName + ",拒绝理由为:" + opinion + ",请尽快重新提交。", "http://yelioa.top//mMySubmittedReimburseDetail.aspx", "1000020");
        }
        else
        {
            result.Add("code", 500);
            result.Add("msg", "操作失败");
        }

        return result.ToString();
    }
}