using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mDemandApplyReportApproval : System.Web.UI.Page
{
    public string type = "0";
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMobileReimbursement",
           "AirlZ8lfY50d1KGDklHPQcLV2RUFAdrhD-WXU23cA-w",
           "1000013",
           "http://yelioa.top/mDemandApplyReportApproval.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        string action = Request.Form["act"];

        if (Request.Params["type"] != null)
            type = Request.Params["type"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "myapply")
            {
                Myapply();
            }
            else if (action == "pending")
            {
                Pending();
            }
            else if (action == "approval")
            {
                Response.Write(approval());
            }
            else if (action == "getDocument")
            {
                Response.Write(getDocument());
            }
            Response.End();
        }
    }

    public string Get()
    {
        return type;
    }

    private void Myapply()
    {
        UserInfo user = (UserInfo)Session["user"];
        string sql = "SELECT distinct b.Id, a.clientName as HospitalName,b.DeliverNumber,b.NetSales,b.Stock,b.LMT,b.Remark,b.UserName,c.ProductName,c.specification Specification, c.unit Unit FROM" +
    " new_client AS a RIGHT JOIN demand_apply_report AS b ON a.`clientCode` = b.HospitalCode LEFT JOIN new_product as c ON b.ProductCode = c.productCode" +
    " where b.UserId=" + user.userId + " order by b.Id desc";
        DataSet ds = SqlHelper.Find(sql);
        string json = "";
        if (ds != null)
        {
            json = JsonHelper.DataTable2Json(ds.Tables[0]);
        }
        Response.Write(json);
    }

    private void Pending()
    {
        UserInfo user = (UserInfo)Session["user"];
        string sql = string.Format("select dar.*, dp.ProductName, dp.specification Specification, dp.unit Unit,jc.`clientName` HospitalName from demand_apply_report dar left join approval_approver aa on dar.Id = aa.DocCode and dar.`Level` = aa.`Level` " +
            "left join new_product dp on dar.ProductCode = dp.productCode left join new_client jc on dar.HospitalCode = jc.clientCode where aa.ApproverId = '{0}' order by dar.Id desc", user.userId);
        DataSet ds = SqlHelper.Find(sql);
        string json = "";
        if (ds != null)
        {
            json = JsonHelper.DataTable2Json(ds.Tables[0]);
        }
        Response.Write(json);
    }

    private string approval()
    {
        string id = Request.Form["docCode"];
        string approvalResult = Request.Form["approvalResult"];
        UserInfo userInfo = (UserInfo)Session["user"];
        string msg = ApprovalFlowManage.ApproveDocument("demand_apply_report", id, userInfo, approvalResult, "",
            "http://yelioa.top//mDemandApplyReportApproval.aspx?type=0&docCode=" + id, "http://yelioa.top//mDemandApplyReportAppRoval.aspx?type=1&docCode=" + id, "http://yelioa.top/mDemandApplyReport.aspx",
            "AirlZ8lfY50d1KGDklHPQcLV2RUFAdrhD-WXU23cA-w", "DemandApplyReport", "1000013");
        if (msg == "审批流程结束")
        {
            if (!ApprovalFlowManage.EndDemandApprove(id).Contains("操作成功"))
                msg += ",添加审批时间失败";

            // 把数据添加到货需日表中
            string insertSql = string.Format("insert into purchase_daily_report (docNo,batchNo,date,code,purchaseNumber) " +
                "values ('{0}','',now(),(select productCode code from demand_apply_report where id = '{0}'),(select deliverNumber purchaseNumber from demand_apply_report where id = '{0}'))", id);

            SqlHelper.Exce(insertSql);
        }
        JObject jObject = new JObject();
        jObject.Add("msg", jObject);
        return jObject.ToString();
    }

    private string getDocument()
    {
        string docCode = Request.Form["docCode"];
        string sql = string.Format("select dar.*, jp.ProductName,jp.specification Specification, jp.unit Unit,jc.clientName HospitalName from demand_apply_report dar " +
            "left join new_product jp on dar.productCode = jp.code left join new_client jc on dar.hospitalCode = jc.code" +
            " where dar.id = " + docCode);
        DataSet ds = SqlHelper.Find(sql);
        string res = "单据未找到或无权限查看该单据！";
        if (ds != null && ds.Tables[0].Rows.Count > 0)
            res = JsonHelper.SerializeObject(ds.Tables[0]);
        return res;
    }
}