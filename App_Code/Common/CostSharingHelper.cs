using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

/// <summary>
/// CostSharingHelper 的摘要说明
/// </summary>
public class CostSharingHelper
{
    public CostSharingHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 网点基础数据导入
    /// </summary>
    /// <param name="sheet">网点数据excel表格</param>
    /// <param name="HeaderRowIndex"></param>
    /// <param name="flag">是否计算上月结余，true表示计算，false表示不计算</param>
    /// <returns></returns>
    public static DataTable importCostSharing(DataTable dt, Boolean flag)
    {
        DateTime lastMonthLastDay = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1); //上月最后一天

        //找出人员 部门 医院 产品  上月综合类移动报销
        string tempSql = "select userName,userId from users;";
        tempSql += "select name,id,parentId from department;";
        tempSql += "select name,id from fee_branch_dict;";
        tempSql += "select name,id from jb_product;";
        tempSql += "select * from fee_detail_dict_copy;";
        tempSql += string.Format("SELECT sum(a.fee_amount) feeAmount,a.fee_detail,b.Id departmentId from yl_reimburse a left join department b on a.fee_department=b.name" +
           " where  a.branch='综合' and a.branch='综合' and a.approval_time BETWEEN '{0}-{1}-01 00:00:00' AND '{0}-{1}-{2} 23:59:59' AND a.apply_time " +
           "BETWEEN '{0}-{1}-01 00:00:00' AND '{0}-{1}-{2} 23:59:59' group by a.fee_department", lastMonthLastDay.Year, lastMonthLastDay.Month, lastMonthLastDay.Day);

        DataSet ds = SqlHelper.Find(tempSql);

        DataTable userTable = ds.Tables[0];
        DataTable departmentTable = ds.Tables[1];
        DataTable branchTable = ds.Tables[2];
        DataTable productTable = ds.Tables[3];

        List<JObject> dataObjectList = new List<JObject>();
        List<string> sqls = new List<string>();
        List<JObject> jarray = new List<JObject>();
        DateTime now = DateTime.Now;
        //search(ref jarray, "", "", "", "制造费用", "");
        DataTable errorTable = dt.Clone();

        string updateSql = "";
        foreach (DataRow dr in dt.Rows)
        {
            // 网点表中1-4是人员，4-6是部门，7是医院，8是产品，从11开始后面都是数字
            string sales = dr["代表"].ToString();
            string salesId = getIdFromName(userTable, sales);

            string supervisor = dr["经理"].ToString();
            string supervisorId = getIdFromName(userTable, supervisor);

            string manager = dr["大区经理"].ToString();
            string managerId = getIdFromName(userTable, manager);

            string director = dr["总监"].ToString();
            string directorId = getIdFromName(userTable, director);

            string first_department = dr["战区"].ToString();
            string second_department = dr["大区"].ToString();
            string third_department = dr["区域"].ToString();

            string first_departmentId = "";
            string second_departmentId = "";
            string third_departmentId = "";

            Boolean tempflag = getIdFromName(departmentTable, ref first_department, ref second_department,
                ref third_department, ref first_departmentId, ref second_departmentId, ref third_departmentId);

            string hospital = dr["医院"].ToString();
            string hospitalId = getIdFromName(branchTable, hospital);

            string product = dr["产品"].ToString();
            string productId = getIdFromName(productTable, product);

            if (string.IsNullOrEmpty(salesId) || !tempflag ||
                string.IsNullOrEmpty(hospitalId) ||
                string.IsNullOrEmpty(productId))
            {

                errorTable.Rows.Add(dr.ItemArray);
                continue;
            }

            // 后面数字的以json的形式存入数据库中
            JObject jObject = new JObject();
            for (int j = 9; j < 109; j++)
            {
                string key = dt.Columns[j].ColumnName;

                
                if (!dr[j].ToString().Contains("%")&&!StringTools.IsNumeric(dr[j].ToString()))
                {
                    dr[j] = "0";
                }


                jObject.Add(key, dr[key].ToString());

            }

            // 再把数据插入到网点表中
            JObject dataObject = new JObject();

            dataObject.Add("SalesId", salesId);
            dataObject.Add("SupervisorId", supervisorId);
            dataObject.Add("ManagerId", managerId);
            dataObject.Add("DirectorId", directorId);
            dataObject.Add("FirstDepartmentId", first_departmentId);
            dataObject.Add("SecondDepartmentId", second_departmentId);
            dataObject.Add("ThirdDepartmentId", third_departmentId);
            dataObject.Add("HospitalId", hospitalId);
            dataObject.Add("ProductId", productId);
            dataObject.Add("DataJson", jObject.ToString());
            dataObject.Add("CreateTime", now);


            dataObjectList.Add(dataObject);

            

            ////if (flag)
            ////{


            ////计算公司研发费用金额上月结余
            //search(ref jarray, "", "", "", "研发费用金额",
            //      (Convert.ToDouble(dr["研发费用金额"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            ////计算公司总部管理费用上月结余
            //search(ref jarray, "", "", "", "总部管理费用",
            //      (Convert.ToDouble(dr["总部管理费用"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            ////计算公司市场学术费上月结余
            //search(ref jarray, "", "", "", "市场学术费",
            // (Convert.ToDouble(dr["市场学术费"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            ////计算公司商务费用金额上月结余
            //search(ref jarray, "", "", "", "商务费用金额",
            //(Convert.ToDouble(dr["商务费用金额"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            ////计算公司工资社保金额上月结余
            //search(ref jarray, "", "", "", "工资社保金额",
            //(Convert.ToDouble(dr["工资社保金额"]) * Convert.ToDouble(dr["当月流向数"])).ToString());


            ////计算公司招商费用上月结余
            //if (dr["部门"].ToString() == "招商部")
            //{
            //    double InvestmentCosts = (Convert.ToDouble(dr["销售总监费用"]) + Convert.ToDouble(dr["大区经理费用"]) +
            //        Convert.ToDouble(dr["开发费用金额"]) + Convert.ToDouble(dr["产品发展基金"]) + Convert.ToDouble(dr["实验费(TF)金额"]) +
            //        Convert.ToDouble(dr["区域中心费用VIP维护"]) + Convert.ToDouble(dr["区域中心费用"])) * Convert.ToDouble(dr["当月流向数"]);
            //    search(ref jarray, "", "", "", "招商费用",
            //       InvestmentCosts.ToString());
            //}
            //else//非招商部费用计算结余
            //{
            //    //计算各战区销售总监费用上月结余
            //    search(ref jarray, "", "", first_departmentId, "销售总监费用",
            //           (Convert.ToDouble(dr["销售总监费用"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            //    //计算各大区大区经理费用本月结余
            //    search(ref jarray, "", "", second_departmentId, "大区经理费用",
            //     (Convert.ToDouble(dr["大区经理费用"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            //    //计算各地区开发费用金额上月结余
            //    search(ref jarray, "", "", third_departmentId, "开发费用金额",
            //    (Convert.ToDouble(dr["开发费用金额"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            //    //计算各地区产品发展基金上月结余
            //    search(ref jarray, "", "", third_departmentId, "产品发展基金",
            //     (Convert.ToDouble(dr["产品发展基金"]) * Convert.ToDouble(dr["当月流向数"])).ToString());


            //    //计算各地区实验费(TF)金额上月结余
            //    search(ref jarray, "", "", third_departmentId, "实验费(TF)金额",
            //   (Convert.ToDouble(dr["实验费(TF)金额"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            //    //计算各地区区域中心费用上月结余
            //    search(ref jarray, "", "", third_departmentId, "区域中心费用",
            //   (Convert.ToDouble(dr["区域中心费用"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            //    //计算各地区区域中心费用VIP维护上月结余
            //    search(ref jarray, "", "", third_departmentId, "区域中心费用VIP维护",
            //    (Convert.ToDouble(dr["区域中心费用VIP维护"]) * Convert.ToDouble(dr["当月流向数"])).ToString());

            //    //计算各网点市场调节基金本月结余
            //    search(ref jarray, hospitalId, productId, "", "市场调节基金",
            //    GetNetLastMonthSurplus(product, hospital, productId, hospitalId, "市场调节基金", "") + (Convert.ToDouble(dr["市场调节基金"]) * Convert.ToDouble(dr["当月纯销"])).ToString());

            //    //计算各网点市场调节基金上月结余
            //    search(ref jarray, hospitalId, productId, "", "销售折让",
            //   GetNetLastMonthSurplus(product, hospital, productId, hospitalId, "销售折让", "") + string.Format("(SELECT ifnull(sum(Flow)*{5},0) FROM sales_allowwance  where ProductId= '{0}' " +
            //   "and HospitalId= '{1}' AND CreateTime BETWEEN'{2}-{3}-01 00:00:00' AND '{2}-{4}-01 00:00:00')",
            //   productId, hospitalId, now.Year, now.Month - 2, now.Month - 1, dr["销售折让1"]));
            //}
        }
        //}

//        foreach(JObject newCostSharingObject in dataObjectList)
//        {
//            JObject dataJson = JObject.Parse(newCostSharingObject["DataJson"].ToString());
//            foreach(var ss in dataJson)
//            {
//                if(ds.Tables[4].Select("Name='" + ss.Key + "'").Length > 0)
//                {
//                    updateSql += UpdateAvailableCredit(newCostSharingObject["ProductId"].ToString(), newCostSharingObject["HospitalId"].ToString(), newCostSharingObject["SalesId"].ToString(),
//                        dataJson["当月流向数"].ToString(),ss.Key.ToString(),ss.Value.ToString(),ds.Tables[5],dataObjectList, newCostSharingObject["FirstDepartmentId"].ToString(),
//                       newCostSharingObject["SecondDepartmentId"].ToString(), newCostSharingObject["ThirdDepartmentId"].ToString());
//                }
//            }
//        }
//        
//        sqls.Add(updateSql);
        string insertSql = SqlHelper.GetInsertStringForImportCostSharing(dataObjectList, "new_cost_sharing", 10000);
        sqls.Add(insertSql);

        //if (flag)
        ////{       
        //foreach (JObject one in jarray)
        //{
        //    if (one["FeeDetail"].ToString() != "销售折让" && one["FeeDetail"].ToString() != "市场调节基金")
        //    {
        //        one["LastMonthSurplus"] += GetNetLastMonthSurplus("", "", "", "", one["FeeDetail"].ToString(), one["DepartmentId"].ToString());
        //    }
        //    one["LastMonthSurplus"] += string.Format("(select ifnull(sum(t1.LastMonthSurplus),0) from (SELECT LastMonthSurplus FROM `surplus` where ProductId={0} AND HospitalId={1} " +
        //        "AND FeeDetail='{2}' and CreateTime BETWEEN '{3}-{4}-01 00:00:00' AND '{3}-{5}-01 00:00:00' and DepartmentId={6}) t1)",
        //        one["ProductId"].ToString() == "" ? "''" : one["ProductId"], one["HospitalId"].ToString() == "" ? "''" : one["HospitalId"], one["FeeDetail"].ToString(), now.Year,
        //        now.Month - 2, now.Month - 1, one["DepartmentId"].ToString() == "" ? "''" : one["DepartmentId"]);

        //}
        //sqls.Add(SqlHelper.GetInsertStringForImportCostSharing(jarray, "surplus", 200));
        //}

        if (SqlHelper.Exce(sqls.ToArray()).Contains("操作失败"))
        {
            return dt;
        }
        else
        {
            return errorTable;
        }
    }

    public static string UpdateAvailableCredit(string productId, string hospitalId, string salesId,
        string lastMonthFlow, string feedetail, string costRate, DataTable dt, List<JObject> newCostSharingObject, string firstDepartmentId, string secondDepartmentId, string thirdDepartmentId)
    {
        DateTime date = DateTime.Now;
        DateTime lastMonthLastDay = date.AddDays(1 - date.Day).AddDays(-1); //上月最后一天

        string availableCredit = string.Format("((SELECT ExpectFlow FROM `expect_flow_submit` where ProductId={0} and HospitalId={1} and SalesId={2} and IsFinished=1 and CreateTime " +
            "BETWEEN '{3}-{4}-01' and '{3}-{4}-28')", productId, hospitalId, salesId, lastMonthLastDay.Year, lastMonthLastDay.Month);//本月预计流向

        availableCredit += "+" + lastMonthFlow + ")*" + costRate;
        //上月报销
        availableCredit += string.Format("-(SELECT ifnull(sum(fee_amount),0)  from yl_reimburse a left join users b on a.`name`=b.userName LEFT JOIN jb_product c on a.product=c.`Name` " +
            "LEFT JOIN fee_branch_dict d on a.branch=d.`Name` where b.userId='{0}' and d.Id='{1}' and c.Id='{2}' AND fee_detail LIKE '{3}%' AND account_approval_time" +
           " BETWEEN '{4}-{5}-26 00:00:00' AND '{6}-{7}-25 23:59:59')", salesId, hospitalId, productId, feedetail, lastMonthLastDay.Year, lastMonthLastDay.Month, date.Year, date.Month);

        //上月综合报销分摊

        foreach (DataRow row in dt.Rows)
        {
            List<string> departmentIdList = new List<string>();
            if (row["departmentId"].ToString() == "291")//销售部
            {
                departmentIdList.Add("296");
                departmentIdList.Add("297");
                departmentIdList.Add("298");
                departmentIdList.Add("299");
                departmentIdList.Add("300");
                departmentIdList.Add("301");
                departmentIdList.Add("302");
            }
            else
            {
                departmentIdList.Add(row["departmentId"].ToString());
            }
            double sum = 0;


            foreach (JObject Object in newCostSharingObject)
            {
                if (departmentIdList.Contains(Object["FirstDepartmentId"].ToString()) || departmentIdList.Contains(Object["SecondDepartmentId"].ToString()) ||
                departmentIdList.Contains(Object["ThirdDepartmentId"].ToString()))//找出该部门下的网点
                {
                    JObject DataJson = JObject.Parse(Object["DataJson"].ToString());
                    sum += Convert.ToDouble(DataJson["当月流向数"]) * Convert.ToDouble(DataJson[feedetail]);
                }
            }
            if (sum != 0)
            {
                sum = Convert.ToDouble(row["feeAmount"]) * (Convert.ToDouble(lastMonthFlow) * Convert.ToDouble(costRate) / sum);
            }
            availableCredit += "-" + sum;

        }

        string condition = string.Format("where ProductId={0} and HospitalId={1} and SalesId={2} and FeeDetailName='{3}' and" +
                 " CreateTime between '{4}-{5}-01 00:00:00' and '{4}-{5}-{6} 23:59:59'", productId, hospitalId, salesId, feedetail, lastMonthLastDay.Year, lastMonthLastDay.Month, lastMonthLastDay.Day);

        JObject temp = new JObject();
        temp.Add("LimitNumber", availableCredit);

        return SqlHelper.GetUpdateStringForCostSharing(temp, "branch_available_credit", condition);
    }

    private static string GetNetLastMonthSurplus(string product, string hospital, string productId, string hospitalId,
        string feedetail, string departmentId)
    {
        DateTime now = DateTime.Now;
        string sql = "";
        if (feedetail == "销售折让" || feedetail == "市场调节基金")
        {
            //上月移动报销
            sql = string.Format("(SELECT ifnull(SUM(fee_amount),0) from yl_reimburse where ApprovalResult='同意' and branch='{0}' AND product='{1}'" +
                   " AND fee_detail LIKE '{2}%' AND approval_time BETWEEN '{3}-{4}-26 00:00:00' AND '{3}-{5}-25 23:59:59')+",
                   hospital, product, feedetail, now.Year, now.Month - 2, now.Month - 1);

            //上月预算外费用申请
            sql += string.Format("(SELECT ifnull(SUM(`金额`),0) FROM `wf_form_销售线预算外费用申请表` a " +
                " LEFT JOIN fee_detail_dict_copy d on a.`费用明细`= d.Id WHERE" +
                " a.产品= '{0}' AND a.网点= '{1}' AND(d.`Name`= '{2}' OR d.ParentName = '{2}') AND a.`Status`= '已审批' and " +
                "a.CreateTime BETWEEN '{3}-{4}-01 00:00:00' AND '{3}-{5}-01 00:00:00')+", productId, hospitalId, feedetail, now.Year, now.Month - 2, now.Month - 1);
        }
        else if (departmentId == "")//招商费用、研发费用金额、总部管理费用、市场学术费、工资社保金额、商务费用金额
        {
            //上月移动报销
            sql = string.Format("(SELECT ifnull(SUM(fee_amount),0) from yl_reimburse where ApprovalResult='同意'" +
                  " AND fee_detail LIKE '{0}%' AND approval_time BETWEEN '{1}-{2}-26 00:00:00' AND '{1}-{2}-25 23:59:59')+",
                   feedetail, now.Year, now.Month - 2, now.Month - 1);

            //上月预算外费用申请
            sql += string.Format("(SELECT ifnull(SUM(`金额`),0) FROM `wf_form_职能线预算外费用申请表` a  " +
                " LEFT JOIN fee_detail_dict_copy d on a.`费用明细`= d.Id WHERE" +
                "  (d.`Name`= '{0}' OR d.ParentName = '{0}') AND a.`Status`= '已审批' and " +
                "a.CreateTime BETWEEN '{1}-{2}-01 00:00:00' AND '{1}-{3}-01 00:00:00')+", feedetail, now.Year, now.Month - 2, now.Month - 1);
        }
        else//其他费用
        {
            //上月移动报销
            sql = string.Format("(SELECT ifnull(SUM(fee_amount),0) from yl_reimburse where ApprovalResult='同意' and fee_department={0}" +
                  " AND fee_detail LIKE '{1}%' AND approval_time BETWEEN '{2}-{3}-26 00:00:00' AND '{2}-{4}-25 23:59:59')+",
                  departmentId, feedetail, now.Year, now.Month - 2, now.Month - 1);

            //上月预算外费用申请
            sql += string.Format("(SELECT ifnull(SUM(`金额`),0) FROM `wf_form_销售线预算外费用申请表` a " +
                " LEFT JOIN fee_detail_dict_copy d on a.`费用明细`= d.Id WHERE" +
                " a.departmentId={0} AND(d.`Name`= '{1}' OR d.ParentName = '{1}') AND a.`Status`= '已审批' and " +
                "a.CreateTime BETWEEN '{2}-{3}-01 00:00:00' AND '{2}-{4}-01 00:00:00')+", departmentId, feedetail, now.Year, now.Month - 2, now.Month - 1);
        }
        return sql;

    }

    private static void search(ref List<JObject> jarray, string hospitalId, string productId, string departmentId, string feedetail, string LastMonthSurplus)
    {
        foreach (JObject one in jarray)
        {
            if (one["ProductId"].ToString() == productId && one["HospitalId"].ToString() == hospitalId
                && one["DepartmentId"].ToString() == departmentId && one["FeeDetail"].ToString() == feedetail)
            {
                one["LastMonthSurplus"] += LastMonthSurplus.ToString() + "+";
                return;
            }
        }
        JObject jObject = new JObject();

        jObject.Add("ProductId", productId);
        jObject.Add("HospitalId", hospitalId);
        jObject.Add("DepartmentId", departmentId);
        jObject.Add("FeeDetail", feedetail);
        jObject.Add("LastMonthSurplus", LastMonthSurplus + "+");
        jObject.Add("CreateTime", DateTime.Now.ToString());
        jarray.Add(jObject);
    }

    private static string getIdFromName(DataTable dt, string name)
    {
        if (dt == null || string.IsNullOrEmpty(name))
        {
            return null;
        }

        foreach (DataRow dr in dt.Rows)
        {
            if (dr == null)
                continue;

            if (name.Equals(dr[0]))
            {
                return dr[1].ToString();
            }
        }

        return null;
    }

    private static Boolean getIdFromName(DataTable dt, ref string department1, ref string department2, ref string department3,
        ref string departmentId1, ref string departmentId2, ref string departmentId3)
    {
        if (dt == null || string.IsNullOrEmpty(department1) || string.IsNullOrEmpty(department2) || string.IsNullOrEmpty(department3))
        {
            return false;
        }
        foreach (DataRow dr in dt.Rows)
        {
            if (dr == null)
                continue;

            if (dr[0].ToString().Contains(department1) && dr[0].ToString().Contains(department2) && dr[0].ToString().Contains(department2 + department3))
            {
                department3 = dr[0].ToString();
                departmentId3 = dr[1].ToString();
                departmentId2 = dr[2].ToString();
                break;
            }
        }
        if (departmentId3 == "" || departmentId2 == "")
        {
            return false;
        }
        else
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (departmentId2 == dr[1].ToString())
                {
                    department2 = dr[0].ToString();
                    departmentId1 = dr[2].ToString();
                    break;
                }
            }
            if (departmentId1 == "")
            {
                return false;
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (departmentId1 == dr[1].ToString())
                    {
                        department1 = dr[0].ToString();
                        return true;
                    }
                }
                return false;
            }
        }
    }
}