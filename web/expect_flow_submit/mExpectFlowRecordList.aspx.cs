using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

public partial class web_expect_flow_submit_mExpectFlowRecordList : System.Web.UI.Page
{
    private JObject jObject;

    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
            "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc",
            "1000009",
            "http://yelioa.top/web/expect_flow_submit/mExpectFlowRecordList.aspx");
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

            if (action == "getExpectFlowRecord")
            {
                Response.Write(getExpectFlowRecord());
            }

            Response.End();
        }
    }

    private string getExpectFlowRecord()
    {
        UserInfo userInfo = (UserInfo) Session["user"];
        return SalesBudgetApplicationManage.getExpectFlowRecord(userInfo);
    }
}