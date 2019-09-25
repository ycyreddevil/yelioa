<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Authorization.aspx.cs" Inherits="Authorization" %>

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
    <div data-options="region:'west',split:true,collapsible:false" title="应用列表" style="width: 300px;">
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
       <script type="text/javascript">
           var url = "Authorization.aspx";
           var applyId = "";
           $(document).ready(function () {
               initDatagrid();
               initUserTree();
               initCombobox();
               getUserTree();
           });
           function initDatagrid() {
               $('#datagrid').datagrid({
                   fitColumns: true,
                   striped: true,
                   singleSelect: true,
                   columns: [[
                       { field: 'Id', width: 20 },
                       { field: 'typeName', title: '类型', width: 80 },
                       { field: 'pageName', title: '应用', width: 80 }
                   ]],
                   onClickRow: function (rowIndex, rowData) {
                       $('#search').combobox('clear');
                       getHasRight(rowData.Id);
                   }
               });
               parent.Loading(true);
               $.post(url, { act: 'initDatagrid' },
                   function (res) {
                       if (res != "") {
                           var datasource = JSON.parse(res);
                           $('#datagrid').datagrid('loadData', datasource);
                       }
                       else
                           $.messager.alert('提示', '应用列表获取失败，请重新获取！', 'info');
                       parent.Loading(false);
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
           function addChooseMember(name,iconCls,nodeId) {
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
                   icons: [{
                       iconCls: 'icon-clear',
                       handler: function (e) {
                           $(e.data.target).parent().remove();
                           //$(e.data.target).prev().remove();

                           //var node = $('#userTree').tree('find', nodeId);                                     
                           //$('#userTree').tree('uncheck', node.target)
                       }
                   }]
               })
           }

           function getHasRight(id) {
               applyId = id;
               $('#chooseMember').empty();
               $.post(url, { act: 'getHasRight', id: id },
                   function (res) {
                       if (res != ""&&res!="F") {
                           var rightList = JSON.parse(res);
                           for (var i = 0; i < rightList.length; i++) {
                               if (rightList[i].DepartmentId != "") {
                                   var node = $('#userTree').tree('find', rightList[i].DepartmentId);
                                   addChooseMember(node.text, node.iconCls, node.id);
                               }
                               else {
                                   var node = $('#userTree').tree('find', rightList[i].userId);
                                   addChooseMember(node.text, node.iconCls, node.id);
                               }
                           }
                       }
                       else if(res=="F")
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
                      var rootNode=$('#userTree').tree('find', $(e).children("input").eq(1).val() );
                      if (rootNode.children.length != 0)
                          DepartmentIds.push(rootNode.id);
                      else
                          UserIds.push(rootNode.id);
                   });
               }                          
               var str1 = JSON.stringify(DepartmentIds);
               var str2 = JSON.stringify(UserIds);
               parent.Loading(true);
               $.post(url, { act:"updateRight", departmentIds:str1, UserIds: str2, id: applyId },
                   function (res) {
                       if (res == "操作成功") {
                           getHasRight(applyId);
                           $.messager.alert('提示',res, 'info');      
                       }
                       else
                           $.messager.alert('提示', '操作失败,请重新操作！'+res, 'info');
                       parent.Loading(false);
                   });
           }
       </script>
</body>
</html>
