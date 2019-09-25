<%@ WebHandler Language="C#" Class="SearchOrganization" %>

using System;
using System.Web;
using System.Data;

public class SearchOrganization : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        //context.Response.Write("Hello World");
        context.Response.Clear();
        string searchString = context.Request.QueryString["search"];
        GetData(searchString);
    }

    private string GetData(string searchString)
    {
        string json="";

        DataTable dt = OrganizationInfoManage.GetData(searchString);
        if(dt != null)
        {
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt).Trim();
        }
        return json;
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}