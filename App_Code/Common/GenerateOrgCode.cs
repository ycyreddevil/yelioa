using System;
using System.Data;

/// <summary>
/// GenerateOrgCode 的摘要说明
/// </summary>
public class GenerateOrgCode
{
	public GenerateOrgCode()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public static String generateCode(String province, String city, string rank, string type)
    {
        string cityCD = "";
        DataSet ds;
        if (!"医院".Equals(type))
        {
            cityCD = "202000";
            rank = "";
        }
        else
        {
            String province_sql = String.Format("select CD from addv_copy where nm = '{0}'", province);
            ds = SqlHelper.Find(province_sql);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;
            String provinceCD = ds.Tables[0].Rows[0][0].ToString();
            String city_sql = String.Format("select CD from addv_copy where nm = '{0}' and cd like '{1}%'", city, provinceCD);
            ds = SqlHelper.Find(city_sql);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;
            cityCD = ds.Tables[0].Rows[0][0].ToString();
        }
        
        String sql = String.Format("select count(*) from organization where code like '{0}%'", cityCD);
        ds = SqlHelper.Find(sql);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
            return null;
        String num = (Convert.ToInt32(ds.Tables[0].Rows[0][0]) + 1).ToString();
        if (num.Length == 1)
        {
            return cityCD + rank + "000" + num;
        }
        else if (num.Length == 2)
        {
            return cityCD + rank + "00" + num;
        }
        else if (num.Length == 3)
        {
            return cityCD + rank + "0" + num;
        }
        else
        {
            return cityCD + rank + num;
        }
    }
}