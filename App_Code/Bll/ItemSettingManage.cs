using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ItemSettingManage 的摘要说明
/// </summary>
public class ItemSettingManage
{
    public ItemSettingManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable getData(int year, int month, string sector)
    {
        DataSet ds = ItemSettingInfoSrc.getData(year, month, sector);
        if (ds == null)
            return null;
        else
            return ds.Tables[0];
    }

    public static string saveOrUpdateFinancialData(int year, int month, string itemnm, float num, string sector)
    {
        SqlExceRes sqlRes = new SqlExceRes(ItemSettingInfoSrc.saveOrUpdateFinancialData(year, month, itemnm, num, sector));
        return sqlRes.GetResultString("提交成功！", "提交失败");
    }

    public static DataTable getSector()
    {
        DataSet ds = ItemSettingInfoSrc.getSector();
        if (ds == null)
            return null;
        else
            return ds.Tables[0];
    }

    public static string DataArchive(DataRow dataRow, int year, int month)
    {
        string feeDetail = dataRow["feedetail"].ToString();
        string relatSector = dataRow["feearea"].ToString();
        float money = float.Parse(dataRow["money"].ToString());
        // 找到数据库中保存的报销项目
        string feeAccount = "";
        DataTable dt = getRelativeAccount(feeDetail);
        if (dt == null || dt.Rows.Count == 0)
        {
            return "";
        }
        else
        {
            feeAccount = dt.Rows[0]["feeAccount"].ToString();

            if ("开发费用金额".Equals(feeAccount))
            {
                feeAccount = "DevelopmentCost";
            }
            else if ("销售总监费用".Equals(feeAccount))
            {
                feeAccount = "SalesDirectorCost";
            }
            else if ("市场学术费".Equals(feeAccount))
            {
                feeAccount = "MarketCost";
            }
            else if ("市场调节基金".Equals(feeAccount))
            {
                feeAccount = "MarketReadjustmentCost";
            }
            else if ("区域中心费用".Equals(feeAccount))
            {
                feeAccount = "RegionalCenterCost";
            }
            else if ("区域中心费用VIP".Equals(feeAccount))
            {
                feeAccount = "RegionalCenterVipCost";
            }
            else if ("商务费用金额".Equals(feeAccount))
            {
                feeAccount = "BusinessCost";
            }
            else if ("产品发展基金".Equals(feeAccount))
            {
                feeAccount = "ProductDevelopmentFundCost";
            }
            else if ("实验费（TF）金额".Equals(feeAccount))
            {
                feeAccount = "TfCost";
            }
        }
        // 找到数据库中对应的盈利中心
        string sector = "";
        dt = getRelativeSector(relatSector);
        if (dt == null || dt.Rows.Count == 0)
        {
            return "";
        }
        else
        {
            sector = dt.Rows[0]["sector"].ToString();
        }

        DataTable oldMoneyDt = ItemSettingManage.getOldMoney(feeAccount, year, month, sector);

        float oldmoney = 0;

        if (oldMoneyDt != null && oldMoneyDt.Rows.Count != 0)
        {
            object feeAccountObj = oldMoneyDt.Rows[0][feeAccount];

            if (feeAccountObj != null)
            {
                oldmoney = float.Parse(feeAccountObj.ToString());
            }
        }

        SqlExceRes sqlRes = new SqlExceRes(ItemSettingInfoSrc.saveOrUpdateFinancialData(year, month, feeAccount, money+oldmoney, sector));

        return sqlRes.GetResultString("提交成功！", "提交失败", "提交失败");
    }

    public static DataTable getRelativeAccount(string feeDetail)
    {
        DataSet ds = ItemSettingInfoSrc.getRelativeAccount(feeDetail);
        if (ds == null)
            return null;
        else
            return ds.Tables[0];
    }

    public static DataTable getRelativeSector(string sector)
    {
        DataSet ds = ItemSettingInfoSrc.getRelativeSector(sector);
        if (ds == null)
            return null;
        else
            return ds.Tables[0];
    }

    public static DataTable getOldMoney(string itemnm, int year, int month, string sector)
    {
        DataSet ds = ItemSettingInfoSrc.getOldMoney(itemnm, year, month, sector);
        if (ds == null)
            return null;
        else
            return ds.Tables[0];
    }
}