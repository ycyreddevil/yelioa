using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ApprovalTimeQuery : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getInfos")
            {
                Response.Write(getInfos());
            }
            Response.End();
        }
    }

    public string getInfos()
    {
        string type = Request.Form["type"];

        // 报销 发货 货需
        if (type == "1")
        {
            string sql = string.Format("SELECT t1.name Approver, t1.code DocId, t2.Time PreviousOperationTime, " +
            "t4.userName CurrentOperator, timestampdiff(day, t2.time, now())+1 Days, '报销' DocName FROM `yl_reimburse` t1 " +
            "inner join approval_record t2 on t1.id = t2.DocCode and t1.`Level` = t2.`Level` +1 " +
            "inner join approval_process t3 on t1.id = t3.DocCode and t1.level = t3.`Level` " +
            "inner join users t4 on t3.approverId = t4.userId " +
            "where t1.status = '审批中' and t2.DocumentTableName = 'yl_reimburse' and t3.DocumentTableName = 'yl_reimburse' " +
            "and timestampdiff(month, t1.apply_time, now()) < 2 order by time;");

            sql += string.Format("SELECT t1.userName Approver, t1.id DocId, t2.Time PreviousOperationTime, " +
            "t4.userName CurrentOperator, timestampdiff(day, t2.time, now())+1 Days, '发货' DocName FROM `deliver_apply_report` t1 " +
            "inner join approval_record t2 on t1.id = t2.DocCode and t1.`Level` = t2.`Level` +1 " +
            "inner join approval_process t3 on t1.id = t3.DocCode and t1.level = t3.`Level` " +
            "inner join users t4 on t3.approverId = t4.userId " +
            "where t1.status = '审批中' and t2.DocumentTableName = 'deliver_apply_report' and t3.DocumentTableName = 'deliver_apply_report' and timestampdiff(month, t1.lmt, now()) < 2;");

            sql += string.Format("SELECT t1.userName Approver, t1.id DocId, t2.Time PreviousOperationTime, " +
            "t4.userName CurrentOperator, timestampdiff(day, t2.time, now())+1 Days, '货需' DocName FROM `demand_apply_report` t1 " +
            "inner join approval_record t2 on t1.id = t2.DocCode and t1.`Level` = t2.`Level` +1 " +
            "inner join approval_process t3 on t1.id = t3.DocCode and t1.level = t3.`Level` " +
            "inner join users t4 on t3.approverId = t4.userId " +
            "where t1.status = '审批中' and t2.DocumentTableName = 'demand_apply_report' and t3.DocumentTableName = 'demand_apply_report' and timestampdiff(month, t1.lmt, now()) < 2;");

            DataSet ds = SqlHelper.Find(sql);

            DataTable dt1 = ds.Tables[0];  // 报销
            DataTable dt2 = ds.Tables[1];  // 发货
            DataTable dt3 = ds.Tables[2];  // 货需

            DataTable tempDt = dt1.Copy();

            foreach (DataRow dr in dt2.Rows)
            {
                tempDt.ImportRow(dr);
            }

            foreach (DataRow dr in dt3.Rows)
            {
                tempDt.ImportRow(dr);
            }

            return JsonHelper.DataTable2Json(tempDt);
        }

        // 表单
        DataTable dt = SqlHelper.Find("select formName from wf_form_config").Tables[0];

        DataTable result = new DataTable();

        result.Columns.Add("Approver", Type.GetType("System.String"));
        result.Columns.Add("DocId", Type.GetType("System.String"));
        result.Columns.Add("PreviousOperationTime", Type.GetType("System.String"));
        result.Columns.Add("CurrentOperator", Type.GetType("System.String"));
        result.Columns.Add("Days", Type.GetType("System.String"));
        result.Columns.Add("DocName", Type.GetType("System.String"));

        foreach (DataRow dr in dt.Rows)
        {
            string formName = dr[0].ToString();

            string sql = string.Format("SELECT t1.userId Approver, t1.docCode DocId, t2.ApprovalTime PreviousOperationTime, " +
            "t4.userName CurrentOperator, timestampdiff(day, t2.ApprovalTime, now())+1 Days, '{0}' DocName FROM wf_form_{0} t1 " +
            "inner join wf_approver t3 on t1.id = t3.DocId and t1.level = t3.`Level` " +
            "inner join wf_approver t5 on t1.id = t5.DocId and t1.level = t5.`Level` +1 " +
            "inner join wf_record t2 on t1.id = t2.DocId and t5.userId = t2.userId " +
            "inner join users t4 on t3.userId = t4.userId " +
            "where t1.status = '审批中' and t2.tableName = '{0}' and t3.tableName = '{0}' and timestampdiff(month, t1.CreateTime, now()) < 2", formName);

            var tempDt = SqlHelper.Find(sql).Tables[0];

            foreach (DataRow tempDr in tempDt.Rows)
            {
                result.ImportRow(tempDr);
            }
        }

        return JsonHelper.DataTable2Json(result);
    }
}