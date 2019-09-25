using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// EnterStockInfoSrv 的摘要说明
/// </summary>
public class EnterStockInfoSrv
{
    public EnterStockInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetInfos(DateTime start, DateTime end, string companyId)
    {
        string sql = string.Format("select enter_stock.*, companys.name as company from enter_stock inner join companys"+
            " on enter_stock.companyId=companys.Id where enter_stock.companyId ={0} and (enter_stock.date between '{1}' and '{2}')"
            , companyId, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
        return SqlHelper.Find(sql);
    }

    public static string InsertInfos(DataTable dt, string companyId)
    {
        string sql = "";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (DataRow row in dt.Rows)
        {
            dict.Clear();
            dict.Add("documentNumber", row["单据编号"].ToString());
            dict.Add("productCode", row["物料代码"].ToString());
            dict.Add("productName", row["物料名称"].ToString());
            dict.Add("date", row["日期"].ToString());
            dict.Add("manufacturer", row["供应商"].ToString());
            dict.Add("specification", row["规格型号"].ToString());
            dict.Add("batchNumber", row["批号"].ToString());
            dict.Add("documentCreaterName", row["制单"].ToString());
            dict.Add("amountReceivable", row["应收数量"].ToString());
            dict.Add("amountReceived", row["实收数量"].ToString());
            dict.Add("unit", row["单位"].ToString());
            dict.Add("price", row["单价"].ToString());
            dict.Add("sumOfMonye", row["金额"].ToString());
            dict.Add("companyId", companyId);
            string cmd = SqlHelper.GetInsertString(dict, "enter_stock");
            cmd = cmd.Replace("Insert", "Insert ignore ");//避免重复插入数据
            sql += cmd;
        }        
        
        return SqlHelper.Exce(sql);
    }

    public static string DeleteData(string id)
    {
        string sql = string.Format("delete from enter_stock where Id = {0}", id);
        if (!StringTools.IsInt(id))
        {
            id = id.Substring(0, id.Length - 1);
            sql = string.Format("delete from enter_stock where Id in ({0})", id);
        }

        return SqlHelper.Exce(sql);
    }
}