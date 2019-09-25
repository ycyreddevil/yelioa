using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

public partial class mSalesReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //WxNetSalesHelper wx = new WxNetSalesHelper("http://yelioa.top/mSalesReport.aspx");
        WxCommon wx = new WxCommon("mSalesData",
            "Zg8Be_YI2m56f5i1u3IWOeJaUtLccRkzc4Ivniv0vco",
            "1000003",
            "http://yelioa.top/mSalesReport.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getDataList")
            {
                Response.Write(getDataList());
            }
            else if (action == "getCenterDataList")
            {
                Response.Write(getCenterDataList());
            }
            else if (action == "generateCenterChart")
            {
                Response.Write(generateCenterChart());
            }
            else if (action == "generateSectorChart")
            {
                Response.Write(generateSectorChart());
            }
            else if (action == "generateCenterMonthChart")
            {
                Response.Write(generateCenterMonthChart());
            }
            else if (action == "dataGridSort")
            {
                Response.Write(dataGridSort());
            }
            Response.End();
        }
    }

    protected string getDataList()
    {
        int year = Int32.Parse(Request.Form["year"]);
        int month = Int32.Parse(Request.Form["month"]);
        string dateStr = Request.Form["dateStr"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        // 读取数据
        DataTable dt = SalesReportManage.getDataFaster(year, month, dateStr);

        if (dt != null)
        {
            dt = PinYinHelper.SortByPinYin(dt, sort, order);

            DataTable footDataTable = generateFooterTable(dt, "Sector");

            string json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt, footDataTable);

            return json;
        }
        else
        {
            return "error";
        }
    }

    protected string getCenterDataList()
    {
        int year = Int32.Parse(Request.Form["year"]);
        int month = Int32.Parse(Request.Form["month"]);
        string dateStr = Request.Form["dateStr"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        // 读取数据
        DataTable dt = SalesReportManage.getCenterDataFaster(year, month, dateStr);

        if (dt != null)
        {
            dt = PinYinHelper.SortByPinYin(dt, sort, order);

            DataTable footDataTable = generateFooterTable(dt, "center");

            string json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt, footDataTable);

            return json;
        }
        else
        {
            return "error";
        }
    }

    protected string generateSectorChart()
    {
        int year = Int32.Parse(Request.Form["year"]);
        int month = Int32.Parse(Request.Form["month"]);
        string dateStr = Request.Form["dateStr"];

        DataTable dt = SalesReportManage.getDataFaster(year, month, dateStr);

        // 组装成echarts图表数据
        if (dt == null)
        {
            return null;
        }

        JObject jObject = new JObject();

        List<string> sectorList = new List<string>();
        List<float> monthCompleteRateList = new List<float>();
        List<float> sumCompleteRateList = new List<float>();

        foreach (DataRow dr in dt.Rows)
        {
            float monthCompleteRate = float.Parse(dr["monthCompleteRate"].ToString());
            float sumCompleteRate = float.Parse(dr["sumCompleteRate"].ToString());
            string sector = dr["Sector"].ToString();

            sectorList.Add(sector);
            monthCompleteRateList.Add(monthCompleteRate * 100);
            sumCompleteRateList.Add(sumCompleteRate * 100);
        }

        jObject.Add("sectorList", JsonHelper.JsonSerializer(sectorList));
        jObject.Add("monthCompleteRateList", JsonHelper.JsonSerializer(monthCompleteRateList));
        jObject.Add("sumCompleteRateList", JsonHelper.JsonSerializer(sumCompleteRateList));

        return jObject.ToString();
    }

    protected string generateCenterChart()
    {
        int year = Int32.Parse(Request.Form["year"]);
        int month = Int32.Parse(Request.Form["month"]);
        string dateStr = Request.Form["dateStr"];
        // 读取总表数据
        DataTable dt = SalesReportManage.getCenterDataFaster(year, month, dateStr);

        JArray jArray1 = new JArray();
        JArray jArray2 = new JArray();

        // 组装成echarts图表数据
        if (dt == null)
        {
            return null;
        }

        foreach (DataRow dr in dt.Rows)
        {
            JObject jObject = new JObject();

            float monthCompleteRate = float.Parse(dr["monthCompleteRate"].ToString());
            float sumCompleteRate = float.Parse(dr["sumCompleteRate"].ToString());

            jObject.Add("name", "年度达成率");
            jObject.Add("value", sumCompleteRate * 100);

            jArray1.Add(jObject);

            jObject = new JObject();

            jObject.Add("name", "月度达成率");
            jObject.Add("value", monthCompleteRate * 100);

            jArray2.Add(jObject);
        }

        JObject totalObject = new JObject
        {
            { "jArray1", jArray1 },
            { "jArray2", jArray2 }
        };

        return totalObject.ToString();
    }

    protected string generateCenterMonthChart()
    {
        int year = Int32.Parse(Request.Form["year"]);
        string sector = Request.Form["sector"];

        DataTable dt = SalesReportManage.getSectorMoneyMonthly(year, sector);

        JObject jObject = new JObject();

        List<string> list = new List<string>();

        list.Add(dt.Rows[0]["jan"].ToString());
        list.Add(dt.Rows[0]["feb"].ToString());
        list.Add(dt.Rows[0]["mar"].ToString());
        list.Add(dt.Rows[0]["apr"].ToString());
        list.Add(dt.Rows[0]["may"].ToString());
        list.Add(dt.Rows[0]["jun"].ToString());
        list.Add(dt.Rows[0]["jul"].ToString());
        list.Add(dt.Rows[0]["aug"].ToString());
        list.Add(dt.Rows[0]["sep"].ToString());
        list.Add(dt.Rows[0]["oct"].ToString());
        list.Add(dt.Rows[0]["nov"].ToString());
        list.Add(dt.Rows[0]["dec"].ToString());

        jObject.Add("list", JsonHelper.JsonSerializer(list));
        return jObject.ToString();
    }

    protected string dataGridSort()
    {
        string data = Request.Form["data"];
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        DataTable dt = JsonHelper.DeserializeJsonToObject<DataTable>(data);
        dt = PinYinHelper.SortByPinYin(dt, sort, order);
        return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
    }

    private DataTable generateFooterTable(DataTable dt, string colunmForSum)
    {
        float monthFlow = 0;
        float sumSales = 0;
        float todayFlow = 0;

        foreach (DataRow dataRow in dt.Rows)
        {
            monthFlow += float.Parse(dataRow["monthFlow"].ToString());
            sumSales += float.Parse(dataRow["sumSales"].ToString());
            todayFlow += float.Parse(dataRow["todayFlow"].ToString());
        }

        DataTable footDataTable = new DataTable();

        footDataTable.Columns.Add("monthFlow");
        footDataTable.Columns.Add("sumSales");
        footDataTable.Columns.Add(colunmForSum);
        footDataTable.Columns.Add("todayFlow");
        footDataTable.Columns.Add("IsFooter");

        DataRow dr = footDataTable.NewRow();

        dr["monthFlow"] = monthFlow.ToString();
        dr["sumSales"] = sumSales.ToString();
        dr[colunmForSum] = "合计";
        dr["todayFlow"] = todayFlow;
        dr["IsFooter"] = true;

        footDataTable.Rows.Add(dr);

        return footDataTable;
    }
}