using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// MobileReimburseManage 的摘要说明
/// </summary>
public class MobileReimburseManage
{
    public MobileReimburseManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string confirmApprovalProcess(ref JArray jobjectList, int fee_department_id, string fee_detail, double fee_amount, UserInfo userInfo, int self_departmentId)
    {
        // 插入提交者数据提交人表中
        //MobileReimburseSrv.insertApprovalProcess(0, userInfo.userId.ToString());
        JObject resJObject = new JObject();
        resJObject.Add("name", userInfo.userName);
//        resJObject.Add("index", 0);
        resJObject.Add("userId", userInfo.userId.ToString());
        jobjectList.Add(resJObject);

        WxUserInfo wxUserInfo = new WxUserInfo();
        string leaders = "";

        int index = 1;

        DataSet ds = MobileReimburseSrv.findParentIdById(fee_department_id);

        if (ds == null || ds.Tables[0].Rows.Count == 0)
            return null;

        int fee_department_parentId = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
        
        while (fee_department_parentId != 1 && fee_department_parentId != 285
            && fee_department_parentId != 282 && fee_department_parentId != 284)
        {
            ds = MobileReimburseSrv.findParentIdById(fee_department_parentId);

            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;

            fee_department_parentId = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
        }

        // 找到自己的顶级部门
        if (self_departmentId == 0)
        {
            string self_wechatUserId = userInfo.wechatUserId;

            ds = MobileReimburseSrv.findDepartmentByWechatUserId(self_wechatUserId);

            if (ds == null)
            {
                return null;
            }

            self_departmentId = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
        }

        ds = MobileReimburseSrv.findParentIdById(self_departmentId);

        if (ds == null || ds.Tables[0].Rows.Count == 0)
            return null;

        int self_department_parentId = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());

        while (self_department_parentId != 1 && self_department_parentId != 285
            && self_department_parentId != 282 && self_department_parentId != 284)
        {
            ds = MobileReimburseSrv.findParentIdById(self_department_parentId);

            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;

            self_department_parentId = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
        }

        //leaders = getDepartmentApprover(ref jobjectList, ref fee_department_id, userInfo, fee_amount, ref index, fee_department_parentId, fee_detail);

        if (fee_detail.Contains("工资社保金额"))
        {
            if (self_department_parentId != 285)
            {
                if (!userInfo.userName.Equals("田红琴"))
                {
                    resJObject = new JObject();
                    resJObject.Add("name", "田红琴");
                    //                resJObject.Add("index", index);
                    resJObject.Add("userId", "100000170");
                    jobjectList.Add(resJObject);
                    leaders += "田红琴,";
                    index++;
                }
                else
                {
                    resJObject = new JObject();
                    resJObject.Add("name", "程丹凤");
                    //                resJObject.Add("index", index);
                    resJObject.Add("userId", "100000225");
                    jobjectList.Add(resJObject);
                    leaders += "程丹凤,";
                }
            }
            else
            {
                if (!"陈永洪".Equals(userInfo.userName) && !"侯莉".Equals(userInfo.userName) && !"彭春燕".Equals(userInfo.userName))
                {
                    while (!leaders.Contains("陈永洪") && !leaders.Contains("侯莉") && !leaders.Contains("彭春燕"))
                    {
                        leaders = getApprover(ref jobjectList, self_departmentId, userInfo, leaders) + ",";
                        self_departmentId = Int32.Parse(MobileReimburseSrv.findParentIdById(self_departmentId).Tables[0].Rows[0][0].ToString());
                    }
                }

                resJObject = new JObject();
                resJObject.Add("name", "田红琴");
                //                resJObject.Add("index", index);
                resJObject.Add("userId", "100000170");
                jobjectList.Add(resJObject);
                leaders += "田红琴,";
                index++;
            }
            
            return leaders;
        }

        if (fee_department_parentId == 284)
        {
            leaders = getDepartmentApprover(ref jobjectList, self_departmentId, userInfo, fee_amount, ref index,
                self_department_parentId, fee_detail);

            //leaders += getDepartmentApprover(ref jobjectList, ref fee_department_id, userInfo, fee_amount, ref index,
            //    fee_department_parentId, fee_detail);
            if (self_departmentId != fee_department_id)
                leaders += getMultiDepartmentApprover(ref jobjectList, fee_department_id, userInfo, self_departmentId, ref index, fee_amount);

            if ("".Equals(leaders) || "".Equals(leaders.Replace(",", "")))
            {
                leaders += "吕正和,";
                resJObject = new JObject();
                resJObject.Add("name", "吕正和");
                //                    resJObject.Add("index", index);
                resJObject.Add("userId", "100000142");
                jobjectList.Add(resJObject);
            }

            // 销售折让 最后加上运营部经理和营销副总
            if (fee_detail.Contains("销售折让"))
            {
                leaders += "洪秀秀,";
                //MobileReimburseSrv.insertApprovalProcess(index, "100000225");
                resJObject = new JObject();
                resJObject.Add("name", "洪秀秀");
                //            resJObject.Add("index", index);
                resJObject.Add("userId", "100000398");
                jobjectList.Add(resJObject);
            }

            if (fee_detail.Contains("市场调节基金"))
            {
                leaders += "洪秀秀,";
                //MobileReimburseSrv.insertApprovalProcess(index, "100000225");
                resJObject = new JObject();
                resJObject.Add("name", "洪秀秀");
                //            resJObject.Add("index", index);
                resJObject.Add("userId", "100000398");
                jobjectList.Add(resJObject);
            }

            if (fee_detail.Contains("推广活动开发费") || fee_detail.Contains("推广活动市场费"))
            {
                resJObject = new JObject
                {
                    { "name", "熊玲玲" },
                    { "userId", "100000406" }
                };
                jobjectList.Add(resJObject);
            }

            if (fee_detail.Contains("推广活动渠道费"))
            {
                resJObject = new JObject
                {
                    { "name", "刘雅玲" },
                    { "userId", "100000534" }
                };
                jobjectList.Add(resJObject);
            }

            if (fee_detail.Contains("培训费") || fee_detail.Contains("会议费"))
            {
                resJObject = new JObject
                {
                    { "name", "张露" },
                    { "userId", "100000538" }
                };
                jobjectList.Add(resJObject);
            }
        }
        else
        {
            Boolean isSameDep = false;

            // 判断是否是跨部门
            if (fee_department_id == self_departmentId)
            {
                isSameDep = !isSameDep;
            }

            // 如果是跨部门审批
            if (!isSameDep)
            {
                leaders = getDepartmentApprover(ref jobjectList, self_departmentId, userInfo, fee_amount, ref index,
                self_department_parentId, fee_detail);
                leaders += getMultiDepartmentApprover(ref jobjectList, fee_department_id, userInfo, self_departmentId, ref index, fee_amount);

                //if (fee_detail.Contains("市场调节基金"))
                //{
                //    leaders += "洪秀秀,";
                //    //MobileReimburseSrv.insertApprovalProcess(index, "100000225");
                //    resJObject = new JObject();
                //    resJObject.Add("name", "洪秀秀");
                //    //            resJObject.Add("index", index);
                //    resJObject.Add("userId", "100000398");
                //    jobjectList.Add(resJObject);

                //    leaders += "龚云云,";
                //    //MobileReimburseSrv.insertApprovalProcess(index, "100000225");
                //    resJObject = new JObject();
                //    resJObject.Add("name", "龚云云");
                //    //            resJObject.Add("index", index);
                //    resJObject.Add("userId", "100000154");
                //    jobjectList.Add(resJObject);
                //}
                //            while (self_department_parentId != 1 && self_department_parentId != 285
                //            && self_department_parentId != 282 && self_department_parentId != 284)
                //            {
                //                ds = MobileReimburseSrv.findParentIdById(self_department_parentId);
                //
                //                if (ds == null || ds.Tables[0].Rows.Count == 0)
                //                    return null;
                //
                //                self_department_parentId = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
                //            }
                //
                //            if (self_department_parentId != 285 || self_department_parentId != fee_department_parentId)
                //            {
                //                leaders += getDepartmentApprover(ref jobjectList, self_departmentId, userInfo, fee_amount, ref index, self_department_parentId, fee_detail);
                //            }
            }
            // 如果不是跨部门审批
            else
            {
                leaders = getDepartmentApprover(ref jobjectList, self_departmentId, userInfo, fee_amount, ref index,
                self_department_parentId, fee_detail);
            }
        }

        if (fee_department_parentId == 1)
        {
            if (fee_amount > 10000)
            {
                leaders += "吕正和,";
                //MobileReimburseSrv.insertApprovalProcess(index++, "100000142");
                resJObject = new JObject();
                resJObject.Add("name", "吕正和");
//                resJObject.Add("index", index);
                resJObject.Add("userId", "100000142");
                jobjectList.Add(resJObject);
            }
        }
        else if (fee_department_parentId == 285)// 如果是制造事业部
        {
            if (fee_amount >= 10000)
            {
                leaders += "吕正和,";
                resJObject = new JObject();
                resJObject.Add("name", "吕正和");
                resJObject.Add("userId", "100000142");
                jobjectList.Add(resJObject);
            }
        }
        // 如果是营销部
        else if (fee_department_parentId == 284)
        {
            if (fee_amount >= 10000)
            {
                leaders += "吕正和,";
                resJObject = new JObject();
                resJObject.Add("name", "吕正和");
                resJObject.Add("userId", "100000142");
                jobjectList.Add(resJObject);
            }
        }

        // 最后去重 留后不留前
        JArray newList = new JArray();
        for (int i = jobjectList.Count-1; i >=0 ; i--)
        {
            Boolean flag = true;
            for (int j = 0; j < newList.Count; j++)
            {
                if (jobjectList[i]["userId"].ToString().Equals(newList[j]["userId"].ToString()))
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                newList.AddFirst(jobjectList[i]);
            }
        }

        jobjectList = newList;

        return leaders;
    }

    private static string getApprover(ref JArray list, int fee_department_parentId, UserInfo userInfo, string leaders)
    {
        string sql = string.Format("SELECT t1.*,t2.userName,t2.userId userId1, t3.`name` FROM `user_department_post` t1 left join users t2 on " +
        "t1.wechatUserId = t2.wechatUserId left join department t3 on t1.departmentId = t3.Id where departmentId = '{0}' and isHead = 1", fee_department_parentId);

        if (SqlHelper.Find(sql).Tables[0].Rows.Count == 0)
        {
            return leaders;
        }

        string queryLeader = SqlHelper.Find(sql).Tables[0].Rows[0]["userName"].ToString();
        string queryLeaderUserId = SqlHelper.Find(sql).Tables[0].Rows[0]["userId1"].ToString();

        if (queryLeaderUserId.Equals(userInfo.userId.ToString()))
        {
            //string parentId = SqlHelper.Find("select parentId from department where id = '" + fee_department_parentId + "'").Tables[0].Rows[0][0].ToString();
            //sql = string.Format("SELECT t1.*,t2.userName,t2.userId userId1, t3.`name` FROM `user_department_post` t1 left join users t2 on " +
            //                    "t1.wechatUserId = t2.wechatUserId left join department t3 on t1.departmentId = t3.Id where departmentId = '{0}' and isHead = 1", parentId);
            //queryLeader = SqlHelper.Find(sql).Tables[0].Rows[0]["userName"].ToString();
            //queryLeaderUserId = SqlHelper.Find(sql).Tables[0].Rows[0]["userId1"].ToString();
            return leaders;
        }

        //if ("吕正和".Equals(queryLeader) && fee_department_parentId != 292)
        //{
        //    return leaders;
        //}

        // 判断找出的审批人是不是提交人的下级 如果是 则跳过
        WxUserInfo wxUserInfo = new WxUserInfo();

        if (wxUserInfo.isSubmitterLeader(userInfo.userId.ToString(), queryLeaderUserId))
        {
            return leaders;
        }

        JObject resJObject = new JObject
        {
            { "name", queryLeader },
            { "userId", queryLeaderUserId }
        };

        list.Add(resJObject);

        if ("".Equals(queryLeader))
            return leaders;

        leaders += queryLeader + ",";
        
        return leaders;
    }

    private static string getMultiDepartmentApprover(ref JArray list, int fee_department_id, UserInfo userInfo, int self_departmentId, ref int index, double fee_amount)
    {
        //string leaders = getApprover(ref list, fee_department_id, userInfo, "") + ",";

        //if ("".Equals(leaders) || ",".Equals(leaders))
        //{
        //    string parentId = SqlHelper.Find("select parentId from department where id = '" + self_departmentId + "'").Tables[0].Rows[0][0].ToString();
        //    leaders = getApprover(ref list, Int32.Parse(parentId), userInfo, "") + ",";

        //    if ("".Equals(leaders) || ",".Equals(leaders))
        //    {
        //        string secondParentId = SqlHelper.Find("select parentId from department where id = '" + parentId + "'").Tables[0].Rows[0][0].ToString();
        //        leaders = getApprover(ref list, Int32.Parse(secondParentId), userInfo, "") + ",";
        //    }
        //}
        string leaders = "";
        if (userInfo.userName.Equals("黄文俊") || userInfo.userName.Equals("付玉林") || userInfo.userName.Equals("刘新文"))
        {
            leaders += "吕正和,";
            JObject resJObject = new JObject();
            resJObject.Add("name", "吕正和");
            //                    resJObject.Add("index", index);
            resJObject.Add("userId", "100000142");
            list.Add(resJObject);
        }

        if (fee_department_id != 472 && fee_department_id != 473 && fee_department_id != 388 && fee_department_id != 474 && fee_department_id != 519)
        {
            for (int i = 0; i < 2; i++)
            {
                string queryLeaders = getApprover(ref list, fee_department_id, userInfo, "");

                if (fee_department_id == 289)
                {
                    JObject resJObject = new JObject();
                    resJObject.Add("name", "田红琴");
                    //                resJObject.Add("index", index);
                    resJObject.Add("userId", "100000170");
                    list.Add(resJObject);

                    resJObject = new JObject();
                    resJObject.Add("name", "程丹凤");
                    //                resJObject.Add("index", index);
                    resJObject.Add("userId", "100000225");
                    list.Add(resJObject);
                }
                else if (fee_department_id == 286)
                {
                    JObject resJObject = new JObject();
                    resJObject.Add("name", "李茂龙");
                    //                resJObject.Add("index", index);
                    resJObject.Add("userId", "100000372");
                    list.Add(resJObject);

                    resJObject = new JObject();
                    resJObject.Add("name", "程丹凤");
                    //                resJObject.Add("index", index);
                    resJObject.Add("userId", "100000225");
                    list.Add(resJObject);
                }

                if ("".Equals(queryLeaders) || ",".Equals(queryLeaders))
                {
                    i--;
                }
                if (!leaders.Contains(queryLeaders))
                {
                    leaders += queryLeaders + ",";
                }

                try
                {
                    fee_department_id = Int32.Parse(MobileReimburseSrv.findParentIdById(fee_department_id).Tables[0].Rows[0][0].ToString());
                }
                catch (Exception e)
                {
                    JObject resJObject = new JObject();
                    resJObject.Add("name", "吕正和");
                    //                resJObject.Add("index", index);
                    resJObject.Add("userId", "100000142");
                    list.Add(resJObject);
                    leaders += "吕正和,";
                    index++;

                    break;
                }
            }
        }
        else
        {
            string queryLeaders = getApprover(ref list, fee_department_id, userInfo, "");
        }

        if (fee_amount >= 10000)
        {
            JObject resJObject = new JObject();
            resJObject.Add("name", "吕正和");
            resJObject.Add("userId", "100000142");
            list.Add(resJObject);
            leaders += "吕正和,";
        }

        return leaders;
    }

    private static string getDepartmentApprover(ref JArray list, int self_department_id, UserInfo userInfo, double fee_amount, 
        ref int index, int self_department_parentId, string fee_detail)
    {
        WxUserInfo wxUserInfo = new WxUserInfo();

        string leaders = "";

        int loopCount = 2;

        JObject resJObject = new JObject();
        // 如果是集团财务中心
        if (self_department_parentId == 1)
        {
            // 向上审批直到审批人中包含 吕正和
            if (userInfo.userName.Equals("张代俊"))
            {
                resJObject = new JObject();
                resJObject.Add("name", "吕正和");
                //                resJObject.Add("index", index);
                resJObject.Add("userId", "100000142");
                list.Add(resJObject);
                leaders += "吕正和,";
                index++;
            }
            else
            {
                while (!leaders.Contains("张代俊"))
                {
                    leaders = getApprover(ref list, self_department_id, userInfo, leaders) + ",";
                    self_department_id = Int32.Parse(MobileReimburseSrv.findParentIdById(self_department_id).Tables[0].Rows[0][0].ToString());
                }
            }
        }
        else if (self_department_parentId == 285)// 如果是制造事业部
        {
            if (userInfo.userName.Equals("陈永洪"))
            {
                resJObject = new JObject();
                resJObject.Add("name", "吕正和");
                //                resJObject.Add("index", index);
                resJObject.Add("userId", "100000142");
                list.Add(resJObject);
                leaders += "吕正和,";
                index++;
            }
            else if (fee_amount <= 3000 && !userInfo.userName.Equals("陈永洪") && !userInfo.userName.Equals("侯莉") && !userInfo.userName.Equals("彭春燕"))
            {
                // 向上审批直到审批人中包含 陈于龙
                while (!leaders.Contains("陈永洪") && !leaders.Contains("侯莉") && !leaders.Contains("彭春燕"))
                {
                    leaders = getApprover(ref list, self_department_id, userInfo, leaders) + ",";
                    self_department_id = Int32.Parse(MobileReimburseSrv.findParentIdById(self_department_id).Tables[0].Rows[0][0].ToString());
                }
            }
            else
            {
                // 向上审批直到审批人中包含 陈永洪
                while (!leaders.Contains("陈永洪"))
                {
                    leaders = getApprover(ref list, self_department_id, userInfo, leaders) + ",";
                    self_department_id = Int32.Parse(MobileReimburseSrv.findParentIdById(self_department_id).Tables[0].Rows[0][0].ToString());
                }
            }
        }
        else if (self_department_parentId == 282)
        {
            if (self_department_id == 289 && !userInfo.userName.Equals("田红琴"))   //人事部
            {
                resJObject = new JObject();
                resJObject.Add("name", "田红琴");
                //                resJObject.Add("index", index);
                resJObject.Add("userId", "100000170");
                list.Add(resJObject);
                leaders += "田红琴,";
                index++;
            }
            else if (self_department_id == 286 && !userInfo.userName.Equals("李茂龙"))  // 行政部或集团办公室
            {
                resJObject = new JObject();
                resJObject.Add("name", "李茂龙");
                //                resJObject.Add("index", index);
                resJObject.Add("userId", "100000372");
                list.Add(resJObject);
                leaders += "李茂龙,";
                index++;
            }

            // 向上审批直到审批人中包含 程丹凤
            while (!leaders.Contains("程丹凤"))
            {
                leaders = getApprover(ref list, self_department_id, userInfo, leaders) + ",";
                self_department_id = Int32.Parse(MobileReimburseSrv.findParentIdById(self_department_id).Tables[0].Rows[0][0].ToString());
            }

            resJObject = new JObject();
            resJObject.Add("name", "王运生");
            //                resJObject.Add("index", index);
            resJObject.Add("userId", "100000371");
            list.Add(resJObject);

            if (fee_amount >= 10000)
            {
                resJObject = new JObject();
                resJObject.Add("name", "吕正和");
                resJObject.Add("userId", "100000142");
                list.Add(resJObject);
                leaders += "吕正和,";
            }
        }
        else if (self_department_parentId == 284)
        {
            //var isMultiDepartment = false;

            //if (fee_department_id != self_department_id)
            //{
            //    isMultiDepartment = true;
            //}

            if (fee_detail.Contains("差旅费") || fee_detail.Contains("区域日常费用"))
            {
                for (int i = 0; i < loopCount; i++)
                {
                    string gmLeader = getApprover(ref list, self_department_id, userInfo, leaders);

                    if ("".Equals(gmLeader))
                    {
                        i--;
                    }

                    self_department_id = Int32.Parse(MobileReimburseSrv.findParentIdById(self_department_id).Tables[0].Rows[0][0].ToString());

                    if (self_department_id == 284 || self_department_id == 291 || self_department_id == 482)
                        break;

                    leaders += gmLeader + ",";
                }
            }
            else
            {
                loopCount = 0;  // 此处loopCount用来记录往上审批了几级 若小于一级 则需要加上吕正和
                while (self_department_id != 284 && self_department_id != 291 && self_department_id != 482)
                {
                    leaders = getApprover(ref list, self_department_id, userInfo, leaders) + ",";
                    self_department_id = Int32.Parse(MobileReimburseSrv.findParentIdById(self_department_id).Tables[0].Rows[0][0].ToString());
                    if (leaders.Contains("吕正和"))
                    {
                        break;
                    }
                    loopCount++;
                }
                if (loopCount <= 1 && !leaders.Contains("吕正和"))
                {
                    resJObject = new JObject
                    {
                        { "name", "吕正和" },
                        { "userId", "100000142" }
                    };
                    list.Add(resJObject);
                }
            }
        }

        return leaders;
    }

    public static DataTable FindProduct(int userId,string name)
    {
        DataTable dt = MobileReimburseSrv.FindProduct(userId);
        if (dt == null)
            return dt;

        DataTable dtRes = new DataTable();
        dtRes.Columns.Add("value", Type.GetType("System.String"));
        dtRes.Columns.Add("target", Type.GetType("System.String"));

        foreach (DataRow row in dt.Rows)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (PinYinHelper.ContainsFirstLetter(row[0].ToString(), name) || row[0].ToString().Contains(name))
                {
                    DataRow dr = dtRes.NewRow();

                    dr["value"] = row[0];
                    dr["target"] = row[0];

                    dtRes.Rows.Add(dr);
                }
            }
            else
            {
                DataRow dr = dtRes.NewRow();

                dr["value"] = row[0];
                dr["target"] = row[0];

                dtRes.Rows.Add(dr);
            }
        }
        return dtRes;
    }

    public static DataTable findProduct(string name, List<DepartmentPost> departmentList, string departmentName)
    {
        // 当销售部非代表填写预算时，网点和产品必须填综合
        DataSet ds = MobileReimburseSrv.findProductName(name, departmentList, departmentName);

        if (ds == null)
            return null;

        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));

        int index = 0;

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DataRow dr = dt.NewRow();

                dr["value"] = row["name"];
                dr["target"] = row["name"];

                dt.Rows.Add(dr);
            }
        }
        else
        {
            ds = MobileReimburseSrv.findProductName();

            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //if (index >= 5)
                //{
                //    break;
                //}

                if (PinYinHelper.ContainsFirstLetter(row["name"].ToString(), name) )
                {
                    DataRow dr = dt.NewRow();

                    dr["value"] = row["name"];
                    dr["target"] = row["name"];

                    dt.Rows.Add(dr);

                    index++;
                }
            }
        }

        return dt;
    }

    public static DataTable FindClient(uint userId,string name)
    {
        DataTable dt = MobileReimburseSrv.FindClient(userId);
        if (dt == null)
            return dt;

        DataTable dtRes = new DataTable();
        dtRes.Columns.Add("value", Type.GetType("System.String"));
        dtRes.Columns.Add("target", Type.GetType("System.String"));

        foreach (DataRow row in dt.Rows)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (PinYinHelper.ContainsFirstLetter(row[0].ToString(), name) || row[0].ToString().Contains(name))
                {
                    DataRow dr = dtRes.NewRow();

                    dr["value"] = row[0];
                    dr["target"] = row[0];

                    dtRes.Rows.Add(dr);
                }
            }
            else
            {
                DataRow dr = dtRes.NewRow();

                dr["value"] = row[0];
                dr["target"] = row[0];

                dtRes.Rows.Add(dr);
            }
        }
        return dtRes;
    }

    public static DataTable findBranch(string name, List<DepartmentPost> departmentList, string departmentName)
    {
        DataSet ds = MobileReimburseSrv.findBranch(name, departmentList, departmentName);

        if (ds == null)
            return null;

        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));

        int index = 0;

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DataRow dr = dt.NewRow();

                dr["value"] = row["name"];
                dr["target"] = row["name"];

                dt.Rows.Add(dr);
            }
        }
        else
        {
            ds = MobileReimburseSrv.findBranch();

            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //if (index >= 5)
                //{
                //    break;
                //}

                if (PinYinHelper.ContainsFirstLetter(row["name"].ToString(), name))
                {
                    DataRow dr = dt.NewRow();

                    dr["value"] = row["name"];
                    dr["target"] = row["name"];

                    dt.Rows.Add(dr);

                    index++;
                }
            }
        }

        return dt;
    }

    public static DataTable findInformer(string name)
    {
        DataSet ds = MobileReimburseSrv.findInformer(name);

        if (ds == null)
            return null;

        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));

        //int index = 0;

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DataRow dr = dt.NewRow();

                dr["value"] = row["userId"];
                dr["target"] = row["userName"];

                dt.Rows.Add(dr);
            }
        }
        else
        {
            ds = MobileReimburseSrv.findInformer();

            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //if (index >= 5)
                //{
                //    break;
                //}

                if (PinYinHelper.ContainsFirstLetter(row["userName"].ToString(), name))
                {
                    DataRow dr = dt.NewRow();

                    dr["value"] = row["userId"];
                    dr["target"] = row["userName"];

                    dt.Rows.Add(dr);

                    //index++;
                }
            }
        }

        return dt;
    }

    public static DataTable findParentFeeDetail(string department,string product)
    {
        DataSet ds = MobileReimburseSrv.findParentFeeDetail(department);

        if (ds == null)
            return null;

        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (product == "非销售" && ("销售折让、市场调节基金、VIP维护费、推广活动市场费、推广活动开发费、推广活动渠道费").Contains(row["FeeDetail"].ToString()))
                {
                    continue;
                }
                DataRow dr = dt.NewRow();

                dr["value"] = row["FeeDetail"];
                dr["target"] = row["FeeDetail"];

                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    public static DataTable findChildrenFeeDetail(string name,string feeDepartment)
    {
        DataSet ds = MobileReimburseSrv.findChildrenFeeDetail(name,feeDepartment);

        if (ds == null)
            return null;

        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));
        dt.Columns.Add("id", Type.GetType("System.String"));

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DataRow dr = dt.NewRow();

                dr["value"] = row["FeeDetail"];
                dr["target"] = row["FeeDetail"];
                dr["id"] = row["Id"];

                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    public static DataTable findFeeDepartment(string name)
    {
        DataSet ds = MobileReimburseSrv.findFeeDepartment(name);

        if (ds == null)
            return null;

        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));

        int index = 0;

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DataRow dr = dt.NewRow();

                dr["value"] = row["name"];
                dr["target"] = row["name"];

                dt.Rows.Add(dr);
            }
        }
        else
        {
            ds = MobileReimburseSrv.findFeeDepartment();

            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //if (index >= 5)
                //{
                //    break;
                //}

                if (PinYinHelper.ContainsFirstLetter(row["name"].ToString(), name))
                {
                    DataRow dr = dt.NewRow();

                    dr["value"] = row["name"];
                    dr["target"] = row["name"];

                    dt.Rows.Add(dr);

                    index++;
                }
            }
        }

        return dt;
    }



    /// <summary>
    /// 是否超预算
    /// </summary>
    /// <param name="fee_amount">报销金额</param>
    /// <param name="fee_detail">费用明细（名称）</param>
    /// <param name="userId">人员ID</param>
    /// <param name="fee_department">费用归属部门（部门名称）</param>
    /// <returns></returns>
    public static string IsOverBudget(string fee_amount, string fee_detail, string userId,string fee_department,string product,string fee_branch,string project)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(fee_amount) || string.IsNullOrEmpty(fee_detail)  || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fee_department))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            List<string> sqlList = new List<string>();
            fee_detail = fee_detail.Split('-')[0];
            if(fee_detail == "工资社保金额" || fee_detail =="制造费用" || fee_detail == "总部管理费用" || fee_detail == "研发费用金额")
            {
                sqlList.Add(MobileReimburseSrv.AccessToTheBudgetOfTheFunctionLine(fee_detail,project));//预算
                sqlList.Add(MobileReimburseSrv.AccessToTheExtraBudgetaryCostOfTheFunctionalLine(fee_detail,project));//预算外
                sqlList.Add(MobileReimburseSrv.GetCostBalanceOfLastMonth(fee_detail, "", "", ""));//上月结余
                sqlList.Add(MobileReimburseSrv.AccessToTheCostOfMobileReimbursement(fee_detail, "", "", "",project));//移动报销
            }

            else if(fee_detail == "市场学术费"||
                fee_detail == "商务费用金额"|| fee_detail == "工资社保金额" || fee_detail == "招商费用")
            {
                sqlList.Add(MobileReimburseSrv.GetTheSalesLineBudget("","",""));//预算
                sqlList.Add(MobileReimburseSrv.AccessToTheExtraBudgetaryCostOfTheFunctionalLine(fee_detail,""));//预算外,由于费用归属问题，这些费用预算外走职能部门的
                sqlList.Add(MobileReimburseSrv.GetCostBalanceOfLastMonth(fee_detail, "", "", ""));//上月结余
                sqlList.Add(MobileReimburseSrv.AccessToTheCostOfMobileReimbursement(fee_detail, "", "", "",""));//移动报销
            }
            else if(fee_detail=="销售折让"||fee_detail=="市场调节基金")
            {
                sqlList.Add(MobileReimburseSrv.GetTheSalesLineBudget("", fee_branch, product));//预算
                sqlList.Add(MobileReimburseSrv.ObtainTheAmountOfExtraBudgetaryExpensesOfTheSalesLine(fee_detail, "", fee_branch, product));//预算外
                sqlList.Add(MobileReimburseSrv.GetCostBalanceOfLastMonth(fee_detail, "", fee_branch, product));//上月结余
                sqlList.Add(MobileReimburseSrv.AccessToTheCostOfMobileReimbursement(fee_detail, "", fee_branch, product,""));//移动报销
            }
            else
            {
                sqlList.Add(MobileReimburseSrv.GetTheSalesLineBudget(fee_department, "", ""));//预算
                sqlList.Add(MobileReimburseSrv.ObtainTheAmountOfExtraBudgetaryExpensesOfTheSalesLine(fee_detail, fee_department, "", ""));//预算外
                sqlList.Add(MobileReimburseSrv.GetCostBalanceOfLastMonth(fee_detail, fee_department, "", ""));//上月结余
                sqlList.Add(MobileReimburseSrv.AccessToTheCostOfMobileReimbursement(fee_detail, fee_department, "", "",""));//移动报销
            }
            for(int i=0;i<sqlList.Count;i++)
            {
                sqlList[i] += ";";
            }
            string msg = "";
            DataSet ds = SqlHelper.Find(sqlList.ToArray(), ref msg);
            if(ds==null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else
            {
                
                double sum = Convert.ToDouble(ds.Tables[2].Rows[0][0]) - Convert.ToDouble(ds.Tables[3].Rows[0][0]); //上月结余-移动报销
                if(fee_detail=="制造费用")
                {
                    sum += Convert.ToDouble(ds.Tables[0].Rows[0][0]);//预算                    
                }
                else
                {
                    foreach (DataRow row in ds.Tables[0].Rows)//预算
                    {
                        JObject data = JObject.Parse(row["DataJson"].ToString());

                        if (fee_detail != "销售折让" && fee_detail != "市场调节基金"&& fee_detail != "招商费用")
                        {
                            
                            sum += Convert.ToDouble(row["预计流向数"].ToString()) * Convert.ToDouble(data[fee_detail]);
                        }
                        else if(fee_detail == "招商费用")
                        {
                            sum += Convert.ToDouble(row["预计流向数"].ToString()) * (Convert.ToDouble(data["销售总监费用"]) + Convert.ToDouble(data["大区经理费用"]) +
                                Convert.ToDouble(data["开发费用金额"]) + Convert.ToDouble(data["产品发展基金"]) + Convert.ToDouble(data["实验费(TF)金额"])
                                + Convert.ToDouble(data["区域中心费用VIP维护"]) + Convert.ToDouble(data["区域中心费用"]) + Convert.ToDouble(data["销售总监费用"])
                                + Convert.ToDouble(data["销售总监费用"]));
                        }

                    }
                }
                sum += Convert.ToDouble(ds.Tables[1].Rows[0][0]);//预算外

                if (sum - Convert.ToDouble(fee_amount) >= 0)
                {
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "在预算内");
                }
                else
                {
                    res.Add("ErrCode", 3);
                    res.Add("ErrMsg", "超出预算,现有预算" + sum.ToString());
                }
            }
        }
        return res.ToString();
    }

    public static string insertMobileReimburse(string apply_time, string product, string branch, string fee_department, string fee_detail, string fee_amount,
        string file, string remark, UserInfo userInfo, string approver, string departmentName, List<string> informerList, List<JObject> approverDataList, 
        List<string> uploadFileUrlsList, string docCode, string project,string isOverBudget, string isPrepaid, string isHasReceipt, List<JObject> reimburseDetailList, string fee_company)
    {
        string code = GenerateDocCode.getReimburseCode();
        if (departmentName == null || "".Equals(departmentName))
        {
            departmentName = MobileReimburseSrv.findDepartmentNameByWechatUserId(userInfo.wechatUserId).Tables[0].Rows[0][0].ToString();
        }
        string reportDepartmentName = SqlHelper.Find(string.Format("select ifnull(reportName, '') from department where name = '{0}'", fee_department)).Tables[0].Rows[0][0].ToString();

        if (reportDepartmentName == "集团行政部" && (fee_detail.Contains("招待费") || fee_detail.Contains("差旅费")) && (userInfo.userName == "黄文俊" || userInfo.userName == "付玉林"))
        {
            reportDepartmentName = "集团办公室";
        }
        else if (reportDepartmentName == "南昌中申医疗" && (fee_detail.Contains("LK") || fee_detail.Contains("中申")))
        {
            reportDepartmentName = "南昌老康科技";
        }

        string jObjectStr = "";
        JObject jObject = new JObject();
        if (string.IsNullOrEmpty(docCode))
        {
            jObjectStr = MobileReimburseSrv.insertMobileReimburse(code, apply_time, product, branch, fee_department, fee_detail, fee_amount,
            file, remark, userInfo, approver, departmentName, project, isOverBudget, isPrepaid, isHasReceipt, fee_company, reportDepartmentName);

            jObject = JObject.Parse(jObjectStr);
        }
        else
        {
            jObjectStr = MobileReimburseSrv.updateMobileReimburse(docCode, apply_time, product, branch, fee_department, fee_detail, fee_amount,
            file, remark, userInfo, approver, departmentName, project, isOverBudget, isPrepaid, isHasReceipt, fee_company, reportDepartmentName);

            code = docCode;
        }
        
        string id = "";
        if (jObjectStr.Contains("操作成功"))
        {
            id = SqlHelper.Find("select id from yl_reimburse where code = '" + code + "'").Tables[0].Rows[0][0].ToString();
        }
        else
        {
            while (!"1".Equals(jObject["Success"].ToString()))
            {
                code = GenerateDocCode.getReimburseCode();
                jObjectStr = MobileReimburseSrv.insertMobileReimburse(code, apply_time, product, branch, fee_department, fee_detail, fee_amount,
                file, remark, userInfo, approver, departmentName, project, isOverBudget, isPrepaid, isHasReceipt, fee_company, reportDepartmentName);
                jObject = JObject.Parse(jObjectStr);
            }

            id = jObject["Id"].ToString();
        }

        // 插入到approval_process表中
        string msg = MobileReimburseSrv.insertApprovalProcess(id, approverDataList, "yl_reimburse");

        if (!msg.Contains("操作成功"))
        {
            return msg;
        }

        // 发审批消息
        msg = ApprovalFlowManage.SubmitDocument("yl_reimburse", id, userInfo,
            "http://yelioa.top//mMySubmittedReimburse.aspx?docCode=" + code, "http://yelioa.top//mMobileReimbursement.aspx?docCode=" + code,
            "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk", "yl_reimburse", "1000006");

        if (!msg.Contains("操作成功"))
        {
            return msg;
        }
        // 新增知悉人到表
        if (informerList != null && informerList.Count > 0)
            msg = MobileReimburseSrv.insertInformer(id, informerList);

        if (!msg.Contains("操作成功"))
        {
            return "新增知悉人失败，请重试";
        }

        // 新增附件表
        if (uploadFileUrlsList != null && uploadFileUrlsList.Count > 0)
            msg = MobileReimburseSrv.insertAttachement(id, uploadFileUrlsList);

        if (!msg.Contains("操作成功"))
        {
            return "新增图片失败，请重试";
        }

        // 新增报销明细表
        if (reimburseDetailList.Count > 0) {
            DataTable dt = new DataTable();
            foreach (JObject tempJObject in reimburseDetailList)
            {
                if (tempJObject["ReceiptPerson"].ToString() != "" && tempJObject["ReceiptPerson"].ToString() != "无")
                {
                    dt = SqlHelper.Find(string.Format("select 1 from users where userName = '{0}'", tempJObject["ReceiptPerson"])).Tables[0];
                }

                if (dt.Rows.Count > 0)
                {
                    if (tempJObject["ReceiptType"].Contains("火车票") && tempJObject["ReceiptPerson"].ToString() != "无")
                    {
                        tempJObject["ReceiptTax"] = Math.Round(double.Parse(tempJObject["ReceiptAmount"].ToString()) * 0.09, 3);
                    }
                    else if (tempJObject["ReceiptType"].Contains("汽车票") && tempJObject["ReceiptPerson"].ToString() != "无")
                    {
                        tempJObject["ReceiptTax"] = Math.Round(double.Parse(tempJObject["ReceiptAmount"].ToString()) * 0.03, 3);
                    }
                    else if (tempJObject["ReceiptType"].Contains("机票") && tempJObject["ReceiptPerson"].ToString() != "无")
                    {
                        tempJObject["ReceiptTax"] = Math.Round(double.Parse(tempJObject["ReceiptAmount"].ToString()) * 0.09, 3);
                    }
                }

                tempJObject.Add("Code", code);
            }

            msg = SqlHelper.Exce(SqlHelper.GetInsertString(reimburseDetailList, "yl_reimburse_detail"));

            if (!msg.Contains("操作成功"))
            {
                return "新增报销明细失败，请重试";
            }
        }

        return "提交成功";
        //string id = MobileReimburseSrv.findMaxId().Tables[0].Rows[0][0].ToString();
    }

    public static DataTable findDepartmentIdByName(string name)
    {
        DataSet ds = MobileReimburseSrv.findDepartmentIdByName(name);

        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    public static string clearApprovalProcess(string docCode)
    {
        SqlExceRes sqlRes = new SqlExceRes(MobileReimburseSrv.clearApprovalProcess(docCode));

        return sqlRes.GetResultString("删除成功！", "删除失败！");
    }

    public static string updateApprovalTimeAndResultAndOpinion(string docCode, string result, string opinion)
    {
        SqlExceRes sqlRes = new SqlExceRes(MobileReimburseSrv.updateApprovalTimeAndResultAndOpinion(docCode, result, opinion));

        return sqlRes.GetResultString("更新成功！", "更新失败！");
    }

    public static DataTable checkMultiDepartment(UserInfo user)
    {
        DataSet ds = MobileReimburseSrv.checkMultiDepartment(user);
        if (ds == null)
            return null;

        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DataRow dr = dt.NewRow();

                dr["value"] = row["name"];
                dr["target"] = row["name"];

                dt.Rows.Add(dr);
            }
        }
        return dt;
    }

    public static DataTable findApprovalRecord(UserInfo user, string keyword,string year,int month)
    {
        DataSet ds = MobileReimburseSrv.findApprovalRecord(user.userId.ToString(), keyword,year,month);
        if (ds == null)
            return null;
        return ds.Tables[0];
    }

    /// <summary>
    /// 按照不同的方式进行报销审批记录的统计
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <param name="year">年份</param>
    /// <param name="month">月份</param>
    /// <param name="type">统计方式：姓名，费用归属部门，费用明细</param>
    /// <returns>统计结果</returns>
    public static string accountStatistics(string userId,string year,int month,string type)
    {
        JObject res = new JObject();
        if(string.IsNullOrEmpty(userId)||string.IsNullOrEmpty(type))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        if(type.Equals("name"))
        {
            type = "`name`";
        }
        else if (type.Equals("department"))
        {
            type = "fee_department";
        }
        else if (type.Equals("detail"))
        {
            type = "fee_detail";
        }
        string msg = "";
        DataSet ds = MobileReimburseSrv.accountStatistics(userId, year, month, type, ref msg);
        if(ds==null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else if(ds.Tables[0].Rows.Count==0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "未找到审批记录");
        }
        else
        {
            var dt1 = ds.Tables[0];
            var dt2 = ds.Tables[1];
            foreach (DataRow dr in dt2.Rows)
            {
                dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
            }
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("statistics", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt1));
            res.Add("detail", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt2));
        }
        return res.ToString();
    }

    public static DataTable findRelatedReimburse(UserInfo user, string keyword)
    {
        DataSet ds = MobileReimburseSrv.findRelatedReimburse(user.userId.ToString(), keyword);
        if (ds == null)
            return null;
        return ds.Tables[0];
    }

    public static DataSet findInformerByCode(string docCode)
    {
        DataSet ds = MobileReimburseSrv.findInformerByCode(docCode);
        if (ds == null)
            return null;
        return ds;
    }
}