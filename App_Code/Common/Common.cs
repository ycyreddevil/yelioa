using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Collections;
using System.Data;

/// <summary>
/// Common 的摘要说明
/// </summary>
public class Common
{
    public Common()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static Dictionary<string, string>  ChangeDictionaryValue(Dictionary<string, string> dict,string key, string value)
    {
        if (!dict.ContainsKey(key))
            dict.Add(key, value);
        else
        {
            dict.Remove(key);
            dict.Add(key, value);
        }        
        return dict;
    }

    public static void AddKeyValueOfDictionary(ref Dictionary<string, string> dict, string oldKey, string newKey)
    {
        if (!dict.Keys.Contains(oldKey) || dict.Keys.Contains(newKey))
        {
            return;
        }
        string value = dict[oldKey];
        dict.Add(newKey, value);
    }

    public static string GetApplicationValid(string application)
    {
        string sql = string.Format("select IsValid from yl_application where Application='{0}'", application);
        DataSet ds = SqlHelper.Find(sql);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
            return "0";
        else
            return ds.Tables[0].Rows[0][0].ToString();
    }
}