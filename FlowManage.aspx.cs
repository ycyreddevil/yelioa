using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using Newtonsoft.Json.Linq;

public partial class FlowManage : System.Web.UI.Page
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
            else if (action == "archive")
            {
                Response.Write(archive());
            }
            else if (action == "dataGridSort")
            {
                Response.Write(dataGridSort());
            }
            //else if (action == "add")
            //{
            //    Response.Write(Add());
            //}
            //else if (action == "edit")
            //{
            //    Response.Write(Edit());
            //}
            else if (action == "showFile")
            {
                Response.Write(ShowFile());
            }
            else if (action == "import")
            {
                Response.Write(Import());
                //Response.Write("");                
            }
            Response.End();
        }
    }

    private string dataGridSort()
    {
        string data = Request.Form["data"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        DataTable dt = JsonHelper.DeserializeJsonToObject<DataTable>(data);
        dt = PinYinHelper.SortByPinYin(dt, sort, order);
        return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
    }

    private string getInfos()
    {
        string date = Request.Form["date"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string json = "";
        bool DataIsArchived = false;
        DataSet ds = FlowInfoManage.GetInfos(date,ref DataIsArchived);
        if(ds != null)
        {
            DataTable dt = PinYinHelper.SortByPinYin(ds.Tables[0], sort, order);
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt,ds.Tables[1]);
        }
        JObject res = new JObject();
        res.Add("DataIsArchived", DataIsArchived);
        res["Data"] = JsonHelper.DeserializeJsonToObject<JObject>(json);
        //res.Add("Data", JsonHelper.DeserializeJsonToObject<object>(json));
        //Dictionary<string, object> resDict = new Dictionary<string, object>();
        //resDict.Add("DataIsArchived", DataIsArchived);
        //resDict.Add("Data", json);
        //json = JsonHelper.SerializeObject(resDict);
        return res.ToString();
    }

    private string archive()
    {
        string date = Request.Form["date"];
        string jsonData = Request.Form["dataJson"];
        return FlowInfoManage.ArchiveData(date, jsonData);
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

    private string Import()
    {
        string res = "F";
        string json = Request.Form["json"];
        string dateString = Request.Form["dateString"];
        if (string.IsNullOrEmpty(dateString))
            return "请选择正确的归档日期";
        
        Dictionary<string, string> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(json);
        if (!dict["状态"].Contains("已导入"))
            dict = FlowInfoManage.ImportInfos(dict,dateString);
        res = JsonHelper.SerializeObject(dict);
        return res;
    }
}