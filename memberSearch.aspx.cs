using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class memberSearch : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getMembers")
            {
                Response.Write(getMembers());
            }
            else if (action == "search")
            {
                string search = Request.Form["searchString"];
                Search(search);
            }
            //else if (action == "check")
            //{
            //    CheckCookie();
            //}
            //else if (action == "logout")
            //{
            //    LogOut();
            //}
            //else if (action == "getValideCode")
            //{
            //    GetValideCode();
            //}
            Response.End();
        }
    }

    private string getMembers()
    {
        string json = "";
        UserInfo user = (UserInfo)Session["user"];
        DataTable dt = UserInfoManage.getInfos(user.companyId.ToString(), "1", Request.Form["searchString"]);
        if (dt != null)
        {
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        //DataSet ds = UserInfoManage.GetMembers(user);
        //json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        return json;
    }

    private string Search(string search)
    {
        string json = "";
        UserInfo user = (UserInfo)Session["user"];
        DataSet ds = UserInfoManage.SearchMembers(user,search);
        json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        return json;
    }
}