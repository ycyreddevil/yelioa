<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FormProcess.aspx.cs" Inherits="FormProcess" %>

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
</head>
<body class="easyui-layout">
    <div data-options="region:'west',split:true,collapsible:false,iconCls:'icon-add',iconAlign:'left'" title="表单列表" style="width: 300px;">
        <div id="datagrid" class="easyui-datagrid"></div>
    </div>
	<div data-options="region:'center',split:true,title:'流程列表',collapsible:false" style="width: 150px;">
	    <%--<div>
	        <div style="margin: 10px 0px 10px 0px; text-align: center;">
	            <a href="javascript:void(0)" class="easyui-linkbutton c8" onclick="dlgDepartOpen('add');"
	               data-options="iconCls:'icon-add'">新增流程</a>               
	            <a href="javascript:void(0)" class="easyui-linkbutton c8" onclick="dlgDepartOpen('edit');"
	               data-options="iconCls:'icon-edit'">编辑流程</a>
	        </div> 
	    </div>
	    <hr />--%>

	    <ul id="conditionItemTree" class="easyui-tree"></ul>

	    <%--<form id="ff" method="post">
	    	<table cellpadding="8">
	    		<tr>
	    			<td>流程名称:</td>
	    			<td><input id="processName" class="easyui-textbox" type="text" name="name" data-options="required:true"></input></td>
	    		</tr>
	    		<tr>
	    			<td>默认审批流程:</td>
	    			<td><a onclick="updateApprover(1)" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add'">新增</a></td>
	    		</tr>
                <tr>
	    			<td>条件审批流程:</td>
	    			<td><a onclick="updateCondApprover(2)" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add'">选择审批条件</a></td>
	    		</tr>
                
	    	</table>
	    </form>
	    <div>
	    	<a href="javascript:void(0)" class="easyui-linkbutton" onclick="submitProcess()">保存</a>
	    </div>--%>
	</div>
    <div id="processDetailDiv" data-options="region:'east',split:true,title:'流程详情',collapsible:false" style="width: 1000px;">
        <a onclick="updateApprover(1)" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add',size:'large'" style="margin-left: 40%; margin-top: 30%">请设计审批流程</a>
        <hr />
        <div>
            条件流程名称:<input id="processName" class="easyui-textbox" style="width: 200px" />
        </div>
        <div>
            <input id="type" name="type" value="aa" style="width:200px;">
           <%-- <select class="easyui-combobox" name="type" id="type" style="width:200px;" label="条件流程字段:">
            </select>--%>
        </div>
        <div>
            条件流程条件:<input id="processName" class="easyui-textbox" style="width: 110px" />
        </div>
    </div>
    <div id="dlg-chooseApprover" class="easyui-dialog easyui-layout" title="选择审批人" data-options="iconCls:'icon-leaveStock',buttons:'#button1',modal:true,closed:true,width:600">
        <div style="margin-top:3%"><input type="radio" name="radio1" id="leader" value="上级"/>上级（ 自动设置通讯录中的上级领导为审批人 ）</div>
        <div style="margin-top:3%"><input type="radio" name="radio1" id="other" value="单个成员"/>单个成员</div>
        <div style="margin-top:3%"><input type="radio" name="radio1" id="department" value="部门负责人"/>部门负责人</div>
        <div style="margin-top:3%"><input type="radio" name="radio1" id="leaderDepartment" value="单个部门"/>选中部门的负责人处理完成时，结束逐级审批</div>
        <div style="margin-top:3%"><input type="radio" name="radio1" id="associate" value="关联字段"/>设置关联字段<select id="associateItem"><option>请选择</option></select></div>
        <div class="easyui-layout" style="height:300px;" id="treeDiv">
            <div data-options="region:'west',title:'通讯录'" style="width:180px">
                <ul id="tree" class="easyui-tree" data-options="checkbox:true,cascadeCheck:false"></ul>
            </div>
            <div id="chooseMember" data-options="region:'center',title:'已选择的部门、成员'">
                
            </div>
        </div>
        <div id="button1">
             <a style="margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="certainApprover();">确定</a>
             <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog('dlg-chooseApprover');">取消</a>
        </div>
    </div>
    <div id="dlg-chooseCondition" class="easyui-dialog easyui-layout" title="选择条件" data-options="iconCls:'icon-leaveStock',buttons:'#button2',modal:true,closed:true,width:600">
        <div id="condition">
            <div style="margin-top:3%">条件字段:<select id="condField"></select></div>
            <div style="margin-top:3%" id="conditionValue">
            </div>
        </div>
        <div class="easyui-layout" style="height:300px;" id="relativeDiv">
            <div data-options="region:'center',title:'已选择的条件值'">
                <ul id="relativeField"></ul>
            </div>
        </div>
        <div id="button2">
             <a style="margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="certainCondition();">确定</a>
             <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog('dlg-chooseCondition');">取消</a>
        </div>
    </div>
</body>
<script>
    var approverIds = new Array();
    var chooseFormId;
    var dialogIndex = 0;
    var DefaultProcessJson = new Array();
    var ConditionItem = new Array();
    var ConditionJson = new Array();

    function ProcessTreeLoad(selectedId) {
        var url = "FormProcess.aspx";
        var data = {
            act: 'getProcessTree',
            formId: selectedId,
        };
        parent.Loading(true);
        $.post(url, data, function (res) {
            parent.Loading(false);
            if (res != "F") {
                TreeDataJson = res;
                var datasource = $.parseJSON($.parseJSON(res).tree);
                $('#conditionItemTree').tree("loadData", datasource);
                company = $('#conditionItemTree').tree('getRoot').text;
                if (selectedId == null) {
                    selectedId = $('#conditionItemTree').tree('getRoot').id;
                    SelectedNode = $('#conditionItemTree').tree('getRoot');
                }
                //dg_Load(selectedId);
                showConditionProcess(selectedId);
            }
        });
    }

    function InitTree() {
        $('#conditionItemTree').tree({
            animate: true, lines: false,
            onClick: function (node) {
                //$(this).tree('beginEdit', node.target);//点击可编辑
                SelectedNode = node;
                showConditionProcess(node.id);
            },
            formatter: function (node) {
                var s = node.text;
                // s += '&nbsp;<span style=\'color:blue\'>(' + node.MemberNumber + ')</span>';
                return s;
            }
        });
    }

    function showConditionProcess(selectedId) {

    }

    function updateApprover(index) {
        removeDefaultOrCondition = 0;

        dialogIndex = index;
        //$("#condition").css("display", "none");
        $("#tree").show();
        //$("#tree2").hide();
        $('#dlg-chooseApprover').dialog("open");
        $('#dlg-chooseApprover').window('center')
        $("#treeDiv").css("display", "none");
        //$("#relativeDiv").css("display", "none");
        //$("#other").trigger("click");
        //TreeLoad();
        $("#chooseMember").empty();
        $(".window-shadow").css("display", "none");
    }
    function updateCondApprover(index) {
        if ($("#ff").children().children().children("tr").eq(1).find("td").length <= 2) {
            alert("请先设置默认流程");
            return;
        }
        removeDefaultOrCondition = 1;

        dialogIndex = index;
        //$("#condition").css("display", "block");
        $("#tree").show();
        //$("#tree2").hide();
        $('#dlg-chooseCondition').dialog("open");
        $('#dlg-chooseCondition').window('center')
        //$("#treeDiv").css("display", "none");
        $("#relativeDiv").css("display", "none");
        //$("#other").trigger("click");
        //TreeLoad();
        $("#chooseMember").empty();
        $(".window-shadow").css("display", "none");

        // 把该表单的相关字段带出
        showFormFiled();
    }

    function showFormFiled() {
        $.ajax({
            url: 'FormProcess.aspx',
            dataType: 'json',
            type: 'post',
            data: {act:'showFormFiled', formId: chooseFormId},
            success: function (data) {
                $("#condField").empty()
                var field = JSON.parse(data.data);
                var option = '<option value="">请选择选中表单中的字段</option>';
                $.each(field, function (i, v) {
                    option += "<option json='" + JSON.stringify(v.json) + "' value='" + v.type + "'>" + v.name + "</option>";
                })
                $("#condField").append(option);
            }
        })
    }

    $("#condField").change(function () {
        // 当表单字段发生改变时，同时改变字段对应的值
        var type = $("#condField").val();
        var html = "";
        $("#conditionValue").empty();
        $("#relativeDiv").css("display", "none");
        if (type == 'name') {
            var html = '';
            var json = $('#condField>option:selected').attr("json");
            json = JSON.parse(json);
            var tableNM = json.RELA1.TABLENM;
            html += '<button type="button" onClick="showRelativeField(\''+tableNM+'\')">查询</button>';
        } else if (type == 'number') {
            var html = '<select id="number">' +
                '<option>小于</option><option>小于等于</option><option>大于</option><option>大于等于</option>' +
                '<option>大于小于</option><option>大于等于小于等于</option><option>大于小于等于</option><option>大于等于小于</option>' +
                '</select><input type="number" value="" id="minNumber"/>--<input type="number" value="" id="maxNumber"/>';
        } else {
            var html = '';
            var json = $('#condField>option:selected').attr("json");
            json = JSON.parse(json);
            for (i = 0; i < json.length; i++) {
                html += "<input type='checkbox' name='jsonCheckbox' value='"+ json[i].VAL +"'>" + json[i].VAL + "</input>";
            }
        }
        $("#conditionValue").append("条件数据"+html);
    })

    function showRelativeField(type) {
        try {
            $("#relativeField").empty();
        }catch(err){

        }
        // 把关联的数据展示出来 并且搜索
        $.ajax({
            url: 'FormProcess.aspx',
            data: {act:'findRemoteData', type:type},
            dataType: 'json',
            type:'post',
            success: function (data) {
                $("#relativeDiv").css("display", "block");
                for (i = 0; i < data.length; i++) {
                    var html = '<li>' + data[i].target + '</li>';
                    $("#relativeField").append(html);
                }
                $("#relativeField").datalist({
                    checkbox: false,
                    fit: false,
                    lines: true,
                    border: false,
                    singleSelect: false,
                    height:300
                })
            },
            error: function () {

            }
        });
    }

    function closeDialog(dlg) {
        $('#'+dlg).dialog("close");
    }
    $(function () {
        $("input[type=radio][name='radio1']").change(function () {
            if (this.value == '单个成员') {
               $("#treeDiv").css("display","block");
                TreeLoad('getTree');
            } else if (this.value == '单个部门' || this.value == '部门负责人') {
                initDepartmentTree();
                $("#treeDiv").css("display","block");
                TreeLoad('getDepartmentTree');
            }else {
                $("#tree").empty();
                $("#treeDiv").css("display", "none");
                $(".window-shadow").css("display", "none");
            }
        })

        initUserTree();
        initFormList();
    })

    function initFormList() {
        $('#datagrid').datagrid({
            fitColumns: true,
            striped: true,
            singleSelect: true,
            columns: [[
                { field: 'Id', width: 20 },
                { field: 'FormName', title: '表单名', width: 60 },
                {
                    field: 'Valid', title: '是否启用', width: 60,
                    editor: {
                        type: 'combobox',
                        options: {
                            data: [{value:0,text:"是"},{value:1,text:"否"}],
                            valueField: "value",
                            textField: "text",
                            editable: false
                        }
                    },
                    formatter: function (value, row, index) {
                        if (value == 0) {
                            return "启用";
                        } else {
                            return "禁用";
                        }
                    }
                },
                {
                    field: 'Type', title: '类型', width: 80,
                    editor: {
                        type: 'combobox',
                        options: {
                            data: [{value:"人事",text:"人事"},{value:"财务",text:"财务"},{value:"运营",text:"运营"},{value:"行政",text:"行政"},{value:"其他",text:"其他"}],
                            valueField: "value",
                            textField: "text",
                            editable: false
                        }
                    },
                },
            ]],
            onClickRow: function (rowIndex, rowData) {
                //getHasRight(rowData.Id);
                // 点击表单
                $("#processName").textbox("setValue", rowData.FormName + "审批流程");

                chooseFormId = rowData.Id;

                // 展示某一个表单对应的流程树
                ProcessTreeLoad(chooseFormId);
                InitTree();

                // 选择的流程回显
                //showProcessData(chooseFormId);

                // 把相关的关联字段全部显示到select中
//                var parameterData = JSON.parse(rowData.ParameterData);
//                var html = "";
//                $.each(parameterData, function (index, value) {
//                    if (parameterData[index].TYP == 'name') {
//                        html += "<option>"+parameterData[index].LBL+"</option>"
//                    }
//                })
//                $("#associateItem").append(html);
            }
        });
        parent.Loading(true);
        $.post('FormProcess.aspx', { act: 'initDatagrid'} ,
            function (res) {
                if (res != "") {
                    var datasource = JSON.parse(res);

                    if (datasource.ErrCode != 0) {
                        $.messager.alert('提示', '暂无表单！', 'info');
                    } else {
                        $('#datagrid').datagrid('loadData', JSON.parse(datasource.data));
                    }
                }
                else
                    $.messager.alert('提示', '表单列表获取失败，请重新获取！', 'info');
                parent.Loading(false);
            }
        );
    }

    function showProcessData(formId) {
        $.ajax({
            url: 'FormProcess.aspx',
            data: { act: 'showProcessData', formId: formId },
            dataType: 'json',
            type: 'post',
            success: function (data) {
                $("#ff").children().children().children("tr").eq(1).html('<td>默认审批流程:</td><td><a onclick="updateApprover(1)" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-add\'">新增</a></td>');

                var processArray = JSON.parse(data.process);

                // 第一条流程一定是默认流程
                var defaultProcess = JSON.parse(processArray[0].DefaultProcessJson);

                $.each(defaultProcess, function (i, v) {
                    var html = ""; var data = "";
                    if (v.type == "OneSuperior") {
                        html = '<td><a href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">上级</a> <span>······</span></td>';
                        data = { "type": "OneSuperior", "mode": "And" };
                    } else if (v.type == "SuperiorUntil" || v.type == "Department") {
                        html = '<td><a href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">' + v.name + '</a> <span>······</span></td>';
                        data = { "type": "SuperiorUntil", "departmentId": v.departmentId, "mode": "And" };
                    } else if (v.type == "User") {
                        html = '<td><a href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">'+v.name+'</a> <span>······</span></td>';
                        data = { "type": "User", "userId": v.userId };
                    } else if (v.type == "Association") {
                        html = '<td><a href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">'+v.AssociatedFields+'</a> <span>······</span></td>';
                        data = { "type": "Association", "AssociatedFields":v.AssociatedFields, "mode": "And" };
                    }

                    DefaultProcessJson.push(data);
                    ConditionItem.push("");
                    ConditionJson.push("");

                    $("#ff").children().children().children("tr").eq(1).children("td:last").after(html);
                    $.parser.parse();
                });

                // 再加载条件审批流程
                if (processArray.length > 1) {
                    for (i = 1; i < processArray.length; i++) {
                        var conditionItem = processArray[i].ConditionItem;
                        var html = '<tr><td><a onclick="addConditionApprover(this)" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-add\'">对应流程</a></td>';
                        html += '<td>' + conditionItem + ':';

                        var conditionJson = JSON.parse(processArray[i].ConditionJson);
                        var conditionSelectItem = conditionJson.selected[0][conditionItem];

                        html += '<a href="#" onclick="removeThisCondition(this)" class="easyui-linkbutton" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">' + conditionSelectItem + '</a></td></tr>';
                        $("#ff").children().children().children("tr:last").after(html);

                        ConditionItem.push(conditionItem);
                        ConditionJson.push(conditionJson);

                        // 加载条件审批流程对应的条件
                        var conditionProcess = JSON.parse(processArray[i].DefaultProcessJson);

                        $.each(conditionProcess, function (index, v) {
                            var html = ""; var data = "";
                            if (v.type == "OneSuperior") {
                                html = '<td><a href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">上级</a> <span>······</span></td>';
                                data = { "type": "OneSuperior", "mode": "And" };
                            } else if (v.type == "SuperiorUntil" || v.type == "Department") {
                                html = '<td><a href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">' + v.name + '</a> <span>······</span></td>';
                                data = { "type": "SuperiorUntil", "departmentId": v.departmentId, "mode": "And" };
                            } else if (v.type == "User") {
                                html = '<td><a href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">'+v.name+'</a> <span>······</span></td>';
                                data = { "type": "User", "userId": v.userId };
                            } else if (v.type == "Association") {
                                html = '<td><a href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">'+v.AssociatedFields+'</a> <span>······</span></td>';
                                data = { "type": "Association", "AssociatedFields":v.AssociatedFields, "mode": "And" };
                            }

                            DefaultProcessJson.push(data);
                            ConditionItem.push("");
                            ConditionJson.push("");

                            $("#ff").children().children().children("tr").eq(2+i).children("td:last").after(html);
                            $.parser.parse();
                        });
       
                        $.parser.parse();

                    }
                }
            },
            error: function () {

            }
        })
    }

    function initUserTree() {
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
        })
    }

    function initDepartmentTree() {
        $("#tree").tree({
            onCheck: function (node) {
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
        })
    }

    function TreeLoad(act, selectedId) {
        var url = "FormProcess.aspx";
        var data = {
            act: act
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

        var divs = $("#chooseMember").find("div");
        var data;
        var html = "";
        if (divs.length == 0) {
            var associateInput = $("#dlg-chooseApprover").find("input:last:checked");
            var associateItem = $("#associateItem option:selected").text();
            if (associateInput.length > 0) {
                html += '<a style="margin-left: 40%; margin-top: 30%" href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'large\'">'+associateItem+'</a> <span>······</span>';
                data = { "type": "Association", "AssociatedFields":associateItem, "mode": "And" };
            } else {
                html += '<a style="margin-left: 40%; margin-top: 30%" href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'large\'">上级</a> <span>······</span>';
                data = { "type": "OneSuperior", "mode": "And" };
            }
        } else {
            divs.each(function (i, e) {
                //var approverId = $(e).children("input").eq(1).val();
                //approverIds.push(approverId);
                var id = $(e).children("input").eq(1).val();

                var approverName = $(e).children("input").eq(0).textbox("getValue");
                html += '<a style="margin-left: 40%; margin-top: 30%" href="#" class="easyui-linkbutton" onclick="removeThisApprover(this)" data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'large\'">' + approverName + '</a> <span>······</span>';
                if (id.length > 3) {
                    data = { "type": "User", "userId": id };
                } else {
                    if ($("input[name='radio1']:checked").val() == "部门负责人") {
                        data = { "type": "Department","departmentId": id,"mode": "And" };
                    } else {
                        data = { "type": "SuperiorUntil","departmentId": id,"mode": "And" };
                    }
                }
            });
        }

        $("#processDetailDiv").html(html);
        
        //$("#ff").children().children().children("tr").eq(dialogIndex).children("td:last").after(html);

        if (dialogIndex == 1) {
            DefaultProcessJson.push(data);
            
        } else {
            DefaultProcessJson.push(data);
        }
        ConditionItem.push("");
        ConditionJson.push("");
        
        $.parser.parse();
        
        $('#dlg-chooseApprover').dialog("close");
    }

    function certainCondition() {
        var data; var _conditionJson;
        var conditionArray = new Array();
         // 再添加选择对应流程的按钮
        var html = '<tr><td><a onclick="addConditionApprover(this)" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-add\'">对应流程</a></td>';
        var allRow = "";
        var conditionName = $("#condField option:selected").text();
        // 获取选中的datalist中的条件值
        if ($("#condField").val() == 'name') {
            var selectRow = $("#relativeField").datalist("getSelections");
            $.each(selectRow, function (i, v) {
                var array = {};
                array[conditionName] = v.value
                //var array = { conditionName: v.value };
                allRow += v.value + ",";
                conditionArray.push(array)
            });
            _conditionJson = {"type": conditionName,"selected": conditionArray};
        } else if ($("#condField").val() == 'number') {
            allRow = $("#minNumber").val() + "," + $("#maxNumber").val();
            var numberConditionName = $("#number").val();
            if (numberConditionName == "小于") {
                _conditionJson = { "type": "Number", "operator": "LT", "min": $("#minNumber").val(), "max": $("#maxNumber").val() };
            } else if (numberConditionName == "小于等于") {
                _conditionJson = { "type": "Number", "operator": "LTOET", "min": $("#minNumber").val(), "max": $("#maxNumber").val() };
            } else if (numberConditionName == "大于") {
                _conditionJson = { "type": "Number", "operator": "GT", "min": $("#minNumber").val(), "max": $("#maxNumber").val() };
            } else if (numberConditionName == "大于等于") {
                _conditionJson = { "type": "Number", "operator": "GTOET", "min": $("#minNumber").val(), "max": $("#maxNumber").val() };
            } else if (numberConditionName == "大于小于") {
                _conditionJson = { "type": "Number", "operator": "LT&GT", "min": $("#minNumber").val(), "max": $("#maxNumber").val() };
            } else if (numberConditionName == "大于等于小于") {
                _conditionJson = { "type": "Number", "operator": "LT&GTOET", "min": $("#minNumber").val(), "max": $("#maxNumber").val() };
            } else if (numberConditionName == "大于等于小于等于") {
                _conditionJson = { "type": "Number", "operator": "LTOET&GTOET", "min": $("#minNumber").val(), "max": $("#maxNumber").val() };
            } else if (numberConditionName == "大于小于等于") {
                _conditionJson = { "type": "Number", "operator": "LTOET&GT", "min": $("#minNumber").val(), "max": $("#maxNumber").val() };
            }
        } else {
            var checkedInputs = $('#conditionValue input:checkbox:checked');
            
            $.each(checkedInputs, function (i, v) {
                var array = {};
                array[conditionName] = $(this).val();
                conditionArray.push(array)
                allRow += $(this).val() + ",";
            })
            _conditionJson = {"type": "string","selected": conditionArray};
        }

        ConditionItem.push(conditionName);
        ConditionJson.push(_conditionJson);
        html += '<td>'+ $('#condField option:selected').text() +':';
        html += '<a href="#" onclick="removeThisCondition(this)" class="easyui-linkbutton" data-options="iconCls:\'icon-clear\',iconAlign:\'right\'">' + allRow + '</a></td></tr>';
        // 显示出选择的条件审批的值
        $("#ff").children().children().children("tr:last").after(html);
       
        $.parser.parse();
        $('#dlg-chooseCondition').dialog("close");
    }

    function addConditionApprover(aa) {
        // 获取这是第几个tr
        dialogIndex = $(aa).parent().parent().index();
        updateApprover(dialogIndex);
    }   

    function removeThisApprover(aa) {
        // 判断删除的是默认流程中的项 还是 条件审批流程中的项
        var removeDefaultOrCondition = $("#ff").find("tr").index($(aa).parent().parent());

        // 删除对应的数据
        if (removeDefaultOrCondition == 1) {
            // 如果删除的是默认审批流程
            var index = $("#ff").find("tr:eq(1)").children("td").index($(aa).parent());
            DefaultProcessJson.splice(index-2, 1);

        } else {
            // 如果删除的是 条件审批流程
            var index1 = $("#ff").find("tr").index($(aa).parent().parent());// 判断选择的第几个条件审批流程
            var index2 = $("#ff").find("tr:eq(" + index1 + ")").find("td").index($(aa).parent());// 判断选择的是第几个条件审批流程中的第几级

            var count = 0;
            var itemCount = 0;
            $.each(ConditionItem, function(i,v) {
                if (v != "") {
                    count++;
                } else {
                    itemCount ++;
                }

                if (count == (index1-2)) {
                    return false;
                }
            })
            DefaultProcessJson.splice(itemCount + index2 - 2, 1);

            //再删除对应的conditionitem
            ConditionItem.splice(itemCount + index2 + count - 2, 1);
            ConditionJson.splice(itemCount + index2 + count - 2, 1);
        }

        // 删除流程的当前节点以及后面的省略号
        $(aa).parent().remove();
      
    }

    function removeThisCondition(aa) {
        // 删除当前条件流程
        var index1 = $("#ff").find("tr").index($(aa).parent().parent());// 判断选择的第几个条件审批流程

        var count = 0;
        var itemCount = 0;
        var conditionCount = 0;
        $.each(ConditionItem, function(i,v) {
            if (v != "") {
                count++;
            } else {
                itemCount ++;
            }
            if (count == (index1 - 2)) {
                conditionCount ++;
            }
            if (count > (index1 - 2)) {
                return false;
            }
        });

        DefaultProcessJson.splice(itemCount - 1, conditionCount - 1);

        //再删除对应的conditionitem
        ConditionItem.splice(itemCount + index2 + count - 2, conditionCount);
        ConditionJson.splice(itemCount + index2 + count - 2, conditionCount);

        $(aa).parent().parent().remove();
    }

    function submitProcess() {
        $.ajax({
            url: 'FormProcess.aspx',
            data: {
                act: 'submitProcess',
                DefaultProcessJson: JSON.stringify(DefaultProcessJson),
                ConditionItem: JSON.stringify(ConditionItem),
                ConditionJson: JSON.stringify(ConditionJson),
                FormId: chooseFormId,
                ProcessName: $("#processName").textbox("getValue")
            },
            type: 'post',
            dataType: 'json',
            success: function (msg) {
                if (msg.ErrCode == 0) {
                    $.messager.alert('提示', '流程设置成功', 'info');
                } else {
                    $.messager.alert('提示', '流程设置失败', 'info');
                }
            },
            error: function () {

            }
        })
    }
</script>
</html>
