<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BudgetDistributeList.aspx.cs" Inherits="BudgetDistributeList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>预算分配列表页</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
</head>
<body>
    <div id="total">
        <el-table :data="tableData" border show-summary>
            <el-table-column type="index" label="序号" width="100">
 
            </el-table-column>
            <el-table-column prop="department" label="待分配部门" width="180">
                
            </el-table-column>  
            <el-table-column prop="CreateTime" label="上级分配日期" width="180">
                
            </el-table-column>
            <el-table-column prop="DocNo" label="单据号" width="180">
                
            </el-table-column>
            <el-table-column prop="BudgetLimitType" label="费用明细" width="180">
                
            </el-table-column>
            <el-table-column prop="BudgetLimit" label="分配额度" width="180">
                
            </el-table-column>
            <el-table-column
                label="操作"
                width="100">
                <template slot-scope="scope">
                    <el-button @click="queryDetail(scope.row)" type="text" size="small">查看详情</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
</body>
<script src="/Scripts/jquery.min.js"></script>
<script src="/Scripts/vue.js"></script>
<script src="https://unpkg.com/element-ui/lib/index.js"></script>
<script>
    var vue = new Vue({
        el: "#total",
        data: {
            tableData:[],
        },
        methods: {
            queryDetail: function (row) {
                location.href = "BudgetDistributeDetail.aspx?BudgetLimitType=" + row.BudgetLimitType +
                    "&&BudgetLimit=" + row.BudgetLimit + "&&firstOrSecondDistribute=" + row.FirstOrSecondDistribute +
                    "&&docNo=" + row.DocNo + "&&departmentId=" + row.DepartmentId + "&&isFinished=" + row.IsFinished;
            }
        },
        mounted: function() {
            getDepartmentDistributeDate();
        }
    })

    function getDepartmentDistributeDate() {
        $.ajax({
            url: 'BudgetDistributeList.aspx',
            data: {action:'getDepartmentDistributeList'},
            type: 'post',
            dataType: 'json',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.tableData = JSON.parse(msg.Doc);
                }
            },
            error: function(msg) {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        })
    }
</script>
</html>
