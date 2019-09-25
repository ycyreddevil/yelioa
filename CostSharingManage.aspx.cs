using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

public partial class CostSharingManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getInfos")
            {
                Response.Write(getInfos());
            }
            else if (action == "getDatalist")
            {
                Response.Write(getDatalist());
            }
            else if (action == "upload")
            {
                Response.Write(uploadFile());
            }
            else if (action == "add")
            {
                Response.Write(Add());
            }
            else if (action == "edit")
            {
                Response.Write(Edit());
            }
            else if (action == "showFile")
            {
                Response.Write(ShowFile());
            }
            else if (action == "CheckRepitition")
            {
                Response.Write(CheckRepitition());
            }
            else if (action == "import")
            {
                Response.Write(Import());
                //Response.Write("");                
            }
            Response.End();
        }
    }

    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }

    private string getInfos()
    {
        string json = "";
        string searchString = Request.Form["searchString"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        DataTable dt = CostSharingInfoManage.getInfos(searchString);
        dt = PinYinHelper.SortByPinYin(dt, sort, order);
        if (dt != null)
        {
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt).Trim();
        }
        return json;
    }

    private string getDatalist()
    {
        string json = "";
        string type = Request.Form["type"];
        string searchString = Request.Form["searchString"];
        if (type == "hospital")
        {
            DataTable dt = OrganizationInfoManage.GetData(searchString);
            if (dt != null)
            {
                json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt).Trim();
            }
        }
        else if (type == "product")
        {
            DataTable dt = ProductInfoManage.GetInfos(GetUserInfo().companyId.ToString(), searchString);
            if (dt != null)
                json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt).Trim();
        }
        else if (type == "sales" || type == "supervisor" || type == "manager" || type == "director")
        {
            DataTable dt = UserInfoManage.getInfos(GetUserInfo().companyId.ToString(), "", searchString);
            dt.Columns["userId"].ColumnName = "Id";
            dt.Columns["userName"].ColumnName = "name";
            if (dt != null)
            {
                json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
            }
        }
        
        return json;
    }

    private string getUserTree()
    {
        DataSet ds = UserInfoManage.getTree(GetUserInfo().companyId.ToString());
        if (ds == null)
        {
            return "F";
        }
        UserTreeHelper tree = new UserTreeHelper(ds.Tables[0]);
        string json = tree.GetJson();
        return json;
    }

    private string uploadFile()
    {
        string res = "读取文件失败！";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            res = CostSharingInfoManage.InsertInfos(dt);
        }
        return res;
    }



    private string Add()
    {
        string res = "添加失败！";
        DataSet ds = SqlHelper.GetFiledNameAndComment("cost_sharing");
        string[] specils = new string[] { "HospitalId", "ProductId", "SalesId", "SupervisorId", "ManagerId", "DirectorId" };
        if (ds != null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string field = row["field"].ToString();
                if (field == "Id")
                {
                    continue;
                }
                string value = "";
                if (specils.Contains(field))
                {
                    value = Request.Form[field.Substring(0,field.Length-2)];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    string[] strs = value.Split(',');
                    if(strs.Length != 2)
                    {
                        continue;
                    }
                    value = strs[1];
                }
                else if(field == "DepartmentId" || field == "SectorId")
                {
                    value = Request.Form[field.Substring(0, field.Length - 2)];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                }
                else
                {
                    value = Request.Form[field];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                }
                dict.Add(field, value);
            }
            res = CostSharingInfoManage.InsertInfos(VerifyDict(dict));
        }
        return res;
    }

    private Dictionary<string, string> VerifyDict(Dictionary<string, string> dict)
    {
        ArrayList list = new ArrayList();
        list.AddRange(dict.Keys);
        foreach(object obj in list)
        {
            string key = obj.ToString();
            if (string.IsNullOrEmpty(dict[key]))
                dict[key] = "0";
        }
        return dict;
    }

    private string Edit()
    {
        string res = "更新失败！";
        DataSet ds = SqlHelper.GetFiledNameAndComment("cost_sharing");
        string[] specils = new string[] { "Hospital", "Product", "Sales", "Supervisor", "Manager", "Director" };
        if (ds != null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string field = row["field"].ToString();
                if (field == "Id")
                {
                    continue;
                }
                string value = "";
                if (specils.Contains(field))
                {
                    value = Request.Form[field.Substring(0, field.Length - 2)];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    string[] strs = value.Split(',');
                    if (strs.Length != 2)
                    {
                        continue;
                    }
                    value = strs[1];
                }
                else if (field == "DepartmentId" || field == "SectorId")
                {
                    value = Request.Form[field.Substring(0, field.Length - 2)];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                }
                else
                {
                    value = Request.Form[field];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                }
                dict.Add(field, value);
            }
            string id = Request.Form["Id"];

            res = CostSharingInfoManage.UpdateInfos(VerifyDict(dict), id);
        }
        return res;
    }

    private string ShowFile()
    {
        string res = "F";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            dt.Columns.Add("状态");
            dt.Columns["状态"].SetOrdinal(0);//调整到第一列
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["状态"] = "未导入";
            }
            //dt = CheckData(dt);
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        return res;
    }

    private string CheckRepitition()
    {
        string res = "F";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            dt.Columns.Add("状态");
            dt.Columns["状态"].SetOrdinal(0);//调整到第一列
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["状态"] = "未导入";
            }
            dt = CheckData(dt);
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        return res;
    }

    private string Import()
    {
        string res = "F";
        string json = Request.Form["json"];
        string index = Request.Form["index"];
        Dictionary<string, string> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(json);
        if (!dict["状态"].Contains("已导入"))
            dict = CostSharingInfoManage.NewImportInfos(dict, index);
            //dict = CostSharingInfoManage.ImportInfos(dict);
        res = JsonHelper.SerializeObject(dict);
        return res;
    }

    private DataTable CheckData(DataTable dt)
    {
        string[] columns = new string[] { "代表", "盈利中心经理", "医院", "产品", "医院供货价","科室" };
        DataTable dt2 = dt.DefaultView.ToTable(true, columns);
        //DataTable res = dt.Clone();
        for(int i=dt.Rows.Count-1;i>=0;i--)
        {
            DataRow row = dt.Rows[i];
            for (int j = dt2.Rows.Count - 1; j >= 0; j--)
            {
                if(object.Equals(row["代表"],dt2.Rows[j]["代表"])&& object.Equals(row["医院供货价"], dt2.Rows[j]["医院供货价"])
                    && object.Equals(row["医院"], dt2.Rows[j]["医院"])&& object.Equals(row["产品"], dt2.Rows[j]["产品"])
                    && object.Equals(row["盈利中心经理"], dt2.Rows[j]["盈利中心经理"]) 
                    && object.Equals(row["科室"], dt2.Rows[j]["科室"]))
                {
                    dt.Rows.RemoveAt(i);
                    dt2.Rows.RemoveAt(j);
                    break;
                }
            }
        }
        return dt;
    }
}