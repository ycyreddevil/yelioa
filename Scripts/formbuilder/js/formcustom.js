$(function () {
    $("#back").click(function () {
        $.confirm({
            title: '系统提示',
            content: '确认返回至表单首页吗？',
            buttons: {
                ok: {
                    text: '确认',
                    btnClass: 'btn-primary',
                    action: function () {
                        location.href = "FormIndex.aspx";
                    }
                },
                cancel: {
                    text: '取消',
                    btnClass: 'btn-primary',
                    action: function () {
                        // button action.
                    }
                },
            }
        });
    })

    $('#preview').click(function () {
        var formData = M;
        var parameterData = F;

        $("#previewModal").dialog({
            title: formData.FRMNM,
            width: 400,
            height: 400,
            modal: true
        });

        //$("#previewModal").dialog({
        //    title: formData.FRMNM,
        //    width: 400,
        //    height: 400,
        //    closed: false,
        //    cache: false,
        //    modal: true
        //})
        // 把表单中的控件属性加到模态框中
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
            else if (parameterData[i].TYP == "text" || parameterData[i].TYP == "name") {
                widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content textcontent"><input type="text" disabled="disabled" maxlength="255" class="input"/><i class="iconfont qrinput hide">&#xe67d;</i></div></br>';
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
                widgt += '<label class="desc">' + parameterData[i].LBL + '</label> <div class="content"><select disabled="disabled" class="m input"></select></div>'
            }
            else if (parameterData[i].TYP == "dropdown2") {
                widgt += '<label class="desc">' + parameterData[i].LBL + '</label> <div class="content"><select disabled="disabled" class="m input"></select> <select disabled="disabled" class="m input"></select></div>';
            }
            else if (parameterData[i].TYP == "image") {
                widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content"><img style="width:100%;" src="/Content/CustomFrom/FormDesign/images/defaultimg.png"';
            }
            else if (parameterData[i].TYP == "date") {
                widgt += '<label class="desc">日期</label> <div class="content oneline reduction"><span>\t<input class="yyyy input" disabled="disabled" maxlength="4" type="text" />\t<label>YYYY</label></span><span class="split"> - </span><span>\t<input class="mm input" disabled="disabled" maxlength="2" type="text" />\t<label>MM</label></span><span class="split"> - </span><span>\t<input class="dd input" disabled="disabled" maxlength="2" type="text" />\t<label>DD</label></span><span><a class="icononly-date" title="选择日期"></a></span></div>';
            }
            else if (parameterData[i].TYP == "time") {
                widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content oneline reduction"><span>\t<select class="hh input" disabled="disabled"></select></span><span class="split"> : </span><span>\t<select class="mm input" disabled="disabled"></select></span></div>';
            }
            else if (parameterData[i].TYP == "file") {
                widgt += '<label class="desc">' + parameterData[i].LBL + '</label><div class="content"><input type="text" disabled="disabled" class="m input" />&nbsp;<input type="button" class="btn file-input" disabled="disabled" value="浏览..." /></div>';
            }
            //else if (parameterData[i].TYP == "name") {
            //    widgt += '';
            //}
            //else if (parameterData[i].TYP == "address") {
            //    widgt += '';
            //}

            $("#previewModal").append(widgt);
        }

        //$.post("/CustomFrom/FormDesign/FormView", { formData: JSON.stringify(M), parameterData: JSON.stringify(F) });
    });

    $('#saveForm').click(function () {
        var action;
        if (formId == "" || formId == null) {
            action = "saveForm";
        } else {
            action = "updateForm";
        }
        $.confirm({
            title: '系统提示',
            content: '确认保存此表单吗？',
            buttons: {
                ok: {
                    text: '确认',
                    btnClass: 'btn-primary',
                    action: function () {
                        // 判断该表单所有控件的名字是否有重复
                        var el_name_array = new Array();
                        for (i = 0; i < F.length; i++) {
                            var el_name = F[i].LBL;
                            if (contains(el_name_array, el_name)) {
                                $.alert({
                                    title: '提示!',
                                    content: '该表单有重复的字段名，请修改后再提交!',
                                });
                                return;
                            } else {
                                el_name_array.push(el_name);
                            }
                        }

                        // 判断该表单是否有用户填写了数据，如果有则不允许修改
                        $.ajax({
                            url: 'FormBuilder.aspx',
                            data: { act: "hasUsedForm", formName: M.FRMNM},
                            dataType: 'json',
                            type: 'post',
                            success: function (data) {
                                if (data.ErrCode == 3 || data.ErrCode == 2) {
                                    $.ajax({
                                        url: 'FormBuilder.aspx',
                                        data: { act: action, formName: M.FRMNM, formData: JSON.stringify(M), parameterData: JSON.stringify(F), id: formId },
                                        dataType: 'json',
                                        type: 'post',
                                        success: function (msg) {
                                            location.href = "FormIndex.aspx";
                                        },
                                        error: function (msg) {

                                        }
                                    })
                                } else {
                                    $.alert({
                                        title: '提示!',
                                        content: '该表单已经存在用户数据，无法进行修改!',
                                    });
                                    // 说明该表单已经有用户使用了 无法进行更改
                                }
                            },
                        })
                        
                    }
                },
                cancel: {
                    text: '取消',
                    btnClass: 'btn-primary',
                    action: function () {
                        // button action.
                    }
                },
            }
        });
    });

    var formId = JSON.parse(GetQueryString('id'));

    $.ajax({
        url: "FormIndex.aspx",
        data: { act: 'findDetail', id: formId },
        dataType: 'json',
        type: 'post',
        success: function (data) {
            var formData = JSON.parse(data.data)[0].FormData;
            var parameterData = JSON.parse(data.data)[0].ParameterData;

            // 把控件的名称带入
            F = JSON.parse(parameterData);
            M = JSON.parse(formData);

            // 加载已经保存的表单数据
            $("#fTitle").html(M.FRMNM);
            
            for (i = 0; i < F.length; i++) {
                var type = F[i].TYP;
                $.each($('#addFields').find('li'), function (j, v) {
                    if ($(v).attr("ftype") == type) {
                        $(v).trigger("click");

                        F = JSON.parse(parameterData);
                        M = JSON.parse(formData);
                        // 判断是否需要必选
                        var isRequiredHtml = "";
                        if (F[i].REQD == "0") {
                            isRequiredHtml = '<span class="req hide">*</span>';
                        } else {
                            isRequiredHtml = '<span class="req show">*</span>';
                        }

                        $("#fields").children("li:last").children("label").html(isRequiredHtml)

                        $("#fields").children("li:last").children("label").children("span").before(F[i].LBL);

                        if (type == "name") {
                            $("#selMatchFrm").val(F[i].RELA.RELA1.TABLENM);
                            $("#selMatchFld").val(F[i].RELA.RELA1.FILEDNM);
                        }
                    }
                })
            }
        }
    })
})

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
