using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mRelatedReimburse : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mRelatedReimburse",
            "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
            "1000006",
            "http://yelioa.top/mRelatedReimburse.aspx");
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
            if (action == "getInfosRelatedToMe")
            {
                Response.Write(getInfosRelatedToMe());
            }
            
            Response.End();
        }
    }

    public string getInfosRelatedToMe()
    {
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string keyword = Request.Form["keyword"];

        string res = "F";
        UserInfo user = (UserInfo)Session["user"];
        if (user != null)
        {
            DataTable dt = MobileReimburseManage.findRelatedReimburse(user, keyword);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt = PinYinHelper.SortByPinYin(dt, sort, order);
                foreach (DataRow dr in dt.Rows)
                {
                    dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
                }
                res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
            }
        }
        return res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
    }
}