using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mEmailList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
                    "Zg8Be_YI2m56f5i1u3IWOeJaUtLccRkzc4Ivniv0vco",
                    "1000003",
                    "http://yelioa.top/mEmailList.aspx");
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
            if (action == "getEmailList")
            {
                Response.Write(getEmailList());
            }
            else if (action == "deleteEmailList")
            {
                Response.Write(deleteEmailList());
            }
            else if (action == "markAsReadOrUnread")
            {
                Response.Write(markAsReadOrUnread());
            }
            else if (action == "getDraftAndSpamList")
            {
                Response.Write(getDraftAndSpamList());
            }
            Response.End();
        }
    }

    private string getEmailList()
    {
        UserInfo user = (UserInfo)Session["user"];
        //NameValueCollection data = new NameValueCollection();
        //data.Add("userId", user.wechatUserId);
        //data.Add("act", "ReceiveEmail");
        //string res = HttpHelper.Post(YlTokenHelper.GetUrl() + "Email.aspx?Token=" + YlTokenHelper.GetToken(), data);
        return EmailHelper.ReceiveEmail(user.wechatUserId);
    }

    private string deleteEmailList()
    {
        UserInfo user = (UserInfo)Session["user"];
        string ids = Request.Form["ids"];

        List<string> idList = new List<string>();
        idList.Add(ids);

        string type = Request.Form["type"];
        return EmailHelper.DeleteEmail(idList, false, type);
    }

    private string markAsReadOrUnread()
    {
       
        string id = Request.Form["id"];
        string status = Request.Form["status"];

        return EmailHelper.ChangeStatusToRead(id, status);
    }

    private string getDraftAndSpamList()
    {
        UserInfo user = (UserInfo)Session["user"];
        string operation = Request.Form["operation"];

        if (operation == "send")
        {
            operation = "已发送";
        }
        else if (operation == "draft")
        {
            operation = "草稿";
        }
        else
        {
            operation = "已删除";
        }

        return EmailHelper.GetDraftAndSpamList(user.wechatUserId, operation);
    }
}