using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// RightSrv 的摘要说明
/// </summary>
public class RightSrv
{
    public RightSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    public static string InitDatagrid()
    {
        string sql = "SELECT Id,typeName,pageName FROM `left_menu` WHERE Id NOT in(8,9,12) ORDER BY Id";
        return HelpSqlAndTableToJson(sql);
    }
    /// <summary>
    /// 获取应用列表
    /// </summary>
    /// <returns></returns>
    public static DataTable GetApplications()
    {
        string sql= "SELECT * FROM `left_menu` WHERE Id NOT in(8, 9, 12) ORDER BY MenuOrder";
        DataSet ds = SqlHelper.Find(sql);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
            return null;
        return ds.Tables[0];
    }

    /// <summary>
    /// 获取权限记录
    /// </summary>
    public static string GetHasRight(string id)
    {
        string sql = "SELECT b.`name`as Department,c.userName as user,c.userId,a.DepartmentId FROM " +
            "yl_rights AS a LEFT JOIN department AS b ON a.DepartmentId = b.Id LEFT JOIN users AS c ON a.WechatUserId = c.wechatUserId " +
            "where a.TableName = 'left_menu' AND a.SettingId ="+id;
        return (HelpSqlAndTableToJson(sql));
    }


 
    public static string UpdateRight( string id, List<string> departmentIds, DataTable wechatUserIds)
    {
        string sql = "DELETE FROM yl_rights WHERE TableName='left_menu' and SettingId =" + id + " ;";
        foreach(string departmentId in departmentIds)
        {
            sql += "insert yl_rights(TableName,SettingId,DepartmentId,LMT) values('left_menu','" + id + "','" + departmentId + "','" + DateTime.Now.ToString() + "');";
        }
        foreach (DataRow row in wechatUserIds.Rows)
        {
            sql += "insert yl_rights(TableName,SettingId,wechatUserId,LMT) values('left_menu','" + id + "','" + row["wechatUserId"] + "','" + DateTime.Now.ToString() + "');";

        }
        return SqlHelper.Exce(sql);   
    }

    public static DataSet GetDptsAndUsers()
    {
        string sql = "SELECT Id,`name` FROM `department`;SELECT userId,userName FROM `users` WHERE isValid='在职' ORDER BY userName";
        DataSet ds = SqlHelper.Find(sql);
        if (ds == null)
            return null;
        return ds;
    }


    public static string GetSettingId(string website)
    {
        DataSet ds = SqlHelper.Find("SELECT MenuOrder FROM left_menu WHERE  webSite = '" + website + "'");
        string settingId = "";
        if (ds != null && ds.Tables[0].Rows.Count > 0)
            settingId = ds.Tables[0].Rows[0][0].ToString();
        return settingId;
    }

    /// <summary>
    /// 由UserId获取wechatUserId
    /// </summary>
    /// <param name="UserId"></param>
    /// <returns></returns>
    public static DataTable GetWechatUserIds(List<string> UserIds)
    {
        string sql = "SELECT wechatUserId FROM users WHERE userId in('";
        DataTable dt = new DataTable();
        foreach(string UserId in UserIds)
        {
            sql += UserId + "','";
        }
        sql = sql.Substring(0, sql.Length - 2);
        sql += ")";
        DataSet ds = SqlHelper.Find(sql);
        if (ds != null)
            dt = ds.Tables[0];
        return dt;
    }
    /// <summary>
    /// 由企业微信ID获取人员所在的所有最底层部门ID
    /// </summary>
    /// <param name="wechatUserId">企业微信ID</param>
    /// <returns>最底层部门ID</returns>
    public static DataTable GetTheBottomDepartmentId(string wechatUserId)
    {
        string sql = "SELECT departmentId from user_department_post WHERE wechatUserId='" + wechatUserId + "'";
        DataSet ds = SqlHelper.Find(sql);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
            return ds.Tables[0];
        return null;
    }
    /// <summary>
    /// 获取该表内部门的部门链，如：（设备软件ID，业力研发部ID，业力集团ID，东森家园ID）是ds.tables[0].rows[0][0];
    /// </summary>
    /// <param name="theBottomDepartmentIds">底层部门ID</param>
    /// <returns>底层部门往上的部门链</returns>
    public static DataTable GetDepartmentIds(DataTable theBottomDepartmentIds)
    {
        string sql = "";
        foreach(DataRow row in theBottomDepartmentIds.Rows)
        {
            sql += "SELECT FindParentDepartment('"+row[0]+"') UNION ALL ";
        }
        sql = sql.Substring(0, sql.Length - 10);
        DataSet ds = SqlHelper.Find(sql);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
            return ds.Tables[0];
        return null;
    }
    /// <summary>
    /// 查询是否拥有应用权限
    /// </summary>
    /// <param name="wechatUserId">企业微信ID</param>
    /// <param name="departmentIds">部门ID列表</param>
    /// <param name="SettingId">应用ID</param>
    /// <returns></returns>
    public static DataSet FindRight(string wechatUserId,DataTable departmentIds,string SettingId)
    {
        string sql = "SELECT * FROM yl_rights WHERE TableName='left_menu' and SettingId='"+SettingId+"' and WechatUserId='" + wechatUserId + "' OR DepartmentId  in('";
        foreach(DataRow row in departmentIds.Rows)
        {
            sql += row[0] + "','";
        }
        sql = sql.Substring(0, sql.Length - 2);
        sql += ")";
        return SqlHelper.Find(sql);
    }

    private static string HelpSqlAndTableToJson(string sql)
    {
        DataSet ds = SqlHelper.Find(sql);
        string json = "";
        if (ds != null && ds.Tables[0].Rows.Count > 0)
            json = JsonHelper.DataTable2Json(ds.Tables[0]);
        return json;
    }


    
}