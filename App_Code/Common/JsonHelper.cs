using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;

using System.Web.Script.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// JsonHelper 的摘要说明
/// </summary>
public class JsonHelper
{
    public JsonHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// JSON序列化
    /// </summary>
    public static string JsonSerializer<T>(T t)
    {
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        MemoryStream ms = new MemoryStream();
        ser.WriteObject(ms, t);
        string jsonString = Encoding.UTF8.GetString(ms.ToArray());
        ms.Close();
        //替换Json的Date字符串
        string p = @"\\/Date\((\d+)\+\d+\)\\/";
        MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
        Regex reg = new Regex(p);
        jsonString = reg.Replace(jsonString, matchEvaluator);
        return jsonString;
    }

    /// <summary>
    /// JSON反序列化
    /// </summary>
    public static T JsonDeserialize<T>(string jsonString)
    {
        //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
        string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
        MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
        Regex reg = new Regex(p);
        jsonString = reg.Replace(jsonString, matchEvaluator);

        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        T obj = (T)ser.ReadObject(ms);
        return obj;
    }
    /// <summary>
    /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
    /// </summary>
    private static string ConvertJsonDateToDateString(Match m)
    {
        string result = string.Empty;
        DateTime dt = new DateTime(1970, 1, 1);
        dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
        dt = dt.ToLocalTime();
        result = dt.ToString("yyyy-MM-dd HH:mm:ss");
        return result;
    }

    /// <summary>
    /// 将时间字符串转为Json时间
    /// </summary>
    private static string ConvertDateStringToJsonDate(Match m)
    {
        string result = string.Empty;
        DateTime dt = DateTime.Parse(m.Groups[0].Value);
        dt = dt.ToUniversalTime();
        TimeSpan ts = dt - DateTime.Parse("1970-01-01");
        result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
        return result;
    }

    private static string VerifyString(string str)
    {
        str = str.Replace("\"", "~");
        return str;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string DataTableToJsonForEasyUiDataGridLoadDataMethod(DataTable dt,DataTable dtFooter)
    {
        return DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt, dtFooter);
    }

   

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string DataTableToJsonForEasyUiDataGridLoadDataMethod( DataTable dt)
    {
        return DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
    }

    /// <summary>
    /// Msdn
    /// </summary>
    /// <param name="jsonName"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string DataTableToJsonForEasyUiDataGridLoadDataMethod(int totalRows, DataTable dt)
    {
        StringBuilder Json = new StringBuilder();
        Json.Append("{\"total\":" + totalRows.ToString() + ",\"rows\":[");
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Json.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string name = VerifyString(dt.Columns[j].ColumnName.ToString());
                    name = name.Replace("\r", "");
                    name = name.Replace("\n", "");
                    string value = VerifyString(dt.Rows[i][j].ToString());
                    value = value.Replace("\r", "");
                    value = value.Replace("\n", "");
                    Json.Append("\"" + name + "\":\"" + value + "\"");
                    if (j < dt.Columns.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
                Json.Append("}");
                if (i < dt.Rows.Count - 1)
                {
                    Json.Append(",");
                }
            }
        }
        Json.Append("]}");
        string res = Json.ToString();
        return res;
    }

    public static string DataTableToJsonForEasyUiDataGridLoadDataMethod(int totalRows, DataTable dt, DataTable dtFooter)
    {
        StringBuilder Json = new StringBuilder();
        Json.Append("{\"total\":" + totalRows.ToString() + ",\"rows\":[");
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Json.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string name = VerifyString(dt.Columns[j].ColumnName.ToString());
                    name = name.Replace("\r", "");
                    name = name.Replace("\n", "");
                    string value = VerifyString(dt.Rows[i][j].ToString());
                    value = value.Replace("\r", "");
                    value = value.Replace("\n", "");
                    Json.Append("\"" + name + "\":\"" + value + "\"");
                    if (j < dt.Columns.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
                Json.Append("}");
                if (i < dt.Rows.Count - 1)
                {
                    Json.Append(",");
                }
            }
        }
        if(dtFooter.Rows.Count >0)
        {
            Json.Append("],\"footer\":[");
            for (int i = 0; i < dtFooter.Rows.Count; i++)
            {
                Json.Append("{");
                for (int j = 0; j < dtFooter.Columns.Count; j++)
                {
                    string name = VerifyString(dtFooter.Columns[j].ColumnName.ToString());
                    name = name.Replace("\r", "");
                    name = name.Replace("\n", "");
                    string value = VerifyString(dtFooter.Rows[i][j].ToString());
                    value = value.Replace("\r", "");
                    value = value.Replace("\n", "");
                    Json.Append("\"" + name + "\":\"" + value + "\"");
                    if (j < dtFooter.Columns.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
                Json.Append("}");
                if (i < dtFooter.Rows.Count - 1)
                {
                    Json.Append(",");
                }
            }
        }
        
        
        Json.Append("]}");
        string res = Json.ToString();
        return res;
    }

    /// <summary>  
    /// dataTable转换成Json格式  
    /// </summary>  
    /// <param name="dt"></param>  
    /// <returns></returns>  
    public static string DataTable2Json(DataTable dt)
    {
        ArrayList arrayList = new ArrayList();
        foreach (DataRow dataRow in dt.Rows)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();  //实例化一个参数集合
            foreach (DataColumn dataColumn in dt.Columns)
            {
                dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
            }
            arrayList.Add(dictionary); //ArrayList集合中添加键值
        }

        //return JsonHelper.JsonSerializer<ArrayList>(arrayList);
        return JsonHelper.SerializeObject(arrayList);
    }

    public static ArrayList Json2ArrayList(string json)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        return jss.Deserialize<ArrayList>(json);
    }

    /// <summary>
    /// Json转DataTable
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static DataTable Json2Dtb(string json)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        ArrayList dic = jss.Deserialize<ArrayList>(json);
        DataTable dtb = new DataTable();

        if (dic.Count > 0)
        {
            foreach (Dictionary<string, object> drow in dic)
            {
                if (dtb.Columns.Count == 0)
                {
                    foreach (string key in drow.Keys)
                    {
                        dtb.Columns.Add(key);
                    }
                }

                DataRow row = dtb.NewRow();
                foreach (string key in drow.Keys)
                {

                    row[key] = drow[key].ToString();
                }
                dtb.Rows.Add(row);
            }
        }
        return dtb;
    }

    public static JObject Json2DtbByLQ(string json)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        ArrayList dic = jss.Deserialize<ArrayList>(json);
        JObject dtb = new JObject();

        if (dic.Count > 0)
        {
            foreach (Dictionary<string, object> drow in dic)
            {
                
                int index = 0;
                string columnNM = "";             
                foreach (string key in drow.Keys)
                {
                    if (index == 0)
                    {
                        dtb.Add(drow[key].ToString(),"");
                        columnNM = drow[key].ToString();
                    }
                    else
                    {

                        dtb[columnNM] = drow[key].ToString();
                    }
                    index++;
                }
            }
        }

        return dtb;
    }

    /// <summary>
    /// DataTable转Json
    /// </summary>
    /// <param name="dtb"></param>
    /// <returns></returns>
    public static string Dtb2Json(DataTable dtb)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        ArrayList dic = new ArrayList();

        foreach (DataRow row in dtb.Rows)
        {
            Dictionary<string, object> drow = new Dictionary<string, object>();
            foreach (DataColumn col in dtb.Columns)
            {
                drow.Add(col.ColumnName, row[col.ColumnName]);
            }
            dic.Add(drow);
        }

        return jss.Serialize(dic);
    }

    /// <summary>
    /// datatable转JArray
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static JArray Dtb2JArray(DataTable dt)
    {
        var jarray = new JArray();

        foreach (DataRow row in dt.Rows)
        {
            var jObject = new JObject();

            foreach (DataColumn col in dt.Columns)
            {
                jObject.Add(col.ColumnName, row[col.ColumnName].ToString());
            }

            jarray.Add(jObject);
        }

        return jarray;
    }

    /// <summary>
    /// 将对象序列化为JSON格式
    /// </summary>
    /// <param name="o">对象</param>
    /// <returns>json字符串</returns>
    public static string SerializeObject(object o)
    {
        string json = JsonConvert.SerializeObject(o);
        return json;
    }

    /// <summary>
    /// 解析JSON字符串生成对象实体
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
    /// <returns>对象实体</returns>
    public static T DeserializeJsonToObject<T>(string json) where T : class
    {
        JsonSerializer serializer = new JsonSerializer();
        StringReader sr = new StringReader(json);
        object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
        T t = o as T;
        return t;
    }

    /// <summary>
    /// 解析JSON数组生成对象实体集合
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
    /// <returns>对象实体集合</returns>
    public static List<T> DeserializeJsonToList<T>(string json) where T : class
    {
        JsonSerializer serializer = new JsonSerializer();
        StringReader sr = new StringReader(json);
        object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
        List<T> list = o as List<T>;
        return list;
    }

    /// <summary>
    /// 反序列化JSON到给定的匿名对象.
    /// </summary>
    /// <typeparam name="T">匿名对象类型</typeparam>
    /// <param name="json">json字符串</param>
    /// <param name="anonymousTypeObject">匿名对象</param>
    /// <returns>匿名对象</returns>
    public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
    {
        T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
        return t;
    }

    /// <summary>
    /// 获取一个类指定的属性值
    /// </summary>
    /// <param name="info">object对象</param>
    /// <param name="field">属性名称</param>
    /// <returns></returns>
    public static object GetPropertyValue(object info, string field)
    {
        if (info == null) return null;
        Type t = info.GetType();
        IEnumerable<System.Reflection.PropertyInfo> property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
        return property.First().GetValue(info, null);
    }

    
}

//public class JObjectAccessor : DynamicObject
//{
//    JToken obj;

//    public JObjectAccessor(JToken obj)
//    {
//        this.obj = obj;
//    }

//    public override bool TryGetMember(GetMemberBinder binder, out object result)
//    {
//        result = null;

//        if (obj == null) return false;

//        var val = obj[binder.Name];

//        if (val == null) return false;

//        result = Populate(val);

//        return true;
//    }


//    private object Populate(JToken token)
//    {
//        var jval = token as JValue;
//        if (jval != null)
//        {
//            return jval.Value;
//        }
//        else if (token.Type == JTokenType.Array)
//        {
//            var objectAccessors = new List<object>();
//            foreach (var item in token as JArray)
//            {
//                objectAccessors.Add(Populate(item));
//            }
//            return objectAccessors;
//        }
//        else
//        {
//            return new JObjectAccessor(token);
//        }
//    }
//}