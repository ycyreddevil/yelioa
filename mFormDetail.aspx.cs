using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mFormDetail : System.Web.UI.Page
{
    public string post;
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
                    "E26TbitJpOlsniJaKMq6lrNYhiu1bKVtRddflNwIsoE",
                    "1000015",
                    "http://yelioa.top/mFormDetail.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        UserInfo userInfo = (UserInfo)Session["user"];
        post = SqlHelper.Find("select post from users where userId = '" + userInfo.userId + "'").Tables[0].Rows[0][0].ToString();

        string action = Request.Form["act"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "findFormById")
            {
                Response.Write(findFormById());
            }
            else if (action == "findRemoteUrl")
            {
                Response.Write(findRemoteUrl());
            }
            else if(action== "getProcessInfo")
            {
                Response.Write(getProcessInfo());
            }
            else if (action == "submitForm")
            {
                Response.Write(submitForm());
            }
            else if (action == "showDraftData")
            {
                Response.Write(showDraftData());
            }

            Response.End();
        }
    }

    private string findFormById()
    {
        string id = Request.Form["id"];
        UserInfo userInfo = (UserInfo) Session["user"];
        return FormBuilderHelper.findDetail(id, userInfo.userId.ToString());
    }

    private string findRemoteUrl()
    {
        string type = Request.Form["type"];
        string value = Request.Form["value"];
        string relatedType = Request.Form["relatedType"];
        string relatedValue = Request.Form["relatedValue"];

        return WorkFlowHelper.find(type,value,relatedType,relatedValue);
    }

    private string getProcessInfo()
    {
        string formData = Request.Form["formData"];       
        string formId = Request.Form["formId"];

        return WorkFlowHelper.GetApprover(formId, formData);
    }
    private string submitForm()
    {
        UserInfo userInfo = (UserInfo)Session["user"];

        string formData = Request.Form["formData"];
        string formName = Request.Form["formName"];

        string id = Request.Form["id"];

        JObject dt = JsonHelper.Json2DtbByLQ(formData);

        dt.Add("Level", "1");
        dt.Add("Status", "审批中");
        string docCode = GenerateDocCode.getYLFormCode(formName);
        dt.Add("DocCode", docCode);
        DateTime date = System.DateTime.Now;
        dt.Add("CreateTime", date.ToString());

        string informerJSON = Request.Form["defaultInformer"];
        string processJSON = Request.Form["processJson"];
        return FormBuilderHelper.saveFormRecord(formName, dt, processJSON, informerJSON, id, userInfo, docCode);
    }
    private DepartmentPost getDepartmentPost()
    {
        return ((List<DepartmentPost>)Session["DepartmentPostList"])[0];
    }

    private string showDraftData()
    {
        string id = Request.Form["id"];
        string formName = Request.Form["formName"];

        return FormBuilderHelper.showDraftData(id, formName);
    }
}