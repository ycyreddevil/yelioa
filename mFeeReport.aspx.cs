using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

public partial class mFeeReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
            "Zg8Be_YI2m56f5i1u3IWOeJaUtLccRkzc4Ivniv0vco",
            "1000003",
            "http://yelioa.top/mFeeReport.aspx");
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
            if (action == "department")
            {
                Response.Write(getFeeDetailTree());
            }
            
            //else if (action == "updateDifferReason")
            //{
            //    Response.Write(updateDifferReason());
            //}
            Response.End();
        }
    }

    private string getFeeDetailTree()
    {
        string keyWord = Request.Form["keyWord"];

        string sql = string.Format("select * from department where name like '%{0}%'", keyWord);

        DataSet ds = SqlHelper.Find(sql);

        if (ds == null)
            return null;

        DataTable parentDt = ds.Tables[0];

        if (parentDt.Rows.Count == 0)
            return null;

        return JsonHelper.DataTable2Json(parentDt);
    }
}