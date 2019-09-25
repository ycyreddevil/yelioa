

//////////////////////////////////Dialoge
function onOpenEventHandler(dlgId){
    var w = $(window).width() - 80;
    var h = $(window).height() - 80;
    $('#'+dlgId).dialog('resize', { width: w, height: h });
    $('#'+dlgId).dialog('move', { left: 40, top: 40 });    
}


///////////////////////////////////////////////Excel导入模块
var TotalImportNumber=0,ImportedNumber=0;
function ExcelDialogInit(dlgId,ImportDgId){
    var dlg = $('#'+dlgId);
    var ImportDg = $('#'+ImportDgId);
    dlg.dialog({
        buttons: [{
            text: '导入',
            iconCls: 'icon-leaveStock', size: 'large',
            handler: function () {
                //uploadFiles('import');
                UploadIndex = 0;
                $('#dg-Import').datagrid('selectRow', UploadIndex);                          
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

function uploadRowData(Url,dgId,index){
    var dg = $('#'+dgId);
    var datas = dg.datagrid('getData');
    if(index >= datas.length){
        return;
    }
    var row = datas.rows[index];
    if(row['状态'] == '已导入'){
        var jsonStr = JSON.stringify(row);
        var data = { act: 'import', json: jsonStr };
        $.post(Url, data, function (res) {
            if(res != "F"){
                var newRow = $.parseJSON(res);
                dg.datagrid('deleteRow', index);
                dg.datagrid('insertRow', {
                    index: index,	// index start with 0
                    row: newRow
                });  
            }           
                     
            uploadRowData(Url,dgId,index+1);
        });
        dg.datagrid('selectRow',index); 
    }    
    else{
        dg.datagrid('selectRow',index); 
        uploadRowData(Url,dgId,index+1);
    }
}

//////////////////////////////////////////////////////////////////////Excel 导出模块

function ExportToExcel(fileName, title, datagridId) {
    var datagrid = $("#" + datagridId);
    var datas = datagrid.datagrid('getData');
    var rows;
    if("originalRows" in datas){
        if (datas.originalRows.length == 0) {
            $.messager.alert('无数据！');
            return;
        }
        rows = datas.originalRows;
    }
    else{
        if (datas.total == 0) {
            $.messager.alert('无数据！');
            return;
        }
        rows = datas.rows;
    }
    

    var columns = datagrid.datagrid('options').columns;
    var ColumnTitles = new Array();
    var ColumnNames = new Array();
    $.each(columns[0], function (index, item) {
        if (item.title != '复选框') {
            ColumnTitles.push(item.title);
            ColumnNames.push(item.field);
        }
    });
    var headText = JSON.stringify(ColumnTitles);
    var columnName = JSON.stringify(ColumnNames);
    var dataJson = JSON.stringify(rows);

    $.post(
        "ExportExcelHelper.aspx", {
            fileName: fileName,
            data: dataJson,
            title: title,
            headText: headText,
            columnName:columnName
        }, function (resJson) {
            var res = $.parseJSON(resJson); 
            if (res.success == 0){
                $.messager.alert(res);
            }
            else if(res.success == 1){
                window.location.href = 'ExportExcelHelper.aspx?fileName=' + fileName + '.xls&fileCode=' + res.fileCode;
            }
        }
    );
    


    
}


////////////////////////////////////////////////////////easyui datagrid 前端分页
function DataGridPagerFilter(data) {

    if (typeof data.length == 'number' && typeof data.splice == 'function') { //判断数据是否是数组
        data = {
            total: data.length,
            rows: data
        }
    }
    let dg = $(this);
    let opts = dg.datagrid('options');
    let pager = dg.datagrid('getPager');
    pager.pagination({
        beforePageText: "页",
        afterPageText: "页，共{pages}页",
        displayMsg: "显示{from}到{to}，共{total}条记录",
        onSelectPage: function (pageNum, pageSize) {
            opts.pageNumber = pageNum;
            opts.pageSize = pageSize;
            pager.pagination('refresh', {
                pageNumber: pageNum,
                pageSize: pageSize
            });
            dg.datagrid('loadData', data);
        }
    });
    if (!data.originalRows) {
        data.originalRows = (data.rows);
    }
    let start = (opts.pageNumber - 1) * parseInt(opts.pageSize);
    let end = start + parseInt(opts.pageSize);
    data.rows = (data.originalRows.slice(start, end));
    return data;
}

