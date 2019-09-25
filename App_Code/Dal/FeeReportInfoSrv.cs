using System.Data;
using System.Web;

/// <summary>
/// FeeReportInfoSrv 的摘要说明
/// </summary>
public class FeeReportInfoSrv
{
    public FeeReportInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet getMonthlyData(int month, int year)
    {
        UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
        if (user == null)
            return null;
        PostHelper postHelper = new PostHelper(user);
        if (postHelper.dpList == null)
            return null;
        string sql = "";
        //if (postHelper.ContainPost(99, 74))
        //{
            sql = string.Format("SELECT sector," +
                "sum(FlowSalesMoney) AS '销售额',sum(SalesAllowances) AS '销售折让',sum(PurchasingCost) AS '采购成本'," +
                "sum(GrossProfit) AS '毛利',sum(TaxCost) AS '税金',sum(FinancialCost) AS '财务费用金额'," +
                "sum(RdCost) AS '研发费用金额',sum(DevelopmentCost) AS '开发费用金额',sum(HeadOfficeManageCost) AS '总部管理费用'," +
                "sum(BusinessCost) AS '商务费用金额',sum(WageSocialSecurityCost) AS '工资社保金额',sum(SalesDirectorCost) AS '销售总监费用'," +
                "sum(ProductDevelopmentFundCost) AS '产品发展基金',sum(MarketCost) AS '市场学术费',sum(MarketReadjustmentCost) AS '市场调节基金'," +
                "sum(ExperimenterBonusCost) AS '实验人员奖金金额',sum(PmsCost) AS 'PMS/支',sum(GuestMaintenanceCost) AS '日常客情维护费'," +
                "sum((BonusRatio * ExaminePrice * 0.01)) AS '奖金',sum(TfCost) AS '实验费(TF)金额',sum(RegionalCenterCost) AS '区域中心费用'," +
                "sum(RegionalCenterVipCost) AS '区域中心费用VIP维护',sum(FixedAssetsCost) AS '固定资产分摊',sum(TotalCost) AS '盈利中心费用合计'," +
                "sum(OperatingProfit) AS '经营利润',sum((OperatingProfitForYear / QuotaForYear)) AS '利润达成率' " +
                "FROM flow_statistics WHERE YEAR = {1} AND MONTH = {0} GROUP BY Sector;", month, year);
        //}
        //else
        //{
        //    sql = string.Format("SELECT sector," +
        //        "sum(FlowSalesMoney) AS '销售额',sum(SalesAllowances) AS '销售折让',sum(PurchasingCost) AS '采购成本'," +
        //        "sum(GrossProfit) AS '毛利',sum(TaxCost) AS '税金',sum(FinancialCost) AS '财务费用金额'," +
        //        "sum(RdCost) AS '研发费用金额',sum(DevelopmentCost) AS '开发费用金额',sum(HeadOfficeManageCost) AS '总部管理费用'," +
        //        "sum(BusinessCost) AS '商务费用金额',sum(WageSocialSecurityCost) AS '工资社保金额',sum(SalesDirectorCost) AS '销售总监费用'," +
        //        "sum(ProductDevelopmentFundCost) AS '产品发展基金',sum(MarketCost) AS '市场学术费',sum(MarketReadjustmentCost) AS '市场调节基金'," +
        //        "sum(ExperimenterBonusCost) AS '实验人员奖金金额',sum(PmsCost) AS 'PMS/支',sum(GuestMaintenanceCost) AS '日常客情维护费'," +
        //        "sum((BonusRatio * ExaminePrice * 0.01)) AS '奖金',sum(TfCost) AS '实验费(TF)金额',sum(RegionalCenterCost) AS '区域中心费用'," +
        //        "sum(RegionalCenterVipCost) AS '区域中心费用VIP维护',sum(FixedAssetsCost) AS '固定资产分摊',sum(TotalCost) AS '盈利中心费用合计'," +
        //        "sum(OperatingProfit) AS '经营利润',sum((OperatingProfitForYear / QuotaForYear)) AS '利润达成率' " +
        //        "FROM flow_statistics WHERE YEAR = {1} AND MONTH = ${0} AND (Sales='{2}' or Supervisor='{3}' or Manager='{4}' or Director='{5})' GROUP BY Sector;", month, year, user.userName, user.userName, user.userName, user.userName);
        //}
        return SqlHelper.Find(sql);
    }
}