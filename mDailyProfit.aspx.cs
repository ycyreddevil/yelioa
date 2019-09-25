using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mDailyProfit : System.Web.UI.Page
{
    public string departmentName = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mDailyProfit",
           "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
           "1000006",
        "http://yelioa.top/mDailyProfit.aspx");

        string res = wx.CheckAndGetUserInfo(HttpContext.Current);

        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        List<DepartmentPost> departmentPostList = (List<DepartmentPost>)Session["DepartmentPostList"];

        departmentName = SqlHelper.Find("select name from department where id = " + departmentPostList[0].departmentId)
            .Tables[0].Rows[0][0].ToString();

        if (!departmentName.Contains("销售部"))
        {
            departmentName = "销售部";
        }

        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            if (action.Equals("getData"))
            {
                Response.Write(getData());
            }
            else if (action.Equals("getDepartment"))
            {
                Response.Write(getDepartment());
            }
            else if (action.Equals("getParentDepartment"))
            {
                Response.Write(getParentDepartment());
            }
            Response.End();
        }
    }

    private string getData()
    {
        var startTm = Request.Form["startTm"];
        var endTm = Request.Form["endTm"];
        var departmentId = Request.Form["departmentId"];
        var userInfo = (UserInfo)Session["user"];

        // 获取所有下级部门
        string sql = string.Format("select REVERSE(LEFT(REVERSE(name),INSTR(REVERSE(name),'/')-1)) name,id, name originName from department where parentId = '{0}'", departmentId);

        if (departmentId == "406")
        {
            sql = string.Format("select REVERSE(LEFT(REVERSE(name),INSTR(REVERSE(name),'/')-1)) name,id, name originName " +
            "from department where id = 497 or id = 408");
        }
        else if (departmentId == "407")
        {
            sql = string.Format("select REVERSE(LEFT(REVERSE(name),INSTR(REVERSE(name),'/')-1)) name,id, name originName " +
            "from department where id = 499 or id = 414");
        }

        var subDepartmentDt = SqlHelper.Find(sql).Tables[0];

        var list = new List<Dictionary<string, object>>();
        foreach (DataRow dr in subDepartmentDt.Rows)
        {
            var dict = getDepartmentData(dr["originName"].ToString(), startTm, endTm);
            dict.Add("name", dr[0].ToString());

            list.Add(dict);
        }

        return JsonHelper.SerializeObject(list);

        //var departmentData = getDepartmentData(departmentName, startTm, endTm);

        //// 获取所有下级部门
        //var subDepartmentDt = SqlHelper.Find(string.Format("select name from department where parentName = '{0}'", departmentName)).Tables[0];

        //var subDepartmentDictList = new List<Dictionary<string, object>>();
        //var subDepartmentTimeDictList = new List<Dictionary<string, object>>();
        //var timeList = new List<string>();

        //foreach (DataRow subDepartmentDr in subDepartmentDt.Rows)
        //{
        //    var subDepartmentName = subDepartmentDr[0].ToString();
        //    var tempDict = getDepartmentData(subDepartmentName, startTm, endTm);

        //    double temp_overcome = double.Parse(tempDict["overcome"].ToString());
        //    double temp_cost = double.Parse(tempDict["cost"].ToString());
        //    double temp_reimburse = double.Parse(tempDict["reimburse"].ToString());
        //    double temp_other_fee = double.Parse(tempDict["other_fee"].ToString());
        //    double temp_profit = double.Parse(tempDict["profit"].ToString());

        //    var subDepartmentDict = new Dictionary<string, object>
        //    {
        //        { "name", subDepartmentName.Substring(subDepartmentName.LastIndexOf("/") + 1) },
        //        { "value", temp_overcome - temp_cost - temp_reimburse - temp_other_fee },
        //        { "overcome", temp_overcome},
        //        { "cost", temp_cost},
        //        { "reimburse", temp_reimburse},
        //        { "other_fee", temp_other_fee},
        //        { "profit", temp_profit}
        //    };

        //    subDepartmentDictList.Add(subDepartmentDict);

        //    // 判断时间间隔 如果小于5天 则往前推两天
        //    var subDepartmentTimeDict = new Dictionary<string, object>
        //    {
        //        { "name", subDepartmentName.Substring(subDepartmentName.LastIndexOf("/") + 1)}
        //    };
        //    var dataList = new List<double>();
        //    int interval_time = Convert.ToDateTime(endTm).Subtract(Convert.ToDateTime(startTm)).Days;
        //    if (interval_time < 5)
        //    {
        //        DateTime tempTm = Convert.ToDateTime(startTm).AddDays(interval_time - 5);

        //        while (tempTm <= Convert.ToDateTime(endTm))
        //        {
        //            if (!timeList.Contains(tempTm.ToString("d")))
        //                timeList.Add(tempTm.ToString("d"));
        //            var tempTimeDict = getDepartmentData(subDepartmentName, tempTm.ToString("d"), tempTm.ToString("d"));

        //            double temp_time_overcome = Math.Round(double.Parse(tempTimeDict["overcome"].ToString()) / 10000, 3);
        //            double temp_time_cost = Math.Round(double.Parse(tempTimeDict["cost"].ToString()) / 10000, 3);
        //            double temp_time_reimburse = Math.Round(double.Parse(tempTimeDict["reimburse"].ToString()) / 10000, 3);
        //            double temp_time_other_fee = Math.Round(double.Parse(tempTimeDict["other_fee"].ToString()) / 10000, 3);

        //            dataList.Add(temp_time_overcome- temp_time_cost-temp_time_reimburse-temp_time_other_fee);

        //            tempTm = tempTm.AddDays(1);
        //        }
        //    }
        //    else if (Convert.ToDateTime(endTm).Subtract(Convert.ToDateTime(startTm)).Days >= 30)
        //    {

        //    }
        //    else
        //    {
        //        DateTime tempTm = Convert.ToDateTime(startTm);
        //        while (tempTm <= Convert.ToDateTime(endTm))
        //        {
        //            if (!timeList.Contains(tempTm.ToString("d")))
        //                timeList.Add(tempTm.ToString("d"));
        //            var tempTimeDict = getDepartmentData(subDepartmentName, tempTm.ToString("d"), tempTm.ToString("d"));
        //            double temp_time_overcome = Math.Round(double.Parse(tempTimeDict["overcome"].ToString()) / 10000,3);
        //            double temp_time_cost = Math.Round(double.Parse(tempTimeDict["cost"].ToString()) / 10000, 3);
        //            double temp_time_reimburse = Math.Round(double.Parse(tempTimeDict["reimburse"].ToString()) / 10000, 3);
        //            double temp_time_other_fee = Math.Round(double.Parse(tempTimeDict["other_fee"].ToString()) / 10000, 3);
        //            dataList.Add(temp_time_overcome - temp_time_cost - temp_time_reimburse - temp_time_other_fee);

        //            tempTm = tempTm.AddDays(1);
        //        }
        //    }
        //    subDepartmentTimeDict.Add("data", dataList);
        //    subDepartmentTimeDict.Add("type", "line");
        //    subDepartmentTimeDictList.Add(subDepartmentTimeDict);
        //}

        //Dictionary<string, object> result = new Dictionary<string, object>
        //{
        //    { "overcome", departmentData["overcome"]},
        //    { "cost", departmentData["cost"]},
        //    { "reimburse", departmentData["reimburse"]},
        //    { "other_fee", departmentData["other_fee"]},
        //    { "subDepartmentDictList", subDepartmentDictList},
        //    { "subDepartmentTimeDictList", subDepartmentTimeDictList},
        //    { "timeList", timeList}
        //};

        //return JsonHelper.SerializeObject(result);
    }

    /// <summary>
    /// 查询一级子部门
    /// </summary>
    /// <returns>搜索结果</returns>
    private string getDepartment()
    {
        var key = Request.Form["key"].ToString();
        var startTm = Request.Form["startTm"].ToString();

        var sql = string.Format("select id,parentId, departmentId name from yl_reimburse_other where parentId = {0} and MONTH(createTime) = MONTH('{1}')", key, startTm);

        //if (key == "406")
        //{
        //    sql = string.Format("select id,parentId,REVERSE(LEFT(REVERSE(parentName),INSTR(REVERSE(parentName),'/')-1)) parentName, REVERSE(LEFT(REVERSE(name),INSTR(REVERSE(name),'/')-1)) name " +
        //    "from department where id = 497 or id = 408", key);
        //}
        //else if (key == "407")
        //{
        //    sql = string.Format("select id,parentId,REVERSE(LEFT(REVERSE(parentName),INSTR(REVERSE(parentName),'/')-1)) parentName, REVERSE(LEFT(REVERSE(name),INSTR(REVERSE(name),'/')-1)) name " +
        //    "from department where id = 499 or id = 414", key);
        //}

        var dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string getParentDepartment()
    {
        var key = Request.Form["key"].ToString();
        var sql = string.Format("select id,parentId,parentName, REVERSE(LEFT(REVERSE(name),INSTR(REVERSE(name),'/')-1)) name " +
            "from department where parentId = (select parentId from department where id = {0})", key);

        var dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private Dictionary<string, object> getDepartmentData(string departmentName, string startTm, string endTm)
    {
        var sqls = new List<string>();

        int year = int.Parse(startTm.Substring(0, 4));
        int month = int.Parse(endTm.Substring(5, endTm.IndexOf("-", 5) - 5));

        // 移动报销
        var sql = string.Format("select ifnull(sum(t1.fee_amount), 0) from yl_reimburse t1 " +
            "where t1.status = '已审批' and t1.approvalResult = '同意' and t1.fee_department like '{2}%' and t1.apply_time between '{0}' and '{1}'", startTm, endTm, departmentName);
        sqls.Add(sql);

        if (departmentName.Contains("销售部") || departmentName.Contains("集团营销中心"))
        {
            // 收入
            sql = string.Format("select ifnull(sum(distinct t1.salesnumber * t4.assessmentPrice), 0) 收入 from sales_daily_report t1" +
                " left join new_product t2 on t1.code = t2.productCode" +
                " left join new_client t3 on t1.hospitalCode = t3.clientCode" +
                " left join new_client_product_users t4 on t2.productCode = t4.productCode and t3.clientCode = t4.clientCode" +
                " left join department t5 on t4.departmentId = t5.id" +
                " where t5.name like '{0}%' and t1.date between '{1}' and '{2}'", departmentName, startTm, endTm);
            sqls.Add(sql);

            // 成本
            sql = string.Format("select ifnull(sum(t1.salesnumber * t3.innerPrice),0) 成本 from sales_daily_report t1 " +
                "left join new_product t2 on t1.code = t2.productCode " +
                "left join inner_price t3 on t2.oldProductType = t3.productType " +
                "where t2.type <> '配件' and '{0}' like concat(t3.departmentName, '%') and t1.date between '{1}' and '{2}'", departmentName, startTm, endTm);
            sqls.Add(sql);

            // todo 需要加入中申的发货申请
        }
        else if (departmentName.Contains("研发") || departmentName.Contains("天津吉诺泰普"))
        {
            // todo 需要添加政府项目补贴 + 技术委托开发
            sql = string.Format("select 0 收入");
            sqls.Add(sql);

            sql = string.Format("select 0 成本");
            sqls.Add(sql);
        }
        else if (departmentName.Contains("生产部") || departmentName.Contains("业力"))
        {
            sql = string.Format("select ifnull(sum(t1.salesnumber * t3.innerPrice),0) 收入 from sales_daily_report t1 " +
                "left join new_product t2 on t1.code = t2.productCode " +
                "left join inner_price t3 on t2.oldProductType = t3.productType " +
                "where t2.type <> '配件' and t2.factory = '业力' and t1.date between '{0}' and '{1}'"
                , startTm, endTm);
            sqls.Add(sql);

            sql = string.Format("select ifnull(sum(t1.salesnumber * t3.cost),0) 成本 from sales_daily_report t1 " +
                "left join new_product t2 on t1.code = t2.productCode " +
                "left join new_product_cost t3 on t2.ProductName like concat(t3.productName, '%') " +
                "where t2.type <> '配件' and t2.factory = '业力' and t1.date between '{0}' and '{1}'", startTm, endTm);
            sqls.Add(sql);
        }
        else if (departmentName.Contains("生产质量部") || departmentName.Contains("中申"))
        {
            sql = string.Format("select ifnull(sum(t1.salesnumber * t3.innerPrice),0) 收入 from sales_daily_report t1 " +
                "left join new_product t2 on t1.code = t2.productCode " +
                "left join inner_price t3 on t2.oldProductType = t3.productType " +
                "where t2.type <> '配件' and t2.factory = '中申' and t1.date between '{0}' and '{1}'"
                , startTm, endTm);
            sqls.Add(sql);

            sql = string.Format("select ifnull(sum(t1.salesnumber * t3.cost),0) 成本 from sales_daily_report t1 " +
                "left join new_product t2 on t1.code = t2.productCode " +
                "left join new_product_cost t3 on t2.ProductName like concat(t3.productName, '%') " +
                "where t2.type <> '配件' and t2.factory = '中申' and t1.date between '{0}' and '{1}'", startTm, endTm);
            sqls.Add(sql);
        }
        else if (departmentName.Contains("制造中心"))
        {
            sql = string.Format("select ifnull(sum(t1.salesnumber * t3.innerPrice),0) 收入 from sales_daily_report t1 " +
                "left join new_product t2 on t1.code = t2.productCode " +
                "left join inner_price t3 on t2.oldProductType = t3.productType " +
                "where t2.type <> '配件' and (t2.factory = '业力' or t2.factory = '中申') and t1.date between '{0}' and '{1}'"
                , startTm, endTm);
            sqls.Add(sql);

            sql = string.Format("select ifnull(sum(t1.salesnumber * t3.cost),0) 成本 from sales_daily_report t1 " +
                "left join new_product t2 on t1.code = t2.productCode " +
                "left join new_product_cost t3 on t2.ProductName like concat(t3.productName, '%') " +
                "where t2.type <> '配件' and (t2.factory = '业力' or t2.factory = '中申') and t1.date between '{0}' and '{1}'", startTm, endTm);
            sqls.Add(sql);
        }
        else
        {
            // 收入和成本
            sql = string.Format("select 0 收入");
            sqls.Add(sql);

            sql = string.Format("select 0 成本");
            sqls.Add(sql);
        }

        // 其他费用
        if (departmentName.Contains("销售部") || departmentName.Contains("集团营销中心"))
        {
            sql = string.Format("select t1.* from yl_reimburse_other t1 where t1.departmentId like '纯销%' or t1.departmentId like '渠道%' and MONTH(t1.createTime) = MONTH('{1}')", departmentName, month);
        }
        else if (departmentName.Contains("集团制造中心"))
        {
            sql = string.Format("select t1.* from yl_reimburse_other t1 where t1.departmentId like '%中申%' or t1.departmentId like '%业力%' or t1.departmentId like '%老康%' or t1.departmentId like '天津%' and MONTH(t1.createTime) = MONTH('{1}')", departmentName, month);
        }
        else
        {
            sql = string.Format("select t1.* from yl_reimburse_other t1 where t1.departmentId like '{0}' and MONTH(t1.createTime) = MONTH('{1}')", departmentName, month);
        }

        sqls.Add(sql);

        DataSet ds = SqlHelper.Find(sqls.ToArray());

        DataTable reimburseDt = ds.Tables[0];
        DataTable overcomeDt = ds.Tables[1];
        DataTable costDt = ds.Tables[2];
        DataTable otherDt = ds.Tables[3];

        double other_fee = 0.0;

        foreach (DataRow dr in otherDt.Rows)
        {
            string startDate = dr["startDate"].ToString();
            string endDate = dr["endDate"].ToString();

            if (dr["type"].ToString() == "0")
            {
                if (string.IsNullOrEmpty(startTm) && string.IsNullOrEmpty(endTm))
                {
                    if (startDate == DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        other_fee += float.Parse(dr["feeAmount"].ToString());
                    }
                }
                else
                {
                    if (Convert.ToDateTime(startDate) >= Convert.ToDateTime(startTm) && Convert.ToDateTime(startDate) <= Convert.ToDateTime(endTm))
                    {
                        other_fee += float.Parse(dr["feeAmount"].ToString());
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(startTm) && string.IsNullOrEmpty(endTm))
                {
                    if (Convert.ToDateTime(startDate) <= DateTime.Now && Convert.ToDateTime(endDate) >= DateTime.Now)
                    {
                        int minusDays = Convert.ToDateTime(endDate).Subtract(Convert.ToDateTime(startDate)).Days;
                        other_fee += float.Parse(dr["feeAmount"].ToString()) / minusDays;
                    }
                }
                else if (Convert.ToDateTime(startDate) > Convert.ToDateTime(endTm) || Convert.ToDateTime(endDate) < Convert.ToDateTime(startTm))
                {
                    other_fee = 0;
                }
                else
                {
                    if (Convert.ToDateTime(startDate) <= Convert.ToDateTime(startTm) && Convert.ToDateTime(endDate) >= Convert.ToDateTime(endTm))
                    {
                        int minusDays = Convert.ToDateTime(endDate).Subtract(Convert.ToDateTime(startDate)).Days + 1;
                        int _minusDays = Convert.ToDateTime(endTm).Subtract(Convert.ToDateTime(startTm)).Days + 1;
                        other_fee += float.Parse(dr["feeAmount"].ToString()) / minusDays * _minusDays;
                    }
                    else if (Convert.ToDateTime(startDate) <= Convert.ToDateTime(startTm) && Convert.ToDateTime(endDate) <= Convert.ToDateTime(endTm))
                    {
                        int minusDays = Convert.ToDateTime(endDate).Subtract(Convert.ToDateTime(startDate)).Days + 1;
                        int _minusDays = Convert.ToDateTime(endDate).Subtract(Convert.ToDateTime(startTm)).Days + 1;
                        other_fee += float.Parse(dr["feeAmount"].ToString()) / minusDays * _minusDays;
                    }
                    else if (Convert.ToDateTime(startDate) >= Convert.ToDateTime(startTm) && Convert.ToDateTime(endDate) >= Convert.ToDateTime(endTm))
                    {
                        int minusDays = Convert.ToDateTime(endDate).Subtract(Convert.ToDateTime(startDate)).Days + 1;
                        int _minusDays = Convert.ToDateTime(endTm).Subtract(Convert.ToDateTime(startDate)).Days + 1;
                        other_fee += float.Parse(dr["feeAmount"].ToString()) / minusDays * _minusDays;
                    }
                }
            }
        }

        double overcome = double.Parse(overcomeDt.Rows[0][0].ToString());
        double cost = double.Parse(costDt.Rows[0][0].ToString()); ;
        double reimburse = double.Parse(reimburseDt.Rows[0][0].ToString());

        return new Dictionary<string, object>()
        {
            { "overcome", Math.Round(overcome/10000, 3)},
            { "cost", Math.Round(cost/10000, 3)},
            { "reimburse", Math.Round(reimburse/10000,3)},
            { "other_fee", Math.Round(other_fee/10000, 3)},
            { "profit", Math.Round((overcome - cost - reimburse - other_fee)/10000, 3)}
        };
    }
}