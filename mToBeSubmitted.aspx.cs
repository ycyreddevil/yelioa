using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mToBeSubmitted : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mToBeSubmitted.aspx",
                    "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
                    "1000006",
                    "http://yelioa.top/mToBeSubmitted.aspx");
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
            if (action == "getDocument")
            {
                Response.Write(getDocument());
            }
            else if (action == "getList")
            {
                Response.Write(getList());
            }

            Response.End();
        }
    }

    private string getDocument()
    {
        string code = Request.Form["code"];
        DataTable dt = ReimbursementManage.findByCode(code);
        foreach (DataRow dr in dt.Rows)
        {
            dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
        }
        return JsonHelper.DataTable2Json(dt);
    }

    private string getList()
    {
        UserInfo userInfo = (UserInfo)Session["user"];

        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string keyWord = Request.Form["keyword"];

        DataTable dt = ReimbursementManage.GetDocumnetsInfosToBeSubmitedByMe(userInfo.userName, keyWord).Tables[0];

        string res = "F";

        if (dt != null && dt.Rows.Count > 0)
        {
            dt = PinYinHelper.SortByPinYin(dt, sort, order);
            foreach (DataRow dr in dt.Rows)
            {
                dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
            }
            res = JsonHelper.DataTable2Json(dt);
        }

        return res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
    }
}