using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// RgithsManage 的摘要说明
/// </summary>
public class RigthsManage
{
    public RigthsManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetMenus(List<DepartmentPost> dpList)
    {
        return RigthsSrv.GetMenus(dpList);
    }

    public static void InsertRight(DataTable dt)
    {
        RigthsSrv.InsertRight(dt);
    }
}