using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

public partial class web_expect_flow_submit_mExpectFlowApprovalDetail : System.Web.UI.Page
{
    private JObject jObject;

    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
            "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc",
            "1000009",
            "http://yelioa.top/web/expect_flow_submit/mExpectFlowApprovalDetail.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        if (!"POST".Equals(Request.RequestType))
        {
            return;
        }

        jObject = HttpHelper.getAjaxData(Request);

        string action = jObject["action"].ToString();

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if (action == "getApprovalDataDetail")
            {
                Response.Write(getApprovalDataDetail());
            }
            else if (action == "submitExpectFlow")
            {
                Response.Write(submitExpectFlow());
            }
            
            Response.End();
        }
    }

    private string getApprovalDataDetail()
    {
        string docCode = jObject["docCode"].ToString();
        return SalesBudgetApplicationManage.getApprovalDetailData(docCode);
    }

    private string submitExpectFlow()
    {
        UserInfo userInfo = (UserInfo)Session["user"];
        List<DepartmentPost> departmentList = (List<DepartmentPost>)Session["DepartmentPostList"];

        return SalesBudgetApplicationManage.submitExpectFlow(jObject, userInfo, departmentList);
    }
}