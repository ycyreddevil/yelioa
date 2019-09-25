using System;
using System.Data;
using System.Web;

public partial class mPrepaid : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxNetSalesHelper wx = new WxNetSalesHelper("http://yelioa.top/mSalesReport.aspx");
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
            if (action == "getPrepaidData")
            {
                Response.Write(getPrepaidData());
            }
            else if (action == "updatePrepaidData")
            {
                Response.Write(updatePrepaidData());
            }

            Response.End();
        }
    }

    protected string getPrepaidData()
    {
        DataTable dt = PrepaidManage.getPrepaidData();

        return JsonHelper.DataTable2Json(dt);
    }

    protected string updatePrepaidData()
    {
        string jsonRows = Request.Form["newRows"];
        
        string msg = PrepaidManage.updatePrepaidData(jsonRows);
        return msg;
    }
}