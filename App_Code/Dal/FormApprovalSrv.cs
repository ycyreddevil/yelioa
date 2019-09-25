using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// FormApprovalSrv 的摘要说明
/// </summary>
public class FormApprovalSrv
{
    public FormApprovalSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetFormApprovalList()
    {
        string sql = "select * from wf_form_config";
        return SqlHelper.Find(sql);
    }

    public static DataSet GetFormListByName(string name) {
        string sql = string.Format("select * from wf_form_" +name);
        return SqlHelper.Find(sql);
    }
}