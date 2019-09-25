using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// mNetSalesApproval 的摘要说明
/// </summary>
public class NetSalesApprovalInfoManage
{
    public NetSalesApprovalInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    // 查询我提交的
    public static DataTable getListOfCommitBySelf(String sales)
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getListOfCommitBySelf(sales);
        return similarReturn(dataSet);
    }

    // 查询待我审批的
    public static DataTable getListOfCommitByOthers(String salesId)
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getListOfCommitByOthers(salesId);
        return similarReturn(dataSet);
    }

    // 查询审批流程
    public static DataTable getProcessInfo(string docCode)
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getProcessInfo(docCode);
        return similarReturn(dataSet);
    }

    // 查询审批所有的流程
    public static DataTable getProcessInfoOnFinalLevel(string docCode, int level)
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getProcessInfoOnFinalLevel(docCode, level);
        return similarReturn(dataSet);
    }

    // 查询总共有几步流程
    public static DataTable getTotalProcessNum()
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getTotalProcessNum();
        return similarReturn(dataSet);
    }

    //查询目前总共有几步
    public static DataTable getTotalRecordNum(String docCode)
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getTotalRecordNum(docCode);
        return similarReturn(dataSet);
    }

    // 查询单据详情
    public static DataTable getDetails(String docCode)
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getDetails(docCode);
        return similarReturn(dataSet);
    }

    // 查询审批记录
    public static DataTable getApprovalRecord(String docCode)
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getApprovalRecord(docCode);
        return similarReturn(dataSet);
    }

    // 子表格数据
    public static DataTable getSubDataList(String hospital, String product, String sales, String userId, String type)
    {
        NetSalesApprovalInfoSrv netSalesApprovalInfoSrv = NetSalesApprovalInfoSrv.getNetSalesApprovalInfoSrvInstance();
        DataSet dataSet = netSalesApprovalInfoSrv.getSubDataList(hospital, product, sales, userId, type);
        return similarReturn(dataSet);
    }

    private static DataTable similarReturn(DataSet dataSet)
    {
        if (dataSet != null)
        {
            return dataSet.Tables[0];
        }
        else
        {
            return null;
        }
    }
}
