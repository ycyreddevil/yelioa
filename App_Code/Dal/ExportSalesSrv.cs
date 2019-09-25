using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ExportSalesSrv 的摘要说明
/// </summary>
public class ExportSalesSrv
{
    public ExportSalesSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string ImportInfos(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertString(dict, "sales_daily_report");
        return SqlHelper.Exce(sql);
    }

    public static DataSet findDuplicate(string docNo, string batchNo, string code, string date)
    {
        string sql = "";
        if (!"".Equals(batchNo))
        {
            sql = string.Format("select 1 from sales_daily_report where DocNo = '{0}' and BatchNo = '{1}' and Code = '{2}' and Date = '{3}'",
            docNo, batchNo, code, date);
        }
        else
        {
            sql = string.Format("select 1 from sales_daily_report where DocNo = '{0}' and Code = '{1}' and Date = '{2}'",
            docNo, code, date);
        }

        return SqlHelper.Find(sql);
    }

    public static DataSet findByDate(string date, string style)
    {
        string sql = string.Format("SELECT t1.Date, t2.`ProductName` AS Name,t2.Specification,t1.SalesNumber,t4.ClientName HospitalName,t2.Unit FROM "
            + " sales_daily_report t1 LEFT JOIN new_product t2 ON t1.`Code` = t2.`ProductCode` LEFT JOIN new_client t4 ON t1.hospitalcode = t4.ClientCode "
            + " WHERE t2.ProductName IS NOT NULL AND t1.date = '{0}' ", date);
        return SqlHelper.Find(sql);
    }

    public static DataSet findByMonth(string cond1, string cond2, string cond3, string cond4, string cond5, string firstday, string lastday, string style)
    {
        string sql = string.Format("select distinct jp.code, jp.name, jp.specification, jp.unit, jc2.name hospitalname, " +
            "(select case when sum(salesnumber) is null then 0 else sum(salesnumber) end from sales_daily_report where code = jp.code and HospitalCode = jc2.`Code` and date {0} ) week1, " +
            "(select case when sum(salesnumber) is null then 0 else sum(salesnumber) end from sales_daily_report where code = jp.code and HospitalCode = jc2.`Code` and date {1} ) week2, " +
            "(select case when sum(salesnumber) is null then 0 else sum(salesnumber) end from sales_daily_report where code = jp.code and HospitalCode = jc2.`Code` and date {2} ) week3, " +
            "(select case when sum(salesnumber) is null then 0 else sum(salesnumber) end from sales_daily_report where code = jp.code and HospitalCode = jc2.`Code` and date {3} ) week4, " +
            "(select case when sum(salesnumber) is null then 0 else sum(salesnumber) end from sales_daily_report where code = jp.code and HospitalCode = jc2.`Code` and date {4} ) week5, " +
            "(select case when sum(salesnumber) is null then 0 else sum(salesnumber) end from sales_daily_report where code = jp.code and HospitalCode = jc2.`Code` and date between '{6}' and '{7}' ) total " +
            " from jb_product jp left join sales_daily_report sdr on jp.code = sdr.code " +
            "left join jb_client jc2 on sdr.hospitalcode = jc2.code where sdr.date between '{6}' and '{7}'", 
            cond1, cond2, cond3, cond4, cond5, style, firstday, lastday);

        return SqlHelper.Find(sql);
    }
}