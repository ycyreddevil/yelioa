using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

public partial class OrganizationAliasManage : System.Web.UI.Page
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
            else if (action == "delete")
            {
                Response.Write(delete());
            }
            else if (action == "upload")
            {
                Response.Write(uploadFile());
            }
            Response.End();
        }
    }

    private string getData()
    {
        string res = "";
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        DataSet ds = OrganizationInfoManage.GetAliasData();

        if (ds != null)
        {
            if (string.IsNullOrEmpty(sort))
                res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
            else
            {
                DataTable dt = PinYinHelper.SortByPinYin(ds.Tables[0], sort, order);
                res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
            }
        }
        return res;
    }

    private string delete()
    {
        string ids = Request.Form["ids"];
        return OrganizationInfoManage.DeleteAliasData(ids);
    }

    private string uploadFile()
    {
        string res = "读取文件失败！";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            res = OrganizationInfoManage.InsertAliasData(dt);
        }
        return res;
    }


}