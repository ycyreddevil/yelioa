using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Newtonsoft.Json.Linq;

public partial class TotalFeeManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["action"];

        if (action == null)
        {
            action = Request.Params["action"];
        }

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if ("getTotalFeeDatagrid".Equals(action))
            {
                Response.Write(getTotalFeeDatagrid());
            }
            else if ("importVoucher".Equals(action)) 
            {
                Response.Write(importVoucher());
            }
            else if ("getTotalTaxDatagrid".Equals(action))
            {
                Response.Write(getTotalTaxDatagrid());
            }
            else if ("exportExcel".Equals(action))
            {
                Response.Write(exportExcel());
            }
            Response.End();
        }
    }

    private string getTotalFeeDatagrid()
    {
        //string dept = Request.Form["dept"];
        string startTm = Request.Form["startTm"] + " 00:00:00";
        string endTm = DateTime.Parse(startTm).AddDays(-1).AddMonths(1).ToString("yyyy-MM-dd") + " 23:59:59";
        string type = Request.Form["type"].ToString();

        string[] salesHeader = { "出差车船费", "住宿费", "出差补贴", "实报实销", "餐费", "市内交通费", "会议费", "培训费", "办公用品", "工作餐","场地费","招待餐费","纪念品","外协劳务","外部人员机票/火车票","外部人员住宿费","外部人员交通费","学术会费","营销办公费"};
        string[] notSalesHeader = { "出差车船费", "交通费", "汽车使用费", "实报实销", "住宿费", "出差补贴", "业务招待费", "培训费","办公费","租赁费","劳保费","通讯费","福利费","物业费","水电费","招聘费","运输费","材料费","试验费","检测费","专利费","注册费","服务费","其他"};

        string sql = "";

        string[] feeDetailHeader = { };

        if (type == "1" || type == "3" || type == "6")
            feeDetailHeader = notSalesHeader;
        else
            feeDetailHeader = salesHeader;

        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

        List<string> nameList = new List<string>();

        foreach (string header in feeDetailHeader)
        {
            if (type == "1")
            {
                // 业力职能
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)) amount, ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company like '江西业力医疗器械%' and t1.department like '%业力%' and t2.receiptType = '{2}' and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "2")
            {
                // 业力销售
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)) amount, ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company like '江西业力医疗器械%' and t1.department not like '%业力%' and t2.receiptType = '{2}' and t2.status != '拒绝' and " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "3")
            {
                // 中申职能
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company like '南昌市中申%' and t1.department like '%中申%' and t2.receiptType = '{2}' and t2.status != '拒绝' and " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "4")
            {
                // 中申销售
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company like '南昌市中申%' and t1.department not like '%中申%' and t2.receiptType = '{2}' and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "5")
            {
                // 东森销售
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company like '江西东森科技发展%' and t1.department like '%销售部%' and t2.receiptType = '{2}' and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "6")
            {
                // 东森职能
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company like '江西东森科技发展%' and t1.department not like '%销售部%' and t2.receiptType = '{2}'  and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "7")
            {
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company = '江西业力科技集团有限公司' and t2.receiptType = '{2}' and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "8")
            {
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company = '南昌老康科技有限公司' and t2.receiptType = '{2}' and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "9")
            {
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company = '天津吉诺泰普生物科技有限公司' and t2.receiptType = '{2}' and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "10")
            {
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company = '南昌业力医学检验实验室有限公司' and t2.receiptType = '{2}' and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }
            else if (type == "11")
            {
                sql = string.Format("select t1.name, (sum(t2.receiptAmount)-ifnull(sum(t2.receiptTax), 0)), ifnull(sum(t2.receiptTax), 0) tax,t1.fee_department,t1.fee_detail from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                    " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                    "and t1.fee_company = '九江傲沐科技发展有限公司' and t2.receiptType = '{2}' and t2.status != '拒绝' " +
                    "group by t1.name,t1.fee_department,t1.fee_detail", startTm, endTm, header);
            }

            DataTable dt = SqlHelper.Find(sql).Tables[0];

            if (dt.Rows.Count == 0)
            {
                if (result.Count > 0)
                {
                    foreach (Dictionary<string, object> temp in result)
                    {
                        string convertHeader = convertFeeDetailHeader(header);
                        if (!temp.ContainsKey(convertHeader))
                            temp.Add(convertHeader, 0);
                    }
                }

                continue;
            }

            foreach (DataRow dr in dt.Rows)
            {
                string name = dr[0].ToString();
                double amount = double.Parse(dr[1].ToString());
                double tax = double.Parse(dr[2].ToString());
                string fee_department = dr[3].ToString();
                string fee_detail = dr[4].ToString();

                Boolean hasRepeatName = false;

                if (result.Count > 0)
                {
                    foreach (Dictionary<string, object> tempDict in result)
                    {
                        if (tempDict["姓名"].ToString() == name && tempDict["部门"].ToString() == fee_department && tempDict["明细"].ToString() == fee_detail)
                        {
                            hasRepeatName = true;

                            double totalTax = double.Parse(tempDict["税额"].ToString()) + tax;
                            double total = double.Parse(tempDict["总计"].ToString()) + amount + double.Parse(tempDict["税额"].ToString());

                            if (!tempDict.ContainsKey(convertFeeDetailHeader(header)))
                                tempDict.Add(convertFeeDetailHeader(header), amount);
                            else
                                tempDict[convertFeeDetailHeader(header)] = double.Parse(tempDict[convertFeeDetailHeader(header)].ToString()) + amount;

                            if (!tempDict.ContainsKey("税额"))
                                tempDict.Add("税额", Math.Round(totalTax, 3));
                            else
                                tempDict["税额"] = totalTax;

                            if (!tempDict.ContainsKey("总计"))
                                tempDict.Add("总计", Math.Round(total, 3));
                            else
                                tempDict["总计"] = total;

                            break;
                        }
                    }
                }

                if (!hasRepeatName)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>
                    {
                        { "姓名", name },
                        { "部门", fee_department},
                        { "明细", fee_detail},
                        { convertFeeDetailHeader(header), amount },
                        { "税额", tax },
                        { "总计", amount+tax}
                    };

                    result.Add(temp);
                }
            }
        }

        foreach (string tempHeader in feeDetailHeader)
        {
            foreach (Dictionary<string, object> temp in result)
            {
                if (!temp.ContainsKey(convertFeeDetailHeader(tempHeader)))
                    temp.Add(convertFeeDetailHeader(tempHeader), 0);
            }
        }

        // 加上核销 以及汇总
        foreach (Dictionary<string, object> temp in result)
        {
            string name = temp["姓名"].ToString();
            string fee_department = temp["部门"].ToString();
            string fee_detail = temp["明细"].ToString();

            sql = string.Format("select ifnull(sum(t2.receiptAmount), 0) from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') where " +
                "t2.createTime between '{0}' and '{1}' and t1.name = '{2}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t2.status != '拒绝' and t1.isPrepaid = '1' and t1.fee_department = '{3}' and t1.fee_detail = '{4}' ", startTm, endTm, name, fee_department, fee_detail);

            double isPrepaidAmount = double.Parse(SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString());

            temp.Add("核销", isPrepaidAmount);

            double total = double.Parse(temp["总计"].ToString());

            total -= isPrepaidAmount;

            temp["总计"] = total;
        }

        List<Dictionary<string, object>> finalResult = new List<Dictionary<string, object>>();

        // 最后排序
        List<string> keyList = new List<string>();

        foreach (Dictionary<string, object> temp in result)
        {
            Dictionary<string, object> tempDict = new Dictionary<string, object>();

            foreach (string key in temp.Keys)
            {
                if (!keyList.Contains(key))
                {
                    keyList.Add(key);
                }

                if (key != "核销" && key != "总计" && key != "税额")
                {
                    tempDict.Add(key, temp[key]);
                }
            }

            tempDict.Add("税额", temp["税额"]);
            tempDict.Add("核销", temp["核销"]);
            tempDict.Add("总计", temp["总计"]);

            finalResult.Add(tempDict);
        }

        Dictionary<string, object> additionDict = new Dictionary<string, object>();

        // 增加竖向汇总
        foreach (string key in keyList)
        {
            double tempAmount = 0;

            if (key == "姓名")
            {
                additionDict.Add(key, "总计");
                continue;
            }
            else if (key == "部门" || key == "明细")
            {
                additionDict.Add(key, "");
                continue;
            }

            foreach (Dictionary<string, object> temp in finalResult)
            {
                tempAmount += double.Parse(temp[key].ToString());
            }

            additionDict.Add(key, tempAmount);
        }

        finalResult.Add(additionDict);

        return JsonHelper.SerializeObject(finalResult);
    }

    private string convertFeeDetailHeader(string feeDetail)
    {
        if (feeDetail == "出差车船费" || feeDetail == "住宿费" || feeDetail == "出差补贴" || feeDetail == "实报实销")
            return "差旅费";
        else if (feeDetail == "餐费")
            return "业务招待费";
        else if (feeDetail == "市内交通费")
            return "交通费";
        else if (feeDetail == "办公用品")
            return "办公费";
        else if (feeDetail == "营销办公费")
            return "业务宣传费";
        else if (feeDetail == "工作餐" || feeDetail == "场地费" || feeDetail == "招待餐费" || feeDetail == "纪念品" || feeDetail == "外协劳务" || feeDetail == "外部人员机票/火车票" || feeDetail == "外部人员住宿费" ||
             feeDetail == "外部人员交通费" || feeDetail == "学术会费")
            return "会议费";
        else
            return feeDetail;
    }

    private string getTotalTaxDatagrid()
    {
        string startTm = Request.Form["startTm"] + " 00:00:00";
        string endTm = DateTime.Parse(startTm).AddDays(-1).AddMonths(1).ToString("yyyy-MM-dd") + " 23:59:59";
        string type = Request.Form["type"].ToString();

        string sql = "";

        if (type == "1")
        {
            // 业力职能
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company like '江西业力医疗器械%' and t1.department like '%业力%' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "2")
        {
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company like '江西业力医疗器械%' and t1.department not like '%业力%' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "3")
        {
            // 中申职能
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company like '南昌市中申%' and t1.department like '%中申%' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "4")
        {
            // 中申销售
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company like '南昌市中申%' and t1.department not like '%中申%' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "5")
        {
            // 东森销售
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company like '江西东森科技发展%' and t1.department like '%销售部%' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "6")
        {
            // 东森职能
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company like '江西东森科技发展%' and t1.department not like '%销售部%' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "7")
        {
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company = '江西业力科技集团有限公司' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "8")
        {
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company = '南昌老康科技有限公司%' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "9")
        {
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company = '天津吉诺泰普生物科技有限公司' and t1.department like '%业力%' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "10")
        {
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company = '南昌业力医学检验实验室有限公司' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }
        else if (type == "11")
        {
            sql = string.Format("select t2.activityDate 日期, t1.name 人员名称, t2.receiptAmount 含税金额, t2.receiptTax 税额, (t2.receiptAmount - t2.receiptTax) 不含税金额, TRUNCATE(t2.receiptTax/(t2.receiptAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
                "from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat('%', t1.code, '%') " +
                " where t2.createTime between '{0}' and '{1}' and t1.status = '已审批' and (t1.account_result != '拒绝' or t1.account_result is null) " +
                "and t1.fee_company = '九江傲沐科技发展有限公司' and t2.status != '拒绝' and t2.receiptTax != 0 and t2.receiptTax is not null "
                , startTm, endTm);
        }

        return JsonHelper.DataTable2Json(SqlHelper.Find(sql).Tables[0]);
    }

    private string importVoucher()
    {
        //SignatureManage manage = new SignatureManage();

        //string authorization = manage.CreateAuthorizationHeader("f1646512-a188-4a67-8d7d-42dc8cf05ce9", "agtcii", "", "C:/Users/Administrator/Desktop/OpenAPI认证资料/cjet_pri.pem", null);

        //JObject nvc = new JObject
        //{
        //    { "userName", "rjcs" },
        //    { "password", manage.GetMd5("ycy111") },
        //    { "accNum", "201902" }
        //};

        //NameValueCollection nvc2 = new NameValueCollection();

        //nvc2.Add("_args", nvc.ToString());

        //var wc = new WebClient();

        //wc.Headers.Add(HttpRequestHeader.Authorization, authorization);

        //string res = HttpHelper.Post("http://17t7n77466.51mypc.cn/TPlus/api/v2/collaborationapp/GetRealNameTPlusToken?IsFree=1", nvc2, wc);

        //OpenAPI api = new OpenAPI("http://17t7n77466.51mypc.cn/TPlus/api/v1/", new Credentials()
        //{
        //    AppKey = "f1646512-a188-4a67-8d7d-42dc8cf05ce9",
        //    AppSecret = "agtcii",
        //    UserName = "rjcs",
        //    Password = manage.GetMd5("ycy111"),
        //    AccountNumber = "201901",
        //    LoginDate = DateTime.Now.ToString("yyyy-MM-dd")
        //});

        //api.ConnectTest();
        //api.GetToken();

        //api.Call("doc/Create",                    //调用凭证的创建服务
        //    @"{  
        //dto: {
        //    ExternalCode:""002"",
        //    DocType: { Code: ""记"" }, 
        //    VoucherDate: """ + DateTime.Now.ToString("yyyy-MM-dd") + @""",
        //    Entrys: [
        //        { 
        //            Account: { ""Code"": ""1001"" },
        //            Currency: { ""Code"": ""RMB"" },
        //            Summary: ""提现"", AmountCr: ""100""
        //        }
        //        , { 
        //            Account: { ""Code"": ""1002"" },
        //            Currency: { ""Code"": ""RMB"" },
        //            Summary: ""提现"", AmountDr: ""100""
        //        }]
        //    }
        //}");
        


        return null;
    }

    private string exportExcel()
    {
        JObject res = new JObject();
        DateTime date = Convert.ToDateTime(Request.Form["date"]);
        string type = Request.Form["type"];
        string strData = Request.Form["dataTotal"];
        DataTable dtTotal = JsonHelper.Json2Dtb(strData);
        DataTable dtFax = JsonHelper.Json2Dtb(Request.Form["dataFax"]);

        int totalLen = dtTotal.Rows.Count;
        if (totalLen == 0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "导入数据为空！");
            return res.ToString();
        }

        DataTable dt = new DataTable();
        for(int i = 1; i <= 55; i++)
        {
            dt.Columns.Add(i.ToString());
        }

        //借方汇总
        DataRow row = dtTotal.Rows[totalLen - 1];
        foreach(DataColumn c in dtTotal.Columns)
        {
            if (c.ColumnName == "姓名" || c.ColumnName == "总计")
                continue;
            DataRow newRow = dt.NewRow();
            newRow[0] = DateTime.Now.ToString("yyyy-MM-dd");
            newRow[1] = "记账凭证";
            newRow[1] = "请手动修改！";
            newRow[6] = "XX.XX支付{0}月";
        }
       


        //借方税金
        int faxLen = dtFax.Rows.Count;

        //贷方




        return res.ToString();
    }
}