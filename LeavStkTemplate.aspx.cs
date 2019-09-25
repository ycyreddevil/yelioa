using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

public partial class LeavStkTemplate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getTable")
            {
                Response.Write(getTable());
            }
            else if (action == "getDatalist")
            {
                Response.Write(getDatalist());
            }
            else if (action == "add")
            {
                Response.Write(add());
            }
            else if (action == "edit")
            {
                Response.Write(edit());
            }
            else if (action == "getFormDetail")
            {
                Response.Write(getFormDetail());
            }
            else if (action == "CheckInfo")
            {
                Response.Write(CheckInfo());
            }
            else if (action == "delete")
            {
                Response.Write(delete());
            }
            Response.End();
        }
    }

    private string getTable()
    {
        string res = "";
        DataSet ds = LeaveStockInfoManage.GetTemplateInfo();
        if(ds !=null)
        {
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        }
        return res;
    }

    private string getDatalist()
    {
        string res = "";
        DataSet ds = LeaveStockInfoManage.GetDatalistInfo();
        if (ds != null)
        {
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        }
        return res;
    }

    private string delete()
    {
        return LeaveStockInfoManage.DeleteTemplate(Request.Form["type"]);
    }

    private string add()
    {
        string res = "";
        DataSet ds = LeaveStockInfoManage.GetTemplateInfo();
        if (ds != null)
        {
            ArrayList list = new ArrayList();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string val = Request.Form[row["field"].ToString()];
                dict.Add("field", row["field"].ToString());
                dict.Add("alias", val);
                dict.Add("type", Request.Form["type"]);
                list.Add(dict);
            }
            res = LeaveStockInfoManage.InsertTemplate(list);
        }           
        return res;
    }

    private string edit()
    {
        string res = "";
        DataSet ds = LeaveStockInfoManage.GetTemplateInfo();
        if (ds != null)
        {
            ArrayList list = new ArrayList();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string val = Request.Form[row["field"].ToString()];
                dict.Add("field", row["field"].ToString());
                dict.Add("alias", val);
                dict.Add("type", Request.Form["type"]);
                list.Add(dict);
            }
            res = LeaveStockInfoManage.UpdateTemplate(list, Request.Form["type"]);
        }
        return res;
    }

    private string getFormDetail()
    {
        string res = "";
        Dictionary<string, string> dict = LeaveStockInfoManage.GetFormDetail(Request.Form["dataType"]);
        res = JsonHelper.SerializeObject(dict);
        return res;
    }

    private string CheckInfo()
    {
        bool isEdit = false;
        if (Request.Form["state"] == "edit")
        {
            isEdit = true;
        }
        DataSet ds = LeaveStockInfoManage.CheckInfo(Request.Form["dataType"]);
        int standartNumber = 0;//插入模式不允许查询到记录
        if (isEdit)
        {
            standartNumber = 1;//编辑模式最多查询到1条记录
        }
        string res = "数据库读取失败!";
        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count > standartNumber)
            {
                res += "数据模板名称已存在，请重新输入!";
            }
            else
            {
                res = "T";
            }
        }

        return res;
    }
}