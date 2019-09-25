using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ApplicationManageManage 的摘要说明
/// </summary>
public class ApplicationManageManage
{
    public ApplicationManageManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    public static string initDatagrid()
    {
        string msg = "";
        JObject res = new JObject();
        DataSet ds = ApplicationManageSrv.initDatagrid(ref msg);
        if(ds==null)
        {
            res.Add("ErrCode", "1");
            res.Add("ErrMsg", msg);
        }
        else
        {
            res.Add("ErrCode", "0");
            res.Add("ErrMsg","操作成功");
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                if (row["IsValid"].ToString()=="1")
                    row["IsValid"] = "启用";
                else
                    row["IsValid"] = "禁用";
            }
            res.Add("document", JsonHelper.DataTable2Json(ds.Tables[0]));
        }
        return res.ToString();           
    }
    public static string sure(string id, string application, string isValid)
    {
        string msg = "";
        JObject res = new JObject();
        if (isValid == "启用")
            isValid = "1";
        else
            isValid = "0";
        msg = ApplicationManageSrv.sure(id,application,isValid);
        if (msg.Contains("操作成功"))
        {
            res.Add("ErrCode", "0");
            res.Add("ErrMsg", "操作成功");
        }
        else
        {
            res.Add("ErrCode", "0");
            res.Add("ErrMsg", "操作失败");
        }
        
        return res.ToString();
    }
}