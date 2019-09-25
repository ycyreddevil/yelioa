using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

public partial class web_budget_submit_BudgetSubmitDetail : System.Web.UI.Page
{
    private JObject jObject;

    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("BudgetSubmitDetail",
           "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
           "1000006",
           "http://yelioa.top/web/budget_submit/BudgetSubmitDetail.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        if (!"POST".Equals(Request.RequestType))
        {
            return;
        }

        jObject = HttpHelper.getAjaxData(Request);

        string action = jObject["action"].ToString();

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if (action == "submitBudget")
            {
                Response.Write(submitBudget());
            }
            else if (action == "getBudgetDetail")
            {
                Response.Write(getBudgetDetail());
            }

            Response.End();
        }
    }

    private string submitBudget()
    {
        string departmentId = jObject["selfDepartmentId"].ToString();
        string docNo = jObject["docNo"].ToString();
        string tableData = jObject["tableData"].ToString();
        JArray jarray = JsonHelper.DeserializeJsonToObject<JArray>(tableData);

        UserInfo userInfo = (UserInfo)Session["user"];

        return SalesBudgetApplicationManage.Submit(jarray, docNo, departmentId, userInfo.userId.ToString(), userInfo.userName.ToString());
    }

    private string getBudgetDetail()
    {
        string departmentId = jObject["departmentId"].ToString();
        string selfDepartmentId = jObject["selfDepartmentId"].ToString();
        UserInfo userInfo = (UserInfo)Session["user"];
        string docNo = jObject["docNo"].ToString();

        return SalesBudgetApplicationManage.GetDetail(departmentId, selfDepartmentId, userInfo.userId.ToString(), docNo);
    }
}