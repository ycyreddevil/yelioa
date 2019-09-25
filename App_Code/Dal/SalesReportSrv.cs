using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;

/// <summary>
/// SalesReportSrv 的摘要说明
/// </summary>
public class SalesReportSrv
{
    public SalesReportSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetDataFaster(int year, int month, string todayStr)
    {
        List<string> list = new List<string>();
        
        // 0 根据盈利中心查询出当月任务和全年任务
        string sql = string.Format("select t1.sector Sector,case when month{1} is null then 0 else month{1} end monthTask, " +
            "case when yearTask is null then 0 else yearTask end yearTask from sector_task t1 " +
            "where t1.year = {0}", year, month);
        list.Add(sql);

        // 1 查询当月的销售数据
        string startTm = year + "-" + month + "-1";
        string endTm = year + "-" + (month + 1) + "-1";
        //sql = string.Format("select case (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
        //                "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector = '{2}') when 0 " +
        //                "then (select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end " +
        //                "from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date >= '{3}' " +
        //                "AND date < '{4}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice," +
        //                "Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
        //                "where cs.Sector = '{2}') else (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
        //                "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector = '{2}') end monthFlow", year, month, sector, startTm, endTm);
        sql = string.Format("select fs.Sector,case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs " +
                        "where fs.Year = {0} and fs.Month = {1} group by Sector", year, month);
        list.Add(sql);

        // 2 查询全年的销售数据
        sql = string.Format("select fs.Sector,sum(fs.FlowSalesMoney) sumSales from flow_statistics fs " +
            "where fs.Year = {0} and fs.Month <= {1} group by Sector", year, month);
        list.Add(sql);

        // 3 查询当天的销售数据
        sql = string.Format("select sector Sector,case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end todayFlow " +
            "from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date = '{0}' GROUP BY ProductId, " +
            "terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing GROUP BY ProductId, " +
            "HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId group by sector", todayStr);
        list.Add(sql);

        // 4 查询当月预付款
        sql = string.Format("select t1.sector Sector, case when t1.prepaidMoney is null then 0 else t1.prepaidMoney end monthPrepaid from prepaid t1 where t1.year = {0} and t1.month = {1}", year, month);
        list.Add(sql);

        // 5 查询全年预付款
        sql = string.Format("select t1.sector Sector, case when t1.sumPrepaidMoney is null then 0 else t1.sumPrepaidMoney end yearPrepaid from prepaid t1 where t1.year = {0}", year);
        list.Add(sql);
        
        return SqlHelper.Find(list.ToArray());
    }

    public static DataSet getSector()
    {
        string sql = string.Format("select distinct sector from cost_sharing");

        return SqlHelper.Find(sql);
    }

    public static DataSet GetCenterDataFaster(int year, int month, string todayStr)
    {
        List<string> list = new List<string>();
        //0统计东森年度任务及月度任务
        string sql = string.Format("SELECT SUM(YearTask) AS yearTask,"
                   + "SUM(month{0}) AS monthTask FROM sector_task where year={1} and sector !='业力组' and sector != '中申组'", month, year);
        list.Add(sql);
        //DataSet ds = SqlHelper.Find(sql);
        //if (ds == null || ds.Tables[0].Rows.Count == 0)
        //    return null;

        //1查询当月flow_statistics表数据
        sql = string.Format("select * from flow_statistics where Year={0} and Month={1}"
            , year, month);
        list.Add(sql);
        //2查询当月出库数据
        sql = string.Format("select * from leave_stock where `date` BETWEEN '{0}-{1}-1' AND '{0}-{1}-31'"
            , year,month);
        list.Add(sql);
        //3查询东森全年已产生的流向数量及流向金额
        sql = string.Format("select SUM(FlowSalesMoney) AS yearFlowSalesMoney, "
            + "SUM(FlowSales) as yearFlowSales from flow_statistics "
            + "where (Sector not like '%中申%' and Sector not like '%业力%') and "
            + "(Year = {0} and Month<={1})",year,month);
        list.Add(sql);

        //4查询业力全年已产生的流向数量及流向金额
        sql = string.Format("select SUM(FlowSalesMoney) AS yearFlowSalesMoney, "
            + "SUM(FlowSales) as yearFlowSales from flow_statistics "
            + "where Sector like '%业力%' and "
            + "(Year = {0} and Month<={1})", year, month);
        list.Add(sql);

        //5查询中申全年已产生的流向数量及流向金额
        sql = string.Format("select SUM(FlowSalesMoney) AS yearFlowSalesMoney, "
            + "SUM(FlowSales) as yearFlowSales from flow_statistics "
            + "where Sector like '%中申%' and "
            + "(Year = {0} and Month<={1})", year, month);
        list.Add(sql);

        //6查询网点信息
        sql = "select * from v_outlet";
        list.Add(sql);

        //7查询东森当天的流向
        sql = string.Format("select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end todayFlow from " +
            "(select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date = '{0}' GROUP BY " +
            "ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing " +
            "GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
            "where cs.Sector != '业力组' and cs.Sector != '中申组'", todayStr);
        list.Add(sql);

        //8查询业力当天的流向
        sql = string.Format("select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end todayFlow from " +
            "(select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date = '{0}' GROUP BY " +
            "ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing " +
            "GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
            "where cs.Sector like '%业力%'", todayStr);
        list.Add(sql);

        //9查询中申当天的流向
        sql = string.Format("select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end todayFlow from " +
            "(select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date = '{0}' GROUP BY " +
            "ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing " +
            "GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
            "where cs.Sector like '%中申%'", todayStr);
        list.Add(sql);

        // 10 查询东森月度以及全年预付款
        sql = string.Format("select (select sum(prepaidMoney) from prepaid where year = {0} and month = {1} and sector != '中申组' and sector != '业力组') dsMonthPrepaid," +
            "(select sumPrepaidMoney from prepaid where year = {0} and month = {1} and sector != '中申组' and sector != '业力组') dsYearPrepaid", year, month);
        list.Add(sql);

        // 11 查询业力月度以及全年预付款
        sql = string.Format("select (select sum(prepaidMoney) from prepaid where year = {0} and month = {1} and sector = '业力组') ylMonthPrepaid," +
           "(select sumPrepaidMoney from prepaid where year = {0} and month = {1} and sector = '业力组') ylYearPrepaid", year, month);
        list.Add(sql);

        // 12 查询中申月度以及全年预付款
        sql = string.Format("select (select sum(prepaidMoney) from prepaid where year = {0} and month = {1} and sector = '中申组') zsMonthPrepaid," +
           "(select sumPrepaidMoney from prepaid where year = {0} and month = {1} and sector = '中申组') zsYearPrepaid", year, month);
        list.Add(sql);

        //13统计业力年度任务及月度任务
        //14统计中申年度任务及月度任务
        sql = string.Format("SELECT SUM(YearTask) AS ylYearTask,"
                + "SUM(month{0}) AS ylMonthTask FROM sector_task where year={1} and sector ='业力组'", month, year);
        list.Add(sql);

        sql = string.Format("SELECT SUM(YearTask) AS zsYearTask,"
                + "SUM(month{0}) AS zsMonthTask FROM sector_task where year={1} and sector = '中申组'", month, year);
        list.Add(sql);

        return SqlHelper.Find(list.ToArray());
    }

    public static DataSet GetData(int year, int month, string todayStr)
    {
        string startTm = year + "-" + month + "-1";
        string endTm = year + "-" + (month + 1) + "-1";
        //string sql = string.Format("select sc.Sector,case when sum(st.MonthTask{1}*st.ExaminePrice) is null then 0 else sum(st.MonthTask{1}*st.ExaminePrice) end monthTask, case when sum(st.YearTask*st.ExaminePrice) is null then 0 else sum(st.YearTask*st.ExaminePrice) end yearTask,0 monthPrepaid,0 sumPrepaid, case when sc.sector like '%中申%' then '中申' when sc.sector like '%业力%' then '业力' else '东森' end center, " +
        //"(select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date = '{4}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId where cs.Sector = sc.Sector) todayFlow, " +
        //"(select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date >= '{2}' AND date < '{3}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId where cs.Sector = sc.Sector) monthFlow, " +
        //"(select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId where cs.Sector = sc.Sector) sumSales, " +
        //"((select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date >= '{2}' AND date < '{3}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId where cs.Sector = sc.Sector) /sum(st.MonthTask{1}*st.ExaminePrice)) monthCompleteRate," +
        //"((select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId where cs.Sector = sc.Sector)/sum(st.YearTask*st.ExaminePrice)) sumCompleteRate " +
        //" from sales_task st right join sector_corresponding sc on sc.sector = st.sector group by sc.Sector order by FIELD(center,'东森','业力','中申'), monthCompleteRate desc, sumCompleteRate desc ", year, month, startTm, endTm, todayStr);
        string sql = string.Format("select sc.Sector,case when sum(st.MonthTask{1}*st.ExaminePrice) " +
            "is null then 0 else sum(st.MonthTask{1}*st.ExaminePrice) end monthTask, case when sum(st.YearTask*st.ExaminePrice)" +
            " is null then 0 else sum(st.YearTask*st.ExaminePrice) end yearTask,0 monthPrepaid," +
            "(select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date = '{4}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId where cs.Sector = sc.Sector) todayFlow, " +
            "0 sumPrepaid, case when sc.sector like '%中申%' then '中申' when sc.sector like '%业力%' then '业力' else '东森' end center " +
            " from sales_task st right join (select distinct sector from cost_sharing) sc on sc.sector = st.sector " +
            "group by sc.Sector order by FIELD(center,'东森','业力','中申') desc ", year, month, startTm, endTm, todayStr);
        // 先算出各个盈利中心的年度任务
        DataSet ds = SqlHelper.Find(sql);
        if (ds != null && ds.Tables[0] != null)
        {
            DataTable dt = ds.Tables[0];

            // 给dataTable新增月销售，年销售,月完成率，年完成率的字段
            dt.Columns.Add("monthFlow", typeof(float));
            dt.Columns.Add("sumSales", typeof(float));
            dt.Columns.Add("monthCompleteRate", typeof(float));
            dt.Columns.Add("sumCompleteRate", typeof(float));

            foreach (DataRow dr in dt.Rows)
            {
                if (dr != null)
                {
                    string sector = dr["Sector"].ToString();
                    // 通过每个盈利中心得出当月以及年销售量
                    // 1.得出当月的
                    sql = string.Format("select case (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector = '{2}') when 0 " +
                        "then (select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end " +
                        "from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date >= '{3}' " +
                        "AND date < '{4}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice," +
                        "Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
                        "where cs.Sector = '{2}') else (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector = '{2}') end monthFlow", year, month, sector, startTm, endTm);
                    DataSet monthFlowDs = SqlHelper.Find(sql);
                    if (monthFlowDs != null)
                    {
                        string monthFlowStr = monthFlowDs.Tables[0].Rows[0]["monthFlow"].ToString();
                        // 当月流向已归档
                        float monthFlow = float.Parse(monthFlowStr);
                        dr["monthFlow"] = monthFlow;

                        //再算出当月完成率
                        float monthCompleteRate = 0;
                        string monthTaskStr = dr["monthTask"].ToString();
                        if ("0".Equals(monthTaskStr) && "0".Equals(monthFlowStr))
                        {
                            monthCompleteRate = 1;
                        }
                        else if (!"0".Equals(monthTaskStr))
                        {
                            monthCompleteRate = monthFlow / float.Parse(monthTaskStr);
                        }

                        dr["monthCompleteRate"] = monthCompleteRate;
                    }

                    // 查询当年的
                    
                    //DataSet sumDataSet = SqlHelper.Find(sql);
                    List<MySqlParameter> ListParam = new List<MySqlParameter>();

                    MySqlParameter p1 = new MySqlParameter("year1", year);
                    MySqlParameter p2 = new MySqlParameter("month1", month);
                    MySqlParameter p3 = new MySqlParameter("sector1", sector);
                    MySqlParameter p4 = new MySqlParameter("sumSalessss", 0);
                    ListParam.Add(p1); ListParam.Add(p2); ListParam.Add(p3); ListParam.Add(p4);

                    cProcedure procedure = new cProcedure("proc3", ListParam);

                    cProcedure[] list = new cProcedure[] { procedure };

                    int[] i = SqlHelper.RunProcedure(list);

                    if (i.Length <= 0)
                    {
                        return null;
                    }

                    string sumSalesStr = p4.Value.ToString();
                    float sumSales = float.Parse(sumSalesStr);
                    //for (int i = 1; i <= month; i++)
                    //{
                    //    startTm = year + "-" + i + "-1";
                    //    endTm = year + "-" + (i + 1) + "-1";
                    //    sql = string.Format("select case (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                    //    "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector = '{2}') when 0 " +
                    //    "then (select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end " +
                    //    "from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date >= '{3}' " +
                    //    "AND date < '{4}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice," +
                    //    "Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
                    //    "where cs.Sector = '{2}') else (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                    //    "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector = '{2}') end monthFlow", year, i, sector, startTm, endTm);
                    //    monthFlowDs = SqlHelper.Find(sql);
                    //    if (monthFlowDs != null)
                    //    {
                    //        string monthFlowStr = monthFlowDs.Tables[0].Rows[0]["monthFlow"].ToString();
                    //        // 当月流向已归档
                    //        float monthFlow = float.Parse(monthFlowStr);

                    //        yearFlow += monthFlow;
                    //    }
                    //}
                    dr["sumSales"] = sumSales;
                    //再算出当年完成率
                    float sumCompleteRate = 0;
                    string yearTaskStr = dr["yearTask"].ToString();
                    if ("0".Equals(yearTaskStr))
                    {
                        sumCompleteRate = 1;
                    }
                    else
                    {
                        sumCompleteRate = sumSales / float.Parse(yearTaskStr);
                    }

                    dr["sumCompleteRate"] = sumCompleteRate;
                }
            }
        }
        else
        {
            return null;
        }
        return ds;
    }

    public static DataSet GetCenterData(int year, int month, string todayStr)
    {
        string startTm = year + "-" + month + "-1";
        string endTm = year + "-" + (month + 1) + "-1";
        string sql = string.Format("select center, case when sum(t.monthTask) is null then 0 else sum(t.monthTask) end monthTask, case when sum(t.yearTask) is null then 0 else sum(t.yearTask) end yearTask, sum(t.monthPrepaid) monthPrepaid ,sum(t.sumPrepaid) sumPrepaid, sum(t.todayFlow) todayFlow from ( " +
        "select sc.Sector,case when sum(st.MonthTask{1}*st.ExaminePrice) is null then 0 else sum(st.MonthTask{1}*st.ExaminePrice) end monthTask, case when sum(st.YearTask*st.ExaminePrice) is null then 0 else sum(st.YearTask*st.ExaminePrice) end yearTask,0 monthPrepaid,0 sumPrepaid, case when sc.sector like '%中申%' then '中申' when sc.sector like '%业力%' then '业力' else '东森' end center, " +
        "(select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date = '{4}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice,Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId where cs.Sector = sc.Sector) todayFlow " +
        " from sales_task st right join (select distinct sector from cost_sharing) sc on sc.sector = st.sector group by sc.Sector ) t group by center order by FIELD(center,'东森','业力','中申')", year, month, startTm, endTm, todayStr);

        DataSet ds = SqlHelper.Find(sql);
        if (ds != null && ds.Tables[0] != null)
        {
            DataTable dt = ds.Tables[0];

            // 给dataTable新增月销售，年销售,月完成率，年完成率的字段
            dt.Columns.Add("monthFlow", typeof(float));
            dt.Columns.Add("sumSales", typeof(float));
            dt.Columns.Add("monthCompleteRate", typeof(float));
            dt.Columns.Add("sumCompleteRate", typeof(float));

            foreach (DataRow dr in dt.Rows)
            {
                if (dr != null)
                {
                    string center = dr["center"].ToString();

                    // 通过每个公司得出当月以及年销售量
                    // 1.得出当月的
                    if (center.Equals("东森"))
                    {
                        sql = string.Format("select case (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector != '业力组' and Sector != '中申组') when 0 " +
                        "then (select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end " +
                        "from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date >= '{2}' " +
                        "AND date < '{3}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice," +
                        "Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
                        "where cs.Sector != '业力组' and cs.Sector != '中申组') else (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector != '业力组' and Sector != '中申组') end monthFlow", year, month, startTm, endTm);
                    }
                    else
                    {
                        sql = string.Format("select case (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector like '%{2}%') when 0 " +
                        "then (select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end " +
                        "from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date >= '{3}' " +
                        "AND date < '{4}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice," +
                        "Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
                        "where cs.Sector like '%{2}%') else (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector like '%{2}%') end monthFlow", year, month, center, startTm, endTm);
                    }
                   
                    DataSet monthFlowDs = SqlHelper.Find(sql);
                    if (monthFlowDs != null)
                    {
                        string monthFlowStr = monthFlowDs.Tables[0].Rows[0]["monthFlow"].ToString();

                        float monthFlow = float.Parse(monthFlowStr);
                        dr["monthFlow"] = monthFlow;

                        //再算出当月完成率
                        float monthCompleteRate = 0;
                        string monthTaskStr = dr["monthTask"].ToString();
                        if ("0".Equals(monthTaskStr) && "0".Equals(monthFlowStr))
                        {
                            monthCompleteRate = 1;
                        }
                        else if (!"0".Equals(monthTaskStr))
                        {
                            monthCompleteRate = monthFlow / float.Parse(monthTaskStr);
                        }

                        dr["monthCompleteRate"] = monthCompleteRate;
                    }
                    float yearFlow = 0;
                    List<MySqlParameter> ListParam = new List<MySqlParameter>();

                    MySqlParameter p1 = new MySqlParameter("year1", year);
                    MySqlParameter p2 = new MySqlParameter("month1", month);
                    MySqlParameter p3 = new MySqlParameter("sector1", center);
                    MySqlParameter p4 = new MySqlParameter("sumSalessss", 0);
                    ListParam.Add(p1); ListParam.Add(p2); ListParam.Add(p3); ListParam.Add(p4);
                    if (center.Equals("东森"))
                    {
                        cProcedure procedure = new cProcedure("proc", ListParam);

                        cProcedure[] list = new cProcedure[] { procedure };

                        int[] i = SqlHelper.RunProcedure(list);

                        if (i.Length <= 0)
                        {
                            return null;
                        }

                        string sumSalesStr = p4.Value.ToString();
                        yearFlow = float.Parse(sumSalesStr);
                    }
                    else
                    {
                        cProcedure procedure = new cProcedure("proc2", ListParam);

                        cProcedure[] list = new cProcedure[] { procedure };

                        int[] i = SqlHelper.RunProcedure(list);

                        if (i.Length <= 0)
                        {
                            return null;
                        }

                        string sumSalesStr = p4.Value.ToString();
                        yearFlow = float.Parse(sumSalesStr);
                    }

                    
                    dr["sumSales"] = yearFlow;
                    //再算出当年完成率
                    float sumCompleteRate = 0;
                    string yearTaskStr = dr["yearTask"].ToString();
                    if ("0".Equals(yearTaskStr))
                    {
                        sumCompleteRate = 1;
                    }
                    else
                    {
                        sumCompleteRate = yearFlow / float.Parse(yearTaskStr);
                    }

                    dr["sumCompleteRate"] = sumCompleteRate;
                }
            }
        }
        else
        {
            return null;
        }

        return ds;
    }

    public static DataSet getSectorMoneyMonthly(int year, string sector)
    {
        List<string> list = new List<string>();

        for (int i = 1; i <= 12; i ++)
        {
            string startTm = year + "-" + i + "-1";
            string endTm = year + "-" + (i + 1) + "-1";

            string sql = string.Format("select case (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector = '{2}') when 0 " +
                        "then (select case when sum(ls.amountSend) is null then 0 else sum(ls.amountSend*cs.ExaminePrice) end " +
                        "from (select ProductId,terminalClientId,sum(amountSend) amountSend from leave_stock where date >= '{3}' " +
                        "AND date < '{4}' GROUP BY ProductId, terminalClientId) ls right join (select ProductId,HospitalId,ExaminePrice," +
                        "Sector from cost_sharing GROUP BY ProductId, HospitalId) cs on ls.ProductId = cs.ProductId and ls.terminalClientId = cs.HospitalId " +
                        "where cs.Sector = '{2}') else (select case when sum(fs.FlowSalesMoney) is null then 0 else sum(fs.FlowSalesMoney) " +
                        "end monthFlow from flow_statistics fs where Year = {0} and Month = {1} and Sector = '{2}') end monthFlow", year, i, sector, startTm, endTm);
            list.Add(sql);
        }

        return SqlHelper.Find(list.ToArray());
    }

    public static DataSet getSalesmanCompleteRate(int year, int month, UserInfo user)
    {
        string sql = "select Sales,GROUP_CONCAT(DISTINCT CONCAT(Sector)) Sector,sum(MonthTask) monthTask,sum(NetSalesMoney) netSalesMoney from flow_statistics " +
            "where year = {0} and month = {1} and Sales != 'KG'";
        if (!Privilege.checkPrivilege(user))
        {
            sql += " and (Sales = '{2}' or Supervisor = '{2}' or Manager = '{2}' or Director = '{2}') ";
        }
        sql += " group by sales";

        sql = string.Format(sql, year, month, user.userName);
        return SqlHelper.Find(sql);
    }
}