using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class web_cost_sharing_delete_cost_sharing_mCostSharingDelete : System.Web.UI.Page
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
            else if (action == "submitData")
            {
                Response.Write(SubmitData());
            }
            Response.End();
        }
    }
    private string GetList()
    {
        UserInfo user = (UserInfo)Session["user"];
        string docId = Request.Form["docId"];
        string docCode = Request.Form["docCode"];
        string IsRecord = Request.Form["IsRecord"];
        return CostSharingDeleteManage.GetList(user.userId.ToString(), docId, docCode, IsRecord);
    }
    private string SubmitData()
    {
        UserInfo user = (UserInfo)Session["user"];
        string docId = Request.Form["docId"];
        string docCode = Request.Form["docCode"];
        JArray jArray =JArray.Parse(Request.Form["firstList"].ToString());
        return CostSharingDeleteManage.SubmitOrApprove(user.userId.ToString(),  docCode,docId, jArray);
    }
}