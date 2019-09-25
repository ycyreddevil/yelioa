using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class mMobileReimbursement : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMobileReimbursement",
            "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
            "1000006",
            "http://yelioa.top/mMobileReimbursement.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getDocument")
            {
                Response.Write(getDocument());
            }
            else if (action == "getInfosRelatedToMe")
            {
                Response.Write(getInfosRelatedToMe());
            }
            else if (action == "approvalReimburse")
            {
                Response.Write(approvalReimburse());
            }
            else if (action == "getBudget")
            {
                Response.Write(getBudget());
            }

            Response.End();
        }
    }
    private string getInfosRelatedToMe()
    {
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string keyword = Request.Form["keyword"];

        string res = "F";
        UserInfo user = (UserInfo)Session["user"];
        if (user != null)
        {
            DataSet ds = ReimbursementManage.GetDocumnetsInfosRelatedToMe(user.userId.ToString(), keyword);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
                }
                res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
            }
        }

        return res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
    }

    private string getDocument()
    {
        string code = Request.Form["docCode"];
        UserInfo user = (UserInfo)Session["user"];

        // 查询这一级的审批人是否是该用户
        DataSet checkApproverDt = ReimbursementManage.checkApprover(code, user.userId.ToString());

        string msg = "";

        if (checkApproverDt == null || checkApproverDt.Tables[0] == null ||
            checkApproverDt.Tables[0].Columns.Count == 0 || checkApproverDt.Tables[0].Rows.Count == 0)
        {
            msg = "当前用户无审批权限";
            return JsonHelper.SerializeObject(msg);
        }

        DataSet ds = ReimbursementManage.GetDocumnetsInfosByCode(code, user.userName);
        string res = "单据未找到或无权限查看该单据！";
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            var dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
            }
            res = JsonHelper.SerializeObject(dt);
        }
            
        return res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
    }
    private string getBudget()
    {
        string docCode = Request.Form["docCode"];
        string sql = "select * from yl_reimburse where id = " + docCode;
        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return ReimbursementManage.IsOverBudget(dt.Rows[0]["fee_department"].ToString(), dt.Rows[0]["fee_detail"].ToString()
            , Convert.ToDouble(dt.Rows[0]["fee_amount"].ToString()), DateTime.Now).ToString();
    }

    private string approvalReimburse()
    {
        string docCode = Request.Form["docCode"];
        string ApprovalResult = Request.Form["ApprovalResult"];
        string ApprovalOpinions = Request.Form["ApprovalOpinions"];

        UserInfo userInfo = (UserInfo)Session["user"];

        string sql = "select * from yl_reimburse where id = " + docCode;
        DataTable dt = SqlHelper.Find(sql).Tables[0];
        string code = dt.Rows[0]["code"].ToString();

        // 查询这一级的审批人是否是该用户
        DataSet checkApproverDt = ReimbursementManage.checkApprover(code, userInfo.userId.ToString());

        string msg = "";

        if (checkApproverDt == null || checkApproverDt.Tables[0] == null ||
            checkApproverDt.Tables[0].Columns.Count == 0 || checkApproverDt.Tables[0].Rows.Count == 0)
        {
            msg = "当前用户无审批权限";
            return JsonHelper.SerializeObject(msg);
        }

       //// 若提交时间超过当月25号 则不允许审批
       // DateTime submitTime = Convert.ToDateTime(dt.Rows[0]["apply_time"]);
       // DateTime theTime = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-25" + " 00:00:00");

       // if (submitTime > theTime)
       // {
       //     msg = "该单据超过25号提交，暂无法审批！";
       //     return JsonHelper.SerializeObject(msg);
       // }

       // if (ApprovalResult == "同意" && dt.Rows[0]["IsOverBudget"].ToString() != "1")
       // {
       //     msg = ReimbursementManage.IsOverBudget(dt.Rows[0]["fee_department"].ToString(), dt.Rows[0]["fee_detail"].ToString(), Convert.ToDouble(dt.Rows[0]["fee_amount"].ToString()));

       //     if (msg.Contains("预算余额不足"))
       //     {
       //         return JsonHelper.SerializeObject(msg);
       //     }
       // }

        msg = ApprovalFlowManage.ApproveDocument("yl_reimburse", docCode, userInfo, ApprovalResult, ApprovalOpinions,
             "http://yelioa.top/mMySubmittedReimburse.aspx?docCode=" + code, "http://yelioa.top/mMobileReimbursement.aspx?docCode=" + code, "http://yelioa.top/mMySubmittedReimburse.aspx?docCode=" + code, "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk", "yl_reimburse", "1000006");

        if ("审批流程结束".Equals(msg) || "审批拒绝！".Equals(msg))
        {
            // 清除approval_process表中内容
            MobileReimburseManage.clearApprovalProcess(docCode);
            // 更新审批时间
            MobileReimburseManage.updateApprovalTimeAndResultAndOpinion(docCode, ApprovalResult, ApprovalOpinions);
        }
        return JsonHelper.SerializeObject(msg);
    }

    private string returnReimburse()
    {
        string docCode = Request.Form["docCode"];
        UserInfo userInfo = (UserInfo)Session["user"];
        ApprovalFlowManage.returnDocument("yl_reimburse", docCode, userInfo);
        return null;
    }
}