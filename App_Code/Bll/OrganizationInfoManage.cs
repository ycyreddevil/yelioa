using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// OrganizationInfoManage 的摘要说明
/// </summary>
public class OrganizationInfoManage
{
    public OrganizationInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable GetData(string searchString)
    {
        DataSet ds = OrganizationInfoSrv.GetData();
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
                    || PinYinHelper.IsEqual(row["fullName"].ToString(), searchString)
                    || row["name"].ToString().Trim().Contains(searchString)
                    || row["fullName"].ToString().Trim().Contains(searchString))
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
        return OrganizationInfoSrv.GetAliasData(code);
    }

    public static DataSet GetAliasData()
    {
        return OrganizationInfoSrv.GetAliasData();
    }

    public static string SaveAliasData(string code, string aName, string type)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("code", code);
        dict.Add("alias", aName);
        dict.Add("type", type);
        return OrganizationInfoSrv.SaveAliasData(dict, code, type);
    }

    public static string InsertAliasData(Dictionary<string, string> dict)
    {
        return OrganizationInfoSrv.InsertAliasData(dict);
    }

    public static string InsertAliasData(DataTable dt)
    {
        dt.Columns["机构编码(必填)"].ColumnName = "code";
        dt.Columns["数据类型(必填)"].ColumnName = "type";
        dt.Columns["转义机构编码"].ColumnName = "aliasCode";
        dt.Columns["转义机构名称"].ColumnName = "alias";

        return OrganizationInfoSrv.InsertAliasData(dt);
    }

    public static string UpdateAliasData(Dictionary<string, string> dict, string id)
    {
        return OrganizationInfoSrv.UpdateAliasData(dict, id);
    }

    public static string DeleteAliasData(string id)
    {
        return OrganizationInfoSrv.DeleteAliasData(id);
    }

    public static string InsertInfo(Dictionary<string, string> dict)
    {
        SqlExceRes res = new SqlExceRes(OrganizationInfoSrv.InsertInfo(dict));
        return res.GetResultString("提交成功！", "数据有重复");
    }

    public static string UpdateInfo(Dictionary<string, string> dict, string id)
    {
        SqlExceRes res = new SqlExceRes(OrganizationInfoSrv.UpdateInfo(dict, id));
        return res.GetResultString("提交成功！", "");
    }

    public static string DeleteInfo(string id)
    {
        SqlExceRes res = new SqlExceRes(OrganizationInfoSrv.DeleteInfo(id));
        return res.GetResultString("删除成功！", "");
    }

    public static string checkCode(string code)
    {
        return OrganizationInfoSrv.checkCode(code);
    }

    public static string SaveInfos(DataTable dt)
    {
        string res = "文件格式有错误！";
        try
        {
            dt.Columns["医院编码"].ColumnName = "code";
            dt.Columns["医院名称"].ColumnName = "name";
            dt.Columns["医院等级"].ColumnName = "rank";
            dt.Columns["地级市"].ColumnName = "city";
            dt.Columns["行政区域"].ColumnName = "administrativeArea";
            for(int i=dt.Columns.Count-1;i>=0;i--)
            {
                if (StringTools.HasChinese(dt.Columns[i].ColumnName))
                    dt.Columns.RemoveAt(i);
            }
        }
        catch (Exception ex)
        {
            res = ex.ToString();
            return res;
        }

        res = OrganizationInfoSrv.SaveInfos(dt);
        return res;
    }
}