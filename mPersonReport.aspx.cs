using System;
using System.Data;
using System.Web;

public partial class mPersonReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //WxNetSalesHelper wx = new WxNetSalesHelper("http://yelioa.top/mPersonReport.aspx");
        WxCommon wx = new WxCommon("mSalesData",
            "Zg8Be_YI2m56f5i1u3IWOeJaUtLccRkzc4Ivniv0vco",
            "1000003",
            "http://yelioa.top/mPersonReport.aspx");
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
            if (action == "getPersonReport")
            {
                Response.Write(getPersonReport());
            }
            else if (action == "dataGridSort")
            {
                Response.Write(dataGridSort());
            }
            
            Response.End();
        }
        else
        {
            action = Request.Params["act"];
            if (!string.IsNullOrEmpty(action))
            {
                Response.Clear();
                if (action == "getAllSector")
                {
                    Response.Write(getAllSector());
                }
                Response.End();
            }
        }
    }

    protected string getPersonReport()
    {
        int year = Int32.Parse(Request.Form["year"]);
        int month = Int32.Parse(Request.Form["month"]);
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string name = Request.Form["name"];
        string sector = Request.Form["sector"];

        UserInfo user = (UserInfo)Session["user"];

        DataTable dt = SalesReportManage.getSalesmanCompleteRate(year, month, name, sector, user);
        string json = "";

        if (dt != null)
        {
            dt = PinYinHelper.SortByPinYin(dt, sort, order);
            json = JsonHelper.DataTable2Json(dt);
        }

        return json.ToString();
    }

    protected string dataGridSort()
    {
        string data = Request.Form["data"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        DataTable dt = JsonHelper.DeserializeJsonToObject<DataTable>(data);
        dt = PinYinHelper.SortByPinYin(dt, sort, order);
        return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
    }

    protected string getAllSector()
    {
        DataTable dt = SalesReportManage.getSector();
        if (dt == null)
            return null;
        DataRow dr = dt.NewRow();
        dr["Sector"] = "全部";
        dt.Rows.InsertAt(dr, 0);
        return JsonHelper.DataTable2Json(dt);
    }
}