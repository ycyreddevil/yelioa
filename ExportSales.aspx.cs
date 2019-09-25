using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ExportSales : System.Web.UI.Page
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
            else if (action == "import")
            {
                Response.Write(Import());
            }

            Response.End();
        }
    }

    private string Import()
    {
        string res = "F";
        string json = Request.Form["json"];
        string type = Request.Form["type"];

        Dictionary<string, string> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(json);
        if (!dict["状态"].Contains("已导入"))
            dict = ExportSalesManage.ImportInfos(dict, type);

        if (dict != null)
            res = JsonHelper.SerializeObject(dict);
        return res;
    }

    private string ShowFile()
    {
        string res = "F";
        DataTable dt = ExcelHelperV2_0.Import(Request);

        // 最后一行是统计行 需删除
        dt.Rows.RemoveAt(dt.Rows.Count - 1);

        dt = LeaveStockInfoManage.FillBlankCell(dt, "过帐");

        if (dt != null)
        {
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
}