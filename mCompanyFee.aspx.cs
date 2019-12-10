using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mCompanyFee : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mDailyProfit",
           "vbmKxak1-a5Ty1cEBJzUFa1OR9f0V4Yh5j0sJq2-e9o",
           "1000023",
        "http://yelioa.top/mCompanyFee.aspx");

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
            Response.Clear();
            if (action == "uploadWageExcel")
            {
                Response.Write(uploadWageExcel());
            }
            else if (action == "uploadOuterWageExcel")
            {
                Response.Write(uploadOuterWageExcel());
            }
            else if (action == "uploadTaxExcel")
            {
                Response.Write(uploadTaxExcel());
            }
            else if (action == "uploadInterestExcel")
            {
                Response.Write(uploadInterestExcel());
            }
            else if (action == "uploadDepreciationExcel")
            {
                Response.Write(uploadDepreciationExcel());
            }
            else if (action == "uploadAmortizeExcel")
            {
                Response.Write(uploadAmortizeExcel());
            }
            else if (action == "getData")
            {
                Response.Write(getData());
            }
            else if (action == "getApprover")
            {
                Response.Write(getApprover());
            }
            else if (action == "submit")
            {
                Response.Write(submit());
            }

            Response.End();
        }
    }

    private string uploadWageExcel()
    {
        DataTable dtExl = ExcelHelperV2_0.Import(Request, 3);

        DataTable dt = new DataTable();

        if (dtExl.Rows.Count == 0)
        {
            return JsonHelper.DataTable2Json(dt);
        }

        // 再新增
        int startId = 0;
        string sql = "select max(Id) from wages;";
        object obj = SqlHelper.Scalar(sql);
        if (obj != null)
            startId = Convert.ToInt32(obj) + 1;

        dt.Columns.Add("Id");
        dt.Columns.Add("DocCode");
        dt.Columns.Add("Level");
        dt.Columns.Add("Status");
        dt.Columns.Add("Name");
        dt.Columns.Add("Company");
        dt.Columns.Add("Department");
        dt.Columns.Add("Position");
        dt.Columns.Add("AttendanceDay");
        dt.Columns.Add("AbsenceDay");
        dt.Columns.Add("PositionSalary");
        dt.Columns.Add("SecretSalary");
        dt.Columns.Add("LevelSalary");
        dt.Columns.Add("TechnicalSalary");
        dt.Columns.Add("StableSalary"); 
        dt.Columns.Add("OtherSalary");
        dt.Columns.Add("EducationAllowance");
        dt.Columns.Add("OtherAllowance");
        dt.Columns.Add("TotalStableSalary");
        dt.Columns.Add("MonthlyPerformance");
        dt.Columns.Add("QuarterlyPerformance");
        dt.Columns.Add("YearlyPerformance");
        dt.Columns.Add("ActualPerformance");
        dt.Columns.Add("CSalesBonus");
        dt.Columns.Add("QSalesBonus");
        dt.Columns.Add("HeadBonus");
        dt.Columns.Add("ManageBonus");
        dt.Columns.Add("UnusualBonus");
        dt.Columns.Add("TesterBonus");
        dt.Columns.Add("OvertimeBonus");
        dt.Columns.Add("ProductBonus");
        dt.Columns.Add("GMBonus");
        dt.Columns.Add("RDBonus");
        dt.Columns.Add("OtherBonus1");
        dt.Columns.Add("OtherBonus2");
        dt.Columns.Add("TotalBonus");
        dt.Columns.Add("TotalPerformanceBonus");
        dt.Columns.Add("AttendaceFine");
        dt.Columns.Add("SanitaryFine");
        dt.Columns.Add("AbsenceFine");
        dt.Columns.Add("ExpireFine");
        dt.Columns.Add("ViolationFine");
        dt.Columns.Add("OtherFine1");
        dt.Columns.Add("OtherFine2");
        dt.Columns.Add("TotalFine");
        dt.Columns.Add("MutualFund");
        dt.Columns.Add("OtherFund");
        dt.Columns.Add("TotalFineAndFund");
        dt.Columns.Add("TotalPay");
        dt.Columns.Add("TotalPay1");
        dt.Columns.Add("TotalPay2");
        dt.Columns.Add("SocialInsurance");
        dt.Columns.Add("SocialInsurance1");
        dt.Columns.Add("PublicFund");
        dt.Columns.Add("PublicFund1");
        dt.Columns.Add("PersonalTax");
        dt.Columns.Add("PersonalTax1");
        dt.Columns.Add("Rent");
        dt.Columns.Add("PersonalTaxAdjust");
        dt.Columns.Add("PersonalTaxAdjust1");
        dt.Columns.Add("ActualPay");
        dt.Columns.Add("ActualPay1");
        dt.Columns.Add("ActualPay2");
        dt.Columns.Add("ActualPay2Decimal");
        dt.Columns.Add("Remark");
        dt.Columns.Add("SubmitterId");
        dt.Columns.Add("Year");
        dt.Columns.Add("Month");
        dt.Columns.Add("CreateTime");

        UserInfo user = (UserInfo)Session["user"];
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string company = Request.Form["company"];

        string docCode = GenerateDocCode.generateRandomDocCode();

        foreach (DataRow dr in dtExl.Rows)
        {
            if (string.IsNullOrEmpty(dr["序号"].ToString()) || dr["序号"].ToString().Contains("合计"))
                continue;

            DataRow r = dt.NewRow();

            r["SubmitterId"] = user.userId.ToString();
            r["Year"] = year;
            r["Month"] = month;
            r["CreateTime"] = DateTime.Now;
            r["Company"] = company;

            r["Id"] = startId++;
            r["DocCode"] = docCode;
            r["Level"] = 1;
            r["Status"] = "审批中";
            r["Name"] = dr["姓名"];
            r["Department"] = dr["中心"].ToString() + dr["部门"] + dr["战区"] + dr["大区"] + dr["地区"];
            r["Position"] = dr["职务"];
            r["AttendanceDay"] = dr["考勤"];
            r["AbsenceDay"] = dr["缺勤天数"];
            r["PositionSalary"] = dr["职位工资"];
            r["SecretSalary"] = dr["保密工资"];
            r["LevelSalary"] = dr["档位工资"];
            r["TechnicalSalary"] = dr["技衔工资"];
            r["OtherSalary"] = dr["其他工资"];
            r["StableSalary"] = dr["固定工资小计"];
            r["EducationAllowance"] = dr["学历津贴"];
            r["OtherAllowance"] = dr["其它津贴"];
            r["TotalStableSalary"] = dr["固定工资合计"];
            r["MonthlyPerformance"] = dr["月度绩效"];
            r["QuarterlyPerformance"] = dr["季度绩效"];
            r["YearlyPerformance"] = dr["年度绩效"];
            r["ActualPerformance"] = dr["实发绩效"];
            r["CSalesBonus"] = dr["纯销代表奖金"];
            r["QSalesBonus"] = dr["渠道代表奖金"];
            r["HeadBonus"] = dr["主管奖金"];
            r["ManageBonus"] = dr["管理层奖金"];
            r["UnusualBonus"] = dr["异样考核"];
            r["TesterBonus"] = dr["实验奖金"];
            r["OvertimeBonus"] = dr["加班"];
            r["ProductBonus"] = dr["生产产能"];
            r["GMBonus"] = dr["总经理特别贡献"];
            r["RDBonus"] = dr["研发项目奖金"];
            r["OtherBonus1"] = dr["其他奖励1"];
            r["OtherBonus2"] = dr["其他奖励2"];
            r["TotalBonus"] = dr["奖励合计"];
            r["TotalPerformanceBonus"] = dr["绩效及奖励小计"];
            r["AttendaceFine"] = dr["考勤扣款"];
            r["SanitaryFine"] = dr["卫生检查罚款"];
            r["AbsenceFine"] = dr["出勤扣款"];
            r["ExpireFine"] = dr["产品过期罚款"];
            r["ViolationFine"] = dr["违反制度罚款"];
            r["OtherFine1"] = dr["其他罚款1"];
            r["OtherFine2"] = dr["其他罚款2"];
            r["TotalFine"] = dr["罚款合计"];
            r["MutualFund"] = dr["代扣基金"];
            r["OtherFund"] = dr["其它减项"];
            r["TotalFineAndFund"] = dr["工资减项小计"];
            r["TotalPay"] = dr["应发合计"];
            r["TotalPay1"] = dr["应发1"];
            r["TotalPay2"] = dr["应发2"];
            r["SocialInsurance"] = dr["社保"];
            r["SocialInsurance1"] = dr["社保1"];
            r["PublicFund"] = dr["公积金"];
            r["PublicFund1"] = dr["公积金1"];
            r["PersonalTax"] = dr["个人所得税"];
            r["PersonalTax1"] = dr["个人所得税1"];
            r["Rent"] = dr["代扣房租"];
            r["PersonalTaxAdjust"] = dr["个税调整项"];
            r["PersonalTaxAdjust1"] = dr["个税调整项1"];
            r["ActualPay"] = dr["实发合计工资"];
            r["ActualPay1"] = dr["实发1"];
            r["ActualPay2"] = dr["实发2"];
            r["ActualPay2Decimal"] = dr["实发合计保留两位"];
            r["Remark"] = dr["备注"];

            dt.Rows.Add(r);
        }

        return JsonHelper.DataTable2Json(dt);
    }

    private string uploadOuterWageExcel()
    {
        DataTable dtExl = ExcelHelperV2_0.Import(Request, 2);

        DataTable dt = new DataTable();

        if (dtExl.Rows.Count == 0)
        {
            return JsonHelper.DataTable2Json(dt);
        }

        // 再新增
        int startId = 0;
        string sql = "select max(Id) from tax;";
        object obj = SqlHelper.Scalar(sql);
        if (obj != null)
            startId = Convert.ToInt32(obj) + 1;

        dt.Columns.Add("Id");
        dt.Columns.Add("DocCode");
        dt.Columns.Add("Level");
        dt.Columns.Add("Status");
        dt.Columns.Add("Name");
        dt.Columns.Add("Company");
        dt.Columns.Add("Department");
        dt.Columns.Add("Position");
        dt.Columns.Add("WorkingHour");
        dt.Columns.Add("WorkingPrice");
        dt.Columns.Add("WorkingAmount");
        dt.Columns.Add("TotalSalary");
        dt.Columns.Add("ActualPerformance");
        dt.Columns.Add("Bonus");
        dt.Columns.Add("TotalPerformanceBonus");
        dt.Columns.Add("RubbishFine");
        dt.Columns.Add("ProductFine");
        dt.Columns.Add("Fine");
        dt.Columns.Add("MutualFund");
        dt.Columns.Add("OtherFund");
        dt.Columns.Add("TotalFineAndFund");
        dt.Columns.Add("TotalPay");
        dt.Columns.Add("SocialInsurance");
        dt.Columns.Add("PublicFund");
        dt.Columns.Add("PersonalTax");
        dt.Columns.Add("Rent");
        dt.Columns.Add("ActualPay");
        dt.Columns.Add("ActualPay2Decimal");
        dt.Columns.Add("Remark");
        dt.Columns.Add("SubmitterId");
        dt.Columns.Add("Year");
        dt.Columns.Add("Month");
        dt.Columns.Add("CreateTime");

        UserInfo user = (UserInfo)Session["user"];
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string company = Request.Form["company"];

        string docCode = GenerateDocCode.generateRandomDocCode();

        foreach (DataRow dr in dtExl.Rows)
        {
            if (string.IsNullOrEmpty(dr["序号"].ToString()) || dr["序号"].ToString().Contains("合计"))
                continue;

            DataRow r = dt.NewRow();

            r["SubmitterId"] = user.userId.ToString();
            r["Year"] = year;
            r["Month"] = month;
            r["CreateTime"] = DateTime.Now;
            r["Company"] = company;

            r["Id"] = startId++;
            r["DocCode"] = docCode;
            r["Level"] = 1;
            r["Status"] = "审批中";
            r["Name"] = dr["姓名"];
            r["Department"] = dr["部门"];
            r["Position"] = dr["职位"];
            r["WorkingHour"] = dr["出勤小时"];
            r["WorkingPrice"] = dr["计件标准（元/件）"];
            r["WorkingAmount"] = dr["计件数"];
            r["TotalSalary"] = dr["计件工资合计"];
            r["ActualPerformance"] = dr["实发绩效"];
            r["Bonus"] = dr["奖励金额"];
            r["TotalPerformanceBonus"] = dr["绩效及奖励小计"];
            r["RubbishFine"] = dr["残次品扣款"];
            r["ProductFine"] = dr["产品扣罚"];
            r["Fine"] = dr["罚款"];
            r["MutualFund"] = dr["代扣基金"];
            r["OtherFund"] = dr["其它"];
            r["TotalFineAndFund"] = dr["工资减项小计"];
            r["TotalPay"] = dr["应发合计"];
            r["SocialInsurance"] = dr["社保"];
            r["PublicFund"] = dr["公积金"];
            r["PersonalTax"] = dr["个人所得税"];
            r["Rent"] = dr["代扣房租"];
            r["ActualPay"] = dr["实发工资"];
            r["ActualPay2Decimal"] = dr["保留两位"];
            r["Remark"] = dr["备注"];

            dt.Rows.Add(r);
        }

        return JsonHelper.DataTable2Json(dt);
    }

    private string uploadTaxExcel()
    {
        DataTable dtExl = ExcelHelperV2_0.Import(Request, 2);

        DataTable dt = new DataTable();

        if (dtExl.Rows.Count == 0)
        {
            return JsonHelper.DataTable2Json(dt);
        }

        // 再新增
        int startId = 0;
        string sql = "select max(Id) from tax;";
        object obj = SqlHelper.Scalar(sql);
        if (obj != null)
            startId = Convert.ToInt32(obj) + 1;

        dt.Columns.Add("Id");
        dt.Columns.Add("DocCode");
        dt.Columns.Add("Level");
        dt.Columns.Add("Status");
        dt.Columns.Add("Company");
        dt.Columns.Add("TaxType");
        dt.Columns.Add("TaxAmount");
        dt.Columns.Add("TaxBasis");
        dt.Columns.Add("TaxRate");
        dt.Columns.Add("Remark");
        dt.Columns.Add("SubmitterId");
        dt.Columns.Add("Year");
        dt.Columns.Add("Month");
        dt.Columns.Add("CreateTime");

        UserInfo user = (UserInfo)Session["user"];
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string company = Request.Form["company"];
        string docCode = GenerateDocCode.generateRandomDocCode();

        foreach (DataRow dr in dtExl.Rows)
        {
            if (string.IsNullOrEmpty(dr["税种"].ToString()) || dr["税种"].ToString().Trim().Contains("合   计") || dr["税种"].ToString().Contains("总经理"))
                continue;

            DataRow r = dt.NewRow();

            r["SubmitterId"] = user.userId.ToString();
            r["Year"] = year;
            r["Month"] = month;
            r["CreateTime"] = DateTime.Now;
            r["Company"] = company;
            r["Id"] = startId++;
            r["DocCode"] = docCode;
            r["Level"] = 1;
            r["Status"] = "审批中";
            r["TaxType"] = dr["税种"];
            r["TaxAmount"] = dr["金额"];
            r["TaxBasis"] = dr["计税依据"];
            r["TaxRate"] = dr["税率"];
            r["Remark"] = dr["备注"];

            dt.Rows.Add(r);
        }

        return JsonHelper.DataTable2Json(dt);
    }

    private string uploadInterestExcel()
    {
        DataTable dtExl = ExcelHelperV2_0.Import(Request, 2);

        DataTable dt = new DataTable();

        if (dtExl.Rows.Count == 0)
        {
            return JsonHelper.DataTable2Json(dt);
        }

        // 再新增
        int startId = 0;
        string sql = "select max(Id) from tax;";
        object obj = SqlHelper.Scalar(sql);
        if (obj != null)
            startId = Convert.ToInt32(obj) + 1;

        dt.Columns.Add("Id");
        dt.Columns.Add("DocCode");
        dt.Columns.Add("Level");
        dt.Columns.Add("Status");
        dt.Columns.Add("Company");

        dt.Columns.Add("Name");
        dt.Columns.Add("LoanDate");
        dt.Columns.Add("DueDate");
        dt.Columns.Add("Frequency");
        dt.Columns.Add("Amount");
        dt.Columns.Add("YearRate");
        dt.Columns.Add("MonthRate");
        dt.Columns.Add("PaidInterest");
        dt.Columns.Add("Proviston");
        dt.Columns.Add("NotProviston");
        dt.Columns.Add("PaidPrincipal");
        dt.Columns.Add("ProvistonDate");
        dt.Columns.Add("Remark");
        dt.Columns.Add("LastPrincipal");

        UserInfo user = (UserInfo)Session["user"];
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string company = Request.Form["company"];
        string docCode = GenerateDocCode.generateRandomDocCode();

        foreach (DataRow dr in dtExl.Rows)
        {
            //if (string.IsNullOrEmpty(dr["税种"].ToString()) || dr["税种"].ToString().Trim().Contains("合   计") || dr["税种"].ToString().Contains("总经理"))
            //    continue;

            DataRow r = dt.NewRow();

            r["SubmitterId"] = user.userId.ToString();
            r["Year"] = year;
            r["Month"] = month;
            r["CreateTime"] = DateTime.Now;
            r["Company"] = company;
            r["Id"] = startId++;
            r["DocCode"] = docCode;
            r["Level"] = 1;
            r["Status"] = "审批中";

            r["Name"] = dr["名称"];
            r["LoanDate"] = dr["借款时间"];
            r["DueDate"] = dr["到期时间"];
            r["Frequency"] = dr["利息支付频率"];
            r["Amount"] = dr["借款金额"];
            r["YearRate"] = dr["年利率"];
            r["MonthRate"] = dr["月利息"];
            r["PaidInterest"] = dr["已付银行利息"];
            r["Proviston"] = dr["已计提"];
            r["NotProviston"] = dr["还应计提"];
            r["PaidPrincipal"] = dr["已还本金"];
            r["ProvistonDate"] = dr["利息计提期间"];
            r["Remark"] = dr["摘要"];
            r["LastPrincipal"] = dr["期末结余本金"];

            dt.Rows.Add(r);
        }

        return JsonHelper.DataTable2Json(dt);
    }


    private string uploadDepreciationExcel()
    {
        DataTable dtExl = ExcelHelperV2_0.Import(Request, 2);

        DataTable dt = new DataTable();

        if (dtExl.Rows.Count == 0)
        {
            return JsonHelper.DataTable2Json(dt);
        }

        // 再新增
        int startId = 0;
        string sql = "select max(Id) from tax;";
        object obj = SqlHelper.Scalar(sql);
        if (obj != null)
            startId = Convert.ToInt32(obj) + 1;

        dt.Columns.Add("Id");
        dt.Columns.Add("DocCode");
        dt.Columns.Add("Level");
        dt.Columns.Add("Status");
        dt.Columns.Add("Company");
        dt.Columns.Add("Type");
        dt.Columns.Add("Name");
        dt.Columns.Add("OriginValue");
        dt.Columns.Add("CurrentD");
        dt.Columns.Add("AccumulatedD");
        dt.Columns.Add("NetValue");
        dt.Columns.Add("SubmitterId");
        dt.Columns.Add("Year");
        dt.Columns.Add("Month");
        dt.Columns.Add("CreateTime");

        UserInfo user = (UserInfo)Session["user"];
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string company = Request.Form["company"];
        string docCode = GenerateDocCode.generateRandomDocCode();

        foreach (DataRow dr in dtExl.Rows)
        {
            if (string.IsNullOrEmpty(dr["类  型"].ToString()) || dr["类  型"].ToString().Trim().Contains("合计") || dr["类  型"].ToString().Contains("总经理"))
                continue;

            DataRow r = dt.NewRow();

            r["SubmitterId"] = user.userId.ToString();
            r["Year"] = year;
            r["Month"] = month;
            r["CreateTime"] = DateTime.Now;
            r["Company"] = company;
            r["Id"] = startId++;
            r["DocCode"] = docCode;
            r["Level"] = 1;
            r["Status"] = "审批中";
            r["Type"] = dr["类  型"];
            r["Name"] = dr["名    称"];
            r["OriginValue"] = dr["原值"];
            r["CurrentD"] = dr["本期折旧"];
            r["AccumulatedD"] = dr["累计折旧"];
            r["NetValue"] = dr["净值"];

            dt.Rows.Add(r);
        }

        return JsonHelper.DataTable2Json(dt);
    }

    private string uploadAmortizeExcel()
    {
        DataTable dtExl = ExcelHelperV2_0.Import(Request, 2);

        DataTable dt = new DataTable();

        if (dtExl.Rows.Count == 0)
        {
            return JsonHelper.DataTable2Json(dt);
        }

        // 再新增
        int startId = 0;
        string sql = "select max(Id) from tax;";
        object obj = SqlHelper.Scalar(sql);
        if (obj != null)
            startId = Convert.ToInt32(obj) + 1;

        dt.Columns.Add("Id");
        dt.Columns.Add("DocCode");
        dt.Columns.Add("Level");
        dt.Columns.Add("Status");
        dt.Columns.Add("Company");
        dt.Columns.Add("Type");
        dt.Columns.Add("Project");
        dt.Columns.Add("OriginValue");
        dt.Columns.Add("CurrentA");
        dt.Columns.Add("AccumulatedA");
        dt.Columns.Add("FinalValue");
        dt.Columns.Add("Remark");
        dt.Columns.Add("SubmitterId");
        dt.Columns.Add("Year");
        dt.Columns.Add("Month");
        dt.Columns.Add("CreateTime");

        UserInfo user = (UserInfo)Session["user"];
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string company = Request.Form["company"];
        string docCode = GenerateDocCode.generateRandomDocCode();

        foreach (DataRow dr in dtExl.Rows)
        {
            if (string.IsNullOrEmpty(dr["类型"].ToString()) || dr["类型"].ToString().Trim().Contains("合计") || dr["类型"].ToString().Contains("总经理"))
                continue;

            DataRow r = dt.NewRow();

            r["SubmitterId"] = user.userId.ToString();
            r["Year"] = year;
            r["Month"] = month;
            r["CreateTime"] = DateTime.Now;
            r["Company"] = company;
            r["Id"] = startId++;
            r["DocCode"] = docCode;
            r["Level"] = 1;
            r["Status"] = "审批中";
            r["Type"] = dr["类型"];
            r["Project"] = dr["项目"];
            r["OriginValue"] = dr["原值"];
            r["CurrentA"] = dr["本月摊销"];
            r["AccumulatedA"] = dr["累计摊销"];
            r["FinalValue"] = dr["期末余额"];
            r["Remark"] = dr["备注"];

            dt.Rows.Add(r);
        }

        return JsonHelper.DataTable2Json(dt);
    }

    private string getData()
    {
        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string company = Request.Form["company"];
        string type = Request.Form["type"];
        UserInfo userInfo = (UserInfo)Session["user"];

        string sql = "";

        if (type == "1")
            sql = string.Format("select * from wages where year = '{0}' and month = '{1}' and company = '{2}'", year, month, company);
        else if (type == "2")
            sql = string.Format("select * from outer_wages where year = '{0}' and month = '{1}' and company = '{2}'", year, month, company);
        else if (type == "3")
            sql = string.Format("select * from tax where year = '{0}' and month = '{1}' and company = '{2}'", year, month, company);
        else if (type == "4")
            sql = string.Format("select * from interest where year = '{0}' and month = '{1}' and company = '{2}'", year, month, company);
        else if (type == "5")
            sql = string.Format("select * from depreciation where year = '{0}' and month = '{1}' and company = '{2}'", year, month, company);
        else if (type == "6")
            sql = string.Format("select * from amortize where year = '{0}' and month = '{1}' and company = '{2}'", year, month, company);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string getApprover()
    {
        string type = Request.Form["type"];
        string tableName = "";

        if (type == "1")
            tableName = "wages";
        else if (type == "2")
            tableName = "outer_wages";
        else if (type == "3")
            tableName = "tax";
        else if (type == "4")
            tableName = "interest";
        else if (type == "5")
            tableName = "depreciation";
        else if (type == "6")
            tableName = "amortize";

        string sql = string.Format("select t2.userId,t2.userName,t2.wechatUserId from approval_process t1 left join users t2 on t1.approverId = t2.userId where t1.documentTableName = '{0}'", tableName);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        UserInfo userInfo = (UserInfo)Session["user"];

        DataRow dr = dt.NewRow();

        dr["userId"] = userInfo.userId;
        dr["userName"] = userInfo.userName;

        dt.Rows.InsertAt(dr, 0);

        return JsonHelper.DataTable2Json(dt);
    }

    private string submit()
    {
        var result = new JObject
        {
            { "code", 200 },
            { "msg", "ok" }
        };

        DataTable dt = JsonHelper.Json2Dtb(Request.Form["tableData"]);
        List<JObject> approver = JsonHelper.DeserializeJsonToList<JObject>(Request.Form["approver"]);

        string year = Request.Form["year"];
        string month = Request.Form["month"];
        string company = Request.Form["company"];
        string type = Request.Form["type"];

        string tableName = "";

        if (type == "1")
            tableName = "wages";
        else if (type == "2")
            tableName = "outer_wages";
        else if (type == "3")
            tableName = "tax";
        else if (type == "4")
            tableName = "interest";
        else if (type == "5")
            tableName = "depreciation";
        else if (type == "6")
            tableName = "amortize";

        // 先删除 后新增
        DataTable tempDt = SqlHelper.Find(string.Format("select docCode from {0} where year = '{1}' and month = '{2}' and company = '{3}' limit 1", tableName, year, month, company)).Tables[0];

        string originDocCode = "";

        if (tempDt.Rows.Count > 0)
            originDocCode = tempDt.Rows[0][0].ToString();

        string sql = string.Format("delete from {3} where year = '{0}' and month = '{1}' and company = '{2}';", year, month, company, tableName);
        sql += string.Format("delete from approval_approver where documentTableName = '{0}' and docCode = '{1}';", tableName, originDocCode);
        sql += string.Format("delete from approval_record where documentTableName = '{0}' and docCode = '{1}';", tableName, originDocCode);
        sql += SqlHelper.GetInsertString(dt, tableName);

        string msg = SqlHelper.Exce(sql);

        if (!msg.Contains("操作成功"))
        {
            result["code"] = 500;
            result["msg"] = "导入数据失败：数据库操作错误！";

            return result.ToString();
        }

        // 新增审批人表
        List<JObject> list = new List<JObject>();
        string docCode = dt.Rows[0]["DocCode"].ToString();

        int index = 0;
        foreach (JObject tempJ in approver)
        {
            JObject approverJ = new JObject
            {
                { "DocumentTableName", tableName },
                { "DocCode", docCode },
                { "ApproverId", tempJ["userId"] },
                { "Level", index++ }
            };
            list.Add(approverJ);
        }

        sql = SqlHelper.GetInsertString(list, "approval_approver");
        msg = SqlHelper.Exce(sql);

        if (!msg.Contains("操作成功"))
        {
            result["code"] = 500;
            result["msg"] = "导入数据失败：数据库操作错误！";

            return result.ToString();
        }

        // 新增审批记录表
        UserInfo userInfo = (UserInfo)Session["user"];

        JObject recordJ = new JObject
        {
            { "DocumentTableName", tableName },
            { "DocCode", docCode },
            { "Time", DateTime.Now },
            { "Level", 0 },
            { "ApproverId", userInfo.userId.ToString()},
            { "SubmitterId", userInfo.userId.ToString()},
            { "ApprovalResult", "单据提交"}
        };

        sql = SqlHelper.GetInsertString(recordJ, "approval_record");
        msg = SqlHelper.Exce(sql);

        if (!msg.Contains("操作成功"))
        {
            result["code"] = 500;
            result["msg"] = "导入数据失败：数据库操作错误！";

            return result.ToString();
        }

        WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper("vbmKxak1-a5Ty1cEBJzUFa1OR9f0V4Yh5j0sJq2-e9o", "发票上报", "1000023");

        string chineseName = "";

        if (type == "1")
            chineseName = "人员工资";
        if (type == "2")
            chineseName = "非全日制人员工资";
        else if (type == "3")
            chineseName = "税金";
        else if (type == "4")
            chineseName = "利息";
        else if (type == "5")
            chineseName = "折旧";
        else if (type == "6")
            chineseName = "摊销";

        // 给提交人发消息通知
        wxNetSalesHelper.GetJsonAndSendWxMsg(userInfo.wechatUserId, string.Format("您的{0}年{1}月{2}{3}已提交，请耐心等待审批!", year, month, company, chineseName), "http://yelioa.top//mCompanyFee.aspx", "1000023");

        // 给审批人发消息通知
        wxNetSalesHelper.GetJsonAndSendWxMsg(approver[1]["wechatUserId"].ToString(), "请及时审批 提交人为:" + userInfo.userName
                + string.Format("提交的{0}年{1}月{2}{3},谢谢!", year, month, company, chineseName), "http://yelioa.top//mCompanyFee.aspx", "1000023");

        return result.ToString();
    }
}