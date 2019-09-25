<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReimburseProcessSetting.aspx.cs" Inherits="ReimburseProcessSetting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>移动报销流程设计</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/base-loading.js"></script>
    <style>
         .panel-body.layout-body>div>.textbox{ border:none;}
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'west',split:true,collapsible:false,iconCls:'icon-add',iconAlign:'left'" title="表单列表" style="width: 300px;">
        <%--<div id="tb2">
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-save" onclick="save()">保存</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-undo" onclick="cancel()">取消</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-add" onclick="add()">新增</a>
            <%--<a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-remove" onclick="deleteForm()">删除</a>--%>
        <%--</div>--%>
        <div id="datagrid" class="easyui-datagrid"></div>--%>
    </div>
	<div data-options="region:'center',split:true,title:'自定义表单流程设计',collapsible:false" style="width: 300px;">
	    <form id="ff" method="post">
	    	<table cellpadding="8">
	    		<tr>
	    			<td>流程名称:</td>
	    			<td><input class="easyui-textbox" type="text" name="name" data-options="required:true"></input></td>
	    		</tr>
	    		<%--<tr>
	    			<td>可见范围:</td>
	    			<td><a onclick="updateVisible()" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add'">新增</a></td>
                <td><a href="#" class="easyui-linkbutton">余昌运1</a></td>
                    <td><a href="#" class="easyui-linkbutton">余昌运2</a></td>
                    <td><a href="#" class="easyui-linkbutton">余昌运3</a></td>
	    		</tr>--%>
	    		<tr>
	    			<td>默认审批人:</td>
	    			<td><a onclick="updateApprover()" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add'">新增</a></td>
	    		</tr>
	    	</table>
	    </form>
	    <div>
	    	<a href="javascript:void(0)" class="easyui-linkbutton" onclick="submitForm()">保存</a>
	    </div>
	</div>
    <div id="dlg-chooseApprover" class="easyui-dialog easyui-layout" title="选择审批人" data-options="iconCls:'icon-leaveStock',buttons:'#button',modal:true,closed:true,width:600">
        <div style="margin-top:3%"><input type="radio" name="radio1" id="leader" value="上级"/>上级（ 自动设置通讯录中的上级领导为审批人 ）</div>
        <div style="margin-top:3%"><input type="radio" name="radio1" id="other" value="单个成员"/>单个成员</div>
        <div class="easyui-layout" style="height:300px;" id="treeDiv">
            <div data-options="region:'west',title:'通讯录'" style="width:180px">
                <ul id="tree" class="easyui-tree" data-options="checkbox:true,cascadeCheck:false"></ul>
                <%--<ul id="tree2" class="easyui-tree" data-options="checkbox:true,cascadeCheck:false"></ul>--%>
            </div>
            <div id="chooseMember" data-options="region:'center',title:'已选择的部门、成员'" >
                <%--<div><input class="easyui-textbox" data-options="value:'ycy',iconCls:'icon-man',iconAlign:'left',editable:'false'"/></div>--%>
            </div>
        </div>
        <div id="button">
             <a style="margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="certainApprover();">确定</a>
             <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog('dlg-chooseApprover');">取消</a>
        </div>
    </div>
</body>
<script>
    var approverIds = new Array();
    function updateApprover() {
        $("#tree").show();
        $("#tree2").hide();
        $('#dlg-chooseApprover').dialog("open");
        $('#dlg-chooseApprover').window('center')
        //$("#other").trigger("click");
        //TreeLoad();
        $("#chooseMember").empty();
    }
    //function updateVisible() {
    //    $("#tree").hide();
    //    $("#tree2").show();
    //    $('#dlg-chooseApprover').dialog("open");
    //    $('#dlg-chooseApprover').window('center');

    //    $("#chooseMember").empty();
    //}
    function closeDialog(dlg) {
        $('#'+dlg).dialog("close");
    }
    $(function () {
        $("input[type=radio][name='radio1']").change(function () {
            if (this.value == '单个成员') {
               $("#treeDiv").css("display","block");
                TreeLoad();
            } else {
                $("#tree").empty();
                $("#treeDiv").css("display", "none");
                $(".window-shadow").css("display", "none");
            }
        })

        $("#tree").tree({
            onCheck: function (node) {
                // 只能选人员 不能选部门
                if (!$('#tree').tree('isLeaf', node.target)){
                    return;
                }
                //实现单选
                var cknodes = $('#tree').tree("getChecked");
                for (var i = 0; i < cknodes.length; i++) {
                    if (cknodes[i].id != node.id) {
                        $('#tree').tree("uncheck", cknodes[i].target);
                    } 
                }

                var name = node.text;
                var iconCls = node.iconCls;
                var nodeId = node.id;
                // 把选中的节点添加到右侧
                if (node.checked) {
                    var html = '<div><input class="easyui-textbox"><input type="hidden" class="nodeId"/><input class="easyui-textbox"></div>';
                    $("#chooseMember").append(html);
                    if (iconCls == null) {
                        $("#chooseMember div:last input:first").textbox({
                            value: name,
                            iconAlign: 'left',
                            editable: false,
                            iconCls: 'icon-treeFolder',
                        })
                    } else {
                        $("#chooseMember div:last input:first").textbox({
                            value: name,
                            iconAlign: 'left',
                            editable: false,
                            iconCls: 'icon-man'
                        })
                    }
                    $("#chooseMember div .nodeId:last").val(nodeId);

                    $("#chooseMember div:last input:last").textbox({
                        iconAlign: 'left',
                        editable: false,
                        icons: [{
                            iconCls: 'icon-clear',
                            handler: function (e) {
                                var nodeId = $(e.data.target).prev().val();
                                var node = $('#tree').tree('find', nodeId);
                                $('#tree').tree('uncheck', node.target)
                            }
                        }]
                    })
                }else{
                    // 找到对应的div删除
                    var divs = $("#chooseMember").find("div");
                    divs.each(function (i, e) {
                        if (nodeId == $(e).children("input").eq(1).val()) {
                            $(e).remove();
                        }
                    });
                }
            },
            //onLoadSuccess: function () {
            //    var rootNodes = $("#tree").tree('getRoots');
            //    for (var i = 0; i < rootNodes.length; i++) {
            //        var node = $("#tree").tree('find', rootNodes[i].id);
            //        $("#tree").tree('uncheck', node.target);
            //    }
            //}
        })

        //$("#tree2").tree({
        //    onCheck: function (node) {
        //        // 只能选人员 不能选部门
        //        //if (!$('#tree').tree('isLeaf', node.target)) {
        //        //    return;
        //        //}
        //        //实现单选
        //        //var cknodes = $('#tree').tree("getChecked");
        //        //for (var i = 0; i < cknodes.length; i++) {
        //        //    if (cknodes[i].id != node.id) {
        //        //        $('#tree').tree("uncheck", cknodes[i].target);
        //        //    }
        //        //}

        //        var name = node.text;
        //        var iconCls = node.iconCls;
        //        var nodeId = node.id;
        //        // 把选中的节点添加到右侧
        //        if (node.checked) {
        //            var html = '<div><input class="easyui-textbox"><input type="hidden" class="nodeId"/><input class="easyui-textbox"></div>';
        //            $("#chooseMember").append(html);
        //            if (iconCls == null) {
        //                $("#chooseMember div:last input:first").textbox({
        //                    value: name,
        //                    iconAlign: 'left',
        //                    editable: false,
        //                    iconCls: 'icon-treeFolder',
        //                })
        //            } else {
        //                $("#chooseMember div:last input:first").textbox({
        //                    value: name,
        //                    iconAlign: 'left',
        //                    editable: false,
        //                    iconCls: 'icon-man'
        //                })
        //            }
        //            $("#chooseMember div .nodeId:last").val(nodeId);

        //            $("#chooseMember div:last input:last").textbox({
        //                iconAlign: 'left',
        //                editable: false,
        //                icons: [{
        //                    iconCls: 'icon-clear',
        //                    handler: function (e) {
        //                        var nodeId = $(e.data.target).prev().val();
        //                        var node = $('#tree').tree('find', nodeId);
        //                        $('#tree').tree('uncheck', node.target)
        //                    }
        //                }]
        //            })
        //        } else {
        //            // 找到对应的div删除
        //            var divs = $("#chooseMember").find("div");
        //            divs.each(function (i, e) {
        //                if (nodeId == $(e).children("input").eq(1).val()) {
        //                    $(e).remove();
        //                }
        //            });
        //        }
        //    },
        //    //onLoadSuccess: function () {
        //    //    var rootNodes = $("#tree").tree('getRoots');
        //    //    for (var i = 0; i < rootNodes.length; i++) {
        //    //        var node = $("#tree").tree('find', rootNodes[i].id);
        //    //        $("#tree").tree('uncheck', node.target);
        //    //    }
        //    //}
        //})
    })
    function TreeLoad(selectedId) {
        var url = "ReimburseProcessSetting.aspx";
        var data = {
            act: 'getTree'
        };
        parent.Loading(true);
        $.post(url, data, function (res) {
            parent.Loading(false);
            if (res != "F") {
                TreeDataJson = res;
                var datasource = $.parseJSON(res);
                
                $('#tree').tree("loadData", datasource);
                company = $('#tree').tree('getRoot').text;
                if (selectedId == null) {
                    selectedId = $('#tree').tree('getRoot').id;
                    SelectedNode = $('#tree').tree('getRoot');
                }
                //dg_Load(selectedId);
            }
        });
    }
    function certainApprover() {
        approverIds = new Array();
        var divs = $("#chooseMember").find("div");
        var html = "";
        if (divs.length == 0) {
            html += '<td><a href="#" class="easyui-linkbutton">上级</a></td>';
            approverIds.push("");
        } else {
            divs.each(function (i, e) {
                var approverId = $(e).children("input").eq(1).val();
                approverIds.push(approverId);

                var approverName = $(e).children("input").eq(0).textbox("getValue");
                html += '<td><a href="#" class="easyui-linkbutton">' + approverName + '</a></td>';
            });
        }
        if ($("#tree").is(":hidden")) {
            $("#ff").children().children().children("tr").eq(1).children("td:last").after(html);
        } else {
            $("#ff").children().children().children("tr:last").children("td:last").after(html);
        }
        
        $.parser.parse();
        $('#dlg-chooseApprover').dialog("close");
    }


</script>
</html>
