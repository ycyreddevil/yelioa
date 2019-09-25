using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ImportPurchaseSrv 的摘要说明
/// </summary>
public class ImportPurchaseSrv
{
    public ImportPurchaseSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string ImportInfos(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertString(dict, "purchase_daily_report");
        return SqlHelper.Exce(sql);
    }

    public static DataSet findDuplicate(string docNo, string batchNo, string code, string date)
    {
        string sql = string.Format("select 1 from purchase_daily_report where DocNo = '{0}' and BatchNo = '{1}' and Code = '{2}' and Date = '{3}'", 
            docNo, batchNo, code, date);
        return SqlHelper.Find(sql);
    }

    public static DataSet findByDate(string date)
    {
        string sql = string.Format("SELECT t1.ApproveTime, t2.`Name`, t2.Specification, t1.DeliverNumber FROM `demand_apply_report` t1 " +
            "left join jb_product t2 on t1.`ProductCode` = t2.`Code` where t2.name is not null and t1.ApproveTime = '{0}'", date);
        return SqlHelper.Find(sql);
    }

    //public static DataSet getCommonPurchaseData(int year, int month)
    //{
    //    string sql = string.Format("select jp.code, jp.name, jp.specification, jp.unit, pwr.targetnumber, pwr.reportnumber, pwr.approvalnumber from jb_product jp left join " +
    //        "purchase_week_report pwr on jp.code = pwr.code and pwr.year = {0} and pwr.month = {1} where jp.commonpurchase = 0", year, month);
    //    return SqlHelper.Find(sql);
    //}

    public static string insertOrUpdateCommonPurchaseData(Dictionary<string, string> dict)
    {
        string code = dict["code"];
        int targetnumber = dict["targetnumber"]==""?0:Int32.Parse(dict["targetnumber"]);
        int reportnumber = dict["reportnumber"]==""?0:Int32.Parse(dict["reportnumber"]);
        int approvalnumber = dict["approvalnumber"] ==""?0:Int32.Parse(dict["approvalnumber"]);
        int year = Int32.Parse(dict["year"]);
        int month = Int32.Parse(dict["month"]);
        string differreason = dict["differreason"];
        string remark = dict["remark"];

        string saveOrUpdateSql = string.Format("insert into purchase_week_report (code,targetnumber,reportnumber,approvalnumber,year, month, differreason, remark) " +
            "values ('{0}',{1},{2},{3},{4},{5},'{6}','{7}') ON DUPLICATE KEY UPDATE targetnumber = {1}, reportnumber = {2}, approvalnumber = {3}, " +
            "differreason= '{6}', remark = '{7}'", code, targetnumber, reportnumber, approvalnumber, year, month, differreason, remark);

        return saveOrUpdateSql;
    }

    public static string saveReocrd(Dictionary<string, string> dict, string name)
    {
        string code = dict["code"];
        int targetnumber = dict["targetnumber"] == "" ? 0 : Int32.Parse(dict["targetnumber"]);
        int reportnumber = dict["reportnumber"] == "" ? 0 : Int32.Parse(dict["reportnumber"]);
        int approvalnumber = dict["approvalnumber"] == "" ? 0 : Int32.Parse(dict["approvalnumber"]);
        int year = Int32.Parse(dict["year"]);
        int month = Int32.Parse(dict["month"]);
        string differreason = dict["differreason"];
        string remark = dict["remark"];

        string eventStr = "更改后,";
        if (targetnumber != 0)
        {
            eventStr += "目标数量为" + targetnumber + ",";
        }
        if (reportnumber != 0)
        {
            eventStr += "上报数量为" + reportnumber + targetnumber + ",";
        }
        if (approvalnumber != 0)
        {
            eventStr += "审批数量为" + approvalnumber + targetnumber + ",";
        }

        if ("更改后,".Equals(eventStr))
        {
            return null;
        }

        string sql = string.Format("insert into account_record (name,code,event,lmt) values ('{0}', '{1}', '{2}', now())", name, "产品编号为:"+code, eventStr);
        return sql;
    }

    public static DataSet findByMonth(string cond1, string cond2, string cond3, string cond4, string cond5, int year, int month)
    {
        string sql = string.Format("select jp.code, jp.name, jp.specification, jp.unit, ifnull(sum(dar.deliverNumber), 0) reportnumber," +
            "ifnull(sum(dar.ApprovalNumber), 0) approvalnumber, pwr.differreason, pwr.remark, " +
            "(select case when sum(PurchaseNumber) is null then 0 else sum(PurchaseNumber) end from purchase_daily_report where code = jp.code and date {0}) week1, " +
            "(select case when sum(PurchaseNumber) is null then 0 else sum(PurchaseNumber) end from purchase_daily_report where code = jp.code and date {1}) week2, " +
            "(select case when sum(PurchaseNumber) is null then 0 else sum(PurchaseNumber) end from purchase_daily_report where code = jp.code and date {2}) week3, " +
            "(select case when sum(PurchaseNumber) is null then 0 else sum(PurchaseNumber) end from purchase_daily_report where code = jp.code and date {3}) week4, " +
            "(select case when sum(PurchaseNumber) is null then 0 else sum(PurchaseNumber) end from purchase_daily_report where code = jp.code and date {4}) week5 " +
            " from jb_product jp left join purchase_week_report pwr on jp.code = pwr.code and pwr.year = {5} and " +
            "pwr.month = {6} left join demand_apply_report dar on jp.code = dar.productCode and dar.ApproveTime BETWEEN " +
            "'{5}-{6}-1' and '{5}-{7}-31' group by code", cond1, cond2, cond3, cond4, cond5, year, month, month+1);

        return SqlHelper.Find(sql);
    }

    public static string updateDifferReason(ArrayList list, List<string> conditionList)
    {
        string sql = SqlHelper.GetUpdateString(list, "purchase_week_report", conditionList);
        return SqlHelper.Exce(sql);
    }
}