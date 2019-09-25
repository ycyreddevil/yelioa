using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// OperationDeliverSrv 的摘要说明
/// </summary>
public class OperationDeliverSrv
{
    public OperationDeliverSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet findByCondInfomer(string informUserId)
    {
        string sql = string.Format("select dar.*,jp.name productName,jp.specification spec,jp.unit unit, jc.name hospitalName from deliver_apply_report dar left join jb_product jp on dar.productCode = jp.code " +
            "left join jb_client jc on dar.hospitalCode = jc.code inner join approval_informer ai on dar.id = ai.DocCode and TableName = 'deliver_apply_report' " +
            "where ai.InformerUserId = '{0}' order by dar.lmt desc", informUserId);

        return SqlHelper.Find(sql);
    }

    public static DataSet findByCond(string starttm, string endtm, string applyName, string hospital, string product, string isChecked)
    {
        string sql = "select dar.*,GROUP_CONCAT(vudp.department) department, jp.productName,jp.specification spec,jp.unit unit, jc.clientName hospitalName, na.name agentName from deliver_apply_report dar left join new_product jp on dar.productCode = jp.productCode " +
            "left join new_client jc on dar.hospitalCode = jc.clientCode left join new_agent na on dar.agentCode = na.code left join v_user_department_post vudp on dar.userId = vudp.userId where status = '已审批' and ApproveTime is not null ";

        if (starttm != null && endtm != null && starttm != "" && endtm != "")
        {
            sql += "and OperationApprovalTime between '" + starttm + "' and '" + endtm + "'";
        }
        if (applyName != null && !"".Equals(applyName))
        {
            sql += "and dar.UserName like '%" + applyName + "%'";
        }
        if (hospital != null && !"".Equals(hospital))
        {
            sql += "and jc.name like '%" + hospital + "%'";
        }
        if (product != null && !"".Equals(product))
        {
            sql += "and jp.name like '%" + product + "%'";
        }
        if (isChecked == "1")
        {
            sql += " and (OperationApprovalTime is null or (DeliverNumber-ApprovalNumber > 0)) and Opinion is null";
        }
        else if (isChecked == "2")
        {
            sql += " and (OperationApprovalTime is not null and (DeliverNumber-ApprovalNumber) = 0) or (Opinion is not null)";
        }
        sql += " GROUP BY dar.Id order by ApproveTime";
        return SqlHelper.Find(sql);
    }

    public static string updateActualFee(ArrayList list)
    {
        List<string> conditionList = new List<string>();
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> dict = (Dictionary<string, string>)list[i];
            string condition = string.Format(" where id = '{0}'", dict["Id"]);
            dict.Remove("Id");

            conditionList.Add(condition);
        }
        string sql = SqlHelper.GetUpdateString(list, "deliver_apply_report", conditionList);
        return SqlHelper.Exce(sql);
    }

    public static string insertSalesData(ArrayList list)
    {
        List<string> sqls = new List<string>();
        foreach (string id in list)
        {
            string sql1 = string.Format("delete from sales_daily_report where docNo = '{0}'", id);
            sqls.Add(sql1);

            string sql = string.Format("insert into sales_daily_report (docNo, date, code, hospitalCode, salesNumber, agentCode)" +
                " select id, OperationApprovalTime, productCode, hospitalCode, approvalNumber, agentCode from deliver_apply_report where id = '{0}'", id);
            sqls.Add(sql);
        }
        return SqlHelper.Exce(sqls.ToArray());
    }

    public static string updateReason(List<string> list, string reason)
    {
        List<string> sqls = new List<string>();
        foreach (string id in list)
        {
            string sql = string.Format("update deliver_apply_report set reason = '{0}' where id = '{1}'", reason, id);

            sqls.Add(sql);
        }
        return SqlHelper.Exce(sqls.ToArray());
    }

    public static string updateDeliverCode(List<string> list, string deliverCode)
    {
        List<string> sqls = new List<string>();

        foreach (string id in list)
        {
            string sql = string.Format("update deliver_apply_report set deliverCode = '{0}' where id = '{1}'", deliverCode, id);

            sqls.Add(sql);
        }

        return SqlHelper.Exce(sqls.ToArray());
    }

    public static string updateReceiptCode(List<string> list, string receiptCode)
    {
        List<string> sqls = new List<string>();

        foreach (string id in list)
        {
            string sql = string.Format("update deliver_apply_report set receiptCode = '{0}' where id = '{1}'", receiptCode, id);

            sqls.Add(sql);
        }

        return SqlHelper.Exce(sqls.ToArray());
    }

    public static string Reject(List<string> list, string opinion)
    {
        List<string> sqls = new List<string>();
        foreach (string id in list)
        {
            string sql = string.Format("update deliver_apply_report set Opinion = '{0}',OperationApprovalTime = now() where id = '{1}'", opinion, id);

            sqls.Add(sql);
        }
        return SqlHelper.Exce(sqls.ToArray());
    }

    public static DataSet GetwechatUserId(List<string> list)
    {
        string sql = "SELECT dar.Id,u.wechatUserId FROM `deliver_apply_report` AS dar LEFT JOIN users AS u ON dar.UserId = u.userId WHERE  dar.id IN('";
        foreach (string id in list)
        {
            sql += id + "','";
        }
        sql = sql.Substring(0, sql.Length - 2);
        sql += ")";
        return SqlHelper.Find(sql);
    }

    public static string UpdateOperationApprovalTime(ArrayList list)
    {
        List<string> sqls = new List<string>();
        foreach (string id in list)
        {
            string sql = "UPDATE  `deliver_apply_report` SET OperationApprovalTime=NOW() WHERE id='" + id + "'";
            sqls.Add(sql);
        }

        return SqlHelper.Exce(sqls.ToArray());
    }

    public static string AddBranch(string name, string code)
    {
        string sql = string.Format("insert into jb_client (Code,Name) value('{0}','{1}')", code, name);
        return SqlHelper.Exce(sql);
    }

    public static string AddProduct(string name, string code, string spec, string unit)
    {
        string sql = string.Format("insert into jb_product (Code,Name,Specification,Unit) value('{0}','{1}','{2}','{3}')", code, name, spec, unit);
        return SqlHelper.Exce(sql);
    }
}