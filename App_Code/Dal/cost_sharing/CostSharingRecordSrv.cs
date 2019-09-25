using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// CostSharingRecordSrv 的摘要说明
/// </summary>
public class CostSharingRecordSrv
{
    public CostSharingRecordSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetList(string userId,string year,string month, ref string msg)
    {
        string sql = string.Format("SELECT a.*, b.InsertOrUpdate FROM cost_sharing_detail a LEFT JOIN cost_sharing_record b ON a.RegistrationCode = b.`Code` WHERE" +
            " b.`Code` IN( SELECT DISTINCT RegistrationCode FROM  cost_sharing_detail WHERE ApproverUserId = '100000154' ) AND ( FieldName = '代表' OR" +
            " FieldName = '产品（包含规格型号）' OR FieldName = '网点医院名称' OR FieldName = '部门') and year(b.CreateTime)='{1}' and month(b.CreateTime)='{2}' AND a.LEVEL = (SELECT MAX(LEVEL) FROM cost_sharing_detail WHERE" +
            "  RegistrationCode = a.RegistrationCode AND FieldName = a.FieldName)GROUP BY a.RegistrationCode,a.FieldName;", userId,year,month);

        sql += string.Format("select distinct userName Name,userId Id from v_user_department_post;");
        sql += string.Format("SELECT `Id`,CONCAT(`Name`,'[',Specification,'][',Unit,']') Name from jb_product;");
        sql += string.Format("SELECT `Id`,Name from fee_branch_dict;");
        sql += string.Format("SELECT Id,name Name from department;");
        return SqlHelper.Find(sql, ref msg);
    }
}