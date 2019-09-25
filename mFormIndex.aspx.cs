using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mFormIndex : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
                    "E26TbitJpOlsniJaKMq6lrNYhiu1bKVtRddflNwIsoE",
                    "1000015",
                    "http://yelioa.top/mFormIndex.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        if(Common.GetApplicationValid("mFormIndex.aspx") =="0")
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
            if (action == "findFormByType")
            {
                Response.Write(findFormByType());
            }
           
            Response.End();
        }
    }

    private string findFormByType()
    {
        string type = Request.Form["type"];
        UserInfo userInfo = (UserInfo)Session["user"];

        return FormBuilderHelper.findFormByType(type, userInfo);
    }
}