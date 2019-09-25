<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BudgetManag.aspx.cs" Inherits="BudgetManag" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/base-loading.js"></script>
    <script src="Scripts/pcCommon.js"></script>
</head>

<body class="easyui-layout">
    <div id="loading"
        style="background-position: center center; width: 110px; height: 110px; background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;"
        class="easyui-dialog" border="false" noheader="true" closed="true" modal="true">
    </div>
    <div data-options="region:'west',split:true,title:'部门列表',collapsible:false" style="width: 500px;">
        <div id="departmentTree" class="tree"></div>
    </div>
    <div data-options="region:'center',split:true,collapsible:false" title="费用明细及其预算列表"
        style="width: 300px;height:auto">
        <div id="tb">
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="appendFirst()">增加一级明细</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="appendSecond()">增加二级明细</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="deleteRow()">删除</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="complete()">完成</a>
            <a href="javascript:void(0)" class="easyui-linkbutton"
                onclick="$('#dlg-budget-Import').dialog('open');">导入Excel</a>
            <span id="departmentId"></span>
        </div>
        <table id="tg" class="easyui-treegrid" title="费用预算详细信息" style="width:700px;height:500px" data-options="
				rownumbers: true,
				animate: true,
				collapsible: true,
				fitColumns: true,
				idField: 'Id',
				treeField: 'FeeDetail',
				onDblClickRow:onDblClickRow,
                onClickRow:onClickRow,
                singleSelect:true,
                toolbar:'#tb'
			">
            <thead>
                <tr>
                    <th data-options="field:'Id', hidden:true,width:60,align:'center',editor:'numberbox'"></th>
                    <th data-options="field:'FeeDetail',width:180,align:'left',editor:'textbox'">费用明细</th>
                    <th data-options="field:'Budget',width:80,align:'center',editor:'numberbox'">预算</th>
                    <th data-options="field:'UsedAmount',width:80,align:'center',editor:'numberbox'">已用</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="dlg-budget-Import" class="easyui-dialog" title="预算导入" style="width: 800px; height: 80px"
        data-options="iconCls:'icon-import',modal:true,closed:true">
        <form id="fmFile" method="post" enctype="multipart/form-data">
            <input id="fbx" class="easyui-filebox" label="预算文件:" labelposition="left"
                data-options="prompt:'请选择一个xls文件...'" style="width: 50%" buttontext="请选择文件"
                accept='application/vnd.ms-excel' name="file">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <input type="hidden" name="act" id="actFbx" />
            <input type="hidden" name="departmentId" id="departmentIdFbx" />
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="ImportExcel()">提交</a>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <a href="javascript:void(0)" onclick="GetTemplate()">下载预算导入模板</a>
        </form>
    </div>
    <script type="text/javascript">

        var editingId;
        var count = 0;
        function onDblClickRow(row) {
            $('#tg').treegrid('select', row.Id);
            endEdit();
            editingId = row.Id;
            $('#tg').treegrid('beginEdit', editingId);
        }
        function onClickRow(row) {
            $('#tg').treegrid('select', row.Id);
            endEdit();
        }
        function appendFirst() {
            $('#tg').treegrid('append', {
                data: [{
                    Id: count,
                    FeeDetail: '费用明细' + count,
                    Budget: 0,
                    ParentId: -1,
                    children: []
                }]
            });
            count++;
        }
        function appendSecond() {
            var row = $('#tg').treegrid('getSelected');
            if (row == null || row == undefined) {
                $.messager.alert('提示', '请先选中要创建二级明细上的一级明细！', 'info');
            } else if (row.ParentId != -1) {
                $.messager.alert('提示', '请先选中一级明细,而不是二级明细！', 'info');
            }
            else {
                $('#tg').treegrid('append', {
                    parent: row.Id,
                    data: [{
                        Id: count,
                        FeeDetail: '费用明细' + count,
                        Budget: 0,
                        ParentId: row.Id,
                        children: []
                    }]
                });
                count++;
            }
        }
        function deleteRow() {
            var row = $('#tg').treegrid('getSelected');
            $('#tg').treegrid('remove', row.Id);
        }


        var url = "BudgetManag.aspx";
        var departmentId = "";
        $(document).ready(function () {
            initDepartmentTree();
            getDepartmentTree();
        });


        function initDepartmentTree() {
            $('#departmentTree').tree({
                animate: true, lines: true,
                formatter: function (node) {
                    var text = node.text;
                    return text.substr(text.lastIndexOf('/') + 1, text.length - text.lastIndexOf('/') - 1);
                },
                onClick: function (node) {
                    departmentId = node.id;
                    $("#departmentId").html('部门ID=' + departmentId);
                    $("#search").textbox("setValue", node.target.textContent);
                    getBudget();
                }
            });
        }

        function getBudget() {
            parent.Loading(true);
            $.post(url, { act: 'getBudget', departmentId: departmentId },
                function (res) {
                    var datasource = JSON.parse(res);
                    parent.Loading(false);
                    if (datasource.message == "success") {

                        $('#tg').treegrid('loadData', JSON.parse(datasource.data));
                        count = datasource.count + 1;
                    }
                    else
                        $.messager.alert('提示', '本月本部门预算获取失败，请重新获取！', 'info');
                });
        }
        function getDepartmentTree() {
            parent.Loading(true);
            $.post(url, { act: 'initDepartmentTree' },
                function (res) {
                    if (res != "") {
                        data = JSON.parse(res);
                        $('#departmentTree').tree('loadData', data);
                    }
                    parent.Loading(false);
                });
        }


        function complete() {
            endEdit();
            $.post(url, { act: "updateBudget", departmentId: departmentId, budgetList: JSON.stringify($('#tg').treegrid('getRoots')) },
                function (res) {
                    if (res.indexOf("操作成功")) {
                        $.messager.alert('提示', res, 'info');
                    }
                    else
                        $.messager.alert('提示', '操作失败,请重新操作！' + res, 'info');
                    parent.Loading(false);
                });
        }
        function endEdit() {
            if (editingId != undefined) {
                var node = $('#tg').treegrid('find', editingId);
                $('#tg').treegrid('endEdit', editingId);
                editingId = undefined;
                if (node.children != null && node.children.length > 0) {
                    updateRow(node);
                } else if (node.ParentId != -1) {
                    var parentNode = $('#tg').treegrid('find', node.ParentId);
                    updateRow(parentNode);
                }
            }
        }
        function updateRow(node) {
            var budget = 0;
            for (var i = 0; i < node.children.length; i++) {
                budget += parseInt(node.children[i].Budget);
            }
            node.Budget = budget;
            $('#tg').treegrid('update', {
                id: node.Id,
                row: node
            });
        }

        function GetTemplate() {
            // parent.Loading(true);
            // $.post(url, { act: "getTemplate" }, function (res) {
            //     parent.Loading(false);
            //     var disp = request.getResponseHeader('Content-Disposition');
            //     if (disp && disp.search('attachment') != -1) {  //判断是否为文件
            //         var form = $('<form method="POST" action="' + url + '">');
            //         $.each(params, function (k, v) {
            //             form.append($('<input type="hidden" name="' + k +
            //                 '" value="' + v + '">'));
            //         });
            //         $('body').append(form);
            //         form.submit(); //自动提交
            //     }
            // });
            window.location.href = url + '?act=getTemplate';
        }

        /*===================下载文件
        * options:{
        * url:'',  //下载地址
        * data:{name:value}, //要发送的数据
        * method:'post'
        * }
        * DownLoad({

        url:'http://www.baidu.com.....', //请求的url

        data:{sc:'xxx'}//要发送的数据

        });
        */
        var DownLoadFile = function (options) {
            var config = $.extend(true, { method: 'post' }, options);
            var $iframe = $('<iframe id="down-file-iframe" />');
            var $form = $('<form target="down-file-iframe" method="' + config.method + '" />');
            $form.attr('action', config.url);
            for (var key in config.data) {
                $form.append('<input type="hidden" name="' + key + '" value="' + config.data[key] + '" />');
            }
            $iframe.append($form);
            $(document.body).append($iframe);
            $form[0].submit();
            $iframe.remove();
        }

        function ImportExcel() {
            var fileName = $('#fbx').filebox('getValue');
            if (fileName == "") {
                return;
            }
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            } else {
                $('#actFbx').val('import');
                $('#departmentIdFbx').val(departmentId);
                parent.Loading(true);
                $('#fmFile').form('submit', {
                    url: url,
                    success: function (res) {
                        parent.Loading(false);
                        $.messager.alert('提示', res);
                    }
                });
            }
        }
    </script>
</body>

</html>