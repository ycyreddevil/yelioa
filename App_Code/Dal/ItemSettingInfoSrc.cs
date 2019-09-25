using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ItemSettingInfoSrc 的摘要说明
/// </summary>
public class ItemSettingInfoSrc
{
    public ItemSettingInfoSrc()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet getData(int year, int month, string sector)
    {
        string getDataSql = string.Format("select * from financial_item where year = {0} and month = {1} and sector = '{2}'", year, month, sector);
        DataSet ds = SqlHelper.Find(getDataSql);
        return ds;
    }

    public static string saveOrUpdateFinancialData(int year, int month, string itemnm, float num, string sector)
    {
        string saveOrUpdateSql = string.Format("insert into financial_item (year,month,sector,{0}) " +
            "values ({1},{2},'{4}',{3}) ON DUPLICATE KEY UPDATE {0} = {3}", itemnm, year, month, num, sector);
        return SqlHelper.Exce(saveOrUpdateSql);
    }

    public static DataSet getSector()
    {
        string getSectorSql = string.Format("select distinct sector from cost_sharing where Sector is not null");
        return SqlHelper.Find(getSectorSql);
    }

    public static DataSet getRelativeAccount(string feeDetail)
    {
        string getRelativeAccountSql = string.Format("select feeAccount from fee_account_corresponding where feeDetail = '{0}'", feeDetail);
        return SqlHelper.Find(getRelativeAccountSql);
    }

    public static DataSet getRelativeSector(string relativeSector)
    {
        string getRelativeSectorSql = string.Format("select sector from sector_corresponding where relatSector = '{0}'", relativeSector);
        return SqlHelper.Find(getRelativeSectorSql);
    }

    public static DataSet getOldMoney(string itemnm, int year, int month, string sector)
    {
        string getOldMoneySql = string.Format("select {0} from financial_item where year = {1} and month = {2} and sector = '{3}'", itemnm, year, month, sector);
        return SqlHelper.Find(getOldMoneySql);
    }
}