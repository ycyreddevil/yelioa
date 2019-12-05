using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mCompanyFeeApproval : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mCompanyFeeApproval",
           "vbmKxak1-a5Ty1cEBJzUFa1OR9f0V4Yh5j0sJq2-e9o",
           "1000023",
        "http://yelioa.top/mCompanyFeeApproval.aspx");

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
            if (action == "getApprovalNum")
            {
                Response.Write(getApprovalNum());
            }
            else if (action == "getData")
            {
                Response.Write(getData());
            }
            else if (action == "approve")
            {
                Response.Write(approve());
            }
            Response.End();
        }
    }

    private string getApprovalNum()
    {
        UserInfo userInfo = (UserInfo)Session["user"];

        JObject result = new JObject
        {
            { "wages", "" },
            { "outer_wages", "" },
            { "tax", "" },
            { "interest", "" },
            { "depreciation", "" },
            { "amortize", "" },
        };

        // 获取当月未审批的数量
        string sql = string.Format("select distinct t1.company from wages t1 left join approval_approver t2 on t1.docCode = t2.docCode and (t1.level = t2.level) " +
            "left join users t3 on t2.approverId = t3.userId where t2.approverId = '{0}' and t1.status = '审批中';", userInfo.userId.ToString());
        sql += string.Format("select distinct t1.company from outer_wages t1 left join approval_approver t2 on t1.docCode = t2.docCode and (t1.level = t2.level) " +
            "left join users t3 on t2.approverId = t3.userId where t2.approverId = '{0}' and t1.status = '审批中';", userInfo.userId.ToString());
        sql += string.Format("select distinct t1.company from tax t1 left join approval_approver t2 on t1.docCode = t2.docCode and (t1.level = t2.level) " +
            "left join users t3 on t2.approverId = t3.userId where t2.approverId = '{0}' and t1.status = '审批中';", userInfo.userId.ToString());
        sql += string.Format("select distinct t1.company from depreciation t1 left join approval_approver t2 on t1.docCode = t2.docCode and (t1.level = t2.level) " +
            "left join users t3 on t2.approverId = t3.userId where t2.approverId = '{0}' and t1.status = '审批中';", userInfo.userId.ToString());
        sql += string.Format("select distinct t1.company from amortize t1 left join approval_approver t2 on t1.docCode = t2.docCode and (t1.level = t2.level) " +
            "left join users t3 on t2.approverId = t3.userId where t2.approverId = '{0}' and t1.status = '审批中';", userInfo.userId.ToString());

        DataSet ds = SqlHelper.Find(sql);

        if (ds.Tables[0].Rows.Count > 0)
            result["wages"] = JsonHelper.DataTable2Json(ds.Tables[0]);
        if (ds.Tables[1].Rows.Count > 0)
            result["outer_wages"] = JsonHelper.DataTable2Json(ds.Tables[1]);
        if (ds.Tables[2].Rows.Count > 0)
            result["tax"] = JsonHelper.DataTable2Json(ds.Tables[1]);
        if (ds.Tables[3].Rows.Count > 0)
            result["depreciation"] = JsonHelper.DataTable2Json(ds.Tables[2]);
        if (ds.Tables[4].Rows.Count > 0)
            result["amortize"] = JsonHelper.DataTable2Json(ds.Tables[3]);

        return result.ToString();
    }

    private string getData()
    {
        string company = Request.Form["company"];
        string type = Request.Form["type"];
        UserInfo userInfo = (UserInfo)Session["user"];

        string sql = string.Format("select t1.* from {2} t1 left join approval_approver t2 on t1.docCode = t2.docCode and (t1.level = t2.level) " +
            "left join users t3 on t2.approverId = t3.userId where t2.approverId = '{0}' and t1.status = '审批中' and t1.company = '{1}'", userInfo.userId.ToString(), company, type);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string approve()
    {
        UserInfo userInfo = (UserInfo)Session["user"];

        string type = Request.Form["type"];
        string result = Request.Form["result"];
        string opinion = Request.Form["opinion"];
        string docCode = Request.Form["docCode"];

        JObject jobject = new JObject
        {
            { "msg", "ok" },
            { "code", 200 },
        };

        // 首先判断是否有权限审批 防止重复点击
        string sql = string.Format("select 1 from {1} t1 left join approval_approver t2 on t1.docCode = t2.docCode and (t1.level = t2.level) " +
            "left join users t3 on t2.approverId = t3.userId where t2.approverId = '{0}' and t1.status = '审批中' and t1.docCode = '{2}'", userInfo.userId.ToString(), type, docCode);

        DataSet ds = SqlHelper.Find(sql);

        if (ds.Tables[0].Rows.Count == 0)
        {
            jobject["msg"] = "没有权限审批";
            jobject["code"] = 500;

            return jobject.ToString();
        }

        string msg = ApprovalFlowManage.ApproveDocument(type, docCode, userInfo, result, opinion,
             "http://yelioa.top/mCompanyFee.aspx", "http://yelioa.top/mCompanyFeeApproval.aspx",
             "http://yelioa.top/mCompanyFee.aspx", "vbmKxak1-a5Ty1cEBJzUFa1OR9f0V4Yh5j0sJq2-e9o", "mCompanyFeeApproval", "1000023");

        jobject["msg"] = msg;

        return jobject.ToString();
    }
}