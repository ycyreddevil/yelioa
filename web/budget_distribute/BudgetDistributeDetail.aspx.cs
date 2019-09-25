using System;
using System.Web;
using Newtonsoft.Json.Linq;

public partial class BudgetDistributeDetail : System.Web.UI.Page
{
    private JObject jObject;

    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("BudgetSubmitDetail",
          "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
          "1000006",
          "http://yelioa.top/web/budget_submit/BudgetDistributeDetail.aspx");
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

            if (action == "getDepartmentDistributeDetail")
            {
                Response.Write(getDepartmentDistributeDetail());
            }
            else if (action == "distributeBudget")
            {
                Response.Write(distributeBudget());
            }

            Response.End();
        }
    }

    public string getDepartmentDistributeDetail()
    {
        UserInfo userInfo = (UserInfo)Session["user"];

        return SalesBudgetApplicationManage.GetDistributeChildren(userInfo.userId.ToString(),jObject["BudgetLimitType"].ToString(),jObject["departmentId"].ToString(), jObject["isFinished"].ToString());
    }

    public string distributeBudget()
    {
        UserInfo userInfo = (UserInfo)Session["user"];

        //int firstOrSecondDistribute = Int32.Parse(jObject["firstOrSecondDistribute"].ToString());

        JArray jarray = JsonHelper.DeserializeJsonToObject<JArray>(jObject["tableData"].ToString());

        //if (firstOrSecondDistribute == 0)
        //{
        //    // 第一次分配
        //    return SalesBudgetApplicationManage.Distribute(jarray, jObject["BudgetLimitType"].ToString(), userInfo.userId.ToString(), jObject["docNo"].ToString());
        //}
        //else
        //{
            // 第二次分配
            return SalesBudgetApplicationManage.SecondDistrbute(jarray, jObject["BudgetLimitType"].ToString(), userInfo, jObject["docNo"].ToString());
        //}
    }
}