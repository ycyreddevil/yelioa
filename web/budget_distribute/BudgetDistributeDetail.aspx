<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BudgetDistributeDetail.aspx.cs" Inherits="BudgetDistributeDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预算分配详情页</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
</head>
<body>
<div id="total">
    <el-container>
        <el-header>
            <el-row :gutter="20">
                <el-col :span="4">
                    <el-input v-model="totalDistributeBudget" readonly>
                        <template slot="prepend">可分配额度</template>
                    </el-input>
                    
                </el-col>
                <el-col :span="4">
                    <el-input v-model="totalDistributedBudget" readonly>
                        <template slot="prepend">已分配额度</template>
                    </el-input>
                </el-col>
                <el-col :span="4">
                    <el-button type="primary" round @click="distribute" :disabled="isFinished == 1">确定分配</el-button>
                </el-col>
            </el-row>
        </el-header>
        <el-main>
            <el-table :data="tableData" border style="width: 100%" @cell-dblclick="tableDbEdit">
                <el-table-column type="index" width="50"></el-table-column>
                <el-table-column v-for="header in tableHeaders" :label="header.label" :prop="header.prop" align="center">
                    <el-table-column v-show="header.children.length > 0" v-for="subHeader in header.children"  :label="subHeader.label" :prop="subHeader.prop" align="center">
                
                    </el-table-column>
                </el-table-column>
            </el-table>
        </el-main>
    </el-container>
</div>
</body>
</html>
<script src="/Scripts/jquery.min.js"></script>
<script src="/Scripts/vue.js"></script>
<script src="https://unpkg.com/element-ui/lib/index.js"></script>
<script>
    var vue = new Vue({
        el: '#total',
        data: {
//            tableData: [{
//                "subDepartmentId": "353",
//                "subDepartmentName": "东森家园/集团营销中心/销售部/南部战区（第三战区）/第三战区第八大区/第三战区第八大区第二十三地区",
//                "市场学术费-会议-顾问费": "100",
//                "市场学术费-会议-顾问费Id": "48",
//                "市场学术费-会议-顾问费SubmitAmount": "300",
//                "市场学术费-会议-圆桌费": "200",
//                "市场学术费-会议-圆桌费Id": "48",
//                "市场学术费-会议-圆桌费SubmitAmount": "350"
//            }],
//            tableHeaders:[{
//                "label":"部门名称",
//                "prop":"subDepartmentName",
//                "children":[],
//            },{
//                "label":"市场学术费",
//                "prop":"",
//                "children":[{
//                        "label":"市场学术费-会议-顾问费",
//                        "prop":"市场学术费-会议-顾问费",
//                    },
//                    {
//                        "label":"市场学术费-会议-圆桌费",
//                        "prop":"市场学术费-会议-圆桌费",
//                    }],
//                }],
            tableData: [],
            tableHeaders: [],
            totalDistributeBudget: 0,
            totalDistributedBudget: 0,
            budgetLimitType: '',
            firstOrSecondDistribute: 0,
            docNo: '',
            departmentId: '',
            isFinished: 0,
        },
        methods: {
            tableDbEdit: function (row, column, cell, event) {
                if (this.isFinished == 1) {
                    vue.$alert("该单据属于已提交单据，无权修改，只能查看！", '错误信息');
                    return;
                }
                if (column.label != "部门名称") {
                    let originValue = event.target.innerHTML;
                    event.target.innerHTML = "";
                    let cellInput = document.createElement("input");
                    cellInput.value = "";
                    cellInput.setAttribute("type", "text");
                    cellInput.setAttribute("id", "tempInput");
                    if (typeof row[column.label + "SubmitAmount"] != "undefined") {
                        cellInput.setAttribute("placeholder","参考值:"+ row[column.label+"SubmitAmount"]); // 第二次分配添加预算提交值为默认值 作为分配时的参考
                    }
                    cellInput.style.width = "80%";
                    cell.appendChild(cellInput);
                    document.getElementById("tempInput").focus();
                    cellInput.onblur = function () {
                        vue.totalDistributedBudget -= isNaN(parseInt(originValue)) == true ? 0 : parseInt(originValue);
                        vue.totalDistributedBudget += parseInt(cellInput.value == ""?"0":cellInput.value);

                        if (vue.totalDistributedBudget > vue.totalDistributeBudget) {
                            vue.$alert('分配额度超出可用范围，请重新填写！', '错误信息', {
                                confirmButtonText: '确定',
                                callback: action => {
                                    event.target.innerHTML = originValue;
                                    vue.totalDistributedBudget -= parseInt(cellInput.value);
                                }
                            });
                        } else {
                            if (cellInput.value == "") {
                                event.target.innerHTML = 0;
                            } else {
                                event.target.innerHTML = cellInput.value;
                            }

                            // 把修改的值绑定到tableData数据中
                            row[column.label] = cellInput.value;
                        }
                        cell.removeChild(cellInput);
                    };
                }
                
            },
            distribute: function() {
                distribute(this.firstOrSecondDistribute, this.tableData, this.budgetLimitType, this.docNo, this.departmentId);
            }
        },
        mounted: function () {
            this.totalDistributeBudget = GetQueryString("BudgetLimit");
            this.firstOrSecondDistribute = GetQueryString("firstOrSecondDistribute");
            this.docNo = GetQueryString("docNo");
            this.departmentId = GetQueryString("departmentId");
            this.isFinished = GetQueryString("IsFinished");

            var url = decodeURI(location.href);
            var tmp1 = url.split("?")[1];
            var tmp2 = tmp1.split("&")[0];
            var tmp3 = tmp2.split("=")[1];
            var BudgetLimitType = tmp3;

            this.budgetLimitType = BudgetLimitType;
            
            getDepartmentDistributeDate(this.budgetLimitType, this.departmentId,this.isFinished);
        }
    });

    function getDepartmentDistributeDate(budgetLimitType, departmentId,isFinished) {
        $.ajax({
            url: 'BudgetDistributeDetail.aspx',
            data: { action: 'getDepartmentDistributeDetail', BudgetLimitType: budgetLimitType, departmentId: departmentId ,isFinished:isFinished},
            type: 'post',
            dataType: 'json',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.tableData = msg.Doc;
                    vue.tableHeaders = msg.Headers;
                }
            },
            error: function(msg) {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        });
    }

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    function distribute(firstOrSecondDistribute, tableData, budgetLimitType, docNo, departmentId) {
        var array = new Array();
        $.each(tableData, function(i, v) {
            array.push(JSON.parse(JSON.stringify(v)));
        });

        $.ajax({
            url:'BudgetDistributeDetail.aspx',
            data: {
                action: 'distributeBudget',
                tableData: JSON.stringify(array),
                firstOrSecondDistribute: firstOrSecondDistribute,
                BudgetLimitType: budgetLimitType,
                docNo: docNo,
            },
            dataType: 'json',
            type: 'post',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.$alert('分配成功！', '提示', {
                        confirmButtonText: '确定',
                        callback: action => {
                            location.href = "BudgetDistributeList.aspx";
                        }
                    });
                }
            },
            error: function() {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        })
    }
</script>
