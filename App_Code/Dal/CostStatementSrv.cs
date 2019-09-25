using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// CostStatementSrv 的摘要说明
/// </summary>
public class CostStatementSrv
{

    public CostStatementSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet TheCostOfObtainingTheSubsidiaryExpenses(string departmentId,string feeDetail,ref string msg)
    {
        DateTime now = DateTime.Now;
        string lastLastMonth = (now.Month - 2).ToString();
        string lastLastYear = now.Year.ToString();
        string lastMonth = (now.Month - 1).ToString();
        string lastYear = now.Year.ToString();
        if (now.Month == 1)
        {
            lastLastMonth = "11";
            lastLastYear = (now.Year - 1).ToString();
            lastMonth = "12";
            lastYear = (now.Year - 1).ToString();
        }
        if (now.Month == 2)
        {
            lastLastMonth = "12";
            lastLastYear = (now.Year - 1).ToString();
        }
        string name = "";
        DataSet ds = GetdepartmentList();
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            if (row["Id"].ToString() == departmentId)
            {
                name = row["name"].ToString();
                break;
            }
        }
        string sql = string.Format("SELECT fee_detail, IFNULL(SUM(fee_amount),0) fee FROM  yl_reimburse WHERE fee_department like '{0}%' and fee_detail like '{3}%'" +
            " AND a.CreateTime '{1}-{2}-01 00:00:00' AND '{1}-{2}-25 23:59:59'GROUP BY fee_detail order by fee desc", name, lastLastYear, lastLastMonth, lastYear, lastMonth,feeDetail);
        return SqlHelper.Find(sql, ref msg);
    }

    private static DataSet GetdepartmentList()
    {
        string sql = "select * from department";
        return SqlHelper.Find(sql);
    }

    public static DataSet GetPieChart(string departmentId,ref string msg,ref List<string> departmentList)
    {
        DateTime now = DateTime.Now;
        string lastLastMonth = (now.Month - 2).ToString();
        string lastLastYear = now.Year.ToString();
        string lastMonth = (now.Month - 1).ToString();
        string lastYear = now.Year.ToString();
        if (now.Month == 1)
        {
            lastLastMonth = "11";
            lastLastYear = (now.Year - 1).ToString();
            lastMonth = "12";
            lastYear = (now.Year - 1).ToString();
        }
        if (now.Month == 2)
        {
            lastLastMonth = "12";
            lastLastYear = (now.Year - 1).ToString();
        }
        DataSet ds = GetdepartmentList();
        List<string> sqlList = new List<string>();
        string name = "";
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            if (row["Id"].ToString() == departmentId)
            {
                name = row["name"].ToString();
                break;
            }
        }
        //子部门的预算表
        string sql = string.Format("SELECT a.DepartmentId,b.`name`,IFNULL(SUM(a.Budget),0) fee from yl_budget a LEFT JOIN department b on a.DepartmentId=b.Id " +
            "where a.DepartmentId in (SELECT Id from department where parentId={0}) AND a.CreateTime '{1}-{2}-01 00:00:00' and '{1}-{2}-25 23:59:59' group by a.DepartmentId"
            , departmentId, now.Year, now.Month);//由于上月预算表是在本月初由CostSharingHelper类生成，故时间取为本月时间
        sqlList.Add(sql);
        //部门上月各父费用明细的费用
        sql = string.Format("SELECT b.ParentName,IFNULL(SUM(a.fee_amount),0) fee FROM yl_reimburse a LEFT JOIN fee_detail_dict_copy b ON a.fee_detail = b.`Name` WHERE " +
            "a.fee_department = ( SELECT NAME FROM department WHERE Id ={0}) AND a.CreateTime '{1}-{2}-01 00:00:00' AND '{1}-{2}-25 23:59:59' GROUP BY b.ParentName order by fee desc"
            , departmentId, lastLastYear, lastLastMonth, lastYear, lastMonth);
        sqlList.Add(sql);
        //获取new_cost_sharing表单
        sql = string.Format("SELECT *from new_cost_sharing where CreateTime BETWEEN '{0}-{1}-01 00:00:00' AND '{0}-{1}-25 23:59:59'" +
            " and (FirstDepartmentId={2} or SecondDepartmentId={2} OR ThirdDepartmentId={2})", now.Year, now.Month,departmentId);
        sqlList.Add(sql);
        //部门上月各子费用明细的费用
        sql = string.Format("SELECT fee_detail, IFNULL(SUM(fee_amount),0) fee FROM  yl_reimburse WHERE fee_department like '{0}%'" +
            " AND a.CreateTime '{1}-{2}-01 00:00:00' AND '{1}-{2}-25 23:59:59'GROUP BY fee_detail order by fee desc", name, lastLastYear, lastLastMonth, lastYear, lastMonth);
        sqlList.Add(sql);
        sql = "";
        foreach (DataRow row in ds.Tables [0].Rows)
        {
            if(row["ParentId"].ToString()==departmentId)
            {
                //子部门上月的移动报销表
                sql = string.Format("SELECT IFNULL(SUM(a.fee_amount),0) fee from yl_reimburse  where " +
                    "fee_department like '{0}%' AND a.CreateTime '{1}-{2}-01 00:00:00' and '{3}-{4}-25 23:59:59'  order by fee desc"
                    , row["name"].ToString(), lastLastYear, lastLastMonth, lastYear, lastMonth);
                departmentList.Add(row["name"].ToString());
                sqlList.Add(sql);
            }
        }
        if(sql=="")//如果无子部门，则查询出部门移动报销
        {
            sql = string.Format("SELECT IFNULL(SUM(a.fee_amount),0) fee from yl_reimburse  where " +
                    "fee_department like '{0}%' AND a.CreateTime '{1}-{2}-01 00:00:00' and '{3}-{4}-25 23:59:59'  order by fee desc"
                    , name, lastLastYear, lastLastMonth, lastYear, lastMonth);
            departmentList.Add(name);
            sqlList.Add(sql);
        }
       
      

        return SqlHelper.Find(sqlList.ToArray(),ref msg);
    }
}