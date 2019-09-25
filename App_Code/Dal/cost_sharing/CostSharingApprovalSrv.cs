using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// CostSharingApprovalSrv 的摘要说明
/// </summary>
public class CostSharingApprovalSrv
{
    public CostSharingApprovalSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet queryPendingApprovalList(UserInfo userInfo)
    {
        // 新增-待审批
        string sql1 = string.Format("SELECT u.username 代表,csd.newValue,csd.FieldName,csr. CODE,csr.level,u.avatar FROM cost_sharing_record csr " +
        "left join cost_sharing_detail csd on csr.`Code` = csd.RegistrationCode LEFT JOIN cost_sharing_detail csd1 ON csr.`Code` = csd1.RegistrationCode and csd1.FieldName ='代表' " +
        "LEFT JOIN users u ON csd1.NewValue = u.userid inner join cost_sharing_field_level csfl on csfl.FieldName = csd.FieldName WHERE csr.approverUserId LIKE '%{0}%' AND csr.State = '审批中' " +
        "AND csr.InsertOrUpdate = 0 and(csfl.FieldName = '网点医院名称' or csfl.FieldName = '产品（包含规格型号）') group by 代表,fieldName,level", userInfo.userId.ToString());

        // 更新1-待审批
        // string sql2 = string.Format("select vncs.代表,csr.code, GROUP_CONCAT(CONCAT(vncs.网点医院名称,'|',vncs.产品（包含规格型号）) SEPARATOR ',') info,u.avatar " +
        // "from cost_sharing_record csr left join v_new_cost_sharing vncs on csr.newCostSharingid = vncs.id " +
        //"left join users u on vncs.salesId = u.userid " +
        //"where csr.approverUserId like '%{0}%' and csr.State = '审批中' and csr.InsertOrUpdate = 1", userInfo.userId.ToString());

        string sql2 = string.Format("SELECT u.username 代表,csd.newValue,csd.FieldName,csr. CODE,csr.level,u.avatar FROM cost_sharing_record csr " +
        "left join cost_sharing_detail csd on csr.`Code` = csd.RegistrationCode LEFT JOIN cost_sharing_detail csd1 ON csr.`Code` = csd1.RegistrationCode and csd1.FieldName ='代表' " +
        "LEFT JOIN users u ON csd1.NewValue = u.userid inner join cost_sharing_field_level csfl on csfl.FieldName = csd.FieldName WHERE csr.approverUserId LIKE '%{0}%' AND csr.State = '审批中' " +
        "AND csr.InsertOrUpdate = 1 and(csfl.FieldName = '网点医院名称' or csfl.FieldName = '产品（包含规格型号）') group by 代表,fieldName,level", userInfo.userId.ToString());

        List<string> sqls = new List<string>();

        sqls.Add(sql1);
        sqls.Add(sql2);

        DataSet ds = SqlHelper.Find(sqls.ToArray());

        if (ds == null)
            return null;

        return ds;
    }
}