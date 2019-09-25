<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BudgetSubmitList.aspx.cs" Inherits="web_budget_submit_BudgetSubmitList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>预算提交列表页</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
</head>
<body>
    <div id="total">
        <el-table :data="tableData" border v-loading="loading">
            <el-table-column type="index" label="序号" width="100">
     
            </el-table-column>
            <el-table-column prop="DepartmentName" label="提交部门" width="180">
                    
            </el-table-column>
           
            <%-- <el-table-column prop="LimitNumber" label="可用额度" width="180"> --%>
            <%--          --%>
            <%-- </el-table-column> --%>
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
            tableData: [],
            loading: true,
        },
        methods: {
            queryDetail: function (row) {
                location.href = "BudgetSubmitDetail.aspx?departmentId=" + row.DepartmentId + "&docNo=" +
                    row.DocNo + "&selfDepartmentId=" + row.SelfDepartmentId;
            }
        },
        mounted: function() {
            getDepartmentSubmitData();
        }
    })

    function getDepartmentSubmitData() {
        $.ajax({
            url: 'BudgetSubmitList.aspx',
            data: {action:'getDepartmentSubmitList'},
            type: 'post',
            dataType: 'json',
            success: function (msg) {
                vue.loading = false;
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.tableData = JSON.parse(msg.Doc);
                }
            },
            error: function (msg) {
                vue.loading = false;
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        })
    }
</script>
</html>
