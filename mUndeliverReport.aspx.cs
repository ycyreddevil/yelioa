using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mUndeliverReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mPointApply",
          "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc",
          "1000009",
          "http://yelioa.top/mUndeliverReport.aspx");
        UserInfo user = new UserInfo();
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        string action = Request.Form["act"];
        if(!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if(action== "UndeliverReport")
            {
                GetUndeliverReport();
            }
            Response.End();
        }
    }

    private void GetUndeliverReport()
    {
        string  d = Request.Form["date"].ToString();
        DateTime date = Convert.ToDateTime(d);
        string month = "";
        if (date.Month < 10)
            month= "0" + date.Month.ToString();
        else
            month= date.Month.ToString();
        string sql = string.Format("SELECT a.UserName,c.`Name` AS HospitalName,b.`Name` as ProductName,b.Specification,(a.DeliverNumber - a.ApprovalNumber) AS UndeliverNumber,b.Unit,a.Remark,a.LMT,a.Reason" +
                    " FROM deliver_apply_report AS a LEFT JOIN jb_product AS b ON a.ProductCode = b.`Code` LEFT JOIN jb_client AS c ON a.HospitalCode = c.`Code` " +
                    "where DATE_FORMAT(a.ApproveTime,'%Y-%m')='{0}-{1}' and (a.DeliverNumber - a.ApprovalNumber) != 0 and Opinion is null  and `Status` = '已审批' and a.ApproveTime is not null", date.Year,month);
        DataSet ds = SqlHelper.Find(sql);
        string json = "";
        if(ds!=null&&ds.Tables[0].Rows.Count>0)
        {
            json = JsonHelper.DataTable2Json(ds.Tables[0]);
        }
        Response.Write(json);
    }
}