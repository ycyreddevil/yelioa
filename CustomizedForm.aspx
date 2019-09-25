<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomizedForm.aspx.cs" Inherits="CustomizedForm" %>
<!DOCTYPE HTML>
<html>
 <head>
  <title>自定义表单设计</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="author" content="leipi.org">
    <link href="Scripts/Public/css/bootstrap/css/bootstrap.css?2024" rel="stylesheet" type="text/css" />
    <link href="Scripts/Public/css/site.css?2024" rel="stylesheet" type="text/css" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link href="Scripts/themes/mobile.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script type="text/javascript">
        var _root = 'http://formbuild/index.php?s=/', _controller = 'index';
    </script>
<style>
      #components{
        min-height: 600px;
      }
      #target{
        min-height: 200px;
        border: 1px solid #ccc;
        padding: 5px;
      }
      #target .component{
        border: 1px solid #fff;
      }
      #temp{
        width: 500px;
        background: white;
        border: 1px dotted #ccc;
        border-radius: 10px;
      }

      .popover-content form {
        margin: 0 auto;
        width: 213px;
      }
      .popover-content form .btn{
        margin-right: 10px
      }
      #source{
        min-height: 500px;
      }
    </style>
 </head>
<body>

<div class="container">
<%-- $1$   <div class="row clearfix"> #1# --%>
<%-- $1$     <div class="span6"> #1# --%>
<%-- $1$       <div class="clearfix"> #1# --%>
<%-- $1$         <h2>我的表单</h2> #1# --%>
<%-- $1$         <hr> #1# --%>
<%-- $1$         <div id="build"> #1# --%>
<%-- $1$           <form id="target" class="form-horizontal"> #1# --%>
<%-- $1$             <fieldset> #1# --%>
<%-- $1$               <div id="legend" class="component" rel="popover" title="编辑属性" trigger="manual" #1# --%>
<%-- $1$                 data-content=" #1# --%>
<%-- $1$                 <form class='form'> #1# --%>
<%-- $1$                   <div class='controls'> #1# --%>
<%-- $1$                     <label class='control-label'>表单名称</label> <input type='text' id='orgvalue' placeholder='请输入表单名称'> #1# --%>
<%-- $1$                     <hr/> #1# --%>
<%-- $1$                     <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$                   </div> #1# --%>
<%-- $1$                 </form>"> #1# --%>
<%-- $1$                  #1# --%>
<%-- $1$                 <input type="hidden" name="form_name" value="" class="leipiplugins" leipiplugins="form_name"/> #1# --%>
<%-- $1$                 <legend class="leipiplugins-orgvalue" style="text-align:center">点击填写表单名</legend> #1# --%>
<%-- $1$               </div> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$               <!-- 编辑的时候 在这里新增已有的数据--> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$             </fieldset> #1# --%>
<%-- $1$           </form> #1# --%>
<%-- $1$           <a class="btn blue left" id="saveForm" href="#">保存</a> #1# --%>
<%-- $1$           <a class="btn  left" id="back" href="#">返回</a> #1# --%>
<%-- $1$         </div> #1# --%>
<%-- $1$       </div> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$     <div class="span6"> #1# --%>
<%-- $1$         <h2>拖拽下面的控件到左侧</h2> #1# --%>
<%-- $1$         <hr> #1# --%>
<%-- $1$       <div class="tabbable"> #1# --%>
<%-- $1$         <ul class="nav nav-tabs" id="navtab"> #1# --%>
<%-- $1$           <li class="active"><a href="#1" data-toggle="tab">常用控件</a></li> #1# --%>
<%-- $1$         </ul> #1# --%>
<%-- $1$         <form class="form-horizontal" id="components"> #1# --%>
<%-- $1$           <fieldset> #1# --%>
<%-- $1$             <div class="tab-content"> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$               <div class="tab-pane active" id="1"> #1# --%>
<%-- $1$ <!-- Text start --> #1# --%>
<%-- $1$ <div class="control-group component" rel="popover" title="文本框控件" trigger="manual" #1# --%>
<%-- $1$   data-content=" #1# --%>
<%-- $1$   <form class='form'> #1# --%>
<%-- $1$     <div class='controls'> #1# --%>
<%-- $1$       <label class='control-label'>控件名称</label> <input type='text' id='orgname' placeholder='必填项'> #1# --%>
<%-- $1$       <label class='control-label'>默认值</label> <input type='text' id='orgvalue' placeholder='默认值'> #1# --%>
<%-- $1$       <label class='control-label'>必填</label><select id='isnecessary'><option >是</option><option>否</option></select> #1# --%>
<%-- $1$       <label class='control-label'>长度</label><input type='text' id='length' placeholder='长度'> #1# --%>
<%-- $1$       <hr/> #1# --%>
<%-- $1$       <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$   </form>" #1# --%>
<%-- $1$   > #1# --%>
<%-- $1$   <!-- Text --> #1# --%>
<%-- $1$   <label class="control-label leipiplugins-orgname">文本框控件</label> #1# --%>
<%-- $1$   <div class="controls"> #1# --%>
<%-- $1$     <input name="leipiNewField" type="text" placeholder="默认值" title="文本框控件" value="" class="leipiplugins" leipiplugins="text"/> #1# --%>
<%-- $1$   </div> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ </div> #1# --%>
<%-- $1$ <!-- Text end --> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ <!-- Textarea start -->            #1# --%>
<%-- $1$ <div class="control-group component" rel="popover" title="多行文本控件" trigger="manual" #1# --%>
<%-- $1$   data-content=" #1# --%>
<%-- $1$   <form class='form'> #1# --%>
<%-- $1$     <div class='controls'> #1# --%>
<%-- $1$       <label class='control-label'>控件名称</label> <input type='text' id='orgname' placeholder='必填项'> #1# --%>
<%-- $1$       <label class='control-label'>默认值</label> <input type='text' id='orgvalue' placeholder='默认值'> #1# --%>
<%-- $1$       <label class='control-label'>必填</label><select id='isnecessary'><option>是</option><option>否</option></select> #1# --%>
<%-- $1$       <label class='control-label'>长度</label><input type='text' id='length' placeholder='长度'> #1# --%>
<%-- $1$       <hr/> #1# --%>
<%-- $1$       <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$   </form>" #1# --%>
<%-- $1$   > #1# --%>
<%-- $1$   <!-- Textarea --> #1# --%>
<%-- $1$   <label class="control-label leipiplugins-orgname">多行文本控件</label> #1# --%>
<%-- $1$   <div class="controls"> #1# --%>
<%-- $1$     <textarea title="多行文本控件" placeholder="默认值" name="leipiNewField" class="leipiplugins" leipiplugins="textarea"/> </textarea> #1# --%>
<%-- $1$   </div> #1# --%>
<%-- $1$ </div> #1# --%>
<%-- $1$ <!-- Textarea end --> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ <!-- Select start --> #1# --%>
<%-- $1$ <div class="control-group component" rel="popover" title="下拉控件" trigger="manual" #1# --%>
<%-- $1$   data-content=" #1# --%>
<%-- $1$   <form class='form'> #1# --%>
<%-- $1$     <div class='controls'> #1# --%>
<%-- $1$       <label class='control-label'>控件名称</label> <input type='text' id='orgname' placeholder='必填项'> #1# --%>
<%-- $1$       <label class='control-label'>必填</label><select id='isnecessary'><option>是</option><option>否</option></select> #1# --%>
<%-- $1$       <label class='control-label'>下拉选项</label> #1# --%>
<%-- $1$       <textarea style='min-height: 200px' id='orgvalue'></textarea> #1# --%>
<%-- $1$       <p class='help-block'>一行一个选项</p> #1# --%>
<%-- $1$       <hr/> #1# --%>
<%-- $1$       <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$   </form>" #1# --%>
<%-- $1$   > #1# --%>
<%-- $1$   <!-- Select --> #1# --%>
<%-- $1$   <label class="control-label leipiplugins-orgname">下拉控件</label> #1# --%>
<%-- $1$   <div class="controls"> #1# --%>
<%-- $1$     <select name="leipiNewField" title="下拉控件" class="leipiplugins" leipiplugins="select"> #1# --%>
<%-- $1$       <option>选项一</option> #1# --%>
<%-- $1$       <option>选项二</option> #1# --%>
<%-- $1$       <option>选项三</option> #1# --%>
<%-- $1$     </select> #1# --%>
<%-- $1$   </div> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ </div> #1# --%>
<%-- $1$ <!-- Select end --> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ <!-- Select start --> #1# --%>
<%-- $1$ $1$<div class="control-group component" rel="popover" title="多选下拉控件" trigger="manual" #1# --%>
<%-- $1$   data-content=" #1# --%>
<%-- $1$   <form class='form'> #1# --%>
<%-- $1$     <div class='controls'> #1# --%>
<%-- $1$       <label class='control-label'>控件名称</label> <input type='text' id='orgname' placeholder='必填项'> #1# --%>
<%-- $1$       <label class='control-label'>下拉选项</label> #1# --%>
<%-- $1$       <textarea style='min-height: 200px' id='orgvalue'></textarea> #1# --%>
<%-- $1$       <p class='help-block'>一行一个选项</p> #1# --%>
<%-- $1$       <hr/> #1# --%>
<%-- $1$       <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$   </form>" #1# --%>
<%-- $1$   > #1# --%>
<%-- $1$   <!-- Select --> #1# --%>
<%-- $1$   <label class="control-label leipiplugins-orgname">多选下拉控件</label> #1# --%>
<%-- $1$   <div class="controls"> #1# --%>
<%-- $1$     <select multiple="multiple" name="leipiNewField" title="多选下拉控件" class="leipiplugins" leipiplugins="select"> #1# --%>
<%-- $1$       <option>选项一</option> #1# --%>
<%-- $1$       <option>选项二</option> #1# --%>
<%-- $1$       <option>选项三</option> #1# --%>
<%-- $1$       <option>选项四</option> #1# --%>
<%-- $1$     </select> #1# --%>
<%-- $1$   </div> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ </div>#1# --%> --%>
<%-- $1$ <!-- Select end --> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ <!-- Multiple Checkboxes start --> #1# --%>
<%-- $1$ <div class="control-group component" rel="popover" title="复选控件" trigger="manual" #1# --%>
<%-- $1$   data-content=" #1# --%>
<%-- $1$   <form class='form'> #1# --%>
<%-- $1$     <div class='controls'> #1# --%>
<%-- $1$       <label class='control-label'>控件名称</label> <input type='text' id='orgname' placeholder='必填项'> #1# --%>
<%-- $1$       <label class='control-label'>必填</label><select id='isnecessary'><option>是</option><option>否</option></select> #1# --%>
<%-- $1$       <label class='control-label'>复选框</label> #1# --%>
<%-- $1$       <textarea style='min-height: 200px' id='orgvalue'></textarea> #1# --%>
<%-- $1$       <p class='help-block'>一行一个选项</p> #1# --%>
<%-- $1$       <hr/> #1# --%>
<%-- $1$       <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$   </form>" #1# --%>
<%-- $1$   > #1# --%>
<%-- $1$   <label class="control-label leipiplugins-orgname">复选框</label> #1# --%>
<%-- $1$   <div class="controls leipiplugins-orgvalue"> #1# --%>
<%-- $1$     <!-- Multiple Checkboxes --> #1# --%>
<%-- $1$     <label class="checkbox inline"> #1# --%>
<%-- $1$       <input type="checkbox" name="leipiNewField" title="复选框" value="选项1" class="leipiplugins" leipiplugins="checkbox" orginline="inline"> #1# --%>
<%-- $1$       选项1 #1# --%>
<%-- $1$     </label> #1# --%>
<%-- $1$     <label class="checkbox inline"> #1# --%>
<%-- $1$       <input type="checkbox" name="leipiNewField" title="复选框" value="选项2" class="leipiplugins" leipiplugins="checkbox" orginline="inline"> #1# --%>
<%-- $1$       选项2 #1# --%>
<%-- $1$     </label> #1# --%>
<%-- $1$   </div> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ </div> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ $1$<div class="control-group component" rel="popover" title="复选控件" trigger="manual" #1# --%>
<%-- $1$   data-content=" #1# --%>
<%-- $1$   <form class='form'> #1# --%>
<%-- $1$     <div class='controls'> #1# --%>
<%-- $1$       <label class='control-label'>控件名称</label> <input type='text' id='orgname' placeholder='必填项'> #1# --%>
<%-- $1$       <label class='control-label'>复选框</label> #1# --%>
<%-- $1$       <textarea style='min-height: 200px' id='orgvalue'></textarea> #1# --%>
<%-- $1$       <p class='help-block'>一行一个选项</p> #1# --%>
<%-- $1$       <hr/> #1# --%>
<%-- $1$       <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$   </form>" #1# --%>
<%-- $1$   > #1# --%>
<%-- $1$   <label class="control-label leipiplugins-orgname">复选框</label> #1# --%>
<%-- $1$   <div class="controls leipiplugins-orgvalue"> #1# --%>
<%-- $1$     <!-- Multiple Checkboxes --> #1# --%>
<%-- $1$     <label class="checkbox"> #1# --%>
<%-- $1$       <input type="checkbox" name="leipiNewField" title="复选框" value="选项1" class="leipiplugins" leipiplugins="checkbox"> #1# --%>
<%-- $1$       选项1 #1# --%>
<%-- $1$     </label> #1# --%>
<%-- $1$     <label class="checkbox"> #1# --%>
<%-- $1$       <input type="checkbox" name="leipiNewField" title="复选框" value="选项2" class="leipiplugins" leipiplugins="checkbox"> #1# --%>
<%-- $1$       选项2 #1# --%>
<%-- $1$     </label> #1# --%>
<%-- $1$   </div> #1# --%>
<%-- $1$ </div>#1# --%> --%>
<%-- $1$ <!-- Multiple Checkboxes end --> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ <!-- Multiple radios start --> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ <!-- 文件上传--> #1# --%>
<%-- $1$ <div class="control-group component" rel="popover" title="文件上传" trigger="manual" #1# --%>
<%-- $1$     data-content=" #1# --%>
<%-- $1$     <form class='form'> #1# --%>
<%-- $1$     <div class='controls'> #1# --%>
<%-- $1$         <label class='control-label'>控件名称</label> <input type='text' id='orgname' placeholder='必填项'> #1# --%>
<%-- $1$         <label class='control-label'>必填</label><select id='isnecessary'><option>是</option><option>否</option></select> #1# --%>
<%-- $1$         <hr/> #1# --%>
<%-- $1$         <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$     </form>" #1# --%>
<%-- $1$     > #1# --%>
<%-- $1$     <label class="control-label leipiplugins-orgname">文件上传</label> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$     <!-- File Upload --> #1# --%>
<%-- $1$     <div> #1# --%>
<%-- $1$     <input type="file" name="leipiNewField" title="文件上传" class="leipiplugins" leipiplugins="uploadfile"> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$ </div> #1# --%>
<%-- $1$ #1# --%>
<%-- $1$ $1$<div class="control-group component" rel="popover" title="单选控件" trigger="manual" #1# --%>
<%-- $1$   data-content=" #1# --%>
<%-- $1$   <form class='form'> #1# --%>
<%-- $1$     <div class='controls'> #1# --%>
<%-- $1$       <label class='control-label'>控件名称</label> <input type='text' id='orgname' placeholder='必填项'> #1# --%>
<%-- $1$       <label class='control-label'>单选框</label> #1# --%>
<%-- $1$       <textarea style='min-height: 200px' id='orgvalue'></textarea> #1# --%>
<%-- $1$       <p class='help-block'>一行一个选项</p> #1# --%>
<%-- $1$       <hr/> #1# --%>
<%-- $1$       <button class='btn btn-info' type='button'>确定</button><button class='btn btn-danger' type='button'>取消</button> #1# --%>
<%-- $1$     </div> #1# --%>
<%-- $1$   </form>" #1# --%>
<%-- $1$   > #1# --%>
<%-- $1$   <label class="control-label leipiplugins-orgname">单选</label> #1# --%>
<%-- $1$   <div class="controls leipiplugins-orgvalue"> #1# --%>
<%-- $1$     <!-- Multiple Checkboxes --> #1# --%>
<%-- $1$     <label class="radio"> #1# --%>
<%-- $1$       <input type="radio" name="leipiNewField" title="单选框" value="选项1" class="leipiplugins" leipiplugins="radio"> #1# --%>
<%-- $1$       选项1 #1# --%>
<%-- $1$     </label> #1# --%>
<%-- $1$     <label class="radio"> #1# --%>
<%-- $1$       <input type="radio" name="leipiNewField" title="单选框" value="选项2" class="leipiplugins" leipiplugins="radio"> #1# --%>
<%-- $1$       选项2 #1# --%>
<%-- $1$     </label> #1# --%>
<%-- $1$   </div> #1# --%>
<%-- $1$ </div>#1# --%> --%>
<%-- $1$ <!-- Multiple radios end --> #1# --%>
<%-- $1$               </div> #1# --%>
<%-- $1$             </fieldset> #1# --%>
<%-- $1$           </form> #1# --%>
<%-- $1$         </div><!--tab-content--> #1# --%>
<%-- $1$         </div><!---tabbable--> #1# --%>
<%-- $1$       </div> <!-- row --> #1# --%>
<%-- $1$        #1# --%>
     </div> <!-- /container -->

<script type="text/javascript" charset="utf-8" src="Scripts/Public/js/jquery-1.7.2.min.js?2024"></script>
<script type="text/javascript"  src="Scripts/Public/js/formbuild/bootstrap/js/bootstrap.min.js?2024"></script>
<script type="text/javascript" charset="utf-8" src="Scripts/Public/js/formbuild/leipi.form.build.core.js?2024"></script>
<script type="text/javascript" charset="utf-8" src="Scripts/Public/js/formbuild/leipi.form.build.plugins.js?2024"></script>
<script type="text/javascript">

</script>

<script>
    $(function () {
        var ifGetData = '<%=ifGetData%>';
        var formName = '<% =formName%>';
        if (ifGetData == 'true') {
            loadForm(formName);
        }
    })

    var itemNames = ""; var itemDefaultValues = ""; var extras = ""; var types = ""; var isnecessary = ""; var length = "";
    $("#saveForm").click(function () {
        var type = "";
        var chooseItem = $("#target").children().children(".control-group");
        var formTitle = $("#target").children().children(".component").children("legend").html();
        chooseItem.each(function (index, element){
            var itemType = $(element).attr("data-original-title");
            var itemName = $(element).children("label").html();
            var itemDefaultValue = $(element).children().children().val();
            var itemIsNecessary = $(element).children().children().attr("isnecessary");
            var itemLength = $(element).children().children().attr("length");

            itemNames += itemName + ";";
            itemDefaultValues += itemDefaultValue + ";";
            isnecessary += itemIsNecessary + ";";
            length += itemLength + ";";

            if ($(element).children().children("select").length > 0) {
                type = "select";
                var itemOption = $(element).children().children().children("option");
                var extraArray = new Array();
                itemOption.each(function (index, _inner_element) {
                    extraArray.push($(_inner_element).text());
                })
                extras += (extraArray.toString() + ";");
            } else if ($(element).children().children(".checkbox").length > 0) {
                type = "checkbox";
                var itemCheckbox = $(element).children().children("label");
                var extraArray = new Array();
                itemCheckbox.each(function (index, _inner_element) {
                    extraArray.push($(_inner_element).children().val());
                })
                extras += (extraArray.toString() + ";");
            } else if ($(element).children().children().attr("leipiplugins") == "uploadfile") {
                type = "file";
                extras += ";"
            } else {
                extras += ";"
            }
            if (type != "select" && type != "checkbox" && type != "file") {
                type = "input";
            }
            types += type + ";"
        })
        $.ajax({
            url: "CustomizedForm.aspx",
            data: {
                act: 'saveForm', formTitle: formTitle, itemNames: itemNames, length: length,
                itemDefaultValues: itemDefaultValues, extras: extras, types: types, isnecessary: isnecessary,
            },
            type: "post",
            dataType: "json",
            success: function (msg) {
                $.messager.alert('创建成功');
                location.href = "FormConfig.aspx";
            },
            error: function (msg) {
                $.messager.alert('创建失败，请稍后重试');
            }
        })
    })

    $("#back").click(function () {
        location.href = "FormConfig.aspx";
    })

    var loadForm = function (formName) {
        $.ajax({
            url: 'CustomizedForm.aspx',
            data: { act: 'loadForm', formName: formName},
            dataType: 'json',
            type: 'post',
            success: function (formDatas) {
                for (i = formDatas.length - 1; i >= 0; i--) {
                    var formData = formDatas[i];
                    var addDiv = "";
                    if (formData.type == 'input') {
                        addDiv = '<div class="control-group component" rel="popover" trigger="manual" data-content="'
                            + '<form class=\'form\'><div class=\'controls\'><label class=\'control-label\'>控件名称</label> <input type=\'text\' id=\'orgname\' placeholder=\'必填项\'><label class=\'control-label\'>默认值</label> <input type=\'text\' id=\'orgvalue\' placeholder=\'默认值\'>'
                            + '<label class=\'control-label\'>必填</label><select id=\'isnecessary\'><option >是</option><option>否</option></select><label class=\'control-label\'>长度</label><input type=\'text\' id=\'length\' placeholder=\'长度\'><hr />'
                            + '<button class=\'btn btn-info\' type=\'button\'>确定</button><button class=\'btn btn-danger\' type=\'button\'>取消</button></div></form>" data-original-title="' + formData.fieldName + '" style="border-top: 1px solid white; border-bottom: none;">'
                            + '<label class="control-label leipiplugins-orgname">' + formData.fieldName + '</label><div class="controls"><input name="leipiNewField" type="text" placeholder="默认值" title="' + formData.fieldName + '" value="' + formData.defaultValue + '" class="leipiplugins" leipiplugins="text" length="' + formData.length + '" isnecessary="' + formData.isNecessary + '"></div></div>';
                    } else if (formData.type == 'select') {
                        var extras = formData.extra.split(",");
                        addDiv = '<div class="control-group component" rel="popover" trigger="manual" data-content="'
                            + '<form class=\'form\'><div class=\'controls\'><label class=\'control-label\'>控件名称</label> <input type=\'text\' id=\'orgname\' placeholder=\'必填项\'>'
                            + '<label class=\'control-label\'>必填</label><select id=\'isnecessary\'><option>是</option><option>否</option></select><label class=\'control-label\'>下拉选项</label>'
                            + '<textarea style=\'min-height: 200px\' id=\'orgvalue\'></textarea><p class=\'help-block\'>一行一个选项</p><hr />'
                            + '<button class=\'btn btn-info\' type=\'button\'>确定</button><button class=\'btn btn-danger\' type=\'button\'>取消</button></div></form>" data-original-title="' + formData.fieldName + '" style="border-top: 1px solid white; border-bottom: none;">'
                            + '<label class="control-label leipiplugins-orgname">' + formData.fieldName + '</label><div class="controls"><select name="leipiNewField" title="下拉控件" class="leipiplugins" leipiplugins="select">'
                        for (j = 0; j < extras.length; j++) {
                            addDiv += '<option>' + extras[j] + '</option>';
                        }
                        addDiv += '</select></div></div>'

                        //addDiv = '<div class="control-group component" rel="popover" trigger="manual" data-content="'
                        //    + '<form class=\'form\'><div class=\'controls\'><label class=\'control-label\'>控件名称</label> <input type=\'text\' id=\'orgname\' placeholder=\'必填项\'><label class=\'control-label\'>默认值</label> <input type=\'text\' id=\'orgvalue\' placeholder=\'默认值\'>'
                        //    + '<label class=\'control-label\'>必填</label><select id=\'isnecessary\'><option >是</option><option>否</option></select><label class=\'control-label\'>长度</label><input type=\'text\' id=\'length\' placeholder=\'长度\'><hr />'
                        //    + '<button class=\'btn btn-info\' type=\'button\'>确定</button><button class=\'btn btn-danger\' type=\'button\'>取消</button></div></form>" data-original-title="' + formData.fieldName + '" style="border-top: 1px solid white; border-bottom: none;">'
                        //    + '<label class="control-label leipiplugins-orgname">' + formData.fieldName + '</label><div class="controls"><select name="leipiNewField" title="下拉控件" class="leipiplugins" leipiplugins="select">'
                        //for (j = 0; j < extras.length; j++) {
                        //    addDiv += '<option>' + extras[j] + '</option>';
                        //}
                        //addDiv += '</select></div></div>'
                    } else if (formData.type == 'checkbox'){
                        var extras = formData.extra.split(",");
                        addDiv = '<div class="control-group component" rel="popover" trigger="manual" data-content="'
                            + '<form class=\'form\'><div class=\'controls\'><label class=\'control-label\'>控件名称</label> <input type=\'text\' id=\'orgname\' placeholder=\'必填项\'>'
                            + '<label class=\'control-label\'>必填</label><select id=\'isnecessary\'><option>是</option><option>否</option></select><label class=\'control-label\'>复选框</label>'
                            + '<textarea style=\'min-height: 200px\' id=\'orgvalue\'></textarea><p class=\'help-block\'>一行一个选项</p><hr />'
                            + '<button class=\'btn btn-info\' type=\'button\'>确定</button><button class=\'btn btn-danger\' type=\'button\'>取消</button></div></form>" data-original-title="' + formData.fieldName + '" style="border-top: 1px solid white; border-bottom: none;">'
                            + '<label class="control-label leipiplugins-orgname">' + formData.fieldName + '</label><div class="controls leipiplugins-orgvalue">'
                        for (j = 0; j < extras.length; j++) {
                            addDiv += '<label class="checkbox inline">'
                                + '<input type= "checkbox" name= "leipiNewField" title= "复选框" value= "选项1" class="leipiplugins" leipiplugins= "checkbox" orginline= "inline" >'
                                + extras[j] + '</label>';
                        }
                        addDiv += '</div></div';

                        //    + '<form class=\'form\'><div class=\'controls\'><label class=\'control-label\'>控件名称</label> <input type=\'text\' id=\'orgname\' placeholder=\'必填项\'><label class=\'control-label\'>默认值</label> <input type=\'text\' id=\'orgvalue\' placeholder=\'默认值\'>'
                        //    + '<label class=\'control-label\'>必填</label><select id=\'isnecessary\'><option >是</option><option>否</option></select><label class=\'control-label\'>复选框</label><textarea style=\'min- height: 200px\' id=\'orgvalue\'></textarea><p class=\'help- block\'>一行一个选项</p><hr />'
                        //    + '<button class=\'btn btn-info\' type=\'button\'>确定</button><button class=\'btn btn-danger\' type=\'button\'>取消</button></div></form>" data-original-title="' + formData.fieldName + '" style="border-top: 1px solid white; border-bottom: none;">'
                        //    + '<label class="control-label leipiplugins-orgname">' + formData.fieldName + '</label><div class="controls leipiplugins-orgvalue">'
                        //for (j = 0; j < extras.length; j++) {
                        //    addDiv += '<label class="checkbox inline">'
                        //        + '<input type= "checkbox" name= "leipiNewField" title= "复选框" value= "选项1" class="leipiplugins" leipiplugins= "checkbox" orginline= "inline" >'
                        //        + extras[j] + '</label>';
                        //}
                        //addDiv += '</div></div';
                    } else if (formData.type == 'file') {
                        //addDiv = '<div><input type="file" name="leipiNewField" title="' + formData.fieldName + '" class="leipiplugins" leipiplugins="uploadfile" isnecessary="' + formData.isnecessary + '"></div>'

                        addDiv = '<div class="control-group component" rel="popover" trigger="manual" data-content="<form class=\'form\'>'
                            + '<div class=\'controls\' > <label class=\'control-label\'>控件名称</label> <input type=\'text\' id=\'orgname\' placeholder=\'必填项\'>'
                            + '<label class=\'control-label\'>必填</label><select id=\'isnecessary\'><option>是</option><option>否</option></select>'
                            + '<hr /><button class=\'btn btn-info\' type=\'button\'>确定</button><button class=\'btn btn-danger\' type=\'button\'>取消</button>'
                            + '</div></form > " data-original-title="文件上传"><label class="control-label leipiplugins-orgname">文件上传</label>'
                            + '<div><input type="file" name="leipiNewField" title="' + formData.fieldName + '" class="leipiplugins" leipiplugins="uploadfile" isnecessary="' + formData.isnecessary + '"></div></div >'

                    }
                    $("#legend").after(addDiv);
                }
            }
        })
    }
</script>
</body>
</html>