using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// ProductInfoManage 的摘要说明
/// </summary>
public class ProductInfoManage
{
    public ProductInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable GetInfos( string companyId,string searchString)
    {
        DataSet ds = ProductInfoSrv.GetInfos(companyId);
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
                if (row["code"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["name"].ToString(), searchString)
                    //|| PinYinHelper.IsEqual(row["fullName"].ToString(), searchString)
                    || row["name"].ToString().Trim().Contains(searchString)
                    //|| row["fullName"].ToString().Trim().Contains(searchString))
                    || row["manufacturer"].ToString().Trim().Contains(searchString)
                    || PinYinHelper.IsEqual(row["manufacturer"].ToString(), searchString)
                    )
                {
                    dt.Rows.Add(row.ItemArray);
                    continue;
                }
            }
        }
        return dt;
    }

    public static DataSet GetAliasData(string code)
    {
        return ProductInfoSrv.GetAliasData(code);
    }

    public static DataSet GetAliasData()
    {
        return ProductInfoSrv.GetAliasData();
    }

    public static string SaveAliasData(string pCode,string aName, string aliasSpecification, string type)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("productCode", pCode);
        dict.Add("alias", aName);
        dict.Add("aliasSpecification", aliasSpecification);
        dict.Add("type", type);
        return ProductInfoSrv.SaveAliasData(dict, pCode, type);
    }

    public static string InsertAliasData(Dictionary<string, string> dict)
    {
        return ProductInfoSrv.InsertAliasData(dict);
    }

    public static string InsertAliasData(DataTable dt)
    {
        dt.Columns["产品编码(必填)"].ColumnName = "productCode";
        dt.Columns["数据类型(必填)"].ColumnName = "type";
        dt.Columns["转义产品编码"].ColumnName = "aliasCode";
        dt.Columns["转义产品名称"].ColumnName = "alias";

        return ProductInfoSrv.InsertAliasData(dt);
    }

    public static string UpdateAliasData(Dictionary<string, string> dict, string id)
    {
        return ProductInfoSrv.UpdateAliasData(dict,id);
    }

    public static string DeleteAliasData(string id)
    {
        return ProductInfoSrv.DeleteAliasData(id);
    }

    public static object ValidateProductCode(string companyId, string code)
    {
        return ProductInfoSrv.ValidateProductCode(companyId, code);
    }

    public static string InsertInfos(Dictionary<string, string> dict)
    {
        return ProductInfoSrv.InsertInfos(dict);
    }

    public static string SaveInfos(DataTable dt)
    {
        string res = "文件格式有错误！";
        try
        {
            dt.Columns["代码"].ColumnName = "code";
            dt.Columns["名称"].ColumnName = "name";
            dt.Columns["全名"].ColumnName = "fullName";
            dt.Columns["规格型号"].ColumnName = "specification";
            dt.Columns["保质期(天)"].ColumnName = "shelfLife";
            dt.Columns["存货科目代码"].ColumnName = "stockAccountCode";
            dt.Columns["销售收入科目代码"].ColumnName = "salesIncomeAccountCode";
            dt.Columns["销售成本科目代码"].ColumnName = "salesCostAccountCode";
            dt.Columns["税率(%)"].ColumnName = "taxRate";
            dt.Columns["生产企业许可证号或备案凭证号"].ColumnName = "manufacturerLicenseNumber";
            dt.Columns["储运条件"].ColumnName = "storageCondition";
            dt.Columns["产地"].ColumnName = "placeOfProduction";
            dt.Columns["生产厂家"].ColumnName = "manufacturer";
            dt.Columns["产品注册证号或备案凭证号"].ColumnName = "productLicenseNumber";
            dt.Columns["商品描述"].ColumnName = "remark";
            
        }
        catch(Exception ex)
        {
            res = ex.ToString();
            return res;
        }

        res = ProductInfoSrv.SaveInfos(dt);
        return res;    
    }

    public static string UpdateInfos(Dictionary<string, string> dict,string id)
    {
        return ProductInfoSrv.UpdateInfos(dict,id);
    }

    public static string Delete(string id)
    {
        return ProductInfoSrv.Delete( id);
    }
}