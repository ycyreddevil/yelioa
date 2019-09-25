using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class NewBranchRegistration : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["action"];
        if(!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if(action== "getApprovalDataDetail")
            {
                Response.Write(GetApprovalDataDetail());
            }
            else if (action == "submitData")
            {
                Response.Write(SubmitData());
            }
            Response.End();
        }
    }
    private string GetApprovalDataDetail()
    {
        UserInfo user = (UserInfo)Session["user"];
        string docCode = Request.Form["docCode"];
        string IsRecord = Request.Form["IsRecord"];
        return NewBranchRegistrationManage.IsAddOrApproveOrRecord(docCode, user.userId.ToString(), IsRecord);
    }
    private string SubmitData()
    {
        UserInfo user = (UserInfo)Session["user"];
        JArray fieldList =JsonHelper.DeserializeJsonToObject<JArray> (Request.Form["fieldList"].ToString());
        string docCode = Request.Form["docCode"];

        return NewBranchRegistrationManage.Submit(docCode,fieldList,user.userId.ToString());
    }
}