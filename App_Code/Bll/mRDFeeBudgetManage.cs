using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;


/// <summary>
/// mRDFeeBudgetManage 的摘要说明
/// </summary>
public class mRDFeeBudgetManage
{
    public mRDFeeBudgetManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string getProjectList(JObject jObject)
    {
        DataSet ds = mRDFeeBudgetSrv.getProjectList(jObject);

        JObject resulJObject = new JObject();

        if (ds == null)
        {
            resulJObject.Add("ErrCode", 2);
            resulJObject.Add("ErrMsg", "sql语句错误");
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            resulJObject.Add("ErrCode", 1);
            resulJObject.Add("ErrMsg", "查询结果为空");
        }
        else
        {
            resulJObject.Add("ErrCode", 0);
            resulJObject.Add("ErrMsg", "操作成功");
            resulJObject.Add("data", JsonHelper.DataTable2Json(ds.Tables[0]));
        }

        return resulJObject.ToString();
    }
}