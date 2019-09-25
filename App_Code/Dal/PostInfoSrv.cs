using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

/// <summary>
/// PostSrv 的摘要说明
/// </summary>
public class PostInfoSrv
{
    public PostInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetPosts(string companyId)
    {
        //string sql = string.Format("select p.*, c.name as company from posts p inner join companys c on p.companyId=c.Id"
        //    + " where p.companyId={0} order by p.postId ", companyId);
        string sql = "select p.*, (select count(*) from user_department_post u where u.postId = p.postId) as counts from posts p "
            +"order by orderBy";
        return SqlHelper.Find(sql);
    }

    public static DataSet GetUserByPost(string postName)
    {
        string sql = string.Format("SELECT users.userName as userName,department.name as department, "
            + "posts.postName as postName FROM posts "
            + "INNER JOIN user_department_post ON posts.postId = user_department_post.postId "
            + "INNER JOIN users ON user_department_post.wechatUserId = users.wechatUserId "
            + "INNER JOIN department ON user_department_post.departmentId = department.Id "
            + "WHERE postName = '{0}' ORDER BY department.orderForSameParent DESC, users.hiredate ASC", postName);
        return SqlHelper.Find(sql);
    }

    public static string GetPostId(string postName)
    {
        string sql = string.Format("select postId from posts where postName='{0}'", postName);
        object obj = SqlHelper.Scalar(sql);
        if (obj == null)
            return "";
        else
            return obj.ToString();
    }

    public static string Delete(string id,string postName)
    {
        string sql = string.Format("DELETE FROM posts WHERE postId = {0}\r\n;", id);
        sql += string.Format("DELETE FROM rights WHERE postName = {0}", postName);
        return SqlHelper.Exce(sql);
    }

    public static string DeleteRights(string post)
    {
        string sql = string.Format("DELETE FROM rights WHERE postName = '{0}'", post);
        return SqlHelper.Exce(sql);
    }

    public static void SaveRights(ArrayList list,string postName)
    {
        string sql = "";
        sql = "";

        foreach (object obj in list)
        {
            Dictionary<string, string> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(obj.ToString());
            string id = GetRightId(postName, dict["typeName"], dict["pageName"]);
            if(!dict.Keys.Contains("postName"))
                dict.Add("postName", postName);
            if (string.IsNullOrEmpty(id))
            {
                sql += SqlHelper.GetInsertIgnoreString(dict, "rights");
            }
            else
            {
                sql += SqlHelper.GetUpdateString(dict, "rights", string.Format(" where Id={0}", id));
            }            
        }
        SqlHelper.Exce(sql);
    }

    public static string InsertRights(ArrayList list)
    {
        string sql = "";
        foreach (Dictionary<string, string> dict in list)
        {
            sql += SqlHelper.GetInsertIgnoreString(dict, "rights");
        }        
        return SqlHelper.Exce(sql);
    }

    public static DataSet GetMenus()
    {
        string sql = "select * from left_menu order by MenuOrder";
        return SqlHelper.Find(sql);
    }

    public static DataSet GetRightsItem()
    {
        string sql = "select typeName,pageName ,0 as hasRight" +
                " from left_menu order by MenuOrder";
        return SqlHelper.Find(sql);
    }

    public static string GetRightId(string postName,string typeName,string pageName)
    {
        string sql = string.Format("select Id from rights where typeName='{0}' and pageName='{1}'"
            + " and postName='{2}'", typeName, pageName, postName);
        object obj = SqlHelper.Scalar(sql);
        if (obj == null)
            return "";
        else
            return obj.ToString();
    }

    public static DataSet GetRightsItem(string post)
    {
        //string sql = "select * from rights where postName = '" + post + "'";
        //DataSet ds = SqlHelper.Find(sql);
        //if(ds == null || ds.Tables[0].Rows.Count==0)
        //{
        //    sql = string.Format("select posts.postName,left_menu.typeName,left_menu.pageName ,0 as hasRight " +
        //        "from left_menu inner join posts where posts.postName='{0}' order by posts.postId,left_menu.MenuOrder"
        //        ,post);
        //    ds = SqlHelper.Find(sql);
        //}
        string sql = string.Format("select posts.postName,left_menu.typeName,left_menu.pageName ,0 as hasRight " +
                "from left_menu inner join posts where posts.postName='{0}' order by posts.postId,left_menu.MenuOrder\r\n;"
                , post);
        sql += "select * from rights where postName = '" + post + "'";
        DataSet ds = SqlHelper.Find(sql);
        if(ds !=null && ds.Tables.Count == 2)
        {
            foreach(DataRow row in ds.Tables[1].Rows)
            {
                if(row["hasRight"].ToString()=="1")
                {
                    for(int i=0;i<ds.Tables[0].Rows.Count;i++)
                    {
                        if(row["typeName"].ToString()== ds.Tables[0].Rows[i]["typeName"].ToString()
                            && row["pageName"].ToString() == ds.Tables[0].Rows[i]["pageName"].ToString())
                        {
                            ds.Tables[0].Rows[i]["hasRight"] = "1";
                            break;
                        }
                    }
                }
            }
        }
        return ds;
    }

    public static bool PostIsCanBeDelete(string id)
    {
        string sql = string.Format("select Id from user_department_post where postId = {0}", id);
        object res = SqlHelper.Scalar(sql);
        if (res == null)
            return true;
        else
            return false;
    }

    public static string InsertInfos(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertString(dict, "posts");
        sql = sql.Replace("Insert", "Insert ignore ");//避免重复插入数据
        return SqlHelper.Exce(sql);
    }

    public static string UpdateInfos(Dictionary<string, string> dict, string id)
    {
        string sql = SqlHelper.GetUpdateString(dict, "posts", "where postId=" + id);
        return SqlHelper.Exce(sql);
    }
}