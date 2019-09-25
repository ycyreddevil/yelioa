using System;
using System.Data;

/// <summary>
/// Prepaid 的摘要说明
/// </summary>
public class PrepaidSrv
{
    public PrepaidSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet getPrepaidData()
    {
        string sql = string.Format("select sector, year, month, prepaidMoney, sumPrepaidMoney from prepaid where year = {0} and month = {1}", DateTime.Now.Year , DateTime.Now.Month);
        return SqlHelper.Find(sql);
    }

    public static string updatePrepaid(string sector, float num, float num2)
    {
        string saveOrUpdateSql = string.Format("insert into prepaid (year,month,sector,prepaidMoney) " +
            "values ({0},{1},'{2}',{3}) ON DUPLICATE KEY UPDATE prepaidMoney = prepaidMoney + {3}, sumPrepaidMoney = {4}", DateTime.Now.Year, DateTime.Now.Month, sector, num, num2);

        return SqlHelper.Exce(saveOrUpdateSql);
    }
}