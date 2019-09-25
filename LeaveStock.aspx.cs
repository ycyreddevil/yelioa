using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

public partial class LeaveStock : System.Web.UI.Page
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
                string start = Request.Form["dateStart"];
                string end = Request.Form["dateEnd"];
                Response.Write(getData(start, end));
            }
            else if (action == "import")
            {
                //Response.Write(Insert());
                Response.Write(Import());
            }
            else if(action == "upload")
            {
                //Response.Write(uploadFile());
            }
            else if (action == "showFile")
            {
                Response.Write(ShowFile());
            }
            else if (action == "delete")
            {
                Response.Write(delete());
            }
            else if (action == "getTemplate")
            {
                Response.Write(getTemplate());
            }
            else if (action == "searchProduct")
            {
                Response.Write(searchProduct());
            }
            else if (action == "searchOrganization")
            {
                Response.Write(searchOrganization());
            }
            else if (action == "saveAlias")
            {
                Response.Write(saveAlias());
            }
            Response.End();
        }
        else
        {
            action = Request.Params["act"];
            if (!string.IsNullOrEmpty(action))
            {
                Response.Clear();
                if (action == "getDataType")
                {
                    Response.Write(getDataType());
                }
                Response.End();
            }
        }
    }

    private string saveAlias()
    {
        string productCode = Request.Params["productCode"];
        string pAliasName = Request.Params["pAliasName"];
        string pAlisaSpecification = Request.Params["pAlisaSpecification"];
        string organizationCode = Request.Params["organizationCode"];
        string oAliasName = Request.Params["oAliasName"];
        string type = Request.Params["type"];
        string res = "";

        if(!string.IsNullOrEmpty(productCode))
        {
            res += ProductInfoManage.SaveAliasData(productCode, pAliasName, pAlisaSpecification, type);
        }
        res += ",";
        if (!string.IsNullOrEmpty(organizationCode))
        {
            res += OrganizationInfoManage.SaveAliasData(organizationCode,oAliasName,type);
        }
        return res;
    }

    private string searchProduct()
    {
        string res = "";
        string id = ((UserInfo)Session["user"]).companyId.ToString();
        string search = Request.Params["searchString"];
        DataTable dt = ProductInfoManage.GetInfos(id, search);
        if(dt!=null)
        {
            ArrayList list = new ArrayList();
            foreach(DataRow row in dt.Rows)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("id", row["code"]);
                dict.Add("text", row["name"] + "," + row["specification"]);
                list.Add(dict);
            }
            res = JsonHelper.SerializeObject(list);
        }
        return res;
    }

    private string searchOrganization()
    {
        string res = "";
        string search = Request.Params["searchString"];
        DataTable dt = OrganizationInfoManage.GetData(search);
        if (dt != null)
        {
            ArrayList list = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("id", row["code"]);
                dict.Add("text", row["name"]);
                list.Add(dict);
            }
            res = JsonHelper.SerializeObject(list);
        }
        return res;
    }

    private string getDataType()
    {
        string res = "";
        string needAll = Request.Params["needAll"];
        DataSet ds = LeaveStockInfoManage.GetDatalistInfo();
        if (ds != null)
        {
            if (needAll != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    DataRow dr = dt.NewRow();
                    dr["type"] = "全部";
                    dt.Rows.InsertAt(dr, 0);
                }
            }
            res = JsonHelper.DataTable2Json(ds.Tables[0]);
        }
        return res;
    }

    private string getData(string start, string end)
    {
        string json = "F";
        DateTime dateStart = Convert.ToDateTime(start);
        DateTime dateEnd = Convert.ToDateTime(end);
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string type = Request.Form["type"];

        DataSet ds = LeaveStockInfoManage.GetInfos(dateStart, dateEnd, type==null?"":type);
        if (ds != null)
        {
            DataTable dt = PinYinHelper.SortByPinYin(ds.Tables[0], sort, order);
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt, ds.Tables[1]);
        }
        return json;
    }

    private string ShowFile()
    {
        string res = "F";
        DataTable dt = ExcelHelperV2_0.Import(Request);

        if (dt!=null)
        {
            //补齐空缺的或省略的cell，第一行为空除外
            //dt = LeaveStockInfoManage.FillBlankCell(dt);
            dt.Columns.Add("状态");
            dt.Columns["状态"].SetOrdinal(0);//调整到第一列
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["状态"] = "未导入";
            }
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        return res;
    }



    private string Insert()
    {
        string res = "F";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            res = LeaveStockInfoManage.InsertInfos(ref dt, Request.Form["dataType"]);
            if(res.IndexOf("error:") != 0)
                res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        return res;
    }

    private string Import()
    {
        string res = "F";
        string json = Request.Form["json"];
        string type = Request.Form["type"];
        Dictionary<string, string> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(json);
        if(!dict["状态"].Contains("已导入"))
            dict = LeaveStockInfoManage.ImportInfos(dict,type);
        res = JsonHelper.SerializeObject(dict);
        return res;
    }

    private string delete()
    {
        string ids = Request.Form["ids"];
        return LeaveStockInfoManage.DeleteData(ids);
    }

    private string getTemplate()
    {
        string res = "";
        string type = Request.Form["type"];
        Dictionary < string, string> dict = LeaveStockInfoManage.GetTemplate(type);
        res = JsonHelper.SerializeObject(dict);
        return res;
    }
}