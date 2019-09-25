using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// SalesTaskInfoManage 的摘要说明
/// </summary>
public class SalesReportManage
{
    public SalesReportManage()
    {

    }

    public static DataTable getData(int year, int month, string todayStr)
    {
        DataSet ds = SalesReportSrv.GetDataFaster(year, month, todayStr);

        if (ds != null)
        {
            DataTable resultDt = new DataTable();

            resultDt.Columns.Add("monthFlow", typeof(float));
            resultDt.Columns.Add("sumSales", typeof(float));
            resultDt.Columns.Add("todayFlow", typeof(float));
            resultDt.Columns.Add("monthCompleteRate", typeof(float));
            resultDt.Columns.Add("sumCompleteRate", typeof(float));
            resultDt.Columns.Add("monthTask", typeof(float));
            resultDt.Columns.Add("monthPrepaid", typeof(float));
            resultDt.Columns.Add("monthFlowAndPrepaid", typeof(float));
            resultDt.Columns.Add("monthDvalue", typeof(float));
            resultDt.Columns.Add("yearTask", typeof(float));
            resultDt.Columns.Add("sumPrepaid", typeof(float));
            resultDt.Columns.Add("sumSalesAndPrepaid", typeof(float));
            resultDt.Columns.Add("Sector");

            DataTable dt0 = ds.Tables[0];
            DataTable dt1 = ds.Tables[1];
            DataTable dt2 = ds.Tables[2];
            DataTable dt3 = ds.Tables[3];
            DataTable dt4 = ds.Tables[4];
            DataTable dt5 = ds.Tables[5];

            foreach (DataRow dr0 in dt0.Rows)
            {
                DataRow newRow = resultDt.NewRow();
                newRow["Sector"] = dr0["Sector"];
                double yearTask = Convert.ToDouble(dr0["yearTask"]); ;
                double monthTask = Convert.ToDouble(dr0["monthTask"]);
                newRow["monthTask"] = monthTask;
                newRow["yearTask"] = yearTask;

                Boolean flag = true;
                double monthFlow = 0;
                foreach (DataRow dr1 in dt1.Rows)
                {
                    if (dr0["Sector"].Equals(dr1["Sector"]))
                    {
                        monthFlow = Convert.ToDouble(dr1["monthFlow"]);
                        newRow["monthFlow"] = monthFlow;
                        flag = !flag;
                        break;
                    }
                }
                if (flag)
                {
                    newRow["monthFlow"] = monthFlow;
                }

                flag = true;
                double sumSales = 0;
                foreach (DataRow dr2 in dt2.Rows)
                {
                    if (dr0["Sector"].Equals(dr2["Sector"]))
                    {
                        sumSales = Convert.ToDouble(dr2["sumSales"]);
                        newRow["sumSales"] = sumSales;
                        flag = !flag;
                        break;
                    }
                }
                if (flag)
                {
                    newRow["sumSales"] = sumSales;
                }

                flag = true;
                double todayFlow = 0;
                foreach (DataRow dr3 in dt3.Rows)
                {
                    if (dr0["Sector"].Equals(dr3["Sector"]))
                    {
                        todayFlow = Convert.ToDouble(dr3["todayFlow"]);
                        newRow["todayFlow"] = todayFlow;
                        flag = !flag;
                        break;
                    }
                }
                if (flag)
                {
                    newRow["todayFlow"] = todayFlow;
                }

                flag = true;
                double monthPrepaid = 0;
                foreach (DataRow dr4 in dt4.Rows)
                {
                    if (dr0["Sector"].Equals(dr4["Sector"]))
                    {
                        flag = !flag;
                        monthPrepaid = Convert.ToDouble(dr4["monthPrepaid"]);
                        if (monthTask == 0)
                        {
                            newRow["monthCompleteRate"] = 0;
                        }
                        else
                        {
                            double monthCompleteRate = (monthFlow + monthPrepaid) / monthTask;
                            newRow["monthCompleteRate"] = monthCompleteRate;
                        }

                        break;
                    }
                }
                newRow["monthPrepaid"] = monthPrepaid;
                newRow["monthFlowAndPrepaid"] = monthFlow + monthPrepaid;
                newRow["monthDvalue"] = monthTask - (monthFlow + monthPrepaid);
                if (flag)
                {
                    if (monthTask == 0)
                    {
                        newRow["monthCompleteRate"] = 0;
                    }
                    else
                    {
                        double monthCompleteRate = monthFlow / monthTask;
                        newRow["monthCompleteRate"] = monthCompleteRate;
                    }
                }

                flag = true;
                double yearPrepaid = 0;
                foreach (DataRow dr5 in dt5.Rows)
                {
                    if (dr0["Sector"].Equals(dr5["Sector"]))
                    {
                        flag = !flag;
                        yearPrepaid = Convert.ToDouble(dr5["yearPrepaid"]);
                        if (yearTask == 0)
                        {
                            newRow["sumCompleteRate"] = 0;
                        }
                        else
                        {
                            double sumCompleteRate = (sumSales + yearPrepaid) / yearTask;
                            newRow["sumCompleteRate"] = sumCompleteRate;
                        }

                        break;
                    }
                }
                newRow["sumPrepaid"] = yearPrepaid;
                newRow["sumSalesAndPrepaid"] = sumSales + yearPrepaid;
                if (flag)
                {
                    if (yearTask == 0)
                    {
                        newRow["sumCompleteRate"] = 0;
                    }
                    else
                    {
                        double sumCompleteRate = sumSales / yearTask;
                        newRow["sumCompleteRate"] = sumCompleteRate;
                    }
                }

                resultDt.Rows.Add(newRow);
            }
            return resultDt;
        }
        return null;
    }

    public static DataTable getDataFaster(int year, int month, string todayStr)
    {
        //DataSet sectorDs = SalesReportSrv.getSector();

        //if (sectorDs != null)
        //{
        //List<string> sectorList = new List<string>();
        //DataTable sectorTable = sectorDs.Tables[0];

        //if (sectorTable != null)
        //{
        //    foreach (DataRow sectDr in sectorTable.Rows)
        //    {
        //        string sector = sectDr["sector"].ToString();
        //        sectorList.Add(sector);
        //    }

        DataSet ds = SalesReportSrv.GetDataFaster(year, month, todayStr);

        if (ds != null)
        {
            DataTable resultDt = new DataTable();

            resultDt.Columns.Add("monthFlow", typeof(float));
            resultDt.Columns.Add("sumSales", typeof(float));
            resultDt.Columns.Add("todayFlow", typeof(float));
            resultDt.Columns.Add("monthCompleteRate", typeof(float));
            resultDt.Columns.Add("sumCompleteRate", typeof(float));
            resultDt.Columns.Add("Sector");

            DataTable dt0 = ds.Tables[0];
            DataTable dt1 = ds.Tables[1];
            DataTable dt2 = ds.Tables[2];
            DataTable dt3 = ds.Tables[3];
            DataTable dt4 = ds.Tables[4];
            DataTable dt5 = ds.Tables[5];

            foreach (DataRow dr0 in dt0.Rows)
            {
                DataRow newRow = resultDt.NewRow();
                newRow["Sector"] = dr0["Sector"];
                double yearTask = Convert.ToDouble(dr0["yearTask"]); ;
                double monthTask = Convert.ToDouble(dr0["monthTask"]);

                Boolean flag = true;
                double monthFlow = 0;
                foreach (DataRow dr1 in dt1.Rows)
                {
                    if (dr0["Sector"].Equals(dr1["Sector"]))
                    {
                        monthFlow = Convert.ToDouble(dr1["monthFlow"]);
                        newRow["monthFlow"] = monthFlow;
                        flag = !flag;
                        break;
                    }
                }
                if (flag)
                {
                    newRow["monthFlow"] = monthFlow;
                }

                flag = true;
                double sumSales = 0;
                foreach (DataRow dr2 in dt2.Rows)
                {
                    if (dr0["Sector"].Equals(dr2["Sector"]))
                    {
                        sumSales = Convert.ToDouble(dr2["sumSales"]);
                        newRow["sumSales"] = sumSales;
                        flag = !flag;
                        break;
                    }
                }
                if (flag)
                {
                    newRow["sumSales"] = sumSales;
                }

                flag = true;
                double todayFlow = 0;
                foreach (DataRow dr3 in dt3.Rows)
                {
                    if (dr0["Sector"].Equals(dr3["Sector"]))
                    {
                        todayFlow = Convert.ToDouble(dr3["todayFlow"]);
                        newRow["todayFlow"] = todayFlow;
                        flag = !flag;
                        break;
                    }
                }
                if (flag)
                {
                    newRow["todayFlow"] = todayFlow;
                }

                flag = true;
                foreach (DataRow dr4 in dt4.Rows)
                {
                    if (dr0["Sector"].Equals(dr4["Sector"]))
                    {
                        flag = !flag;
                        double monthPrepaid = Convert.ToDouble(dr4["monthPrepaid"]);
                        if (monthTask == 0)
                        {
                            newRow["monthCompleteRate"] = 0;
                        }
                        else
                        {
                            double monthCompleteRate = (monthFlow + monthPrepaid) / monthTask;
                            newRow["monthCompleteRate"] = monthCompleteRate;
                        }
                        
                        break;
                    }
                }
                if (flag)
                {
                    if (monthTask == 0)
                    {
                        newRow["monthCompleteRate"] = 0;
                    }
                    else
                    {
                        double monthCompleteRate = monthFlow / monthTask;
                        newRow["monthCompleteRate"] = monthCompleteRate;
                    }
                    
                }

                flag = true;
                foreach (DataRow dr5 in dt5.Rows)
                {
                    if (dr0["Sector"].Equals(dr5["Sector"]))
                    {
                        flag = !flag;
                        double yearPrepaid = Convert.ToDouble(dr5["yearPrepaid"]);
                        if (yearTask == 0)
                        {
                            newRow["sumCompleteRate"] = 0;
                        }
                        else
                        {
                            double sumCompleteRate = (sumSales + yearPrepaid) / yearTask;
                            newRow["sumCompleteRate"] = sumCompleteRate;
                        }
                        
                        break;
                    }
                }
                if (flag)
                {
                    if (yearTask == 0)
                    {
                        newRow["sumCompleteRate"] = 0;
                    }
                    else
                    {
                        double sumCompleteRate = sumSales / yearTask;
                        newRow["sumCompleteRate"] = sumCompleteRate;
                    }
                }

                resultDt.Rows.Add(newRow);
            }
            return resultDt;
        }
        return null;
        //for (int i = 0; i < sectorList.Count; i++)
        //{
        //    DataTable dt = ds.Tables[6*i];
        //    DataRow newRow = resultDt.NewRow();
        //    newRow["Sector"] = sectorList[i];
        //    double yearTask = 0;
        //    double monthTask = 0;
        //    if (dt.Rows.Count > 0)
        //    {
        //        yearTask = Convert.ToDouble(dt.Rows[0]["yearTask"]);
        //        monthTask = Convert.ToDouble(dt.Rows[0]["monthTask"]);
        //    }

        //    dt = ds.Tables[6 * i + 1];
        //    double monthFlow = Convert.ToDouble(dt.Rows[0]["monthFlow"]);

        //    dt = ds.Tables[6 * i + 2];
        //    double sumSales = Convert.ToDouble(dt.Rows[0]["sumSales"]);

        //    dt = ds.Tables[6 * i + 3];
        //    double todayFlow = Convert.ToDouble(dt.Rows[0]["todayFlow"]);

        //    dt = ds.Tables[6 * i + 4];
        //    double monthPrepaid = 0;
        //    if (dt.Rows.Count != 0)
        //    {
        //        monthPrepaid = Convert.ToDouble(dt.Rows[0]["monthPrepaid"]);
        //    }

        //    dt = ds.Tables[6 * i + 5];
        //    double yearPrepaid = 0;
        //    if (dt.Rows.Count != 0)
        //    {
        //        yearPrepaid = Convert.ToDouble(dt.Rows[0]["yearPrepaid"]);
        //    }

        //    double monthCompleteRate = 1;
        //    double sumCompleteRate = 1;
        //    if (yearTask != 0)
        //    {
        //        sumCompleteRate = (sumSales + yearPrepaid) / yearTask;
        //    }
        //    if (monthTask != 0)
        //    {
        //        monthCompleteRate = (monthFlow + monthPrepaid) / monthTask;
        //    }

        //    newRow["monthFlow"] = monthFlow;
        //    newRow["sumSales"] = sumSales;
        //    newRow["todayFlow"] = todayFlow;
        //    newRow["sumCompleteRate"] = sumCompleteRate;
        //    newRow["monthCompleteRate"] = monthCompleteRate;

        //    resultDt.Rows.Add(newRow);
        //}
        //}
        //}
    }

    public static DataTable getCenterDataFaster(int year, int month, string todayStr)
    {
        DataSet ds = SalesReportSrv.GetCenterDataFaster(year, month, todayStr);
        DataTable dt = null;
        if (ds != null)
        {
            // 给dataTable新增月销售，年销售,月完成率，年完成率的字段
            dt = new DataTable();
            dt.Columns.Add("monthFlow", typeof(double));
            dt.Columns.Add("sumSales", typeof(double));
            dt.Columns.Add("todayFlow", typeof(double));
            dt.Columns.Add("monthCompleteRate", typeof(double));
            dt.Columns.Add("sumCompleteRate", typeof(double));

            dt.Columns.Add("center");
            //dt.Columns.Add("yearTask", typeof(float));
            //dt.Columns.Add("monthTask", typeof(float));


            double monthFlowDS = 0, sumSalesDS = 0, monthFlowYl = 0, sumSalesYl = 0, monthFlowZs = 0, sumSalesZs = 0, todayFlow = 0;
            if (ds.Tables[1].Rows.Count > 0)//当月流向表里有数据
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    double flowSalesMoney = Convert.ToDouble(row["FlowSalesMoney"]);
                    if (row["Sector"].ToString().Contains("业力"))
                    {
                        monthFlowYl += flowSalesMoney;
                    }
                    else if (row["Sector"].ToString().Contains("中申"))
                    {
                        monthFlowZs += flowSalesMoney;
                    }
                    else
                    {
                        monthFlowDS += flowSalesMoney;
                    }
                }
            }
            else//当月流向表里没有数据，从出库计算
            {
                DataTable dtLeaveStockCurrentMonth = ds.Tables[2].Copy();
                foreach (DataRow row in ds.Tables[6].Rows)
                {
                    for (int j = dtLeaveStockCurrentMonth.Rows.Count - 1; j >= 0; j--)
                    {
                        DataRow r = dtLeaveStockCurrentMonth.Rows[j];
                        if (object.Equals(r["terminalClientId"], row["HospitalId"]) && object.Equals(r["ProductId"], row["ProductId"]))
                        {
                            double flowSalesMoney = Convert.ToInt32(r["amountSend"]) * Convert.ToDouble(row["ExaminePrice"]);
                            if (row["Sector"].ToString().Contains("业力"))
                            {
                                monthFlowYl += flowSalesMoney;
                                sumSalesYl += flowSalesMoney;
                            }
                            else if (row["Sector"].ToString().Contains("中申"))
                            {
                                monthFlowZs += flowSalesMoney;
                                sumSalesZs += flowSalesMoney;
                            }
                            else
                            {
                                monthFlowDS += flowSalesMoney;
                                sumSalesDS += flowSalesMoney;
                            }
                            dtLeaveStockCurrentMonth.Rows.RemoveAt(j);
                        }
                    }
                }
            }
            //东森
            DataRow newRow = dt.NewRow();
            newRow["center"] = "东森";
            double yearTask = Convert.ToDouble(ds.Tables[0].Rows[0]["yearTask"] == DBNull.Value ? 0 : ds.Tables[0].Rows[0]["yearTask"]);// monthCompleteRate
            double monthTask = Convert.ToDouble(ds.Tables[0].Rows[0]["monthTask"] == DBNull.Value ? 0 : ds.Tables[0].Rows[0]["monthTask"]); //sumCompleteRate
            
            newRow["monthFlow"] = monthFlowDS;
            double dsMonthPrepaid = Convert.ToDouble(ds.Tables[10].Rows[0]["dsMonthPrepaid"] == DBNull.Value ? 0 : ds.Tables[10].Rows[0]["dsMonthPrepaid"]);
            newRow["monthCompleteRate"] = (monthFlowDS + dsMonthPrepaid) / monthTask;
            sumSalesDS += Convert.ToDouble(ds.Tables[3].Rows[0]["yearFlowSalesMoney"] == DBNull.Value ? 0 : ds.Tables[3].Rows[0]["yearFlowSalesMoney"]);
            newRow["sumSales"] = sumSalesDS;
            double dsYearPrepaid = Convert.ToDouble(ds.Tables[10].Rows[0]["dsYearPrepaid"] == DBNull.Value ? 0 : ds.Tables[10].Rows[0]["dsYearPrepaid"]);
            newRow["sumCompleteRate"] = (sumSalesDS + dsYearPrepaid) / yearTask;
            newRow["todayFlow"] = Convert.ToDouble(ds.Tables[7].Rows[0]["todayFlow"] == DBNull.Value ? 0 : ds.Tables[7].Rows[0]["todayFlow"]);
            dt.Rows.Add(newRow);
            //2017年业力每个月80万任务，中申60万任务
            newRow = dt.NewRow();
            newRow["center"] = "业力";
            double ylYearTask = Convert.ToDouble(ds.Tables[13].Rows[0]["ylYearTask"] == DBNull.Value ? 0 : ds.Tables[13].Rows[0]["ylYearTask"]);// monthCompleteRate
            double ylMonthTask = Convert.ToDouble(ds.Tables[13].Rows[0]["ylMonthTask"] == DBNull.Value ? 0 : ds.Tables[13].Rows[0]["ylMonthTask"]); //sumCompleteRate
            newRow["monthFlow"] = monthFlowYl;
            double ylMonthPrepaid = Convert.ToDouble(ds.Tables[11].Rows[0]["ylMonthPrepaid"] == DBNull.Value ? 0 : ds.Tables[11].Rows[0]["ylMonthPrepaid"]);
            newRow["monthCompleteRate"] = (monthFlowYl + ylMonthPrepaid) / ylMonthTask;
            sumSalesYl += Convert.ToDouble(ds.Tables[4].Rows[0]["yearFlowSalesMoney"] == DBNull.Value ? 0 : ds.Tables[4].Rows[0]["yearFlowSalesMoney"]);
            newRow["sumSales"] = sumSalesYl;
            double ylYearPrepaid = Convert.ToDouble(ds.Tables[11].Rows[0]["ylYearPrepaid"] == DBNull.Value ? 0 : ds.Tables[11].Rows[0]["ylYearPrepaid"]);
            newRow["sumCompleteRate"] = (sumSalesYl + ylYearPrepaid) / ylYearTask;
            newRow["todayFlow"] = Convert.ToDouble(ds.Tables[8].Rows[0]["todayFlow"] == DBNull.Value ? 0 : ds.Tables[8].Rows[0]["todayFlow"]);
            dt.Rows.Add(newRow);

            newRow = dt.NewRow();
            newRow["center"] = "中申";
            double zsYearTask = Convert.ToDouble(ds.Tables[14].Rows[0]["zsYearTask"] == DBNull.Value ? 0 : ds.Tables[14].Rows[0]["zsYearTask"]);// monthCompleteRate
            double zsMonthTask = Convert.ToDouble(ds.Tables[14].Rows[0]["zsMonthTask"] == DBNull.Value ? 0 : ds.Tables[14].Rows[0]["zsMonthTask"]); //sumCompleteRate
            newRow["monthFlow"] = monthFlowZs;
            double zsMonthPrepaid = Convert.ToDouble(ds.Tables[12].Rows[0]["zsMonthPrepaid"] == DBNull.Value ? 0 : ds.Tables[12].Rows[0]["zsMonthPrepaid"]);
            newRow["monthCompleteRate"] = (monthFlowZs + zsMonthPrepaid) / zsMonthTask;
            sumSalesZs += Convert.ToDouble(ds.Tables[5].Rows[0]["yearFlowSalesMoney"] == DBNull.Value ? 0 : ds.Tables[5].Rows[0]["yearFlowSalesMoney"]);
            newRow["sumSales"] = sumSalesZs;
            double zsYearPrepaid = Convert.ToDouble(ds.Tables[12].Rows[0]["zsYearPrepaid"] == DBNull.Value ? 0 : ds.Tables[12].Rows[0]["zsYearPrepaid"]);
            newRow["sumCompleteRate"] = (sumSalesZs + zsYearPrepaid) / zsYearTask;
            newRow["todayFlow"] = Convert.ToDouble(ds.Tables[9].Rows[0]["todayFlow"] == DBNull.Value ? 0 : ds.Tables[9].Rows[0]["todayFlow"]);
            dt.Rows.Add(newRow);
        }
        return dt;
    }

    public static DataTable getCenterData(int year, int month, string todayStr)
    {
        DataSet ds = SalesReportSrv.GetCenterDataFaster(year, month, todayStr);
        DataTable dt = null;
        if (ds != null)
        {
            // 给dataTable新增月销售，年销售,月完成率，年完成率的字段
            dt = new DataTable();
            dt.Columns.Add("monthFlow", typeof(float));
            dt.Columns.Add("sumSales", typeof(float));
            dt.Columns.Add("todayFlow", typeof(float));
            dt.Columns.Add("monthCompleteRate", typeof(float));
            dt.Columns.Add("sumCompleteRate", typeof(float));
            dt.Columns.Add("monthTask", typeof(float));
            dt.Columns.Add("monthPrepaid", typeof(float));
            dt.Columns.Add("monthFlowAndPrepaid", typeof(float));
            dt.Columns.Add("monthDvalue", typeof(float));
            dt.Columns.Add("yearTask", typeof(float));
            dt.Columns.Add("sumPrepaid", typeof(float));
            dt.Columns.Add("sumSalesAndPrepaid", typeof(float));

            dt.Columns.Add("center");
            //dt.Columns.Add("yearTask", typeof(float));
            //dt.Columns.Add("monthTask", typeof(float));


            double monthFlowDS = 0, sumSalesDS = 0, monthFlowYl = 0, sumSalesYl = 0, monthFlowZs = 0, sumSalesZs = 0, todayFlow = 0;
            if (ds.Tables[1].Rows.Count > 0)//当月流向表里有数据
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    double flowSalesMoney = Convert.ToDouble(row["FlowSalesMoney"]);
                    if (row["Sector"].ToString().Contains("业力"))
                    {
                        monthFlowYl += flowSalesMoney;
                    }
                    else if (row["Sector"].ToString().Contains("中申"))
                    {
                        monthFlowZs += flowSalesMoney;
                    }
                    else
                    {
                        monthFlowDS += flowSalesMoney;
                    }
                }
            }
            else//当月流向表里没有数据，从出库计算
            {
                DataTable dtLeaveStockCurrentMonth = ds.Tables[2].Copy();
                foreach (DataRow row in ds.Tables[6].Rows)
                {
                    for (int j = dtLeaveStockCurrentMonth.Rows.Count - 1; j >= 0; j--)
                    {
                        DataRow r = dtLeaveStockCurrentMonth.Rows[j];
                        if (object.Equals(r["terminalClientId"], row["HospitalId"]) && object.Equals(r["ProductId"], row["ProductId"]))
                        {
                            double flowSalesMoney = Convert.ToInt32(r["amountSend"]) * Convert.ToDouble(row["ExaminePrice"]);
                            if (row["Sector"].ToString().Contains("业力"))
                            {
                                monthFlowYl += flowSalesMoney;
                                sumSalesYl += flowSalesMoney;
                            }
                            else if (row["Sector"].ToString().Contains("中申"))
                            {
                                monthFlowZs += flowSalesMoney;
                                sumSalesZs += flowSalesMoney;
                            }
                            else
                            {
                                monthFlowDS += flowSalesMoney;
                                sumSalesDS += flowSalesMoney;
                            }
                            dtLeaveStockCurrentMonth.Rows.RemoveAt(j);
                        }
                    }
                }
            }
            //东森
            DataRow newRow = dt.NewRow();
            newRow["center"] = "东森";
            double yearTask = Convert.ToDouble(ds.Tables[0].Rows[0]["yearTask"] == DBNull.Value ? 0 : ds.Tables[0].Rows[0]["yearTask"]);// monthCompleteRate
            double monthTask = Convert.ToDouble(ds.Tables[0].Rows[0]["monthTask"] == DBNull.Value ? 0 : ds.Tables[0].Rows[0]["monthTask"]); //sumCompleteRate
            newRow["monthTask"] = monthTask;
            newRow["yearTask"] = yearTask;
            newRow["monthFlow"] = monthFlowDS;
            double dsMonthPrepaid = Convert.ToDouble(ds.Tables[10].Rows[0]["dsMonthPrepaid"] == DBNull.Value ? 0 : ds.Tables[10].Rows[0]["dsMonthPrepaid"]);
            newRow["monthPrepaid"] = dsMonthPrepaid;
            newRow["monthFlowAndPrepaid"] = dsMonthPrepaid + monthFlowDS;
            newRow["monthDvalue"] = monthTask - (dsMonthPrepaid + monthFlowDS);
            newRow["monthCompleteRate"] = (monthFlowDS + dsMonthPrepaid) / monthTask;
            sumSalesDS += Convert.ToDouble(ds.Tables[3].Rows[0]["yearFlowSalesMoney"] == DBNull.Value ? 0 : ds.Tables[3].Rows[0]["yearFlowSalesMoney"]);
            newRow["sumSales"] = sumSalesDS;
            double dsYearPrepaid = Convert.ToDouble(ds.Tables[10].Rows[0]["dsYearPrepaid"] == DBNull.Value ? 0 : ds.Tables[10].Rows[0]["dsYearPrepaid"]);
            newRow["sumPrepaid"] = dsYearPrepaid;
            newRow["sumSalesAndPrepaid"] = sumSalesDS + dsYearPrepaid;
            newRow["sumCompleteRate"] = (sumSalesDS + dsYearPrepaid) / yearTask;
            newRow["todayFlow"] = Convert.ToDouble(ds.Tables[7].Rows[0]["todayFlow"] == DBNull.Value ? 0 : ds.Tables[7].Rows[0]["todayFlow"]);
            dt.Rows.Add(newRow);

            // 业力
            newRow = dt.NewRow();
            newRow["center"] = "业力";
            double ylYearTask = Convert.ToDouble(ds.Tables[13].Rows[0]["ylYearTask"] == DBNull.Value ? 0 : ds.Tables[13].Rows[0]["ylYearTask"]);// monthCompleteRate
            double ylMonthTask = Convert.ToDouble(ds.Tables[13].Rows[0]["ylMonthTask"] == DBNull.Value ? 0 : ds.Tables[13].Rows[0]["ylMonthTask"]); //sumCompleteRate
            newRow["monthTask"] = ylMonthTask;
            newRow["yearTask"] = ylYearTask;
            newRow["monthFlow"] = monthFlowYl;
            double ylMonthPrepaid = Convert.ToDouble(ds.Tables[11].Rows[0]["ylMonthPrepaid"] == DBNull.Value ? 0 : ds.Tables[11].Rows[0]["ylMonthPrepaid"]);
            newRow["monthPrepaid"] = ylMonthPrepaid;
            newRow["monthFlowAndPrepaid"] = ylMonthPrepaid + monthFlowYl;
            newRow["monthDvalue"] = ylMonthTask - (ylMonthPrepaid + monthFlowYl);
            newRow["monthCompleteRate"] = (monthFlowYl + ylMonthPrepaid) / ylMonthTask;
            sumSalesYl += Convert.ToDouble(ds.Tables[4].Rows[0]["yearFlowSalesMoney"] == DBNull.Value ? 0 : ds.Tables[4].Rows[0]["yearFlowSalesMoney"]);
            newRow["sumSales"] = sumSalesYl;
            double ylYearPrepaid = Convert.ToDouble(ds.Tables[11].Rows[0]["ylYearPrepaid"] == DBNull.Value ? 0 : ds.Tables[11].Rows[0]["ylYearPrepaid"]);
            newRow["sumPrepaid"] = ylYearPrepaid;
            newRow["sumSalesAndPrepaid"] = sumSalesYl + ylYearPrepaid;
            newRow["sumCompleteRate"] = (sumSalesYl + ylYearPrepaid) / ylYearTask;
            newRow["todayFlow"] = Convert.ToDouble(ds.Tables[8].Rows[0]["todayFlow"] == DBNull.Value ? 0 : ds.Tables[8].Rows[0]["todayFlow"]);
            dt.Rows.Add(newRow);

            // 中申
            newRow = dt.NewRow();
            newRow["center"] = "中申";
            double zsYearTask = Convert.ToDouble(ds.Tables[14].Rows[0]["zsYearTask"] == DBNull.Value ? 0 : ds.Tables[14].Rows[0]["zsYearTask"]);// monthCompleteRate
            double zsMonthTask = Convert.ToDouble(ds.Tables[14].Rows[0]["zsMonthTask"] == DBNull.Value ? 0 : ds.Tables[14].Rows[0]["zsMonthTask"]); //sumCompleteRate
            newRow["monthTask"] = zsMonthTask;
            newRow["yearTask"] = zsYearTask;
            newRow["monthFlow"] = monthFlowZs;
            double zsMonthPrepaid = Convert.ToDouble(ds.Tables[12].Rows[0]["zsMonthPrepaid"] == DBNull.Value ? 0 : ds.Tables[12].Rows[0]["zsMonthPrepaid"]);
            newRow["monthPrepaid"] = zsMonthPrepaid;
            newRow["monthFlowAndPrepaid"] = zsMonthPrepaid + monthFlowZs;
            newRow["monthDvalue"] = zsMonthTask - (zsMonthPrepaid + monthFlowZs);
            newRow["monthCompleteRate"] = (monthFlowZs + zsMonthPrepaid) / zsMonthTask;
            sumSalesZs += Convert.ToDouble(ds.Tables[5].Rows[0]["yearFlowSalesMoney"] == DBNull.Value ? 0 : ds.Tables[5].Rows[0]["yearFlowSalesMoney"]);
            newRow["sumSales"] = sumSalesZs;
            double zsYearPrepaid = Convert.ToDouble(ds.Tables[12].Rows[0]["zsYearPrepaid"] == DBNull.Value ? 0 : ds.Tables[12].Rows[0]["zsYearPrepaid"]);
            newRow["sumPrepaid"] = zsYearPrepaid;
            newRow["sumSalesAndPrepaid"] = sumSalesZs + zsYearPrepaid;
            newRow["sumCompleteRate"] = (sumSalesZs + zsYearPrepaid) / zsYearTask;
            newRow["todayFlow"] = Convert.ToDouble(ds.Tables[9].Rows[0]["todayFlow"] == DBNull.Value ? 0 : ds.Tables[9].Rows[0]["todayFlow"]);
            dt.Rows.Add(newRow);
        }
        return dt;
    }

    public static DataTable getSectorMoneyMonthly(int year, string sector)
    {
        DataSet ds = SalesReportSrv.getSectorMoneyMonthly(year, sector);

        if (ds == null)
        {
            return null;
        }

        DataTable dt = new DataTable();

        dt.Columns.Add("jan", typeof(float));
        dt.Columns.Add("feb", typeof(float));
        dt.Columns.Add("mar", typeof(float));
        dt.Columns.Add("apr", typeof(float));
        dt.Columns.Add("may", typeof(float));
        dt.Columns.Add("jun", typeof(float));
        dt.Columns.Add("jul", typeof(float));
        dt.Columns.Add("aug", typeof(float));
        dt.Columns.Add("sep", typeof(float));
        dt.Columns.Add("oct", typeof(float));
        dt.Columns.Add("nov", typeof(float));
        dt.Columns.Add("dec", typeof(float));

        DataRow newRow = dt.NewRow();
        
        newRow["jan"] = Convert.ToDouble(ds.Tables[0].Rows[0]["monthFlow"]);
        newRow["feb"] = Convert.ToDouble(ds.Tables[1].Rows[0]["monthFlow"]);
        newRow["mar"] = Convert.ToDouble(ds.Tables[2].Rows[0]["monthFlow"]);
        newRow["apr"] = Convert.ToDouble(ds.Tables[3].Rows[0]["monthFlow"]);
        newRow["may"] = Convert.ToDouble(ds.Tables[4].Rows[0]["monthFlow"]);
        newRow["jun"] = Convert.ToDouble(ds.Tables[5].Rows[0]["monthFlow"]);
        newRow["jul"] = Convert.ToDouble(ds.Tables[6].Rows[0]["monthFlow"]);
        newRow["aug"] = Convert.ToDouble(ds.Tables[7].Rows[0]["monthFlow"]);
        newRow["sep"] = Convert.ToDouble(ds.Tables[8].Rows[0]["monthFlow"]);
        newRow["oct"] = Convert.ToDouble(ds.Tables[9].Rows[0]["monthFlow"]);
        newRow["nov"] = Convert.ToDouble(ds.Tables[10].Rows[0]["monthFlow"]);
        newRow["dec"] = Convert.ToDouble(ds.Tables[11].Rows[0]["monthFlow"]);

        dt.Rows.Add(newRow);

        return dt;
    }

    public static DataTable getSalesmanCompleteRate(int year, int month, string searchString, string sector, UserInfo user)
    {
        DataSet ds = SalesReportSrv.getSalesmanCompleteRate(year, month, user);

        if (ds == null)
            return null;
        DataTable dt = ds.Tables[0];
        if (dt == null)
            return null;
        dt.Columns.Add("completeRate", typeof(float));
        foreach (DataRow dr in dt.Rows)
        {
            double monthTask = Convert.ToDouble(dr["monthTask"]);
            double netSalesMoney = Convert.ToDouble(dr["netSalesMoney"]);
            double completeRate = 0;
            
            if (monthTask == 0 && netSalesMoney != 0)
            {
                completeRate = 1;
            }
            else if (monthTask != 0)
            {
                completeRate = netSalesMoney / monthTask;
            }

            dr["completeRate"] = completeRate;
        }

        if ("".Equals(searchString) && "".Equals(sector))
        {
            return dt;
        }
        else if ("".Equals(sector))
        {
            DataTable dt1 = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                if (PinYinHelper.IsEqual(row["Sales"].ToString(), searchString) || (row["Sales"].ToString()).IndexOf(searchString) >= 0)
                {
                    dt1.Rows.Add(row.ItemArray);
                    continue;
                }
            }
            return dt1;
        }
        else if ("".Equals(searchString))
        {
            DataTable dt1 = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                if ((row["Sector"].ToString()).IndexOf(sector) >= 0)
                {
                    dt1.Rows.Add(row.ItemArray);
                    continue;
                }
            }
            return dt1;
        }
        else
        {
            DataTable dt1 = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                if ((PinYinHelper.IsEqual(row["Sales"].ToString(), searchString) || (row["Sales"].ToString()).
                    IndexOf(searchString) >= 0) && (row["Sector"].ToString()).IndexOf(sector) >= 0)
                {
                    dt1.Rows.Add(row.ItemArray);
                    continue;
                }
            }
            return dt1;
        }
    }

    public static DataTable getSector()
    {
        DataSet ds = SalesReportSrv.getSector();

        if (ds == null)
            return null;

        return ds.Tables[0];
    }
}