using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ApplicationManageSrv 的摘要说明
/// </summary>
public class ApplicationManageSrv
{
    public ApplicationManageSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    public static DataSet initDatagrid(ref string msg)
    {
        string sql = "select * from yl_application";
        return SqlHelper.Find(sql, ref msg);
    }

    public static string sure(string id,string application,string isValid)
    {
        string sql = "";
        if(!string.IsNullOrEmpty(id))
        {
            sql = string.Format("update yl_application set application='{0}',IsValid='{1}' where Id='{2}'", application, isValid, id);
        }
        else
        {
            sql = string.Format("insert into yl_application  (Application,IsValid) values('{0}','{1}')", application, isValid);
        }
        return SqlHelper.Exce(sql);
    }
}