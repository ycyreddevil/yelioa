using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;

public partial class mFeeAnalyse : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mFeeAnalyse",
           "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
           "1000006",
        "http://yelioa.top/mFeeAnalyse.aspx");

        string res = wx.CheckAndGetUserInfo(HttpContext.Current);

        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            if (action.Equals("getFeeAnalyse"))
            {
                Response.Write(getFeeAnalyse());
            }
            Response.End();
        }
    }

    private string getFeeAnalyse()
    {
        var type = Request.Form["type"];
        var startDate = Request.Form["startTm"];

        var result = new List<Dictionary<string, object>>();

        if (type == "0")
        {
            //部门费用占比分析
            result = DepartmentFeeRatio(startDate, result);
        }
        else if (type == "1")
        {
            //科目费用占比分析
            result = FeeDetailFeeRatio(startDate, result);
        }
        else if (type == "2")
        {
            result = BudgetDiff(startDate, result);
        }
        else if (type == "3")
        {
            result = ExpenseRatio(startDate, result);
        }
        else if (type == "4")
        {
            result = GrossProfitRatio(startDate, result);
        }

        return JsonHelper.SerializeObject(result);
    }

    /// <summary>
    /// 部门费用占比 数据
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="result"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    private List<Dictionary<string, object>> DepartmentFeeRatio(string startDate, List<Dictionary<string, object>> result)
    {
        var sql = string.Format("select name from report_department", startDate);

        var depTable = SqlHelper.Find(sql).Tables[0];

        if (depTable.Rows.Count == 0)
            return result;

        double marketFee = 0.0;

        foreach (DataRow dr in depTable.Rows)
        {
            var depName = dr[0].ToString();
            var reimburse = 0.000;

            sql = string.Format("select ifnull(sum(fee_amount),0) from yl_reimburse where report_department = '{0}' and " +
                "(account_result != '拒绝' or account_result is null) and date_format(approval_time, '%Y-%m' ) = '{1}' and status = '已审批' and fee_detail != '固定资产' and fee_detail not like '%租赁费%'", depName, startDate);

            if (depName == "集团人力资源部")
                sql += " and fee_detail not like '%工资社保金额%'";

            reimburse = double.Parse(SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString());

            var otherReimburse = 0.000;

            sql = string.Format("select ifnull(sum(feeAmount),0) from yl_reimburse_other where reportDepartmentName = '{0}' and feeName != '流向' " +
                "and feeName != '纯销' and feeName != '毛利' and year = {1} and month = {2}", depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

            otherReimburse = double.Parse(SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString());

            var totalAmount = reimburse + otherReimburse;

            if (depName == "免疫线" || depName == "分子线" || depName == "自身免疫线" || depName == "病理线" || depName == "耗材线")
            {
                marketFee += totalAmount;
                continue;
            }

            var tempDict = new Dictionary<string, object>
            {
                { "name", depName },
                { "value", Math.Round(totalAmount / 10000, 3) },
                { "const", 100 }
            };
            //tempDict.Add("移动报销", reimburse);
            //tempDict.Add("未上移动报销", feeAmount);

            result.Add(tempDict);
        }

        var marketDict = new Dictionary<string, object>
        {
            { "name", "市场部" },
            { "value", Math.Round(marketFee / 10000, 3) },
            { "const", 100 }
        };

        result.Add(marketDict);

        return result;
    }

    /// <summary>
    /// 科目费用占比 数据
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private List<Dictionary<string, object>> FeeDetailFeeRatio(string startDate, List<Dictionary<string, object>> result)
    {
        string sql = string.Format("select distinct feeDetail from import_budget where date_format(createTime, '%Y-%m' ) = '{0}' and parentId = -1", startDate);

        var feeDetailDt = SqlHelper.Find(sql).Tables[0];

        var rdFeeAmount = 0.000;    // 研发费用

        foreach (DataRow dr in feeDetailDt.Rows)
        {
            var dict = new Dictionary<string, object>();

            string feeDetailName = dr[0].ToString();

            if (feeDetailName == "固定资产" || feeDetailName == "租赁费")
                continue;

            sql = string.Format("select ifnull(sum(fee_amount),0) from yl_reimburse where fee_detail like '{0}%' " +
                "and date_format(approval_time, '%Y-%m' ) = '{1}' and (account_result != '拒绝' or account_result is null) and status = '已审批'", feeDetailName, startDate);

            var dt = SqlHelper.Find(sql).Tables[0];

            if (dt.Rows.Count == 0)
                continue;

            double feeDetailAmount = Double.Parse(dt.Rows[0][0].ToString());

            if (feeDetailName.Contains("YL") || feeDetailName.Contains("LK") || feeDetailName.Contains("ZS") || feeDetailName.Contains("JT-RJ") ||
                feeDetailName.Contains("研发") || feeDetailName.Contains("设备组") || feeDetailName.Contains("试剂组"))
            {
                rdFeeAmount += feeDetailAmount;
                continue;
            }

            if (feeDetailName == "工资社保金额")
            {
                // 工资只取非移动报销中的数据
                sql = string.Format("select ifnull(sum(feeAmount),0) from yl_reimburse_other where feeName = '工资奖金' and year = {0} " +
                    "and month = {1}", int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

                feeDetailAmount = Double.Parse(SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString());

                dict.Add("name", "工资社保金额");
                dict.Add("value", Math.Round(feeDetailAmount / 10000, 3));
            }
            else if (feeDetailName == "制造费用")
            {
                sql = string.Format("select ifnull(sum(feeAmount),0) from yl_reimburse_other where (feeName = '水电费' or feeName = '厂房租金' or feeName = '灭菌费' or feeName = '制造费用') and year = {0} " +
                    "and month = {1}", int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

                feeDetailAmount += Double.Parse(SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString());

                dict.Add("name", "生产费用");
                dict.Add("value", Math.Round(feeDetailAmount / 10000, 3));
            }
            else
            {
                dict.Add("name", feeDetailName);
                dict.Add("value", Math.Round(feeDetailAmount / 10000, 3));
            }

            dict.Add("const", 100);

            result.Add(dict);
        }

        // 处理非移动报销科目
        sql = string.Format("select ifnull(feeAmount,0), feeName from yl_reimburse_other where feeName != '工资奖金' and feeName != '水电费' and feeName != '制造费用' " +
            "and feeName != '厂房租金' and feeName != '灭菌费' and feeName != '流向' and feeName != '纯销' and feeName != '毛利' and year = {0} and month = {1}", 
            int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

        DataTable tempDt = SqlHelper.Find(sql).Tables[0];

        Dictionary<string, object> additionalDict;

        foreach (DataRow dr in tempDt.Rows)
        {
            additionalDict = new Dictionary<string, object>();

            additionalDict.Add("value", Math.Round(Double.Parse(dr[0].ToString()) / 10000, 3));
            additionalDict.Add("name", dr[1].ToString());
            additionalDict.Add("const", 100);

            result.Add(additionalDict);
        }

        additionalDict = new Dictionary<string, object>();
        additionalDict.Add("name", "研发费用");

        sql = string.Format("select ifnull(sum(feeAmount),0) from yl_reimburse_other where feeName = '研发费用' and year = {0} " +
            "and month = {1}", int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

        rdFeeAmount += Double.Parse(SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString());

        additionalDict.Add("value", Math.Round(rdFeeAmount / 10000, 3));
        additionalDict.Add("const", 100);

        result.Add(additionalDict);

        return result;
    }
    
    /// <summary>
    /// 预算实际差异 数据
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private List<Dictionary<string, object>> BudgetDiff(string startDate, List<Dictionary<string, object>> result)
    {
        string sql = string.Format("select name from report_department");

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        double marketBudget = 0.0;
        double marketFee = 0.0;

        foreach (DataRow dr in dt.Rows)
        {
            string depName = dr[0].ToString();

            if (depName == "集团办公室")
                continue;

            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("name", depName);

            List<string> sqls = new List<string>();

            // 获取预算
            if (depName == "南昌老康科技")
            {
                sql = string.Format("SELECT sum(budget) FROM `import_budget` " +
                "where departmentId = 517 and date_format(createTime, '%Y-%m' ) = '{0}' and  (feeDetail like '%LK%' or feeDetail = '销售费用' or feeDetail like '%老康%')", startDate);
            }
            else if (depName == "南昌中申医疗")
            {
                sql = string.Format("SELECT sum(budget) FROM `import_budget` " +
                "where departmentId = 517 and date_format(createTime, '%Y-%m' ) = '{0}' and  (feeDetail like '%ZS%' or feeDetail = '制造费用' or feeDetail like '%中申%')", startDate);
            }
            else
            {
                sql = string.Format("SELECT a.budget,a.id FROM `import_budget` a LEFT JOIN department d on a.DepartmentId=d.Id " +
                "where a.parentId = -1 and date_format(createTime, '%Y-%m' ) = '{0}' and d.reportName = '{1}' order by a.Id;", startDate, depName);
            }
            sqls.Add(sql);

            // 获取非移动报销预算
            string other_sql = string.Format("select ifnull(sum(budget), 0) from yl_reimburse_other where reportDepartmentName = '{0}' and year = {1} and month = {2} and feeName != '流向' " +
                "and feeName != '纯销' and feeName != '毛利'",
                depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));
            sqls.Add(other_sql);

            if (depName == "集团行政部")
            {
                DataTable tempDt = SqlHelper.Find(sql).Tables[0];

                double tempBudget = double.Parse(tempDt.Rows[0][0].ToString());
                string id = tempDt.Rows[0][1].ToString();

                sqls = new List<string>();

                sql = string.Format("SELECT sum(a.budget) FROM `import_budget` a " +
                "where a.parentId = '{0}' and date_format(createTime, '%Y-%m' ) = '{1}' and (feeDetail = '差旅费' or feeDetail = '招待费')", id, startDate);

                sqls.Add(sql);

                other_sql = string.Format("select ifnull(sum(budget), 0) from yl_reimburse_other where reportDepartmentName = '{0}' and year = {1} and month = {2} and feeName != '流向' " +
                "and feeName != '纯销' and feeName != '毛利'",
                depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

                sqls.Add(other_sql);

                // 找集团办公室的实际费用
                string temp_sql = string.Format("select ifnull(sum(fee_amount),0) from yl_reimburse where report_department = '{0}' and " +
                "(account_result != '拒绝' or account_result is null) and date_format(approval_time, '%Y-%m' ) = '{1}' and status = '已审批'", "集团办公室", startDate);

                sqls.Add(temp_sql);

                string temp_other_sql = string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where reportDepartmentName = '{0}' and year = {1} and month = {2} and feeName != '流向' " +
                "and feeName != '纯销' and feeName != '毛利'",
                depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

                sqls.Add(temp_other_sql);

                DataSet tempds = SqlHelper.Find(sqls.ToArray());

                double temp_budget = double.Parse(tempds.Tables[0].Rows[0][0].ToString());

                temp_budget += double.Parse(tempds.Tables[1].Rows[0][0].ToString());

                Dictionary<string, object> temp_dict = new Dictionary<string, object>();

                temp_dict.Add("name", "集团办公室");
                temp_dict.Add("budget", Math.Round(temp_budget / 10000, 3));

                double temp_real_fee = double.Parse(tempds.Tables[2].Rows[0][0].ToString());
                temp_real_fee += double.Parse(tempds.Tables[3].Rows[0][0].ToString());

                temp_dict.Add("real_fee", Math.Round(temp_real_fee / 10000, 3));
                temp_dict.Add("diff", Math.Round((temp_budget - temp_real_fee) / 10000, 3));

                result.Add(temp_dict);

                tempBudget -= temp_budget;
                dict["budget"] = tempBudget;
            }

            // 再找除了集团办公室外的实际费用
            string fee_sql = string.Format("select ifnull(sum(fee_amount),0) from yl_reimburse where report_department = '{0}' and " +
            "(account_result != '拒绝' or account_result is null) and date_format(approval_time, '%Y-%m' ) = '{1}' and status = '已审批'", depName, startDate);

            sqls.Add(fee_sql);

            string other_fee_sql = "";

            if (depName == "集团行政部")
            {
                other_fee_sql = string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where (reportDepartmentName = '{0}' and feeName != '流向' " +
                "and feeName != '纯销' and feeName != '毛利' or feeName = '租赁费') and year = {1} and month = {2} ",
                depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));
            }
            else if (depName == "集团人力资源部")
            {
                other_fee_sql = string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where (reportDepartmentName = '{0}' and feeName != '流向' " +
                "and feeName != '纯销' and feeName != '毛利' or feeName = '工资社保金额') and year = {1} and month = {2} ",
                depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));
            }
            else
            {
                other_fee_sql = string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where reportDepartmentName = '{0}' and year = {1} and month = {2} and feeName != '流向' " +
                "and feeName != '纯销' and feeName != '毛利'", depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));
            }

            sqls.Add(other_fee_sql);

            DataSet ds = SqlHelper.Find(sqls.ToArray());

            double budget = dict.ContainsKey("budget") ? double.Parse(dict["budget"].ToString()) : double.Parse(ds.Tables[0].Rows[0][0].ToString());
            budget += double.Parse(ds.Tables[1].Rows[0][0].ToString());

            if (depName == "免疫线" || depName == "分子线" || depName == "自身免疫线" || depName == "病理线" || depName == "耗材线")
                marketBudget += budget;
            else
                dict["budget"] = Math.Round(budget / 10000, 3);

            double real_fee = double.Parse(ds.Tables[2].Rows[0][0].ToString());
            real_fee += double.Parse(ds.Tables[3].Rows[0][0].ToString());

            if (depName == "免疫线" || depName == "分子线" || depName == "自身免疫线" || depName == "病理线" || depName == "耗材线")
            {
                marketFee += real_fee;
            }
            else
            {
                dict["real_fee"] = Math.Round(real_fee / 10000, 3);
                dict["diff"] = Math.Round((budget - real_fee) / 10000, 3);

                result.Add(dict);
            }
        }

        Dictionary<string, object> market_dict = new Dictionary<string, object>();

        market_dict["name"] = "市场部";
        market_dict["budget"] = Math.Round(marketBudget / 10000, 3);
        market_dict["real_fee"] = Math.Round(marketFee / 10000, 3);
        market_dict["diff"] = Math.Round((marketBudget - marketFee) / 10000, 3);

        result.Add(market_dict);

        return result;
    }
    
    /// <summary>
    /// 总费用率分析
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private List<Dictionary<string, object>> ExpenseRatio(string startDate, List<Dictionary<string, object>> result)
    {
        string sql = string.Format("select name from report_department");

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        double marketFee = 0.0;
        double marketOtherFee = 0.0;
        double marketFlow = 0.0;

        foreach (DataRow dr in dt.Rows)
        {
            string depName = dr[0].ToString();

            // 移动报销总费用
            sql = string.Format("select ifnull(sum(fee_amount),0) from yl_reimburse where report_department = '{0}' and " +
            "(account_result != '拒绝' or account_result is null) and date_format(approval_time, '%Y-%m' ) = '{1}' and status = '已审批';", depName, startDate);

            // 非移动报销总费用
            sql += string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where reportDepartmentName = '{0}' and year = {1} and month = {2} and feeName != '流向' " +
                "and feeName != '纯销' and feeName != '毛利';",
                depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

            // 总流向
            sql += string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where reportDepartmentName = '{0}' and year = {1} and month = {2} and " +
                "feeName = '流向';", depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

            sql += string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where year = {0} and month = {1} and " +
                "feeName = '流向';", int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

            DataSet ds = SqlHelper.Find(sql);

            double fee = double.Parse(ds.Tables[0].Rows[0][0].ToString());
            double other_fee = double.Parse(ds.Tables[1].Rows[0][0].ToString());
            double flow = double.Parse(ds.Tables[2].Rows[0][0].ToString());
            double totalFlow = double.Parse(ds.Tables[3].Rows[0][0].ToString());

            // 市场部下面的线需要合并
            if (depName == "免疫线" || depName == "分子线" || depName == "自身免疫线" || depName == "病理线" || depName == "耗材线")
            {
                marketFee += fee;
                marketOtherFee += other_fee;
                marketFlow += flow;

                continue;
            }

            // 费用率 = 总费用/总流向
            double ratio = 0;

            if (flow == 0 && totalFlow != 0)
                ratio = Math.Round((fee + other_fee) / totalFlow, 4) * 100;
            else if (flow != 0)
                ratio = Math.Round((fee + other_fee) / flow, 4) * 100;

            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["Name"] = depName;
            dict["Value"] = ratio + "%";
            dict["const"] = 100;

            result.Add(dict);
        }

        Dictionary<string, object> marketDict = new Dictionary<string, object>();

        marketDict["Name"] = "市场部";
        marketDict["Value"] = Math.Round((marketFee + marketOtherFee) / marketFlow, 4) * 100 + "%";

        return result;
    }

    /// <summary>
    /// 总毛利率分析
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private List<Dictionary<string, object>> GrossProfitRatio(string startDate, List<Dictionary<string, object>> result)
    {
        string sql = string.Format("select name from report_department");

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        double marketGrossProfit = 0.0;
        double marketFlow = 0.0;

        foreach (DataRow dr in dt.Rows)
        {
            string depName = dr[0].ToString();

            // 总毛利
            sql += string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where reportDepartmentName = '{0}' and year = {1} and month = {2} and " +
                "feeName = '毛利';", depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

            // 总流向
            sql += string.Format("select ifnull(sum(feeAmount), 0) from yl_reimburse_other where reportDepartmentName = '{0}' and year = {1} and month = {2} and " +
                "feeName = '流向';", depName, int.Parse(startDate.Split('-')[0]), int.Parse(startDate.Split('-')[1]));

            DataSet ds = SqlHelper.Find(sql);

            double gross_profit = double.Parse(ds.Tables[0].Rows[0][0].ToString());
            double flow = double.Parse(ds.Tables[1].Rows[0][0].ToString());

            // 市场部下面的线需要合并
            if (depName == "免疫线" || depName == "分子线" || depName == "自身免疫线" || depName == "病理线" || depName == "耗材线")
            {
                marketGrossProfit = gross_profit;
                marketFlow += flow;

                continue;
            }

            // 费用率 = 总费用/总流向
            double ratio = Math.Round(gross_profit / flow, 4) * 100;

            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["Name"] = depName;
            dict["Value"] = ratio;

            result.Add(dict);
        }

        Dictionary<string, object> marketDict = new Dictionary<string, object>();

        marketDict["Name"] = "市场部";
        marketDict["Value"] = Math.Round(marketGrossProfit / marketFlow, 4) * 100 + "%";

        return result;
    }
}