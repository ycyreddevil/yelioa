using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json.Linq;

/// <summary>
/// UserInfoSrv 的摘要说明
/// </summary>
public class UserInfoSrv
{
    public UserInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static UserInfo GetUserInfo()
    {
        return (UserInfo)HttpContext.Current.Session["user"];
    }

    public static UserInfo GetUserInfo(string wechatUserId,ref string msg)
    {
        DataSet dataSet = SqlHelper.Find("select * from users where wechatUserId = '" + wechatUserId + "'",ref msg);

        if (dataSet == null)
            return null;
        else if (dataSet.Tables[0].Rows.Count == 0)
        {
            msg = "未找到用户信息！";
            return null;
        }            
        else
            return ModelHandler<UserInfo>.FillModel(dataSet.Tables[0].Rows[0]);
    }

    public static string ModifyPassword(ref UserInfo user)
    {
        string sql = string.Format("update users set passWord='{0}' where userId={1}", user.passWord, user.userId);
        return SqlHelper.Exce(sql);
    }

    public static string Login(ref UserInfo user,ref string token)
    {
        string ret = "";
        string msg = "";
        string sql = string.Format("select userName from users where userName='{0}' or mobilePhone='{1}'\r\n;", user.userName, user.mobilePhone);
        sql += string.Format("select * from users where (userName='{0}' and passWord='{1}') or (mobilePhone='{2}' and passWord='{3}')",
                user.userName, user.passWord, user.mobilePhone, user.passWord);
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if(ds == null)
        {
            ret = msg;
        }
        else if(ds.Tables[0].Rows.Count == 0)
        {
            ret = "用户不存在！";
        }
        else if (ds.Tables[1].Rows.Count == 0)
        {
            ret = "用户名（手机号）或密码错误！";
        }
        else if(ds.Tables[1].Rows.Count > 0)
        {
            ret = "登录成功";
            user = ModelHandler<UserInfo>.FillModel(ds.Tables[1].Rows[0]);
            if (user.isValid != "在职")
                ret = "用户无权限登录！";
            //添加token
            sql = "delete from login_info where UserId = " + user.userId + "\r\n;";
            JObject obj = new JObject();
            obj.Add("UserId", user.userId);
            token = ValideCodeHelper.GetRandomCode(128);
            obj.Add("Token", token);
            obj.Add("LoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            string ip = getIp();
            obj.Add("IpAddress", ip);
            sql += SqlHelper.GetInsertString(obj, "login_info");
            SqlHelper.Exce(sql);
        }

        return ret;
    }

    private static string getIp()
    {
        if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
        else
            return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
    }

    public static DataSet GetToken(string token,string userName)
    {
        string sql = string.Format("select * from login_info where Token='{0}'\r\n;", token);
        sql += string.Format("select * from users where userName='{0}'", userName);
        return SqlHelper.Find(sql);
    }

    public static DataSet GetMembers(UserInfo user)
    {
        string sql = string.Format("select * from users where companyId={0} order by hiredate ", user.companyId);
        return SqlHelper.Find(sql);
    }

    public static DataSet getInfos(string companyId, string departId)
    {
        DataSet ds = getTree(companyId);
        List<string> list = new List<string>();
        list.Add(departId);
        int count = 0;
        while (list.Count != count)
        {
            count = list.Count;
            List<string> tempList = new List<string>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (list.Contains(row["Id"].ToString()))
                {
                    continue;
                }
                foreach (string id in list)
                {
                    if (row["parentId"].ToString() == id)
                    {
                        tempList.Add(row["Id"].ToString());
                    }
                }
            }
            list.AddRange(tempList.ToArray());
        }
        string departsId = "";
        foreach (string id in list)
        {
            departsId += string.Format("{0},", id);
        }
        departsId = departsId.Substring(0, departsId.Length - 1);
        string sql = "select * from users where isValid = '在职' and wechatUserId in ( select wechatUserId from ";
        sql += string.Format("user_department_post where departmentId in ({0})", departsId);
        sql += ") order by hiredate\r\n;";
        sql += string.Format("select * from v_user_department_post where departmentId in ({0})", departsId);
        return SqlHelper.Find(sql);
    }

    public static DataSet GetDepartmentPostList(UserInfo user)
    {
        string sql = string.Format("select * from user_department_post where wechatUserId = '{0}'", user.wechatUserId);
        return SqlHelper.Find(sql);
    }

    public static DataSet getInfos(string companyId, Dictionary<string,string> dict)
    {
        string sql = "";
        //Dictionary<string, int> dictRes = new Dictionary<string, int>();
        List<string> keys = new List<string>(dict.Keys);
        for (int i = 0; i < dict.Count; i++)
        {
            sql += string.Format("select departmentId,userName from users where companyId={0} and departmentId in ({1}) order by userName \r\n;"
            , companyId, dict[keys[i]]);
        }
        return SqlHelper.Find(sql);
    }

    public static DataSet getInfos(string companyId)
    {
        string sql = string.Format("select u.*, c.name as company from users u inner join companys c where u.companyId={0} order by u.hiredate"
            , companyId); 
        return SqlHelper.Find(sql);
    }

    public static DataSet getUserTree(string companyId, string range)
    {
        string sql = string.Format("select departmentId,userName,userId from users where companyId={0} and departmentId in ({1}) order by userName \r\n;"
            , companyId, range);
       
        return SqlHelper.Find(sql);
    }

    public static DataSet GetSelectedTree(string wechatUserId)
    {
        string sql = string.Format("select * from v_user_department_post where wechatUserId = '{0}'", wechatUserId);
        return SqlHelper.Find(sql);
    }

    public static DataSet GetUserDepartment()
    {
        string sql = string.Format("select * from v_user_department_post ");
        return SqlHelper.Find(sql);
    }

    public static DataSet GetAllUsers()
    {
        string sql = "select userName,userId,avatar from users where isValid = '在职'";
        return SqlHelper.Find(sql);
    }

    public static DataSet getTree(string companyId)
    {
        string sql = string.Format("select * from department where companyId={0} and state='启用' order by orderForSameParent DESC"
            , companyId);
        //string sql = string.Format("SELECT DISTINCT d.*, (select count(*) from users u2 where u2.departmentId like CONCAT('%(',d.Id,')%') "
        //    + "and u2.isValid = '在职' ) as count from department d "
        //    + "where d.companyId={0} and d.state='启用' order by d.orderForSameParent DESC", companyId);

        return SqlHelper.Find(sql);
    }

    public static string AddTree(Dictionary<string, string> dict)
    {
        //dict.Add("deep", string.Format("(select deep+1 from department where Id={0})", dict["parentId"]));
        //dict.Add("Id", "(SELECT max(Id)+1 FROM department)");
        string sql = SqlHelper.GetInsertIgnoreString(dict, "department");
        sql += string.Format("update department set deep=(select a.dp from(select (deep+1)dp from department where Id={0})a)," +
            "Id=(SELECT selectMinNotExistID()) where name='{1}' and parentId={2}"
            , dict["parentId"], dict["name"], dict["parentId"]);
        return SqlHelper.Exce(sql);
    }

    public static string UpdateTree(Dictionary<string, string> dict,string id)
    {
        string leaderWechatUserId = dict["wechatUserId"].ToString();
        string departmentId = dict["departmentId"].ToString();
        dict.Remove("wechatUserId"); dict.Remove("departmentId");
        int deepCmd = Convert.ToInt32(SqlHelper.Scalar(string.Format("select deep from department where Id={0}", dict["parentId"])));
        dict.Add("deep", (deepCmd + 1).ToString());
        string sql = SqlHelper.GetUpdateString(dict, "department", "where Id=" + id);
        //sql += string.Format("UPDATE department SET parentName = '{0}' where parentId={1}\r\n;", dict["name"], id);
        //sql += string.Format("UPDATE users SET department = '{0}' where departmentId={1}\r\n;", dict["name"], id);

        // 更新部门负责人
        dict.Add("departmentId", departmentId);
        DataTable dt = SqlHelper.Find(string.Format("select * from user_department_post where " +
            "departmentId = '{0}'", dict["departmentId"])).Tables[0];

        foreach (DataRow dr in dt.Rows)
        {
            if ("".Equals(leaderWechatUserId))
            {
                // 部门没有负责人 则全部设置为非负责人
                if (dr["isHead"].ToString().Equals("1"))
                {
                    sql += string.Format("update user_department_post set isHead = 0 where wechatUserId = '{0}' and departmentId = '{1}';",
                        dr["wechatUserId"], dict["departmentId"]);
                }
            }
            else
            {
                if (dr["wechatUserId"].Equals(leaderWechatUserId) && dr["isHead"].ToString().Equals("0"))
                {
                    // 更新为部门负责人
                    sql += string.Format("update user_department_post set isHead = 1 where wechatUserId = '{0}' and departmentId = '{1}';",
                        dr["wechatUserId"], dict["departmentId"]);
                }
                else if (!dr["wechatUserId"].Equals(leaderWechatUserId) && dr["isHead"].ToString().Equals("1"))
                {
                    // 更新为非部门负责人
                    sql += string.Format("update user_department_post set isHead = 0 where wechatUserId = '{0}' and departmentId = '{1}';",
                        dr["wechatUserId"], dict["departmentId"]);
                }
            }
        }

        return SqlHelper.Exce(sql);
    }

    public static string DeleteTree(string id)
    {
        string sql = string.Format("UPDATE department SET state='停用'  WHERE Id = {0};", id);
        sql += string.Format("UPDATE user_department_post SET departmentId = 1 WHERE departmentId = {0} AND wechatUserId NOT IN("+
                              "select a.wechatUserId from(SELECT * FROM user_department_post WHERE  departmentId = 1)a);", id);
        sql += string.Format("delete from user_department_post WHERE departmentId = {0} AND wechatUserId IN(select a.wechatUserId from(" +
                             "SELECT * FROM user_department_post WHERE departmentId = 1)a)",id);
        return SqlHelper.Exce(sql);
    }

    public static string GetDepartRemark(string companyId, string id)
    {
        string sql = string.Format("select remark from department where companyId={0} and Id={1} ", companyId, id);
        object res = SqlHelper.Scalar(sql);
        if (res == null)
            return null;
        else
            return res.ToString();
    }

    public static string Delete(string id)
    {
        string sql = string.Format("DELETE FROM users WHERE userId = {0}", id);
        return SqlHelper.Exce(sql);
    }

    public static string InsertInfos(Dictionary<string, string> dict)
    {
        string departmentName = SqlHelper.Scalar(string.Format("select name from department where Id={0}", dict["departmentId"])).ToString();
        dict.Add("department", departmentName);
        string sql = SqlHelper.GetInsertString(dict, "users");
        sql = sql.Replace("Insert", "Insert ignore ");//避免重复插入数据
        return SqlHelper.Exce(sql);
    }

    public static string InsertInfos(DataTable dt)
    {
        string sql = SqlHelper.GetInsertIgnoreString(dt, "users");
        return SqlHelper.Exce(sql);
    }

    public static string UpdateInfos(Dictionary<string, string> dict, string id)
    {
        //string departmentName = SqlHelper.Scalar(string.Format("select name from department where Id={0}", dict["departmentId"])).ToString();
        //dict.Add("department", departmentName);
        string sql = SqlHelper.GetUpdateString(dict, "users", "where userId=" + id);
        return SqlHelper.Exce(sql);
    }

    public static object CheckMobile(string mobile)
    {
        string sql = string.Format("select count(mobilePhone) from users where mobilePhone='{0}'", mobile);
        return SqlHelper.Scalar(sql);
    }

    public static object CheckEmployeeCode(string code, string companyId)
    {
        string sql = string.Format("select count(employeeCode) from users where employeeCode='{0}' and companyId={1}", code, companyId);
        return SqlHelper.Scalar(sql);
    }

    public static DataSet CheckInfo(string code, string companyId, string mobile, string idNubmer)
    {
        string sql = string.Format("select mobilePhone from users where mobilePhone='{0}'\r\n;", mobile);
        sql += string.Format("select employeeCode from users where employeeCode='{0}' and companyId={1}\r\n;", code, companyId);
        sql += string.Format("select idNumber from users where idNumber='{0}'\r\n;", idNubmer);
        return SqlHelper.Find(sql);
    }

    public static string SaveDepartmentFromWx(ArrayList list)
    {
        List<string> listSql = new List<string>();
        listSql.Add(string.Format("delete from department where companyId=1"));
        foreach(Dictionary<string, string> dict in list)
        {
            //string[] sqlCmd = new string[2];
            ////查询本地数据库是否有和微信端数据完全一样的记录，有则跳过
            //sqlCmd[0] = string.Format("select Id from department where Id={0} and name='{1}' and parentId={2} "
            //    + "and orderForSameParent={3}", dict["Id"], dict["name"], dict["parentId"], dict["orderForSameParent"]);
            ////查询本地数据库是否已有和微信端数据Id一样的记录，有则更新
            //sqlCmd[1] = string.Format("select Id from department where Id={0}", dict["Id"]);
            //string[] res = SqlHelper.Scalar(sqlCmd);

            //string sql = "";
            //if (!string.IsNullOrEmpty(res[0]))
            //    continue;
            //else if (!string.IsNullOrEmpty(res[1]))
            //{
            //    string id = dict["Id"];
            //    dict.Remove("Id");
            //    sql = SqlHelper.GetUpdateString(dict, "department", " where Id=" + id);
            //}
            //else
            //{
            //    sql = SqlHelper.GetInsertIgnoreString(dict, "department");
            //}
            string fileds = "";
            string values = "";
            fileds = string.Format("Id,name,parentId,companyId,orderForSameParent,state");
            values = string.Format("{0},'{1}',{2},1,{3},'启用'", dict["Id"], dict["name"], dict["parentId"], dict["orderForSameParent"]);
            string sql = string.Format("Insert into {0} ({1}) values ({2})", "department", fileds, values);

            listSql.Add(sql);
        }
        return SqlHelper.Exce(listSql.ToArray());
    }

    public static string SaveUserInfoFromWx(ArrayList listUser)
    {
        if (listUser == null || listUser.Count == 0)
            return "未在企业微信端获得用户数据！";

        List<string> listSql = new List<string>();
        string sql = "";
        sql = "delete from user_department_post";
        listSql.Add(sql);

        sql = "select * from users";
        DataSet ds = SqlHelper.Find(sql);

        foreach (UserAndDepartmentInfo userDpt in listUser)
        {
            if (ds == null)
                continue;
            int len = ds.Tables[0].Rows.Count;
            bool hasFindTheSameData = false;
            for (int i = len - 1; i >= 0; i--)
            {
                if (ds.Tables[0].Rows[i]["userName"].ToString() == userDpt.UserInfo["userName"])
                {
                    string name = userDpt.UserInfo["userName"];
                    userDpt.UserInfo.Remove("userName");
                    sql = SqlHelper.GetUpdateString(userDpt.UserInfo, "users", " ,isValid='在职' where userName='"
                        + name + "'");
                    listSql.Add(sql);//找到相同名字的数据则进行更新
                    ds.Tables[0].Rows.RemoveAt(i);
                    hasFindTheSameData = true;
                    break;
                }
            }
            if (!hasFindTheSameData)//未查询到相同名字的数据则插入数据
            {
                sql = SqlHelper.GetInsertIgnoreString(userDpt.UserInfo, "users");
                listSql.Add(sql);
            }

            //插入用户部门信息
            foreach (Dictionary<string, string> dictDepart in userDpt.DepartmentPost)
            {
                sql = SqlHelper.GetInsertIgnoreString(dictDepart, "user_department_post");
                listSql.Add(sql);
            }

        }
        //未在企微端获取到数据，但出现在本系统user表当中，说明已经离职
        foreach(DataRow row in ds.Tables[0].Rows)
        {
            sql = string.Format("Update users set isValid='离职' where userName = '{0}'"
                , row["userName"].ToString());
            listSql.Add(sql);
        }
        return SqlHelper.Exce(listSql.ToArray());
    }

    public static string InsertDepartPost(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertIgnoreString(dict, "user_department_post");
        return SqlHelper.Exce(sql);
    }

    public static string UpdateDepartPost(Dictionary<string, string> dict, string id)
    {
        string sql = SqlHelper.GetUpdateString(dict, "user_department_post", " where Id=" + id);
        return SqlHelper.Exce(sql);
    }

    public static string DeleteDepartPost(string id)
    {
        string sql = string.Format("delete from user_department_post where Id={0}", id);
        return SqlHelper.Exce(sql);
    }

    public static DataSet FindByUserId(string userId)
    {
        string sql = string.Format("select * from users where userId = {0}", userId);
        return SqlHelper.Find(sql);
    }
}