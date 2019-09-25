using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mPurchaseReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
            "Zg8Be_YI2m56f5i1u3IWOeJaUtLccRkzc4Ivniv0vco",
            "1000003",
            "http://yelioa.top/mPurchaseReport.aspx");
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
            if (action == "getDailyReport")
            {
                Response.Write(getDailyReport());
            }
            else if (action == "getMonthReport")
            {
                Response.Write(getMonthReport());
            }
            //else if (action == "updateDifferReason")
            //{
            //    Response.Write(updateDifferReason());
            //}
            Response.End();
        }
    }

    private string getDailyReport()
    {
        string date = Request.Form["dateString"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];

        DataTable dt = ImportPurchaseManage.findByDate(date);

        string json = "";

        if (dt != null)
        {
            dt = PinYinHelper.SortByPinYin(dt, sort, order);
            json = JsonHelper.DataTable2Json(dt);
        }

        return json;
    }

    private string getMonthReport()
    {
        string date = Request.Form["dateString"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];

        DataTable dt = ImportPurchaseManage.findByMonth(date);

        string json = "";

        if (dt != null)
        {
            dt = PinYinHelper.SortByPinYin(dt, sort, order);
            json = JsonHelper.DataTable2Json(dt);
        }

        return json;
    }

    //private string updateDifferReason()
    //{
    //    string dataJson = Request.Form["dataJson"];
    //    string year = Request.Form["year"];
    //    string month = Request.Form["month"];

    //    return ImportPurchaseManage.updateDifferReason(dataJson, year, month);

    //}
}