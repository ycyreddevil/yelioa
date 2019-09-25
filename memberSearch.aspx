<%@ Page Language="C#" AutoEventWireup="true" CodeFile="memberSearch.aspx.cs" Inherits="memberSearch" %>

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
</head>
<body>
    <%--<form id="form1" runat="server">
        <div>
        </div>
    </form>--%>
    <table id="dg" class="easyui-datagrid" title="人员信息">
    </table>
    <div id="dg-tb">
        <input class="easyui-searchbox" data-options="prompt:'人员名字或工号的拼音或文字',searcher:doSearch" style="width: 300px">
    </div>
    <script type="text/javascript">        
        var toolbar = [{
            text: '新建用户',
            iconCls: 'icon-add',
            handler: function () { alert('add') }
        }, {
            text: 'Cut',
            iconCls: 'icon-cut',
            handler: function () { alert('cut') }
        }, '-', {
            text: '查看详情',
            iconCls: 'icon-save',
            handler: function () { alert('save') }
        }];

        $(document).ready(function () {
            InitDatagrid();
            dg_memberSearchLoad();
        });

        function formatState(val,row)
        {
            if(val == '在职')
            {
                return '<span style="color: #FFFFFF; background-color: #00CC00">' + val + '</span>'; 
            }
            else {
                return '<span style="color: #FFFFFF; background-color: #FF0000">' + val + '</span>';
            }
        }

        function dg_memberSearchLoad(search) {
            var url = "memberSearch.aspx";
            var data = {
                act: 'getMembers',
                searchString: search
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                parent.Loading(false);
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
            });
        }
        function InitDatagrid() {
            $('#dg').datagrid({
                singleSelect: true,fit:true,
                toolbar: '#dg-tb',
                striped:true,
                rownumbers:true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    {field:'userName',width:20,align:'center',title:'姓名'},
                    {field:'sex',width:10,align:'center',title:'性别'},
                    {field:'mobilePhone',width:20,align:'center',title:'手机号'},
                    {field:'employeeCode',width:20,align:'center',title:'工号'},
                    //{field:'idNumber',width:20,align:'center',title:'身份证号'},
                    //{ field: 'company', width: 40, align: 'center', title: '公司' },
                    {field:'department',width:20,align:'center',title:'部门'},
                    {field:'post',width:10,align:'center',title:'岗位'},
                    {
                        field: 'hiredate', width: 20, align: 'center', title: '入职时间',
                        formatter: function (value, row, index) {
                            if (value == null || value == '') {
                                return '';
                            }
                            var dt ;
                            if (value instanceof Date) {
                                dt = value;
                            } else {
                                dt = new Date(value);
                            }

                            var res = dt.getFullYear() + '年' + (dt.getMonth()+1) + '月' + dt.getDate() + '日';
                            return res;
                        }
                    },
                    {
                        field: 'isValid', title: '状态', width: 10,align:'center',
                        formatter: function (value, row, index) {
                            if(value == '在职')
                            {
                                return '<span style="color: #FFFFFF; background-color: #00CC00">' + value + '</span>'; 
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #FF0000">' + value + '</span>';
                            }
                        }
                    },
                    {field:'remark',width:30,align:'center',title:'备注'}
                ]],
            });
        }
        function doSearch(value) {
            dg_memberSearchLoad(value);
        }
    </script>
</body>
</html>
