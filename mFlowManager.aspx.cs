using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;

public partial class mFlowManager : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //WxNetSalesHelper wx = new WxNetSalesHelper("http://yelioa.top/mFlowManager.aspx");
        WxCommon wx = new WxCommon("mSalesData",
            "Zg8Be_YI2m56f5i1u3IWOeJaUtLccRkzc4Ivniv0vco",
            "1000003",
            "http://yelioa.top/mFlowManager.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        String action = Request.Params["act"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if (action == "getDataList")
            {
                Response.Write(getDataList());
            }
            else if (action == "getDetailData")
            {
                Response.Write(getDetailData());
            }

            Response.End();
        }
    }

    private string getDetailData()
    {
        UserInfo user = (UserInfo)Session["user"];
        Boolean isPrivilege = Privilege.checkPrivilege(user);

        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string id = Request.Params["id"];
        DataSet ds = FlowInfoManage.GetMobileDetail(id);
        string json = "";
        if (ds != null)
        {
            DataTable dt = PinYinHelper.SortByPinYin(ds.Tables[0], sort, order);
            json = JsonHelper.DataTable2Json(dt);
            JObject jObject = new JObject();
            jObject.Add("data", json);
            jObject.Add("privilege", isPrivilege);
            return jObject.ToString();
        }
        return json.ToString();
    }

    private string getDataList()
    {
        int year = Int32.Parse(Request.Form["year"]);
        int month = Int32.Parse(Request.Form["month"]);
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string searchString = Request.Form["searchString"];
        string hospital = Request.Form["hospital"];
        string product = Request.Form["product"];
        string json = "";
        DataTable dt = FlowInfoManage.GetMobileInfos(year, month, searchString, hospital, product);
        if (dt != null)
        {
            dt = PinYinHelper.SortByPinYin(dt, sort, order);
            json = JsonHelper.DataTable2Json(dt);
        }
        //JObject res = new JObject();
        //res.Add("DataIsArchived", DataIsArchived);
        //res["Data"] = JsonHelper.DeserializeJsonToObject<JObject>(json);
        //res.Add("Data", JsonHelper.DeserializeJsonToObject<object>(json));
        //Dictionary<string, object> resDict = new Dictionary<string, object>();
        //resDict.Add("DataIsArchived", DataIsArchived);
        //resDict.Add("Data", json);
        //json = JsonHelper.SerializeObject(resDict);
        return json.ToString();
    }
}