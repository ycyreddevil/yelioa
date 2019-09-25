using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// SalesTaskInfoManage 的摘要说明
/// </summary>
public class SalesTaskInfoManage
{
    public SalesTaskInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable getInfos()
    {
        DataSet ds = SalesTaskInfoSrv.getInfos();
        DataTable dt = null;
        if (ds != null)
        {
            dt = ds.Tables[0];
        }
        return dt;
    }

    public static Dictionary<string, string> ImportInfos(Dictionary<string, string> dict)
    {
        Dictionary<string, string> fieldDict = new Dictionary<string, string>();


        fieldDict.Add("代表", "Sales");
        fieldDict.Add("主管", "Supervisor");
        fieldDict.Add("盈利中心经理", "Manager");
        fieldDict.Add("总监", "Director");
        fieldDict.Add("部门", "Department");
        fieldDict.Add("区域", "Sector");
        fieldDict.Add("医院", "Hospital");
        fieldDict.Add("产品", "Product");

        fieldDict.Add("税金", "TaxCost");
        fieldDict.Add("财务费用金额", "FinancialCost");
        fieldDict.Add("研发费用金额", "RdCost");
        fieldDict.Add("销售总监费用（0.5%）", "SalesDirectorCost");
        fieldDict.Add("开发费用金额", "DevelopmentCost");
        fieldDict.Add("总部管理费用（3.5%）", "HeadOfficeManageCost");
        fieldDict.Add("产品发展基金", "ProductDevelopmentFundCost");
        fieldDict.Add("市场学术费", "MarketCost");
        fieldDict.Add("市场调节基金", "MarketReadjustmentCost");
        fieldDict.Add("PMS/支", "PmsCost");
        fieldDict.Add("包干费6%", "GuestMaintenanceCost");
        fieldDict.Add("商务费用金额", "BusinessCost");
        fieldDict.Add("实验人员奖金金额", "ExperimenterBonusCost");
        fieldDict.Add("工资社保金额", "WageSocialSecurityCost");
        fieldDict.Add("实验费(TF)金额", "TfCost");
        fieldDict.Add("区域中心费用", "RegionalCenterCost");
        fieldDict.Add("区域中心费用VIP维护", "RegionalCenterVipCost");

        fieldDict.Add("医院供货价", "HospitalSupplyPrice");
        fieldDict.Add("公司开票价", "InvoicePrice");
        fieldDict.Add("销售折让", "SalesAllowances");
        fieldDict.Add("考核价", "ExaminePrice");
        fieldDict.Add("采购成本", "PurchasingCost");
        fieldDict.Add("毛利", "GrossProfit");

        int totalYearTask = 0;
        for(int i = 1;i<=12;i++)
        {
            fieldDict.Add(i + "月", "MonthTask" + i);
            try
            {
                int monthTask = Convert.ToInt32(dict[i + "月"]);
                totalYearTask += monthTask;
            }
            catch { }
        }
            
        

        foreach (string key in fieldDict.Keys)
        {
            if (!dict.Keys.Contains(key))
            {
                dict["状态"] = string.Format("文件中未找到“{0}”列", key);
                return dict;
            }
            string val = dict[key];
            if (!string.IsNullOrEmpty(val) && StringTools.IsNumeric(val) && (!fieldDict[key].Contains("Ratio")))
                val = (Convert.ToDouble(val)).ToString("F2");
            dict.Add(fieldDict[key], val);
        }
        dict.Add("Year", DateTime.Now.Year.ToString());
        dict.Add("YearTask", totalYearTask.ToString());

        string res = SalesTaskInfoSrv.ImportInfos(dict);
        if (!string.IsNullOrEmpty(res))
        {
            if (res.Contains("操作成功"))
                dict["状态"] = "已导入";
            else
                dict["状态"] = res;
        }
        return dict;
    }
}