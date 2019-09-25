using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using System.Collections;

/// <summary>
/// LeftMenu 的摘要说明
/// </summary>
public class LeftMenu
{
    public LeftMenu(string wechatUserId)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        
        Initi(wechatUserId);
        AddDefautShowMenu();
    }

    Dictionary<string, List<Menu>> dict = null;

    private void Initi(string wechatUserId)
    {
        
        
        DataTable dt = RightHelper.GetUserRights(wechatUserId);
        if (dt==null||dt.Rows.Count == 0)
        {
            return;
        }
        dict = new Dictionary<string, List<Menu>>();
        foreach (DataRow row in dt.Rows)
        {
            string key = row["typeName"].ToString();
            Menu menu = new Menu(row["pageName"].ToString(), row["webSite"].ToString());
            if (dict.Keys.Contains(key))
            {
                bool hasContained = false;
                foreach(Menu m in dict[key])
                {
                    if (m.Name == menu.Name)
                    {
                        hasContained = true;
                        break;
                    }                        
                }      
                if(!hasContained)
                    dict[key].Add(menu);
            }
            else
            {
                List<Menu> list = new List<Menu>();
                list.Add(menu);
                dict.Add(key, list);
            }
        }
    }

    private void AddDefautShowMenu()
    {
        //DataTable dt = new DataTable();
        //dt.Columns.Add("postName");
        //dt.Columns.Add("typeName");
        //dt.Columns.Add("pageName");
        //dt.Columns.Add("hasRight");

        //DataRow row = dt.NewRow();
        //row["postName"] = post;
        //row["typeName"] = "联系人";
        //row["pageName"] = "通讯录";
        //row["hasRight"] = "1";
        //dt.Rows.Add(row);

        //row = dt.NewRow();
        //row["postName"] = post;
        //row["typeName"] = "我的信息";
        //row["pageName"] = "个人设置";
        //row["hasRight"] = "1";
        //dt.Rows.Add(row);
        //RigthsManage.InsertRight(dt);
        if(dict == null)
            dict = new Dictionary<string, List<Menu>>();
        {
            List<Menu> list = new List<Menu>();
            Menu menu = new Menu("通讯录", "memberSearch.aspx");
            list.Add(menu);
            if(!dict.Keys.Contains("联系人"))
                dict.Add("联系人", list);
        }
        {
            List<Menu> list = new List<Menu>();
            Menu menu = new Menu("个人设置", "MyInfo.aspx");
            list.Add(menu);
            if (!dict.Keys.Contains("我的信息"))
                dict.Add("我的信息", list);
        }

    }

    public string GetJson()
    {
        return JsonConvert.SerializeObject(dict);
    }
}

public class Menu
{
    public Menu(string name, string webSite)
    {
        Name = name;
        WebSite = webSite;
    }

    public string Name { get; set; }
    public string WebSite { get; set; }
}