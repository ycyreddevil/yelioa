<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FormIndex.aspx.cs" Inherits="FormIndex" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>自定义表单首页</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link type="text/css" rel="stylesheet" href="Scripts/formbuilder/css/formbuild.css?v=20160929" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/base-loading.js"></script>
    <script src="Scripts/pcCommon.js"></script>
    <script src="Scripts/jquery.edatagrid.js"></script>
</head>
<body class="easyui-layout">
    <div data-options="region:'west',split:true,collapsible:false,iconCls:'icon-add',iconAlign:'left'" title="表单列表" style="width: 300px;">
        <div id="tb2">
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-save" onclick="save()">保存</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-undo" onclick="cancel()">取消</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-add" onclick="add()">新增</a>
            <%--<a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-remove" onclick="deleteForm()">删除</a>--%>
        </div>
        <div id="datagrid" class="easyui-datagrid"></div>
    </div>
    <div data-options="region:'center',split:true,title:'部门人员列表',collapsible:false" style="width: 300px;">
        <div style="margin-bottom: 10px">
            <input id="search" class="easyui-combobox" style="width: 100%" data-options="prompt:'请输入中文汉字或者拼音首字母进行搜索'" />
        </div>
        <div id="userTree" class="tree"></div>
    </div>
    <div id="chooseMember" style="width: 220px" data-options="region:'east',split:true,title:'已选中部门或人员',collapsible:false">
        <footer>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="updateRight()">确认修改</a>
        </footer>
    </div>
    <div id="showFormDialog" style="padding: 20px 10px" class="easyui-dialog" title="表单展示" data-options="iconCls:'icon-leaveStock',modal:true,closed:true">
        
    </div>
</body>
<script>
    var url = "FormIndex.aspx";
    var applyId = "";
    $(document).ready(function () {
        initDatagrid();
        initUserTree();
        initCombobox();
        getUserTree();
    });

    function add() {
        location.href='FormBuilder.aspx'
    }

    function showForm(id) {
        $('#showFormDialog').dialog('open');
        //$('#showFormDialog').window('center');

        var w = $(window).width() - 80;
        var h = $(window).height() - 80;
        $('#showFormDialog').dialog('resize', { width: w/2, height: h });
        $('#showFormDialog').dialog('move', { left: 280, top: 40 });

        $("#showFormDialog").empty();
        $.ajax({
            url: url,
            data: {act: 'findDetail',id: id},
            dataType: 'json',
            type: 'post',
            success: function (data) {
                var formData = JSON.parse(JSON.parse(data.data)[0].FormData);
                var parameterData = JSON.parse(JSON.parse(data.data)[0].ParameterData);

                for (i = 0; i < parameterData.length; i++) {
                    var widgt = "";
                    if (parameterData[i].TYP == "checkbox") {
                        //widgt += "<div>" + parameterData[i].LBL;
                        //$.each(parameterData[i].ITMS, function (index, value) {
                        //    widgt += "<input type='checkbox' value='" + value.VAL + "'/>";
                        //})
                        //widgt += "</div>"

                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content">';
                        $.each(parameterData[i].ITMS, function (index, value) {
                            widgt += '<span><input type="checkbox" /><label>' + value.VAL + '</label></span>';
                        })
                        widgt += '</div></br>';
                    }
                    else if (parameterData[i].TYP == "textarea") {
                        //widgt += "<div>" + parameterData[i].LBL + "<input type='textarea' value=" + parameterData[i].DEF + "/></div>";
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content"><textarea disabled="disabled" class="input">' + parameterData[i].DEF + '</textarea></div></br>';
                    }
                    else if (parameterData[i].TYP == "text" ) {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content textcontent"><input type="text" disabled="disabled" maxlength="255" class="input"/><i class="iconfont qrinput hide">&#xe67d;</i></div></br>';
                    }
                    else if (parameterData[i].TYP == "name") {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content oneline reduction"><input type="text" disabled="disabled" maxlength="255" class="input"/><i class="iconfont qrinput hide">&#xe67d;</i></div></br>';
                    }
                    else if (parameterData[i].TYP == "number") {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label> <div class="content"><input type="text" disabled="disabled" maxlength="32" class="input" /></div></br>';
                    }
                    else if (parameterData[i].TYP == "ratio") {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label> <div class="content">';
                        $.each(parameterData[i].ITMS, function (index, value) {
                            widgt += '<span><input type="ratio" /><label>' + value.VAL + '</label></span>';
                        })
                        widgt += '</div></br>';
                    }
                    else if (parameterData[i].TYP == "dropdown") {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label> <div class="content"><select disabled="disabled" class="m input"></select></div></br>'
                    }
                    else if (parameterData[i].TYP == "dropdown2") {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label> <div class="content"><select disabled="disabled" class="m input"></select> <select disabled="disabled" class="m input"></select></div></br>';
                    }
                    else if (parameterData[i].TYP == "image") {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content"><img style="width:100%;" src="/Content/CustomFrom/FormDesign/images/defaultimg.png" /></div></br>';
                    }
                    else if (parameterData[i].TYP == "date") {
                        widgt += '<label class="desc">日期</label> <div class="content oneline reduction"><span>\t<input class="yyyy input" disabled="disabled" maxlength="4" type="text" />\t</span><span class="split"> - </span><span>\t<input class="mm input" disabled="disabled" maxlength="2" type="text" />\t</span><span class="split"> - </span><span>\t<input class="dd input" disabled="disabled" maxlength="2" type="text" />\t</span><span><a class="icononly-date" title="选择日期"></a></span></div></br></br></br>';
                    }
                    else if (parameterData[i].TYP == "time") {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content oneline reduction"><span>\t<select class="hh input" disabled="disabled"></select></span><span class="split"> : </span><span>\t<select class="mm input" disabled="disabled"></select></span></div></br>';
                    }
                    else if (parameterData[i].TYP == "file") {
                        widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content"><input type="text" disabled="disabled" class="m input" />&nbsp;<input type="button" class="btn file-input" disabled="disabled" value="浏览..." /></div></br>';
                    }

                    $("#showFormDialog").append(widgt);
                }
            }
        })
    }

    function editForm(id) {
        location.href = "FormBuilder.aspx?id="+id;
    }

    function save() {
        $('#datagrid').data('isSave', true)
        $('#datagrid').edatagrid('saveRow');
    }

    function cancel() {
        $('#datagrid').edatagrid('cancelRow');
    }

    function deleteForm(){
        var rows = $('#datagrid').datagrid('getSelections');
    }

    function initDatagrid() {
        $('#datagrid').edatagrid({
            fitColumns: true,
            striped: true,
            singleSelect: true,
            toolbar: '#tb2',
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
                            data: [{value:"人事",text:"人事"},{value:"财务",text:"财务"},{value:"运营",text:"运营"},{value:"行政",text:"行政"},{value:"研发",text:"研发"},{value:"销售",text:"销售"},{value:"其他",text:"其他"}],
                            valueField: "value",
                            textField: "text",
                            editable: false
                        }
                    },
                },
                {
                    field: 'operate',
                    title: '操作',
                    formatter: function (value, row, index) {
                        return '<a href="#" onclick="showForm('+row.Id+')">预览</a>  <a href="#" onclick="editForm('+row.Id+')">编辑</a>';  
                    }
                }
            ]],
            onSave: function (index, row) {
                var $datagrid = $('#datagrid');
                if ($datagrid.data('isSave')) {
                    //如果需要刷新，保存完后刷新
                    $datagrid.removeData('isSave');
                    $.ajax({
                        url: url,
                        data: {
                            act: 'updateFormTypeAndValid',
                            id: row.Id,
                            type: row.Type,
                            valid: row.Valid
                        },
                        type: 'post',
                        dataType: 'json',
                        success: function (data) {
                            if (data.ErrCode == 0) {
                                $.messager.alert('提示', '保存成功', 'info');
                            } else {
                                $.messager.alert('提示', '保存失败', 'info');
                            }
                            $datagrid.edatagrid('reload');
                        },
                        error: function (msg) {
                            $.messager.alert('提示', '保存失败', 'info');
                            $datagrid.edatagrid('reload');
                        }
                    })
                }
            },
            onClickRow: function (rowIndex, rowData) {
                $('#search').combobox('clear');
                getHasRight(rowData.Id);
            }
        });
        parent.Loading(true);
        $.post(url, { act: 'initDatagrid'},
            function (res) {
                if (res != "") {
                    var datasource = JSON.parse(res);
                    $('#datagrid').datagrid('loadData', JSON.parse(datasource.data));
                }
                else
                    $.messager.alert('提示', '表单列表获取失败，请重新获取！', 'info');
                parent.Loading(false);
            }
        );

        $('#dlg-Import').dialog({
            buttons: [{
                text: '关闭',
                iconCls: 'icon-cancel', size: 'large',
                handler: function () {
                    $('#showForm').dialog('close');
                }
            }],
            onOpen: function () {
                var w = $(window).width() - 80;
                var h = $(window).height() - 80;
                $('#showForm').dialog('resize', { width: w, height: h });
                $('#showForm').dialog('move', { left: 40, top: 40 });
                $('#showForm').datagrid({
                    columns: [[]]
                });
            },
            onClose: function () {
                
            }
        });

    }

    function getSearched() {
        searchStr = $('#search').combobox('getText');
        parent.Loading(true);
        $.post(url, { act: 'getSearched', searchStr: searchStr },
            function (res) {
                if (res != "") {
                    var datasource = JSON.parse(res);
                    $('#search').combobox('loadData', datasource);
                }
                else
                    $.messager.alert('提示', '未找到相关部门和人员，请重新输入！', 'info');
                parent.Loading(false);
            });
    }
    function initCombobox() {
        $('#search').combobox({
            valueField: 'value',
            iconAlign: 'left',
            icons: [{

                iconCls: 'icon-search',
                handler: function (e) {
                    getSearched();
                }
            }],
            textField: 'text',
            panelHeight: 'auto',
            hasDownArrow: false,
            onSelect: function (record) {
                var node = $('#userTree').tree('find', record.value);
                isAdd(node);
                $('#search').combobox('unselect', record.value);
                $('#search').combobox('loadData', []);
            },
            formatter: function (row) {
                var opts = $(this).combobox('options');
                return row[opts.textField];
            }
        });
    }

    function initUserTree() {
        $('#userTree').tree({
            animate: true, lines: true, lines: false,
            onClick: function (node) {
                if (applyId == "") {
                    $('#userTree').tree('uncheck', node.target);
                    $.messager.alert('提示', '未选择应用，请先选择应用！', 'info');
                    return;
                }
                isAdd(node);
            },
            formatter: function (node) {
                return node.text;
            }
        });
    }

    //右侧是否含有次结点
    function isAdd(node) {
        var flag = -1;
        if ($('#chooseMember div').length > 0) {

            $('#chooseMember div').each(function (i, e) {
                if ($(e).children("input").eq(1).val() == node.id) {
                    flag = i;

                }
            });
        }
        if (flag == -1)
            addChooseMember(node.text, node.iconCls, node.id);
        else
            $('#chooseMember div')[flag].remove();
    }

    // 把选中的节点添加到右侧
    function addChooseMember(name, iconCls, nodeId) {
        var html = '<div><input class="easyui-textbox" style="width:80%"><input type="hidden" class="nodeId"/><input class="easyui-textbox" style="width:20%"></div>';
        $("#chooseMember").append(html);
        if (iconCls == null) {
            $("#chooseMember div:last input:first").textbox({
                value: name,
                iconAlign: 'left',
                editable: false,
                iconCls: 'icon-organization'
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
            icons: [
                {
                    iconCls: 'icon-clear',
                    handler: function(e) {
                        $(e.data.target).parent().remove();
                        //$(e.data.target).prev().remove();

                        //var node = $('#userTree').tree('find', nodeId);                                     
                        //$('#userTree').tree('uncheck', node.target)
                    }
                }
            ]
        });
    }

    function getHasRight(id) {
        applyId = id;
        $('#chooseMember').empty();
        $.post(url, { act: 'getHasRight', id: id },
            function (res) {
                if (res != "") {
                    var rightList = JSON.parse(JSON.parse(JSON.parse(res).data)[0].VisibleRange);
                    var departmentList = JSON.parse(rightList.departmentJSON);
                    var userList = JSON.parse(rightList.userJSON);

                    for (var i = 0; i < departmentList.length; i++) {
                        if (departmentList[i] != "") {
                            var node = $('#userTree').tree('find', departmentList[i]);
                            addChooseMember(node.text, node.iconCls, node.id);
                        }
                    }
                    for (var i = 0; i < userList.length; i++) {
                        if (userList[i] != "") {
                            var node = $('#userTree').tree('find', userList[i]);
                            addChooseMember(node.text, node.iconCls, node.id);
                        }
                    }
                }
                else if (res == "F")
                    $.messager.alert('提示', '获取可见范围失败，请重新获取！', 'info');
            });
    }
    function getUserTree() {
        parent.Loading(true);
        $.post(url, { act: 'initUserTree' },
            function (res) {
                if (res != "") {
                    data = JSON.parse(res);
                    $('#userTree').tree('loadData', data);
                }
                parent.Loading(false);
            });
    }


    function updateRight() {
        var DepartmentIds = new Array();
        var UserIds = new Array();
        if ($('#chooseMember div').length > 0) {
            $('#chooseMember div').each(function (i, e) {
                var rootNode = $('#userTree').tree('find', $(e).children("input").eq(1).val());
                if (rootNode.children.length != 0)
                    DepartmentIds.push(rootNode.id);
                else
                    UserIds.push(rootNode.id);
            });
        }
        var str1 = JSON.stringify(DepartmentIds);
        var str2 = JSON.stringify(UserIds);

        var visionJson = { departmentJSON: str1, userJSON: str2 };
        parent.Loading(true);
        $.post(url, { act: "updateRight", visionJson: JSON.stringify(visionJson), id: applyId },
            function (res) {
                var resdata = JSON.parse(res);
                if (resdata["ErrMsg"] == "操作成功") {
                    getHasRight(applyId);
                    $.messager.alert('提示', resdata["ErrMsg"], 'info');
                }
                else
                    $.messager.alert('提示', '操作失败,请重新操作！' + res, 'info');
                parent.Loading(false);
            });
    }
</script>
</html>
