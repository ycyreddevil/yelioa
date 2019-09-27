using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mMySubmittedReimburseDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMySubmittedReimburseDetail",
           "v5afj_CYpboe-JWNOrCU0Cy-xP5krFq6cWYM9KZfe4o",
           "1000020",
           "http://yelioa.top/mMySubmittedReimburseDetail.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        string action = Request.Params["action"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getList")
            {
                Response.Write(getList());
            }
            else if (action == "getDetail")
            {
                Response.Write(getDetail());
            }
            Response.End();
        }
    }

    private string getList()
    {
        UserInfo user = (UserInfo)Session["user"];
        string date = Request.Form["date"];

        string sql = string.Format("select distinct t1.BatchNo, t1.CreateTime, t1.Code,t1.ReceiptAmount,t1.Status,t3.avatar from (select BatchNo, " +
            "CreateTime,CODE,sum(ReceiptAmount) ReceiptAmount,STATUS from yl_reimburse_detail group by BatchNo) t1 left join yl_reimburse t2 " +
           "on t1.code like concat('%', t2.code, '%') left join users t3 on t2.name = t3.userName " +
           " where t2.name = '{0}' and t1.status != '草稿' and DATE_FORMAT( createTime, '%Y%m' ) = DATE_FORMAT( CURDATE( ) , '%Y%m' ) group by t1.batchNo", user.userName);

        if (!string.IsNullOrEmpty(date))
        {
            sql = string.Format("select distinct t1.BatchNo, t1.CreateTime, t1.Code,t1.ReceiptAmount,t1.Status,t3.avatar from (select BatchNo, " +
            "CreateTime,CODE,sum(ReceiptAmount) ReceiptAmount,STATUS from yl_reimburse_detail group by BatchNo) t1 left join yl_reimburse t2 " +
            "on t1.code like concat('%', t2.code, '%') left join users t3 on t2.name = t3.userName " +
            " where t2.name = '{0}' and t1.status != '草稿' and to_days(t1.CreateTime) = to_days('{1}') group by t1.batchNo", user.userName, date);
        }

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string getDetail()
    {
        string batchNo = Request.Form["batchNo"];
        string sql = string.Format("select * from yl_reimburse_detail where batchNo = '{0}'", batchNo);
        DataTable dt = SqlHelper.Find(sql).Tables[0];
        return JsonHelper.DataTable2Json(dt);
    }
}