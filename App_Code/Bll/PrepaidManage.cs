using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// PrepaidManage 的摘要说明
/// </summary>
public class PrepaidManage
{
    public PrepaidManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable getPrepaidData()
    {
        DataSet ds = PrepaidSrv.getPrepaidData();
        if (ds != null)
        {
            return ds.Tables[0];
        }
        return null;
    }

    public static string updatePrepaidData(string jsonRows)
    {
        DataTable dt = JsonHelper.Json2Dtb(jsonRows);

        if (dt != null)
        {
            foreach (DataRow dr in dt.Rows)
            {
                string sector = dr["sector"].ToString();
                float num = float.Parse(dr["prepaidMoney"].ToString());
                float num2 = float.Parse(dr["sumPrepaidMoney"].ToString());

                PrepaidSrv.updatePrepaid(sector, num, num2);
            }

            return "ok";
        }

        return "";
    }
}