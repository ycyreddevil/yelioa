using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// CustomizedFormSrv 的摘要说明
/// </summary>
public class CustomizedFormSrv
{
    public CustomizedFormSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet getCustomizedForm()
    {
        string sql = "SELECT distinct formName, userName, lmt FROM form_attribute";
        return SqlHelper.Find(sql);
    }

    public static DataSet getFormData(string formName)
    {
        string sql = string.Format("select * from form_attribute where formName = '{0}'", formName);
        return SqlHelper.Find(sql);
    }
}