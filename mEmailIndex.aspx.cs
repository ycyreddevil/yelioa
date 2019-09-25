
using System;
using System.Collections.Specialized;
using System.Web;

public partial class mEmailIndex : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
                    "Zg8Be_YI2m56f5i1u3IWOeJaUtLccRkzc4Ivniv0vco",
                    "1000003",
                    "http://yelioa.top/mEmailIndex.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        if (Common.GetApplicationValid("mEmailIndex.aspx") == "0")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>location.href='Default.aspx';</script>");
            Response.End();
            return;
        }

        string action = Request.Form["act"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getAllNumbers")
            {
                Response.Write(getAllNumbers());
            }
            else if (action == "createEmail")
            {
                Response.Write(createEmail());
            }
            else if (action == "searchEmail")
            {
                Response.Write(searchEmail());
            }
            Response.End();
        }
    }

    private string getAllNumbers()
    {
        UserInfo user = (UserInfo)Session["user"];
        //NameValueCollection data = new NameValueCollection();
        //data.Add("userId", user.wechatUserId);
        //data.Add("act", "EmailStatistique");
        //string res = HttpHelper.Post(YlTokenHelper.GetUrl() + "Email.aspx?Token=" + YlTokenHelper.GetToken(), data);
        //return res;

        return EmailHelper.EmailStatistique(user.wechatUserId);
    }

    private string createEmail()
    {
        UserInfo user = (UserInfo)Session["user"];
        return EmailHelper.CreateEmail(user.wechatUserId, "");
    }

    private string searchEmail()
    {
        UserInfo user = (UserInfo)Session["user"];
        string value = Request.Form["value"];
        string type = Request.Form["type"];

        if ("receive".Equals(type))
        {
            type = "已接收";
        }
        else if ("trash".Equals(type))
        {
            type = "已删除";
        }
        else if ("send".Equals(type))
        {
            type = "已发送";
        }
        else
        {
            type = "草稿";
        }

        return EmailHelper.SearchEmail(value, user.wechatUserId, type);
    }
}