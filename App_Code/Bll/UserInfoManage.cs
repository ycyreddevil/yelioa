using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

//using Newtonsoft;
using Newtonsoft.Json.Linq;

/// <summary>
/// UserInfoManage 的摘要说明
/// </summary>
public class UserInfoManage
{
    public UserInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static UserInfo GetUserInfo(string wechatUserId, ref string msg)
    {
        return UserInfoSrv.GetUserInfo(wechatUserId,ref msg);
    }

    /// <summary>
    /// 防止sql注入式攻击，过滤输入的字符串
    /// </summary>
    private static void AntiInjectionAttack(ref UserInfo user)
    {
        user.passWord = SqlHelper.MD5((user.passWord));
        user.userName = SqlHelper.Split(user.userName);
        user.mobilePhone = SqlHelper.Split(user.mobilePhone);
    }

    public static string Login(ref UserInfo user, ref string token)
    {
        
        AntiInjectionAttack(ref user);
        string res = UserInfoSrv.Login(ref user,ref token);
        //user.passWord = "888888";//隐藏真实密码
        return res;
    }

    public static List<DepartmentPost> GetDepartmentPostList(UserInfo user)
    {
        DataSet ds = UserInfoSrv.GetDepartmentPostList(user);
        if (ds == null || ds.Tables[0].Rows.Count==0)
            return null;
        //List<DepartmentPost> list = new List<DepartmentPost>();
        //foreach(DataRow row in ds.Tables[0].Rows)
        //{
        //    DepartmentPost dp = new DepartmentPost();
        //    dp = ModelHandler<DepartmentPost>.FillModel(row);
        //}
        List<DepartmentPost> list = ModelHandler<DepartmentPost>.FillModel(ds.Tables[0]);
        return list;
    }

    public static string ModifyPassword(ref UserInfo user)
    {
        user.passWord = SqlHelper.MD5((user.passWord));
        string res = UserInfoSrv.ModifyPassword(ref user);
        //user.passWord = "888888";//隐藏真实密码
        return res;
    }

    public static string CookieLogin(ref UserInfo user,string token)
    {
        //user.passWord = SqlHelper.Split(user.passWord);
        //user.userName = SqlHelper.Split(user.userName);
        //user.mobilePhone = SqlHelper.Split(user.mobilePhone);
        //string res = UserInfoSrv.Login(ref user,ref token);
        //user.passWord = "888888";//隐藏真实密码
        string res = "";
        //user = null;
        DataSet ds = UserInfoSrv.GetToken(token,user.userName);
        if (ds == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[1].Rows.Count == 0)
            return res;
        DateTime dtDb = Convert.ToDateTime(ds.Tables[0].Rows[0]["LoginTime"]).AddDays(7);
        
        if(dtDb.CompareTo(DateTime.Now) >-1)//时间在有效范围内
        {
            user = ModelHandler<UserInfo>.FillModel(ds.Tables[1].Rows[0]);
            //user.passWord = "888888";//隐藏真实密码
            if (user.userId == Convert.ToInt32(ds.Tables[0].Rows[0]["UserId"]))//用户ID与token相互匹配
            {       
                res = "登录成功";
            }            
        }
        return res;
    }

    

    public static bool IsLogined()
    {
        if (HttpContext.Current.Session["user"] == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static DataSet GetMembers(UserInfo user)
    {
        DataSet ds = UserInfoSrv.GetMembers(user);
        if(ds == null)
        {
            return null;
        }
        ds.Tables[0].Columns.Remove("passWord");
        return ds;
    }

    public static DataSet SearchMembers(UserInfo user,string search)
    {
        DataSet ds = GetMembers(user);
        if (ds == null)
        {
            return null;
        }



        return ds;
    }

    public static DataSet getInfos(Dictionary<string, string> dict)
    {
        UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
        DataSet ds = UserInfoSrv.getInfos(user.companyId.ToString(), dict);
        return ds;
    }

    public static DataSet getUserTree(string range)
    {
        UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
        DataSet ds = UserInfoSrv.getInfos(user.companyId.ToString(), range);
        return ds;
    }

    private static DataTable UserInfoProcess(DataSet ds)
    {
        DataTable dt = ds.Tables[0].Copy();
        dt.Columns.Add("department");
        if(!dt.Columns.Contains("post"))
            dt.Columns.Add("post");

        for (int i=0;i<dt.Rows.Count;i++)
        {
            DataRow row = dt.Rows[i];            
            string wechatUserId = row["wechatUserId"].ToString();
            DataRow[] rows = ds.Tables[1].Select(string.Format("wechatUserId = '{0}'", wechatUserId));
            if (rows.Length > 0)
            {
                string department = "";
                string post = "";
                foreach (DataRow r in rows)
                {
                    department += r["department"].ToString() + ",";
                    if(!string.IsNullOrEmpty(r["postName"].ToString()))
                        post += r["postName"].ToString() + ",";
                }
                department = department.Substring(0, department.Length - 1);
                if (!string.IsNullOrEmpty(post))
                    post = post.Substring(0, post.Length - 1);
                row["department"] = department;
                row["post"] = post;
            }                
        }
        return dt;
        //dt.Columns.Add("SortColumn_BeSureThereIsNoRepetitiveColumn");
        //for (int i = 0; i < dt.Rows.Count; i++)
        //{
        //    string tempStr = StringTools.JustKeepNumbers(dt.Rows[i]["wechatUserId"].ToString());
        //    dt.Rows[i]["SortColumn_BeSureThereIsNoRepetitiveColumn"] = tempStr;
        //}
        //DataView dv = dt.DefaultView;
        //dv.Sort = "SortColumn_BeSureThereIsNoRepetitiveColumn ASC";
        //return dv.ToTable();
    }

    public static DataTable getInfos(string companyId, string departId,string searchString)
    {
        DataSet ds = null;
        
        if(string.IsNullOrEmpty(departId))
        {
            ds = UserInfoSrv.getInfos(companyId);
        }
        else
        {
            ds = UserInfoSrv.getInfos(companyId, departId);
        }
        DataTable dt = null;
        if (ds != null)
        {
            DataTable dtUsing = UserInfoProcess(ds);
            if (string.IsNullOrEmpty(searchString))//搜索字符为空时，不搜索，直接返回
            {
                return dtUsing;
            }
            else
            {
                dt = dtUsing.Clone();
                foreach (DataRow row in dtUsing.Rows)
                {
                    if (row["employeeCode"].ToString().Trim().Contains(searchString)
                        || PinYinHelper.IsEqual(row["userName"].ToString(), searchString)
                        //|| PinYinHelper.IsEqual(row["fullName"].ToString(), searchString)
                        || row["userName"].ToString().Trim().Contains(searchString)
                        //|| row["fullName"].ToString().Trim().Contains(searchString))
                        )
                    {
                        //row["passWord"] = "888888";
                        dt.Rows.Add(row.ItemArray);
                        dt.Rows[dt.Rows.Count - 1]["passWord"] = "888888";
                        continue;
                    }
                }
            }
        }
        ClearMemory.Clear(ds);
        return dt;
    }

    public static DataSet GetSelectedTree(string wechatUserId)
    {
        return UserInfoSrv.GetSelectedTree(wechatUserId);
    }

    public static DataSet getTree(string companyId)
    {
        return UserInfoSrv.getTree(companyId);
    }

    public static DataTable GetAllUsers()
    {
        DataSet ds = UserInfoSrv.GetAllUsers();
        DataTable dt = null;
        if (ds != null)            
        {
            dt = ds.Tables[0].Copy();
            dt.Columns.Add("HanZiPinYinSortColumn");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tempStr = PinYinHelper.getPinyin(dt.Rows[i]["userName"].ToString());
                dt.Rows[i]["HanZiPinYinSortColumn"] = StringTools.JustKeepLetters(tempStr);
            }

            DataView dv = dt.DefaultView;
            dv.Sort = "HanZiPinYinSortColumn asc";
            return dv.ToTable();
        }
        else
            return dt;
    }

    public static DataSet GetUserDepartment()
    {
        return UserInfoSrv.GetUserDepartment();
    }

    public static string AddTree(Dictionary<string, string> dict)
    {
        string res= UserInfoSrv.AddTree(dict);
        SqlExceRes sqlRes = new SqlExceRes(res);
        return sqlRes.GetResultString("新建成功", "部门名称有重复，请重新输入");
    }

    public static string EditTree(Dictionary<string, string> dict,string id)
    {
        string res = UserInfoSrv.UpdateTree(dict, id);
        SqlExceRes sqlRes = new SqlExceRes(res);
        return sqlRes.GetResultString("操作成功", "");
    }

    public static string DeleteTree(string id)
    {
        string res = UserInfoSrv.DeleteTree(id);
        SqlExceRes sqlRes = new SqlExceRes(res);
        return sqlRes.GetResultString("操作成功", "");
    }

    public static string GetDepartRemark(string companyId, string id)
    {
        return UserInfoSrv.GetDepartRemark(companyId, id);
    }

    public static string Delete(string id)
    {
        string res = UserInfoSrv.Delete(id);
        SqlExceRes sqlRes = new SqlExceRes(res);
        return sqlRes.GetResultString("操作成功", "");
    }

    public static string InsertInfos(Dictionary<string, string> dict)
    {
        string res = UserInfoSrv.InsertInfos(dict);
        string[] strs1 = res.Split(':');
        string[] strs2 = strs1[1].Split(',');
        if (strs2[1].Contains("操作成功") && Convert.ToInt32(strs2[0]) > 0)
        {
            res = "新建成功";
        }
        else if (strs2[1].Contains("操作成功") && Convert.ToInt32(strs2[0]) == 0)
        {
            res = "人员名称已存在，请重新输入!";
        }
        else
        {
            res = strs2[1];
        }
        return res;
    }

    public static string InsertInfos(DataTable dt)
    {
        dt.Columns.Remove("序号");
        dt.Columns.Remove("工龄");
        dt.Columns.Remove("年龄");
        dt.Columns.Remove("部门");
        dt.Columns["工号"].ColumnName = "employeeCode";
        dt.Columns["姓名"].ColumnName = "userName";
        dt.Columns["性别"].ColumnName = "sex";
        //dt.Columns["部门"].ColumnName = "department";
        dt.Columns["岗位"].ColumnName = "post";
        dt.Columns["入职日期"].ColumnName = "hiredate";
        dt.Columns["转正日期"].ColumnName = "regularEmployeeDate";
        dt.Columns["状态"].ColumnName = "isValid";
        dt.Columns["毕业学校"].ColumnName = "graduationSchool";
        dt.Columns["专业"].ColumnName = "major";
        dt.Columns["学历"].ColumnName = "education";
        dt.Columns["手机号码"].ColumnName = "mobilePhone";
        dt.Columns["私人邮箱"].ColumnName = "email";
        dt.Columns["企业QQ"].ColumnName = "enterpriseQQ";
        dt.Columns["企业邮箱"].ColumnName = "enterpriseEmail";
        dt.Columns["银行开户行"].ColumnName = "bank";
        dt.Columns["账号"].ColumnName = "bankAccount";
        dt.Columns["身份证号"].ColumnName = "idNumber";
        dt.Columns["出生日期"].ColumnName = "birthday";
        dt.Columns["家庭住址"].ColumnName = "address";
        dt.Columns["紧急联络人"].ColumnName = "emergencyContact";
        dt.Columns["紧急联络人电话"].ColumnName = "emergencyContactNumber";
        dt.Columns["社保个人编号"].ColumnName = "socialSecurityNumber";

        return UserInfoSrv.InsertInfos(dt);
    }

    public static string UpdateInfos(Dictionary<string, string> dict, string id)
    {
        string res = UserInfoSrv.UpdateInfos(dict, id);
        SqlExceRes sqlRes = new SqlExceRes(res);
        return sqlRes.GetResultString("修改成功", "");
    }

    public static object CheckMobile(string mobile)
    {
        return UserInfoSrv.CheckMobile(mobile);
    }

    public static object CheckEmployeeCode(string code, string company)
    {
        return UserInfoSrv.CheckEmployeeCode(code,company);
    }

    public static DataSet CheckInfo(string code, string company, string mobile,string idNubmer)
    {
        return UserInfoSrv.CheckInfo(code, company,mobile,idNubmer);
    }

    public static string SaveDepartmentFromWx(object json)
    {
        //去掉所有回车换行符
        //json = json.Replace('\r', (char)0);
        //json = json.Replace('\n', (char)0);
        JArray listSrc = (JArray)(json);
        ArrayList listDst = new ArrayList();
        foreach(JObject val in listSrc)
        {
            //Dictionary<string, object> dict = (Dictionary < string, object> )val;
            Dictionary<string, string> newDict = new Dictionary<string, string>();
            //newDict.Add("Id", dict["id"].ToString());
            //newDict.Add("name", dict["name"].ToString());
            //newDict.Add("parentId", dict["parentid"].ToString());
            //newDict.Add("companyId", "1");
            //newDict.Add("state", "启用");
            //newDict.Add("order", dict["order"].ToString());
            string name = val["name"].ToString();
            int i = Convert.ToInt32(val["parentid"]);
            while(i>0)
            {
               foreach(JObject j in listSrc)
                {
                    if(i==Convert.ToInt32(j["id"]))
                    {
                        name = j["name"].ToString() +"/"+ name;
                        i = Convert.ToInt32(j["parentid"]);
                        break;
                    }
                }
            }
            newDict.Add("Id", val["id"].ToString());
            newDict.Add("name", name);
            newDict.Add("parentId", val["parentid"].ToString());
            newDict.Add("companyId", "1");
            newDict.Add("state", "启用");
            newDict.Add("orderForSameParent", val["order"].ToString());
            listDst.Add(newDict);
        }
        

        return UserInfoSrv.SaveDepartmentFromWx(listDst);
    }

    public static string SaveUserInfoFromWx(object json)
    {
        JArray listSrc = (JArray)(json);
        ArrayList listUser = new ArrayList();
        
        foreach (JObject val in listSrc)
        {
            UserAndDepartmentInfo userDpt = new UserAndDepartmentInfo();
            userDpt.UserInfo = new Dictionary<string, string>();
            userDpt.UserInfo.Add("wechatUserId", val["userid"].ToString());
            userDpt.UserInfo.Add("userName", val["name"].ToString());
            userDpt.UserInfo.Add("mobilePhone", val["mobile"].ToString());
            userDpt.UserInfo.Add("companyId", "1");
            userDpt.UserInfo.Add("isValid", "在职");
            if(val["gender"].ToString() == "1")
                userDpt.UserInfo.Add("sex", "男");
            else if (val["gender"].ToString() == "2")
                userDpt.UserInfo.Add("sex", "女");
            userDpt.UserInfo.Add("post", val["position"].ToString());
            userDpt.UserInfo.Add("avatar", val["avatar"].ToString());
            //newDict.Add("departmentId", SqlHelper.ToMultiData(val["department"]));
            userDpt.DepartmentPost = new ArrayList();
            JArray list = (JArray)(val["department"]);
            foreach (int jVal in list)
            {
                Dictionary<string, string> dictDepart = new Dictionary<string, string>();
                dictDepart.Add("wechatUserId", val["userid"].ToString());
                dictDepart.Add("departmentId", jVal.ToString());
                dictDepart.Add("postId", "71");//默认正式员工
                dictDepart.Add("isHead", val["isleader"].ToString());
                userDpt.DepartmentPost.Add(dictDepart);
            }
            listUser.Add(userDpt);
        }
        return UserInfoSrv.SaveUserInfoFromWx(listUser);
    }

    public static string InsertDepartPost(Dictionary<string,string> dict)
    {
        return UserInfoSrv.InsertDepartPost(dict);
    }

    public static string UpdateDepartPost(Dictionary<string, string> dict,string id)
    {
        return UserInfoSrv.UpdateDepartPost(dict,id);
    }

    public static string DeleteDepartPost(string id)
    {
        return UserInfoSrv.DeleteDepartPost( id);
    }
}