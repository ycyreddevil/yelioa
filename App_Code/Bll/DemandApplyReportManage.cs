using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;


/// <summary>
/// DemandApplyReportManage 的摘要说明
/// </summary>
public class DemandApplyReportManage
{
    public DemandApplyReportManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string find(string name, string q, string userId)
    {
        DataSet ds = new DataSet();
        if (name == "findHospitalName")
        {
            ds = DemandApplyReportSrv.findHospitalName(q, userId);
        }
        else if (name == "findProductName")
        {
            ds = DemandApplyReportSrv.findProductName(q, userId);
        }
        else if (name == "findInformer")
        {
            ds = DemandApplyReportSrv.findInformer();
        }
        else if (name == "findSpec")
        {
            ds = DemandApplyReportSrv.findSpec(q);
        }
        else if (name == "findUnit")
        {
            ds = DemandApplyReportSrv.findUnit(q);
        }
        else if (name == "findAgentName")
        {
            ds = DeliverApplyReportSrv.findAgent(q, userId);
        }


        if (ds == null)
            return "";

        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (q != "" && name != "findUnit" && name != "findSpec" && name != "findInformer" && name != "findAgentName")
                {
                    if (PinYinHelper.IsEqual(row[0].ToString(), q)
                       || row[0].ToString().Trim().Contains(q)
                        )
                    {
                        DataRow dr = dt.NewRow();
                        dr["value"] = row[0];
                        dr["target"] = row[0];
                        dt.Rows.Add(dr);
                    }
                }
                else if (name == "findInformer")
                {
                    if (PinYinHelper.IsEqual(row[0].ToString(), q)
                    || row[0].ToString().Trim().Contains(q)
                     )
                    {
                        DataRow dr = dt.NewRow();
                        dr["value"] = row[1];
                        dr["target"] = row[0];
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr["value"] = row[0];
                    dr["target"] = row[0];
                    dt.Rows.Add(dr);
                }

            }
        }
        if (dt == null)
            return "";
        return JsonHelper.DataTable2Json(dt);
    }

    public static DataTable findHospitalName(string name, string userId)
    {
        DataSet ds = DemandApplyReportSrv.findHospitalName(name, userId);

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
            ds = DemandApplyReportSrv.findHospitalName(userId);

            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (index >= 5)
                {
                    break;
                }

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

    public static DataTable findProductName(string name, string userId)
    {
        DataSet ds = DemandApplyReportSrv.findProductName(name, userId);

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
            ds = DemandApplyReportSrv.findProductName(userId);

            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (index >= 5)
                {
                    break;
                }

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

    public static string insertDemandApplyReport(string hospitalName,string netSales,string stock,
            string productName, string agentName, string spec, string unit, string applyNumber, string remark, UserInfo userInfo, List<string> approverList, List<string> informerList)
    {
        // 通过产品名 规格 单位来确定产品代码
        DataSet productDs = DemandApplyReportSrv.findProductCode(productName, spec, unit);

        string productCode = "";

        if (productDs != null && productDs.Tables[0].Rows.Count > 0)
        {
            productCode = productDs.Tables[0].Rows[0][0].ToString();
        }

        // 通过医院名来确定医院代码
        DataSet hospitalDs = DemandApplyReportSrv.findHospitalCode(hospitalName);

        string hospitalCode = "";

        if (hospitalDs != null && hospitalDs.Tables[0].Rows.Count > 0)
        {
            hospitalCode = hospitalDs.Tables[0].Rows[0][0].ToString();
        }

        // 通过代理商名称来确定代理商编码
        DataSet agentDs = DeliverApplyReportSrv.findAgentCode(agentName);

        string agentCode = "";

        if (agentDs != null && agentDs.Tables[0].Rows.Count > 0)
        {
            agentCode = agentDs.Tables[0].Rows[0][0].ToString();
        }

        string data = DemandApplyReportSrv.insertDemandApplyReport(hospitalCode, productCode, agentCode, applyNumber, remark, userInfo, netSales, stock);
        string id = JsonHelper.DeserializeJsonToObject<JObject>(data)["Id"].ToString();

        // 新增审批人到审批流程表中
        List<JObject> list = new List<JObject>();

        for (int i = 0; i < approverList.Count; i++)
        {
            JObject jO = new JObject();
            jO.Add("index", i);
            jO.Add("userId", approverList[i]);

            list.Add(jO);
        }

        MobileReimburseSrv.insertApprovalProcess(id, list, "demand_apply_report");

        DataSet maxIdDt = DemandApplyReportSrv.findMaxId();
        string docCode = maxIdDt.Tables[0].Rows[0][0].ToString();
        // 审批单提交
        ApprovalFlowManage.SubmitDocument("demand_apply_report", docCode, userInfo,
            "http://yelioa.top//mDemandApplyReportAppRoval.aspx?type=0&docCode=" + docCode, "http://yelioa.top//mDemandApplyReportAppRoval.aspx?type=1&docCode=" + docCode,
            "AirlZ8lfY50d1KGDklHPQcLV2RUFAdrhD-WXU23cA-w", "DemandApplyReport", "1000013");

        // 新增知悉人到表
        DemandApplyReportSrv.insertInformer(docCode, informerList);

        return "提交成功";
    }


    public static string GetDocument(string starttm, string endtm, string applyName, string hospital, string product, string isChecked)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(isChecked))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
        }
        else
        {
            string msg = "";
            DataSet ds = DemandApplyReportSrv.GetDocument(starttm, endtm, applyName, hospital, product, isChecked,ref msg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else if(ds.Tables[0].Rows.Count==0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "无相关单据");
            }
            else
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
                res.Add("Document", JsonHelper.DataTable2Json(ds.Tables[0]));
            }
        }



        return res.ToString();
    }

    public static string updateActualFee(ArrayList list)
    {
        ArrayList codeList = new ArrayList();
        ArrayList actualFeeList = new ArrayList();

        foreach (Dictionary<string, string> dict in list)
        {
            if (dict != null)
            {
                codeList.Add(dict["Id"].ToString());
                actualFeeList.Add(dict["ApprovalNumber"].ToString());
            }
            else
            {
                codeList.Add("");
                actualFeeList.Add("");
            }
        }

        string res = DemandApplyReportSrv.updateActualFee(list);

        DemandApplyReportSrv.UpdateOperationApprovalTime(codeList);



        string[] msgs = res.Split(';');
        DataTable dtUser = ReimbursementSrv.GetUserNameAndWxUserId();
        WxCommon wx = new WxCommon("DeliverApplyReport",
            "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc",
            "1000009",
            "");
        for (int i = 0; i < msgs.Length - 1; i++)
        {
            SqlExceRes sqlRes = new SqlExceRes(msgs[i]);
            if (sqlRes.Result == SqlExceRes.ResState.Success)
            {
                Dictionary<string, string> dict = (Dictionary<string, string>)list[i];
                string WxUserId = "";
                foreach (DataRow row in dtUser.Rows)
                {
                    if (row["userName"].ToString() == dict["ApprovalName"])
                    {
                        WxUserId = row["wechatUserId"].ToString();
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(WxUserId))
                {
                    // 发送审批的消息给提交者
                    wx.SendWxMsg(WxUserId, "审批通知", "您编号为" + codeList[i] + "的审批单据已被运营部进行批准数量复审,审批人为:"
                        + dict["ApprovalName"] + "，实发数量为" + actualFeeList[i] + "，请知悉"
                        , "http://yelioa.top//mDemandApplyReportAppRoval.aspx?type=0&docCode=" + codeList[i]);
                }
            }
        }

        return res;
    }

    public static string Reject(List<string> ids, string opinion, string name)
    {
        // 审批拒绝后要把发货申请表相关数据删除
        string msg = DemandApplyReportSrv.Reject(ids, opinion);
        DataSet ds = new DataSet();
        if ((msg.Length - msg.Replace("操作成功", "").Length) / "操作成功".Length == ids.Count)
        {
            ds = OperationDeliverSrv.GetwechatUserId(ids);
            WxCommon wx = new WxCommon("DeliverApplyReport",
           "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc",
           "1000009",
           "");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    wx.SendWxMsg(row["wechatUserid"].ToString(), "审批通知", "您编号为" + row["Id"] + "的审批单据已被运营部拒绝,审批人为:" + name + ",拒绝理由为:" + opinion + "。请知悉",
                "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0&docCode=" + row["Id"]);
                }
            }
        }
        return msg;
    }

}