using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Newtonsoft.Json.Linq;

/// <summary>
/// MobileReimburseSrv 的摘要说明
/// </summary>
public class MobileReimburseSrv
{
    public MobileReimburseSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet findParentIdById(int departmentId)
    {
        string sql = string.Format("select parentId from department where id = {0}", departmentId);
        return SqlHelper.Find(sql) ;
    }

    public static string insertApprovalProcess(string id, List<JObject> list, string type)
    {
        List<string> sqls = new List<string>();

        string deleteProcessSql = "delete from approval_process where DocumentTableName = '"+type+"' and DocCode = '" + id + "'";
        string deleteApproverSql = "delete from approval_approver where DocumentTableName = '" + type + "' and DocCode = '" + id + "'";

        sqls.Add(deleteProcessSql);
        sqls.Add(deleteApproverSql);

        foreach (JObject jObject in list) {
            string sql = string.Format("insert into approval_process (DocumentTableName,DocCode,Level,ApproverId) " +
            "values ('{3}','{2}',{0},'{1}')", jObject["index"], jObject["userId"], id, type);
            sqls.Add(sql);
        }
        return SqlHelper.Exce(sqls.ToArray());
    }

    public static DataSet findDepartmentByWechatUserId(string wechatUserId)
    {
        string sql = string.Format("select departmentId from user_department_post where wechatUserId = '{0}'", wechatUserId);
        return SqlHelper.Find(sql);
    }

    public static DataSet findBranch(string name, List<DepartmentPost> departmentList, string departmentName)
    {
        Boolean isLeader = false;

        if (departmentList != null && departmentName != "")
        {
            foreach (DepartmentPost departmentPost in departmentList)
            {
                int departmentId = departmentPost.departmentId;
                string findDepartmentName = SqlHelper.Find("select name from department where id = " + departmentId).Tables[0].Rows[0][0].ToString();

                if (findDepartmentName.Equals(departmentName) && departmentPost.isHead == 1 && departmentName.Contains("集团营销中心"))
                {
                    isLeader = true;
                    break;
                }
            }
        }

        string sql = "";
        if (!isLeader)
        {
            sql = string.Format("select name from fee_branch_dict where name like '%{0}%' order by Id", name);
        }
        else
        {
            sql = string.Format("select '综合' name from fee_branch_dict limit 1");
        }

        return SqlHelper.Find(sql);
    }
    public static DataSet findBranch()
    {
        string sql = string.Format("SELECT name from fee_branch_dict");
        return SqlHelper.Find(sql);
    }

    public static DataSet findChildrenFeeDetail(string name,string feeDepartment)
    {
        int nextMonth, nextYear;
        DateTime now = DateTime.Now;
        if (now.Month == 12)
        {
            nextMonth = 1;
            nextYear = now.Year + 1;
        }
        else
        {
            nextMonth = now.Month + 1;
            nextYear = now.Year;
        }
        string sql = string.Format("select FindParentDepartment((select Id from department where name='{0}'));", feeDepartment);
        DataSet ds = SqlHelper.Find(sql);
        sql = string.Format("select a.Id,a.FeeDetail from import_budget a where a.ParentId=(select Id from import_budget where FeeDetail='{0}' and " +
            "CreateTime between '{1}-{2}-1 00:00:00 ' and '{3}-{4}-1 00:00:00' and DepartmentId in ({5})) ", 
            name, now.Year, now.Month, nextYear, nextMonth, ds.Tables[0].Rows[0][0].ToString());
        return SqlHelper.Find(sql);
    }

    public static DataSet findParentFeeDetail(string department)
    {
        int nextMonth, nextYear;
        DateTime now = DateTime.Now;
        if (now.Month == 12)
        {
            nextMonth = 1;
            nextYear = now.Year + 1;
        }
        else
        {
            nextMonth = now.Month + 1;
            nextYear = now.Year;
        }

        string sql = string.Format("select FindParentDepartment((select Id from department where name='{0}'));", department);
        DataSet ds = SqlHelper.Find(sql);
        sql = string.Format("SELECT FeeDetail  from  import_budget   where DepartmentId in({0})  and CreateTime between '{1}-{2}-1 00:00:00 ' and '{3}-{4}-1 00:00:00'" +
            " and ParentId=-1", ds.Tables[0].Rows[0][0].ToString(), now.Year, now.Month, nextYear, nextMonth);
        //if (!"".Equals(department))
        //{
        //    sql = string.Format("select t3.name from fee_detail_department t1 left join department t2 on t1.departmentId = t2.id " +
        //                               "left join fee_detail_dict_copy t3 on t1.feeDetailId = t3.id " +
        //                               "where t2.name = '{0}'", department);

        //    DataTable dt = SqlHelper.Find(sql).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        return SqlHelper.Find(sql);
        //    }
        //    else
        //    {
        //        if (department.Contains("营销中心"))
        //        {
        //            sql = string.Format("select t2.name from fee_detail_department t1 left join fee_detail_dict_copy t2 on t1.feeDetailId = t2.id where specialItem = '营销中心'");
        //        }
        //        else if (department.Contains("制造中心"))
        //        {
        //            sql = string.Format("select t2.name from fee_detail_department t1 left join fee_detail_dict_copy t2 on t1.feeDetailId = t2.id where specialItem = '制造中心'");
        //        }
        //    }
        //}
        //else
        //{
        //    sql = string.Format("select name from fee_detail_dict_copy where parentName is null");
        //}

        return SqlHelper.Find(sql);
    }
    public static DataSet findFeeDepartment(string name)
    {
        string sql = string.Format("SELECT name from department where name like '%{0}%'", name);
        return SqlHelper.Find(sql);
    }
    public static DataSet findFeeDepartment()
    {
        string sql = string.Format("SELECT name from department");
        return SqlHelper.Find(sql);
    }

    public static DataSet findDepartmentIdByName(string name)
    {
        string sql = string.Format("select id from department where name = '{0}'", name);
        return SqlHelper.Find(sql);
    }
    public static DataSet findInformer()
    {
        string sql = string.Format("SELECT userId, userName from users");
        return SqlHelper.Find(sql);
    }
    public static DataSet findInformer(string name)
    {
        string sql = string.Format("SELECT distinct userId, userName from users where userName like '%{0}%'",name);
        return SqlHelper.Find(sql);
    }
    
    /// <summary>
    /// 获取职能线预算
    /// </summary>
    /// <param name="feeDetail">费用明细</param>
    /// <returns></returns>
    public static string  AccessToTheBudgetOfTheFunctionLine(string feeDetail,string project)
    {
        DateTime now = DateTime.Now;
        string sql = string.Format("SELECT IFNULL(SUM(`预算`),0) FROM `wf_form_职能线预算审批表` where `费用明细`=(SELECT Id FROM fee_detail_dict_copy WHERE name='{0}')" +
            " AND CreateTime BETWEEN '{1}-{2}-26 00:00:00' and '{1}-{3}-25 23:59:59'", feeDetail,now.Year, now.Month - 1, now.Month);
        if(!string.IsNullOrEmpty(project))
        {
            sql = string.Format("SELECT IFNULL((交通费+差旅费+其他+外包费+实验费+固定资产+水电费+福利费+通讯费+折旧+会议费+咨询费+注册费+办公费+培训费+招待费),0) FROM `wf_form_研发部预算申请表` where `项目`=(SELECT Id FROM yl_project WHERE code='{0}')" +
                                " AND CreateTime BETWEEN '{1}-{2}-26 00:00:00' and '{1}-{3}-25 23:59:59' ", project, now.Year, now.Month - 1, now.Month);
            
        }
        return sql;
    }
    /// <summary>
    /// 获取职能线预算外费用申请金额
    /// </summary>
    /// <param name="feeDetail">费用明细</param>
    /// <returns></returns>
    public static string AccessToTheExtraBudgetaryCostOfTheFunctionalLine(string feeDetail,string project)
    {
        DateTime now = DateTime.Now;
        string sql = string.Format("SELECT IFNULL(SUM(`金额`),0) FROM `wf_form_职能线预算外费用申请表` where `费用明细`=(SELECT Id FROM fee_detail_dict_copy WHERE name='{0}')" +
            " AND CreateTime BETWEEN '{1}-{2}-26 00:00:00' and '{1}-{3}-31 23:59:59'", feeDetail, now.Year, now.Month - 1, now.Month);
        if (!string.IsNullOrEmpty(project))
        {
            sql = string.Format("SELECT IFNULL((交通费+差旅费+其他+外包费+实验费+固定资产+水电费+福利费+通讯费+折旧+会议费+咨询费+注册费+办公费+培训费+招待费),0) FROM `wf_form_研发部预算外申请表` where `项目`=(SELECT Id FROM yl_project WHERE code='{0}')" +
                                " AND CreateTime BETWEEN '{1}-{2}-26 00:00:00' and '{1}-{3}-31 23:59:59' ", project, now.Year, now.Month - 1, now.Month);
        }
        return sql;
    }

    /// <summary>
    /// 获取销售线预算费用申请金额
    /// </summary>
    /// <param name="feeDetail">费用明细</param>
    /// <returns></returns>
    public static string GetTheSalesLineBudget(string fee_department, string fee_branch, string product)
    {
        DateTime now = DateTime.Now;
        string sql = string.Format("SELECT * FROM `wf_form_销售线预计流向审批表` a LEFT JOIN new_cost_sharing b on a.`产品`=b.ProductId and a.`网点`=b.HospitalId WHERE   a.CreateTime between '{0}-{1}-26 00:00:00' " +
                "and '{2}-{3}-25 23:59:59' and a.status='已审批' and b.CreateTime between '{2}-{3}-01 00:00:00' and '{2}-{4}-01 00:00:00'", now.Year, now.Month - 1, now.Year, now.Month, now.Month + 1);
        if(fee_branch!="")
        {
            string.Format(" and b.ProductId in (select Id from jb_product where name='{0}') and b.HospitalId=(select Id from fee_branch_dict where Name='{1}')", product, fee_branch);
        }
        if(fee_department!="")
        {
            sql += string.Format(" and (b.FirstDepartmentId=(select Id from department where name='{0}') or b.SecondDepartmentId=(select Id from department where name='{0}') " +
                "or b.ThirdDepartmentId=(select Id from department where name='{0}'))", fee_department);
        }
        return sql;
    }

    /// <summary>
    /// 获取销售线预算外费用申请金额
    /// </summary>
    /// <param name="feeDetail">费用明细</param>
    /// <returns></returns>
    public static string ObtainTheAmountOfExtraBudgetaryExpensesOfTheSalesLine(string feeDetail, string fee_department, string fee_branch, string product)
    {
        DateTime now = DateTime.Now;
        string sql = "";
        if (fee_branch != "")
        {
            sql += string.Format("SELECT IFNULL(SUM(`金额`),0) FROM `wf_form_销售线预算外费用申请表` WHERE 产品 in (select Id from jb_product where name='{0}') and 网点=(select Id from fee_branch_dict where Name='{1}')" +
                " and CreateTime between '{2}-{3}-26 00:00:00' and '{4}-{5}-25 23:59:59' and status='已审批' and " +
                "`费用明细`=(SELECT Id FROM fee_detail_dict_copy WHERE name='{6}')", product, fee_branch, now.Year, now.Month - 1, now.Year, now.Month,feeDetail);
        }
        if (fee_department != "")
        {
            sql += string.Format("SELECT IFNULL(SUM(`金额`),0) FROM `wf_form_销售线预算外费用申请表` WHERE departmentId=(select Id from department where name='{0}')" +
               " and CreateTime between '{1}-{2}-26 00:00:00' and '{3}-{4}-25 23:59:59' and status='已审批' and `费用明细`" +
               "=(SELECT Id FROM fee_detail_dict_copy WHERE name='{5}')", fee_department, now.Year, now.Month - 1, now.Year, now.Month, feeDetail);
        }   
        return sql;
    }

    /// <summary>
    /// 获取移动报销相关费用金额
    /// </summary>
    /// <param name="feeDetail">费用明细</param>
    /// <returns></returns>
    public static string AccessToTheCostOfMobileReimbursement(string feeDetail, string fee_department, string fee_branch, string product,string project)
    {
        DateTime now = DateTime.Now;
        string sql = string.Format("SELECT IFNULL(sum(fee_amount),0) from yl_reimburse where fee_detail in(SELECT IF(ParentName is null, `name`,CONCAT(ParentName,'-',`name`)) FROM fee_detail_dict_copy WHERE" +
                  " ParentName='{0}' OR `name`='{1}') AND `status`<>'已拒绝' AND ApprovalResult<>'已拒绝'AND  `status`<>'草稿' and apply_time between " +
                  "'{2}-{3}-26 00:00:00' and '{4}-{5}-25 23:59:59'", feeDetail, feeDetail, now.Year, now.Month - 1, now.Year, now.Month);
        if (fee_branch != "")
        {
            sql += string.Format(" and branch='{0}' and product='{1}'", fee_branch, product);
        }
        if (fee_department != "")
        {
            sql += string.Format(" and fee_department='{0}'", fee_department);
        }
        if (string.IsNullOrEmpty(project))
        {
            string.Format(" and Project='{0}'", project);
        }
        return sql;
    }

    public static string GetCostBalanceOfLastMonth(string feeDetail, string fee_department, string fee_branch, string product)
    {
        DateTime now = DateTime.Now;
        string sql= string.Format("select IFNULL(sum(LastMonthSurplus),0) from surplus where CreateTime between '{0}-{1}-01 00:00:00' and '{2}-{3}-01 00:00:00' and FeeDetail='{4}'",
               now.Year, now.Month, now.Year, now.Month + 1, feeDetail);
        if (fee_branch != "")
        {
            sql += string.Format(" and ProductId in (select Id from jb_product where name='{0}') and HospitalId=(select Id from fee_branch_dict where Name='{1}')", product, fee_branch);
        }
        if (fee_department != "")
        {
            sql += string.Format(" and DepartmentId=(select Id from department where name='{0}')", fee_department);
        }
        return sql;
    }



    ///// <summary>
    ///// 获取费用归属部门费用详细信息
    ///// </summary>
    ///// <param name="fee_detail">费用明细</param>
    ///// <param name="fee_department">费用归属部门</param>
    ///// <returns></returns>
    //public static DataSet GetNetInformation(string fee_detail,string fee_department,string fee_branch,string product)
    //{
    //    DateTime date = DateTime.Now;
    //    List<string> sqlList = new List<string>();
    //    if (fee_detail == "制造费用")
    //    {
    //        sqlList.Add(AccessToTheBudgetOfTheFunctionLine(fee_detail));
    //        sqlList.Add(AccessToTheExtraBudgetaryCostOfTheFunctionalLine(fee_detail));
    //        string sql = string.Format("select * from surplus where CreateTime between '{0}-{1}-01 00:00:00' and '{2}-{3}-01 00:00:00' and FeeDetail='{4}'",
    //           date.Year, date.Month, date.Year, date.Month + 1, fee_detail);
    //        sqlList.Add(sql);
    //    }
    //    else
    //    {
    //        string sql1 = string.Format("SELECT * FROM `wf_form_预计流向审批表` a LEFT JOIN new_cost_sharing b on a.`产品`=b.ProductId and a.`网点`=b.HospitalId WHERE   a.CreateTime between '{0}-{1}-26 00:00:00' " +
    //            "and '{2}-{3}-25 23:59:59' and a.status='已审批' b.CreateTime between '{2}-{3}-01 00:00:00' and '{2}-{4}-01 00:00:00'", date.Year, date.Month - 1, date.Year, date.Month, date.Month + 1);

    //        string sql2 = string.Format("SELECT IFNULL(sum(fee_amount),0) from yl_reimburse where fee_detail in(SELECT IF(ParentName is null, `name`,CONCAT(ParentName,'-',`name`)) FROM fee_detail_dict_copy WHERE" +
    //             " ParentName='{0}' OR `name`='{1}') AND `status`<>'已拒绝' AND ApprovalResult<>'已拒绝'AND  `status`<>'草稿' and apply_time between " +
    //             "'{2}-{3}-26 00:00:00' and '{4}-{5}-25 23:59:59'", fee_detail, fee_detail, date.Year, date.Month - 1, date.Year, date.Month);
    //        string sql3 = string.Format("select * from surplus where CreateTime between '{0}-{1}-01 00:00:00' and '{2}-{3}-01 00:00:00' and FeeDetail='{4}'",
    //            date.Year, date.Month, date.Year, date.Month + 1, fee_detail);
    //        string sql4 = "";
    //        if (fee_department != "1" && fee_department != "2")
    //        {
    //            sql1 += string.Format(" and b.DepartmentId=(select Id from department where name='{0}')", fee_department);
    //            sql2 += string.Format(" and fee_department='{0}'", fee_department);
    //            sql3 += string.Format(" and DepartmentId=(select Id from department where name='{0}')", fee_department);
    //            sql4 += string.Format("SELECT SUM(`金额`) FROM `wf_form_预算外费用申请表` WHERE departmentId=(select Id from department where name='{0}')" +
    //            " and CreateTime between '{1}-{2}-26 00:00:00' and '{3}-{4}-25 23:59:59' and status='已审批' ", fee_department, date.Year, date.Month - 1, date.Year, date.Month);
    //        }
    //        else if (fee_department == "2")
    //        {
    //            sql1 += string.Format(" and b.ProductId in (select Id from jb_product where name='{0}') and b.HospitalId=(select Id from fee_branch_dict where Name='{1}')", product, fee_branch);
    //            sql2 += string.Format(" and branch='{0}' and product='{1}'", fee_branch, product);
    //            sql3 += string.Format(" and ProductId in (select Id from jb_product where name='{0}') and HospitalId=(select Id from fee_branch_dict where Name='{1}')", product, fee_branch);
    //            sql4 += string.Format("SELECT SUM(`金额`) FROM `wf_form_预算外费用申请表` WHERE 产品 in (select Id from jb_product where name='{0}') and 网点=(select Id from fee_branch_dict where Name='{1}')" +
    //            " and CreateTime between '{2}-{3}-26 00:00:00' and '{4}-{5}-25 23:59:59' and status='已审批' ", product, fee_branch, date.Year, date.Month - 1, date.Year, date.Month);
    //        }
    //        sqlList.Add(sql1);
    //        sqlList.Add(sql2);
    //        sqlList.Add(sql3);
    //        if (sql4 != "")
    //            sqlList.Add(sql4);
    //    }
    //    return SqlHelper.Find(sqlList.ToArray());
    //}
    public static string insertMobileReimburse(string code, string apply_time, string product, string branch, string fee_department, string fee_detail, string fee_amount, 
        string file, string remark, UserInfo userInfo, string approver, string department, string project,string isOverBudget, string isPrepaid, string isHasReceipt, string fee_company, string reportName)
    {
        string sql = string.Format("insert into yl_reimburse (code,apply_time,name,department, branch, fee_department, fee_detail, product, approver,remark,file,fee_amount,LMT,project,IsOverBudget,isPrepaid,isHasReceipt,fee_company,remain_fee_amount, report_department) " +
            "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',NOW(),'{12}','{13}','{14}','{15}', '{16}','{17}', '{18}')",
            code, apply_time, userInfo.userName, department, branch, fee_department, fee_detail, product, approver, remark, file, fee_amount, project, isOverBudget,isPrepaid,isHasReceipt, fee_company,fee_amount,reportName);

        return SqlHelper.InsertAndGetLastId(sql);
    }

    public static string updateMobileReimburse(string code, string apply_time, string product, string branch, string fee_department, string fee_detail, string fee_amount,
        string file, string remark, UserInfo userInfo, string approver, string department, string project, string isOverBudget, string isPrepaid, string isHasReceipt, string fee_company, string reportName)
    {
        string sql = string.Format("update yl_reimburse set apply_time = '{0}',name = '{1}',department = '{2}',branch = '{3}',fee_department = '{4}'," +
            "fee_detail = '{5}',product = '{6}',approver = '{7}',remark = '{8}',file = '{9}',fee_amount = '{10}',LMT = NOW(), project = '{12}', isOverBudget = '{13}',isPrepaid = '{14}',isHasReceipt = '{15}', fee_company = '{16}', report_department= '{17}' where code = '{11}'", apply_time, userInfo.userName, 
            department, branch, fee_department, fee_detail, product, approver, remark, file, fee_amount, code, project, isOverBudget, isPrepaid, isHasReceipt, fee_company, reportName);
        return SqlHelper.Exce(sql);
    }

    public static DataSet findDepartmentNameByWechatUserId(string wechatUserId)
    {
        string sql = string.Format("select t1.name from department t1 left join user_department_post t2 on t1.id = t2.departmentId where t2.wechatUserId = '{0}'", wechatUserId);
        return SqlHelper.Find(sql);
    }

    public static DataSet findMaxId()
    {
        string sql = string.Format("select max(id) from yl_reimburse");
        return SqlHelper.Find(sql);
    }

    public static string clearApprovalProcess(string docCode)
    {
        string sql = string.Format("delete from approval_process where DocumentTableName = 'yl_reimburse' and docCode = '{0}'", docCode);
        return SqlHelper.Exce(sql);
    }

    public static string updateApprovalTimeAndResultAndOpinion(string docCode, string result, string opinion)
    {
        string sql = string.Format("update yl_reimburse set approval_time = now(),ApprovalOpinions = '{1}',ApprovalResult = '{2}' where id = '{0}'", docCode,opinion,result);
        return SqlHelper.Exce(sql);
    }

    public static DataSet checkMultiDepartment(UserInfo userInfo)
    {
        string sql = string.Format("select dt.name from user_department_post udp left join department dt on udp.departmentId = dt.id " +
            "where udp.wechatUserId = '{0}'", userInfo.wechatUserId);
        return SqlHelper.Find(sql);
    }

    public static DataSet findApprovalRecord(string userId, string keyword,string year,int month)
    {
        int nextMonth, nextYear;
        if (year == "12")
        {
            nextMonth = 1;
            nextYear = int.Parse(year) + 1;
        }
        else
        {
            nextMonth = month + 1;
            nextYear = int.Parse(year);
        }
        string sql = string.Format("select distinct yr.* from yl_reimburse yr left join approval_record ac on yr.id = ac.docCode " +
            "where ac.documentTableName = 'yl_reimburse' and ac.approverId = '{0}'" +
            "and ac.Time between '{1}-{2}-1 00:00:00' and '{3}-{4}-1 00:00:00' and ac.Level > 0 ", userId, year, month, nextYear, nextMonth);
        if (!String.IsNullOrEmpty(keyword))
            sql += " and (remark like '%" + keyword + "%' or fee_detail like '%" + keyword + "%' or name like '%" + keyword + "%')";
        sql += " order by Id desc";
        return SqlHelper.Find(sql);
    }

    public static string insertInformer(string docCode, List<string> userIdList)
    {
        List<string> sqls = new List<string>();
        foreach (string userId in userIdList)
        {
            string sql = string.Format("insert into approval_informer (tablename,doccode,informerUserId) values ('yl_reimburse','{0}','{1}')", docCode, userId);
            sqls.Add(sql);
        }
        return SqlHelper.Exce(sqls.ToArray());
    }

    public static string insertAttachement(string docCode, List<string> attachMentList)
    {
        List<string> sqls = new List<string>();
        string sql = string.Format("delete from yl_reimburse_attachment where docCode = '{0}'", docCode);
        sqls.Add(sql);

        foreach (string attachment in attachMentList)
        {
            sql = string.Format("insert into yl_reimburse_attachment (docCode,imageUrl,lmt) values ('{0}', '{1}', now())", docCode, attachment.Replace("\\","/"));
            sqls.Add(sql);
        }
        return SqlHelper.Exce(sqls.ToArray());
    }

    public static DataSet findRelatedReimburse(string userId, string keyword)
    {
        string sql = string.Format("select yr.* from yl_reimburse yr inner join approval_informer ai on yr.id = ai.docCode and " +
            "ai.TableName = 'yl_reimburse' where ai.InformerUserId = '{0}'", userId);
        if (!String.IsNullOrEmpty(keyword))
        {
            sql += " and (remark like '%" + keyword + "%' or fee_detail like '%" + keyword + "%' or name like '%" + keyword + "%')";
        }
        return SqlHelper.Find(sql);
    }

    public static DataTable FindProduct(int userId)
    {
        List<string> listSql = new List<string>();
        //listSql.Add(string.Format("select distinct ProductName from v_client_product_user where UserId={0} order by ProductCode", userId));
        listSql.Add(string.Format("select DISTINCT t2.ProductName from new_client_product_users t1 left join new_product t2 on t1.ProductCode = t2.ProductCode where t1.userId = '{0}'",userId));
        listSql.Add(string.Format("select DISTINCT concat(t2.ProductName, '(',t2.specification, ')(',t2.unit,')') ProductName from new_product t2"));
        listSql.Add(string.Format("SELECT * FROM `v_user_department_post` WHERE `department` LIKE '%营销中心/销售部%' and " +
            "userId = {0}", userId));
            //"userId <> 100000143 and userId <> 100000216"));
        DataSet ds = SqlHelper.Find(listSql.ToArray());
        if (ds == null)
            return null;
        int i = 0;
        //不属于销售部，并且如果是黄文俊和付玉林，可选择所有产品和网点
        if (ds.Tables[2].Rows.Count==0 || (userId == 100000143 || userId == 100000216))
        {
            i = 1;
        }

        DataRow row = ds.Tables[i].NewRow();
        row[0] = "综合";
        ds.Tables[i].Rows.InsertAt(row,0);

        row = ds.Tables[i].NewRow();
        row[0] = "非销售";
        ds.Tables[i].Rows.InsertAt(row,0);

        return ds.Tables[i];
    }

    public static DataTable FindClient(uint userId)
    {
        List<string> listSql = new List<string>();
        //listSql.Add(string.Format("select distinct ClientName from v_client_product_user where UserId={0} order by ClientCode", userId));
        listSql.Add(string.Format("select distinct t2.ClientName from new_client_product_users t1 left join new_client t2 on t1.ClientCode = t2.ClientCode where t1.userId = '{0}'", userId));
        listSql.Add(string.Format("select distinct t2.ClientName from new_client t2"));
        listSql.Add(string.Format("SELECT * FROM `v_user_department_post` WHERE `department` LIKE '%营销中心/销售部%' and " +
            "userId = {0}", userId));
        DataSet ds = SqlHelper.Find(listSql.ToArray());
        if (ds == null)
            return null;
        int i = 0;
        //不属于销售部，并且如果是黄文俊和付玉林，可选择所有产品和网点
        if (ds.Tables[2].Rows.Count == 0 || (userId == 100000143 || userId == 100000216))
        {
            i = 1;
        }

        DataRow row = ds.Tables[i].NewRow();
        row[0] = "综合";
        ds.Tables[i].Rows.InsertAt(row, 0);

        row = ds.Tables[i].NewRow();
        row[0] = "非销售";
        ds.Tables[i].Rows.InsertAt(row, 0);

        return ds.Tables[i];
    }

    public static DataSet findProductName(string name, List<DepartmentPost> departmentList, string departmentName)
    {
        Boolean isLeader = false;

//        if (departmentList != null && departmentName != "")
//        {
//            foreach (DepartmentPost departmentPost in departmentList)
//            {
//                int departmentId = departmentPost.departmentId;
//                string findDepartmentName = SqlHelper.Find("select name from department where id = " + departmentId).Tables[0].Rows[0][0].ToString();
//
//                if (findDepartmentName.Equals(departmentName) && departmentPost.isHead == 1 && departmentName.Contains("集团营销中心"))
//                {
//                    isLeader = true;
//                    break;
//                }
//            }
//        }

        string sql = "";
        if (!isLeader)
        {
            sql = string.Format("select distinct name from jb_product where name like '%{0}%' order by Id", name);
        }
        else
        {
            sql = string.Format("select '综合' name from jb_product limit 1");
        }

        return SqlHelper.Find(sql);
    }

    public static DataSet findProductName()
    {
        string sql = string.Format("select distinct name from jb_product order by Id");
        return SqlHelper.Find(sql);
    }

    public static DataSet accountStatistics(string userId,string year,int month,string type,ref string msg)
    {
        string sql = string.Format("SELECT yr1.{0}, count(yr1.`code`) as sum,sum(yr1.fee_amount) AS feeAmount, sum(yr1.actual_fee_amount) AS actualFeeAmount " +
            " FROM (SELECT distinct yr.code,yr.`name`,yr.fee_department,yr.fee_detail,yr.remark,yr.fee_amount,yr.actual_fee_amount FROM yl_reimburse " +
            "yr LEFT JOIN approval_record ac ON yr.id = ac.docCode WHERE ac.documentTableName = 'yl_reimburse' AND  ac.ApprovalResult = '同意' " +
            "and ac.ApprovalOpinions != '自动审批' AND approverId = '{1}' and yr.approval_time between '{4}-{5}-01 00:00:00' and '{4}-{5}-31 23:59:59') yr1" +
            " GROUP BY yr1.{6};", type,userId,year,month-1,year,month,type);
        sql+= string.Format("SELECT distinct yr.code,yr.`name`,yr.fee_department,yr.fee_detail,yr.remark,yr.fee_amount,yr.actual_fee_amount" +
            " FROM yl_reimburse yr LEFT JOIN approval_record ac ON yr.id = ac.docCode WHERE ac.documentTableName = 'yl_reimburse' AND" +
            " ac.ApprovalResult = '同意' and ac.ApprovalOpinions != '自动审批' AND approverId = '{1}' and yr.approval_time between '{4}-{5}-01 00:00:00' and '{4}-{5}-31 23:59:59'"
            , type, userId, year, month - 1, year, month, type);
        return SqlHelper.Find(sql,ref msg);
    }

    public static DataSet findInformerByCode(string docCode)
    {
        string sql1 = string.Format("SELECT GROUP_CONCAT(t1.InformerUserId) userId, GROUP_CONCAT(t2.userName) userName FROM approval_informer t1 left join users t2 on " +
            "t1.InformerUserId = t2.userId left join yl_reimburse t3 on t1.docCode = t3.id where t3.code = '{0}' and t1.TableName = 'yl_reimburse'", docCode);

        string sql2 = string.Format("select GROUP_CONCAT(t1.ImageUrl) url from yl_reimburse_attachment t1 left join yl_reimburse t2 on t1.docCode = t2.id where t2.code = '{0}'", docCode);

        List<string> sqls = new List<string>();

        sqls.Add(sql1);
        sqls.Add(sql2);

        return SqlHelper.Find(sqls.ToArray());
    }

    public static string cancel(string docCode, UserInfo userInfo)
    {
        string sql1 = string.Format("update yl_reimburse set status = '草稿', level = '-1' where code = '{0}'", docCode);

        string sql2 = string.Format("insert into approval_record (DocumentTableName,DocCode,Level,Time,ApproverId,SubmitterId,ApprovalResult)" +
            "values ('yl_reimburse',(select id from yl_reimburse where code = '{0}'),'0',now(),'{1}','{1}','撤消')",docCode, userInfo.userId);

        List<string> sqls = new List<string>();

        sqls.Add(sql1);
        sqls.Add(sql2);

        return SqlHelper.Exce(sqls.ToArray());
    }

    public static DataSet GetDepartmentListAndFeeDetailList()
    {
        string sql = "select * from fee_detail_dict_copy";
        return SqlHelper.Find(sql);
    }
}