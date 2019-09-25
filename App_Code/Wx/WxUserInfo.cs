using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

/// <summary>
/// WxUserInfo 的摘要说明
/// </summary>
public class WxUserInfo
{
    public WxUserInfo()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    private string Corpid = "wx9f620907462561ca";
    private string UserInfoAppSecret = "A2Tm3YhMzlZCI7Hbw5v4Ye3Jb8KzXbqrAVOSTLsYrns";
    private string AgentId = "1000002";

    public string GetWxToken(string AppSecret)
    {
        string WxToken = CookieHelper.GetCookieValueStatic(AppSecret + "WxToken");
        if (string.IsNullOrEmpty(WxToken))
        {
            WxToken = GetWxTokenFromWx(AppSecret);
        }
        return WxToken;
    }
    public string GetWxTokenFromWx(string AppSecret)
    {
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}",
               Corpid, AppSecret);

        string res = HttpHelper.Get(url);
        Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(res);
        if (!dict.Keys.Contains("access_token"))
        {
            return res;
        }
        else
        {
            int expires_in = Convert.ToInt32(dict["expires_in"]);
            CookieHelper.SetCookieStatic(AppSecret + "WxToken", dict["access_token"].ToString(), DateTime.Now.AddSeconds(expires_in));
            return dict["access_token"].ToString();
        }

    }

    public object GetWxDepartmentJson(string id)
    {
        string wxToken = GetWxToken(UserInfoAppSecret);
        string url = "";
        if (string.IsNullOrEmpty(id))
        {
            url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}&id=", wxToken);
        }
        else
        {
            url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}&id={1}", wxToken, id);
        }
        string json = HttpHelper.Get(url);
        Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(json);
        if (dict["errcode"].ToString() != "0")//token失效,重新获取token
        {
            wxToken = GetWxTokenFromWx(UserInfoAppSecret);
            url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}", wxToken);
            json = HttpHelper.Get(url);
            dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(json);
        }

        return dict["department"];
    }

    /// <summary>
    /// 按照从企业微信端获取到的部门信息获取部门名称
    /// </summary>
    /// <param name="departmentsId">从企业微信端获取到的部门ID信息</param>
    /// <returns>部门名称，可以用于按部门名称查询单据，例如："(部门1，部门2)"</returns>
    public string  GetWxDepartmentNameByID(string departmentsId)
    {
        object obj = ((JArray)GetWxDepartmentJson(null));
        JArray jar = (JArray)(obj);//获取所有部门信息
        string str = departmentsId.Replace("[", "").Replace("]", "").Replace("\r\n", "").Replace(" ", "");
        string[] departs = str.Split(',');
        string res = "(";

        foreach(JObject jobj in jar)
        {
            if(jobj["id"].ToString() == str)
                res += jobj["name"].ToString() + ",";
        }
        if(jar.Count >0 )
        {
            res = res.Substring(0, res.Length - 1);
        }
        res += ")";
        return res;
    }

    public object GetWxUserInfoJsonByDepartmentId(string id)
    {
        string wxToken = GetWxToken(UserInfoAppSecret);
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={0}&department_id={1}&fetch_child=1"
            , wxToken, id);
        string json = HttpHelper.Get(url);
        Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(json);
        if (dict["errcode"].ToString() != "0")//token失效,重新获取token
        {
            wxToken = GetWxTokenFromWx(UserInfoAppSecret);
            url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={0}&department_id={1}&fetch_child=1"
            , wxToken, id);
            json = HttpHelper.Get(url);
            dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(json);
        }
        return dict["userlist"];
    }

    public object GetOnlyWxUserInfoJsonByDepartmentId(string id)
    {
        string wxToken = GetWxToken(UserInfoAppSecret);
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={0}&department_id={1}&fetch_child=0"
            , wxToken, id);
        string json = HttpHelper.Get(url);
        Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(json);
        if (dict["errcode"].ToString() != "0")//token失效,重新获取token
        {
            wxToken = GetWxTokenFromWx(UserInfoAppSecret);
            url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={0}&department_id={1}&fetch_child=0"
            , wxToken, id);
            json = HttpHelper.Get(url);
            dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(json);
        }
        return dict["userlist"];
    }

    public object GetWxUserInfoJsonByUserId(string id)
    {
        string wxToken = GetWxToken(UserInfoAppSecret);
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}"
            , wxToken, id);
        string json = HttpHelper.Get(url);
        JObject jobj = JObject.Parse(json);
        if (jobj["errcode"].ToString() != "0")
        {
            return null;
        }
        else
            return jobj;
    }

    private void GotoGetCode(HttpContext context, string RedirectUri)
    {
        string randomString = ValideCodeHelper.GetRandomCode(16);
        context.Session["randomString"] = randomString;
        //
        //string urlForGettingCode = string.Format(@"https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid={0}" +
        //        "&agentid={1}&redirect_uri={2}&state={3}",
        //        Corpid, AgentId, HttpUtility.UrlEncode(RedirectUri), randomString);
        string urlForGettingCode = string.Format(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}"
            + "&redirect_uri={1}&response_type=code&scope=snsapi_base&agentid={2}&state={3}#wechat_redirect"
            , Corpid, HttpUtility.UrlEncode(RedirectUri), AgentId, randomString);
        context.Response.Redirect(urlForGettingCode, false);
    }

    public List<string> WechartUserIdToUserId(List<string> wechatUserIds)
    {
        List<string> sqlList = new List<string>();
        foreach (string wechatUserId in wechatUserIds)
        {
            string sql = "select userId from users where wechatUserId = '" + wechatUserId + "'";
            sqlList.Add(sql);
        }
        DataSet ds = SqlHelper.Find(sqlList.ToArray());

        List<string> userIdList = new List<string>();

        if (ds == null)
            return null;
        foreach (DataTable dt in ds.Tables)
        {
            userIdList.Add(dt.Rows[0][0].ToString());
        }
        return userIdList;
    }

    public List<string> userIdToWechatUserId(List<string> userIds)
    {
        List<string> sqlList = new List<string>();
        foreach (string userId in userIds)
        {
            string sql = "select wechatUserId from users where userId = '" + userId + "'";
            sqlList.Add(sql);
        }
        DataSet ds = SqlHelper.Find(sqlList.ToArray());

        List<string> wechatUserIdList = new List<string>();

        if (ds == null)
            return null;
        foreach (DataTable dt in ds.Tables)
        {
            wechatUserIdList.Add(dt.Rows[0][0].ToString());
        }
        return wechatUserIdList;
    }

    public string userIdToWechatUserId(string userId)
    {
        string sql = "select wechatUserId from users where userId = '" + userId + "'";

        DataSet ds = SqlHelper.Find(sql);

        if (ds  == null)
            return null;
        
        return ds.Tables[0].Rows[0][0].ToString();
    }

    public Boolean isSubmitterLeader(string submitterId, string approverUserId)
    {
        string submitterDepartmentId = SqlHelper.Find("select departmentId from v_user_department_post where userId = '" + submitterId + "'").Tables[0].Rows[0][0].ToString();
        string approverDepartmentId = SqlHelper.Find("select departmentId from v_user_department_post where userId = '" + approverUserId + "'").Tables[0].Rows[0][0].ToString();

        DataTable dt = new DataTable();
        dt.Columns.Add("departmentId", Type.GetType("System.String"));
        dt.Rows.Add(approverDepartmentId);

        DataTable table = RightSrv.GetDepartmentIds(dt);
        string[] parentDepartmentIds = table.Rows[0][0].ToString().Split(',');
        Boolean flag = false;

        int index = 0;
        foreach (string parentDptId in parentDepartmentIds)
        {
            if (index == 0)
            {
                index++;
                continue;
            }
            if (parentDptId == submitterDepartmentId.ToString())
            {
                flag = true;
                break;
            }

            
        }

        return flag;
    }
}