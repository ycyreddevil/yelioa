using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class OrganizationMng : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getData")
            {
                Response.Write(getData());
            }
            else if (action == "add")
            {
                Response.Write(Add());
            }
            else if (action == "edit")
            {
                Response.Write(Edit()); 
            }
            else if (action == "del")
            {
                Response.Write(Delete());
            }
            else if (action == "checkCode")
            {
                Response.Write(checkCode());
            }
            else if (action == "upload")
            {
                Response.Write(uploadFile());
            }
            Response.End();
        }
        else
        {
            action = Request.Params["act"];
            if (!string.IsNullOrEmpty(action))
            {
                Response.Clear();
                if (action == "getAliasData")
                {
                    Response.ContentType = "application/json";
                    Response.Write(getAliasData());
                }
                else if (action == "saveAliasData")
                {
                    Response.ContentType = "application/json";
                    Response.Write(saveAliasData());
                }
                else if (action == "updateAliasData")
                {
                    Response.ContentType = "application/json";
                    Response.Write(updateAliasData());
                }
                else if (action == "destroyAliasData")
                {
                    Response.ContentType = "application/json";
                    Response.Write(destroyAliasData());
                }
                Response.End();
            }
        }
    }

    private string Add()
    {
        string res = "";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("name", Request.Form["name"]);
        dict.Add("type", Request.Form["type"]);
        dict.Add("rank", Request.Form["rank"]);
        dict.Add("remark", Request.Form["remark"]);
        dict.Add("province", Request.Form["province"]);
        dict.Add("city", Request.Form["city"]);
        dict.Add("fullName", Request.Form["province"] + "_" + Request.Form["city"] + "_" + Request.Form["name"]);
        String code = GenerateOrgCode.generateCode(Request.Form["province"], Request.Form["city"], Request.Form["rank"], Request.Form["type"]);
        dict.Add("code", code);
        res = OrganizationInfoManage.InsertInfo(dict);
        return res;
    }

    private string Edit()
    {
        string res = "";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        //dict.Add("code", Request.Form["code"]);
        dict.Add("name", Request.Form["name"]);
        //dict.Add("fullName", Request.Form["fullName"]);
        dict.Add("type", Request.Form["type"]);
        dict.Add("rank", Request.Form["rank"]);
        dict.Add("remark", Request.Form["remark"]);
        dict.Add("province", Request.Form["province"]);
        dict.Add("city", Request.Form["city"]);
        dict.Add("fullName", Request.Form["province"] + "_" + Request.Form["city"] + "_" + Request.Form["name"]);
        String code = GenerateOrgCode.generateCode(Request.Form["province"], Request.Form["city"], Request.Form["rank"], Request.Form["type"]);
        dict.Add("code", code);
        res = OrganizationInfoManage.UpdateInfo(dict, Request.Form["Id"]);
        return res;
    }

    private string Delete()
    {
        string res = "";
        res = OrganizationInfoManage.DeleteInfo(Request.Form["Id"]);
        return res;
    }

    private string checkCode()
    {
        if (string.IsNullOrEmpty(OrganizationInfoManage.checkCode(Request.Form["code"])))
        {
            return "T";
        }
        else
        {
            return "F";
        }
    }

    private string saveAliasData()
    {
        string json = "";
        string code = Request.Form["code"];
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("code", code);
        dict.Add("type", Request.Form["type"]);
        dict.Add("aliasCode", Request.Form["aliasCode"]);
        dict.Add("alias", Request.Form["alias"]);
        OrganizationInfoManage.InsertAliasData(dict);
        return json;
    }

    private string updateAliasData()
    {
        string json = "";
        string code = Request.Form["code"];
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("code", code);
        dict.Add("type", Request.Form["type"]);
        dict.Add("aliasCode", Request.Form["aliasCode"]);
        dict.Add("alias", Request.Form["alias"]);
        OrganizationInfoManage.UpdateAliasData(dict, Request.Form["Id"]);
        return json;
    }

    private string destroyAliasData()
    {
        string json = "删除成功！";
        string id = Request.Form["Id"];
        OrganizationInfoManage.DeleteAliasData(id);
        return json;
    }

    private string getAliasData()
    {
        string json = "";
        string code = Request.Params["code"];
        DataSet ds = OrganizationInfoManage.GetAliasData(code);
        if (ds != null)
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        return json;
    }

    private string getData()
    {
        string json = "";
        string searchString = Request.Form["search"];
        //if (searchString == "undefine")
        //{
        //    searchString = "";
        //}
        DataTable dt = OrganizationInfoManage.GetData(searchString);
        if(dt != null)
        {
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt).Trim();
            //json = json.Replace("\r", "");
            //json = json.Replace("\n", "");
        }
        return json;
    }

    private string uploadFile()
    {
        string res = "读取文件失败！";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            res = OrganizationInfoManage.SaveInfos(dt);
        }
        return res;
    }


}