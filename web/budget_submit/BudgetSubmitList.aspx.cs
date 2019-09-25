using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

public partial class web_budget_submit_BudgetSubmitList : System.Web.UI.Page
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

            if (action == "getDepartmentSubmitList")
            {
                Response.Write(getDepartmentSubmitList());
            }

            Response.End();
        }
    }

    private string getDepartmentSubmitList()
    {
        UserInfo userInfo = (UserInfo) Session["user"];

        return SalesBudgetApplicationManage.GetFormData(userInfo.userId.ToString());
    }
}