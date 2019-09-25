using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mFinanceApprovalRecord : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mFinanceApprovalRecord",
            "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
            "1000006",
            "http://yelioa.top/mFinanceApprovalRecord.aspx");
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
            if (action == "getInfos")
            {
                Response.Write(getInfo());
            }
            else if (action == "getStatistics")
            {
                Response.Write(getStatistics());
            }          
            Response.End();
        }
    }

    protected string getInfo()
    {
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string keyword = Request.Form["keyword"];
        string year = Request.Form["year"];
        int month =Convert.ToInt32( Request.Form["month"]);
        string res = "F";
        UserInfo user = (UserInfo)Session["user"];
        if (user != null)
        {
            DataTable dt = MobileReimburseManage.findApprovalRecord(user, keyword,year, month);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
                }

                dt = PinYinHelper.SortByPinYin(dt, sort, order);
                res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
            }
        }
        return res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
    }

    private string getStatistics()
    {
        string type = Request.Form["type"];
        string year = Request.Form["year"];
        int month = Convert.ToInt32(Request.Form["month"]);
        UserInfo user = (UserInfo)Session["user"];

        return MobileReimburseManage.accountStatistics(user.userId.ToString(), year, month, type);
    }   
}