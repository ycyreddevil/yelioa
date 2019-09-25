using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bll.cost_sharing;
using Newtonsoft.Json.Linq;

public partial class web_cost_sharing_mCostSharingSubmit : System.Web.UI.Page
{
    private JObject jObject;

    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
            "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc",
            "1000009",
            "http://yelioa.top/web/cost_sharing/mCostSharingSubmit.aspx");
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

            if (action == "queryRelativeCostSharing")
            {
                Response.Write(queryRelativeCostSharing());
            }

            Response.End();
        }
    }

    private string queryRelativeCostSharing()
    {
        UserInfo userInfo = (UserInfo) Session["user"];
        return CostSharingSubmitManage.queryRelativeCostSharing(userInfo);
    }
}