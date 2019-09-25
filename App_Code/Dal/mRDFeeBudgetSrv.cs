using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

/// <summary>
/// mRDFeeBudgetSrv 的摘要说明
/// </summary>
public class mRDFeeBudgetSrv
{
    public mRDFeeBudgetSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet getProjectList(JObject jObject)
    {
        string sql = string.Format(" select yp.name,sum(ybd.budget) budget from yl_project yp " +
                                   " left join yl_rd_budget_record yrbr on yp.id = yrbr.projectId and yrbr.date between '{0}' and '{1}'" +
                                   " left join yl_budget_detail ybd on yrbr.id = ybd.budgetrecordId and budgetType = 'rd'" +
                                   " where yp.managerId = '{2}' group by yp.name", jObject["startDate"], jObject["endDate"],jObject["userId"]);
        return SqlHelper.Find(sql);
    }

    public static string insertRdFeeBudget(JObject jObject)
    {
        List<string> sqls = new List<string>();
        sqls.Add(SqlHelper.GetInsertString(jObject, "yl_rd_budget_record"));
        sqls.Add(SqlHelper.GetInsertString(jObject, "yl_budget_detail"));
        return SqlHelper.Exce(sqls.ToArray());
    }
}