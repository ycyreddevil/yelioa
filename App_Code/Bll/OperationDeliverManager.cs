using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// OperationDeliverManage 的摘要说明
/// </summary>
public class OperationDeliverManager
{
    public OperationDeliverManager()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataTable findByCond(string starttm, string endtm, string applyName, string hospital, string product, string isChecked)
    {
        DataSet ds = OperationDeliverSrv.findByCond(starttm, endtm, applyName, hospital, product, isChecked);

        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    public static DataTable findByCondInfomer(UserInfo user)
    {
        DataSet ds = OperationDeliverSrv.findByCondInfomer(user.userId.ToString());

        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    public static string updateReason(List<string> ids, string reason)
    {
        string msg = OperationDeliverSrv.updateReason(ids, reason);

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
                    wx.SendWxMsg(row["wechatUserid"].ToString(), "未发货通知", "您编号为" + row["Id"] + "的发货单据因'" + reason + "'无法发货，请知悉。",
                "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0&docCode=" + row["Id"]);
                }
            }
        }

        return msg;
    }

    public static string updateDeliverCode(List<string> ids, string deliverCode)
    {
        string msg = OperationDeliverSrv.updateDeliverCode(ids, deliverCode);

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
                    wx.SendWxMsg(row["wechatUserid"].ToString(), "快递单号更新通知", "您编号为" + row["Id"] + "的发货单据快递单号为:'" + deliverCode + "'，请知悉。",
                "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0&docCode=" + row["Id"]);
                }
            }
        }

        return msg;
    }

    public static string updateReceiptCode(List<string> ids, string receiptCode)
    {
        string msg = OperationDeliverSrv.updateReceiptCode(ids, receiptCode);

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
                    wx.SendWxMsg(row["wechatUserid"].ToString(), "发票单号更新通知", "您编号为" + row["Id"] + "的发货单据发票单号为:'" + receiptCode + "'，请知悉。",
                "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0&docCode=" + row["Id"]);
                }
            }
        }

        return msg;
    }

    public static string Reject(List<string> ids, string opinion, string name)
    {
        // 审批拒绝后要把发货申请表相关数据删除
        string msg = OperationDeliverSrv.Reject(ids, opinion);
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
                    wx.SendWxMsg(row["wechatUserid"].ToString(), "审批通知", "您编号为" + row["Id"] + "的发货单据已被运营部拒绝,审批人为:" + name + ",拒绝理由为:" + opinion + "。请知悉",
                "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0&docCode=" + row["Id"]);
                }
            }
        }
        return msg;
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

        string res = OperationDeliverSrv.updateActualFee(list);

        OperationDeliverSrv.UpdateOperationApprovalTime(codeList);

        // 插入一条数据到销售日表中
        OperationDeliverSrv.insertSalesData(codeList);

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
                    wx.SendWxMsg(WxUserId, "审批通知", "您编号为" + codeList[i] + "的发货单据已被运营部进行实发数量复审,审批人为:"
                        + dict["ApprovalName"] + "，实发数量为" + actualFeeList[i] + "，请知悉"
                        , "http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0&docCode=" + codeList[i]);
                }
            }
        }

        return res;
    }


    public static string AddBranch(string name, string code)
    {
        string res = OperationDeliverSrv.AddBranch(name, code);
        if (res.Contains("操作成功"))
        {
            res = "操作成功";
        }
        return res;
    }

    public static string AddProduct(string name, string code, string spec, string unit)
    {
        string res = OperationDeliverSrv.AddProduct(name, code, spec, unit);
        if (res.Contains("操作成功"))
        {
            res = "操作成功";
        }
        return res;
    }
}