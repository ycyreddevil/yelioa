using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// SalesLineBudgetApplicationSrv 的摘要说明
/// </summary>
public class SalesLineBudgetApplicationSrv
{
    public SalesLineBudgetApplicationSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    public static DataSet GetUserDepartmentList(string userId, ref string ErrMsg)
    {

        string sql = string.Format("select * FROM v_user_department_post where userId='{0}';" +
            "SELECT DISTINCT parentId from department;", userId);
        sql += "select * from department";
        return SqlHelper.Find(sql, ref ErrMsg);
    }
    public static DataSet GetUserMsg(string userId,ref string ErrMsg)
    {

        string sql = string.Format("select isHead,departmentId FROM v_user_department_post where userId='{0}';" +
            "SELECT DISTINCT parentId from department", userId);
        return SqlHelper.Find(sql, ref ErrMsg);
    }

    public static DataSet GetNetBelongToMe(string departmentId,ref string msg)
    {
        string sql = string.Format("SELECT a.*,b.`Name` HospitalName,CONCAT(c.`Name`,'[',c.Specification,'][',c.Unit,']') ProductName from new_cost_sharing a " +
            "LEFT JOIN fee_branch_dict b on a.HospitalId=b.Id  LEFT JOIN jb_product c ON a.ProductId=c.Id where FirstDepartmentId = '{0}' or" +
            " SecondDepartmentId = '{0}' or ThirdDepartmentId = '{0}'; ", departmentId);
        sql += "SELECT a.FeeDetailId,b.`Name` from fee_detail_department a LEFT JOIN fee_detail_dict_copy b on a.FeeDetailId=b.Id WHERE SpecialItem='地区' and FeeDetailId!=2 and FeeDetailId!=9;";
        sql += "SELECT * FROM fee_detail_dict_copy where ParentName is NOT null;";
        sql += "select * from department";

        return SqlHelper.Find(sql, ref msg);
    }
}