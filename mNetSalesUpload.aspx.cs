using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using Newtonsoft.Json;

public partial class mNetSalesUpload : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxNetSalesHelper wx = new WxNetSalesHelper("http://yelioa.top/mNetSalesUpload.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res+"')</script>");
            Response.End();
            return ;
        }

        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getDatalist")
            {
                Response.Write(getDatalist());
            }
            else if (action == "getFlowNumOfReportSales")
            {
                Response.Write(getFlowNumOfReportSales());
            }
            else if (action == "saveNetSalesNumOfReportSales")
            {
                Response.Write(saveNetSalesNumOfReportSales());
            }
            else if (action == "submitNetSalesNumOfReportSales")
            {
                Response.Write(submitNetSalesNumOfReportSales());
            }
            Response.End();
        }
    }

    private String saveNetSalesNumOfReportSales()
    {
        String hospital = Request.Form["group"];
        String product = Request.Form["item"];
        String netSalesNum = Request.Form["netSales"];
        String docCode = Request.Form["docCode"];
        String time = Request.Form["time"];
        UserInfo user = (UserInfo)Session["user"];
        String sales = user.userName;

        if (!StringTools.IsNumeric(docCode))
            return docCode;

        Dictionary<string, string> dict = NetSalesInfoManage.SaveNetSales(hospital,product,sales,netSalesNum,docCode,time);
        String result = dict["returnMsg"];
        docCode = dict["docCode"];
        if (result == "success")
            return docCode;
        else
            return result;
    }

    private String submitNetSalesNumOfReportSales()
    {
        String docCode = Request.Form["docCode"];
        UserInfo user = (UserInfo)Session["user"];
        return ApprovalFlowManage.SubmitDocument("net_sales", docCode, user,
            "http://yelioa.top/mNetSalesApproval.aspx?type=others", "http://yelioa.top/mNetSalesApproval.aspx?type=mine",
            "PyO4Il3bIxyuFquBAGrrr76GVcUbIN5NPpxNGAja-4U", "netSales", "1000002");
    }

    private string getFlowNumOfReportSales()
    {
        string hospital = Request.Form["group"];
        string product = Request.Form["item"];
        UserInfo user = (UserInfo)Session["user"];
        string res = NetSalesInfoManage.getFlowNumOfReportSales(hospital, product,user.userName);
        String res1 = NetSalesInfoManage.getNetSalesNum(hospital, product, user.userName);
        Dictionary<string, string> dict = new Dictionary<string, string>();
        if(StringTools.IsInt(res))//返回流向数值
        {
            dict.Add("FlowNum", res);
            dict.Add("errorMsg", "success");
        }
        else//返回错误信息
        {
            dict.Add("FlowNum", "0");
            dict.Add("errorMsg", res);
        }
        // 返回保存的纯销数据
        if (StringTools.IsInt(res1))
        {
            dict.Add("netSalesNum", res1);
        }
        else//返回错误信息
        {
            dict.Add("netSalesNum", "0");
        }
        return JsonHelper.SerializeObject(dict);
    }

    private string getDatalist()
    {
        string res = "";
        UserInfo user = (UserInfo)Session["user"];
        DataTable dt = NetSalesInfoManage.getInfos(user.userId.ToString());
        if(dt != null)
        {
            dt = PinYinHelper.SortByPinYin(dt, "Hospital", "asc");
            ArrayList list = new ArrayList();
            foreach(DataRow row in dt.Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("group", row["Hospital"].ToString());
                dict.Add("item", row["Product"].ToString());
                dict.Add("id", row["Id"].ToString());
                dict.Add("status", row["State"].ToString());
                dict.Add("docCode", row["DocCode"].ToString());
                dict.Add("time", row["CorrespondingTime"].ToString());
                list.Add(dict);
            }
            res = JsonHelper.SerializeObject(list);
        }
        return res;
    }
}