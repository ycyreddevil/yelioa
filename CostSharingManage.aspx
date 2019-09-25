<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CostSharingManage.aspx.cs" Inherits="CostSharingManage" %>

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
    <script src="Scripts/jquery.json-2.4.min.js"></script>
    <style type="text/css">
        table input, table select {
            width: 200px;
        }
    </style>
</head>
<body>
    <table id="dg" class="easyui-datagrid"></table>
    <div id="dg-tb">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-add'," onclick="dlgOpen('add')">新增</a>
        <%--<a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-remove'," onclick="Delete()">停用</a>--%>
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-edit'," onclick="dlgOpen('edit')">编辑</a>
        <input id="dg-searchbox" class="easyui-searchbox" data-options="prompt:'人员或部门或医院或产品的拼音或文字',searcher:doSearch" style="width: 300px">
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-leaveStock" onclick="OpenFbxDialoge();dgLoad();">导入批量信息</a>
    </div>
    <div id="dlg-Import" class="easyui-dialog" title="批量信息导入" data-options="iconCls:'icon-leaveStock',modal:true,closed:true">
        <table id="dg-Import" class="easyui-datagrid">
        </table>
        <div id="tb-Import">
            <form id="fmFile" method="post" enctype="multipart/form-data">
                <input id="fbx" class="easyui-filebox" label="信息文件:" labelposition="left" 
                    data-options="onChange:function(){uploadFiles('showFile');},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1" >
                <input type="hidden" name="act" id="actFbx"/>
                <span id="ImportState"></span>
            </form>
        </div>
    </div>
    <div id="dlg-search" class="easyui-dialog" title="搜索选择" style="width: 560px; height: 560px; display: none" 
        data-options="toolbar:'#dlg-search-tools',modal:true,closed:true">        
        <div>
            <ul id="datalist" class="easyui-datalist" style="width: 95%;" data-options="valueField:'Id',textField:'name',striped:true,lines:true,
                onSelect: function(index,row){
                    <%--if(row.fullName)
                        $('#searchbox').textbox('setValue', row.fullName+','+row.Id);
                    else--%>
                        $('#searchbox').textbox('setValue', row.name+','+row.Id);
                }">
            </ul>
        </div>
    </div>
    <div id="dlg-search-tools">
            <%--<input id="searchbox" class="easyui-searchbox" data-options="prompt:'请输入拼音或文字',searcher:doSearch_dlg" style="width: 100%">--%>
            <input id="searchbox" class="easyui-textbox" style="width:95%;" data-options="
                iconWidth: 22,
                icons: [{
                    iconCls:'icon-search',
                    handler: function(e){
                        var value = $(e.data.target).textbox('getValue');
                        doSearch_dlg(value);
                    }
                },{
                    iconCls:'icon-ok',
                    handler: function(e){
                        var value = $(e.data.target).textbox('getValue');
                        GetValue_dlg_search(value);
                    }
                }]
                " />
        </div>
    <div id="dlg" class="easyui-dialog" title="详细信息" style="width: 800px; height: 650px; display: none"
            data-options="modal:true,closed:true">
        <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
            <table style="width: 100%;">
                <tr>
                    <td style="width: 5%;">&nbsp;</td>
                    <td style="width: 10%;text-align:right">&nbsp;</td>
                    <td style="width: 35%;text-align:left">&nbsp;</td>
                    <td style="width: 10%;text-align:right">&nbsp;</td>
                    <td style="width: 35%;text-align:left">
                        <input type="hidden" id="Id" name="Id" />
                        <input id="actSubmit" name="act" type="hidden" />
                    </td>
                    <td style="width: 5%;">&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>医院</td>
                    <td>
                        <%--<input class="easyui-combobox" id="HospitalId" name="HospitalId" style="width:100%;" data-options="
                            valueField:'Id',textField:'name',onChange:function(newValue,oldValue){HospitalIdOnChange(newValue, oldValue);}" />--%>
                        <input id="Hospital" name="Hospital" class="easyui-textbox" 
                            data-options="buttonText:'查找',onClickButton:function(){SearchFormOpen('hospital')},required:true">
                    </td>
                    <td>产品</td>
                    <td>
                        <input id="Product" name="Product" class="easyui-textbox" 
                            data-options="buttonText:'查找',required:true,onClickButton:function(){SearchFormOpen('product')}">
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>业务员</td>
                    <td>
                        <%--<input class="easyui-combobox" id="HospitalId" name="HospitalId" style="width:100%;" data-options="
                            valueField:'Id',textField:'name',onChange:function(newValue,oldValue){HospitalIdOnChange(newValue, oldValue);}" />--%>
                        <input id="Sales" name="Sales" class="easyui-textbox" 
                            data-options="buttonText:'查找',required:true,onClickButton:function(){SearchFormOpen('sales')}">
                    </td>
                    <td>主管</td>
                    <td>
                        <input id="Supervisor" name="Supervisor" class="easyui-textbox" 
                            data-options="buttonText:'查找',required:true,onClickButton:function(){SearchFormOpen('supervisor')}">
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>经理</td>
                    <td>
                        <%--<input class="easyui-combobox" id="HospitalId" name="HospitalId" style="width:100%;" data-options="
                            valueField:'Id',textField:'name',onChange:function(newValue,oldValue){HospitalIdOnChange(newValue, oldValue);}" />--%>
                        <input id="Manager" name="Manager" class="easyui-textbox" 
                            data-options="buttonText:'查找',required:true,onClickButton:function(){SearchFormOpen('manager')}">
                    </td>
                    <td>总监</td>
                    <td>
                        <input id="Director" name="Director" class="easyui-textbox" 
                            data-options="buttonText:'查找',required:true,onClickButton:function(){SearchFormOpen('director')}">
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>部门</td>
                    <td>
                        <input id="Department" name="Department" class="easyui-combotree" data-options="required:true,panelHeight:300" />
                    </td>
                    <td>区域</td>
                    <td>
                        <input id="Sector" name="Sector" class="easyui-combotree" data-options="required:true,panelHeight:300" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>医院供货价</td>
                    <td>
                        <input name="HospitalSupplyPrice" class="easyui-numberbox" data-options="required:true,min:0,precision:2" />
                    </td>
                    <td>公司开票价</td>
                    <td>
                        <input name="InvoicePrice" class="easyui-numberbox" data-options="required:true,min:0,precision:2" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>销售折让</td>
                    <td>
                        <input name="SalesAllowances" class="easyui-numberbox" data-options="required:true,min:0,precision:2" />
                    </td>
                    <td>固定资产分摊比率</td>
                    <td>
                        <input name="FixedAssetsRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>财务费用比率</td>
                    <td>
                        <input name="FinancialRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>研发费用比率</td>
                    <td>
                        <input name="RdRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>总部管理费用比率</td>
                    <td>
                        <input name="HeadOfficeManageRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>工资社保金额比率</td>
                    <td>
                        <input name="WageSocialSecurityRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>税金比率</td>
                    <td>
                        <input name="TaxRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>商务费用金额比率</td>
                    <td>
                        <input name="BusinessRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>开发费用比率</td>
                    <td>
                        <input name="DevelopmentRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>销售总监费用比率</td>
                    <td>
                        <input name="SalesDirectorRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>产品发展基金比率</td>
                    <td>
                        <input name="ProductDevelopmentFundRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>市场学术费比率</td>
                    <td>
                        <input name="MarketRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>市场调节基金</td>
                    <td>
                        <input name="MarketReadjustment" class="easyui-numberbox" data-options="required:true,min:0,precision:2" />
                    </td>
                    <td>实验员人员奖金比率</td>
                    <td>
                        <input name="ExperimenterBonus" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>实验员人员奖金比率</td>
                    <td>
                        <input name="ExperimenterBonusRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>PMS</td>
                    <td>
                        <input name="Pms" class="easyui-numberbox" data-options="required:true,min:0,precision:2" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>日常客情维护费</td>
                    <td>
                        <input name="GuestMaintenance" class="easyui-numberbox" data-options="required:true,min:0,precision:2" />
                    </td>
                    <td>实验费（TF）金额比率</td>
                    <td>
                        <input name="TfRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>区域中心费用比率</td>
                    <td>
                        <input name="RegionalCenterRatio" class="easyui-numberbox" data-options="required:true,min:0,precision:2,suffix:'%'" />
                    </td>
                    <td>区域中心费用VIP</td>
                    <td>
                        <input name="RegionalCenterVip" class="easyui-numberbox" data-options="required:true,min:0,precision:2" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </form>
    </div>
    <%--<div id="dlg-Import" class="easyui-dialog" title="批量信息导入" style="width: 800px; height: 60px"
        data-options="iconCls:'icon-import',modal:true,closed:true">
        <form id="fmFile" method="post" enctype="multipart/form-data">
            <input id="fbx" class="easyui-filebox" label="批量信息文件:" labelposition="left"
                data-options="onChange:function(){uploadFiles('upload');},prompt:'请选择一个xls文件...'"
                style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<input type="hidden" name="act" id="actFbx" />
        </form>
    </div>--%>

    <script type="text/javascript">
        var Url = "CostSharingManage.aspx";
        var SearchFormType = "";
        var ValueSearched, dlgType, TreeDataJson,UploadIndex=-1,DgImportLength;
        var TotalImportNumber,ImportedNumber;


        $(document).ready(function () {
            InitTable();
            InitDlgs();
            dgLoad();
            InitDepartmentData();
        });

        function AjaxSync(url, data) {
            parent.Loading(true);
            var res = $.ajax({
                async: false,
                cache: false,
                type: 'post',
                url: url,
                data: data
            }).responseText;
            parent.Loading(false);
            return res;
        }

        function InitDlgs() {
            $('#dlg').dialog({//用户对话框
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok', size: 'large',
                    handler: function () {
                        Submit();
                    }
                }, {
                    text: '清除',
                    iconCls: 'icon-clear', size: 'large',
                    handler: function () {
                        $('#fm').form('clear');
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        $('#dlg').dialog('close');
                    }
                }],
                onOpen: function () {
                    var h = $(window).height() - 60;
                    $(this).dialog('resize', { height: h });
                    $(this).dialog('move', { top: 30 });
                }
            });
            $('#dlg-search').dialog({
                onOpen: function () {
                    var h = $(window).height() - 60;
                    $(this).dialog('resize', { height: h });
                    $(this).dialog('move', { top: 30 });
                }
            });

            $('#dlg-Import').dialog({
                buttons: [{
                    text: '导入',
                    iconCls: 'icon-leaveStock', size: 'large',
                    handler: function () {
                        //uploadFiles('import');
                        UploadIndex = 0;
                        $('#dg-Import').datagrid('selectRow', UploadIndex);
                        //var rows = $('#dg-Import').datagrid('getRows');
                        
                        //for (var i = 5; i >= 0; i--) {
                            
                        //    $.post(Url, { act: 'test' }, function () {
                        //        $('#dg-Import').datagrid('selectRow', i);
                        //    });
                        //}
                        //var upload =setTimeout(function () {
                        //    $('#dg-Import').datagrid('scrollTo', i);
                        //    $('#dg-Import').datagrid('highlightRow', i);
                        //    i--;
                        //    if (i < 0)
                        //        clearTimeout(upload);
                        //}, 200);                           
                    }
                }, {
                    text: '排查重复网点',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        DatagridClear('dg-Import');
                        uploadFiles('CheckRepitition');
                    }
                }, {
                    text: '隐藏已提交网点',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        var data = $('#dg-Import').datagrid('getData');
                        for(var i = data.total-1;i>=0;i--){
                            var row = data.rows[i];
                            if(row['状态'] == '已导入'){
                                $('#dg-Import').datagrid('deleteRow', i);
                            }
                        }
                    }
                }, {
                    text: '关闭',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        $('#dlg-Import').dialog('close');
                    }
                }],
                onOpen: function () {
                    var w = $(window).width() - 80;
                    var h = $(window).height() - 80;
                    $('#dlg-Import').dialog('resize', { width: w, height: h });
                    $('#dlg-Import').dialog('move', { left: 40, top: 40 });
                    $('#dg-Import').datagrid({
                        columns: [[]]
                    });
                    TotalImportNumber=0;
                    ImportedNumber=0;
                    $('#ImportState').html('');
                },
                onClose: function () {
                    dgLoad();
                }
            });
        }

        function InitTable()
        {
            $('#dg').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#dg-tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: 'Hospital', width: 20, align: 'center', title: '医院',sortable:"true" },
                    { field: 'Product', width: 20, align: 'center', title: '产品', sortable: "true" },
                    { field: 'Department', width: 10, align: 'center', title: '部门', sortable: "true" },
                    { field: 'Sector', width: 10, align: 'center', title: '区域', sortable: "true" },
                    { field: 'Sales', width: 10, align: 'center', title: '业务员', sortable: "true" },
                    { field: 'Supervisor', width: 10, align: 'center', title: '主管', sortable: "true" },
                    { field: 'Manager', width: 10, align: 'center', title: '经理', sortable: "true" },
                    { field: 'Director', width: 10, align: 'center', title: '总监', sortable: "true" }
                ]],
                onDblClickRow: function () { dlgOpen('edit'); },
                sortName: 'Hospital',
                sortOrder: 'asc',
                onSortColumn: function (sort, order) {
                    dgLoad($('#dg-searchbox').searchbox('getValue'), sort, order);
                }
            });

            $('#dg-Import').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#tb-Import',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: false,
                columns: [[]],
                onSelect: function (index, row) {                    
                    if (UploadIndex == index) {
                        var jsonStr = JSON.stringify(row);
                        var data = { act: 'import', json: jsonStr ,index:index};
                        $.post(Url, data, function (res) {
                            var newRow = $.parseJSON(res);
                            $('#dg-Import').datagrid('deleteRow', index);
                            $('#dg-Import').datagrid('insertRow', {
                                index: index,	// index start with 0
                                row: newRow
                            });
                            if(newRow.状态=='已导入'){
                                ImportedNumber++;
                                $('#ImportState').html('共'+TotalImportNumber+'条数据，已导入'+ImportedNumber+'条');
                            }
                            
                            if (index < (DgImportLength - 1)) {
                                UploadIndex = index + 1;
                                $('#dg-Import').datagrid('selectRow', UploadIndex);
                            }
                            else {//最后一行
                                UploadIndex = -1;
                                //$('#dlg-Import').dialog('close');
                                //dgLoad();
                            }
                                

                        });
                    }                                       
                }
            });            
        }

        function InitDepartmentData()
        {
            var url = "MemberManage.aspx";
            var data = {
                act: 'getTree'
            };
            TreeDataJson = AjaxSync(url, data);
        }

        function dgLoad(searchString, sort, order) {
            if (searchString == undefined) {
                searchString = "";
            }
            if (sort == undefined)
            {
                sort = 'Hospital'; order = 'asc';
            }
            var data = {
                act: 'getInfos',
                searchString: searchString, sort: sort, order: order
            };
            var res = AjaxSync(Url, data);
            var datasource = $.parseJSON(res);
            $('#dg').datagrid("loadData", datasource);
        }

        function doSearch(value) {
            dgLoad(value);
        }

        function doSearch_dlg(value){
            DlistLoad(value);
        }

        function GetValue_dlg_search(value) {
            ValueSearched = value;
            $('#dlg-search').dialog('close'); 
            if(SearchFormType == 'hospital')
            {
                $('#Hospital').textbox("setValue", value);
            }
            else if (SearchFormType == 'product') {
                $('#Product').textbox("setValue", value);
            }
            else if (SearchFormType == 'sales') {
                $('#Sales').textbox("setValue", value);
            }
            else if (SearchFormType == 'supervisor') {
                $('#Supervisor').textbox("setValue", value);
            }
            else if (SearchFormType == 'manager') {
                $('#Manager').textbox("setValue", value);
            }
            else if (SearchFormType == 'director') {
                $('#Director').textbox("setValue", value);
            }
        }

        function SearchFormOpen(type) {
            SearchFormType = type;
            $('#dlg-search').dialog('open');
            DlistLoad();
        }

        function DlistLoad(searchString) {
            if (searchString == undefined) {
                searchString = "";
            }
            var res = AjaxSync(Url, { act: 'getDatalist', type: SearchFormType, searchString: searchString });
            if (res != "") {
                var data = $.parseJSON(res);
                if (data.total > 0)
                {
                    if (data.rows[0].fullName) {
                        $('#datalist').datalist({ valueField: 'Id', textField: 'fullName' });
                    }
                    else {
                        $('#datalist').datalist({ valueField: 'Id', textField: 'name' });
                    }
                }
                $('#datalist').datalist("loadData", data);
                $('#searchbox').textbox("setValue", "");
            }
        }

        function CollapseTreeNode(combotree)
        {
            var root = combotree.combotree('tree').tree('getRoot');
            //var children = combotree.combotree('tree').tree('getChildren',root);
            //for (var node in root.children) {
            //    var text = node.text;
            //    if (text.indexOf('东森') < 0) {
            //        combotree.combotree('tree').tree('collapse', node);
            //    }
            //}
            $.each(root.children, function (i, val) {
                var text = val.text;
                if (text.indexOf('东森') < 0) {
                    var node = combotree.combotree('tree').tree('find', val.id);
                    combotree.combotree('tree').tree('collapse', node.target);
                }
            });
        }

        function dlgOpen(type) {
            dlgType = type;
            $('#Department').combotree("loadData", $.parseJSON(TreeDataJson));
            CollapseTreeNode($('#Department'));
            $('#Sector').combotree("loadData", $.parseJSON(TreeDataJson));
            CollapseTreeNode($('#Sector'));
            if (type == 'add')
            {
                $('#fm').form('reset');
                
            }
            else if (type == 'edit') {
                var row = $('#dg').datagrid('getSelected');
                if (row == null) {
                    $.messager.alert("提示", "请先选择一行数据!", "info");
                    return;
                }
                else {
                    $('#fm').form('load', row);
                    $('#Department').combotree("setValue", row.DepartmentId);
                    $('#Sector').combotree("setValue", row.SectorId);
                    $('#Hospital').textbox("setValue", row.Hospital + ',' + row.HospitalId);
                    $('#Product').textbox("setValue", row.Product + ',' + row.ProductId);
                    $('#Sales').textbox("setValue", row.Sales + ',' + row.SalesId);
                    $('#Supervisor').textbox("setValue", row.Supervisor + ',' + row.SupervisorId);
                    $('#Manager').textbox("setValue", row.Manager + ',' + row.ManagerId);
                    $('#Director').textbox("setValue", row.Director + ',' + row.DirectorId);
                }
            }
            $('#dlg').dialog('open');
            $('#fm').form('disableValidation');
        }
        function DatagridClear(dgId) {
            $('#' + dgId).datagrid('loadData', { total: 0, rows: [] });
        }
        function OpenFbxDialoge() {
            DatagridClear('dg-Import');
            $('#dlg-Import').dialog('open');
            $('#fmFile').form('clear');
        }

        function uploadFiles(type) {
            var fileName = $('#fbx').filebox('getValue');
            if (fileName == "") {
                return;
            }
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            } else {
                $('#actFbx').val(type);
                parent.Loading(true);
                $('#fmFile').form('submit', {
                    url: Url,
                    success: function (res) {
                        parent.Loading(false);
                        try {
                            var datasource = $.parseJSON(res);
                            if(datasource.total > 0)
                                DgImportLoad(datasource);
                            TotalImportNumber = datasource.total;
                            ImportedNumber = 0;
                            $('#ImportState').html('共'+datasource.total+'条数据，已导入'+ImportedNumber+'条');
                        }
                        catch (e) {
                            $.messager.alert('错误提示', res + '\r\n' + e, 'error');
                        }
                    }
                });
            }
        }

        function DgImportLoad(data) {
            var columns = new Array();
            var frozenColumns = new Array();
            var obj = data.rows[0];
            DgImportLength = data.total;
            //columns.push();
            $.each(obj, function (key, value) {
                if (key == '状态') {
                    frozenColumns.push({
                        field: '状态', title: '状态', width: 150, align: 'center',frozen:true,
                        formatter: function (value, row, index) {
                            if (value == '已导入') {
                                return '<span style="color: #FFFFFF; background-color: #00CC00">已导入</span>';
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #FF0000">' + value + '</span>';
                            }
                        }
                    });
                }
                else {
                    var column = {};
                    column["field"] = key;
                    column["title"] = key;
                    column["width"] = 100;
                    column["align"] = 'center';
                    column["sortable"] = true;
                    columns.push(column);
                }
            });
            $('#dg-Import').datagrid({
                columns: [columns],
                frozenColumns: [frozenColumns]
            }).datagrid("loadData", data);
        }

        function Submit() {
            $('#fm').form('submit', {
                url: Url,
                onSubmit: function () {
                    var res = false;
                    if ($(this).form('enableValidation').form('validate')) {
                        parent.Loading(true);
                        $('#actSubmit').val(dlgType);
                        res = true;
                    }
                    return res;
                },
                success: function (res) {
                    parent.Loading(false);
                    $.messager.alert({
                        title: '提示', msg: res, icon: 'info',
                        fn: function () {
                            if (res.indexOf('新建成功') >= 0 || res.indexOf('修改成功') >= 0) {
                                $('#dlg').dialog('close');
                                dgLoad();
                            }
                        }
                    });

                }
            });
        }

    </script>
</body>
</html>
