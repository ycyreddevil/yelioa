using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

/// <summary>
/// CostSharingInfoManage 的摘要说明
/// </summary>
public class CostSharingInfoManage
{
    public CostSharingInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    private static string[] NoNumeriqueField = { "SalesId", "SupervisorId", "ManagerId", "DirectorId" , "Department"
        ,"Sector","HospitalId","ProductId","Specification","HospitalDepartment","HospitalCode","ProductCode"};

    public static DataTable getInfos(string searchString)
    {
        DataSet ds = CostSharingInfoSrv.getInfos();
        DataTable dt = null;
        if (ds != null)
        {
            if (string.IsNullOrEmpty(searchString))//搜索字符为空时，不搜索，直接返回
            {
                return ds.Tables[0];
            }
            dt = ds.Tables[0].Clone();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (PinYinHelper.IsEqual(row["Department"].ToString(), searchString)
                    || row["Department"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["Sector"].ToString(), searchString)
                    || row["Sector"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["Hospital"].ToString(), searchString)
                    || row["Hospital"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["Product"].ToString(), searchString)                    
                    || row["Product"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["Sales"].ToString(), searchString)
                    || row["Sales"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["Supervisor"].ToString(), searchString)
                    || row["Supervisor"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["Manager"].ToString(), searchString)
                    || row["Manager"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["Director"].ToString(), searchString)
                    || row["Director"].ToString().Trim().Contains(searchString))
                {
                    dt.Rows.Add(row.ItemArray);
                    continue;
                }
            }
        }
        return dt;
        //return CostSharingInfoSrv.getInfos();
    }

    public static string InsertInfos(DataTable dt)
    {
        dt.Columns["代表"].ColumnName = "SalesId";
        dt.Columns["主管"].ColumnName = "SupervisorId";
        dt.Columns["盈利中心经理"].ColumnName = "ManagerId";
        dt.Columns["总监"].ColumnName = "DirectorId";
        dt.Columns["部门"].ColumnName = "DepartmentId";
        dt.Columns["区域"].ColumnName = "SectorId";
        dt.Columns["医院"].ColumnName = "HospitalId";
        dt.Columns["产品"].ColumnName = "ProductId";
        dt.Columns["医院供货价"].ColumnName = "HospitalSupplyPrice";
        dt.Columns["公司开票价"].ColumnName = "InvoicePrice";
        dt.Columns["销售折让"].ColumnName = "SalesAllowances";
        dt.Columns["税点"].ColumnName = "TaxRatio";
        dt.Columns["包干费6%"].ColumnName = "GuestMaintenanceRatio";
        dt.Columns["财务费用占比"].ColumnName = "FinancialRatio";
        dt.Columns["研发费用占比（3%）"].ColumnName = "RdRatio";
        dt.Columns["开发费用占比（2%）"].ColumnName = "DevelopmentRatio";
        dt.Columns["产品发展基金占比"].ColumnName = "ProductDevelopmentFundRatio";
        dt.Columns["市场学术费占比"].ColumnName = "MarketRatio";
        dt.Columns["市场调节基金占比"].ColumnName = "MarketReadjustmentRatio";
        dt.Columns["商务费用占比（1%）"].ColumnName = "BusinessRatio";
        dt.Columns["实验人员奖金占比"].ColumnName = "ExperimenterBonus";
        dt.Columns["工资社保占比（10%）"].ColumnName = "WageSocialSecurityRatio";
        dt.Columns["实验费(TF)占比（0.5%）"].ColumnName = "TfRatio";
        dt.Columns["区域中心费用比例"].ColumnName = "RegionalCenterRatio";
        int len = dt.Columns.Count;
        for(int i= len-1;i>=0;i--)
        {
            string name = dt.Columns[i].ColumnName;
            if (StringTools.HasChinese(name))
            {
                dt.Columns.RemoveAt(i);
            }            
        }

        return CostSharingInfoSrv.InsertInfos(dt);
    }


    public static string InsertInfos(Dictionary<string, string> dict)
    {
        string res = CostSharingInfoSrv.InsertInfos(dict);
        SqlExceRes sqlRes = new SqlExceRes(res);
        return sqlRes.GetResultString("新建成功", "网点信息有重复，请重新输入");
    }

    public static void ChangeKeyValueOfDictionary(Dictionary<string, string> dict,string oldKey, string newKey)
    {
        if (!dict.Keys.Contains(oldKey) || dict.Keys.Contains(newKey))
        {
            return;
        }
        string value = dict[oldKey];
        dict.Remove(oldKey);
        dict.Add(newKey, value);
    }

    public static void AddKeyValueOfDictionary(ref Dictionary<string, string> dict, string oldKey, string newKey)
    {
        if (!dict.Keys.Contains(oldKey) || dict.Keys.Contains(newKey))
        {
            return;
        }
        string value = dict[oldKey];
        dict.Add(newKey, value);
    }

    public static Dictionary<string, string> NewImportInfos(Dictionary<string, string> dict,string index)
    {
        Dictionary<string, string> fieldDict = new Dictionary<string, string>();
        fieldDict.Add("网点代码", "ClientCode");
        fieldDict.Add("产品代码", "ProductCode");
        fieldDict.Add("代理商编码", "AgentCode");
        fieldDict.Add("医院", "ClientName");
        fieldDict.Add("产品", "ProductName");
        fieldDict.Add("代理商名称", "AgentName");
        fieldDict.Add("规格型号", "Specification");
        fieldDict.Add("单位", "Unit");
        fieldDict.Add("考核价", "AssessmentPrice");
        fieldDict.Add("区域（新）", "Department");
        fieldDict.Add("操作模式", "DepartmentTitle");
        fieldDict.Add("渠道代表", "UserId");
        fieldDict.Add("渠道经理", "UserId");
        fieldDict.Add("渠道负责人", "UserId");
        fieldDict.Add("代表", "UserId");
        fieldDict.Add("主管", "UserId");
        fieldDict.Add("区域经理", "UserId");
        fieldDict.Add("大区经理", "UserId");
        fieldDict.Add("销售负责人", "UserId");
        fieldDict.Add("省级负责人", "UserId");
        fieldDict.Add("地区级负责人", "UserId");
        fieldDict.Add("大区级负责人", "UserId");

        foreach (string key in fieldDict.Keys)
        {
            if (!dict.Keys.Contains(key))
            {
                dict["状态"] = string.Format("文件中未找到“{0}”列", key);
                return dict;
            }
        }
        if(string.IsNullOrEmpty(dict["网点代码"]) || dict["区域（新）"].Equals("自然流"))
        {
            dict["状态"] = "无需导入";
            return dict;
        }

        string res = CostSharingInfoSrv.NewImportInfos(dict,index);
        if (!string.IsNullOrEmpty(res))
        {
            if (res.Contains("操作成功"))
                dict["状态"] = "已导入";
            else
                dict["状态"] = res;
        }
        return dict;
    }

    public static Dictionary<string, string> ImportInfos(Dictionary<string, string> dict)
    {
        Dictionary<string, string> fieldDict = new Dictionary<string, string>();
        fieldDict.Add("网点代码", "HospitalCode");
        fieldDict.Add("产品代码", "ProductCode");
        fieldDict.Add("代表", "SalesId");
        fieldDict.Add("主管", "SupervisorId");
        fieldDict.Add("盈利中心经理", "ManagerId");
        fieldDict.Add("总监", "DirectorId");
        fieldDict.Add("部门", "Department");
        fieldDict.Add("区域", "Sector");
        fieldDict.Add("医院", "HospitalId");
        fieldDict.Add("产品", "ProductId");
        fieldDict.Add("规格", "Specification");
        fieldDict.Add("科室", "HospitalDepartment");

        fieldDict.Add("税点", "TaxRatio");
        fieldDict.Add("财务费用占比", "FinancialRatio");
        fieldDict.Add("研发费用占比（3%）", "RdRatio");
        fieldDict.Add("开发费用占比（2%）", "DevelopmentRatio");
        fieldDict.Add("产品发展基金占比", "ProductDevelopmentFundRatio");
        fieldDict.Add("市场学术费占比", "MarketRatio");
        fieldDict.Add("市场调节基金占比", "MarketReadjustmentRatio");
        fieldDict.Add("商务费用占比（1%）", "BusinessRatio");
        fieldDict.Add("实验人员奖金占比", "ExperimenterBonusRatio");
        fieldDict.Add("工资社保占比（10%）", "WageSocialSecurityRatio");
        fieldDict.Add("实验费(TF)占比（0.5%）", "TfRatio");
        fieldDict.Add("区域中心费用比例", "RegionalCenterRatio");

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
        fieldDict.Add("2017年营业利润", "OperatingProfitForYear");
        fieldDict.Add("2017年指标（数量）", "QuotaForYear");

        fieldDict.Add("医院供货价", "HospitalSupplyPrice");
        fieldDict.Add("公司开票价", "InvoicePrice");
        fieldDict.Add("销售折让", "SalesAllowances");
        fieldDict.Add("考核价", "ExaminePrice");
        fieldDict.Add("采购成本", "PurchasingCost");
        fieldDict.Add("毛利", "GrossProfit");
        fieldDict.Add("费用合计", "TotalCost");
        fieldDict.Add("营业利润", "OperatingProfit");
        fieldDict.Add("营业利润率", "OperatingProfitRatio");
        fieldDict.Add("企业所得税", "CorporateIncomeTaxRatio");
        fieldDict.Add("净利润", "NetProfit");
       

        foreach (string key in fieldDict.Keys)
        {
            if (!dict.Keys.Contains(key))
            {
                dict["状态"] = string.Format("文件中未找到“{0}”列", key);
                return dict;
            }
            if (dict.Keys.Contains(fieldDict[key]))
                dict.Remove(fieldDict[key]);
            string val = dict[key];
            if (!NoNumeriqueField.Contains(fieldDict[key])  )
            {
                if (!StringTools.IsNumeric(dict[key]))
                    val = "0";
                else if(fieldDict[key].Contains("Ratio"))
                    val = (Convert.ToDouble(val)).ToString("f4");
                else
                    val = (Convert.ToDouble(val)).ToString("f2");
            }
                
            dict.Add(fieldDict[key], val);
        }
        string res = CostSharingInfoSrv.ImportInfos(dict);
        if(!string.IsNullOrEmpty(res))
        {
            if(res.Contains("操作成功"))
                dict["状态"] = "已导入";
            else
                dict["状态"] = res;
        }
        return dict;
    }

    public static string UpdateInfos(Dictionary<string, string> dict, string id)
    {
        string res = CostSharingInfoSrv.UpdateInfos(dict,id);
        SqlExceRes sqlRes = new SqlExceRes(res);
        return sqlRes.GetResultString("修改成功", "修改失败！");
    }

    public static DataTable importCostSharing(HttpRequest request,Boolean flag)
    {
        HttpFileCollection httpFileCollection = request.Files;
        HttpPostedFile file = null;
        DataTable dt = new DataTable();
        if (httpFileCollection.Count > 0)
        {
            file = httpFileCollection[0];
            //Excel读取
            dt = Import(file.InputStream, 0, 0,flag);
        }

        return dt;
    }

    public static DataTable Import(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex,Boolean flag)
    {
        XSSFWorkbook wbXssf = null;
        HSSFWorkbook wbHssf = null;
        ISheet sheet = null;
        //        JObject jObject = new JObject();

        DataTable dt = new DataTable();
        try
        {
            wbXssf = new XSSFWorkbook(ExcelFileStream);
            sheet = wbXssf.GetSheetAt(SheetIndex);
        }
        catch (Exception ex)
        {
            if (ex.ToString().Contains("Wrong Local header signature"))
            {
                wbHssf = new HSSFWorkbook(ExcelFileStream);
                sheet = wbHssf.GetSheetAt(SheetIndex);
            }
        }
        finally
        {
            dt = ExcelHelperV2_0.Import(sheet, HeaderRowIndex);
            dt = CostSharingHelper.importCostSharing(dt, flag);

            //string result = CostSharingHelper.importCostSharing(dt, true);

            //if (result.Contains("成功"))
            //{
            //    jObject.Add("ErrCode", 0);
            //    jObject.Add("ErrMsg", "操作成功");
            //}
            //else
            //{
            //    jObject.Add("ErrCode", 1);
            //    jObject.Add("ErrMsg", "操作失败");
            //}

            ExcelFileStream.Close();
        }

        return dt;
    }
}