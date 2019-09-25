using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Dal.cost_sharing
{
    /// <summary>
    /// NewBranchUpdateSrv 的摘要说明
    /// </summary>
    public class NewBranchUpdateSrv
    {
        public NewBranchUpdateSrv()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public static DataSet getFormColumnsAndDataForSubmit(string newCostSharingId, UserInfo userInfo)
        {
            // 查询该网点是否已经进行了更新操作
            string isUpdateFinishedSql = string.Format("select 1 from cost_sharing_record where newCostSharingId = {0} " +
                                                      "and insertOrUpdate = 1 and state = '审批中' and ApproverUserId != '{1}'", 
                newCostSharingId, userInfo.userId);

            // 查询页面显示哪些字段
            string queryColumnSql = "select * from cost_sharing_field_level where insertOrUpdate = 1 and FieldLevel = 0";

            // 查询网点现有内容来回显
            string queryContentSql = string.Format("select * from v_new_cost_sharing where id = '{0}'", newCostSharingId);

            List<string> sqls = new List<string>();

            sqls.Add(isUpdateFinishedSql);
            sqls.Add(queryColumnSql);
            sqls.Add(queryContentSql);

            DataSet ds = SqlHelper.Find(sqls.ToArray());

            if (ds == null)
                return null;

            return ds;
        }

        public static DataSet getFormColumnsAndDataForApproval(string costSharingRecordId, int level)
        {
            // 查询页面上显示哪些字段以及对应的值
            string inner_queryColumnSql = string.Format("select t1.*,t3.NewValue, t3.OldValue, t3.level from cost_sharing_field_level t1 left join cost_sharing_record t2 on t1.fieldLevel = t2.level " +
            "left join cost_sharing_detail t3 on t1.fieldName = t3.fieldName where t2.insertOrUpdate = {1} and t2.code = '{0}' and t3.RegistrationCode = '{0}' " +
            "group by t1.fieldName, RelativeFieldName, level", costSharingRecordId, level);

            string queryColumnSql = string.Format("select aaa.* from ({0}) aaa where not exists (select bbb.* from ({0}) bbb where aaa.level < bbb.level)", inner_queryColumnSql);

            DataSet ds = SqlHelper.Find(queryColumnSql);

            if (ds == null)
                return null;

            return ds;
        }

        public static DataTable showColumnRelativeData(string tableName)
        {
            string sql = "";
            if ("users".Equals(tableName))
            {
                // 用户表
                sql = String.Format("select userId id, userName name from {0}", tableName);
            }
            else if ("jb_product".Equals(tableName))
            {
                // 产品表
                sql = String.Format("select id, concat(name,'(',specification,')') name from {0}", tableName);
            }
            else
            {
                // 其他表
                sql = string.Format("select id, name from {0}", tableName);
            }

            DataSet ds = SqlHelper.Find(sql);

            if (ds == null)
                return null;

            return ds.Tables[0];
        }

        public static DataSet getCostSharingInfoById(string costSharingRecordId, UserInfo userInfo)
        {
            List<string> sqls = new List<string>();

            // 通过 newCostSharingid得到记录id和审批到哪一级
            string sql = string.Format("select * from cost_sharing_record where code = '{0}'", costSharingRecordId);
            sqls.Add(sql);

            // 取上级负责人
            sql = string.Format("select userName,userId,wechatUserId from v_user_department_post where departmentId = (select " +
                                "departmentId from user_department_post where wechatUserId = '{0}') and isHead = 1", userInfo.wechatUserId);
            sqls.Add(sql);

            DataSet ds = SqlHelper.Find(sqls.ToArray());
            if (ds == null)
                return null;

            return ds;
        }
    }
}