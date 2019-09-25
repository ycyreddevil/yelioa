using System.Data;

namespace Dal.cost_sharing
{
    /// <summary>
    /// CostSharingSubmitSrv 的摘要说明
    /// </summary>
    public class CostSharingSubmitSrv
    {
        public CostSharingSubmitSrv()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public static DataTable queryRelativeCostSharing(UserInfo userInfo)
        {
            string sql = string.Format("select t1.代表, GROUP_CONCAT(CONCAT(t1.网点医院名称,'|',t1.产品（包含规格型号）) SEPARATOR ',') info,t2.avatar from v_new_cost_sharing t1 " +
                                       "left join users t2 on t1.salesId = t2.userid where t1.supervisorId = '{0}' and 代表 != 'KG' and t2.isValid = '在职' group by 代表", userInfo.userId.ToString());
            DataSet ds = SqlHelper.Find(sql);
            if (ds == null)
                return null;
            return ds.Tables[0];
        }
    }
}