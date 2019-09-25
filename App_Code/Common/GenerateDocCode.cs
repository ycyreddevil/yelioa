using System;
using System.Data;
/// <summary>
/// GenerateDocCode 的摘要说明
/// </summary>
public class GenerateDocCode
{
    public GenerateDocCode()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static String getDocCode()
    {
        String sql = String.Format("select count(*) from net_sales where CreateTime >= '{0}' and CreateTime <= '{1}'",
            DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
        string msg = "";
        DataSet ds = SqlHelper.Find(sql,ref msg);
        if (ds == null)
            return msg;
        String num = (Convert.ToInt32(ds.Tables[0].Rows[0][0]) + 1).ToString();
        if (num.Length == 1)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "00" + num;
        }
        else if (num.Length == 2)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "0" + num;
        }
        else
        {
            return DateTime.Now.ToString("yyyyMMdd") + num;
        }
    }

    public static String getReimburseCode()
    {
        String sql = String.Format("select count(*) from yl_reimburse where lmt = '{0}'", DateTime.Now.ToString("yyyy-MM-dd"));
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
            return msg;
        String num = (Convert.ToInt32(ds.Tables[0].Rows[0][0]) + 1).ToString();
        if (num.Length == 1)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "000" + num;
        }
        else if (num.Length == 2)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "00" + num;
        }
        else
        {
            return DateTime.Now.ToString("yyyyMMdd") + "0" + num;
        }
    }
    
    public static String getYLFormCode(string formName)
    {
        String sql = String.Format("select count(*) from wf_form_{1} where DATE_FORMAT(CreateTime,'%Y-%m-%d') = '{0}'", DateTime.Now.ToString("yyyy-MM-dd"), formName);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
            return msg;
        String num = (Convert.ToInt32(ds.Tables[0].Rows[0][0]) + 1).ToString();
        if (num.Length == 1)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "000" + num;
        }
        else if (num.Length == 2)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "00" + num;
        }
        else
        {
            return DateTime.Now.ToString("yyyyMMdd") + "0" + num;
        }
    }

    public static String getExpectFlowCode()
    {
        String sql = String.Format("select count(*) from expect_flow_submit where CreateTime >= '{0}' and CreateTime <= '{1}'",
            DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
            return msg;
        String num = (Convert.ToInt32(ds.Tables[0].Rows[0][0]) + 1).ToString();
        if (num.Length == 1)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "000" + num;
        }
        else if (num.Length == 2)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "00" + num;
        }
        else
        {
            return DateTime.Now.ToString("yyyyMMdd") + "0" + num;
        }
    }

    public static String getUpdateCostSharingCode()
    {
        String sql = String.Format("select count(*) from cost_sharing_record where CreateTime >= '{0}' and CreateTime <= '{1}' and insertOrUpdate = 1",
            DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
            return msg;
        String num = (Convert.ToInt32(ds.Tables[0].Rows[0][0]) + 1).ToString();
        if (num.Length == 1)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "000" + num;
        }
        else if (num.Length == 2)
        {
            return DateTime.Now.ToString("yyyyMMdd") + "00" + num;
        }
        else
        {
            return DateTime.Now.ToString("yyyyMMdd") + "0" + num;
        }
    }
}