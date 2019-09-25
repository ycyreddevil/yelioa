using System;
using System.Data;
using System.Web;

public partial class mPointApplyRecord : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mPointApply",
           "i0SFeOuq3eonsbAWYfmAnrB0k_4K5d3Ub7Y6Z-KkYrc",
           "1000008",
           "http://yelioa.top/mPointApply.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        string action = Request.Form["act"];
        if(!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if(action=="initdatagrid1")
            {
                Initdatagrid("initdatagrid1");
            }
            else if (action == "initdatagrid2")
            {
                Initdatagrid("initdatagrid2");
            }
            Response.End();
        }
    }

    private void Initdatagrid(string act)
    {
        string d = Request.Form["date"];
        DateTime date = Convert.ToDateTime(d);
        string month = "";
        if (date.Month < 10)
            month = "0" + date.Month.ToString();
        else
            month = date.Month.ToString();
        UserInfo user = (UserInfo)Session["user"];
        string sql = "";
        if(act=="initdatagrid1")
         sql = string.Format("select Target,Event,EffectiveTime,Type,Bpoint,CheckState from accumulate_points where" +
            " DATE_FORMAT(EffectiveTime,'%Y-%m')='{0}-{1}' and Proposer='{2}' ORDER BY CheckState DESC,EffectiveTime DESC",
            date.Year,month, user.userName);
        else
            sql = string.Format("select Proposer,Event,EffectiveTime,Type,Bpoint,CheckState from accumulate_points where" +
            " DATE_FORMAT(EffectiveTime,'%Y-%m')='{0}-{1}' and Target='{2}' ORDER BY CheckState DESC,EffectiveTime DESC",
            date.Year, month, user.userName);
        DataSet ds = SqlHelper.Find(sql);
        string json = "";
        if(ds!=null&&ds.Tables[0].Rows.Count>0)
        {
            json = JsonHelper.DataTable2Json(ds.Tables[0]);
        }
        Response.Write(json);
    }
}