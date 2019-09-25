using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// CostSharingDeleteSrc 的摘要说明
/// </summary>
public class CostSharingDeleteSrv
{
    public CostSharingDeleteSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    public static DataSet GetList(string docId,ref string msg)
    {
        string sql = string.Format("select * from new_cost_sharing where Id='{0}';", docId);
        sql += string.Format("select * from cost_sharing_field_level where FieldLevel=0 and InsertOrUpdate=3");
        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetDoc(string docCode, ref string msg)
    {
        string sql = string.Format("select * from cost_sharing_record where Code='{0}';", docCode);
        sql += string.Format("SELECT a.*,b.FieldType FROM cost_sharing_detail a LEFT JOIN cost_sharing_field_level b on a.FieldName=b.FieldName" +
            " and a.`Level`=b.FieldLevel where a.RegistrationCode='{0}' and b.InsertOrUpdate=3  and a.`Level`=(SELECT `Level` from cost_sharing_record where CODE='{0}')-1;", docCode);
        sql += string.Format("SELECT DISTINCT b.userName,GROUP_CONCAT(DISTINCT b.department)  departmentName,b.avatar FROM `cost_sharing_detail` a LEFT JOIN v_user_department_post b " +
           "on a.ApproverUserId=b.userId where a.RegistrationCode='{0}' group by b.userId order by a.Level ;", docCode);
        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetRecord(string docCode, string userId,ref string msg)
    {
        string sql =  string.Format("SELECT a.*,b.FieldType FROM cost_sharing_detail a LEFT JOIN cost_sharing_field_level b on a.FieldName=b.FieldName" +
            " and a.`Level`=b.FieldLevel where a.RegistrationCode='{0}' and b.InsertOrUpdate=3 and and a.ApproverUserId='{1}'", docCode,userId);
        sql += string.Format("SELECT DISTINCT b.userName,b.department,b.avatar FROM `cost_sharing_detail` a LEFT JOIN v_user_department_post b " +
            "on a.ApproverUserId=b.userId where a.RegistrationCode='{0}';", docCode);
        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetMaxDocCode(ref string msg)
    {
        string sql = string.Format("select ifnull(max(Code),0) from cost_sharing_record  where to_days(CreateTime) = to_days(now())");

        
        return SqlHelper.Find(sql, ref msg);
    }
}