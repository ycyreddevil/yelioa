using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ImportPurchase : System.Web.UI.Page
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
            else if (action == "getCommonPurchaseData")
            {
                Response.Write(getCommonPurchaseData());
            }
            else if (action == "InsertOrUpdateCommonPurchaseData")
            {
                Response.Write(InsertOrUpdateCommonPurchaseData());
            }
           
            Response.End();
        }
    }

    private string getCommonPurchaseData()
    {
        string date = Request.Form["date"];

        DataTable dt = ImportPurchaseManage.findByMonth(date);

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string Import()
    {
        string res = "F";
        string json = Request.Form["json"];
        string type = Request.Form["type"];

        Dictionary<string, string> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(json);
        if (!dict["状态"].Contains("已导入"))
            dict = ImportPurchaseManage.ImportInfos(dict, type);

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

    private string InsertOrUpdateCommonPurchaseData()
    {
        string data = Request.Form["dataJson"];
        List<Dictionary<string, string>> dictList = JsonHelper.DeserializeJsonToObject<List<Dictionary<string, string>>>(data);
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string msg = ImportPurchaseManage.insertOrUpdateCommonPurchaseData(dictList, year, month);

        // 保存到日志表中
        UserInfo user = (UserInfo)Session["user"];
        ImportPurchaseManage.saveRecord(dictList, user.userName);

        JObject jObject = new JObject();

        jObject.Add("msg", msg);
        return jObject.ToString();
    }
}