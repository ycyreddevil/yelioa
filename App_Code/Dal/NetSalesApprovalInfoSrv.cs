using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// NetSalesApprovalInfoSrv 的摘要说明
/// </summary>
public class NetSalesApprovalInfoSrv
{
    private static NetSalesApprovalInfoSrv instance;

    public static NetSalesApprovalInfoSrv getNetSalesApprovalInfoSrvInstance()
    {
        if (instance == null)
        {
            instance = new NetSalesApprovalInfoSrv();
        }

        return instance;
    }

    public DataSet getListOfCommitBySelf(String sales)
    {
        String sql = String.Format("select Hospital,Product,NetSalesNumber from " +
            "v_net_sales where Sales='{0}' and (Editable = 0 or Editable = 2) group by hospital,product", sales);
        return SqlHelper.Find(sql);
    }

    public DataSet getListOfCommitByOthers(String salesId)
    {
        //String sql = String.Format("SELECT t1.* FROM v_net_sales t1 left join approval_approver t2 on t1.DocCode = t2.DocCode" +
        //                           " where t2.ApproverId = {0}", salesId);

        string sql = string.Format("select t2.* from approval_approver t1 inner join " +
            "v_net_sales t2 on t1.DocCode = t2.DocCode and t1.Level = t2.Level where t1.ApproverId = '{0}'", salesId);
        return SqlHelper.Find(sql);
    }

    public DataSet getProcessInfo(string docCode)
    {
        //String sql = String.Format("SELECT t1.level,t2.userName,t3.postName,t4.`name` FROM approval_process t1 " +
        //    "left join users t2 on t1.ApproverId = t2.userId left join posts t3 on t1.ApproverPostId = t3.postId " +
        //    "left join department t4 on t1.ApproverDepartmentId = t4.Id where t1.DocumentTableName = 'net_sales' and t1.Level > 1");

        string sql = string.Format("SELECT LEVEL,GROUP_CONCAT(userName SEPARATOR ',') AS userNames FROM " +
        "(select t1.*, t2.userName from approval_approver t1 LEFT JOIN users t2 on t1.approverId = t2.userId where t1.DocCode = '{0}' and Level > 0) as tt GROUP BY LEVEL", docCode);

        return SqlHelper.Find(sql);
    }

    public DataSet getProcessInfoOnFinalLevel(string docCode, int level)
    {
        string sql = string.Format("select * from (select id,t2.userName userNames from approval_record t1 left join users t2 on t1.ApproverId = t2.userId where DocumentTableName = 'net_sales' and DocCode = '{0}' and Level > 0 order by id desc limit {1}) t order by t.id asc", docCode, level);
        //string sql = string.Format("select t2.userName from cost_sharing t1 left join users t2 on t1.ManagerId = t2.UserId where t1.HospitalId = {0} "
        //                    + "and t1.ProductId = {1} and t1.SalesId={2}", HospitalId
        //                    , ProductId, userId);
        return SqlHelper.Find(sql);
    }

    public DataSet getTotalProcessNum()
    {
        String sql = String.Format("select count(*) as total_process from approval_process where DocumentTableName = 'net_sales'");
        return SqlHelper.Find(sql);
    }

    public DataSet getTotalRecordNum(String docCode)
    {
        String sql = String.Format("select Level as total_record from approval_record where DocumentTableName = 'net_sales' and DocCode = {0} order by time desc limit 1", docCode);
        return SqlHelper.Find(sql);
    }

    public DataSet getDetails(String docCode)
    {
        String sql = String.Format("select * from v_net_sales where DocCode = {0}", docCode);
        return SqlHelper.Find(sql);
    }

    public DataSet getApprovalRecord(String docCode)
    {
        String sql = String.Format("select t1.time, t1.ApprovalOpinions,t1.ApprovalResult,t2.userName " +
            "from approval_record t1 left join users t2 on t1.ApproverId = t2.userId where DocCode = {0}", docCode);
        return SqlHelper.Find(sql);
    }

    public DataSet getSubDataList(String hospital, String product, String sales, String userId, String type)
    {
        String sql = "";
        if ("mine".Equals(type))
        {
            sql = String.Format("select DocCode, CreateTime, Sales, State, NetSalesNumber, Hospital, Product from " +
            "v_net_sales where Hospital = '{0}' and Product = '{1}' and Sales = '{2}'"
            , hospital, product, sales);
        }
        else
        {
            //sql = String.Format("select t1.DocCode, t1.CreateTime, t1.Sales, t1.State, t1.NetSalesNumber, t1.Hospital, t1.Product from " +
            //"v_net_sales t1 left join approval_approver t2 on t1.DocCode = t2.DocCode where t1.Hospital = '{0}' and t1.Product = '{1}' and t2.ApproverId = '{2}'"
            //, hospital, product, userId);
            sql = string.Format("select t2.* from approval_approver t1 inner join " +
            "v_net_sales t2 on t1.DocCode = t2.DocCode and t1.Level = t2.Level where t1.ApproverId = '{0}'", userId);
        }

        return SqlHelper.Find(sql);
    }
}