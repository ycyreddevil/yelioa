using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data;

/// <summary>
/// FlowInfoManage 的摘要说明
/// </summary>
public class FlowInfoManage
{
    public FlowInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    private static double ConvertVeriy(object obj)
    {
        double res = 0;
        if (obj != null)
        {
            try
            {
                res = Convert.ToDouble(obj);
            }
            catch (Exception ex)
            {

            }
        }
        return res;
    }

    public static DataTable GetMobileInfos(int year, int month, string searchString, string hospital, string product)
    {
        //DateTime d = DateTime.Now;
        //if (!string.IsNullOrEmpty(date))
        //    d = Convert.ToDateTime(date);
        DataSet ds = FlowInfoSrv.GetMobileInfo(year, month);
        DataTable dt = null;
        if (ds != null)
        {
            dt = ds.Tables[0];
            DataTable resDt = dt.Clone();
            UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
            if (!Privilege.checkPrivilege(user))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    resDt.Rows.Add(dr.ItemArray);
                    string ProductCode = dr["ProductCode"].ToString();
                    string HospitalCode = dr["HospitalCode"].ToString();
                    string Sales = dr["Sales"].ToString();

                    ds = FlowInfoSrv.GetMobileSimilarInfo(ProductCode, HospitalCode, Sales, year, month);
                    if (ds != null)
                    {
                        foreach (DataRow tempDr in ds.Tables[0].Rows)
                        {
                            if (tempDr != null)
                            {
                                resDt.Rows.Add(tempDr.ItemArray);
                            }
                        }
                    }
                }
                dt = resDt;
            }

            if (string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(hospital)
                && string.IsNullOrEmpty(product))//搜索字符为空时，不搜索，直接返回
            {
                return dt;
            }
            else if (string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(hospital))
            {
                dt = dt.Clone();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (checkProductEqual(row, product))
                    {
                        dt.Rows.Add(row.ItemArray);
                        continue;
                    }
                }
            }else if (string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(product))
            {
                dt = dt.Clone();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (checkHospitalEqual(row, hospital))
                    {
                        dt.Rows.Add(row.ItemArray);
                        continue;
                    }
                }
            }else if (string.IsNullOrEmpty(hospital) && string.IsNullOrEmpty(product))
            {
                dt = dt.Clone();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (checkUserOrDepartmentEqual(row, searchString))
                    {
                        dt.Rows.Add(row.ItemArray);
                        continue;
                    }
                }
            }else if (string.IsNullOrEmpty(hospital))
            {
                dt = dt.Clone();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (checkUserOrDepartmentEqual(row, searchString) && checkProductEqual(row, product))
                    {
                        dt.Rows.Add(row.ItemArray);
                        continue;
                    }
                }
            }
            else if (string.IsNullOrEmpty(product))
            {
                dt = dt.Clone();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (checkUserOrDepartmentEqual(row, searchString) && checkHospitalEqual(row, hospital))
                    {
                        dt.Rows.Add(row.ItemArray);
                        continue;
                    }
                }
            }
            else if (string.IsNullOrEmpty(searchString))
            {
                dt = dt.Clone();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (checkHospitalEqual(row, hospital) && checkProductEqual(row, product))
                    {
                        dt.Rows.Add(row.ItemArray);
                        continue;
                    }
                }
            }
            else
            {
                dt = dt.Clone();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (checkHospitalEqual(row, hospital) && checkProductEqual(row, product) && checkUserOrDepartmentEqual(row, searchString))
                    {
                        dt.Rows.Add(row.ItemArray);
                        continue;
                    }
                }
            }


            //dt = dt.Clone();
            //foreach (DataRow row in ds.Tables[0].Rows)
            //{
            //    if ((PinYinHelper.IsEqual(row["Department"].ToString(), searchString)
            //        || row["Department"].ToString().Trim().Contains(searchString)
            //        || PinYinHelper.IsEqual(row["Sector"].ToString(), searchString)
            //        || row["Sector"].ToString().Trim().Contains(searchString)
            //        || PinYinHelper.IsEqual(row["Sales"].ToString(), searchString)
            //        || row["Sales"].ToString().Trim().Contains(searchString)
            //        || PinYinHelper.IsEqual(row["Supervisor"].ToString(), searchString)
            //        || row["Supervisor"].ToString().Trim().Contains(searchString)
            //        || PinYinHelper.IsEqual(row["Manager"].ToString(), searchString)
            //        || row["Manager"].ToString().Trim().Contains(searchString)
            //        || PinYinHelper.IsEqual(row["Director"].ToString(), searchString)
            //        || row["Director"].ToString().Trim().Contains(searchString))
            //        && (PinYinHelper.IsEqual(row["Hospital"].ToString(), hospital)
            //        || row["Hospital"].ToString().Trim().Contains(hospital))
            //        && (PinYinHelper.IsEqual(row["Product"].ToString(), product)
            //        || row["Product"].ToString().Trim().Contains(product)))
            //    {
            //        dt.Rows.Add(row.ItemArray);
            //        continue;
            //    }
            //}
        }
        return dt;
    }

    private static Boolean checkUserOrDepartmentEqual(DataRow row, string searchString)
    {
        return PinYinHelper.IsEqual(row["Department"].ToString(), searchString)
        || row["Department"].ToString().Trim().Contains(searchString)
        || PinYinHelper.IsEqual(row["Sector"].ToString(), searchString)
        || row["Sector"].ToString().Trim().Contains(searchString)
        || PinYinHelper.IsEqual(row["Sales"].ToString(), searchString)
        || row["Sales"].ToString().Trim().Contains(searchString)
        || PinYinHelper.IsEqual(row["Supervisor"].ToString(), searchString)
        || row["Supervisor"].ToString().Trim().Contains(searchString)
        || PinYinHelper.IsEqual(row["Manager"].ToString(), searchString)
        || row["Manager"].ToString().Trim().Contains(searchString)
        || PinYinHelper.IsEqual(row["Director"].ToString(), searchString)
        || row["Director"].ToString().Trim().Contains(searchString);
    }

    private static Boolean checkHospitalEqual(DataRow row, string hospital)
    {
        return PinYinHelper.IsEqual(row["Hospital"].ToString(), hospital)
                    || row["Hospital"].ToString().Trim().Contains(hospital);
    }

    private static Boolean checkProductEqual(DataRow row, string product)
    {
        return PinYinHelper.IsEqual(row["Product"].ToString(), product)
                    || row["Product"].ToString().Trim().Contains(product);
    }

    public static DataSet GetMobileDetail(string id)
    {
        return FlowInfoSrv.GetMobileDetail(id);
    }

    public static DataSet GetInfos(string date,ref bool DataIsArchived)
    {
        DataSet dsRes = new DataSet();
        DateTime d = DateTime.Now;
        if(!string.IsNullOrEmpty(date))
            d = Convert.ToDateTime(date);
        DataSet ds = FlowInfoSrv.GetInfo(d);
        DataTable dt = null;
        DataIsArchived = false;
        if (ds == null || ds.Tables.Count==0)
        {
            return null;
        }
        else if(ds.Tables[1].Rows.Count>0)
        {
            dt = ds.Tables[1];
            DataIsArchived = true;
        }
        else
        {
            ArrayList listHospitalProduct = new ArrayList();//用于保存已计算了流向的医院产品名称
            dt = ds.Tables[0];
            //dt.Columns.Add("ExaminePrice");//考核价
            //dt.Columns.Add("SalesAmount");//销售额
            //dt.Columns.Add("GrossProfit");//毛利
            //dt.Columns.Add("FixedAssetsCost");//固定资产分摊
            //dt.Columns.Add("FinancialCost");//财务费用
            //dt.Columns.Add("RdCost");//研发费用
            ////dt.Columns.Add("HeadOfficeManageCost");//总部管理费用
            //dt.Columns.Add("WageSocialSecurityCost");//工资社保
            //dt.Columns.Add("TaxCost");//税金
            //dt.Columns.Add("BusinessCost");//商务费用
            //dt.Columns.Add("DevelopmentCost");//开发费用
            ////dt.Columns.Add("SalesDirectorCost");//销售总监费用
            //dt.Columns.Add("ProductDevelopmentFundCost");//产品发展基金
            //dt.Columns.Add("MarketCost");//市场学术费
            //dt.Columns.Add("MarketReadjustmentCost");//市场调节基金
            //dt.Columns.Add("ExperimenterBonusCost");//实验员奖金
            ////dt.Columns.Add("PmsCost");//PMS
            ////dt.Columns.Add("GuestMaintenanceCost");//日常客情维护费
            //dt.Columns.Add("TfCost");//实验费(TF)
            //dt.Columns.Add("RegionalCenterCost");//区域中心费用
            ////dt.Columns.Add("RegionalCenterVipCost");//区域中心费用VIP
            //dt.Columns.Add("TotalCost");//费用合计
            //dt.Columns.Add("OperatingProfit");//运营利润
            //dt.Columns.Add("OperatingProfitRatio");//运营利润率
            //dt.Columns.Add("OperatingProfitForYear");//运营利润率
            //dt.Columns.Add("NetProfit");//运营利润率
            dt.Columns.Add("FlowSales");//运营利润率
            dt.Columns.Add("FlowSalesMoney");//运营利润率
            dt.Columns.Add("NetSales");//运营利润率
            dt.Columns.Add("NetSalesMoney");//运营利润率
            dt.Columns.Add("StockThisMonth");//运营利润率
            dt.Columns.Add("StockLastMonth");//运营利润率
            dt.Columns.Add("Year");
            dt.Columns.Add("Month");
            dt.Columns.Add("CreateTime");
            //dt.Columns.Add("");
            for (int i=0;i<dt.Rows.Count;i++)
            {
                DataRow row = dt.Rows[i];
                double ExaminePrice = ConvertVeriy(row["ExaminePrice"]);
                //row["InvoicePrice"] = ConvertVeriy(row["InvoicePrice"]).ToString("f2");
                //row["ExaminePrice"] = ConvertVeriy(row["ExaminePrice"]).ToString("f2");
                //row["ExaminePrice"] = (ConvertVeriy(row["InvoicePrice"]) - ConvertVeriy(row["SalesAllowances"])).ToString("f2");
                //row["GrossProfit"] = (ExaminePrice - ConvertVeriy(row["PurchasingCost"])).ToString("f2");
                //row["FixedAssetsCost"] = ExaminePrice * ConvertVeriy(row["FixedAssetsRatio"])/100;
                //row["FinancialCost"] = ExaminePrice * ConvertVeriy(row["FinancialRatio"]) / 100;
                //row["RdCost"] = ExaminePrice * ConvertVeriy(row["RdRatio"]) / 100;
                ////row["HeadOfficeManageCost"] = ExaminePrice * ConvertVeriy(row["HeadOfficeManageRatio"]) / 100;
                //row["WageSocialSecurityCost"] = ExaminePrice * ConvertVeriy(row["WageSocialSecurityRatio"]) / 100;
                //row["TaxCost"] = ExaminePrice * ConvertVeriy(row["TaxRatio"]) / 100;
                //row["BusinessCost"] = ExaminePrice * ConvertVeriy(row["BusinessRatio"]) / 100;
                //row["DevelopmentCost"] = ExaminePrice * ConvertVeriy(row["DevelopmentRatio"]) / 100;
                ////row["SalesDirectorCost"] = ExaminePrice * ConvertVeriy(row["SalesDirectorRatio"]) / 100;
                //row["ProductDevelopmentFundCost"] = ExaminePrice * ConvertVeriy(row["ProductDevelopmentFundRatio"]) / 100;
                //row["MarketCost"] = ExaminePrice * ConvertVeriy(row["MarketRatio"]) / 100;
                //row["MarketReadjustmentCost"] = ConvertVeriy(row["MarketReadjustmentCost"]);
                //row["ExperimenterBonusCost"] = ExaminePrice * ConvertVeriy(row["ExperimenterBonusRatio"]) / 100;
                ////row["PmsCost"] = ConvertVeriy(row["PmsCost"]);
                ////row["GuestMaintenanceCost"] = ConvertVeriy(row["GuestMaintenanceCost"]);
                //row["TfCost"] = ExaminePrice * ConvertVeriy(row["TfRatio"]) / 100;
                //row["RegionalCenterCost"] = ExaminePrice * ConvertVeriy(row["RegionalCenterRatio"]) / 100;
                ////row["RegionalCenterVipCost"] = ConvertVeriy(row["RegionalCenterVipCost"]);                

                int flowSales = 0;
                for (int j = ds.Tables[3].Rows.Count - 1; j >= 0; j--)
                {
                    DataRow r = ds.Tables[3].Rows[j];
                    if (object.Equals(r["terminalClientId"], row["HospitalId"]) && object.Equals(r["ProductId"], row["ProductId"]))
                    {
                        //Dictionary<string, int> dict = new Dictionary<string, int>();
                        //dict.Add("h", Convert.ToInt32(r["terminalClientId"]));
                        //dict.Add("p", Convert.ToInt32(r["ProductId"]));
                        //listHospitalProduct.Add(dict);
                        flowSales += Convert.ToInt32(r["amountSend"]);
                        ds.Tables[3].Rows.RemoveAt(j);
                    }
                }
                
                row["FlowSales"] = flowSales;
                row["FlowSalesMoney"] = ExaminePrice * flowSales;
                row["SalesAmount"] = ExaminePrice * flowSales;
                row["NetSales"] = 0;
                row["NetSalesMoney"] = 0;

                //double totalCost = 0;
                //row["TotalCost"] = totalCost;
                //foreach (DataColumn col in dt.Columns)
                //{
                //    string val = row[col.ColumnName].ToString();
                    
                //    if (StringTools.IsNumeric(val) && !col.ColumnName.Contains("Id"))
                //    {
                //        if (col.ColumnName.Contains("Ratio"))                        
                //            row[col.ColumnName] = ConvertVeriy(row[col.ColumnName]).ToString("f4");                        
                //        else
                //            row[col.ColumnName] = ConvertVeriy(row[col.ColumnName]).ToString("f2");                        
                //    }
                    
                //    if (col.ColumnName.Contains("Cost"))
                //    {
                //        totalCost += ConvertVeriy(row[col.ColumnName]);
                //    }
                //}
                //通过出库数据查询流向
                //int flowSales = 0;
                //bool flowSalesHasBeenCounted = false;
                //foreach(Dictionary<string, int> dict in listHospitalProduct)
                //{
                //    if(dict["h"] == Convert.ToInt32(row["HospitalId"]) && dict["p"] == Convert.ToInt32(row["ProductId"]))
                //    {
                //        flowSalesHasBeenCounted = true;
                //        break;
                //    }
                //}
                //if(!flowSalesHasBeenCounted)
                //{
                //    foreach (DataRow r in ds.Tables[3].Rows)
                //    {
                //        if (object.Equals(r["terminalClientId"], row["HospitalId"]) && object.Equals(r["ProductId"], row["ProductId"]))
                //        {
                //            Dictionary<string, int> dict = new Dictionary<string, int>();
                //            dict.Add("h", Convert.ToInt32(r["terminalClientId"]));
                //            dict.Add("p", Convert.ToInt32(r["ProductId"]));
                //            listHospitalProduct.Add(dict);
                //            flowSales += Convert.ToInt32(r["amountSend"]);
                //        }
                //    }
                //}
                

                //row["TotalCost"] = totalCost.ToString("f2");
                //row["OperatingProfit"] = (ExaminePrice - totalCost).ToString("f2");
                //row["OperatingProfitRatio"] = (ConvertVeriy(row["OperatingProfit"]) / ExaminePrice * 100).ToString("f2");
                //row["OperatingProfitForYear"] = (ConvertVeriy(row["OperatingProfit"]) * ConvertVeriy(row["QuotaForYear"])).ToString("f2");
                //row["NetProfit"] = (ConvertVeriy(row["OperatingProfitForYear"]) * (1 - ConvertVeriy(row["CorporateIncomeTaxRatio"]))).ToString("f2");

                //查询上月库存
                double StockLastMonth = 0;
                //for(int j=0;j<ds.Tables[2].Rows.Count;j++)
                for (int j = ds.Tables[2].Rows.Count-1; j >= 0; j--)
                {
                    DataRow r = ds.Tables[2].Rows[j];
                    if(object.Equals(r["Hospital"], row["Hospital"]) && object.Equals(r["Product"], row["Product"]))
                        //&& object.Equals(r["Sales"], row["Sales"]) && object.Equals(r["Manager"], row["Manager"]))
                    {
                        string val = r["StockThisMonth"].ToString();
                        StockLastMonth += ConvertVeriy(val);
                        ds.Tables[2].Rows.Remove(r);
                        //break;
                    }
                }
                row["StockLastMonth"] = StockLastMonth;
                row["StockThisMonth"] = flowSales+ StockLastMonth;
                dt.Rows[i]["Year"] = d.Year;
                dt.Rows[i]["Month"] = d.Month;
                dt.Rows[i]["CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        dsRes.Tables.Add(dt.Copy());
        dsRes.Tables.Add(GetDatagridFooter(dt));
         
        return dsRes;
    }

    private static DataTable GetDatagridFooter(DataTable dt)
    {
        DataTable footer = new DataTable();
        footer.Columns.Add("Sales");        
        footer.Columns.Add("FlowSalesMoney");
        footer.Columns.Add("FlowSales");
        footer.Columns.Add("NetSalesMoney");
        footer.Columns.Add("NetSales");
        footer.Columns.Add("StockLastMonth");
        footer.Columns.Add("StockThisMonth");
        footer.Columns.Add("IsFooter");
        DataRow row = footer.NewRow();
        row["Sales"] = "合计";
        row["IsFooter"] = true;
        double FlowSalesMoney=0, NetSalesMoney=0;
        int FlowSales=0, NetSales=0, StockLastMonth=0, StockThisMonth=0;
        foreach(DataRow r in dt.Rows)
        {
            FlowSalesMoney += Convert.ToDouble(r["FlowSalesMoney"]);
            NetSalesMoney += Convert.ToDouble(r["NetSalesMoney"]);
            FlowSales += Convert.ToInt32(Convert.ToDouble(r["FlowSales"]));
            NetSales += Convert.ToInt32(Convert.ToDouble(r["NetSales"]));
            StockLastMonth += Convert.ToInt32(Convert.ToDouble(r["StockLastMonth"]));
            StockThisMonth += Convert.ToInt32(Convert.ToDouble(r["StockThisMonth"]));
        }
        row["FlowSalesMoney"] = FlowSalesMoney;
        row["NetSalesMoney"] = NetSalesMoney;
        row["FlowSales"] = FlowSales;
        row["NetSales"] = NetSales;
        row["StockLastMonth"] = StockLastMonth;
        row["StockThisMonth"] = StockThisMonth;
        footer.Rows.Add(row);
        return footer;
    }

    public static string ArchiveData(string date,string jsonData)
    {
        //获取flow_statistics表字段名称
        DataSet ds = SqlHelper.GetFiledNameAndComment("flow_statistics");
        DateTime d = DateTime.Now;
        if (!string.IsNullOrEmpty(date))
            d = Convert.ToDateTime(date);
        DataTable dt = JsonHelper.DeserializeJsonToObject<DataTable>(jsonData);
       
        for (int i = dt.Columns.Count-1;i>=0;i--)
        {
            bool hasFound = false;
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                if (row["field"].ToString()==(dt.Columns[i].ColumnName))
                {
                    hasFound = true;
                    break;                    
                }
            }
            if(!hasFound)
                dt.Columns.RemoveAt(i);
        }
        dt.Columns.Remove("Id");

        string res = FlowInfoSrv.ArchiveData(d, dt);
        return res;
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

    public static Dictionary<string, string> ImportInfos(Dictionary<string, string> dict,string dateString)
    {
        Dictionary<string, string> fieldDict = new Dictionary<string, string>();

        fieldDict.Add("代表", "Sales");
        fieldDict.Add("经理", "Supervisor");
        fieldDict.Add("大区经理", "Manager");
        fieldDict.Add("总监", "Director");
        fieldDict.Add("大区", "Department");
        fieldDict.Add("区域", "Sector");
        fieldDict.Add("医院", "Hospital");
        fieldDict.Add("产品", "Product");
        fieldDict.Add("规格型号", "Specification");

        //fieldDict.Add("网点代码", "HospitalCode");
        //fieldDict.Add("产品代码", "ProductCode");
        //fieldDict.Add("科室", "HospitalDepartment");
        //fieldDict.Add("当月任务", "MonthTask");
        //AddKeyValueOfDictionary(ref dict, "代表", "SalesId");
        //AddKeyValueOfDictionary(ref dict, "主管", "SupervisorId");
        //AddKeyValueOfDictionary(ref dict, "盈利中心经理", "ManagerId");
        //AddKeyValueOfDictionary(ref dict, "总监", "DirectorId");
        //AddKeyValueOfDictionary(ref dict, "部门", "Department");
        //AddKeyValueOfDictionary(ref dict, "区域", "Sector");
        //AddKeyValueOfDictionary(ref dict, "医院", "HospitalId");
        //AddKeyValueOfDictionary(ref dict, "产品", "ProductId");
        //AddKeyValueOfDictionary(ref dict, "规格", "Specification");

        fieldDict.Add("税点", "TaxRatio");
        fieldDict.Add("财务费用占比", "FinancialRatio");
        fieldDict.Add("研发费用占比", "RdRatio");
        fieldDict.Add("开发费用占比", "DevelopmentRatio");
        fieldDict.Add("产品发展基金占比", "ProductDevelopmentFundRatio");
        fieldDict.Add("市场学术费占比", "MarketRatio");
        fieldDict.Add("市场调节基金占比", "MarketReadjustmentRatio");
        fieldDict.Add("商务费用占比", "BusinessRatio");
        //fieldDict.Add("实验人员奖金占比", "ExperimenterBonusRatio");
        fieldDict.Add("工资社保占比", "WageSocialSecurityRatio");
        fieldDict.Add("实验费(TF)占比", "TfRatio");
        fieldDict.Add("区域中心费用比例", "RegionalCenterRatio");
        //AddKeyValueOfDictionary(ref dict, "税点", "TaxRatio");        
        //AddKeyValueOfDictionary(ref dict, "财务费用占比", "FinancialRatio");
        //AddKeyValueOfDictionary(ref dict, "研发费用占比（3%）", "RdRatio");
        //AddKeyValueOfDictionary(ref dict, "开发费用占比（2%）", "DevelopmentRatio");
        //AddKeyValueOfDictionary(ref dict, "产品发展基金占比", "ProductDevelopmentFundRatio");
        //AddKeyValueOfDictionary(ref dict, "市场学术费占比", "MarketRatio");
        //AddKeyValueOfDictionary(ref dict, "市场调节基金占比", "MarketReadjustmentRatio");
        //AddKeyValueOfDictionary(ref dict, "商务费用占比（1%）", "BusinessRatio");
        //AddKeyValueOfDictionary(ref dict, "实验人员奖金占比", "ExperimenterBonusRatio");
        //AddKeyValueOfDictionary(ref dict, "工资社保占比（10%）", "WageSocialSecurityRatio");
        //AddKeyValueOfDictionary(ref dict, "实验费(TF)占比（0.5%）", "TfRatio");
        //AddKeyValueOfDictionary(ref dict, "区域中心费用比例", "RegionalCenterRatio");


        fieldDict.Add("税金", "TaxCost");
        fieldDict.Add("财务费用金额", "FinancialCost");
        fieldDict.Add("研发费用金额", "RdCost");
        fieldDict.Add("销售总监费用", "SalesDirectorCost");
        fieldDict.Add("开发费用金额", "DevelopmentCost");
        fieldDict.Add("总部管理费用", "HeadOfficeManageCost");
        fieldDict.Add("产品发展基金", "ProductDevelopmentFundCost");
        fieldDict.Add("市场学术费", "MarketCost");
        fieldDict.Add("市场调节基金", "MarketReadjustmentCost");
        fieldDict.Add("PMS/支", "PmsCost");
        fieldDict.Add("包干费", "GuestMaintenanceCost");
        fieldDict.Add("商务费用金额", "BusinessCost");
        //fieldDict.Add("实验人员奖金金额", "ExperimenterBonusCost");
        fieldDict.Add("工资社保金额", "WageSocialSecurityCost");
        fieldDict.Add("实验费(TF)金额", "TfCost");
        fieldDict.Add("区域中心费用", "RegionalCenterCost");
        fieldDict.Add("区域中心费用VIP维护", "RegionalCenterVipCost");
        //AddKeyValueOfDictionary(ref dict, "税金", "TaxCost");
        //AddKeyValueOfDictionary(ref dict, "财务费用金额", "FinancialCost");
        //AddKeyValueOfDictionary(ref dict, "研发费用金额", "RdCost");
        //AddKeyValueOfDictionary(ref dict, "销售总监费用（0.5%）", "TaxCost");
        //AddKeyValueOfDictionary(ref dict, "开发费用金额", "DevelopmentCost");
        //AddKeyValueOfDictionary(ref dict, "总部管理费用（3.5%）", "HeadOfficeManageCost");
        //AddKeyValueOfDictionary(ref dict, "产品发展基金", "ProductDevelopmentFundCost");
        //AddKeyValueOfDictionary(ref dict, "市场学术费", "MarketCost");
        //AddKeyValueOfDictionary(ref dict, "市场调节基金", "MarketReadjustmentCost");
        //AddKeyValueOfDictionary(ref dict, "PMS/支", "PmsCost");
        //AddKeyValueOfDictionary(ref dict, "包干费6%", "GuestMaintenance");
        //AddKeyValueOfDictionary(ref dict, "商务费用金额", "BusinessCost");
        //AddKeyValueOfDictionary(ref dict, "实验人员奖金金额", "ExperimenterBonusCost");
        //AddKeyValueOfDictionary(ref dict, "工资社保金额", "WageSocialSecurityCost");
        //AddKeyValueOfDictionary(ref dict, "实验费(TF)金额", "TfCost");
        //AddKeyValueOfDictionary(ref dict, "区域中心费用", "RegionalCenterCost");
        //AddKeyValueOfDictionary(ref dict, "区域中心费用VIP维护", "RegionalCenterVipCost");


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
        fieldDict.Add("上月库存", "StockLastMonth");
        fieldDict.Add("当月流向数", "FlowSales");
        fieldDict.Add("当月纯销", "NetSales");
        fieldDict.Add("当月库存", "StockThisMonth");
        fieldDict.Add("当月流向金额", "FlowSalesMoney");
        fieldDict.Add("当月纯销金额", "NetSalesMoney");
        //fieldDict.Add("2017年营业利润", "OperatingProfitForYear");
        //fieldDict.Add("2017年指标（数量）", "QuotaForYear");

        //AddKeyValueOfDictionary(ref dict, "医院供货价", "HospitalSupplyPrice");
        //AddKeyValueOfDictionary(ref dict, "公司开票价", "InvoicePrice");
        //AddKeyValueOfDictionary(ref dict, "销售折让", "SalesAllowances");
        //AddKeyValueOfDictionary(ref dict, "考核价", "ExaminePrice");
        //AddKeyValueOfDictionary(ref dict, "采购成本", "PurchasingCost");
        //AddKeyValueOfDictionary(ref dict, "毛利", "GrossProfit");
        //AddKeyValueOfDictionary(ref dict, "费用合计", "TotalCost");
        //AddKeyValueOfDictionary(ref dict, "营业利润", "OperatingProfit");
        //AddKeyValueOfDictionary(ref dict, "营业利润率", "OperatingProfitRatio");
        //AddKeyValueOfDictionary(ref dict, "企业所得税", "ExperimenterBonusRatio");
        //AddKeyValueOfDictionary(ref dict, "净利润", "WageSocialSecurityRatio");
        //AddKeyValueOfDictionary(ref dict, "上月库存", "StockLastMonth");
        //AddKeyValueOfDictionary(ref dict, "当月流向数", "FlowSales");
        //AddKeyValueOfDictionary(ref dict, "当月纯销", "NetSales");
        //AddKeyValueOfDictionary(ref dict, "当月库存", "StockThisMonth");
        //AddKeyValueOfDictionary(ref dict, "当月流向金额", "FlowSalesMoney");
        //AddKeyValueOfDictionary(ref dict, "当月纯销金额", "NetSalesMoney");
        //AddKeyValueOfDictionary(ref dict, "2017年营业利润", "OperatingProfitForYear");
        //AddKeyValueOfDictionary(ref dict, "2017年指标（数量）", "QuotaForYear");
        foreach(string key in fieldDict.Keys)
        {
            if(!dict.Keys.Contains(key))
            {
                dict["状态"] = string.Format("文件中未找到“{0}”列",key);
                return dict;
            }
            string val = dict[key];
            if (!string.IsNullOrEmpty(val) &&StringTools.IsNumeric(val) && (!fieldDict[key].Contains("Ratio")))
                val = (Convert.ToDouble(val)).ToString("F2");
            dict.Add(fieldDict[key], val);
        }

        dict.Add("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        DateTime date = Convert.ToDateTime(dateString);
        dict.Add("Year", date.Year.ToString());
        dict.Add("Month", date.Month.ToString());

        string res = FlowInfoSrv.ImportInfos(dict);
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