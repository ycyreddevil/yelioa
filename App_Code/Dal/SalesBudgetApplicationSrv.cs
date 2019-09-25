using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Reflection;
using System.Web;
using Dapper;

/// <summary>
/// SalesBudgetApplicationSrv 的摘要说明
/// </summary>
public class SalesBudgetApplicationSrv
{
    public SalesBudgetApplicationSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
  
    public static DataSet GetDoc(string userId,ref string msg)
    {
        string sql = string.Format("SELECT a.*,b.`name` department FROM `yl_budget_distribute` a LEFT JOIN department b on a.DepartmentId=b.Id WHERE " +
            " DepartmentId in (SELECT DepartmentId from v_user_department_post where UserId={0} and isHead=1) and a.FirstOrSecondDistribute=0 and a.IsFinished=0;", userId);
        sql += "select distinct parentId from department";
        return SqlHelper.Find(sql, ref msg);
    }

    public static DataSet GetChildrenDepartment(string departmentId, ref string msg)
    {
        string sql = string.Format("select * from department where parentId={0}", departmentId);

        return SqlHelper.Find(sql, ref msg);
    }
 

    public static DataSet GetMaxDocNo(ref string msg)
    {
        string sql = string.Format("select ifnull(max(DocNo),0) from yl_budget_distribute where to_days(CreateTime) = to_days(now());");
        sql += string.Format("select Name from fee_detail_dict_copy");
        return SqlHelper.Find(sql, ref msg);
    }


    public static DataSet GetSecondDoc(string userId, ref string msg)
    {
        string sql = string.Format("SELECT a.*,b.`name` department FROM `yl_budget_distribute` a LEFT JOIN department b on a.DepartmentId=b.Id WHERE " +
           " DepartmentId in (SELECT DepartmentId from v_user_department_post where UserId={0} and isHead=1) ;", userId);
        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetBranch(string departmentId)
    {
        string sql = string.Format("select a.HospitalId,a.ProductId,a.SupervisorId,b.`Name` HospitalName,CONCAT(c.`Name`,'[',c.Specification,'][',c.Unit,']')  ProductName,d.userName" +
            " from new_cost_sharing a LEFT JOIN fee_branch_dict b on a.HospitalId=b.Id LEFT JOIN jb_product c on a.ProductId=c.Id LEFT JOIN users d on a.SupervisorId=d.userId " +
            " where ThirdDepartmentId={0}", departmentId);
        return SqlHelper.Find(sql);
    }
    public static DataSet GetBranchHistory(string userId,string BudgetLimitType)
    {
        DateTime lastMonthLastDay = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1); //上月最后一天
        DateTime lastDay = lastMonthLastDay.AddMonths(1);//本月最后一天

        string sql = string.Format("select a.Budget,b.HospitalId,b.ProductId,b.SupervisorId,d.`Name` HospitalName,CONCAT(e.`Name`, '[', e.Specification, '][', e.Unit, ']')  ProductName," +
            "f.userName SalesName from yl_budget_detail a LEFT JOIN yl_sales_hospital_budget_record b on a.HospitalRecordId=b.Id LEFT JOIN fee_detail_dict_copy c on a.FeeDetailId=c.Id " +
            "LEFT JOIN fee_branch_dict d on b.HospitalId=d.Id LEFT JOIN jb_product e on b.ProductId = e.Id LEFT JOIN users f on b.SupervisorId = f.userId" +
            " where a.SupervisorId='{0}' and b.DepartmentRecordDoc is NULL and c.Name='{1}' and b.CreateTime BETWEEN  '{2}-{3}-01 00:00:00' and  '{2}-{3}-{4} 23:59:59' " 
            , userId, BudgetLimitType, lastDay.Year, lastDay.Month, lastDay.Day);

        return SqlHelper.Find(sql);
    }
    public static DataSet GetSecondChildrenDepartment(string departmentId, ref string msg)
    {
        string sql = string.Format("select Id,name from department where parentId={0}", departmentId);

        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetSecondChildrenDepartment(string userId,string BudgetLimitType,ref string msg)
    {
        DateTime lastMonthLastDay = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1); //上月最后一天
        DateTime lastDay = lastMonthLastDay.AddMonths(1);//本月最后一天

        string sql = string.Format("SELECT a.DepartmentId,b.`name`,a.BudgetLimitType,a.BudgetLimit from yl_budget_distribute a " +
            "LEFT JOIN department b on a.DepartmentId=b.Id where UserId='{0}' and CreateTime between '{1}-{2}-01 00:00:00' and '" +
            "{1}-{2}-{3} 23:59:59'", userId, lastDay.Year, lastDay.Month, lastDay.Day);

        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetFeeDetail(string departmentId,string BudgetLimitType)
    {
        if(departmentId== "293"|| departmentId == "292")
        {
            departmentId = "296,297,298,299,300,301,302,290";
        }
        else if(departmentId=="291")
        {
            departmentId = "296,297,298,299,300,301,302";
        }
        string sql = string.Format("SELECT ifnull(SUM(a.Budget),0) from yl_budget_detail a LEFT JOIN fee_detail_dict_copy b on a.FeeDetailId=b.Id where" +
            " a.HospitalRecordId IN (SELECT c.Id from yl_sales_hospital_budget_record c LEFT JOIN new_cost_sharing d on c.HospitalId=d.HospitalId and c.ProductId=d.ProductId and c.SupervisorId=d.SupervisorId " +
            " where (d.ThirdDepartmentId in ({0}) OR d.SecondDepartmentId in ({0}) or d.FirstDepartmentId in ({0})) and  c.DepartmentRecordDoc is not null)" +
            " and (b.ParentName='{1}' or b.`Name`='{1}')   GROUP BY ifnull(b.ParentName,b.Name)", departmentId, BudgetLimitType);

        return SqlHelper.Find(sql);
    }
    public static DataSet GetFeeDetail(string productId,string hospitalId,string SupervisorId, string BudgetLimitType)
    {
        string sql = string.Format("SELECT sum(a.Budget) budget,ifnull(b.ParentName,b.Name) FeeDetailName from " +
            "yl_budget_detail a LEFT JOIN fee_detail_dict_copy b on a.FeeDetailId=b.Id LEFT JOIN yl_sales_hospital_budget_record c on a.HospitalRecordId=c.Id " +
            " where c.HospitalId={0} and c.ProductId={1} AND c.SupervisorId={2} AND b.ParentName='{3}' OR b.`Name`='{3}' and c.DepartmentRecordDoc is not null group by ifnull(b.ParentName,b.Name);", hospitalId,productId,SupervisorId, BudgetLimitType);
        sql += string.Format("select Id from fee_detail_dict_copy where name='{0}'", BudgetLimitType);
        return SqlHelper.Find(sql);
    }
    public static DataSet GetFeeDetail(string departmentId,ref string msg)
    {
        string sql = string.Format("select * from  fee_detail_dict_copy;");
        sql += string.Format("SELECT * FROM v_user_department_post WHERE departmentId={0} and isHead=1 order by userId;", departmentId);
        sql += string.Format("call FindParentDPTAndUser({0});", departmentId);
        return SqlHelper.Find(sql, ref msg);
    }

    public static DataSet GetUserDepartment(string userId,ref string msg)
    {
        string sql = string.Format("SELECT * from v_user_department_post where userId='{0}' and isHead=1;",userId);
        sql += string.Format("SELECT * from department");
        return SqlHelper.Find(sql, ref msg);
    }
    public static DataSet GetLeafSubmitDoc(string departmentIds)
    {
        string sql = "";

        if(departmentIds.Contains(","))//地区以上
        {
            sql = string.Format("SELECT distinct a.DepartmentId DepartmentId,a.DocNo, d. NAME DepartmentName FROM" +
                " `yl_sales_department_budget_record` a LEFT JOIN yl_sales_hospital_budget_record b on a.DocNo=b.DepartmentRecordDoc" +
                " LEFT JOIN yl_budget_detail c on b.Id=c.HospitalRecordId left join department d on a.DepartmentId=d.Id " +
                "where a.DepartmentId in ({0})", departmentIds);
        }
        else//地区
        {
            sql = string.Format("select distinct c.`name` DepartmentName,c.id DepartmentId, null DocNo from branch_available_credit a LEFT JOIN " +
                "new_cost_sharing b on a.HospitalId=b.HospitalId and a.ProductId=b.ProductId and a.SupervisorId=b.SupervisorId  LEFT JOIN " +
                "department c on b.ThirdDepartmentId=c.Id WHERE  ThirdDepartmentId={0} and a.feeDetailName!='研发费用金额' and a.feeDetailName!='工资社保金额' and a.feeDetailName!='总部管理费用'", departmentIds);
        }
        return SqlHelper.Find(sql);
    }

    public static DataSet GetDistributeDoc(string selfDepartmentId, string departmentId,string docCode,string userId, Boolean readOnly)
    {
        string sql =
        string.Format("SELECT DISTINCT a.*,c.userName SalesName,d.`Name` ProductName,e.`Name` HospitalName FROM	branch_available_credit a" +
            " LEFT JOIN new_cost_sharing b ON a.HospitalId = b.HospitalId AND a.ProductId = b.ProductId AND a.SupervisorId = b.SupervisorId " +
            "LEFT JOIN users c on b.SupervisorId=c.userId LEFT JOIN jb_product d on b.ProductId=d.Id LEFT JOIN fee_branch_dict e on b.HospitalId=e.Id " +
            "WHERE ThirdDepartmentId ={0} and a.feeDetailName!='研发费用金额' and a.feeDetailName!='工资社保金额' and a.feeDetailName!='总部管理费用' and " +
            "DATE_FORMAT(a.CreateTime, '%Y%m' ) = DATE_FORMAT( CURDATE( ) , '%Y%m' );", departmentId);

      

        sql += string.Format("select * from fee_detail_dict_copy;");

        if (!readOnly && docCode == "-1")
            sql += string.Format("SELECT a.*,c.userName SalesName,d.`Name` ProductName,e.`Name` HospitalName FROM	branch_available_credit a" +
           " LEFT JOIN new_cost_sharing b ON a.HospitalId = b.HospitalId AND a.ProductId = b.ProductId AND a.SupervisorId = b.SupervisorId " +
           "LEFT JOIN users c on b.SupervisorId=c.userId LEFT JOIN jb_product d on b.ProductId=d.Id LEFT JOIN fee_branch_dict e on b.HospitalId=e.Id " +
           "WHERE ThirdDepartmentId ={0} and a.feeDetailName!='研发费用金额' and a.feeDetailName!='工资社保金额' and a.feeDetailName!='总部管理费用' and " +
           "DATE_FORMAT(a.CreateTime, '%Y%m' ) = DATE_FORMAT( CURDATE( ) , '%Y%m' );", departmentId);

        else
        {
            if (readOnly)
            {
                sql += string.Format("SELECT DISTINCT a.*,b.* from yl_budget_detail a LEFT JOIN yl_sales_hospital_budget_record b " +
                                 "on a.HospitalRecordId=b.Id left join budget_submit_record c on b.hospitalId = c.hospitalId " +
                                 "and b.productId = c.productId and b.SupervisorId = c.SupervisorId and a.feeDetailId = c.feeDetailId " +
                                 "where b.DepartmentRecordDoc={0} and c.approverUserId = {1} and c.ApproverDepartmentId={2};", docCode, userId, selfDepartmentId);
            }
            sql += string.Format("SELECT DISTINCT a.*,b.* from yl_budget_detail a LEFT JOIN yl_sales_hospital_budget_record b " +
                                 "on a.HospitalRecordId=b.Id left join budget_submit_record c on b.hospitalId = c.hospitalId " +
                                 "and b.productId = c.productId and b.SupervisorId = c.SupervisorId and a.feeDetailId = c.feeDetailId " +
                                 "where b.DepartmentRecordDoc={0} and a.approverUserId = {1} and a.ApprovalDepartmentId={2};", docCode, userId, selfDepartmentId);

        }
        //if (!readOnly)
        //    sql += string.Format("SELECT a.*,b.* from yl_budget_detail a LEFT JOIN yl_sales_hospital_budget_record b on a.HospitalRecordId=b.Id where b.DepartmentRecordDoc={0} and a.ApproverUserId={1} and a.ApprovalDepartmentId={2}", docCode,userId,selfDepartmentId);
        //else
        //    sql += string.Format("SELECT a.*,b.* from yl_budget_detail a LEFT JOIN yl_sales_hospital_budget_record b " +
        //                         "on a.HospitalRecordId=b.Id left join budget_submit_record c on b.hospitalId = c.hospitalId " +
        //                         "and b.productId = c.productId and b.SupervisorId = c.SupervisorId and a.feeDetailId = c.feeDetailId " +
        //                         "where b.DepartmentRecordDoc={0} and c.approverUserId = {1} and c.ApproverDepartmentId={2}", docCode, userId,selfDepartmentId);

        return SqlHelper.Find(sql);
    }

    public static DataSet getRelatedBranch(string userId, ref string errorMsg)
    {
        string sql = string.Format("select distinct t2.name hospitalName, t2.id hospitalId from new_cost_sharing t1 left join fee_branch_dict t2 on t1.hospitalId = t2.id" +
                    " where SupervisorId = '{0}';", userId);

        sql += string.Format("select * from v_user_department_post where userId = '{0}'", userId);

        return SqlHelper.Find(sql, ref errorMsg);
    }

    public static DataSet getRelatedBranchProduct(string userId, string hospitalId ,ref string errorMsg)
    {
        string sql = string.Format("select distinct t3.name productName, t3.id productId from new_cost_sharing t1 " +
                        " left join jb_product t3 on t1.productId = t3.id where SupervisorId = '{0}' and hospitalId = '{1}';", userId, hospitalId);

        return SqlHelper.Find(sql, ref errorMsg);
    }

    public static DataSet determineInsertOrUpdateRecord(string hospitalId, string productId, string SupervisorId, ref string errorMsg, UserInfo userInfo)
    {
        string sql = string.Format("select t1.docCode,t1.approverUserId from expect_flow_submit t1 left join expect_flow_submit_record t2 on " +
                                   "t1.docCode= t2.docCode where t1.hospitalId = '{0}' and t1.productId = '{1}' and " +
                                   "t1.SupervisorId = '{2}' and t2.modifiedUserId = '{3}';", hospitalId, productId, SupervisorId,userInfo.userId);
        sql += string.Format("select thirdDepartmentid from new_cost_sharing where hospitalId = '{0}' and productId = '{1}' " +
                             "and SupervisorId = '{2}'", hospitalId, productId, SupervisorId);
        return SqlHelper.Find(sql, ref errorMsg); ;
    }

    public static DataSet determineInsertOrUpdateRecord(string hospitalId, string productId, string SupervisorId, ref string errorMsg)
    {
        string sql = string.Format("select t1.docCode,t1.approverUserId from expect_flow_submit t1 where t1.hospitalId = '{0}' " +
                                   "and t1.productId = '{1}' and t1.SupervisorId = '{2}'", hospitalId, productId, SupervisorId);
        return SqlHelper.Find(sql, ref errorMsg); ;
    }

    public static DataSet checkNumOfRecord(string approverUserIds, string findDocCode)
    {
        string sql = string.Format("select 1 from expect_flow_submit_record where ModifiedUserId in ({0}) and docCode = '{1}'", approverUserIds, findDocCode);
        return SqlHelper.Find(sql);
    }

    public static DataSet IsLeafDepartmentWhenSubmit(string departmentId,ref string msg)
    {
        string sql = string.Format("select DocNo from yl_sales_department_budget_record where DepartmentId={0}", departmentId);
        return SqlHelper.Find(sql, ref msg);
    }

    public static DataSet GetSubmitMaxDocNo(string departmentId,ref string msg)
    {
        string sql = string.Format("select ifnull(max(DocNo),0) from yl_sales_department_budget_record where to_days(CreateTime) = to_days(now());");
        sql += string.Format("select ifnull(max(Id),0) from yl_sales_hospital_budget_record;");
        sql += string.Format("select * from fee_detail_dict_copy;");
        sql += string.Format("call FindParentDPTAndUser({0});", departmentId);
        return SqlHelper.Find(sql, ref msg);
    }

    public static DataSet GetParnetDepartmentHeader(int departmentId, ref string msg)
    {
        string sql = string.Format("SELECT * FROM v_user_department_post WHERE departmentId=(SELECT parentId from department where Id={0}) and isHead=1", departmentId);
        return SqlHelper.Find(sql, ref msg);
    }

    public static DataSet GetSelfDepartmentHeader(int departmentId, ref string msg)
    {
        string sql = string.Format("SELECT * FROM v_user_department_post WHERE departmentId= {0} and isHead=1", departmentId);
        return SqlHelper.Find(sql, ref msg);
    }

    public static DataSet getApprovalListData(string userId, ref string errorMsg)
    {
        string sql = string.Format("select * from (select t1.*,t1.ApproverUserId submitterId, t3.username submitterName, t2.modifiedUserId lastApproverId, " +
                                   "t4.username lastApproverName, t2.CreateTime modifytime, t4.avatar from expect_flow_submit t1 left join expect_flow_submit_record t2 on t1.docCode = t2.docCode " +
                                   "left join users t3 on t1.SupervisorId = t3.userId left join users t4 on t2.modifiedUserId = t4.userid) t0 " +
                                   "where find_in_set('{0}', t0.approverUserId) and t0.isFinished = 0 and not exists ( select 1 from ( " +
                                   "select t1.*,t1.ApproverUserId submitterId, t3.username submitterName, t2.modifiedUserId lastApproverId, t4.username lastApproverName, " +
                                   "t2.CreateTime modifytime, t4.avatar from expect_flow_submit t1 left join expect_flow_submit_record t2 on t1.docCode = t2.docCode left join users t3 on " +
                                   "t1.SupervisorId = t3.userId left join users t4 on t2.modifiedUserId = t4.userid) t00 where t00.DocCode = t0.DocCode and " +
                                   "t00.modifytime > t0.modifytime) order by t0.createtime desc", userId);
        return SqlHelper.Find(sql, ref errorMsg);
    }

    public static DataSet getApprovalDetailData(string docCode, ref string errorMsg)
    {
        string sql = string.Format("select t1.*,t2.name productName, t3.name hospitalName from expect_flow_submit t1 left join jb_product t2 on t1.productId = t2.id " +
                                   "left join fee_branch_dict t3 on t1.hospitalId = t3.id where docCode = '{0}'", docCode);
        sql += ";" + string.Format("select t1.*,t2.userName from expect_flow_submit_record t1 left join users t2 on t1.modifiedUserId = t2.userId where docCode = '{0}' order by createtime", docCode);
        return SqlHelper.Find(sql, ref errorMsg);
    }

    public static DataSet getRecordDetailData(string docCode, ref string errorMsg, string userId)
    {
        string sql = string.Format("select t1.*,t2.name productName, t3.name hospitalName,t4.* from expect_flow_submit t1 left join jb_product t2 on t1.productId = t2.id " +
                                   "left join fee_branch_dict t3 on t1.hospitalId = t3.id left join expect_flow_submit_record t4 on t1.docCode = t4.docCode where t4.modifiedUserId = '{0}' and t1.docCode = '{1}'", userId, docCode);
        return SqlHelper.Find(sql, ref errorMsg);
    }

    public static DataSet GetExpectedFlowThisMonth(string productId,string hospitalId,string SupervisorId,DateTime date)
    {
        DateTime  lastMonthLastDay = date.AddDays(1 - date.Day).AddDays(-1); //上月最后一天
        DateTime lastDay = lastMonthLastDay.AddMonths(1);//本月最后一天
        //该网点本月预计流向--0
        string sql = string.Format("SELECT ifnull(expectFlow, 0) FROM `expect_flow_submit` where ProductId={0} and HospitalId={1} and SupervisorId={2} AND " +
            "CreateTime BETWEEN '{3}-{4}-01' AND '{3}-{4}-28';", productId, hospitalId, SupervisorId, lastMonthLastDay.Year, lastMonthLastDay.Month );
        //该网点费用率--1
        sql += string.Format("SELECT MAX(CreateTime),a.* FROM new_cost_sharing a where ProductId={0} and HospitalId={1} and SupervisorId={2};", productId, hospitalId, SupervisorId);
        //本月报销--销售代表--2
        sql += string.Format("SELECT ifnull(sum(a.fee_amount), 0) feeAmount,a.fee_detail from yl_reimburse a LEFT JOIN users b on a.`name`=b.userName LEFT JOIN jb_product c on a.product=c.`Name` LEFT JOIN fee_branch_dict d on a.branch=d.`Name` " +
            "where b.userId={0} AND c.Id={1} AND d.Id={2} AND a.approval_time BETWEEN '{3}-{4}-01 00:00:00' AND '{3}-{4}-{5} 23:59:59' AND a.apply_time BETWEEN '{3}-{4}-01 00:00:00' AND '{3}-{4}-{5} 23:59:59'" +
            " group by a.fee_detail;", SupervisorId, productId, hospitalId, lastDay.Year, lastDay.Month,lastDay.Day);
        //费用明细--3
        sql += string.Format("select * from fee_detail_dict_copy where ParentName is null;");
        //所有网点费用率表--4
        sql += string.Format("SELECT * FROM new_cost_sharing where createtime BETWEEN '{0}-{1}-01 00:00:00' AND '{0}-{1}-{2} 23:59:59';", lastDay.Year, lastDay.Month, lastDay.Day);
        //所有网点本月预计流向--5
        sql += string.Format("SELECT * FROM `expect_flow_submit` where  CreateTime BETWEEN '{0}-{1}-01' AND '{0}-{1}-28';", lastMonthLastDay.Year, lastMonthLastDay.Month );
      
        return SqlHelper.Find(sql);
    }

    public static DataSet GetMobileReimbursementSynthesis(List<string> departments)
    {
        DateTime lastMonthLastDay = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1); //上月最后一天
        DateTime lastDay = lastMonthLastDay.AddMonths(1);//本月最后一天
        string sql = string.Format("SELECT ifnull(sum(a.fee_amount), 0) feeAmount,a.fee_detail,b.Id departmentId from yl_reimburse a left join department b on a.fee_department=b.name" +
           " where  a.branch='综合' and a.branch='综合' and a.approval_time BETWEEN '{0}-{1}-01 00:00:00' AND '{0}-{1}-{2} 23:59:59' AND a.apply_time BETWEEN '{0}-{1}-01 00:00:00' AND '{0}-{1}-{2} 23:59:59'" +
           " and a.fee_department IN (SELECT `name` from department where id in ({3})) group by a.fee_detail,a.fee_department", lastDay.Year, lastDay.Month, lastDay.Day, string.Join(",", departments.ToArray()));
        return SqlHelper.Find(sql);
    }

    public static DataSet getExpectFlowRecord(string userId, ref string errorMsg)
    {
        string sql = string.Format("select t1.*,t2.SupervisorId, t4.userName salesName, t4.avatar from expect_flow_submit_record t1 inner join expect_flow_submit t2 on t1.docCode = t2.docCode " +
                                   "inner join users t3 on t1.modifiedUserId = t3.userId inner join users t4 on t2.SupervisorId = t4.userId where t1.modifiedUserId = '{0}'", userId);
        return SqlHelper.Find(sql, ref errorMsg);
    }

    public static DataSet InsertTheFirstToDistribute(string departmentDocNo,string feeDetail)
    {
        DateTime date = DateTime.Now;
        string sql = string.Format("SELECT SUM(a.Budget) Budget,IFNULL(c.ParentName, c.`Name`) feeDetail FROM yl_budget_detail a LEFT JOIN yl_sales_hospital_budget_record b ON a.HospitalRecordId = b.Id" +
            " LEFT JOIN fee_detail_dict_copy c ON a.FeeDetailId = c.Id WHERE b.DepartmentRecordDoc = {0} and a.ApproverUserId=0 AND " +
            "(c.ParentName IN({1}) OR c.`Name` in ({1}))  GROUP BY IFNULL(c.ParentName, c.`Name`);", departmentDocNo,feeDetail);

        sql += string.Format("SELECT * FROM yl_budget_distribute where BudgetLimitType IN (SELECT DISTINCT IFNULL(c.ParentName, c.`Name`) feeDetail " +
            "FROM yl_budget_detail a LEFT JOIN yl_sales_hospital_budget_record b ON a.HospitalRecordId = b.Id LEFT JOIN fee_detail_dict_copy c ON a.FeeDetailId = c.Id" +
            " WHERE b.DepartmentRecordDoc = '{0}' and a.ApproverUserId is NULL) and CreateTime BETWEEN '{1}-{2}-26 00:00:00' and '{1}-{2}-27 23:59:59';", departmentDocNo, date.Year, date.Month);

        sql+=string.Format("select ifnull(max(DocNo),0) from yl_budget_distribute where to_days(CreateTime) = to_days(now());");

        sql += string.Format("select * from yl_sales_department_budget_record where DocNo='{0}'", departmentDocNo);
        return SqlHelper.Find(sql);
    }

    public static Boolean isSubmitDuplicate(string selfDepartmentId, string userId)
    {
        // 判断是否重复提交
        string sql = string.Format("select 1 from budget_submit_record where approverUserId = '{0}' " +
                                   "and approverDepartmentId in ({1})", userId, selfDepartmentId);

        DataSet ds = SqlHelper.Find(sql);

        if (ds.Tables[0].Rows.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static DataSet getDocSubmitRecord(string docNo)
    {
        string sql = string.Format("select distinct t1.approverUserId,t2.userName, t1.createTime from budget_submit_record t1 " +
                                   "left join users t2 on t1.approverUserId = t2.userId where departmentDocNo = '{0}'", docNo);
        return SqlHelper.Find(sql);
    }

    public static DataSet findDistrictDocNo(string departmentId, string userId, ref string errMsg)
    {
        string sql = string.Format("select docNo from yl_sales_department_budget_record where departmentId = '{0}' " +
                                   "and submitterId = '{1}'", departmentId, userId);
        return SqlHelper.Find(sql, ref errMsg);
    }
}