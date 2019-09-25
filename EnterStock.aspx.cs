using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class EnterStock : System.Web.UI.Page
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
                Response.Write(getData(start, end, user.companyId.ToString()));
            }
            else if (action == "import")
            {
                Response.Write(Insert());
            }
            else if (action == "showFile")
            {
                Response.Write(ShowFile());
            }
            else if (action == "upload")
            {
                Response.Write(uploadFile());
            }
            else if (action == "delete")
            {
                Response.Write(delete());
            }
            Response.End();
        }
    }

    private string Insert()
    {
        string res = "";
        string jsonData = Request.Form["data"];
        DataTable dt = JsonHelper.Json2Dtb(jsonData);
        return res;
    }
    private string getData(string start, string end, string company)
    {
        string json = "F";
        DateTime dateStart = Convert.ToDateTime(start);
        DateTime dateEnd = Convert.ToDateTime(end);
        DataSet ds = EnterStockInfoManage.GetInfos(dateStart,dateEnd, company);
        if(ds !=null)
        {
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        }        
        return json;
    }

    private string ShowFile()
    {
        string res = "F";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            dt.Columns.Add("状态");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["状态"] = "未导入";
            }
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        return res;
    }



    private string uploadFile()
    {
        string res = "";
        try
        {
            HttpFileCollection httpFileCollection = Request.Files;
            HttpPostedFile file = null;
            if (httpFileCollection.Count <= 0)
            {
                res = "文件上传未成功";
            }
            else
            {
                file = httpFileCollection[0];

                //Excel读取
                DataTable dt = ExcelHelperV2_0.Import(file.InputStream);
                UserInfo user = (UserInfo)Session["user"];
                DataTable newDt = EnterStockInfoManage.InsertInfos(dt, user.companyId.ToString());
                
                res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(newDt.Rows.Count, newDt);
            }            
        }
        catch(Exception ex)
        {
            res = ex.ToString();
        }

        return res;
    }

    private string delete()
    {
        string ids = Request.Form["ids"];
        return EnterStockInfoSrv.DeleteData(ids);
    }
}