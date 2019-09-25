<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyInfo.aspx.cs" Inherits="MyInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <!--<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />-->
    <meta charset="UTF-8">
    <title></title>
    <%--<meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />--%>
    <!-- 引入样式 -->
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <!--  <script src="Scripts/base-loading.js"></script>-->
    <style>
        
    </style>
</head>
<body>
	<div style="width: 400px;margin:20px 20px 0px 0px" id="app">
		<el-form ref="form" :model="form" :rules="rules" label-width="120px" >
		    <el-form-item label="原密码" prop="OldPsw">
                <el-input v-model="form.OldPsw" type="password"></el-input>
            </el-form-item>
            <el-form-item label="新密码" prop="NewPsw">
                <el-input v-model="form.NewPsw" type="password"></el-input>
            </el-form-item>
            <el-form-item label="重复新密码" prop="ReNewPsw">
                <el-input v-model="form.ReNewPsw" type="password"></el-input>
            </el-form-item>
            <el-form-item >
                <el-button type="primary" size="large"  @click="onSubmit">提交</el-button>
            </el-form-item>
		</el-form>
	</div>

</body>
<!-- 先引入 Vue -->
<script src="https://unpkg.com/vue/dist/vue.js"></script>
<!-- 引入组件库 -->
<script src="https://unpkg.com/element-ui/lib/index.js"></script>
<script src="Scripts/jquery.min.js"></script>
<script>
    var Url = "MyInfo.aspx";
    var app = new Vue({
        el: '#app',
        data: {
            form: {
                OldPsw: '',
                NewPsw: '',
                ReNewPsw: ''
            },
            rules: {
                OldPsw: [
                { required: true, message: '请输入旧密码', trigger: 'blur' },
                { min: 6, max: 16, message: '长度在 6 到 16 个字符', trigger: 'blur' }
                ],
                NewPsw: [
                { required: true, message: '请输入新密码', trigger: 'blur' },
                { min: 6, max: 16, message: '长度在 6 到 16 个字符', trigger: 'blur' }
                ],
                ReNewPsw: [
                { required: true, message: '请重复输入新密码', trigger: 'blur' },
                { min: 6, max: 16, message: '长度在 6 到 16 个字符', trigger: 'blur' }
                ]
            }
        },
        
        //data(){
        //    var validatePass2 = (rule, value, callback) => {
        //        if (value === '') {
        //            callback(new Error('请再次输入密码'));
        //        } else if (value !== this.form.pass) {
        //            callback(new Error('两次输入密码不一致!'));
        //        } else {
        //            callback();
        //        }
        //    };
        //    return {
        //        data: {
        //            form: {
        //                OldPsw: '',
        //                NewPsw: '',
        //                ReNewPsw: ''
        //            }
        //        },
        //        rules: {
        //            OldPsw: [
        //            { required: true, message: '请输入旧密码', trigger: 'blur' },
        //            { min: 6, max: 16, message: '长度在 6 到 16 个字符', trigger: 'blur' }
        //            ],
        //            NewPsw: [
        //            { required: true, message: '请输入新密码', trigger: 'blur' },
        //            { min: 6, max: 16, message: '长度在 6 到 16 个字符', trigger: 'blur' }
        //            ],
        //            ReNewPsw: [
        //            { required: true, message: '请重复输入新密码', trigger: 'blur' },
        //            { min: 6, max: 16, message: '长度在 6 到 16 个字符', trigger: 'blur' }
        //            ]
        //        }
        //    };
        //},
        methods: {
            onSubmit() {                              
                this.$refs['form'].validate((valid) => {
                    if (valid) {
                        if (this.form.NewPsw != this.form.ReNewPsw) {
                            this.$notify.error({
                                title: '错误',
                                message: '两次输入的新密码不一致，请重新输入'
                            });
                            this.form.NewPsw = '';
                            this.form.ReNewPsw = '';
                            return false;
                        }
                        $.post(Url, { act: 'modifyPsw', oldPsw: this.form.OldPsw, newPsw: this.form.NewPsw }, function (res) {
                            if (res == '密码修改成功') {
                                this.$notify({
                                    title: '成功',
                                    message: res,
                                    type: 'success'
                                });
                            }
                            else {
                                this.$notify({
                                    title: '错误',
                                    message: res,
                                    type: 'error'
                                });
                            }
                            
                            if (title == '成功') {
                                this.form.NewPsw = '';
                                this.form.ReNewPsw = '';
                                this.form.OldPsw = '';
                            }
                            else {
                                this.form.OldPsw = '';
                            }
                        });
                    } else {
                        //console.log('error submit!!');
                        return false;
                    }
                });                
            }
        }
    });

</script>

</html>