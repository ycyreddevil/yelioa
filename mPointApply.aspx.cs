using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Newtonsoft.Json.Linq;

public partial class mPointApply : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mPointApply",
           "i0SFeOuq3eonsbAWYfmAnrB0k_4K5d3Ub7Y6Z-KkYrc",
           "1000008",
           "http://yelioa.top/mPointApply.aspx");
        UserInfo user = new UserInfo();
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        if (Common.GetApplicationValid("mPointApply.aspx") == "0")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>location.href='Default.aspx';</script>");
            Response.End();
            return;
        }
        user = (UserInfo)Session["user"];
         DataSet ds = SqlHelper.Find("select PointApply from new_right where wechatUserId='" + user.wechatUserId+"'");
        if(ds==null||ds.Tables[0].Rows.Count==0|| ds.Tables[0].Rows[0][0].ToString()=="0")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('抱歉，您无访问此页面的权限！')</script>");
            Response.End();
            return;
        }
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if(action=="applypoint")
            {
                Response.Write(ApplyPoint((int)ds.Tables[0].Rows[0][0]));
            }
            else if(action=="findTarget")
            {
               FindTarget();
            }
            Response.End();
        }
    }

    private string ApplyPoint(int right)
    {
        string date = Request.Form["date"].ToString();
        string type = Request.Form["type"].ToString();
        string Event = Request.Form["event"].ToString();
        string targets = Request.Form["targets"].ToString();
        DataTable targetTB = JsonHelper.Json2Dtb(targets);
        UserInfo user =(UserInfo) Session["user"];
        targetTB.Columns.Add("CreatingTime", Type.GetType("System.String"));
        targetTB.Columns.Add("Proposer", Type.GetType("System.String"));
        targetTB.Columns.Add("EffectiveTime", Type.GetType("System.String"));
        targetTB.Columns.Add("Type", Type.GetType("System.String"));
        targetTB.Columns.Add("Event", Type.GetType("System.String"));
        string txt = "申请人：" + user.userName;
        string ids = "";
        foreach (DataRow row in targetTB.Rows)
        {
            if (row["Target"].ToString() == user.userName)
                return "抱歉，您无法给自己进行加分或者扣分！";
            else if (right < System.Math.Abs(Convert.ToInt32(row["Bpoint"])))
                return "您所请求的加分超出您的权限范围，您的权限为-" + right + "与" + right + "之间，请重新输入！";
            else
            {
                row["CreatingTime"] = DateTime.Now.ToString();
                row["Proposer"] = user.userName;
                row["EffectiveTime"] = date;
                row["Type"] = type;
                row["Event"] = Event;
                if (type == "奖分")
                {
                    txt += "<br>被申请人：" + row["Target"] + ",B积分：" + row["Bpoint"];
                }
                else
                {
                    txt += "<div class=\"highlight\">被申请人：" + row["Target"] + ",B积分：" + row["Bpoint"]+"</div>";
                }
                string sql = SqlHelper.GetInsertString(row, "accumulate_points");
                string res = SqlHelper.InsertAndGetLastId(sql);
                JObject jObject = JObject.Parse(res);
                if (jObject["Success"].ToString() != "1")
                    return jObject["message"].ToString();
                else
                    ids+=jObject["Id"].ToString()+",";
            }
        }

        ids = ids.Substring(0, ids.Length - 1);
            WxCommon wx = new WxCommon("mPointApply", "i0SFeOuq3eonsbAWYfmAnrB0k_4K5d3Ub7Y6Z-KkYrc",
          "1000008", "http://yelioa.top/mPointApply.aspx");
            JObject message = new JObject();
            if(type=="扣分")
                message.Add("title", "扣分申请");
            else
                message.Add("title", "加分申请");
            txt += "<br>事件：" + Event +  "<br>状态：已申请";
            message.Add("description", txt);
            message.Add("url", "http://yelioa.top/mPointDetail.aspx?ids="+ids);
            message.Add("btntxt","单据详情");
            string resSendmsg = wx.SendChatGroupTextMsg("PointMessageChatGroup", "textcard",message);
            if (resSendmsg.Contains("ok"))
                return "提交发布成功！";
            else
                return "提交成功，群发消息发送失败！\n" + resSendmsg;
    }

    private void FindTarget()
    {
        string name = Request.Form["name"];         
        string json = "";
        string sql = "select distinct wechatUserId,userName from users where isValid='在职' ORDER BY userName DESC";
        DataSet ds = SqlHelper.Find(sql);
        if (ds != null)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("value", Type.GetType("System.String"));
            dt.Columns.Add("target", Type.GetType("System.String"));
            int i = 0;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (PinYinHelper.IsEqual(row["userName"].ToString(), name)
                   || row["userName"].ToString().Trim().Contains(name)
                    )

                    dt.Rows.Add(row["wechatUserId"], row["userName"]);

            }
            json = JsonHelper.DataTable2Json(dt);
        }
        Response.Write(json);
    }
}