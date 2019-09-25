using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class SalesTaskManage : System.Web.UI.Page
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
            else if (action == "import")
            {
                Response.Write(Import());
                //Response.Write("");                
            }
            else if (action == "showFile")
            {
                Response.Write(ShowFile());
            }
            Response.End();
        }
    }

    private string getInfos()
    {
        string json = "";
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        DataTable dt = SalesTaskInfoManage.getInfos();
        dt = PinYinHelper.SortByPinYin(dt, sort, order);
        if (dt != null)
        {
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt).Trim();
        }
        return json;
    }

    private string Import()
    {
        string res = "F";
        string json = Request.Form["json"];
        if (string.IsNullOrEmpty(json))
            return "";
        Dictionary<string, string> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(json);
        if (!dict["状态"].Contains("已导入"))
        {
            dict = SalesTaskInfoManage.ImportInfos(dict);
            res = JsonHelper.SerializeObject(dict);
        }
        else
            res = json;
            
        return res;
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
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        return res;
    }
}