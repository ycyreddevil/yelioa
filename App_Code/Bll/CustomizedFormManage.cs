using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// getCustomizedFormManage 的摘要说明
/// </summary>
public class CustomizedFormManage
{
    public CustomizedFormManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable getCustomizedForm()
    {
        DataSet ds = CustomizedFormSrv.getCustomizedForm();
        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    public static DataTable getFormData(string formName)
    {
        DataSet ds = CustomizedFormSrv.getFormData(formName);
        if (ds == null)
            return null;

        return ds.Tables[0];
    }
}