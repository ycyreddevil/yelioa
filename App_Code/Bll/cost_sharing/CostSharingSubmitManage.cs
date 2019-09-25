using System.Data;
using System.Linq;
using Dal.cost_sharing;
using Newtonsoft.Json.Linq;

namespace Bll.cost_sharing
{
    /// <summary>
    /// CostSharingSubmit 的摘要说明
    /// </summary>
    public class CostSharingSubmitManage
    {
        public CostSharingSubmitManage()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public static string queryRelativeCostSharing(UserInfo userInfo)
        {
            DataTable dt = CostSharingSubmitSrv.queryRelativeCostSharing(userInfo);

            JObject resJObject = new JObject();

            if (dt == null || dt.Rows.Count == 0)
            {
                resJObject.Add("ErrCode", 2);
                resJObject.Add("ErrMsg", "系统中无对应的网点信息");

                return resJObject.ToString();
            }
            resJObject.Add("ErrCode", 0);
            JArray jArray = new JArray();
            foreach (DataRow dr in dt.Rows)
            {
                JObject tempJObject = new JObject();
                tempJObject.Add("代表", dr["代表"].ToString());
                string[] infos = dr["info"].ToString().Split(',');
                JArray tempJA = new JArray();
                foreach (string info in infos)
                {
                    string newCostSharingId = SqlHelper.Find(string.Format("select id from v_new_cost_sharing where 代表='{0}' " +
                    "and 产品（包含规格型号）='{1}' and 网点医院名称='{2}'", dr["代表"].ToString(), info.Split('|')[1], info.Split('|')[0])).Tables[0].Rows[0][0].ToString();
                    JObject tempJ = new JObject();
                    tempJ.Add("info", info);
                    tempJ.Add("costSharingId", newCostSharingId);
                    tempJA.Add(tempJ);
                }
                tempJObject.Add("网点信息", tempJA);
                tempJObject.Add("avatar", dr["avatar"].ToString());

                jArray.Add(tempJObject);
            }

            resJObject.Add("RelativeCostSharing", jArray);

            return resJObject.ToString();
        }
    }
}