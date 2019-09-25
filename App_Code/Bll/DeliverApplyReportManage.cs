using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// DeliverApplyReportManage 的摘要说明
/// </summary>
public class DeliverApplyReportManage
{
    public DeliverApplyReportManage()
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
            ds = DeliverApplyReportSrv.findHospitalName(q, userId);
        }
        else if (name == "findProductName")
        {
            ds = DeliverApplyReportSrv.findProductName(q, userId);
        }
        else if (name == "findInformer")
        {
            ds = DeliverApplyReportSrv.findInformer();
        }
        else if (name == "findSpec")
        {
            ds = DeliverApplyReportSrv.findSpec(q);
        }
        else if (name == "findUnit")
        {
            ds = DeliverApplyReportSrv.findUnit(q);
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
        DataSet ds = DeliverApplyReportSrv.findHospitalName(name, userId);

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
            ds = DeliverApplyReportSrv.findHospitalName(userId);

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
        DataSet ds = DeliverApplyReportSrv.findProductName(name, userId);

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
            ds = DeliverApplyReportSrv.findProductName(userId);

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


    //public static string findSpecAndUnit(string name)
    //{
    //    DataSet ds = DeliverApplyReportSrv.findSpecAndUnit(name);

    //    if (ds == null || ds.Tables[0].Rows.Count == 0)
    //        return null;

    //    DataTable specDt = new DataTable();
    //    specDt.Columns.Add("value", Type.GetType("System.String"));
    //    specDt.Columns.Add("target", Type.GetType("System.String"));

    //    DataTable unitDt = new DataTable();
    //    unitDt.Columns.Add("value", Type.GetType("System.String"));
    //    unitDt.Columns.Add("target", Type.GetType("System.String"));

    //    List<string> specList = new List<string>();
    //    List<string> unitList = new List<string>();

    //    foreach (DataRow row in ds.Tables[0].Rows)
    //    {
    //        if (row["specification"] != null && !specList.Contains(row["specification"].ToString()))
    //        {
    //            DataRow specDr = specDt.NewRow();
    //            specDr["value"] = row["specification"];
    //            specDr["target"] = row["specification"];

    //            specDt.Rows.Add(specDr);

    //            specList.Add(row["specification"].ToString());
    //        }

    //        if (row["unit"] != null && !unitList.Contains(row["unit"].ToString()))
    //        {
    //            DataRow unitDr = unitDt.NewRow();
    //            unitDr["value"] = row["unit"];
    //            unitDr["target"] = row["unit"];

    //            unitDt.Rows.Add(unitDr);

    //            unitList.Add(row["unit"].ToString());
    //        }
    //    }

    //    JObject jObject = new JObject();

    //    jObject.Add("specJson", JsonHelper.DataTable2Json(specDt));
    //    jObject.Add("unitJson", JsonHelper.DataTable2Json(unitDt));

    //    return jObject.ToString();
    //}

    public static string insertDeliverApplyReport(string deliverType, string hospitalName,
            string productName, string agentName, string spec, string unit, string applyNumber, string remark, UserInfo userInfo, 
            List<string> approverList, List<string> informerList, List<string> uploadFileUrlsList,
            string stock, string netSales, string period, string isStockReceiptTogether, string deliverAddress, string deliverName, string deliverPhone)
    {
        // 通过产品名 规格 单位来确定产品代码
        DataSet productDs = DeliverApplyReportSrv.findProductCode(productName, spec, unit);

        string productCode = "";

        if (productDs != null && productDs.Tables[0].Rows.Count > 0)
        {
            productCode = productDs.Tables[0].Rows[0][0].ToString();
        }

        // 通过医院名来确定医院代码
        DataSet hospitalDs = DeliverApplyReportSrv.findHospitalCode(hospitalName);

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

        string data = DeliverApplyReportSrv.insertDeliverApplyReport(deliverType, hospitalCode, productCode, agentCode, applyNumber, 
            remark, userInfo, stock, netSales, period, isStockReceiptTogether, deliverAddress, deliverName, deliverPhone);

        string id = JsonHelper.DeserializeJsonToObject<JObject>(data)["Id"].ToString();

        // 新增审批人到审批流程表中
        List<JObject> list = new List<JObject>();

        if (Double.Parse(applyNumber) > ((Double.Parse(netSales) / 30 * Double.Parse(period)) - Double.Parse(stock)))
        {
            approverList.Insert(1, "100000645");
        }

        for (int i = 0; i < approverList.Count; i++)
        {
            JObject jO = new JObject();

            jO.Add("index", i);
            jO.Add("userId", approverList[i]);

            list.Add(jO);
        }

        MobileReimburseSrv.insertApprovalProcess(id, list, "deliver_apply_report");

        // 审批单提交
        ApprovalFlowManage.SubmitDocument("deliver_apply_report", id, userInfo,
            "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0&docCode=" + id, "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=1&docCode=" + id,
            "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc", "DeliverApplyReport", "1000009");

        // 新增知悉人到表
        DeliverApplyReportSrv.insertInformer(id, informerList);

        if (uploadFileUrlsList != null && uploadFileUrlsList.Count > 0)
            DeliverApplyReportSrv.insertAttachement(id, uploadFileUrlsList);

        //// 新增一条记录到审批记录表中
        //DeliverApplyReportSrv.insertDeliverAppyReportRecord("0", userInfo.wechatUserId, userInfo.wechatUserId, "发货申请单提交", "");

        //// 给待审批人发送消息
        //WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper();

        //wxNetSalesHelper.GetJsonAndSendWxMsg(approverIds, "请及时审批 提交人为:" + userInfo.userName
        //    + "的发货申请单,谢谢!", "http://yelioa.top/mNetSalesApproval.aspx?type=others");
        //// 给提交人发送消息
        //wxNetSalesHelper.GetJsonAndSendWxMsg(userInfo.wechatUserId, "您的发货申请单已提交，请点击查看", "http://yelioa.top/mNetSalesApproval.aspx?type=mine");

        return "提交人";
    }

    //public static string approvalDeliverApplyReport(string docCode, string deliverType, UserInfo userInfo, string approvalResult, string approvalOpinion)
    //{
    //    ApprovalFlowManage.ApproveDocument("deliver_apply_report", docCode, userInfo, approvalResult, approvalOpinion);

    //    //int recordCount = 0; int approverCount = 0;

    //    //// 先判断该单据审批记录表中含有几条记录
    //    //DataSet ds = DeliverApplyReportSrv.findRecordCount(docCode);

    //    //if (ds != null && ds.Tables[0].Rows.Count > 0)
    //    //{
    //    //    string count = ds.Tables[0].Rows[0][0].ToString();

    //    //    recordCount = Int32.Parse(count);
    //    //}

    //    //ds = DeliverApplyReportSrv.findApproverCount(docCode);

    //    //// 再判断该单据的类型 如果是非样品
    //    //if (! "4".Equals(deliverType))
    //    //{
    //    //    // 更细单据状态为审批结束
    //    //    DeliverApplyReportSrv.updateRecordStatus(docCode, 2);

    //    //    // 新增一条记录到审批记录表中
    //    //    DeliverApplyReportSrv.insertDeliverAppyReportRecord("", );
    //    //}
    //    //else
    //    //{

    //    //}





    //    return null;
    //}
}