using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

/// <summary>
/// PostInfoManage 的摘要说明
/// </summary>
public class PostInfoManage
{
    public PostInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetPostsDataset(string companyId)
    {
        return PostInfoSrv.GetPosts(companyId); ;
    }

    public static DataSet GetUserByPost(string postName)
    {
        return PostInfoSrv.GetUserByPost(postName);
    }

    public static ArrayList GetPosts(string companyId)
    {       
        DataSet ds = PostInfoSrv.GetPosts(companyId);
        if (ds == null)
        {
            return null;
        }
        ArrayList list = new ArrayList();
        
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("postName", row["postName"].ToString());
            dict.Add("postId", row["postId"].ToString());
            list.Add(dict);
        }
        return list;
    }

    

    public static DataTable GetRightsItem(string post)
    {
        DataTable dt = null;
        DataSet ds = null;
        if(string.IsNullOrEmpty(post))
            ds = PostInfoSrv.GetRightsItem();
        else
            ds = PostInfoSrv.GetRightsItem(post);
        if (ds != null)
        {
            dt = ds.Tables[0];
        }
        
        return dt;
    }

    public static string Delete(string id,string postName)
    {
        string res = "岗位已分配人员，无法删除！";
        if(PostInfoSrv.PostIsCanBeDelete(id))
        {
            res = PostInfoSrv.Delete(id, postName);
        }
        return res;
    }

    public static string InsertRight(string postName)
    {
        string res = "";
        PostInfoSrv.DeleteRights(postName);
        DataSet ds = PostInfoSrv.GetMenus();
        ArrayList list = new ArrayList();
        if (ds != null)
        {            
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("postName", postName);
                dict.Add("pageName", row["pageName"].ToString());
                dict.Add("hasRight", "0");
                dict.Add("typeName", row["typeName"].ToString());
                list.Add(dict);
            }
            res=PostInfoSrv.InsertRights(list);
        }
        return res;
    }
    

    public static string InsertInfos(Dictionary<string, string> dict, ArrayList list)
    {
        string res= PostInfoSrv.InsertInfos(dict);
        string[] strs1 = res.Split(':');
        string[] strs2 = strs1[1].Split(',');
        if (strs2[1].Contains("操作成功") && Convert.ToInt32(strs2[0]) > 0)
        {
            res = "新建岗位成功";
            //InsertRight(dict["postName"]);
            PostInfoSrv.SaveRights(list, dict["postName"]);
        }
        else if (strs2[1].Contains("操作成功") && Convert.ToInt32(strs2[0]) == 0)
        {
            res = "岗位名称已存在，请重新输入!";
        }
        else
        {
            res = strs2[1];
        }
        return res;
    }

    public static string UpdateInfos(Dictionary<string, string> dict, string id, ArrayList list)
    {
        string res = "岗位名称已存在，请重新输入!";
        string oldId = PostInfoSrv.GetPostId(dict["postName"]);
        if(string.Equals(id,oldId))
        {
            res = PostInfoSrv.UpdateInfos(dict, id);
            PostInfoSrv.SaveRights(list, dict["postName"]);
        }
        return res;
    }
}