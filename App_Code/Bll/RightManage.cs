using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Authorization 的摘要说明
/// </summary>
public class RightManage
{
    public RightManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 初始化应用目录
    /// </summary>
   public static string InitDatagrid()
    {
        return RightSrv.InitDatagrid();
    }

    /// <summary>
    /// 获取权限记录
    /// </summary>
    public static string GetHasRight(string  id)
    {
        return RightSrv.GetHasRight(id);
    }

    /// <summary>
    /// 由企业微信ID获取该成员的一级级往上的所有部门
    /// </summary>
    /// <param name="wechatUserId">企业微信ID</param>
    /// <returns>该成员的部门链</returns>
    public static DataTable GetDepartmentIds(string wechatUserId)
    {
        DataTable theBottomDepartmentIds = RightSrv.GetTheBottomDepartmentId(wechatUserId);
        if(theBottomDepartmentIds!=null)
        {
            DataTable dt = RightSrv.GetDepartmentIds(theBottomDepartmentIds);
            DataTable departmentIds = new DataTable();
            departmentIds.Columns.Add("departmentId", Type.GetType("System.String"));
            if(dt!=null)
            {
                foreach(DataRow row in dt.Rows)
                {
                    string[] departmentId = row[0].ToString().Split(',');
                    foreach(string str in departmentId)
                    {
                        departmentIds.Rows.Add(str);
                    }
                }
            }
            return departmentIds;
        }
        return null;
    }

 
    public static string UpdateRight(string id, string  departIds, string  UserIds)
    {
        List<string> departmentIds = new List<string>();
        DataTable wechatUserIds = new DataTable();
        if(departIds!=null)
           departmentIds = StrsToStr(departIds);
        if(UserIds!=null)
           wechatUserIds = RightSrv.GetWechatUserIds(StrsToStr(UserIds));
        string res= RightSrv.UpdateRight(id, departmentIds,wechatUserIds);
        return SuccessOperate(res,departmentIds.Count+wechatUserIds.Rows.Count+1);
    }

    public static string  GetSearched(string searchStr)
    {
        DataSet ds = RightSrv.GetDptsAndUsers();
        if(ds!=null&&ds.Tables[0].Rows.Count>0&& ds.Tables[1].Rows.Count > 0)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("value", Type.GetType("System.String"));
            dt.Columns.Add("text", Type.GetType("System.String"));
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                if (PinYinHelper.IsEqual(row[1].ToString(), searchStr)
                        || row[1].ToString().Trim().Contains(searchStr)
                         )
                    dt.Rows.Add(row[0], row[1]);
            }
            foreach (DataRow row in ds.Tables[1].Rows)
            {
                if (PinYinHelper.IsEqual(row[1].ToString(), searchStr)
                        || row[1].ToString().Trim().Contains(searchStr)
                         )
                    dt.Rows.Add(row[0], row[1]);
            }
            return JsonHelper.DataTable2Json(dt);
        }
        return "";
    }
    /// <summary>
    /// json字符串转换成Object<list<string>>
    /// </summary>
    /// <param name="strs"></param>
    /// <returns></returns>
    private static List<string> StrsToStr(string strs)
    {
        return JsonHelper.DeserializeJsonToObject<List<string>>(strs);
    }



 
    ///// <summary>
    ///// 将datatable转换成website名称 menu
    ///// </summary>
    //private static Dictionary<string, List<Menu>> GetMenuDictionary(DataTable dt)
    //{
    //    Dictionary<string, List<Menu>> dict = new Dictionary<string, List<Menu>>();
    //    foreach (DataRow row in dt.Rows)
    //    {
    //        string key = row["typeName"].ToString();
    //        Menu menu = new Menu(row["pageName"].ToString(), row["website"].ToString());
    //        if (dict.Keys.Contains(key))
    //        {
    //            bool hasContained = false;
    //            foreach (Menu m in dict[key])
    //            {
    //                if (m.Name == menu.Name)
    //                {
    //                    hasContained = true;
    //                    break;
    //                }
    //            }
    //            if (!hasContained)
    //                dict[key].Add(menu);
    //        }
    //        else
    //        {
    //            List<Menu> list = new List<Menu>();
    //            list.Add(menu);
    //            dict.Add(key, list);
    //        }
    //    }
    //    return dict;
    //}

    private static string SuccessOperate(string str, int sum)
    {
        string ret = "操作成功";
        int count = (str.Length - str.Replace(ret, "").Length) / ret.Length;
        if (count == sum)
            return ret;
        else
            return "";
    }
}