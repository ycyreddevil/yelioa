using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// RgithsSrv 的摘要说明
/// </summary>
public class RigthsSrv
{
    public RigthsSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetMenus(List<DepartmentPost> dpList)
    {
        if (dpList.Count == 0)
            return null;
        string posts = "";
        foreach(DepartmentPost dp in dpList)
        {
            posts += string.Format("'{0}',",dp.postId);
        }
        posts = posts.Substring(0, posts.Length - 1);
        string sql = string.Format("select * from rights where postName in ("
            + "SELECT postName from posts where postId in ({0})) and hasRight=1", posts);
        DataSet ds = SqlHelper.Find(sql);
        if(ds == null)
        {
            return null;
        }
        else if(ds.Tables[0].Rows.Count == 0)
        {
            return ds;
        }

        sql = "";
        foreach(DataRow row in ds.Tables[0].Rows)
        {
            sql += string.Format("select * from left_menu where typeName='{0}' and pageName ='{1}'"
                , row["typeName"].ToString(), row["pageName"].ToString());
            sql += " union all ";
        }
        sql = sql.Substring(0, sql.Length - " union all ".Length);
        sql += " order by MenuOrder";
        return SqlHelper.Find(sql);
    }

    public static void InsertRight(DataTable dt)
    {
        string sql = SqlHelper.GetInsertIgnoreString(dt, "rights");
        SqlHelper.Exce(sql);
    }
}

public class MenuRight
{
    public string PageName { get; set; }
    //public string TypeName { get; set; }
    public string HasRigth { get; set; }

    public MenuRight(string pName, string b)
    {
        PageName = pName;
        //TypeName = tName;
        HasRigth = b;
    }
}