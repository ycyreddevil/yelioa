<%@ Application Language="C#" %>
<%@ Import Namespace="System.Timers" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Newtonsoft.Json.Linq" %>
<%@ Import Namespace="System.Collections.Generic" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        // 在应用程序启动时运行的代码
        System.Timers.Timer objTimer = new System.Timers.Timer();
        objTimer.Interval = 30000; //这个时间单位毫秒,比如10秒，就写10000
        objTimer.Enabled = true;
        objTimer.Elapsed += new ElapsedEventHandler(objTimer_Elapsed);
    }

    protected void objTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        int intHour = e.SignalTime.Hour;
        int intMinute = e.SignalTime.Minute;
        int intSecond = e.SignalTime.Second;

        if (intHour == 00 && intMinute == 00 && intSecond <= 30)
        {
            //找到报销中超过7天没有审批的人 发消息提醒
            string sql = string.Format("SELECT distinct t4.wechatUserId ,timestampdiff(day, t2.time, now())+1 Days FROM `yl_reimburse` t1 " +
        "inner join approval_record t2 on t1.id = t2.DocCode and t1.`Level` = t2.`Level` +1 " +
        "inner join approval_process t3 on t1.id = t3.DocCode and t1.level = t3.`Level` " +
        "inner join users t4 on t3.approverId = t4.userId " +
        "where t1.status = '审批中' and t2.DocumentTableName = 'yl_reimburse' and t3.DocumentTableName = 'yl_reimburse' " +
        "and timestampdiff(day, t2.time, now()) > 6 and date_format( t1.lmt, '%Y%m' ) = date_format( curdate( ) , '%Y%m' ) order by time;");

            DataTable dt = SqlHelper.Find(sql).Tables[0];

            string wechatUserIds = "";

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    wechatUserIds += dr[0].ToString() + "|";
                }

                wechatUserIds.Substring(0, wechatUserIds.Length - 1);

                JObject jObject = new JObject();
                jObject.Add("touser", wechatUserIds);
                jObject.Add("msgtype", "textcard");
                jObject.Add("agentid", "1000006");
                JObject innerJObject = new JObject();
                innerJObject.Add("title", "审批通知");
                innerJObject.Add("description", "您有单据超过7天未审批，请及时审批!");
                innerJObject.Add("url", "http://yelioa.top//mMobileReimbursement.aspx");
                jObject.Add("textcard", innerJObject);

                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", "wx9f620907462561ca", "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk");
                string res = HttpHelper.Get(url);
                Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(res);
                string token = dict["access_token"].ToString();

                url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + token);
                HttpHelper.Post(url,jObject.ToString());
            }
        }
    }

    void Application_End(object sender, EventArgs e)
    {
        //  在应用程序关闭时运行的代码

    }

    void Application_Error(object sender, EventArgs e)
    {
        // 在出现未处理的错误时运行的代码

    }

    void Session_Start(object sender, EventArgs e)
    {
        // 在新会话启动时运行的代码

    }

    void Session_End(object sender, EventArgs e)
    {
        // 在会话结束时运行的代码。 
        // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
        // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer
        // 或 SQLServer，则不引发该事件。

    }

</script>
