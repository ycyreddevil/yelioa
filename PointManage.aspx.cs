using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Newtonsoft.Json.Linq;

public partial class PointManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sort = Request.Params["sort"];
        string action = Request.Form["act"];
        string order = Request.Params["order"];
        if (action == null)
        {
            action = Request.Params["act"];
        }
        if (!string.IsNullOrEmpty(sort))
        {    
            string data = Request.Params["data"];
            Response.Clear();
            if (data == "getdata2")
            {
                Session["sort2"] = sort;
                Session["order2"] = order;
                Getdata2();
            }
            else if (data == "getdata3")
            {                               
                    Session["sort3"] = sort;
                    Session["order3"] = order;            
                    Getdata(sort, order, data);
            }
            else
            {
                Session["sort1"] = sort;
                Session["order1"] = order;
                Getdata1();
            }
            Response.End();
        }
        else if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if (action == "getdata1")
            {
                Session["pagenumber1"] = Request.Form["pagenumber"];
                Session["pagesize1"] = Request.Form["pagesize"];
                Getdata1();
            }
            else if(action=="getdata2")
            {
                Session["date1"] = Request.Form["date1"];
                Session["date2"] = Request.Form["date2"];
                Session["name"] = Request.Form["name"];
                Session["pagenumber2"] = Request.Form["pagenumber"];
                Session["pagesize2"] = Request.Form["pagesize"];
                Getdata2();
            }
            else if (action == "getdata3")
            {
                Session["date3"] = Request.Form["date3"];
                Session["departments"] = Request.Form["departments"];
                Getdata(sort, order, action);
            }
            else if(action=="pass")
            {
                PassOrRejectOrBack("已审核");
            }
            else if (action == "reject")
            {
                PassOrRejectOrBack("已拒绝");
            }
            else if (action == "back")
            {
                PassOrRejectOrBack("未审核");
            }
            else if (action == "getTree")
            {
                Response.Write(GetTree());
            }
            else if(action== "exportExcel")
            {
                Response.Write(exportExcel());
            }
            else if(action=="upload")
            {
                Response.Write(uploadFile());
            }
            else if (action == "getUsers")
            {
                Response.Write(GetUsers());
            }
            else if (action == "setRight")
            {
                Response.Write(SetRight());
            }
            else if (action == "addPoint")
            {
                Response.Write(AddPoint());
            }
            Response.End();
        }
        
        
    }

    private void Getdata(string sort,string order,string act)
    {
        DataTable dt = new DataTable();
        string json = "";
        
            dt = Getdata3();
            if (string.IsNullOrEmpty(sort))
            {
                sort = "total_point";
                order = "desc";
                Session["sort3"] = sort;
                Session["order3"] = order;
            }
            DataTable dt1 = dt;
            if (!string.IsNullOrEmpty(sort))
                dt1 = PinYinHelper.SortByPinYin(dt, sort, order);
            json = JsonHelper.DataTable2Json(dt1);        
        Response.Write (json.ToString());
    }



    private void Getdata1()
    {
        string sort = "";
        string order = "";
        if (Session["sort1"] == null)
        {
            sort = "CreatingTime";
            order = "desc";
        }
        else
        {
            sort = Session["sort1"].ToString();
            order = Session["order1"].ToString();
        }
        string json = "";
        int pagenumber = Convert.ToInt32(Session["pagenumber1"]);
        int pagesize = Convert.ToInt32(Session["pagesize1"]);
        int begin = (pagenumber - 1) * pagesize - 1;
        if (begin == -1)
            begin = 0;
        string sql = "select Id,Proposer,Target,Event,Type,CreatingTime,EffectiveTime,Bpoint,CheckState,Opinion from accumulate_points where CheckState='未审核' order by " + sort + " " + order + " limit " + begin + "," + pagesize +
            ";select count(*) from accumulate_points where CheckState='未审核'";
        DataSet ds = SqlHelper.Find(sql);
        if(ds!=null&&ds.Tables[0].Rows.Count>0)
        {
            Object ob = new { total = ds.Tables[1].Rows[0][0], rows = ds.Tables[0] };
            json = JsonHelper.SerializeObject(ob);           
        }
        Response.Write(json);
    }

    private void Getdata2()
    {
        string date1 = Session["date1"].ToString();
        string date2 = Session["date2"].ToString();
        int pagenumber = Convert.ToInt32(Session["pagenumber2"]);
        int pagesize = Convert.ToInt32(Session["pagesize2"]);
        int begin = (pagenumber - 1) * pagesize - 1;
        if (begin == -1)
            begin = 0;
        DateTime date3 = Convert.ToDateTime(date1);
        DateTime date4 = Convert.ToDateTime(date2);
        string name = Session["name"].ToString();
        string sort = "";
        string order = "";
        if (Session["sort2"] == null)
        {
            sort = "EffectiveTime";
            order = "desc";
        }
        else
        {
             sort = Session["sort2"].ToString();
             order = Session["order2"].ToString();
        }
        string sql = string.Format("select * from accumulate_points where  CheckState in('已审核','已拒绝') and  EffectiveTime between '{0}-{1}-{2}' and '{3}-{4}-{5}'"
           , date3.Year, date3.Month, date3.Day, date4.Year, date4.Month, date4.Day);
        string sql2 = "";
        if (name != "")
        {
            sql2 += "and Target in('";
            string sql1 = "select distinct userName from v_user_department_post";
            DataSet ds1 = SqlHelper.Find(sql1);
            foreach (DataRow row in ds1.Tables[0].Rows)
            {
                if (PinYinHelper.IsEqual(row["userName"].ToString(),name)
                   || row["userName"].ToString().Trim().Contains(name)
                    )
                    sql2 += row["userName"] + "','";
            }
            sql2 = sql2.Substring(0, sql2.Length - 2);
            sql2 += ")";
        }       
        sql += sql2 + "order by " + sort + " " + order + " "+ string.Format("limit {0},{1};select count(*) from " +
            "accumulate_points where  CheckState='已审核' and  EffectiveTime between '{2}-{3}-{4}' and '{5}-{6}-{7}'"
           ,begin, pagesize, date3.Year, date3.Month, date3.Day, date4.Year, date4.Month, date4.Day)+sql2;      
        DataSet ds = SqlHelper.Find(sql);
        Object ob = new { total = ds.Tables[1].Rows[0][0], rows = ds.Tables[0] };
        string json = JsonHelper.SerializeObject(ob);
        Response.Write(json);
    }
    private DataTable Getdata3()
    {
        string date3 = Session["date3"].ToString();
        DateTime date = Convert.ToDateTime(date3);
        string month = "";
        if (date.Month < 10)
            month = "0";
        month += date.Month.ToString();
        int month1, month2, day;
        if (date.Month > 0 && date.Month < 4)
        {
            month1 = 1;
            month2 = 3;
            day = 31;
        }
        else if (date.Month > 3 && date.Month < 7)
        {
            month1 = 4;
            month2 = 6;
            day = 30;
        }
        else if (date.Month > 6 && date.Month < 10)
        {
            month1 = 7;
            month2 = 9;
            day = 30;
        }
        else
        {
            month1 = 10;
            month2 = 12;
            day = 31;
        }
        string departments = Session["departments"].ToString();
        List <string>  departmentid = JsonHelper.DeserializeJsonToObject<List<string>>(departments);
        //string[] strs = departmentid.Split(',');
        string sql1 = "('";
        foreach (string str in departmentid)
        {
            sql1 += str;
            sql1 += "','";
        }
        sql1 = sql1.Substring(0, sql1.Length - 2);
        sql1 += ")";
        string sql = string.Format("SELECT DISTINCT d.Target,GROUP_CONCAT(DISTINCT d.department)as department,a.month_point," +
            "b.season_point,c.year_point,d.total_point,e.month_add_point_times,e.month_add_point,f.month_cut_point_times,f.month_cut_point  FROM" +
            "(SELECT ap.Target,ud.department,sum(ap.Bpoint) as month_point FROM accumulate_points as ap JOIN v_user_department_post as ud ON ap.Target = ud.userName WHERE ap.CheckState = '已审核' AND DATE_FORMAT(ap.EffectiveTime, '%Y-%m') ='{0}-{1}'AND departmentId IN" +
             sql1 + "GROUP BY ud.userName,ud.department,ud.postName) as a RIGHT JOIN" +
             "(SELECT ap.Target, ud.department, sum(ap.Bpoint) as season_point FROM accumulate_points as ap JOIN v_user_department_post as ud ON ap.Target = ud.userName WHERE ap.CheckState = '已审核' AND  ap.EffectiveTime BETWEEN '{2}-{3}-01'AND '{4}-{5}-{6}'AND departmentId IN" +
             sql1 + "GROUP BY ud.userName,ud.department,ud.postName) as b on a.Target=b.Target RIGHT JOIN" +
             "(SELECT ap.Target, ud.department, sum(ap.Bpoint) as year_point FROM accumulate_points as ap JOIN v_user_department_post as ud ON ap.Target = ud.userName WHERE ap.CheckState = '已审核' AND  DATE_FORMAT(ap.EffectiveTime, '%Y') ='{7}'AND departmentId IN" +
             sql1 + "GROUP BY ud.userName,ud.department,ud.postName) c on c.Target=b.Target RIGHT JOIN" +
             "(SELECT ap.Target, ud.department, sum(ap.Bpoint) as total_point FROM accumulate_points as ap JOIN v_user_department_post as ud ON ap.Target = ud.userName WHERE ap.CheckState = '已审核' AND departmentId IN" +
             sql1 + "GROUP BY ud.userName,ud.department,ud.postName) d on c.Target=d.Target " +
             " LEFT JOIN( SELECT ap.Proposer, ud.department,count(ap.Bpoint)as month_add_point_times,  sum(ap.Bpoint) AS month_add_point FROM accumulate_points AS ap JOIN v_user_department_post AS ud ON ap.Proposer = ud.userName" +
            " WHERE ap.CheckState = '已审核' AND DATE_FORMAT(ap.EffectiveTime, '%Y-%m') = '{8}-{9}' AND ap.Bpoint>0 and departmentId IN"+sql1+" GROUP BY ud.userName, ud.department, ud.postName" +
             ")e ON e.Proposer = d.Target LEFT JOIN(SELECT ap.Proposer, ud.department, count(ap.Bpoint) as month_cut_point_times , sum(ap.Bpoint) AS month_cut_point FROM accumulate_points AS ap JOIN v_user_department_post AS ud ON ap.Proposer = ud.userName" +
            " WHERE ap.CheckState = '已审核' AND DATE_FORMAT(ap.EffectiveTime, '%Y-%m') = '{10}-{11}' AND ap.Bpoint<0 and departmentId IN" + sql1 + " GROUP BY ud.userName, ud.department, ud.postName" +
             ")f ON e.Proposer = f.Proposer group by d.Target", date.Year, month, date.Year, month1, date.Year, month2, day, date.Year,date.Year,month, date.Year, month);
             
        DataSet ds = SqlHelper.Find(sql);
        foreach (DataRow dw in ds.Tables[0].Rows)
        {
            if (dw[2].Equals(DBNull.Value))
                dw[2] = 0;
            if (dw[3].Equals(DBNull.Value))
                dw[3] = 0;
            if (dw[4].Equals(DBNull.Value))
                dw[4] = 0;
            if (dw[5].Equals(DBNull.Value))
                dw[5] = 0;
            if (dw[6].Equals(DBNull.Value))
                dw[6] = 0;
            if (dw[7].Equals(DBNull.Value))
                dw[7] = 0;
            if (dw[8].Equals(DBNull.Value))
                dw[8] = 0;
            if (dw[9].Equals(DBNull.Value))
                dw[9] = 0;

        }
        return ds.Tables[0];
    }

    private string uploadFile()
    {
        string res = "读取文件失败！";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {            
                dt.Columns.Add("Proposer");
                dt.Columns.Add("CreatingTime");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!dt.Rows[i]["事件日期"].ToString().Contains("/") && !dt.Rows[i]["事件日期"].ToString().Contains("-"))
                        dt.Rows[i]["事件日期"]=DateTime.FromOADate(double.Parse(dt.Rows[i]["事件日期"].ToString())).ToString();
                    dt.Rows[i]["Proposer"] = GetUserInfo().userName;
                    dt.Rows[i]["CreatingTime"] = DateTime.Now.ToString();
                }
                dt.Columns["被申请人"].ColumnName = "Target";
                dt.Columns["B积分"].ColumnName = "Bpoint";
                dt.Columns["事件"].ColumnName = "Event";
                dt.Columns["事件类型"].ColumnName = "Type";
                dt.Columns["事件日期"].ColumnName = "EffectiveTime";
                res = SqlHelper.Exce(SqlHelper.GetInsertIgnoreString(dt, "accumulate_points"));                      
        }
        return res;
    }
    protected string exportExcel()
    {

        DataTable dt = PinYinHelper.SortByPinYin(Getdata3(), Session["sort3"].ToString(), Session["order3"].ToString());
        dt.Columns.Remove(dt.Columns[10]);
        JObject resultJObject = new JObject();
        string[] chineseHeaders = new string[] { "姓名", "部门", "月度积分", "季度积分", "年度积分", "总积分","月度加分次数","月度加分数","月度扣分次数","月度扣分数" };
        if (dt == null || dt.Columns.Count == 0)
        {
            resultJObject.Add("msg", "暂无数据导出");
        }
        else
        {
            ExcelHelperV2_0.ExportByWeb(dt, "部门积分信息", "部门积分信息.xls",chineseHeaders);
            resultJObject.Add("msg", "导出成功");
        }
        return resultJObject.ToString();
    }
    private void PassOrRejectOrBack(string str)
    {
        string json = Request.Form["data"];
        UserInfo user = (UserInfo)Session["user"];
        string res = "";
        DataTable dt = JsonHelper.Json2Dtb(json);
        if (dt!=null&& dt.Rows.Count > 0)
        {
            WxCommon wx = new WxCommon("mPointApply", "i0SFeOuq3eonsbAWYfmAnrB0k_4K5d3Ub7Y6Z-KkYrc",
             "1000008", "http://yelioa.top/mPointApply.aspx");
            JObject jObject = new JObject();
            jObject.Add("url", "http://yelioa.top/mPointApplyRecord.aspx");
            jObject.Add("btntxt", "单据详情");
            jObject.Add("description", "");
            jObject.Add("title", "积分申请");
            string str1 = "";
            string sql = "";
            if(str!="未审核")
            {
                dt.Columns.Add("CheckTime", Type.GetType("System.String"));
                dt.Columns.Add("Auditor", Type.GetType("System.String"));
            }
            foreach (DataRow dw in dt.Rows)
            {
                jObject["url"] = "http://yelioa.top/mPointDetail.aspx?ids=" + dw["Id"].ToString();
                dw["CheckState"] = str;
                if (str == "未审核")
                {
                    dw["CheckTime"] = "";
                    dw["Auditor"] = "";
                }
                else
                {
                    dw["CheckTime"] = DateTime.Now.ToString();
                    dw["Auditor"] = user.userName;
                }
                string condition = "where Id=" + dw[0];
                sql = SqlHelper.GetUpdateString(dw, "accumulate_points", condition);
                str1= SqlHelper.Exce(sql);
                string state = str;
                if (state == "未审核")
                    state = "已撤销";
                if (str1.Contains("操作成功"))
                {
                    if (str != "已审核")
                    {
                        jObject["description"] = "申请人：" + dw["Proposer"] + "<br>被申请人：" + dw["Target"] + "<br>事件：" + dw["Event"] +
                      "<br>B积分：" + dw["Bpoint"] + "<br>状态：" + state + "<br>审批原因：" + dw["Opinion"] + "<br>审批人：" + user.userName;
                        if (wx.SendChatGroupTextMsg("PointMessageChatGroup", "textcard", jObject).Contains("ok"))
                            res += "发布成功";
                    }
                    else
                        res += "发布成功";
                }
                
            }
            string ret = "发布成功";
            int count = (res.Length - res.Replace(ret, "").Length) / ret.Length;
            if (count == dt.Rows.Count)
            {              
                res = "操作发布成功！";
            }
        }
        else
            res = "请选择所要操作的记录！";
        Response.Write(res); 
    }


    private string GetUsers()
    {
        string searchStr = Request.Form["searchStr"];
        string sql = "select distinct userName from users where isValid='在职'";
        DataSet ds = SqlHelper.Find(sql);
        if (ds == null && ds.Tables[0].Rows.Count == 0)
        {
            return "";
        }
        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("text", Type.GetType("System.String"));
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            if (PinYinHelper.IsEqual(row[0].ToString(), searchStr)
                    || row[0].ToString().Trim().Contains(searchStr)
                     )
                dt.Rows.Add(row[0], row[0]);
        }
        return JsonHelper.DataTable2Json(dt);
    }

    private string SetRight()
    {
        string data = Request.Form["data"].ToString();
        DataTable dt = JsonHelper.Json2Dtb(data);
        string sql = "delete from new_right where wechatUserId=(select wechatUserId from users where userName='" + dt.Rows[0][1] +
            "');insert into new_right (wechatUserId,PointApply) values((select wechatUserId from users where userName='" + dt.Rows[0][1] +"'),'"+ dt.Rows[1][1]+"')";
        string res = SqlHelper.Exce(sql);
        if ((res.Length - res.Replace("操作成功", "").Length) / "操作成功".Length == 2)
            res = "操作成功!";
        return res;
    }

    private string AddPoint()
    {
        string data = Request.Form["data"];
        string date = Request.Form["date"];
        DataTable dt = JsonHelper.Json2Dtb(data);
        string sql = "insert into accumulate_points (Proposer,Target,EffectiveTime,Bpoint,Type,Event,CreatingTime) values('" + GetUserInfo().userName + "','" + dt.Rows[0][1] + "','" + date + "','" + dt.Rows[1][1] + "','奖分','基础积分','" + DateTime.Now.ToString() + "')";
        string res = SqlHelper.Exce(sql);
        if ((res.Length - res.Replace("操作成功", "").Length) / "操作成功".Length == 1)
            res = "操作成功!";
        return res;
    }

    private string GetTree()
    {
        DataSet ds = UserInfoManage.getTree(GetUserInfo().companyId.ToString());
        if (ds == null)
        {
            return "F";
        }
        DepartmentTreeHelper tree = new DepartmentTreeHelper(ds.Tables[0]);
        string json = tree.GetJson();
        return json;
    }



    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }

}