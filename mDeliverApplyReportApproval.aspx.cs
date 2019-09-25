using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mDeliverApplyReportApproval : System.Web.UI.Page
{
    public string type = "0";
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMobileReimbursement",
           "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
           "1000006",
           "http://yelioa.top/mDeliverApplyReportApproval.aspx");
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
            else if (action == "getAttachmentAndProcess")
            {
                Response.Write(getAttachmentAndProcess());
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
        string sql = "SELECT a.clientName as HospitalName,b.*,c.productName as ProductName,c.specification Specification, c.unit Unit FROM" +
    " new_client AS a RIGHT JOIN deliver_apply_report AS b ON a.clientCode = b.HospitalCode LEFT JOIN new_product as c ON b.ProductCode = c.productCode" +
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
        string sql = string.Format("select dar.*, dp.ProductName, dp.specification Specification, dp.unit Unit, jc.clientName HospitalName from deliver_apply_report dar left join approval_approver aa on dar.Id = aa.DocCode and dar.`Level` = aa.`Level` " +
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
        string msg = ApprovalFlowManage.ApproveDocument("deliver_apply_report", id, userInfo, approvalResult, "",
            "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0&docCode=" + id, "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=1&docCode=" + id, "http://yelioa.top/mDeliverApplyReport.aspx",
            "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc", "DeliverApplyReport", "1000009");
        if (msg == "审批流程结束")
        {
            if (!ApprovalFlowManage.EndDeliverApprove(id).Contains("操作成功"))
                msg += ",添加审批时间失败";
        }
        JObject jObject = new JObject();
        jObject.Add("msg", jObject);
        return jObject.ToString();
    }

    private string getDocument()
    {
        string docCode = Request.Form["docCode"];
        string type = Request.Form["type"];

        var sqls = new List<string>();
        // 判断是否有审批权限

        var sql = "";
        if (type == "0")
        {
            sql = string.Format("select t2.approverId from deliver_apply_report t1 left join approval_approver t2 on t1.level = t2.level where t1.id = '{0}' " +
            "and t2.documentTableName = 'deliver_apply_report';", docCode);
            sqls.Add(sql);
        }

        sql = string.Format("select dar.*, jp.ProductName,jp.specification Specification, jp.unit Unit,jc.clientName HospitalName from deliver_apply_report dar " +
            "left join new_product jp on dar.productCode = jp.productCode left join new_client jc on dar.hospitalCode = jc.clientCode" +
            " where dar.id = " + docCode + ";");
        sqls.Add(sql);

        DataSet ds = SqlHelper.Find(sqls.ToArray());
        string res = "单据未找到或无权限查看该单据！";

        UserInfo userInfo = (UserInfo)Session["user"];

        if (type != "0" && ds.Tables[0].Rows[0][0].ToString() != userInfo.userId.ToString())
        {
            return res;
        }

        if (type == "0" && ds.Tables[1].Rows.Count > 0)
        {
            res = JsonHelper.SerializeObject(ds.Tables[1]);
        }
        else if (type == "1" && ds.Tables[0].Rows.Count > 0)
        {
            res = JsonHelper.SerializeObject(ds.Tables[0]);
        } 
            
        return res;
    }

    private string getAttachmentAndProcess()
    {
        string docCode = Request.Form["docCode"];
        string sql = string.Format("select da.ImageUrl from deliver_apply_report dar left join deliver_attachment da on dar.id = da.docCode " +
        "where dar.id = '{0}';", docCode);

        sql += string.Format("select t1.*, t2.userName name from approval_process t1 left join users t2 on t1.approverId = t2.userId where documentTableName = 'deliver_apply_report' and docCode = '{0}';", docCode);
        sql += string.Format("select t1.*,t2.userName Name from approval_record t1 left join users t2 on " +
            "t1.approverId = t2.userId where docCode = '{0}' and DocumentTableName = 'deliver_apply_report' order by id ", docCode);

        DataSet ds = SqlHelper.Find(sql);
        string res = "单据未找到或无权限查看该单据！";

        var dictionary = new Dictionary<string, object>();

        if (ds != null)
        {
            dictionary.Add("attachment", ds.Tables[0]);
            dictionary.Add("approver", ds.Tables[1]);
            dictionary.Add("record", ds.Tables[2]);
        }
            
        return JsonHelper.SerializeObject(dictionary);
    }
}