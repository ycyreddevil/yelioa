<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mFormDetail.aspx.cs" Inherits="mFormDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>业力表单</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link rel="stylesheet" href="Scripts/themes/mobile.css">
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link href="Scripts/themes/weui.min.css" rel="stylesheet"/>
    <link rel="stylesheet" href="https://unpkg.com/mint-ui/lib/style.css">
    <style>
        [v-cloak] {
            display: none;
        }
    </style>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
        background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog" border="false"
        noheader="true" closed="true" modal="true">
    </div>
    <div id="p1" class="easyui-navpanel" style="position:relative;padding:20px" data-options="footer:'#footer'" v-cloak>
    <header>
        <div class="m-toolbar" id="totaltitle">
            <span class="m-title" id="formName"></span>
            <a onclick="showPopup()" href="javascript:void(0)" v-show="hasDesc"></a>
            <div class="m-left" v-if="!isFreeProcess">
                <a id="getApprover" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="getProcessInfo()" style="width:100px">获取审批人</a>
            </div>
            <div class="m-left" v-else>
                <a id="getApprover" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="vue.isAddApprover=true;vue.showRemoteItems('用户表', this, '填表人', '','')" style="width:100px">添加审批人</a>
            </div>
            <div class="m-right">
                <a id="submit" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="submit()" style="width:60px">提交</a>
            </div>
            <%-- <mt-popup v-model="popupVisible" modal="false" popup-transition="popup-fade"> --%>
            <%--     <div> --%>
            <%--         <h3></h3>  --%>
            <%--     </div> --%>
            <%-- </mt-popup> --%>
        </div>
    </header>
    <div id="vm" >
        <form id="form" enctype="multipart/form-data" method="post">
            <ul class="m-list">
                <b>说明：{{formDesc}}</b>
                <br />
                <li>填表人:<label style="color:red">*</label><a id="submitter" name="userId" style="text-align:right;width:60%;float:right" href="javascript:void(0)" onclick="vue.showRemoteItems('用户表', this, '填表人', '','')">请选择</a></li>
                <li>所属部门:<label style="color:red">*</label><a id="department" name="departmentId" style="text-align:right;width:60%;float:right" href="javascript:void(0)" onclick="vue.showRemoteItems('部门表', this, '填表人部门', 'userId','用户表')">请选择</a></li>
                <li v-for="parameter in parameterData" v-show="parameter.TYP!='image' && parameter.TYP !='file'">
                    {{parameter.LBL}}:<label v-show="parameter.REQD!='0'" style="color:red">*</label>
                    <a v-if="parameter.TYP=='date'" style="text-align:right;width:80%;float:right" href="javascript:void(0)">
                        <input :name="parameter.LBL" type="date" style="width:40%;float:right;border:0px" />
                    </a>
                    <a v-if="parameter.TYP=='time'" style="text-align:right;width:80%;float:right" href="javascript:void(0)">
                        <input :name="parameter.LBL" type="time" style="width:40%;float:right;border:0px" />
                    </a>
                    <a v-if="parameter.TYP=='dropdown'" style="text-align:right;width:80%;float:right" href="javascript:void(0)">
                        <select class="easyui-combobox">
                            <option v-for="item in parameter.ITMS" :value="item.VAL">{{item.VAL}}</option>
                        </select>
                    </a>
                    <a v-else-if="parameter.TYP=='textarea'" style="text-align:right;width:80%;float:right" href="javascript:void(0)">
                        <textarea data-options="multiline:true,prompt:parameter.DEF" class="easyui-textbox" :name="parameter.LBL" style="width:90%;float:right;border:0px;height:300px"></textarea>
                    </a>
                    <a v-else-if="parameter.TYP=='radio'" :name="parameter.LBL" style="text-align:right;width:80%;float:right" href="javascript:void(0)" @click="showLocalItems(parameter.ITMS, $event, parameter.LBL,parameter.TYP)">请选择</a>
                    <a v-else-if="parameter.TYP=='checkbox'" :name="parameter.LBL" style="text-align:right;width:80%;float:right" href="javascript:void(0)" @click="showLocalItems(parameter.ITMS, $event, parameter.LBL)">请选择</a>
                    <a v-else-if="parameter.TYP=='number'" style="text-align:right;width:80%;float:right" href="javascript:void(0)">
                        <input type="number" :readonly="parameter.READONLY === '0' ? true : false" class="easyui-numberbox" :name="parameter.LBL" style="width:40%;float:right;border:0px" @focus="autoCalculate(parameter) "/>
                    </a>
                    <a v-else-if="parameter.TYP=='name'" :name="parameter.LBL" :tableName="parameter.RELA.RELA1.TABLENM" style="text-align:right;width:80%;float:right" href="javascript:void(0)" @click="showRemoteItems(parameter.RELA.RELA1.TABLENM, $event, parameter.LBL, parameter.RELA.RELA1.RELANM, parameter.RELA.RELA1.RELATABLE)">请选择</a>
                    <a v-else-if="parameter.TYP=='text'" style="text-align:right;width:80%;float:right" href="javascript:void(0)">
                        <input class="easyui-textbox" :name="parameter.LBL" style="width:40%;float:right;border:0px"/>
                    </a>
                </li>
                 <li>抄送人:<a id="informer" name="informer" style="text-align:right;width:60%;float:right" href="javascript:void(0)" onclick="vue.showRemoteItems('用户表', this, '抄送人', '','')">请选择</a></li>
            </ul>
        </form>
        <div class="weui-gallery" id="gallery" v-show="hasImage">  
            <span class="weui-gallery__img" id="galleryImg"></span>  
            <div class="weui-gallery__opr">  
                <a href="javascript:" class="weui-gallery__del">  
                    <i class="weui-icon-delete weui-icon_gallery-delete"></i>  
                </a>  
            </div>  
        </div>  
        <div class="weui-cells weui-cells_form" v-show="hasImage">  
            <div class="weui-cell">  
                <div class="weui-cell__bd">  
                    <div class="weui-uploader">  
                        <div class="weui-uploader__hd">  
                            <p class="weui-uploader__title" style="font-size:10px">{{imageName}}:<label v-show="imageNecessary == '1'" style="color:red">*</label></p>
                        </div>  
                        <div class="weui-uploader__bd">  
                            <ul class="weui-uploader__files" id="uploaderFiles">  
                                  
                            </ul>  
                            <div class="weui-uploader__input-box">  
                                <input id="uploaderInput" name="uploaderInput" class="weui-uploader__input zjxfjs_file" type="file" accept="image/*" multiple="">  
                            </div>  
                        </div>  
                    </div>  
                </div>  
            </div>  
        </div>
        <!-- 文件上传-->
        <div id="fileDiv111" style="margin-left:10px" v-show="hasFile">
			<div><label v-show="fileNecessary == '1'" style="color:red">*</label>{{fileName}}:</div>
			<input class="easyui-filebox" name="file1" data-options="prompt:'请选择文件...'" style="width:300px">
		</div>
    </div>
    </div>  
    <div style="padding:30px 10px" id="footer">
        <a id="sumbitter" href="#" class="easyui-linkbutton" data-options="iconAlign:'top',size:'small'">
            提交人
        </a>
        <span>-></span>
    </div>
    <div id="p2" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span id="p2-title" class="m-title">Detail</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" onclick="$.mobile.back()">返回</a>
                </div>
                <div class="m-right">
                    <a id="confirmRemoteData" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="confirmRemoteData()" style="width:60px">确定</a>
                </div>
            </div>
        </header>
        <div>
            <input id="search" class="easyui-textbox" style="width:100%;" data-options="prompt:'请输入中文或者首字母拼音进行搜索'">
            <div id="2222">
                <ul id="remoteDataList">
                </ul>
            </div>
          
            <ul id="tree" class="easyui-tree" data-options="cascadeCheck:false"></ul>
            
        </div>
    </div>
    <div id="p3" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span id="p3-title" class="m-title">Detail</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" onclick="$.mobile.back()">Back</a>
                </div>
                <div class="m-right">
                    <a id="confirmItems" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="confirmItems()" style="width:60px">确定</a>
                </div>
            </div>
        </header>
        <div>
            <ul id="itemList">
                
            </ul>
        </div>
    </div>
    <div id="p4" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span id="p4-title" class="m-title">Detail</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" onclick="$.mobile.back()">Back</a>
                </div>
            </div>
        </header>
        <div>
            <ul class="m-list" id="childrenFeeDetailList">
            </ul>
            <div id="dl2" style="height:500px" data-options="
                border: false,
                lines: true,
                singleSelect: false
                ">
            </div>
        </div>
    </div>
</body>
<script src="Scripts/jquery.min.js"></script>
<script src="Scripts/vue.js"></script>
<script src="Scripts/jquery.easyui.min.js"></script>
<script src="Scripts/jquery.easyui.mobile.js"></script>
<script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
<script src="Scripts/mobileCommon.js"></script>
<script src="Scripts/ajaxfileupload.js"></script>
<script src="https://unpkg.com/mint-ui/lib/index.js"></script>
<script src="Scripts/weui.min.js"></script>
<script src="Scripts/weui-upload.js"></script>
<script>
    var uploadFileUrls = new Array(); // 存图片
    var uploadNotImage = new Array(); // 存文件
    var vue = new Vue({
        el: '#p1',
        data: {
            formTitle: '',
            formDesc: '',
            parameterData: [],
            hasImage: false,
            chooseLocalItemName: '',
            chooseLocalItem: '',
            choosedLocalData: [],
            chooseRemoteItemName: '',
            chooseRemoteRelaTable: '',
            chooseRemoteRelaName: '',
            chooseRemoteItem: '',
            chooseDepartmentNode: [],
            messagebox: '',
            process: [],
            imageName: '',
            fileName: '',
            defaultInformer: [],
            hasGetApprover: false,
            popupVisible: false,
            hasDesc: false,
            imageNecessary: false,
            fileNecessary: false,
            isFreeProcess: false,
            isAddApprover: false,
            hasFile: false
        },
        methods: {
            showRemoteItems: function(tableName, thisItem, name, relaName, relaTable) {
                this.chooseRemoteItem = thisItem.currentTarget;
                if (this.chooseRemoteItem == "undefined" || typeof this.chooseRemoteItem == "undefined") {
                    this.chooseRemoteItem = thisItem;
                }
                this.chooseRemoteItemName = name;
                this.chooseRemoteRelaTable = relaTable;
                this.chooseRemoteRelaName = relaName;
                var remoteUrl = "";
                if (tableName == "用户表" && name != "抄送人") {
                    remoteUrl = "findUsers";
                } else if (tableName == "部门表") {
                    remoteUrl = "findDepartments";
                } else if (tableName == "产品表") {
                    remoteUrl = "findProducts";
                } else if (tableName == "费用明细表") {
                    remoteUrl = "findFeeDetails";
                } else if (tableName == "网点表") {
                    remoteUrl = "findBranches";
                } else if (tableName == "用户表" && name == "抄送人") {
                    tableName = "抄送人表";
                    remoteUrl = "findInformer";
                } else if (tableName == "项目表") {
                    remoteUrl = "findProjects";
                } else if (tableName == "代理商表") {
                    remoteUrl = "findAgent"
                }
                $("#p2-title").html(tableName);
                $("#search").textbox("setValue", "");

                var relaData = "";
                if (relaName != "") {
                    relaData = $("a[name='" + relaName + "']").attr("itemId");
                }

                if (relaName != "" &&
                    ($("a[name='" + relaName + "']").html() == "" || $("a[name='" + relaName + "']").html() == "请选择")) {
                    vue.messagebox.alert('请先填写' + name + "关联数据").then(action => {

                    });
                } else {
                    showRemoteData("", remoteUrl, relaData, relaTable);
                }

            },
            showLocalItems: function(data, thisItem, name, type) {
                $("#p3-title").html(name);
                this.chooseLocalItem = thisItem.currentTarget;
                this.chooseLocalItemName = name;
                $("#itemList").empty();
                var html = "";
                for (i = 0; i < data.length; i++) {
                    html += '<li>' + data[i].VAL + '</li>';
                }
                $("#itemList").append(html);

                var singleSelect = false;
                if (type == "radio") {
                    singleSelect = true;
                }

                $.mobile.go('#p3');

                $("#itemList").datalist({
                    checkbox: true,
                    fit: false,
                    lines: true,
                    border: false,
                    singleSelect: singleSelect,
                    //height: 600
                });

                // 判断是否需要回显
                $.each(this.choosedLocalData,
                    function(i, v) {
                        if (name == v.name) {
                            $.each(data,
                                function(i1, v1) {
                                    if (v.value.indexOf(v1.VAL + ",") >= 0) {
                                        $('#itemList').datalist('selectRow', i1);
                                    }
                                });
                        }
                    })
            },
            autoCalculate(data) {
                if (data.LBL === '合计金额') {
                    var 车船费 = parseFloat($("#form").find('input[name="车船费"]')[0].value)
                    var 过桥过路费 =  parseFloat($("#form").find('input[name="过桥过路费"]')[0].value)
                    var 住宿费 =  parseFloat($("#form").find('input[name="住宿费"]')[0].value)
                    var 出差补贴 =  parseFloat($("#form").find('input[name="出差补贴"]')[0].value)

                    $("#form").find('input[name="合计金额"]')[0].value = 车船费 + 过桥过路费 + 住宿费 + 出差补贴
                }
            }
        },
        created: function () {
            this.messagebox = this.$messagebox;
            var id = GetQueryString('id');
            var docId = GetQueryString('docId');
            var url = decodeURI(location.href);

            var tmp1 = url.split("?")[1];
            var tmp2 = tmp1.split("&")[0];
            var tmp3 = tmp2.split("=")[1];
            var formName = tmp3;

            setTimeout($(".mint-popup").css("border-radius", "8px"), 1000);
            findFormDetail(id, docId, formName);
        },
        mounted: function() {
        }
    });

    $(".easyui-filebox").filebox({
        onChange: function () {
            // 上传文件
            var file = $(".easyui-filebox").filebox('files')[0];
            if (file.size > 20 * 1024 * 1024) {
                $.messager.alert('提示', "无法上传超过20m的文件");
            } else {
                //调用ajaxfileupload上传文件
                $.ajaxFileUpload({
                    url: 'mFinanceReimburse.aspx',
                    type: "post",
                    secureuri: false,
                    fileElementId: "filebox_file_id_2",
                    dataType: "json",
                    data: { act: 'uploadReimburseImage' },
                    success: function (data) {
                        Loading(false)
                        if (data.status == "文件上传成功") {
                            uploadNotImage.push(data.filePath);
                            //下面显示文件名
                            var fileHtml = '<span style="margin-left:10px">已上传:' + file.name + '</span><button onclick="removeFile(this,\''+data.filePath+'\')">删除</button><br />';
                            $("#fileDiv111").after(fileHtml);
                        }
                        $.messager.alert('提示', data.status);
                    },
                    error: function (data) {
                        Loading(false)
                        $.messager.alert('提示', "上传失败");
                    }
                })
            }   
        }
    });

    function removeFile(a, path) {
        $(a).prev().remove();
        $(a).remove();

        uploadNotImage.splice(uploadNotImage.indexOf(path), 1);
    }

    $(document).keydown(function(event){
        if(event.keyCode == 13){
            var target = $('#p2-title').html();
            var url = "";
            if (target == '产品表') {
                url = "findProducts";
            } else if (target == "网点表") {
                url = "findBranches";
            } else if (target == "部门表") {
                url = "findDepartments";
            } else if (target == "抄送人表") {
                url = "findInformer";
            } else if (target == "代理商表") {
                url = "findAgent"
            }else {
                url = "findUsers";
            }
            var name = $("#search").textbox("getValue");
            var relaData = "";
            if (vue.chooseRemoteRelaName != "") {
                relaData = $("a[name='"+vue.chooseRemoteRelaName+"']").attr("itemId");
            }
            showRemoteData(name,url,relaData, vue.chooseRemoteRelaTable);
        }
    });

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    function showPopup() {
        vue.popupVisible = true;
    }

    function findFormDetail(id, docId, formName) {
        $.ajax({
            url: 'mFormDetail.aspx',
            data: { act: 'findFormById', id: id },
            dataType: 'json',
            type: 'post',
            success: function (data) {
                
                if (data.ErrCode == 0) {
                    var dataJosn = JSON.parse(data.data)[0];
                    if (dataJosn.FormName == "签呈表") {
                        vue.isFreeProcess = true;
                        $("#getApprover").children().children().html("添加审批人");
                        //$.parser.parse('#totaltitle')
                    }

                    vue.formTitle = dataJosn.FormName;
                    vue.parameterData = JSON.parse(dataJosn.ParameterData);

                    // 初始化人员和部门
                    $("#submitter").attr("itemId", dataJosn.userId);
                    $("#department").attr("itemId", dataJosn.departmentId);

                    $("#submitter").html(dataJosn.userName);
                    $("#department").html(dataJosn.department);

                    $("#formName").html(vue.formTitle);

                    // 判断表单有没有备注需要显示
                    var formData = JSON.parse(dataJosn.FormData);
                    if (formData.DESC != "") {
                        vue.hasDesc = true;
                        vue.formDesc = formData.DESC;
                    }

                    // 判断该表单需不需要上传图片或者上传文件控件
                    var parameterData = JSON.parse(dataJosn.ParameterData);
                    for (i = 0; i < parameterData.length; i++) {
                        if (parameterData[i].TYP == "image") {
                            vue.imageName = parameterData[i].LBL;
                            vue.hasImage = true;

                            if (parameterData[i].REQD == "1") {
                                vue.imageNecessary = true;
                            }
                            break;
                        }
                    }
                    for (i = 0; i < parameterData.length; i++) {
                        if (parameterData[i].TYP == "file") {
                            vue.fileName = parameterData[i].LBL;
                            vue.hasFile = true;

                            if (parameterData[i].REQD == "1") {
                                vue.fileNecessary = true;
                            }
                            break;
                        }
                    }

                    if (formName != "" && docId != "") {
                        // 草稿数据回显
                        showDraftData(docId, formName);
                    }
                } else {
                    alert("暂无对应表单");
                }

            }
        });
    }

    function showDraftData(id, formName) {
        $.ajax({
            url: 'mFormDetail.aspx',
            data: { act: 'showDraftData', id: id, formName: formName },
            dataType: 'json',
            type: 'post',
            success: function(data) {
                if (data.ErrCode == 0) {
                    var as = $("#form").find("a");
                    var _data = JSON.parse(data.data)[0];
                    for (var key in _data) {
                        $.each(as,
                            function(i, v) {
                                if ($(v).attr("name") + "1" == key || $(v).attr("name") == key) {
                                    $(v).html(_data[key]);
                                    $(v).attr("itemId", _data[$(v).attr("name")]);
                                }
                            });

                        var textareas = $("#form").find("textarea");
                        $.each(textareas,
                            function (i, v) {
                                if ($(v).attr("name") == key){
                                    $(v).val(_data[key]);
                                }
                            });

                        var inputs = $("#form").find("input");
                        $.each(inputs,
                            function(i, v) {
                                if ($(v).attr("name") == key) {
                                    // 日期格式要转换
                                    if (isDate(_data[key])) {
                                        var r = _data[key].split(" ")[0].split("/");
                                        var year = r[0];
                                        var month = r[1];
                                        var day = r[2];
                                        if (month.length < 2) {
                                            month = "0" + month;
                                        }
                                        if (day.length < 2) {
                                            day = "0" + day;
                                        }
                                        $(v).val(year + "-" + month + "-" + day);
                                    } else {
                                        $(v).val(_data[key]);
                                    }

                                    //$("input[name='" + $(v).attr("name") + "']").textbox("setValue", "123"), 2000
                                }
                            });
                    };
                    $.parser.parse('#form');
                }
            }
        });
    }

    function isDate(datestring){
        if (datestring.indexOf("/") != -1 && datestring.indexOf(":") != -1) {
            return true;
        }
        return false;
    } 

    function confirmItems() {
        var checkedItem = $("#itemList").datalist("getChecked");
        var itemStr = "";
        $.each(checkedItem, function (i,v) {
            itemStr += v.text + ","
        })
        itemStr = itemStr.substring(0, itemStr.length - 1);
        $(vue.chooseLocalItem).html(itemStr);
        $(vue.chooseLocalItem).attr("itemId", itemStr);

        // 记录值来使下次点击时能回显
        vue.choosedLocalData = [];
        var name = vue.chooseLocalItemName;
        var array = { name: name, value: itemStr};
        vue.choosedLocalData.push(array);

        $.mobile.back();
    }

    function showRemoteData(value, remoteUrl, relaData, relaTable) {
        $.mobile.go('#p2');
        var singleSelect = true;
        if (remoteUrl == "findInformer") {
            singleSelect = false;
            remoteUrl = "findUsers";
        }
        if (remoteUrl == "findDepartments" && relaTable == "") {
            $("#2222").hide();
            $('#tree').empty();
            $("#search").textbox('disable');
            //清空人员列表
            try {
                $("#remoteDataList").datagrid("loadData", { "total": 0, "rows": [] });
            } catch (err) {

            }
            $("#remoteDataList").empty();
            $("#childrenFeeDetailList").empty();
            initTree();
            TreeLoad();
        } else {
            $.ajax({
                url: 'mFormDetail.aspx',
                data: {
                    act: 'findRemoteUrl',
                    type: remoteUrl,
                    value: value,
                    relatedValue: relaData,
                    relatedType: relaTable
                },
                dataType: 'json',
                type: 'post',
                success: function (remoteData) {
                    if (remoteData.ErrCode == 4) {
                        vue.messagebox.alert('暂无数据').then(action => {

                        });
                    } else if (remoteData.ErrCode != 0) {
                        vue.messagebox.alert('获取数据失败').then(action => {
                            $.mobile.back();
                        });
                    } else {
                        //清空人员列表
                        try {
                            $("#remoteDataList").datagrid("loadData", { "total": 0, "rows": [] });
                        } catch (err) {

                        }
                        remoteData = JSON.parse(remoteData.data);
                        $("#childrenFeeDetailList").empty();

                        if (remoteUrl == "findFeeDetails") {
                            $("#2222").show();
                            $("#search").textbox('disable');
                            $('#tree').empty();
                            $("#confirmRemoteData").attr("disabled", true);

                            var html = "";
                            for (i = 0; i < remoteData.length; i++) {
                                html += '<li value='+remoteData[i].value+'>' + remoteData[i].target + '</li>';
                            }
                            $("#remoteDataList").empty();
                            $("#remoteDataList").append(html);

                            $("#remoteDataList").datalist({
                                checkbox: false,
                                fit: false,
                                lines: true,
                                border: false,
                                singleSelect: singleSelect,
                                //height: 700,
                                onClickRow: function (rowIndex, rowData) {
                                    setChildFeeDetailValue(rowData.text, rowData.value);
                                }
                            });
                        } else {
                            $("#2222").show();
                            $("#search").textbox('enable');
                            
                                $("#remoteDataList").empty();
                            
                            $('#tree').empty();

                            var html = "";
                            if (!singleSelect && $("#remoteDataList").html() == "") {
                                //// 取得选中的数据
                                try {
                                    var selectInform = $("#informer").html().split(",");
                                    var selectInformId = $("#informer").attr("itemId").split(",");
                                    // 添加回datalist中
                                    for (i = 0; i < selectInform.length; i++) {
                                        html += '<li value=' + selectInformId[i] + '>' + selectInform[i] + '</li>';
                                    }
                                } catch (err) {

                                }
                            }

                            if (relaTable != "" || value != "") {
                                for (i = 0; i < remoteData.length; i++) {
                                    html += '<li value=' + remoteData[i].value + '>' + remoteData[i].target + '</li>';
                                }
                                if (vue.formTitle.indexOf('网点') > -1) {
                                    html += '<li value="无">无</li>'
                                }
                            }

                            $("#remoteDataList").append(html);

                            $("#remoteDataList").datalist({
                                checkbox: true,
                                fit: false,
                                lines: true,
                                border: false,
                                singleSelect: singleSelect,
                                height: 700
                            });

                        }
                    }

                }
            });
        }
        
    }

    function setChildFeeDetailValue(data, itemId) {
        $("#childrenFeeDetailList").empty();
        $.ajax({
        url: 'mFinanceReimburse.aspx',
            data: { act: 'findChildrenFeeDetail', name: data, itemId:itemId},
            dataType: 'json',
            type: 'post',
            success: function (childrenData) {
                if (childrenData == "" || childrenData == null) {
                    $(vue.chooseRemoteItem).html(data);
                    $(vue.chooseRemoteItem).attr("itemId", itemId);
                    $.mobile.go('#p1');
                } else {
                    var html = "";
                    for (i = 0; i < childrenData.length; i++) {
                        html += '<li><a href="javascript:void(0)" onclick="setChildrenFeeDetailValue(\'' + childrenData[i].target + '\',\'' + data + '\',\''+ childrenData[i].id + '\')">' + childrenData[i].target + '</a></li>';
                    }
                    $("#p4-title").html(data);
                    $("#childrenFeeDetailList").append(html);
                    $.mobile.go('#p4');
                }
            }
        })
    }

    function setChildrenFeeDetailValue(data,parentData,itemId) {
        $(vue.chooseRemoteItem).html(parentData + '-' + data);
        $(vue.chooseRemoteItem).attr("itemId", itemId);
             
        $.mobile.go('#p1');
    }

    function TreeLoad(selectedId) {
        var url = "MemberManage.aspx";
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
            }
        });
    }

    function initTree() {
        $('#tree').tree({
            animate: true, lines: true,
            onClick: function (node) {
                //$(this).tree('beginEdit', node.target);//点击可编辑
                vue.chooseDepartmentNode = node
                $('#tree').tree('expand',node.target);
            //  $.mobile.back();
            },
            formatter: function (node) {
                var s = node.text;
                // s += '&nbsp;<span style=\'color:blue\'>(' + node.MemberNumber + ')</span>';
                return s;
            }
        });
    }

    $("#search").textbox({
        icons: [{
            iconCls: 'icon-search',
            handler: function (e) {
                var target = $('#p2-title').html();
                var url = "";
                if (target == '产品表') {
                    url = "findProducts";
                } else if (target == "网点表") {
                    url = "findBranches";
                } else if (target == "部门表") {
                    url = "findDepartments";
                } else if (target == "抄送人表") {
                    url = "findInformer";
                } else if (target == "代理商表") {
                    url = "findAgent"
                } else {
                    url = "findUsers";
                }
                var name = $("#search").textbox("getValue");
                var relaData = "";
                if (vue.chooseRemoteRelaName != "") {
                    relaData = $("a[name='" + vue.chooseRemoteRelaName + "']").attr("itemId");
                }
                showRemoteData(name, url, relaData, vue.chooseRemoteRelaTable);
            }
        }],
    })

    function confirmRemoteData() {
        if (!vue.isAddApprover) {
            if ($("#remoteDataList").children().length > 0) {
                var checkedItem = $("#remoteDataList").datalist("getChecked");
                var itemStr = "";
                var itemId = "";
                $.each(checkedItem, function (i, v) {
                    itemStr += v.text + ",";
                    itemId += v.value + ",";
                });
                itemStr = itemStr.substring(0, itemStr.length - 1);
                itemId = itemId.substring(0, itemId.length - 1);
                $(vue.chooseRemoteItem).html(itemStr);
                $(vue.chooseRemoteItem).attr("itemId", itemId);
            } else {
                $(vue.chooseRemoteItem).html(vue.chooseDepartmentNode.text);
                $(vue.chooseRemoteItem).attr("itemId", vue.chooseDepartmentNode.id);
            }
        } else {
            // 点击添加审批人的时候触发
            if (vue.isFreeProcess) {
                var checkedItem = $("#remoteDataList").datalist("getChecked");
                var itemStr = ''; var itemId = '';
                $.each(checkedItem, function (i, v) {
                    itemStr = v.text + ",";
                    itemId = v.value + ",";
                });
                itemStr = itemStr.substring(0, itemStr.length - 1);
                itemId = itemId.substring(0, itemId.length - 1);

                vue.process.push({level: vue.process.length+1, userId: itemId, name: itemStr})

                var html = '<a href="#" class="easyui-linkbutton" data-options="iconAlign:\'top\',size:\'small\'">';
                html += itemStr;
                html += "</a >";
                html += "<span>-></span>";
                $("#footer").append(html);
                $.parser.parse($("#footer"));
            
                //$("#footer").children("span:last").remove();

                vue.hasGetApprover = true;
                vue.isAddApprover = false;
                Loading(false);
            }
        }

        $.mobile.go('#p1');
    }

    function getProcessInfo() {
        var formData = new Array();
        var allRequired = true;
        var as = $("#form").find("a");

        $.each(as,
            function (i, v) {
                if ($(v).attr("name") != undefined || typeof $(v).attr("name") != "undefined") {
                    if (($(v).prev().length > 0 && $(v).prev().css("display") != "none") && typeof $(v).attr("itemId") == "undefined") {
                        allRequired = false;
                        return false;
                    }
                    var onedata = { name: $(v).attr("name"), value: $(v).attr("itemId") }
                    formData.push(onedata);
                }
            });

        if (allRequired) {
            var textareas = $("#form").find("textarea");
            $.each(textareas,
                function (i, v) {
                    if ($(v).attr("name") != undefined || typeof $(v).attr("name") != "undefined") {
                        if (($(v).prev().length > 0 || $(v).parent().prev().css("display") != "none") && $(v).val() == "") {
                            allRequired = false;
                            return false;
                        }
                        var onedata = { name: $(v).attr("name"), value: $(v).val() }
                        formData.push(onedata);
                    }
                });

            var inputs = $("#form").find("input");
            $.each(inputs,
                function (i, v) {
                    if ($(v).attr("name") != undefined || typeof $(v).attr("name") != "undefined") {
                        if (($(v).prev().length > 0 || $(v).parent().prev().css("display") != "none") && $(v).val() == "") {
                            allRequired = false;
                            return false;
                        }
                        var onedata = { name: $(v).attr("name"), value: $(v).val() }
                        formData.push(onedata);
                    }
                });
            if (!allRequired) {
                vue.messagebox.alert('存在必填项为空的选项!').then(action => {

                });
            } else {
                if (vue.imageNecessary && $("#uploaderFiles").html() == "") {
                    allRequired = false;
                    vue.messagebox.alert('存在必填项为空的选项!').then(action => {

                    });
                }
            }
        } else {
            vue.messagebox.alert('存在必填项为空的选项!').then(action => {
                   
            });
        }
        
        if (allRequired) {
            Loading(true);
            $.post('mFormDetail.aspx', {
                act: "getProcessInfo", formData: JSON.stringify(formData), formId: GetQueryString("id")
            },function (res) {
                var data = JSON.parse(res);
                if (data.ErrCode == 0) {
                    $("#footer").empty();             
                    var approval = JSON.parse(data.process);
                    vue.process = JSON.parse(data.process);
                    if(data.informer != "")
                        vue.defaultInformer = JSON.parse(data.informer);
                    for (var i = 0; i < approval.length; i++) {
                        html = '<a href="#" class="easyui-linkbutton" data-options="iconAlign:\'top\',size:\'small\'">';
                        html += approval[i].name;
                        html += "</a >";
                        html += "<span>-></span>";
                        $("#footer").append(html);
                        $.parser.parse($("#footer"));
                    }
                    $("#footer").children("span:last").remove();

                    vue.hasGetApprover = true;
                    Loading(false);
                }
            });
        }
    }

    function submit() {
        if (!vue.hasGetApprover) {
            vue.messagebox.alert('请先获取审批人!').then(action => {
                    
            });
            return;
        }

        var formData = new Array();
        var allRequired = true;

        var as = $("#form").find("a");

        $.each(as,
            function (i, v) {
                if ($(v).attr("name") != undefined || typeof $(v).attr("name") != "undefined") {
                    if (($(v).prev().length > 0 && $(v).prev().css("display") != "none") && typeof $(v).attr("itemId") == "undefined") {
                        allRequired = false;
                        return false;
                    }
                    var onedata = { name: $(v).attr("name"), value: $(v).attr("itemId") }
                    formData.push(onedata);
                }
            });

        if (allRequired) {
            var textareas = $("#form").find("textarea");
            $.each(textareas,
                function (i, v) {
                    if ($(v).attr("name") != undefined || typeof $(v).attr("name") != "undefined") {
                        if (($(v).prev().length > 0 || $(v).parent().prev().css("display") != "none") && $(v).val() == "") {
                            allRequired = false;
                            return false;
                        }
                        var onedata = { name: $(v).attr("name"), value: $(v).val() }
                        formData.push(onedata);
                    }
                });

            var inputs = $("#form").find("input");
            $.each(inputs,
                function (i, v) {
                    if ($(v).attr("name") != undefined || typeof $(v).attr("name") != "undefined") {
                        if (($(v).prev().length > 0 || $(v).parent().prev().css("display") != "none") && $(v).val() == "") {
                            allRequired = false;
                            return false;
                        }
                        var onedata = { name: $(v).attr("name"), value: $(v).val() }
                        formData.push(onedata);
                    }
                });
            if (!allRequired) {
                vue.messagebox.alert('存在必填项为空的选项!').then(action => {

                });
            } else {
                if (vue.imageNecessary && $("#uploaderFiles").html() == "") {
                    allRequired = false;
                    vue.messagebox.alert('存在必填项为空的选项!').then(action => {

                    });
                }
            }
        } else {
            vue.messagebox.alert('存在必填项为空的选项!').then(action => {
                   
            });
        }

        if (vue.hasImage) {
            var imageData = { name: vue.imageName, value: JSON.stringify(uploadFileUrls)};
            formData.push(imageData);
        }

        if (vue.hasFile) {
            var fileData = { name: vue.fileName, value: JSON.stringify(uploadNotImage) };
            formData.push(fileData);
        }

        if (vue.formTitle === '借款单') {
            let a = ''
            formData.forEach((v, i) => {
                if (v.name === '借款金额')
                    a = v.value
            })
            const remainAmount = { name: 'remainAmount', value: a }
            formData.push(remainAmount);
        }
        
        if (vue.formTitle === '差旅申请') {
            let a = ''
            let b = ''
            let c = ''
            formData.forEach((v, i) => {
                if (v.name === '交通工具')
                    a = v.value
                if (v.name === '车船费')
                    b = v.value
                if (v.name === '公里数')
                    c = v.value
            })
            if (a === '私车公用') {
                if (c === '') {
                    vue.messagebox.alert('私车公用公里数必填!').then(action => {
                        return
                    });
                }
                if ('<%=post %>' === '经理' || '<%=post %>' === '总监' ||'<%=post %>' === '副总经理' || '<%=post %>' === '总经理'){
                    if (parseFloat(b) > parseFloat(c) * 0.6) {
                        vue.messagebox.alert('车船费超标，无法提交!').then(action => {
                            return
                        });
                    }
                }
            }
        }

        //var list = $('#form').serializeArray();
        //var id = GetQueryString('id');
        //var flag = "";
        //$.each(list, function (i, v) {
        //    if ((v.value == "" || v.value == "请选择" || v.value == null) && vue.parameterData[i].REQD == "1") {
        //        flag += v.name + "为必填项! ";
        //    }
        //});
        //if (flag !="") {
        //    vue.messagebox.alert(flag);
        //    return;
        //}
        //var formData = JSON.stringify(list);
        if (allRequired) {
            Loading(true);
            $.ajax({
                url: 'mFormDetail.aspx',
                data: {
                    act: 'submitForm',
                    formData: JSON.stringify(formData),
                    formName: vue.formTitle,
                    processJson: JSON.stringify(vue.process),
                    id: GetQueryString('docId'),
                    defaultInformer: JSON.stringify(vue.defaultInformer),
                },
                dataType: 'json',
                type: 'post',
                success: function(data) {
                    Loading(false)
                    if (data.ErrCode == 0) {
                        vue.messagebox.alert('单据提交成功!').then(action => {
                            location.href = 'mFormListAndDetail.aspx?formName=' + vue.formTitle;
                        });

                    } else {
                        vue.messagebox.alert('单据提交失败!').then(action => {

                        });
                    }
                }
            });
        }
    }
</script>
</html>
