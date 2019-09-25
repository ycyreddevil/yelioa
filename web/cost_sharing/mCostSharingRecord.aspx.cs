using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class web_cost_sharing_mCostSharingRecord : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["action"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "GetList")
            {
                Response.Write(GetList());
            }
            
            Response.End();
        }
    }
    private string GetList()
    {
        UserInfo user = (UserInfo)Session["user"];
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        return CostSharingRecordManage.GetList(user.userId.ToString(),year,month);
    }
}