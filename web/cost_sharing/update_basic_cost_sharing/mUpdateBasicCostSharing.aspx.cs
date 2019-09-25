using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bll.cost_sharing;
using Newtonsoft.Json.Linq;

public partial class web_cost_sharing_update_basic_cost_sharing_mUpdateBasicCostSharing : System.Web.UI.Page
{
    private JObject jObject;

    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
            "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc",
            "1000009",
            "http://yelioa.top/web/cost_sharing/update_basic_cost_sharing/mUpdateBasicCostSharing.aspx");
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

            if (action == "getFormColumnsAndData")
            {
                Response.Write(getFormColumnsAndData());
            }
            else if (action == "submissionOfCostSharingUpdating")
            {
                Response.Write(submissionOfCostSharingUpdating());
            }
            else if (action == "queryRelatedBranch")
            {
                Response.Write(queryRelatedBranch());
            }
            else if (action == "getRelativeProduct")
            {
                Response.Write(getRelativeProduct());
            }

            Response.End();
        }
    }

    private string getFormColumnsAndData()
    {
        UserInfo userInfo = (UserInfo) Session["user"];
        if (jObject["costSharingRecordId"] == null)
        {
            //提交
            return NewBranchUpdateManage.getFormColumnsAndDataForSubmit(jObject, userInfo);
        }
        else
        {
            //审批
            return NewBranchUpdateManage.getFormColumnsAndDataForApproval(jObject, 1);
        }
    }

    private string submissionOfCostSharingUpdating()
    {
        UserInfo userInfo = (UserInfo) Session["user"];

        if (jObject["costSharingRecordId"] == null)
        {
            //提交
            return NewBranchUpdateManage.submissionOfCostSharingUpdating(jObject, userInfo);
        }
        else
        {
            //审批
            return NewBranchUpdateManage.approvalOfCostSharingUpdating(jObject, userInfo);
        }
    }

    private string queryRelatedBranch()
    {
        UserInfo userInfo = (UserInfo)Session["user"];
        return SalesBudgetApplicationManage.getRelatedBranch(userInfo.userId.ToString());
    }

    private string getRelativeProduct()
    {
        UserInfo userInfo = (UserInfo)Session["user"];
        return SalesBudgetApplicationManage.getRelatedBranchProduct(userInfo.userId.ToString(), jObject["hospitalId"].ToString());
    }
}