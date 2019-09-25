using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

/// <summary>
/// FormApprovalManager 的摘要说明
/// </summary>
public class FormApprovalManage
{
    public FormApprovalManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static ArrayList GetFormApprovalList()
    {
        DataSet ds = FormApprovalSrv.GetFormApprovalList();
        if (ds == null || ds.Tables.Count == 0)
        {
            return null;
        }
        //else
        //    return ds.Tables[0];
        ArrayList list = new ArrayList();

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("valueField", row["Id"].ToString());
            dict.Add("textField", row["FormName"].ToString());
            dict.Add("ManageRange", row["ManageRange"].ToString());
            list.Add(dict);
        }
        return list;
    }

    public static ArrayList GetFormListByName(string name) {
        DataSet ds = FormApprovalSrv.GetFormListByName(name);
        if (ds == null)
        {
            return null;
        }
        ArrayList list = new ArrayList();
        foreach (DataRow mDr in ds.Tables[0].Rows)
        {
            foreach (DataColumn mDc in ds.Tables[0].Columns)
            {
                list.Add(mDr[mDc].ToString());
            }
        }
        return list;
    }
}