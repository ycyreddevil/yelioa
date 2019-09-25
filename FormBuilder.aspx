﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FormBuilder.aspx.cs" Inherits="FormBuilder" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta name="renderer" content="webkit">
    <title>自定义表单</title>
    <link type="text/css" rel="stylesheet" href="Scripts/formbuilder/css/common.css?v=20160929" />
    <link type="text/css" rel="stylesheet" href="Scripts/formbuilder/css/jquery-ui-1.9.2.custom.css" />
    <link type="text/css" rel="stylesheet" href="Scripts/formbuilder/css/widgets.css?v=20160929" />
    <link type="text/css" rel="stylesheet" href="Scripts/formbuilder/css/jquery.mCustomScrollbar.min.css?v=20160929" />
    <link type="text/css" rel="stylesheet" href="Scripts/formbuilder/css/formbuild.css?v=20160929" />
    <link href="//cdn.bootcss.com/jquery-confirm/3.1.0/jquery-confirm.min.css" rel="stylesheet">
    <style>
        .jconfirm-box.jconfirm-hilight-shake.jconfirm-type-default.jconfirm-type-animated{
            width: 50%;
            margin-top: 315px;
            margin-bottom: 50px;
            transition-duration: 0.4s;
            transition-timing-function: cubic-bezier(0.36, 0.55, 0.19, 1);
            transition-property: all, margin;
        }
    </style>
</head>
<body>
    <div id="container">
        <!-- left state -->
        <div id="left">

            <div id="addFields" class="overhide">
                <h3 class="fields-group">通用字段</h3>
                <ul id="col1">
                    <li id="drag_text" ftype="text"><a id="sl" class="btn-field" title="适用于填写简短的文字内容，身份证号、银行卡号、工号等请使用此类型。" href="#"><i class="iconfont">&#xe643;</i>单行文本</a></li>
                    <li id="drag_textarea" ftype="textarea"><a id="pt" class="btn-field" title="适用于填写大段文本，如“备注”、“留言”" href="#"><i class="iconfont">&#xe61a;</i>多行文本</a></li>
                    <li id="drag_radio" ftype="radio"><a id="mc" class="btn-field" title="适用于在少量选项里选一个，如“男/女”" href="#"><i class="iconfont">&#xe66f;</i>单选框</a></li>
                    <li ftype="date"><a id="dt" class="btn-field" title="适用于选择特定的日期" href="#"><i class="iconfont">&#xe62a;</i>日期</a></li>
                    <li ftype="image"><a id="im" class="btn-field" title="在表单上加入图片，起到宣传产品或美化表单的作用" href="#"><i class="iconfont">&#xe613;</i>图片</a></li>
                    <%--<li id="drag_radio" ftype="section"><a id="sb" class="btn-field" title="用于将字段分组显示，更清晰" href="#"><i class="iconfont">&#xe61b;</i>分隔符</a></li>--%>
                </ul>
                <ul id="col2">
                    <li id="drag_number" ftype="number"><a id="nb" class="btn-field" title="适用于填写涉及到数学运算的数字，身份证号、银行卡号、工号等请使用单行文本。" href="#"><i class="iconfont">&#xe640;</i>数字</a></li>
                    <li id="drag_checkboxes" ftype="checkbox"><a id="cb" class="btn-field" title="适用于在几个选项里选多个，如投票" href="#"><i class="iconfont">&#xe64a;</i>多选框</a></li>
                    <li id="drag_dropdown" ftype="dropdown"><a id="dd" class="btn-field" title="适用于在非常多的选项里选一个，如省份选择" href="#"><i class="iconfont">&#xe626;</i>下拉框</a></li>
                    <li ftype="time"><a id="ti" class="btn-field" title="适用于填写特定的时间" href="#"><i class="iconfont">&#xe646;</i>时间</a></li>
<%--                    <li ftype="likert"><a id="lk" class="btn-field" title="适用于处理批量单选" href="#"><i class="iconfont">&#xe670;</i>组合单选框</a></li>--%>
                    <li ftype="file"><a id="fu" class="btn-field" title="适用于收集文件，如简历、照片" href="#"><i class="iconfont">&#xe62b;</i>文件上传</a></li>
                    <%--<li id="drag_dropdown" ftype="html"><a id="ht" class="btn-field" title="适用于添加HTML显示元素，如“p,a,span,div”等" href="#"><i class="iconfont">&#xe65a;</i>HTML</a></li>--%>
                </ul>
                <h3 class="fields-group">关联信息字段</h3>
                <ul id="col3">
                    <li ftype="name"><a id="nm" class="btn-field" title="适用于填写关联基础数据" href="#"><i class="iconfont">&#xe652;</i>关联数据</a></li>
                    <%--<li ftype="phone"><a id="ph" class="btn-field" title="适用于填写中国大陆内的手机和座机号码" href="#"><i class="iconfont">&#xe634;</i>电话</a></li>--%>
                </ul>
               <%-- <ul id="col4">
                    <li ftype="address"><a id="ad" class="btn-field" title="适用于填写全国的地址" href="#"><i class="iconfont">&#xe64e;</i>部门</a></li>
                  <li ftype="email"><a id="em" class="btn-field" title="适用于填写电子邮箱地址" href="#"><i class="iconfont">&#xe644;</i>电子邮箱</a></li>
                    <li ftype="map"><a id="mp" class="btn-field" title="通过地图收集地理信息，手机上可自动定位" href="#"><i class="iconfont">&#xe63f;</i>地理位置</a></li>
                    <li ftype="url"><a id="ws" class="btn-field" title="适用于填写网站链接" href="#"><i class="iconfont">&#xe614;</i>网址</a></li>
                    <li ftype="money"><a id="pr" class="btn-field" title="适用于填写价格" href="#"><i class="iconfont">&#xe643;</i>价格</a></li> 
                </ul>--%>
                <%--<h3 class="fields-group">商品字段</h3>
                <ul id="col5">
                    <li ftype="goods"><a id="gd" class="btn-field" title="适用于发布精美配图的商品" href="#"><i class="iconfont">&#xe671;</i>配图商品</a></li>
                </ul>
                <ul id="col6">
                    <li ftype="goods" subtype="noimg"><a id="gd2" class="btn-field" title="适用于发布无图的商品，如门票" href="#"><i class="iconfont">&#xe65d;</i>无图商品</a></li>
                </ul>--%>
            </div>
            <!-- addFields -->
        </div>
        <!-- left end -->
        <!-- middle state -->
        <div id="middle">
            <div class="forms">
                <div id="fbForm" class="form form-focused">
                    <h2 id="fTitle"></h2>
                    <div id="fDescription"></div>
                </div>
            </div>
            <div id="nofields" class="notice hide" style="margin: 30px 18px 0px 28px">
                <div id="addFromButton" style="cursor: pointer;">
                    <h2 class="color-red">没有字段!</h2>
                    <a href="#">表单中没有字段，点击或拖动左边的组件添加字段。</a>
                </div>
            </div>
            <!--表单绘制区域-->
            <ul id="fields" class="fields">
            </ul>
            <!--表单绘制区域-->
            <div class="formButtons hide" id="formButtons">
                <table style="margin: auto; font-size: 1.0em">
                    <tr>
                        <td style="border: none;"><a class="btn left" id="back" href="javascript:void(0)">返回</a></td>
                        <td style="border: none;"><a class="btn blue left" id="saveForm"  href="javascript:void(0)">保存</a></td>

                        <!-- <td><a class="btn green left" id="btnAddField2" href="#"><b></b>添加新字段</a></td> -->
                    </tr>
                </table>
            </div>
        </div>
        <!-- middle end -->
        <!-- right state-->
        <div id="right">
            <!--  -->
            <div class="notice hide" style="margin-top: 30px; border: none" id="noFieldSelected">
                <h3><b>没有选择字段</b></h3>
                <p>请先在右侧选择需要编辑的字段，然后在此编辑字段的属性。</p>
            </div>
            <div id="fieldProperties" class="hide">
                <!-- field properties -->
                <h3 class="property-title">表单属性</h3>
                <div id="allPropsContainer">
                    <ul id="allProps">
                        <!-- <li class="num" id="liPos">1.</li> -->
                        <li id="plabel">
                            <label class="desc" for="lbl">
                                字段名称
                            </label>
                            <textarea id="lbl" name="LBL" class="xxl" rows="2"></textarea>
                        </li>
                        <li>
                            <label class="desc" for="defval_text">
                                默认值
		    
                                <a href="#" class="help hide" title="关于默认值" rel="在用户访问表单时，此值将作为默认值显示在输入框中。如果不需要默认值，请将此处设置为空。">(?)</a>
                            </label>
                            <input id="defval_text" name="DEF" class="xxl" type="text" value="" maxlength="255" />
                        </li>
                        

                        <%--<li id="ptype" class="left half">
                            <label class="desc" for="type">
                                字段类型 
		
                                <a href="#" class="help" title="关于字段类型" rel="可以修改表单保存之前添加字段的类型。">(?)</a>
                            </label>
                            <select id="type" name="TYP" class="xxl">
                                <optgroup label="标准类型">
                                    <option value="text">单行文本</option>
                                    <option value="textarea">多行文本</option>
                                    <option value="radio">单选框</option>
                                    <option value="number">数字</option>
                                    <option value="checkbox">多选框</option>
                                    <option value="dropdown">下拉框</option>
                                </optgroup>
                                <optgroup label="常用类型">
                                    <option value="email">电子邮箱</option>
                                    <option value="address">地址</option>
                                    <option value="map">地理位置</option>
                                    <option value="phone">手机</option>
                                    <option value="name">姓名</option>
                                    <option value="file">上传文件</option>
                                    <option value="date">日期</option>
                                    <option value="time">时间</option>
                                    <option value="url">网址</option>
                                    <option value="likert">组合单选框</option>
                                    <option value="dropdown2">多级下拉框</option>
                                    <option value="image">图片</option>
                                    <option value="goods">配图商品</option>
                                    <option value="goodsnoimg">无图商品</option>

                                </optgroup>
                            </select>
                        </li>--%>

                        <%--<li class="right half" id="pfldsize">
                            <label class="desc" for="fldsize">
                                字段长度
		
                                <a href="#" class="help" title="关于字段长度" rel="用于限定字段输入框的长度（“多行文本”字段限定输入框高度）。">(?)</a>
                            </label>
                            <select id="fldsize" name="FLDSZ" class="xxl">
                                <option value="s">短</option>
                                <option value="m">中</option>
                                <option value="xxl">长</option>
                            </select>
                        </li>

                        <li class="right half" id="playout">
                            <label class="desc" for="layout">
                                字段布局
		
                                <a href="#" class="help hide" title="关于字段布局" rel="此属性仅对复选框和单选框类型的字段有效，用于定义复选框或单选框的排列方式。其中自动排列是指按一个接一个的方式进行排列。">(?)</a>
                            </label>
                            <select id="layout" name="LAY" class="xxl">
                                <option value="one">一列</option>
                                <option value="two">二列</option>
                                <option value="three">三列</option>
                                <option value="oneByOne">自动排列</option>
                            </select>
                        </li>

                        <li class="right half" id="pdateformat">
                            <label class="desc" for="dateformat">
                                日期格式
		
                                <a href="#" class="help hide" title="关于日期格式" rel="此属性用于指定日期的输入格式。YYYY代表年，MM代表月，DD代表日。">(?)</a>
                            </label>
                            <select id="dateformat" name="FMT" class="xxl">
                                <option value="ymd" selected="selected">YYYY - MM - DD</option>
                                <option value="mdy">MM / DD / YYYY</option>
                                <option value="dmy">DD / MM / YYYY</option>
                            </select>
                        </li>

                        <li class="right half" id="pphoneformat">
                            <label class="desc" for="phoneformat">
                                电话格式
		
                                <a href="#" class="help hide" title="关于电话格式" rel="此属性用于指定电话的输入格式。支持普通的电话号码输入和“区号-总机-分机”的座机号码输入。">(?)</a>
                            </label>
                            <select id="phoneformat" name="FMT" class="xxl">
                                <option value="mobile" selected="selected">手机</option>
                                <option value="tel">座机</option>
                            </select>
                        </li>

                        <li class="right half" id="pnameformat">
                            <label class="desc" for="nameformat">
                                姓名格式
		
                                <a href="#" class="help hide" title="关于姓名格式" rel="此属性用于指定姓名的输入格式。支持普通的姓名格式和带称呼的加长格式。">(?)</a>
                            </label>
                            <select id="nameformat" name="FMT" class="xxl">
                                <option value="short" selected="selected">普通</option>
                                <option value="extend">加长</option>
                            </select>
                        </li>

                        <li class="right half" id="pmoneyformat">
                            <label class="desc" for="moneyfomat">货币格式</label>
                            <select id="moneyfomat" name="FMT" class="xxl">
                                <option value="yen">¥ 人民币/日元</option>
                                <option value="dollars">$ 美元</option>
                                <option value="pounds">£ 英镑</option>
                                <option value="euros">€ 欧元</option>
                            </select>
                        </li>

                        <li class="left half" id="pN">
                            <label class="desc" for="N">层级</label>
                            <select id="N" name="pN" class="xxl">
                                <option value="2" selected="selected">2</option>
                                <option value="3">3</option>
                                <option value="4">4</option>
                            </select>
                        </li>--%>

                        <li class="clear noheight"></li>
                        <li id="plikert" class="bggray">
                            <fieldset>
                                <legend>行标签
		
                                    <a href="#" class="help hide" title="关于行标签" rel="此属性用于指定组合单选框中表示组合类别的标签。">(?)</a>
                                </legend>
                                <ul id="likertRows"></ul>
                            </fieldset>
                            <fieldset>
                                <legend>列标签
		
                                    <a href="#" class="help hide" title="关于列标签" rel="此属性用于指定组合单选框中表示级次的标签。">(?)</a>
                                </legend>
                                <ul id="likertCols"></ul>
                                <div class="center">
                                    <a id="btnLikertPredefine" href="#" class="btn gray">批量编辑</a>
                                </div>
                            </fieldset>
                        </li>
                        <li class="clear noheight"></li>

                        <li id="pitems" class="clear bggray">
                            <fieldset>
                                <legend>选择项
	
                                    <a href="#" class="help hide" title="关于选择项" rel="此属性用于指定有哪些选择项可以提供给用户选择。利用旁边的增加或删除按钮或以增加或删除选择项。对于下拉框在没有指定默认选中项的情况下将自动选中第一项。">(?)</a>
                                </legend>
                                <ul id="itemList">
                                </ul>

                                <div id="pitems_radio" class="center">
                                    <a id="btnItemsPredefine" href="#" class="btn gray">批量编辑</a>
                                </div>
                                <div id="pitems_batchedit" class="center">
                                    <a id="btnItemsBatch" href="#" class="btn gray">批量编辑</a>
                                </div>
                            </fieldset>
                        </li>

                        <%--<li id="pgoods" class="clear bggray">
                            <fieldset>
                                <legend>商品列表
	
                                    <a href="#" class="help hide" title="关于商品列表" rel="此属性用于指定在表单中显示的商品。如果是图片商品，图片长宽比例请保持1:1，文件体积需要在500KB以内，支持.jpg格式。提示：按住商品列表拖动可以排序哦。">(?)</a>
                                </legend>
                                <ul id="goodsList" class="clearfix">
                                </ul>

                                <div id="pgoods_radio" class="center add-goods">
                                    <form name="goodsUploadForm" class="add-image-btn" action="" method="POST" enctype="multipart/form-data" style="height: 35px; padding: 5px 0px; vertical-align: middle;">
                                        <a id="btnAddGoods" title="添加配图商品" class="btn gray" href="#"><span style="display: block;">+ 添加配图商品</span>
                                            <input id="fileToUpload" title="添加配图商品" name="fileToUpload" class="file-prew" title="支持jpg、jpeg、png格式，文件小于500K" type="file" size="3" accept="image/jpeg,image/png,image/bmp,image/gif" /></a>
                                        <!-- <a id="btnGoodsPredefine" href="#" title="添加常用配图商品" style="display:inline-block;padding-bottom:20px;vertical-align: middle;color: #3670af;text-decoration: underline;">添加常用配图商品</a> -->
                                    </form>
                                    <div id="addNoImgGoods" class="add-goods-btn" style="height: 35px; padding: 5px 0px; vertical-align: middle;">
                                        <a id="btnAddNoImgGoods" title="添加无图商品" class="btn gray"><span>+ 添加无图商品</span></a>
                                    </div>
                                    <div class="clear">
                                        <input id="goodsForBuy" value="1" name="FBUY" type="checkbox">
                                        <label for="goodsForBuy">商品用于向供应商询价</label>
                                        <a href="#" class="help hide" title="关于商品用于向供应商询价" rel="当勾选此选项时，将由制表人确信数量，填表人根据数量填写单价。主要用于向供应商询价，供应商填写表单进行报价的场景。">(?)</a><br>
                                    </div>
                                </div>
                            </fieldset>
                        </li>--%>

                        <li id="pimage">
                            <form name="uploadImageForm" action="" method="POST" enctype="multipart/form-data" style="padding: 5px 0px;">
                                <label class="desc" for="uploadImage">
                                    上传图片<a href="#" class="help hide" title="关于图片" rel="在表单中添加图片显示，支持gif格式，每张图片最大2M。">(?)</a>
                                </label>
                                <a class="btn gray filewrap">
                                    <span>上传图片</span>
                                    <input type="file" id="uploadImage" name="uploadImage" title="支持jpg、jpeg、png格式，文件小于500K" accept="image/jpeg,image/png,image/bmp,image/gif" />
                                </a>
                            </form>
                        </li>

                        <li class="left half clear" id="poptions">
                            <fieldset>
                                <legend>设置</legend>
                                <ul>
                                    <li id="popt_required">
                                        <input id="reqd" name="REQD" type="checkbox" value="1" />
                                        <label for="reqd">必须输入</label>
                                        <a href="#" class="help hide" title="关于必须输入" rel="强制填表人该字段必须输入，否则将不能提交表单。<a href='#' class='video help' videosrc='images/videos/2-1.mp4'><i class='iconfont icon green2' >&#xe64d;</i>观看视频说明</a>">(?)</a>
                                    </li>
                                    <%--<li id="popt_unique">
                                        <input id="uniq" name="UNIQ" type="checkbox" value="1" />
                                        <label for="uniq">不许重复</label>
                                        <a href="#" class="help" title="关于不许重复" rel="用于保证字段输入值的唯一性，适用于如手机号、QQ号等需要保证唯一性的输入值。">(?)</a>
                                    </li>--%>

                                    <!-- <li id="popt_qrinput">
		<input id="qrinput" name="QRINPUT" type="checkbox" value="1"/>
		<label for="qrinput">扫码输入</label>
		<a href="#" class="help hide" title="关于扫码输入" rel="通过扫描二维码或条形码的方式输入数据。<i>注意：目前仅支持在微信中打开表单扫码。</i>">(?)</a>
		</li> -->

                                    <!-- <li id="popt_random">
		<input id="random" name="RDM" type="checkbox" value="1" />
		<label for="random">随机</label>
		<a href="#" class="help hide" title="关于随机" rel="此属性用于指定单选框中的选项在访问时出现的顺序是否是随机的。如果勾选，则选择项在每次访问时出现的顺序是随机的。">(?)</a>
		</li> -->

                                    <!-- <li id="popt_allowother">
		<input id="allowOther" name="OTHER"  type="checkbox" value="1"/>
		<label for="allowOther">允许其他值</label>
		<a href="#" class="help hide" title="关于允许其他值" rel="此属性用于指定是否可以让用户输入除选择项以外的其他值。">(?)</a>
		</li> -->

                                    <%--<li id="popt_hidenum">
                                        <input id="hidenum" name="HDNM" type="checkbox" value="1" />
                                        <label for="hidenum">隐藏数字</label>
                                        <a href="#" class="help hide" title="关于隐藏数字" rel="在单选框下方通常都有一个数字用于标识此选项的分值，此属性用于指定是否隐藏此数字。">(?)</a>
                                    </li>

                                    <li id="popt_authcode">
                                        <div>
                                            <input id="internal" name="INTERNAL" type="checkbox" value="1" />
                                            <label for="internal">启用国际手机</label>
                                            <a href="#" class="help hide" title="关于启用国际手机号" rel="启用后可以向全球200多个国家和地区发送短信，请先联系客户咨询使用详情。<a href='help/smsprice.html' target='_blank'>查看资费明细</a>">(?)</a>
                                        </div>

                                        <input id="authcode" name="AUTH" type="checkbox" value="1" />
                                        <label for="authcode">验证码</label>
                                        <a href="#" class="help hide" title="关于手机验证码" rel="启用验证码需要满足以下两个条件：1、签名通过审核（可联系在线客服审核）；2、有可用短信量（短信需要单独购买）；<a href='help/smsprice.html' target='_blank'>查看资费明细</a>">(?)</a>

                                        <div id="signcnt" class="hide">
                                            <input id="sign" name="SIGN" placeholder="短信签名" type="text" maxlength="16" style="width: 60px;" />
                                            <a id="btnSignSumbmit" target="_blank" href="/web/formview/5606403b0cf2f6fe39b1965d" class="btn no-icon btn-blue small">提交审核</a>
                                        </div>
                                    </li>--%>

                                    <!-- 
		<li id="popt_compress">
		<input id="chkCompress" type="checkbox" value="1"/>
		<label for="chkCompress">压缩图片</label> <a href="#" class="help hide" title="关于压缩图片" rel="如果上传的是图片，勾上此项后将按照您设置的压缩比对图片进行压缩，达到减少网络流量和节省存储空间的目的。数值越小，表示压缩越严重，越省流量和存储空间，但也意味着图象质量越差。<i>注意：此选项仅当上传的文件是图片时才起作用，且不支持IE10以下浏览器及少部分低版本手机操作系统。对于 不支持的设备，将采用正常方式上传。</i>">(?)</a>
		<div id="divCompress" class="hide">
		<label for="selCompress">压缩到</label>
		<select id="selCompress" name="CPRS" style="width:70px;">
			<option value="10">10%</option>
			<option value="20">20%</option>
			<option value="30">30%</option>
			<option value="40">40%</option>
			<option value="50">50%</option>
		</select>
		</div>
		</li>
		 -->

                                    <%--<li id="popt_dismark">
                                        <input id="chkDismark" name="DISMK" type="checkbox" value="1" />
                                        <label for="chkDismark">禁止手动标注</label>
                                        <a href="#" class="help hide" title="禁止手动标注" rel="默认情况下，地理位置支持自动定位和手动标注。您可以勾选此选项来禁用手动标注，满足某些需要真实确认填表人位置的场景。">(?)</a>
                                    </li>--%>

                                </ul>
                            </fieldset>
                        </li>

                        <!-- <li class="right half" id="psecurity">
	<fieldset>
		<legend>字段可见性</legend>
		<input id="sec_pub" name="SCU" type="radio" value="pub"/>
		<label for="sec_pub">每个人可见</label>
		<br/>
		<input id="sec_pri" name="SCU" type="radio"  value="pri" checked="checked"/>
		<label for="sec_pri">登录用户可见</label>
		<a href="#" class="help" title="关于登录用户可见" rel="勾选此项之后，只有登录本表单对应的账户，才能看到该隐藏字段，普通填表者将看不到该字段。">(?)</a>
	</fieldset>
	</li> -->

                        <li class="clear noheight"></li>

                        <%--<li id="prange" class="bggray">
                            <fieldset>
                                <legend>范围
		
                                    <a href="#" class="help" title="关于范围" rel="数值型字段用于限定数值的范围；文本型字段用于限定字数的多少。">(?)</a>
                                </legend>
                                <div>
                                    <div class="half left">
                                        <label class="desc min not-bold" for="min">最小值</label>
                                        <input class="xxl number" id="min" name="MIN" type="text" value="" />
                                    </div>
                                    <div class="half right">
                                        <label class="desc max not-bold" for="max">最大值</label>
                                        <input class="xxl number" id="max" name="MAX" type="text" value="" />
                                    </div>
                                </div>
                            </fieldset>
                        </li>--%>

                        <!-- 为了尽早简单，此功能仅在支持关系的版本中显示-->

                        <li id="pconfine">
                            <fieldset>
                                <legend>关联
		
                                    <a href="#" class="help" title="关于关联" rel="可以通过关联指定此字段的值只能来源于某个其它表单的数据。">(?)</a>
                                </legend>
                                <div id="pconfine1" class="highlight-blue" style="margin-bottom: 3px;">
                                    <label>
                                        <input type="checkbox" value="1" name="MAT" id="chkMatch" />
                                        指定数据来源并自动匹配
		
                                    <a href="#" class="help" title="关于自动匹配" rel="类似搜索引擎的自动提示效果，根据输入的值与指定的表单和字段进行匹配，实现自动完成输入">(?)</a></label>
                                    <div id="divMatchContainer" style="padding-bottom: 5px;" class="hide">
                                        <label class="desc" for="selMatchFrm">来源表单</label>
                                        <select name="MATFRM" class="xxl" id="selMatchFrm">
                                            <option>用户表</option><option>部门表</option><option>费用明细表</option>
                                            <option>网点表</option><option>产品表</option><option>项目表</option>
                                        </select>
                                        <label class="desc min" for="selMatchFld">来源字段</label>
                                        <select name="MATFLD" class="xxl" id="selMatchFld">
                                            <option>名</option>
                                        </select>
                                        <label class="desc min" for="selMatchFld">关联字段</label>
                                        <select name="MATRELFLD" class="xxl" id="selMatchRELFld">
                                            
                                        </select>
                                    </div>
                                </div>
                                <%--<div id="pconfine2" class="highlight-blue">
                                    <label>
                                        <input type="checkbox" value="1" name="ACMP" id="chkAutoComp" />
                                        根据匹配数据自动带出值
		
                                        <a href="#" class="help" title="关于自动带出值" rel="依据自动匹配的字段，当匹配到数据后，自动带出此条数据的其它字段的值作为此字段的值。如在客户管理类的表单中，可以将客户名称设置为“指定数据来源并自动匹配”，在此可以自动带出对应的联系人或联系方式等。" />
                                        <i>注意：<br />
                                            若要进行此设置，表单中必须至少有一个设置了“指定数据来源并自动匹配”的字段。目前只有文本框支持自动匹配的设置。
                                            <i>">(?)</a></label>
                                    <div id="divAutoCompContainer" style="padding-bottom: 5px;" class="hide">
                                        <label class="desc" for="selAutoCompSrcFld">自动匹配字段</label>
                                        <select name="ACMPSRCFLD" class="xxl" id="selAutoCompSrcFld">
                                            
                                        </select>
                                        <label class="desc min" for="selAutoCompFld">需要带出的字段</label>
                                        <select name="ACMPFLD" class="xxl" id="selAutoCompFld"></select>
                                    </div>
                                </div>--%>
                            </fieldset>
                        </li>

                        <%--<li id="pdefval_text">
                            <label class="desc" for="defval_text">
                                默认值
		
                                <a href="#" class="help hide" title="关于默认值" rel="在用户访问表单时，此值将作为默认值显示在输入框中。如果不需要默认值，请将此处设置为空。">(?)</a>
                            </label>
                            <input id="defval_text" name="DEF" class="xxl" type="text" value="" maxlength="255" />
                        </li>

                        <li id="pdefval_number">
                            <label class="desc" for="defval_number">
                                默认值
		
                                <a href="#" class="help hide" title="关于默认值" rel="在用户访问表单时，此值将作为默认值显示在输入框中。如果不需要默认值，请将此处设置为空。">(?)</a>
                            </label>
                            <input id="defval_number" name="DEF" class="xxl" type="text" value="" maxlength="255" />
                        </li>

                        <li id="pdefval_date">
                            <label class="desc" for="defval_date">
                                默认值
		
                                <a href="#" class="help hide" title="关于默认值" rel="在用户访问表单时，此值将作为默认值显示在输入框中。默认值可以是'YYYY-MM-DD'格式的固定日期，也可以是如下一些动态日期：'today'， '+n days'， '+n weeks'， '+n months'， '-n days'， '-n weeks'， '-n months'，其中n为正整数，如+2 days。对于动态日期，将根据用户访问表单时的时间自动转换为对应的日期。如果不需要默认值，请将此处设置为空。">(?)</a>
                            </label>
                            <input id="defval_date" name="DEF" class="xxl" type="text" value="" maxlength="255" />
                        </li>

                        <li id="pdefval_time">
                            <label class="desc" for="defval_time">
                                默认值
		
                                <a href="#" class="help hide" title="关于默认值" rel="在用户访问表单时，此值将作为默认值显示在输入框中。默认值可以是'HH:MM'格式的固定时间，也可以是如下一些动态时间：'now'， '+n minutes'， '+n hours'， '-n minutes'， '-n hours'，其中n为正整数，如+30 minutes。对于动态时间，将根据用户访问表单时的时间自动转换为对应的时间。如果不需要默认值，请将此处设置为空。">(?)</a>
                            </label>
                            <input id="defval_time" name="DEF" class="xxl" type="text" value="" maxlength="255" />
                        </li>

                        <li id="pdefval_phone_tel" class="overhide clear hide">
                            <label class="desc" for="pdefval_phone_tel">
                                默认值
		
                                <a href="#" class="help hide" title="关于默认值" rel="在用户访问表单时，此值将作为默认值显示在输入框中。如果不需要默认值，请将此处设置为空。">(?)</a>
                            </label>
                            <div class="oneline tel reduction">
                                <span>
                                    <input id="defval_phone_tel_1" class="input tel" maxlength="4" size="4" type="text" />
                                    <label for="defval_phone_tel_1">区号</label>
                                </span>
                                <span>- </span>
                                <span>
                                    <input id="defval_phone_tel_2" class="input tel" maxlength="8" size="8" type="text" />
                                    <label for="defval_phone_tel_2">总机</label>
                                </span>
                                <span>- </span>
                                <span>
                                    <input id="defval_phone_tel_3" class="input tel" maxlength="4" size="4" type="text" />
                                    <label for="defval_phone_tel_3">分机</label>
                                </span>
                                <input id="defval_phone_tel" type="hidden" size="18" name="DEF" />
                            </div>
                        </li>

                        <li id="pdefval_phone_mobile" class="clear hide">
                            <label class="desc" for="defval_phone_mobile">
                                默认值
		
                                <a href="#" class="help hide" title="关于默认值" rel="在用户访问表单时，此值将作为默认值显示在输入框中。如果不需要默认值，请将此处设置为空。">(?)</a>
                            </label>
                            <input id="defval_phone_mobile" name="DEF" class="m" type="text" maxlength="18" />
                        </li>

                        <li id="pdefval_addr">
                            <label class="desc" for="defval_country">
                                默认值
		
                                <a href="#" class="help hide" title="关于默认值" rel="在用户访问表单时，此值将作为默认值显示在输入框中。如果不需要默认值，请将此处设置为空。">(?)</a>
                            </label>
                            <select id="defval_province" name="DEF_PROVINCE" class="s"></select>
                            <select id="defval_city" name="DEF_CITY" class="s">
                                <option>市</option>
                            </select>
                            <select id="defval_zip" name="DEF_ZIP" class="s">
                                <option>区/县</option>
                            </select>
                        </li>--%>

                        <li id="psection" class="clear">
                            <label class="desc" for="secdesc">
                                分隔描述
		
                                <a href="#" class="help hide" title="关于分隔描述" rel="请在此处添加对分隔符的描述，如果不需要描述可以清空。">(?)</a>
                            </label>
                            <textarea class="xxl" rows="5" id="secdesc" name="SECDESC"></textarea>
                        </li>
                        <li id="phtml" class="clear">
                            <label class="desc" for="html">
                                HTML内容
		
                                <a href="#" class="help hide" title="关于HTML内容" rel="如果您需要在表单上显示HTML内容，只支持显示型HTML（如p,a,div等），不支持输入型HTML（如input,select,radio等），请在此处输入相应HTML代码。<a href='help/formbuilder.html#t31' target='_blank'>如何插入图片和链接？</a>">(?)</a>
                            </label>
                            <textarea class="xxl" rows="5" id="html" name="HTML"></textarea>
                        </li>

                        <%--<li id="pinstruct" class="clear">
                            <label class="desc" for="instruct">
                                字段说明 
		
                                <a href="#" class="help" title="关于字段说明" rel="对字段进行解释，帮助填表人进行理解和输入，并在字段右侧显示。">(?)</a>
                            </label>
                            <textarea class="xxl" rows="3" id="instruct" name="INSTR"></textarea>
                        </li>
                        <li class="clear noheight"></li>--%>

                        <li id="playout" class="bggray">
                            <label class="desc" for="layout">
                                字段宽度（仅填表时可见）
		
                                <a href="#" class="help hide" title="字段宽度" rel="让多个字段并列显示在同一行（仅适用于PC端）。<a href='#' class='video help' videosrc='images/videos/2-1-2.mp4'><i class='iconfont icon green2' >&#xe64d;</i>观看视频说明</a><i>注意：设置的宽度在设计模式不可见，仅在查看表单时才能看到效果。</i>">(?)</a>
                            </label>
                            <select class="xxl" id="selLayout" name="LAYOUT">
                                <option value="">充满整行</option>
                                <option value="leftHalf">整行宽度的1/2</option>
                                <option value="third">整行宽度的1/3</option>
                                <option value="quarter">整行宽度的1/4</option>
                            </select>
                        </li>
                        <li id="pexprop" class="hide">
                            <label class="desc" for="css">扩展属性</label>
                            <input id="exprop" class="xxl" type="text" name="EX" value="" maxlength="1024" />
                            <!--
			20160821 by xiangjuntao
			1.字段的扩展属性不对外开放，仅内部人员使用；
			2.扩展属性是一个JSON对象
			3.目前支持的JSON属性为:{matfld:"F1,F2"}
			matfld:关联查询时下拉框中可以带出的其它字段名；
			
		 -->
                        </li>
                        <!-- 
	<li style="overflow:hidden;padding:10px 0px 10px 20px;">
		<a href="#" class="btn btn-blue icon-plus left" title="复制当前字段" id="btnDup">复制</a>
		<a href="#" class="btn btn-red bg-blue icon-reduce left" title="删除当前字段" id="btnDel">删除</a>
		<a href="#" class="btn btn-blue icon-plus left" style="width:70px" title="添加字段" id="btnAddField">添加新字段</a>
	</li>
	 -->

                        <li class="clear noheight"></li>
                    </ul>
                </div>

            </div>
            <!-- end field properties -->


            <!-- form properties -->
            <div id="formProperties">
                <h3 class="property-title">表单属性</h3>
                <ul id="allFormPerperties">
                    <li>
                        <label for="formName" class="desc">表单名称  </label>
                        <input id="formName" name="FRMNM" class="xxl" maxlength="64" type="text" />
                    </li>
                    <li class="clear">
                        <label for="desc" class="desc">描述 <a href="#" class="help hide" title="关于描述" rel="用于表单的描述、说明或解释，同时描述内容支持HTML。<a href='help/formbuilder.html#t31' target='_blank'>利用HTML标记插入图片和链接</a>。">(?)</a></label>
                        <textarea id="desc" name="DESC" class="xxl" rows="3" placeholder="表单描述"></textarea>
                    </li>
                    <%--<li>
                        <label class="desc" for="labelAlign">
                            标签对齐方式
  	
                        </label>
                        <select id="labelAlign" name="LBLAL" class="xxl">
                            <option value="T">上对齐</option>
                            <option value="L">左对齐</option>
                            <option value="R">右对齐</option>
                        </select>
                    </li>
                    <li>
                        <label class="desc" for="labelAlign">
                            多列操作
  	
                        </label>
                        <select id="labelAlign" name="LBLAL" class="xxl">
                            <option value="T">单列</option>
                            <option value="L">两列</option>
                            <option value="R">三列</option>
                        </select>
                    </li>
                    <li>
                        <label class="desc" for="labelAlign">提交后跳转选项</label>
                        <ul>
                            <li class="left">
                                <input id="confirmType_text" name="CFMTYP" value="T" checked="checked" type="radio" />
                                <label for="confirmType_text">显示文本</label>
                                <a href="#" class="help hide" title="关于显示文本" rel="表单提交成功后，将显示下面文本框内设定的文字。<a href='#' class='video help' videosrc='images/videos/2-4-2-2.mp4'><i class='iconfont icon green2' >&#xe64d;</i>观看视频说明</a>">(?)</a>
                            </li>

                            <li class="right">
                                <input id="confirmType_url" name="CFMTYP" value="U" type="radio" />
                                <label for="confirmType_url">跳转至网页</label>
                                <a href="#" class="help hide" title="关于跳转至网页" rel="表单提交成功后，将自动跳转到下面文本框内设定的网址。<a href='#' class='video help' videosrc='images/videos/2-4-2-1.mp4'><i class='iconfont icon green2' >&#xe64d;</i>观看视频说明</a>">(?)</a>
                            </li>
                            <li class="clear" style="padding-top: 5px;">
                                <textarea id="confirmMsg_text" name="CFMMSG" class="xxl hide" rows="3">Thank you. Your entry has been successfully submitted.</textarea>
                                <input id="confirmMsg_url" name="CFMURL" class="xxl hide" type="text" placeholder="http://" />
                            </li>
                        </ul>
                    </li>--%>
                    <!--
  	<li class="left half">
  		<label for="language" class="desc">语言 <a href="#" class="help"  title="关于语言" rel="此属性用于指定系统提示信息所使用的语言，当用护访问表单时系统的提示信息（如提交时的验证错误信息）将以此语言显示。目前仅支持简体中文和英文两种语言。">(?)</a></label>
		<select id="language" name="LANG" class="xxl">
			<option value="cn">简单中文</option>
			<option value="en">English</option>
		</select>
    </li>
  <li class="clear noheight"></li>
  <li id="liGoods" class="hide">
  <fieldset>
		<legend>商品相关</legend>
		<ul>
		<li id="liSale" style="display: list-item;">
		<input id="sale" name="SALE" type="checkbox" value="1"> <label for="sale">促销:</label>
			满   <input type="text" id="salem" name="SALEM" disabled class="number" style="width:50px"/>
			减   <input type="text" id="salej" name="SALEJ" disabled class="number" style="width:50px"/>
		<a href="#" class="help" title="关于商品促销" rel="当表单中有商品字段时，可以进行“满就减”的促销活动，当金额达到一定量时自动减掉相应金额，助您提升商品销量。">(?)</a>
		</li>
		<li id="liPay">
		<div class="highlight">
			<input type="checkbox" id="chkAliPay" value="1" name="ALIPAY"/> <label class="bold" for="chkAliPay">在线支付</label> <a class="help" href="#" title="关于在线支付" rel="目前仅支持支付宝在线支付，需要开通支付宝商家服务才能使用。<a href='help/formbuilder.html#t62' target='_blank'>查看开通方法</a>">(?)</a>
			<div id="divPay" class="hide">
			<a href="#" id="btnPaySetting" class="btn no-icon btn-gray">配置支付参数</a><br/>
		  	<label>不跳转到在线支付条件</label> <a class="help" href="#" title="关于不跳转条件" rel="您可以根据用户填写的数据来确定是否跳转到在线支付页面，当满足如下条件时将不进行跳转，默认跳转。比如您可以添加一个“付款方式”的单选框，并在此设置当选择“货到付款”时不跳转到在线支付。">(?)</a>
		  	<div id="noAliConditionDiv" style="margin-bottom: 5px;">
		  	<select id="noAliConditionField" name="PAYCONFLD" style="width:115px;"></select>
		  	等于
		  	<select id="noAliConditionValue" name="PAYCONVAL" style="width:115px;"></select>
		  	</div>
		  	</div>
		</div>
		</li>
		
		<li style="text-align: center;display:none;">
			<a style="text-decoration: underline;color:#3B699F;" target="_blank" href="jsform-setup.msi">下载小票自动打印程序</a>
			<a href="#" class="help" title="关于小票自动打印程序" rel="小票打印程序是专为商品类表单设计的一款应用程序。安装后，当有新订单提交时将会自动打印出小票，您要做的仅是“见单送货”。非常适合外卖及实体店铺开展电子商务的应用场景。">(?)</a>
		</li>
		
		</ul>
	</fieldset>
  </li> 
  <li class="clear noheight"></li>
  
  <li id="liConfirm" class="clear">
  <fieldset class="confirm"><legend>跳转选项</legend>
  <ul>
    <li class="left">
	    <input id="confirmType_text" name="CFMTYP" value="T" checked="checked" type="radio"/>
	    <label for="confirmType_text">显示文本</label>
	    <a href="#" class="help" title="关于显示文本" rel="当用户提交表单成功时，系统将跳转至默认的确认页面，页面中将显示下面文本框设定的内容。">(?)</a>
    </li>

    <li class="right">
    	<input id="confirmType_url" name="CFMTYP" value="U" type="radio"/>
    	<label for="confirmType_url">跳转至网页</label>
    	<a href="#" class="help" title="关于跳转至网页" rel="当用户提交表单成功时，系统将转至下面文本框所设定的网址，而不是默认的确认页面。">(?)</a>
    </li>
    <li class="clear">
	<textarea id="confirmMsg_text" name="CFMMSG" class="xxl hide" rows="3">Thank you. Your entry has been successfully submitted.</textarea>
	<input id="confirmMsg_url" name="CFMURL" class="xxl hide" type="text" value="http://" />
	</li>
  </ul>
	</fieldset>
	</li>
	<li class="clear noheight"></li>
	<li>
	  <fieldset><legend>填写控制</legend>
	  <ul>
	    <li>
	    	<label for="captcha">验证码
	    	<a href="#" class="help" title="关于验证码" rel="通过一张只有人眼能识别而电脑无法识别的验证码图片来确定表单是手工提交，而 不是通过软件进行恶意的大量提交。默认情况下，系统将根据同一IP提交频率自动决定是否显示验证码。您也可以选择一直显示验证码或从不显示验证码。从不显示验证码通常用于在某个局域网络有多次提交的情况，用以简化用户输入。">(?)</a></label>
		    <div>
			    <select id="captcha" class="m" name="CAPTCHA">
				    <option value="1">自动 (推荐)</option>
				    <option value="2">一直显示</option>
				    <option value="0">从不显示</option>
			    </select>
		    </div>
	    </li>
	    <li>
		    <label for="entriesLimit">达到如下数据量后关闭表单
		    	<a href="#" class="help" title="关于数据量限制" rel="此属性用于指定表单可以收集数据的最大数据量，当达到此数据量后表单将自动关闭。如果不想进行最大数据量限制，请将此处设置为空。">(?)</a>
		    </label>
		    <input id="entriesLimit" class="intnumber m" name="ENLMT" maxlength="8" type="text"/>
	    </li>
	   	<li>
	    	<input id="onePerIp" name="IPLMT" value="1" type="checkbox"/>
	    	<label class="choice" for="onePerIp">每个IP只允许提交一次</label>
	    	<a href="#" class="help" title="关于IP访问限制" rel="此属性用于指定每一个IP地址只能提交一次表单，可以防止用户在同一台计算机上进行多次提交。<br/><i>注意:相对外网而言，同一局域网内的不同IP可能会当作同一IP处理。局域网可通过手机验证码确定唯一性。</i>">(?)</a>
	    	<br/>
	    </li>
	    <li>
	    	<input id="chkAutoFill" type="checkbox" value="1" name="AUTOFILL"/>
		    <label for="chkAutoFill">自动填充上次填写数据</label>
		    <a href="#" class="help" title="自动填充上次填写数据" rel="当某个表单需要同一人（同一台设备）多次填写，并且填写数据变化不大时可以勾选此选项，自动填充上次填写数据，加快输入速度。">(?)</a>
	    </li>
	    <li>
 	<div class="highlight">
 		<input id="schActive" value="1" name="SCHACT" type="checkbox"/>
 		<label class="choice bold" for="schActive">表单只允许在规定的时间范围内访问</label>
 		<a href="#" class="help" title="关于访问时间限制" rel="此属性用于指定表单的有效时间范围。表单将仅在此范围内能够正常访问，过期后将自动关闭。如果不需要进行访问时间限制，请不要勾选此选项。">(?)</a>
 		<div id="listDateRange" class="hide">
 		<div id="startTime" class="oneline overhide reduction color-green start">
 	<label class="h3">开始时间</label>
 	<span>
 		<input class="yyyy" maxlength="4" type="text"/>
 		<label>YYYY</label>
 	</span>
 	<span>
 		<input class="mm" maxlength="2" type="text"/>
 		<label>MM</label>
 	</span>
 	<span>
 		<input class="dd" maxlength="2" type="text"/>
 		<label>DD</label>
 	</span>
 	<span><input type="text" class="hide datepincker"></input></span>
 	<span>
 		<select class="ho">
 			<option value="0">00</option>
 			<option value="1">01</option>
 			<option value="2">02</option>
 			<option value="3">03</option>
 			<option value="4">04</option>
   		<option value="5">05</option>
   		<option value="6">06</option>
   		<option value="7">07</option>
   		<option value="8">08</option>
   		<option value="9">09</option>
   		<option value="10">10</option>
   		<option value="11">11</option>
   		<option selected="selected" value="12">12</option>
   		<option value="13">13</option>
 			<option value="14">14</option>
 			<option value="15">15</option>
 			<option value="16">16</option>
   		<option value="17">17</option>
   		<option value="18">18</option>
   		<option value="19">19</option>
   		<option value="20">20</option>
   		<option value="21">21</option>
   		<option value="22">22</option>
   		<option value="23">23</option>
   	</select>
   	<label>HH</label>
   </span>
   <span>
    <select class="mi">
	    <option value="00">00</option>
	    <option value="15">15</option>
	    <option value="30">30</option>
	    <option value="45">45</option>
  	</select>
  	<label>MM</label>
</span>
</div>
<div id="endTime" class="oneline overhide reduction color-red end">
 	<label class="h3">结束时间</label>
 	<span>
 		<input class="yyyy" maxlength="4" type="text"/>
 		<label>YYYY</label>
 	</span>
 	<span>
 		<input class="mm" maxlength="2" type="text"/>
 		<label>MM</label>
 	</span>
 	<span>
 		<input class="dd" maxlength="2" type="text"/>
 		<label>DD</label>
 	</span>
 	<span><input type="text" class="hide datepincker"></input></span>
 	<span>
 		<select class="ho">
 			<option value="0">00</option>
 			<option value="1">01</option>
 			<option value="2">02</option>
 			<option value="3">03</option>
 			<option value="4">04</option>
   		<option value="5">05</option>
   		<option value="6">06</option>
   		<option value="7">07</option>
   		<option value="8">08</option>
   		<option value="9">09</option>
   		<option value="10">10</option>
   		<option value="11">11</option>
   		<option selected="selected" value="12">12</option>
   		<option value="13">13</option>
 			<option value="14">14</option>
 			<option value="15">15</option>
 			<option value="16">16</option>
   		<option value="17">17</option>
   		<option value="18">18</option>
   		<option value="19">19</option>
   		<option value="20">20</option>
   		<option value="21">21</option>
   		<option value="22">22</option>
   		<option value="23">23</option>
   	</select>
   	<label>HH</label>
   </span>
   <span>
   	<select class="mi">
   		<option value="00">00</option>
   		<option value="15">15</option>
   		<option value="30">30</option>
   		<option value="45">45</option>
   	</select>
   	<label>MM</label>
   </span>
   </div>
   <div class="noheight clear"></div>
   </div>
 	</div>
 </li>
 </ul>
 </fieldset>
 </li>
 
 <li class="clear noheight"></li>
 <li>
  <fieldset><legend>数据查看</legend>
  <ul>
    <li>
    	<label for="chkHideEmpty"><input type="checkbox" value="1" name="HDEMP" id="chkHideEmpty"> 查看数据时隐藏值为空的字段</label>
    	<a title="关于隐藏值为空的字段" rel="当勾选此选项后，查看数据时将不显示值为空的字段。此设置对手机端查看和发送到邮箱的数据副本同样有效。" class="help" href="#">(?)</a>
    	<div class="highlight">
    	<label for="chkPublicData"><input type="checkbox" value="1" name="PUBDT" id="chkPublicData"> 允许未登录用户查询数据</label>
    	<a title="关于允许未登录用户查询数据" rel="当勾选此选项后，将对外发布一个查询页面，未登录的用户也可以通过此页面查询表单提交的数据，通常用于通讯录查询，执行进度查询，成绩查询等场景。" class="help" href="#">(?)</a>
    	<a id="btnPubDataSetting" href="#" class="btn no-icon btn-gray hide">设置详细参数</a>
    	</div>
    </li>
  </ul>
  </fieldset>
 </li>
 -->
                </ul>
            </div>
            <!-- form properties end -->
        </div>
        <!-- right end -->
    </div>
    <!-- container end -->
    
    <!-- 模态框用来预览表格-->
    <div id="previewModal" style="padding:10px 0 20px 20px">

    </div>

    <div class="hide">
        <input id="itemselectbtn" value="选择文件" type="button" />
    </div>

    <div id="overlay" class="overlay hide"></div>
    <div id="lightBox" class="lightbox hide">
        <div id="lbContent" class="lbcontent"></div>
    </div>
    <div id="status" class="status hide">
        <div id="y" class="y" style="top: 0xp; left: 0px">
            <div id="statusText" class="statusText">正在处理...</div>
        </div>
    </div>
    <span id="helpTip" class="helpTip hide"><b></b><em></em></span>
    <script type="text/javascript" src="Scripts/formbuilder/js/head.load.min.js"></script>
    
    <script type="text/javascript">
        var M = { FRMNM: "表单名称", DESC: "", LANG: "cn", LBLAL: "T", CFMTYP: "T", CFMMSG: "提交成功。", SDMAIL: "0", CAPTCHA: "1", IPLMT: "0", SCHACT: "0", INSTR: "0", ISPUB: "1" }
        var F = [];
        var fieldsLimit = 150;
        var goodsNumber = 60;
        var imageNumber = 10;
        var LVL = 4;
        var hasSetRelaField = false;

        var isForTemplate=false;
        M.GID=M.GID || '';
        var resRoot="#",GOODSIMAGEURL="http://goodsimages.jsform.com/",IMAGESURL="#",GOODSIMAGESTYLE="@1e_200w_200h_1c_0i_1o_90Q_1x";
        IMGBUCKET="kmonkey";
        head.js("Scripts/formbuilder/js/jquery-1.7.2.min.js",
            "Scripts/formbuilder/js/jquery-ui-1.8.24.custom.min.js",
            "Scripts/formbuilder/js/wangEditor.min.js?v=20160929",
            "Scripts/formbuilder/js/ajaxfileupload.js?v=20160929",
            "Scripts/formbuilder/js/plupload.full.min.js?v=20160929",
            "Scripts/formbuilder/js/directfileupload.js?v=20160929",
            "Scripts/formbuilder/js/utils.js?v=20160929",
            "Scripts/formbuilder/js/widgets.js?v=20160929",
            "Scripts/formbuilder/js/jquery.mCustomScrollbar.min.js?v=20160929",
            "Scripts/formbuilder/js/jquery.mousewheel.min.js?v=20160929",
            "Scripts/formbuilder/js/formbuilder.js?v=20160929",
            "Scripts/formbuilder/js/formcustom.js?v=20160929",
            "Scripts/formbuilder/js/address-cn.js?v=20160929",
            "Scripts/mobileCommon.js?v=20160929",
            "//cdn.bootcss.com/jquery-confirm/3.1.0/jquery-confirm.min.js?v=20160929");
    </script>
</body>
</html>
