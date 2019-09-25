using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

public partial class BudgetDistributeList : System.Web.UI.Page
{
    private JObject jObject;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!"POST".Equals(Request.RequestType))
        {
            return;
        }

        jObject = HttpHelper.getAjaxData(Request);

        string action = jObject["action"].ToString();

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if (action == "getDepartmentDistributeList")
            {
                Response.Write(getDepartmentDistributeList());
            }

            Response.End();
        }
    }

    private string getDepartmentDistributeList()
    {
        UserInfo userInfo = (UserInfo)Session["user"];

        return SalesBudgetApplicationManage.GetSecondDoc(userInfo.userId.ToString());
    }
}