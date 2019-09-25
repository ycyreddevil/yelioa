using System.Collections.Generic;
using System.Data;

/// <summary>
/// FeeReportManage 的摘要说明
/// </summary>
public class FeeReportManage
{
    public FeeReportManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static List<Dictionary<string, object>> getMonthlyData(int month, int year)
    {
        DataSet ds = FeeReportInfoSrv.getMonthlyData(month, year);
        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            for (int j = 1; j < ds.Tables[0].Columns.Count; j ++)
            {
                string sector = ds.Tables[0].Rows[i]["sector"].ToString();
                Dictionary<string, object> tempDict = new Dictionary<string, object>();
                tempDict.Add("sector", sector);
                string columnnm = ds.Tables[0].Columns[j].ToString();
                tempDict.Add("item", columnnm);
                tempDict.Add(month+"flowNum", ds.Tables[0].Rows[i][columnnm]);
                dataList.Add(tempDict);
            }
        }
        return dataList;
    }
}