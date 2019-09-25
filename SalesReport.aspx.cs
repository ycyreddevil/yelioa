using System;
using System.Data;

public partial class SalesReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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
        DataTable dt = SalesReportManage.getData(year, month, dateStr);

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
        DataTable dt = SalesReportManage.getCenterData(year, month, dateStr);

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

        footDataTable.Columns.Add("monthTask");
        footDataTable.Columns.Add("monthPrepaid");
        footDataTable.Columns.Add("monthFlowAndPrepaid");
        footDataTable.Columns.Add("monthDvalue");
        footDataTable.Columns.Add("yearTask");
        footDataTable.Columns.Add("sumPrepaid");
        footDataTable.Columns.Add("sumFlowAndPrepaid");
        footDataTable.Columns.Add("sumCompleteRate"); 
        footDataTable.Columns.Add("monthCompleteRate");

        DataRow dr = footDataTable.NewRow();

        dr["monthFlow"] = monthFlow.ToString();
        dr["sumSales"] = sumSales.ToString();
        dr[colunmForSum] = "合计";
        dr["todayFlow"] = todayFlow;
        dr["IsFooter"] = true;

        footDataTable.Rows.Add(dr);

        return footDataTable;
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
}