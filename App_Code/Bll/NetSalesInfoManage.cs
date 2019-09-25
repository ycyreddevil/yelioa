using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// NetSalesInfoManage 的摘要说明
/// </summary>
public class NetSalesInfoManage
{
    public NetSalesInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable getInfos(string salesId)
    {
        DataSet ds = NetSalesInfoSrv.getInfos(salesId);
        if (ds != null)
        {
            return ds.Tables[0];
        }
        else
            return null;
    }

    public static string getFlowNumOfReportSales(string hospital,string product, string sales)
    {
        return NetSalesInfoSrv.getFlowNumOfReportSales(hospital, product,sales);
    }

    public static Dictionary<String, String> SaveNetSales(string hospital, string product, string sales, string netSalesNumber, string docCode, string time)
    {
        return NetSalesInfoSrv.SaveNetSales(hospital, product, sales,netSalesNumber, docCode, time);
    }

    public static String getNetSalesNum(string hospital, string product, string sales)
    {
        return NetSalesInfoSrv.getNetSalesNum(hospital, product, sales);
    }
}