using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mEmailDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
                    "Zg8Be_YI2m56f5i1u3IWOeJaUtLccRkzc4Ivniv0vco",
                    "1000003",
                    "http://yelioa.top/mEmailDetail.aspx");
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
            if (action == "loadDetail")
            {
                Response.Write(loadDetail());
            }
            else if (action == "replyOrForwordEmail")
            {
                Response.Write(replyOrForwordEmail());
            }
            Response.End();
        }
    }

    private string loadDetail()
    {
        UserInfo user = (UserInfo)Session["user"];
        string id = Request.Form["id"];

        return EmailHelper.ReadEmail(user.wechatUserId, id);
    }

    private string replyOrForwordEmail()
    {
        UserInfo userInfo = (UserInfo)Session["user"];
        string emailId = Request.Form["id"];
        string type = Request.Form["type"];

        if ("reply".Equals(type))
        {
            type = "回复";
        }
        else
        {
            type = "转发";
        }

        return EmailHelper.ReplyOrForwordEmail(userInfo.wechatUserId, emailId, type);
    }
}