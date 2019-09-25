using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// RightHelper 的摘要说明
/// </summary>
public class RightHelper
{
    public RightHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    /// <summary>
    /// 根据企业微信ID获取该成员所拥有的应用权限
    /// </summary>
    /// <param name="wechatUserId">人员企业微信ID</param>
    /// <returns>该人员拥有权限的列表</returns>
    public static DataTable  GetUserRights(string wechatUserId)
    {
        DataTable dt = RightManage.GetDepartmentIds(wechatUserId);
        if (dt != null && dt.Rows.Count > 0)
        {
            DataTable rights = RightSrv.GetApplications();
            if (rights != null)
            {
                for (int i = rights.Rows.Count - 1; i >= 0; i--)
                {
                    if (!HasRight(dt, wechatUserId, rights.Rows[i]["Id"].ToString()))
                        rights.Rows.RemoveAt(i);
                }
            }
            return rights;
        }
        return null;
    }
    /// <summary>
    /// 判断人员是否拥有该应用权限
    /// </summary>
    /// <param name="wechatUserId">人员企业微信ID</param>
    /// <param name="SettingId">应用ID</param>
    /// <returns>是否拥有应用权限</returns>
    public static Boolean HasRight(string wechatUserId,string  SettingId)
    {
        DataTable dt = RightManage.GetDepartmentIds(wechatUserId);
        if (dt != null && dt.Rows.Count > 0)
        {
            DataSet ds = RightSrv.FindRight(wechatUserId, dt, SettingId);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return false;
            return true;
        }
        return false;
    }
    /// <summary>
    /// 判断人员是否拥有该应用权限
    /// </summary>
    /// <param name="departmentIds">部门ID</param>
    /// <param name="wechatUserId">成员企业微信ID</param>
    /// <param name="SettingId">应用ID</param>
    /// <returns></returns>
    public static Boolean HasRight(DataTable departmentIds, string wechatUserId, string SettingId)
    {
        DataSet ds = RightSrv.FindRight(wechatUserId, departmentIds, SettingId);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
            return false;
        return true;
    }
}