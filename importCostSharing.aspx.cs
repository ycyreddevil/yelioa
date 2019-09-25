using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class importCostSharing : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "ShowFile")
            {
                Response.Write(ShowFile());
            }
            Response.End();
        }
    }

    private string ShowFile()
    {
        string res = "F";
        DataTable dt = CostSharingInfoManage.importCostSharing(Request, true);

        if (dt != null)
        {
            dt.Columns.Add("状态");
            dt.Columns["状态"].SetOrdinal(0);//调整到第一列
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["状态"] = "导入失败";
            }
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        return res;
    }
}