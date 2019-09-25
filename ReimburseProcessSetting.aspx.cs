using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ReimburseProcessSetting : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getTree")
            {
                Response.Write(getTree());
            }
            Response.End();
        }
    }

    private string getTree()
    {
        DataSet ds = UserInfoManage.getTree(GetUserInfo().companyId.ToString());
        if (ds == null)
        {
            return "F";
        }
        UserTreeHelper tree = new UserTreeHelper(ds.Tables[0]);
        string json = tree.GetJson();
        return json;
    }

    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }
}