using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class mMyPoint : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMyPoint",
           "i0SFeOuq3eonsbAWYfmAnrB0k_4K5d3Ub7Y6Z-KkYrc",
           "1000008",
           "http://yelioa.top/mMyPoint.aspx");
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
            if (action == "getmypoint")
            {
                GetMyPoint();
            }
       
            Response.End();
        }
    }

    private void GetMyPoint()
    {
        //string user = "余昌运";
        UserInfo user =(UserInfo)Session["user"];
        string json = "";
        string sql = string.Format("SELECT DISTINCT d.Target, IFNULL(a.month_point, 0) as month_point, IFNULL(b.season_point, 0) as season_point, IFNULL(c.year_point, 0) as year_point, d.total_point FROM" +
            "(SELECT Target,sum(Bpoint)  AS month_point FROM accumulate_points WHERE Target = '{0}' AND CheckState='已审核' AND DATE_FORMAT(EffectiveTime, '%Y%m') = DATE_FORMAT(CURDATE(), '%Y%m') ) as a RIGHT JOIN" +
            "(SELECT Target,  sum(Bpoint) as season_point  FROM accumulate_points WHERE Target = '{1}' AND CheckState='已审核' AND QUARTER(EffectiveTime) = QUARTER(now()) AND YEAR (EffectiveTime) = YEAR (NOW())) b on a.Target = b.Target RIGHT JOIN" +
            "( SELECT Target, sum(Bpoint) as year_point  FROM accumulate_points  WHERE Target = '{2}' AND CheckState='已审核' AND YEAR(EffectiveTime) = YEAR(NOW()))c on c.Target = b.Target RIGHT JOIN" +
            "( SELECT Target, sum(Bpoint) as total_point  FROM  accumulate_points WHERE Target = '{3}' AND CheckState='已审核')d on c.Target = d.Target", user.userName, user.userName, user.userName, user.userName);
        DataSet ds = SqlHelper.Find(sql);
        if (ds != null)
        {
            json = JsonHelper.DataTable2Json(ds.Tables[0]);
        }
        Response.Write(json);
    }
}