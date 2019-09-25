using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ExportSalesManage 的摘要说明
/// </summary>
public class ExportSalesManage
{
    public ExportSalesManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static Dictionary<string, string> ImportInfos(Dictionary<string, string> dict, string type)
    {
        string code = "";string date = "";string salesNumber = "";string docNo = "";
        string batchNo = "";string clientCode = "";string hospitalCode = "";string style = "";

        if ("jd".Equals(type))
        {
            style = "xs";
            code = dict["产品代码"].ToString();
            date = dict["日期"].ToString();
            if (dict.ContainsKey("实发数量"))
            {
                salesNumber = dict["实发数量"].ToString();
            }
            else
            {
                salesNumber = dict["数量"].ToString();
            }
            docNo = dict["单据编号"].ToString();
            if (dict.ContainsKey("批号"))
            {
                batchNo = dict["批号"].ToString();
            }
            if (dict.ContainsKey("结算单位代码"))
            {
                hospitalCode = dict["结算单位代码"].ToString();
            }
            else
            {
                hospitalCode = dict["客户代码"].ToString();
            }

            if (!"Y".Equals(dict["审核标志"].ToString()))
            {
                dict["状态"] = "该单据未过账";
                return dict;
            }
        }
        else
        {
            code = dict["器械代码"].ToString();
            date = dict["日期"].ToString();
            salesNumber = dict["基本数量"].ToString();
            docNo = dict["单号"].ToString();
            clientCode = dict["客户代码"].ToString();
            if (dict.ContainsKey("终端客户代码"))
            {
                hospitalCode = dict["终端客户代码"].ToString();
                style = "xs";
            }
            else
            {
                style = "db";
            }
            if (dict.ContainsKey("批号/序列号"))
            {
                batchNo = dict["批号/序列号"].ToString();
            }
            if (dict.ContainsKey("过帐") && "".Equals(dict["过帐"].ToString()))
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

        DataSet ds = ExportSalesSrv.findDuplicate(docNo, batchNo, code, date);

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            dict["状态"] = "单据重复";
            return dict;
        }

        Dictionary<string, string> resultDict = new Dictionary<string, string>();

        resultDict.Add("code", code);
        resultDict.Add("date", date);
        resultDict.Add("salesNumber", salesNumber);
        resultDict.Add("docno", docNo);
        resultDict.Add("batchNo", batchNo);
        resultDict.Add("clientCode", clientCode);
        resultDict.Add("hospitalCode", hospitalCode);
        resultDict.Add("style", style);

        if ("jd".Equals(type))
        {
            resultDict.Add("type", "jd");
        }
        else if ("jb".Equals(type))
        {
            resultDict.Add("type", "jb");
        }

        string res = ExportSalesSrv.ImportInfos(resultDict);

        if (!string.IsNullOrEmpty(res))
        {
            if (res.Contains("操作成功"))
                dict["状态"] = "已导入";
            else
                dict["状态"] = res;
        }

        return dict;
    }

    public static DataTable findByDate(string date, string style)
    {
        DataSet ds = ExportSalesSrv.findByDate(date, style);

        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    public static DataTable findByMonth(string date, string style)
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

        if ((29 - day) < lastDay)
        {
            fifthWeekFirstDay = date + "-" + (30 - day);
            fifthWeekLastDay = date + "-" + lastDay;
            fifthWeekCond = "between '" + fifthWeekFirstDay + "' and '" + fifthWeekLastDay + "'";
        }

        string monthFirstDay = date + "-1"; string monthLastDay = date + "-" + lastDay;

        DataSet ds = ExportSalesSrv.findByMonth(firstWeekCond, secondWeekCond, thirdWeekCond, fourthWeekCond, fifthWeekCond, monthFirstDay, monthLastDay, style);

        if (ds == null)
            return null;

        return ds.Tables[0];
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