using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mDeliverRelated : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMobileReimbursement",
          "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
          "1000006",
          "http://yelioa.top/mDeliverRelated.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        //if (HasRight(user.wechatUserId) == 0)
        //    return;
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getData")
            {
                GetData();
            }
            Response.End();
        }
    }

    //private int  HasRight(string wechatUserId)
    //{
    //    if (wechatUserId != "D17110393" && wechatUserId != "D16030170")
    //    {
    //        Response.Clear();
    //        Response.Write("<script language='javascript'>alert('抱歉，您无访问此页面的权限！')</script>");
    //        Response.End();
    //        return 0;
    //    }
    //    else
    //        return 1;
    //}

    private void GetData()
    {
        UserInfo user = (UserInfo)Session["user"];
        DataTable dt = OperationDeliverManager.findByCondInfomer(user);
        string res = "";
        if (dt != null)
            res = JsonHelper.DataTable2Json(dt);
        Response.Write(res);
    }
}