using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ImportPurchaseManage 的摘要说明
/// </summary>
public class ImportPurchaseManage
{
    public ImportPurchaseManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static Dictionary<string, string> ImportInfos(Dictionary<string, string> dict, string type)
    {
        string code = "";
        string date = "";
        string purchaseNumber = "";
        string docNo = "";
        string batchNo = "";

        if ("jd".Equals(type))
        {
            code = dict["物料代码"].ToString();
            if (!dict["日期"].Contains("/") && !dict["日期"].Contains("-"))
            {
                date = DateTime.FromOADate(double.Parse(dict["日期"].ToString())).ToString();
            }
            else
            {
                date = dict["日期"];
            }
            purchaseNumber = dict["实收数量"];
            docNo = dict["单据编号"];
            batchNo = dict["批号"];
        }
        else if ("jb".Equals(type))
        {
            code = dict["器械代码"];
            if (!dict["日期"].Contains("/") && !dict["日期"].Contains("-"))
            {
                date = DateTime.FromOADate(double.Parse(dict["日期"])).ToString();
            }
            else
            {
                date = dict["日期"];
            }

            purchaseNumber = dict["到货数量"];
            docNo = dict["单号"];
            batchNo = dict["批号/序列号"];

            if ("".Equals(dict["过帐"]))
            {
                dict["状态"] = "该单据未过账";
                return dict;
            }
        }

        if (docNo == null || "".Equals(docNo) || date == null || "".Equals(date) || code == null || "".Equals(code))
        {
            dict["状态"] = "存在空值";
            return dict;
        }

//        DataSet ds = ImportPurchaseSrv.findDuplicate(docNo, batchNo, code, date);
//
//        if (ds != null && ds.Tables[0].Rows.Count > 0)
//        {
//            dict["状态"] = "单据重复";
//            return dict;
//        }

        Dictionary<string, string> resultDict = new Dictionary<string, string>();

        resultDict.Add("code", code);
        resultDict.Add("date", date);
        resultDict.Add("purchaseNumber", purchaseNumber);
        resultDict.Add("docno", docNo);
        resultDict.Add("batchNo", batchNo);

        if ("jd".Equals(type))
        {
            resultDict.Add("type", "jd");
        }
        else if ("jb".Equals(type))
        {
            resultDict.Add("type", "jb");
        }

        string res = ImportPurchaseSrv.ImportInfos(resultDict);

        if (!string.IsNullOrEmpty(res))
        {
            if (res.Contains("操作成功"))
                dict["状态"] = "已导入";
            else
                dict["状态"] = res;
        }

        return dict;
    }

    public static DataTable findByDate(string date)
    {
        DataSet ds = ImportPurchaseSrv.findByDate(date);

        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    public static DataTable findByMonth(string date)
    {
        string firstWeekFirstDay = date + "-1";
        int day = GetWeekNum(firstWeekFirstDay);
        string firstWeekLastDay = date + "-" + (8 - day);

        string firstWeekCond = "between '" + firstWeekFirstDay + "' and '" + firstWeekLastDay + "'";

        string secondWeekFirstDay = date + "-" + (9 - day);
        string secondWeekLastDay = date + "-" + (15 - day);

        string secondWeekCond = "between '" + secondWeekFirstDay + "' and '" + secondWeekLastDay + "'";

        string thirdWeekFirstDay = date + "-" + (16 - day);
        string thirdWeekLastDay = date + "-" + (22 - day);

        string thirdWeekCond = "between '" + thirdWeekFirstDay + "' and '" + thirdWeekLastDay + "'";

        string fourthWeekFirstDay = date + "-" + (23 - day);
        string fourthWeekLastDay = date + "-" + (29 - day);

        string fourthWeekCond = "between '" + fourthWeekFirstDay + "' and '" + fourthWeekLastDay + "'";

        int lastDay = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        string fifthWeekFirstDay = "";
        string fifthWeekLastDay = "";
        string fifthWeekCond = "between '0000-00-01' and '0000-00-01'";

        if ((29-day) < lastDay)
        {
            fifthWeekFirstDay = date + "-" + (30 - day);
            fifthWeekLastDay = date + "-" + lastDay;
            fifthWeekCond = "between '" + fifthWeekFirstDay + "' and '" + fifthWeekLastDay + "'";
        }

        int year = Int32.Parse(date.Split(new Char[] {'-'})[0]);
        int month = Int32.Parse(date.Split(new Char[] {'-'})[1]);

        DataSet ds = ImportPurchaseSrv.findByMonth(firstWeekCond, secondWeekCond, thirdWeekCond, fourthWeekCond, fifthWeekCond, year, month);

        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    //public static DataTable getCommonPurchaseData(int year, int month)
    //{
    //    DataSet ds = ImportPurchaseSrv.getCommonPurchaseData(year, month);

    //    if (ds == null)
    //        return null;

    //    return ds.Tables[0];
    //}

    public static string insertOrUpdateCommonPurchaseData(List<Dictionary<string, string>> dictList, string year, string month)
    {
        List<string> sqls = new List<string>();
        foreach (Dictionary<string, string> dict in dictList)
        {
            dict.Add("year", year);
            dict.Add("month", month);
            string sql = ImportPurchaseSrv.insertOrUpdateCommonPurchaseData(dict);
            sqls.Add(sql);
        }
        string msg = SqlHelper.Exce(sqls.ToArray());
        SqlExceRes sqlExceRes = new SqlExceRes(msg);
        return sqlExceRes.GetResultString("保存成功", "保存失败");
    }

    public static string saveRecord(List<Dictionary<string, string>> dictList, string name)
    {
        List<string> sqls = new List<string>();
        foreach (Dictionary<string, string> dict in dictList)
        {
            string sql = ImportPurchaseSrv.saveReocrd(dict, name);
            if (sql != null)
            {
                sqls.Add(sql);
            }
        }
        string msg = SqlHelper.Exce(sqls.ToArray());
        SqlExceRes sqlExceRes = new SqlExceRes(msg);
        return sqlExceRes.GetResultString("保存成功", "保存失败");
    }

    public static string updateDifferReason(string dataJson, string year, string month)
    {
        List<Dictionary<string, string>> dictList = JsonHelper.DeserializeJsonToObject<List<Dictionary<string, string>>>(dataJson);

        List<string> conditionList = new List<string>();
        ArrayList tempDictList = new ArrayList();

        foreach (Dictionary<string, string> dict in dictList)
        {
            Dictionary<string, string> tempDict = new Dictionary<string, string>();

            tempDict.Add("DifferReason", dict["differreason"]);
            tempDict.Add("Remark", dict["remark"]);

            tempDictList.Add(tempDict);

            string conditionSql = "where ";
            string code = dict["code"];

            conditionSql += "code = '" + code + "' and year = " + year + " and month = " + month;

            conditionList.Add(conditionSql);
        }

        ImportPurchaseSrv.updateDifferReason(tempDictList, conditionList);

        return null;
    }

    private static int GetWeekNum(string strData)
    {
        string strDayOfWeek = Convert.ToDateTime(strData).DayOfWeek.ToString();
        int intWeekday = -1;
        switch (strDayOfWeek)
        {
            case "Monday":
                intWeekday = 1;
                break;
            case "Tuesday":
                intWeekday = 2;
                break;
            case "Wednesday":
                intWeekday = 3;
                break;
            case "Thursday":
                intWeekday = 4;
                break;
            case "Friday":
                intWeekday = 5;
                break;
            case "Saturday":
                intWeekday = 6;
                break;
            case "Sunday":
                intWeekday = 7;
                break;
            default:
                intWeekday = -1;
                break;
        }
        return intWeekday;
    }
}