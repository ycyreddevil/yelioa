using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// NewBranchRegistrationSrv 的摘要说明
/// </summary>
public class NewBranchRegistrationSrv
{
    public NewBranchRegistrationSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet IsRegionalManager(string userId,ref string msg)
    {
        string sql = string.Format("SELECT * from v_user_department_post where isHead=1 and userId='{0}' AND" +
            " departmentId not in (SELECT DISTINCT parentId from department);", userId);
        sql += string.Format("select * from cost_sharing_field_level where FieldLevel=0;");

        sql += string.Format("select distinct userName Name,userId Id from v_user_department_post;");
        sql += string.Format("SELECT `Id`,CONCAT(`Name`,'[',Specification,'][',Unit,']') Name from jb_product;");
        sql += string.Format("SELECT `Id`,Name from fee_branch_dict;");
        sql += string.Format("SELECT * from department;");

        sql += string.Format("SELECT departmentId from v_user_department_post where userId='{0}'", userId);
        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetApprovalRecordAndApprovalFields(string docCode,ref string msg)
    {
        string sql = string.Format("select distinct b.userName,GROUP_CONCAT(DISTINCT b.department)  departmentName from  cost_sharing_detail a " +
            "left join v_user_department_post b on a.ApproverUserId=b.userId where RegistrationCode='{0}' GROUP BY b.userId;", docCode);

        sql += string.Format("select * from cost_sharing_field_level where FieldLevel=(select Level from cost_sharing_record where Code='{0}');", docCode);
        
        sql += string.Format("select distinct userName Name,userId Id from v_user_department_post;");
        sql += string.Format("SELECT `Id`,CONCAT(`Name`,'[',Specification,'][',Unit,']') Name from jb_product;");
        sql += string.Format("SELECT `Id`,Name from fee_branch_dict;");
        sql += string.Format("SELECT * from department;");

        sql += string.Format("SELECT a.departmentId FROM v_user_department_post a LEFT JOIN cost_sharing_record  b on a.userId=b.SubmitterUserId where b.`Code`='{0}';", docCode);
        sql += string.Format("SELECT * from cost_sharing_detail where RegistrationCode='{0}' GROUP BY FieldName ORDER BY `Level` DESC ", docCode);

        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetApprovalRecord(string docCode,string userId, ref string msg)
    {
        string sql = string.Format("select distinct b.userName,GROUP_CONCAT(b.department)  departmentName from  cost_sharing_detail a " +
           "left join v_user_department_post b on a.ApproverUserId=b.userId where RegistrationCode='{0}' GROUP BY b.userId", docCode);

        sql += string.Format("select a.*,b.* from cost_sharing_detail a left join cost_sharing_field_level b on a.FieldName=b.FieldName  and a.Level=b.Level" +
            "where a.ApproverUserId='{0}' and a.RegistrationCode='{1}'", userId,docCode);

        sql += string.Format("select distinct userName Name,userId Id from v_user_department_post;");
        sql += string.Format("SELECT `Id`,CONCAT(`Name`,'[',Specification,'][',Unit,']') Name from jb_product;");
        sql += string.Format("SELECT `Id`,Name from fee_branch_dict;");
        sql += string.Format("SELECT Id,name Name from department;");
        return SqlHelper.Find(sql, ref msg);
    }

    public static DataSet GetMaxDocCode(ref string msg)
    {
        string sql = string.Format("select ifnull(max(Code),0) from cost_sharing_record  where to_days(CreateTime) = to_days(now()); ");

        sql += string.Format("select * from v_user_department_post where isHead=1");
        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetDoc(string docCode,ref string msg)
    {
        string sql = string.Format("select * from cost_sharing_detail  where RegistrationCode='{0}' and FieldName='部门' order by CreateTime desc limit 1; ",docCode);

        sql += string.Format("select * from v_user_department_post where isHead=1;");
        sql += string.Format("select * from cost_sharing_record where Code='{0}'", docCode);
        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetDetail(string docCode)
    {
        string sql = string.Format("SELECT a.*, b.RelativeFieldName FROM cost_sharing_detail a LEFT JOIN cost_sharing_field_level b on a.FieldName = b.FieldName WHERE" +
            " a.RegistrationCode = '20181017000001' AND a.`Level` = (SELECT MAX(LEVEL) FROM cost_sharing_detail WHERE RegistrationCode = a.RegistrationCode AND " +
            "FieldName = a.FieldName)GROUP BY a.RegistrationCode,a.FieldName; ", docCode);
        return SqlHelper.Find(sql);
    }
}