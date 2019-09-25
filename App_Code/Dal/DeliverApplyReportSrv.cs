using System.Collections.Generic;
using System.Data;

/// <summary>
/// DeliverApplyReportSrv 的摘要说明
/// </summary>
public class DeliverApplyReportSrv
{
    public DeliverApplyReportSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet findInformer()
    {
        string sql = "select distinct userName, userId from users where isValid='在职' order by userName";
        return SqlHelper.Find(sql);
    }

    public static DataSet findInformer(string q)
    {
        string sql = "select distinct userName from users where isValid='在职' and userName like '%" + q + "%' order by userName";
        return SqlHelper.Find(sql);
    }

    public static DataSet findHospitalName(string userId)
    {
        string sql = string.Format("select distinct t1.clientName from new_client t1 left join new_client_product_users t2 on t1.clientCode = t2.clientCode" +
            " where t2.userId = '{0}'", userId);
        return SqlHelper.Find(sql);
    }

    public static DataSet findHospitalName(string name, string userId)
    {
        string sql = string.Format("select department from v_user_department_post where userId = '{0}'", userId);
        var departmentName = SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString();

        if (departmentName.Contains("销售部"))
        {
            sql = string.Format("select distinct t1.clientName from new_client t1 left join new_client_product_users t2 on t1.clientCode = t2.clientCode " +
            " where clientName like '%{0}%' and t2.userId = '{1}'", name, userId);

            if (SqlHelper.Find(sql).Tables[0].Rows.Count == 0)
            {
                sql = string.Format("select distinct clientName from new_client");
            }
        }
        else
        {
            sql = string.Format("select distinct clientName from new_client");
        }

        
        return SqlHelper.Find(sql);
    }

    public static DataSet findProductName(string name, string userId)
    {
        string sql = string.Format("select department from v_user_department_post where userId = '{0}'", userId);
        var departmentName = SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString();

        if (departmentName.Contains("销售部"))
        {
            sql = string.Format("select distinct t1.productName from new_product t1 left join new_client_product_users t2 on t1.productCode = t2.productCode" +
            " where t1.productName like '%{0}%' and (t2.userId = '{1}' or t1.type = '配件')", name, userId);

            if (SqlHelper.Find(sql).Tables[0].Rows.Count == 0)
            {
                sql = string.Format("select distinct productName from new_product");
            }
        }
        else
        {
            sql = string.Format("select distinct productName from new_product");
        }

        return SqlHelper.Find(sql);
    }

    public static DataSet findProductName(string userId)
    {
        string sql = string.Format("select distinct t1.productName from new_product t1 left join new_client_product_users t2 on t1.productCode = t2.productCode" +
            " where t2.userId = '{0}' or t1.type = '配件'", userId);
        return SqlHelper.Find(sql);
    }

    public static DataSet findSpec(string q)
    {
        string sql = string.Format("select distinct specification from new_product where specification <> '' AND productName = '{0}'", q);
        return SqlHelper.Find(sql);
    }
    public static DataSet findUnit(string q)
    {
        string sql = string.Format("select distinct unit from new_product where Unit<>'' AND productName = '{0}'", q);
        return SqlHelper.Find(sql);
    }

    public static DataSet findAgent(string name, string userId)
    {
        //string sql = string.Format("select distinct t2.name agentName from new_cost_sharing t1 " +
        //    "left join new_agent t2 on t1.代理商名称 = t2.id " +
        //    "left join new_client t3 on t1.医院 = t3.id " +
        //    "where t3.clientName = '{0}' and t1.代理商名称 != ''", q);
        //return SqlHelper.Find(sql);

        string sql = string.Format("select department from v_user_department_post where userId = '{0}'", userId);
        var departmentName = SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString();

        if (departmentName.Contains("销售部"))
        {
            sql = string.Format("select distinct t1.name from new_agent t1 left join new_client_product_users t2 on t1.code = t2.agentCode" +
            " where t1.name like '%{0}%' and t2.userId = '{1}'", name, userId);

            if (SqlHelper.Find(sql).Tables[0].Rows.Count == 0)
            {
                sql = string.Format("select distinct name from new_agent");
            }
        }
        else
        {
            sql = string.Format("select distinct name from new_agent");
        }

        return SqlHelper.Find(sql);
    }

    public static DataSet findMaxId()
    {
        string sql = string.Format("select max(id) from deliver_apply_report");
        return SqlHelper.Find(sql);
    }

    public static string insertDeliverApplyReport(string deliverStyle, string hospitalCode, string productCode, string agentCode, string deliverNumber, string remark, UserInfo userInfo, 
        string stock, string netSales, string period, string isStockReceiptTogether, string deliverAddress, string deliverName, string deliverPhone)
    {
        string sql = string.Format("insert into deliver_apply_report (deliverStyle, hospitalCode, productCode, agentCode, deliverNumber, remark, " +
            "userId, userName, lmt, status, level, stock, netSales, period, isStockReceiptTogether, deliverAddress, deliverName, deliverPhone) " +
            "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}', now(), 0, 0, '{8}','{9}','{10}','{11}', '{12}','{13}','{14}')",
            deliverStyle, hospitalCode, productCode, agentCode, deliverNumber, remark, userInfo.userId, userInfo.userName, stock, netSales, period, isStockReceiptTogether, deliverAddress, deliverName, deliverPhone);

        return SqlHelper.InsertAndGetLastId(sql);
    }

    public static string insertAttachement(string docCode, List<string> attachMentList)
    {
        List<string> sqls = new List<string>();
        string sql = string.Format("delete from deliver_attachment where docCode = '{0}'", docCode);
        sqls.Add(sql);

        foreach (string attachment in attachMentList)
        {
            sql = string.Format("insert into deliver_attachment (docCode,imageUrl,lmt) values ('{0}', '{1}', now())", docCode, attachment.Replace("\\", "/"));
            sqls.Add(sql);
        }
        return SqlHelper.Exce(sqls.ToArray());
    }

    public static DataSet findProductCode(string productName, string specification, string unit)
    {
        string sql = string.Format("select productCode from new_product where productName = '{0}' and specification = '{1}' and unit = '{2}'", productName, specification, unit);
        return SqlHelper.Find(sql);
    }

    public static DataSet findHospitalCode(string hospitalName)
    {
        string sql = string.Format("select clientCode from new_client where clientName = '{0}'", hospitalName);
        return SqlHelper.Find(sql);
    }

    public static DataSet findAgentCode(string agentName)
    {
        string sql = string.Format("select code from new_agent where name = '{0}'", agentName);
        return SqlHelper.Find(sql);
    }

    public static string insertDeliverAppyReportRecord(string level, string approverId, string submitterId, string approvalResult, string approvalOptions)
    {
        string sql = string.Format("insert into approval_record (DocumentTableName,DocCode,Level,Time,ApproverId,SubmitterId,ApprovalResult,ApprovalOpinions)" +
            " Values ('deliver_apply_record', (select max(id) from deliver_apply_report) ,{0},now(),'{1}','{2}','{3}','{4}')", level, approverId, submitterId, approvalResult, approvalOptions);
        return SqlHelper.Exce(sql);
    }

    public static DataSet findRecordCount(string docCode)
    {
        string sql = string.Format("select count(*) from approval_record where docCode = '{0}'", docCode);
        return SqlHelper.Find(sql);
    }

    public static string updateRecordStatus(string docCode, int status)
    {
        string sql = string.Format("update deliver_apply_record set status = {0} where docCode = '{1}'", status, docCode);
        return SqlHelper.Exce(sql);
    }

    public static string insertApprover(List<string> approverIds, string docCode)
    {
        List<string> sqls = new List<string>();
        for (int i = 0; i < approverIds.Count; i++)
        {
            string sql = string.Format("insert into approval_approver (documentTableName, docCode, approverId,level) values " +
                "('deliver_apply_report','{0}','{1}','{2}')", docCode, approverIds[i], i);
            sqls.Add(sql);
        }

        return SqlHelper.Exce(sqls.ToArray());
    }

    public static DataSet findApproverCount(string docCode)
    {
        string sql = string.Format("select count(*) from deliver_apply_report_approver where docCode = '{0}'", docCode);
        return SqlHelper.Find(sql);
    }

    public static string insertInformer(string docCode, List<string> userIdList)
    {
        List<string> sqls = new List<string>();
        foreach (string userId in userIdList)
        {
            string sql = string.Format("insert into approval_informer (tablename,doccode,informerUserId) values ('deliver_apply_report','{0}','{1}')", docCode, userId);
            sqls.Add(sql);
        }
        return SqlHelper.Exce(sqls.ToArray());
    }
    public static DataSet UpdateApprover(string userId, ref string msg)
    {
        string sql = string.Format("SELECT * FROM v_user_department_post WHERE userId = '{0}' AND  FIND_IN_SET( '292',FindParentDepartment(departmentId)) > 0 ", userId);
        return SqlHelper.Find(sql, ref msg);
    }
}