<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FinancialItemSetting.aspx.cs" Inherits="FinancialItemSetting" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>江西东森各项目预算设置</title>
    <link href="Scripts/bootstrap-table/css/bootstrap-table.css" rel="stylesheet" />
    <link rel="stylesheet" href="Scripts/bootstrap-table/css/bootstrap-editable.css">
    <link href="Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/bootstrap-datetime-picker/css/bootstrap-datetimepicker.css" rel="stylesheet" />
    <link href="Scripts/bootstrap-fileinput/fileinput.css" rel="stylesheet"/>
    <link href="Scripts/bootstrap-select/bootstrap-select.css" rel="stylesheet"/>
    <style>
        .bootstrap-table {position:absolute}
    </style>
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/bootstrap/js/bootstrap.min.js"></script>
    <script src="Scripts/bootstrap-table/js/bootstrap-table.js"></script>
    <script src="Scripts/bootstrap-table/js/bootstrap-editable.js"></script>
    <script src="Scripts/bootstrap-table/js/bootstrap-table-editable.js"></script>
    <script src="Scripts/bootstrap-datetime-picker/js/bootstrap-datetimepicker.js"></script>
    <script src="Scripts/bootstrap-fileinput/fileinput.js"></script>
    <script src="Scripts/bootbox.js"></script>
    <script src="Scripts/bootstrap-fileinput/zh.js"></script>
    <script src="Scripts/bootstrap-select/bootstrap-select.js"></script>
</head>
<body>
   <%-- <div class="input-append date" id="datetimepicker" data-date="12-02-2012" data-date-format="dd-mm-yyyy">
        <input size="16" type="text" value="12-02-2012" readonly>
        <span class="add-on"><i class="icon-th"></i></span>
        <button id="idsdis" type="button" class="btn btn-lg btn-danger" data-toggle="popover" data-trigger="focus" 
            title="Popover title" data-content="And here's some amazing content. It's very engaging. Right?">点我弹出/隐藏弹出框</button>
    </div>--%>
    <div>
        <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#ExcelModal">批量信息导入</button>
    </div>

    <select class="selectpicker" >

    </select>
    
    <table id="tb_departments">
        
    </table>
    <%--填报数据模态框--%>
    <div class="modal" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="myModalLabel"></h4>
                </div>
                <div class="modal-body">
                    <form>
                        <div class="form-group" id="aggre_num_div">
                            <label for="aggre_num" class="control-label">累计数据:</label>
                            <input type="text" class="form-control" id="aggre_num">
                        </div>
                        <div class="form-group">
                            <label for="new_num" class="control-label">本次数据:</label>
                            <input type="text" class="form-control" id="new_num">
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                    <button type="button" onclick="submit()" class="btn btn-primary">上报</button>
                </div>
            </div>
        </div>
    </div>
    <%--上传excel模态框--%>
    <div class="modal bs-example-modal-lg" id="ExcelModal" tabindex="-1" role="dialog" aria-labelledby="myExcelModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="myExcelModalLabel">请选择Excel文件</h4>
                </div>
                <div class="modal-body">
                    <form id="ffImport" method="post">
                        <p title="Excel导入操作" style="padding: 5px">
                        <input type="hidden" id="AttachGUID" name="AttachGUID" />
                        <input type="file" name="txt_file" id="txt_file" class="file-loading" />
                        </p>
                    </form>
                    <table id="gridImport" class="table table-striped table-bordered table-hover" cellpadding="0" cellspacing="0" border="0">
                        　

                    </table>
                </div>
            </div>
        </div>
    </div>
</body>

<script>
    var count = 0;
    var date= new Date();
    //$('#datetimepicker').datetimepicker({
    //    language: 'zh-CN',//显示中文
    //    format: 'yyyy-mm-dd',//显示格式
    //    minView: "month",//设置只显示到月份
    //    initialDate: new Date(),//初始化当前日期
    //    autoclose: true,//选中自动关闭
    //    todayBtn: true//显示今日按钮
    //});
    var InitImport = function (data) {
        $('#gridImport').bootstrapTable('refresh')
        $("#gridImport").bootstrapTable({
            method: 'post',
            contentType: "application/x-www-form-urlencoded",
            //url : 'query',    //请求地址，返回数据格式可以参照下面data数据格式
            cache: false,
            //striped: true,
            pagination: true,
            pageSize: 10,
            pageNumber: 1,
            //pageList: [10, 20, 50, 100, 200, 500],
            //showColumns: true,
            //showRefresh: true,
            //showExport : true,
            //exportTypes : [ 'excel', 'txt', 'xml' ],
            //search: true,
            clickToSelect: true,
            columns: [{
                field: 'name',
                title: '报销人',
            }, {
                field: 'department',
                title: '所属部门'
            }, {
                field: 'filecode',
                title: '报销单编号',
            }, {
                field: 'feearea',
                title: '费用归属区域',
            }, {
                field: 'feedetail',
                title: '费用明细'
            }, {
                field: 'money',
                title: '金额',
            }, {
                field: 'beginDate',
                title: '开始时间',
            }, {
                field: 'endDate',
                title: '结束时间'
            }, {
                field: 'remark',
                title: '备注',
            }, {
                field: 'status',
                title: '审核情况',
            }],
            data: JSON.parse(data)
        })
    }

    var loadData = function () {
        $('#tb_departments').bootstrapTable({
            url: 'FinancialItemSetting.aspx',            //请求后台的URL（*）
            method: 'post',                      //请求方式（*）
            //toolbar: '#toolbar',                //工具按钮用哪个容器
            //striped: true,                      //是否显示行间隔色
            contentType: "application/x-www-form-urlencoded",
            queryParams: {
                act: 'getData',
                sector: $(".selectpicker").selectpicker("val")
            },
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            //pagination: false,                   //是否显示分页（*）
            sortable: false,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            //queryParams: oTableInit.queryParams,//传递参数（*）
            //: "server",           //分页方式：client客户端分页，server服务端分页（*）
            //pageNumber: 1,                       //初始化加载第一页，默认第一页
            //pageSize: 10,                       //每页的记录行数（*）
            columns: [{
                field: 'index',
                title: '序号',
            }, {
                field: 'item',
                title: '科目'
            }, {
                field: 'num',
                title: '数据',
                class: 'financialItem',
                formatter: function (value, row, index) {
                    return '<a onclick="sss(\'' + row.item + '\',' + row.num + ','+ index + ')" class="btn btn- primary btn- lg" data-toggle="modal" data-target="#myModal">' + value +'</a >';
                }
                //editable: {
                //    type: 'text',
                //    title: '数据',
                //    validate: function (v) {
                //        if (!v)
                //            return '数据不能为空';
                //    }
                //}
            }],
            onLoadSuccess: function () {
                $('[data-toggle="popover"]').popover();
            }
            //onEditableSave: function (field, row, oldValue, $el) {
            //    if (type == 3 || type == 4) {
            //        row.num = row.num + oldValue;
            //    }
            //    var type = row.index;
            //    $.ajax({
            //        url: "FinancialItemSetting.aspx",
            //        data: { act: 'saveData', year: date.getFullYear(), month: date.getMonth() + 1, num: row.num, type: type },
            //        type: "post",
            //        dataType: "json",
            //        success: function (res) {
            //            $('#tb_departments').bootstrapTable('refresh')
            //        },
            //        error: function (res) {
            //            $('#tb_departments').bootstrapTable('refresh')
            //        }
            //    });
            //}
        });
    }

    $(function () {
        showSector();
        changeSector();
        closeModal();
        var oFileInput = new FileInput();
        oFileInput.Init("txt_file", "FinancialItemSetting.aspx");
    });
    var type = 0;
    var sss = function (item, num, index) {
        $("#new_num").val("");
        type = index + 1;
        $("#myModalLabel").html(item);
        if (index != 2 && index != 3) {
            $("#aggre_num_div").css("display", "none");
        } else {
            $("#aggre_num").val(num);
            $("#aggre_num_div").css("display", "block");
        }
    }

    var submit = function(){
        var num = 0;
        var new_num = parseFloat($("#new_num").val());
        var aggre_num = parseFloat($("#aggre_num").val());
        if ($("#aggre_num_div").css("display") == 'block') {
            num = new_num + aggre_num;
        } else {
            num = new_num;
        }
        $.ajax({
            url: "FinancialItemSetting.aspx",
            data: {
                act: 'saveData',
                year: date.getFullYear(),
                month: date.getMonth() + 1,
                num: num,
                type: type,
                sector: $(".selectpicker").selectpicker("val")
            },
            type: "post",
            dataType: "json",
            success: function (res) {
                $('#tb_departments').bootstrapTable('refresh');
                $("#myModal").modal("hide");
            },
            error: function (res) {
                $('#tb_departments').bootstrapTable('refresh');
                $("#myModal").modal("hide");
            }
        });
    }

    var FileInput = function () {
        var oFile = new Object();

        //初始化fileinput控件（第一次初始化）
        oFile.Init = function (ctrlName, uploadUrl) {
            //记录GUID
            //$("#AttachGUID").val(newGuid());
            $("#txt_file").fileinput({
                uploadUrl: "FinancialItemSetting.aspx",//上传的地址
                uploadExtraData: { act: "importExcel", year: date.getFullYear(),month: date.getMonth() + 1},
                uploadAsync: true, //异步上传
                language: "zh",  //设置语言
                showCaption: true, //是否显示标题
                showUpload: true, //是否显示上传按钮
                showRemove: true, //是否显示移除按钮
                showPreview: true, //是否显示预览按钮
                browseClass: "btn btn-primary", //按钮样式
                dropZoneEnabled: false, //是否显示拖拽区域
                allowedFileExtensions: ["xls", "xlsx"], //接收的文件后缀
                maxFileCount: 1,  //最大上传文件数限制
                previewFileIcon: '<i class="glyphicon glyphicon-file"></i>',
                allowedPreviewTypes: null,
                previewFileIconSettings: {
                    'docx': '<i class="glyphicon glyphicon-file"></i>',
                    'xlsx': '<i class="glyphicon glyphicon-file"></i>',
                    'pptx': '<i class="glyphicon glyphicon-file"></i>',
                    'jpg': '<i class="glyphicon glyphicon-picture"></i>',
                    'pdf': '<i class="glyphicon glyphicon-file"></i>',
                    'zip': '<i class="glyphicon glyphicon-file"></i>',
                }
            })
            //文件预览时加载表格
            //$("#txt_file").on('fileloaded', function (event, file, previewId, index, reader) {
            //    count++;
            //    var formData = new FormData();
            //    formData.append("file", file);
            //    formData.append("act", "getExcel");
            //    $.ajax({
            //        url: "FinancialItemSetting.aspx",
            //        type: "post",
            //        contentType: false,
            //        processData: false,
            //        data: formData,
            //        success: function (res) {
            //            if (res == "excel文件有误，请使用标准格式上传") {
            //                bootbox.alert(res);
            //            } else {
            //                $("#ExcelModal .modal-content").css("height", 1250);
            //                if (count > 1) {
            //                    $('#gridImport').bootstrapTable('load', JSON.parse(res))
            //                } else {
            //                    InitImport(res);
            //                }
            //            }
            //        }
            //    });
            //});
            $('#txt_file').on('fileclear', function (event) {
                $('#gridImport').bootstrapTable('removeAll')
                //$("#ExcelModal .modal-content").css("height", originHeight);
            });
            $("#txt_file").on("fileuploaded", function (event, data, previewId, index) {
                var errorList = data.response.errorList;
                if (errorList.length == 0) {
                    bootbox.alert("上传成功");
                } else {
                    var errorMsgs = "";
                    for (i = 0; i < errorList.length; i++) {
                        errorMsgs += errorList[i] + "<br/>";
                    }
                    bootbox.alert(errorMsgs);
                }
            });
        };
           
        return oFile;
    };

    var showSector = function () {
        $.ajax({
            url: "FinancialItemSetting.aspx",
            data: { act:"getSector"},
            dataType: "json",
            type: "post",
            success: function (res) {
                var str = "";
                for (i = 0; i < res.length; i++) {
                    str += '<option>' + res[i].sector + '</option>'
                }
                $(".selectpicker").html(str);
                $(".selectpicker").selectpicker('refresh');

                loadData();
            },
            error: function (res) {
                console.log(res+"222")
            }
        })
    }

    var changeSector = function () {
        $('.selectpicker').on('change.bs.select', function (e) {
            var _sector = e.currentTarget.value;
            $('#tb_departments').bootstrapTable('refresh', {
                query: {
                    act: 'getData',
                    sector: _sector
                },})
        });
    }

    var closeModal = function () {
        $('#ExcelModal').on('hidden.bs.modal', function (e) {
            $('#tb_departments').bootstrapTable('refresh');
        })
    }
</script>
</html>