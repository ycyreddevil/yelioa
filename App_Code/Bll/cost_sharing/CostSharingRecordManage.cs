using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// CostSharingRecordManage 的摘要说明
/// </summary>
public class CostSharingRecordManage
{
    public CostSharingRecordManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string GetList(string userId,string year,string month)
    {
        JObject res = new JObject();
        if(string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            string errMsg = "";
            DataSet ds = CostSharingRecordSrv.GetList(userId,year,month,ref errMsg);
            if(ds==null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
                res.Add("data", TableToJArray(ds.Tables[0], ds.Tables[1], ds.Tables[2], ds.Tables[3], ds.Tables[4]));
            }
        }
        return res.ToString();
    }

    private static JArray TableToJArray(DataTable fieldDT, DataTable userDT, DataTable producrDT, DataTable hospitalDT, DataTable departmentDT)
    {
        JArray fieldList = new JArray();
        JObject firstJobject = new JObject();//新增
        JObject secondJobject = new JObject();//基本信息更新
        JObject thirdJobject = new JObject();//费用率更新
        JObject forthJobject = new JObject();//丢失

        firstJobject.Add("header", "网点备案新增");
        secondJobject.Add("header", "网点备案基本信息更新");
        thirdJobject.Add("header", "网点备案其他信息更新");
        forthJobject.Add("header", "网点备案丢失");

        firstJobject.Add("List", GetRecord(fieldDT, userDT, producrDT, hospitalDT, departmentDT, "0"));
        secondJobject.Add("List", GetRecord(fieldDT, userDT, producrDT, hospitalDT, departmentDT, "1"));
        thirdJobject.Add("List", GetRecord(fieldDT, userDT, producrDT, hospitalDT, departmentDT, "2"));
        forthJobject.Add("List", GetRecord(fieldDT, userDT, producrDT, hospitalDT, departmentDT, "3"));

        fieldList.Add(firstJobject);
        fieldList.Add(secondJobject);
        fieldList.Add(thirdJobject);
        fieldList.Add(forthJobject);

        return fieldList;
    }
    private static JArray GetRecord(DataTable dt, DataTable userDT, DataTable producrDT, DataTable hospitalDT, DataTable departmentDT,string type)
    {
        JArray fieldList = new JArray();
        foreach (DataRow row in dt.Rows)
        {
            if (row["InsertOrUpdate"].ToString() == type)
            {
                int index = JarrayContainJobject(fieldList, row["RegistrationCode"].ToString());
                if(index>-1)
                {
                    fieldList[index] = GetRecordFieldList(row["FieldName"].ToString(), row["NewValue"].ToString(), userDT, producrDT, hospitalDT, departmentDT, (JObject)fieldList[index]);
                }
                else
                {
                    JObject fieldObject = new JObject();
                    fieldObject.Add("docCode", row["RegistrationCode"].ToString());
                    fieldObject = GetRecordFieldList(row["FieldName"].ToString(), row["NewValue"].ToString(), userDT, producrDT, hospitalDT, departmentDT, fieldObject);
                    fieldList.Add(fieldObject);
                }
            }

        }
        return fieldList;
    }
    private static string IDToName(DataTable dt, string id)
    {
        foreach (DataRow row in dt.Rows)
        {
            if (row["Id"].ToString() == id)
            {
                return row["Name"].ToString();
            }
        }
        return null;
    }
    private static int JarrayContainJobject(JArray jarray, string docCode)
    {
        if(jarray==null||jarray.Count==0)
        {
            return -1;
        }
        for (int i = 0; i < jarray.Count; i++)
        {
            int n = 1;
            JObject temp = (JObject)jarray[i];
            if(temp["docCode"].ToString()==docCode)
            {
                return i;
            }
        }
        return -1;
    }
    private static JObject GetRecordFieldList(string fieldName, string Id, DataTable userDT, DataTable producrDT, DataTable hospitalDT, DataTable departmentDT, JObject temp)
    {

        if (fieldName == "代表")
        {
            temp.Add("userName", IDToName(userDT, Id));
        }
        else if (fieldName == "产品（包含规格型号）")
        {
            temp.Add("secondValue", IDToName(producrDT, Id));
        }
        else if (fieldName == "网点医院名称")
        {
            temp.Add("firstValue", IDToName(hospitalDT, Id));
        }
        else if (fieldName == "部门")
        {
            string name = IDToName(departmentDT, Id);
            temp.Add("department", name.Substring(name.LastIndexOf("/") + 1, name.Length - name.LastIndexOf("/") - 1));
        }
        return temp; 
    }
}