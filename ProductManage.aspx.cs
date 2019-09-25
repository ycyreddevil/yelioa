using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ProductManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getData")
            {
                UserInfo user = (UserInfo)Session["user"];
                Response.Write(getData(user.companyId.ToString()));
            }
            else if (action == "del")
            {
                Response.Write(Delete());
            }
            else if (action == "validate")
            {
                string code = Request.Form["code"];
                Response.Write(ValidateProductCode(Request.Form["code"]));
            }
            else if (action == "getAliasData")
            {
                Response.ContentType = "application/json";
                Response.Write(getAliasData());
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
                if (action.Contains("add"))
                {
                    Response.Write(Add());
                }
                else if (action.Contains( "edit"))
                {
                    Response.Write(Edit());
                }
                else if (action == "getAliasData")
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

    private string uploadFile()
    {
        string res = "读取文件失败！";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            dt.Columns.Add("companyId");
            for(int i=0;i<dt.Rows.Count;i++)
            {
                dt.Rows[i]["companyId"] = GetUserInfo().companyId;
            }
            res = ProductInfoManage.SaveInfos(dt);
        }
        return res;
    }



    private string saveAliasData()
    {
        string json = "";
        string code = Request.Form["productCode"];
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("productCode", code);
        dict.Add("type", Request.Form["type"]);
        dict.Add("aliasCode", Request.Form["aliasCode"]);
        dict.Add("alias", Request.Form["alias"]);
        ProductInfoManage.InsertAliasData(dict);
        //DataSet ds = ProductInfoManage.GetAliasData(code);
        //if (ds != null)
        //    json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        return json;
    }

    private string updateAliasData()
    {
        string json = "";
        string code = Request.Form["productCode"];
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("productCode", code);
        dict.Add("type", Request.Form["type"]);
        dict.Add("aliasCode", Request.Form["aliasCode"]);
        dict.Add("alias", Request.Form["alias"]);
        ProductInfoManage.UpdateAliasData(dict, Request.Form["Id"]);
        return json;
    }

    private string destroyAliasData()
    {
        string json = "删除成功！";
        string id = Request.Form["Id"];
        ProductInfoManage.DeleteAliasData(id);
        return json;
    }

    private string getAliasData()
    {
        string json = "";
        string code = Request.Form["code"];
        if (string.IsNullOrEmpty(code))
        {
            code = Request.Params["code"];
        }
        DataSet ds = ProductInfoManage.GetAliasData(code);
        if (ds != null)
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        return json;
    }

    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }

    private string getData(string company)
    {
        string json = "";
        DataTable dt = ProductInfoManage.GetInfos( company, Request.Form["search"]);
        if(dt!=null)
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt).Trim();
        return json;
    }

    private string ValidateProductCode(string code)
    {
        string company = GetUserInfo().companyId.ToString();
        object res = ProductInfoManage.ValidateProductCode(company, code);
        if (res == null)
            return "";
        else
            return res.ToString();
        //if(string.IsNullOrEmpty(ProductInfoManage.ValidateProductCode(company,code)))
        //{
        //    return "T";
        //}
        //else
        //{
        //    return "F";
        //}
    }

    private string Add()
    {
        //System.Threading.Thread.Sleep(3000);
        string res = "添加成功！";
        //string name = Request.Form["name"];
        UserInfo user = (UserInfo)Session["user"];
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("code", Request.Form["code"]);
        dict.Add("name", Request.Form["name"]);
        dict.Add("fullName", Request.Form["fullName"]);
        dict.Add("specification", Request.Form["specification"]);
        if(!string.IsNullOrEmpty(Request.Form["shelfLife"]))//为空则加载默认值
        {
            dict.Add("shelfLife", Request.Form["shelfLife"]);
        }
        
        dict.Add("stockAccountCode", Request.Form["stockAccountCode"]);
        dict.Add("salesIncomeAccountCode", Request.Form["salesIncomeAccountCode"]);
        dict.Add("salesCostAccountCode", Request.Form["salesCostAccountCode"]);
        if (!string.IsNullOrEmpty(Request.Form["taxRate"]))//为空则加载默认值
        {
            dict.Add("taxRate", Request.Form["taxRate"]);
        }
        dict.Add("manufacturerLicenseNumber", Request.Form["manufacturerLicenseNumber"]);
        dict.Add("storageCondition", Request.Form["storageCondition"]);
        dict.Add("placeOfProduction", Request.Form["placeOfProduction"]);
        dict.Add("manufacturer", Request.Form["manufacturer"]);
        dict.Add("productLicenseNumber", Request.Form["productLicenseNumber"]);
        dict.Add("remark", Request.Form["remark"]);
        dict.Add("companyId", user.companyId.ToString());

        res = ProductInfoManage.InsertInfos(dict);
        return res;
    }

    private string Edit()
    {
        string res = "添加成功！";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("code", Request.Form["code"]);
        dict.Add("name", Request.Form["name"]);
        dict.Add("fullName", Request.Form["fullName"]);
        dict.Add("specification", Request.Form["specification"]);
        dict.Add("shelfLife", Request.Form["shelfLife"]);
        dict.Add("stockAccountCode", Request.Form["stockAccountCode"]);
        dict.Add("salesIncomeAccountCode", Request.Form["salesIncomeAccountCode"]);
        dict.Add("salesCostAccountCode", Request.Form["salesCostAccountCode"]);
        dict.Add("taxRate", Request.Form["taxRate"]);
        dict.Add("manufacturerLicenseNumber", Request.Form["manufacturerLicenseNumber"]);
        dict.Add("storageCondition", Request.Form["storageCondition"]);
        dict.Add("placeOfProduction", Request.Form["placeOfProduction"]);
        dict.Add("manufacturer", Request.Form["manufacturer"]);
        dict.Add("productLicenseNumber", Request.Form["productLicenseNumber"]);
        dict.Add("remark", Request.Form["remark"]);

        string id = Request.Form["Id"]; 

        res = ProductInfoManage.UpdateInfos(dict,id);
        return res;
    }

    private string Delete()
    {
        string id = Request.Form["id"];
        return ProductInfoManage.Delete( id);
    }
}