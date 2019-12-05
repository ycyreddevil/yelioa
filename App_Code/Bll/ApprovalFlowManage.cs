using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using Newtonsoft.Json.Linq;

/// <summary>
/// ApprovalFlowManage 的摘要说明
/// </summary>
public class ApprovalFlowManage
{
    public ApprovalFlowManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 审批流程--创建表单。
    /// 需要在dict当中添加"DocumentTableName"的key，用于保存表单表名
    /// </summary>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static string CreateDocument(Dictionary<string,string> dict)
    {
        //单据草稿为可编辑状态
        if (!dict.Keys.Contains("Editable"))
            dict.Add("Editable", "1");
        else
        {
            dict.Remove("Editable");
            dict.Add("Editable", "1");
        }
        SqlExceRes sqlRes = new SqlExceRes(ApprovalFlowSrv.CreateDocument(dict));
        return sqlRes.GetResultString("提交成功！", "该单据已创建，请勿重复创建！");
    }

    public static string SubmitDocument(string table, string docCode, UserInfo user, string url1, string url2, 
        string appSecret, string thisAppName, string agentId)
    {
        string res = "";
        DataSet ds = ApprovalFlowSrv.GetDocumentInfo(table, docCode, ref res);
        if (ds == null)//出错，返回错误信息
            return "获取单据信息出错，错误信息：" + res;

        if (ds.Tables[0].Rows.Count > 0 && ds.Tables[2].Rows.Count > 0)
        {
            return "该单据已提交，请勿重复提交";
        }
        //保存审批记录信息
        Dictionary<string, string> dict = new Dictionary<string, string>();
        SqlExceRes sqlRes = new SqlExceRes(ApprovalFlowSrv.SubmitDocumentSaveRecord(user, ds,ref dict));
        res = sqlRes.GetResultString("提交成功！", "该单据已提交，请勿重复提交！");
        if (res != "提交成功！")
        {
            return "提交单据出错，错误信息：" + res;
        }
        //保存所有审批人信息
        JArray listApproverInfo = null;//所有审批人信息
        ApprovalFlowSrv.SaveAllApproverInfo(user, ds,dict,ref listApproverInfo);

        //更新单据状态,state="审批中",level=1
        res = ApprovalFlowSrv.UpdateDocStatus(table, docCode, "审批中",1);

        DataSet userDs = ApprovalFlowSrv.GetUserInfoByUserId(
            ApprovalFlowSrv.GetApproverIddByLevel(1, listApproverInfo));        

        string approverIds = "";
        foreach (DataTable dt in userDs.Tables)
        {
            approverIds += dt.Rows[0]["wechatUserId"].ToString()+"|";
        }
        approverIds = approverIds.Substring(0, approverIds.Length - 1);

        WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper(appSecret, thisAppName, agentId);
        //// 给待审批人发送消息
        //wxNetSalesHelper.GetJsonAndSendWxMsg(approverIds, "请及时审批 提交人为:" + user.userName
        //    + "的单据,谢谢!", url2, agentId);
        //// 给提交人发送消息
        wxNetSalesHelper.GetJsonAndSendWxMsg(user.wechatUserId, "您的审批单据已提交 请耐心等待审批人审批", url1, agentId);

        return res;
    }

    public static string returnDocument(string table, string docCode, UserInfo user)
    {
        String res = "";
        DataSet ds = ApprovalFlowSrv.GetDocumentInfo(table, docCode, ref res);
        if (ds == null)//出错，返回错误信息
            return "获取单据信息出错，错误信息：" + res;

        if (Convert.ToInt32(ds.Tables[3].Rows[0]["Level"]) >1)
        {
            return "该单据已被审批，无法进行撤回";
        }
        //保存单据撤回记录
        SqlExceRes sqlRes = new SqlExceRes(ApprovalFlowSrv.returnDocument(user, ds));
        res = sqlRes.GetResultString("撤回成功！", "该单据已被审批，无法进行撤回！");
        if (res != "撤回成功！")
        {
            return "撤回单据出错，错误信息：" + res;
        }

        // 更新表单状态
        ApprovalFlowSrv.UpdateDocStatus(table, docCode, "未提交", 0);

        //清除审批人相关信息
        ApprovalFlowSrv.ClearCurrentApproverInfo(table, docCode);

        return res;
    }

    public static string ApproveDocument(string table, string docCode, UserInfo user
        , string ApprovalResult, string ApprovalOpinions, string url1, string url2, string url3,
        string appSecret, string thisAppName, string agentId)
    {
        string res = "";
        DataSet ds = ApprovalFlowSrv.GetDocumentInfo(table, docCode, ref res);
        if (ds == null)//出错，返回错误信息
            return "获取单据信息出错，错误信息：" + res;

        WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper(appSecret, thisAppName, agentId);
        DataTable DtAppApprover = ds.Tables[0];
        DataTable DtAppProcess = ds.Tables[1];
        DataTable DtAppRecord = ds.Tables[2];
        DataTable DtDocument = ds.Tables[3];
        DataTable DtInformer = ds.Tables[5];

        int SubmitorUserId = Convert.ToInt32(DtAppRecord.Rows[0]["SubmitterId"]);
        DataSet dsUserInfo = ApprovalFlowSrv.GetUserInfoByUserId(new int[] { SubmitorUserId });
        string SubmitorWechatId = dsUserInfo.Tables[0].Rows[0]["wechatUserId"].ToString();
        string SubmitorName = dsUserInfo.Tables[0].Rows[0]["userName"].ToString();

        int level = Convert.ToInt32(DtDocument.Rows[0]["Level"]);
        int maxLevel = Convert.ToInt32(DtAppProcess.Rows[DtAppProcess.Rows.Count - 1]["Level"]);

        string informer = "";
        foreach(DataRow row in ds.Tables[5].Rows)
        {
            informer += row["wechatUserId"].ToString() + "|";
        }
        Boolean flag = false;
        List<string> tempList = new List<string>();
        for(int i=0;i<DtAppApprover.Rows.Count;i++)
        {
            tempList.Add(DtAppApprover.Rows[i]["ApproverId"].ToString());
            if (i == level)
            {

                if(!flag)
                {
                    string saveRecordMsg = ApprovalFlowSrv.ApproveDocumentSaveRecord(DtAppApprover.Rows[i]["ApproverId"].ToString(), ds, ApprovalResult, ApprovalOpinions, level);
                    SqlExceRes sqlRes = new SqlExceRes(saveRecordMsg);
                    res = sqlRes.GetResultString("审批结果提交成功！", "该单据已提交，请勿重复提交！");
                    if (res != "审批结果提交成功！")
                    {
                        return "提交审批结果出错，错误信息：" + res;
                    }
                }
                else
                {
                    string saveRecordMsg = ApprovalFlowSrv.ApproveDocumentSaveRecord(DtAppApprover.Rows[i]["ApproverId"].ToString(), ds, ApprovalResult, "自动审批", level);
                    SqlExceRes sqlRes = new SqlExceRes(saveRecordMsg);
                    res = sqlRes.GetResultString("审批结果提交成功！", "该单据已提交，请勿重复提交！");
                    if (res != "审批结果提交成功！")
                    {
                        return "提交审批结果出错，错误信息：" + res;
                    }
                }
               

                if (!ApprovalResult.Contains("不同意"))//审批同意
                {
                    level++;
                    if (level > maxLevel)//审批到最后一级，审批流程结束
                    {
                        if (informer != "")
                        {
                            informer = informer.Substring(0, informer.Length - 1);
                            wxNetSalesHelper.GetJsonAndSendWxMsg(informer, "有一条与您相关的单据审批流程结束，请知悉", url1, agentId);
                        }

                        res = "审批流程结束";
                        //清除审批人相关信息
                        ApprovalFlowSrv.ClearCurrentApproverInfo(table, docCode);

                        // 更新表单状态
                        ApprovalFlowSrv.UpdateDocStatus(table, docCode, "已审批", level);

                        // 发送审批的消息给提交者
                        wxNetSalesHelper.GetJsonAndSendWxMsg(SubmitorWechatId, "您的审批单据审批流程结束，请知悉", url1, agentId);

                        // 审批流程结束后，需要把纯销数据更新到flow_statistic表中
                        //NetSalesInfoSrv.updateNetSalesAndStockAfterApproval(docCode);



                    }
                    else
                    {
                        string tempUserId = tempUserId = DtAppApprover.Rows[level]["ApproverId"].ToString();

                        res = "审批同意！";

                        // 更新表单状态
                        ApprovalFlowSrv.UpdateDocStatus(table, docCode, "审批中", level);


                        // 发送审批的消息给提交者
                        wxNetSalesHelper.GetJsonAndSendWxMsg(SubmitorWechatId, "您审批单据已被"
                            + user.userName + "审批，结果为同意，请知悉"
                           , url1, agentId);
                        if (!tempList.Contains(tempUserId))
                        {
                            
                        // 向审批人发送审批通知 
                        DataSet userDs = ApprovalFlowSrv.GetUserInfoByUserId(
                            ApprovalFlowSrv.GetApproverIddByLevel(level, DtAppApprover));

                        string approverIds = "";
                        foreach (DataTable dt in userDs.Tables)
                        {
                            approverIds += dt.Rows[0]["wechatUserId"].ToString() + "|";
                        }
                        approverIds = approverIds.Substring(0, approverIds.Length - 1);

                            wxNetSalesHelper.GetJsonAndSendWxMsg(approverIds, "已收到提交人为: " + SubmitorName
                                + "的单据,请及时审批，谢谢"
                                , url2, agentId);
                            break;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
                else//审批不同意
                {
                    if (informer != "")//发送消息给抄送人
                    {
                        informer = informer.Substring(0, informer.Length - 1);
                        wxNetSalesHelper.GetJsonAndSendWxMsg(informer, "有一条与您相关的单据被审批拒绝，请知悉", url1, agentId);
                    }
                    //清除审批人相关信息
                    ApprovalFlowSrv.ClearCurrentApproverInfo(table, docCode);
                    // 更新表单状态
                    if ("yl_reimburse".Equals(table) || "wages".Equals(table) || "outer_wages".Equals(table) || "tax".Equals(table))
                    {
                        ApprovalFlowSrv.UpdateDocStatus(table, docCode, "已拒绝", 0);
                    }
                    else
                    {
                        ApprovalFlowSrv.UpdateDocStatus(table, docCode, "未提交", 0);
                    }

                    res = "审批拒绝！";

                   

                    string approver = "";

                    foreach (DataRow row in DtAppProcess.Rows)
                    {
                        if (Convert.ToInt32(row["Level"]) < level && Convert.ToInt32(row["Level"]) > 0)
                            approver += row["wechatUserId"].ToString() + "|";
                    }
                    if (approver != "")//发送消息给抄送人
                    {
                        approver = approver.Substring(0, approver.Length - 1);
                        wxNetSalesHelper.GetJsonAndSendWxMsg(approver, "有一条您审批通过的单据被审批拒绝，请知悉", url1, agentId);
                    }

                    // 发送审批被拒绝的消息
                    if ("".Equals(ApprovalOpinions))
                    {
                        wxNetSalesHelper.GetJsonAndSendWxMsg(SubmitorWechatId, "您的审批单据已被"
                        + user.userName + "审批，结果为拒绝，请重新提交该单据"
                        , url3, agentId);
                    }
                    else
                    {
                        wxNetSalesHelper.GetJsonAndSendWxMsg(SubmitorWechatId, "您的审批单据已被"
                        + user.userName + "审批，结果为拒绝，意见为：" + ApprovalOpinions + "，请重新提交该单据"
                        , url3, agentId);
                    }
                    
                    break;
                }
                if (table == "deliver_apply_report")//销售订单重复的审批人不需要自动审批
                {
                    break;
                }
            }
            
        }
        //保存审批记录
      
        return res;
    }

    public static string EndDeliverApprove(string id)
    {
        return ApprovalFlowSrv.AddDeliverApproveTime(id);
    }

    public static string EndDemandApprove(string id)
    {
        return ApprovalFlowSrv.AddDemandApproveTime(id);
    }
}