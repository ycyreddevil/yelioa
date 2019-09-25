using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mFormListAndDetail : System.Web.UI.Page
{
    public UserInfo userInfo;
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMobileReimbursement",
           "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
           "1000006",
        "http://yelioa.top/mFormListAndDetail.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        userInfo = (UserInfo)Session["user"];

        string action = Request.Form["act"];
        if(!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if(action=="getData")
            {
                Response.Write(GetData());
            }
            else if(action=="getDetail")
            {
                Response.Write(GetDetail());
            }
            else if(action=="back")
            {
                Response.Write(Back());
            }
            else if (action == "approve")
            {
                Response.Write(Approve());
            }
            Response.End();
        }
    }

    private string GetData()
    {
        string formName = Request.Form["formName"];
        UserInfo user = (UserInfo)Session["user"];

        JObject res = new JObject();

        res.Add("toBeSubmitedByMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 1));
        res.Add("toBeApprovedByMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 2));
        res.Add("submitedByMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 3));
        res.Add("hasApprovedByMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 4));
        res.Add("relatedToMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 5));

        return res.ToString();
    }
    private string GetDetail()
    {
        string formName = Request.Form["formName"];
        string docId = Request.Form["docId"];
        string type = Request.Form["type"];
        return WorkFlowHelper.GetDocumentDetail(formName, docId, ((UserInfo)Session["user"]).userId.ToString(), type);
    }
    private string Back()
    {
        string formName = Request.Form["formName"];
        string docId = Request.Form["docId"];
        UserInfo user = (UserInfo)Session["user"];
        return WorkFlowHelper.BackDocument(formName, docId, user.userId.ToString());
    }

    private string Approve()
    {
        string formName = Request.Form["formName"];
        string docId = Request.Form["docId"];
        string result = Request.Form["result"];
        string opinion = Request.Form["opinion"];
        string hospitalCode = Request.Form["hospitalCode"];
        string agentCode = Request.Form["agentCode"];

        UserInfo user = (UserInfo)Session["user"];

        return WorkFlowHelper.Approving(formName, docId, user.userId.ToString(), result, opinion, hospitalCode, agentCode);
    }
}