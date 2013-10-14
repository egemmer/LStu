<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%
    ViewData["WebRoot"] = (Request.ApplicationPath == "/") ? "" : Request.ApplicationPath;
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>档案管理系统-用户管理</title>
    <link href="../../Scripts/ExtJS/resources/css/ext-all.css" rel="stylesheet" type="text/css" />
 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/adapter/ext/ext-base.js"></script>
 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/ext-all.js"></script>
 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/Security.js"></script>
</head>
<body>
    <script type="text/javascript">
        Ext.onReady(function () {
            var WebRoot = '<%=ViewData["WebRoot"] %>';

            var viewport = new Ext.Viewport({
                layout: 'border',
                frame: true,
                items: [
                {
                    region: 'center',
                    items: [
                    {
                        title: '项目阶段',
                        collapsible: true,
                        items: [
                        {
                            xtype: 'listview',
                            id: 'stageListview',
                            height: 180,
                            store: new Ext.data.JsonStore({
                                id: 'stageStore',
                                url: WebRoot + '/ProjectStage/List',
                                autoLoad: true,
                                fields: ['ID', 'Name']
                            }),
                            autoWidth: true,
                            singleSelect: true,
                            disableHeaders: true,
                            trackOver: true,
                            reserveScrollOffset: true,
                            columns: [{
                                header: '专业名称',
                                dataIndex: 'Name'
                            }]
                        }],
                        tbar: [
                        {
                            id: 'stageName',
                            xtype: 'textfield',
                            allowBlank: false
                        }, {
                            text: '添加',
                            width: 60,
                            handler: function () {
                                var field = Ext.getCmp('stageName');

                                if (field.isValid()) {
                                    Ext.Ajax.request({
                                        url: WebRoot + '/ProjectStage/Create',
                                        method: 'post',
                                        params: { Name: field.getValue() },
                                        success: function (resp, opts) {
                                            var data = Ext.util.JSON.decode(resp.responseText);
                                            if (data.errors == 'B000') {
                                                Ext.getCmp('stageListview').store.reload();
                                            } else {
                                                Ext.Msg.alert('操作失败', data.message);
                                            }
                                        }
                                    });
                                }
                            }
                        }, {
                            text: '删除',
                            width: 60,
                            handler: function () {
                                var listview = Ext.getCmp('stageListview');
                                var records = listview.getSelectedRecords();
                                if (records == null || records.length == 0) {
                                    Ext.Msg.alert('操作提示', '请选择删除记录');
                                } else {
                                    Ext.Msg.confirm('操作确认', '确定要删除该项目阶段吗？', function (btn) {
                                        if (btn == 'yes') {
                                            Ext.Ajax.request({
                                                url: WebRoot + '/ProjectStage/Delete',
                                                method: 'post',
                                                params: { ID: records[0].get('ID') },
                                                success: function (resp, opts) {
                                                    var data = Ext.util.JSON.decode(resp.responseText);
                                                    if (data.errors == 'B000') {
                                                        Ext.getCmp('stageListview').store.reload();
                                                    } else {
                                                        Ext.Msg.alert('操作失败', data.message);
                                                    }
                                                }
                                            });
                                        }
                                    });
                                }
                            }
                        }]
                    }, {
                        title: '项目专业',
                        items: [
                        {
                            xtype: 'listview',
                            id: 'subjectListview',
                            height: 180,
                            store: new Ext.data.JsonStore({
                                id: 'subjectStore',
                                url: WebRoot + '/ProjectSubject/List',
                                autoLoad: true,
                                fields: ['ID', 'Name']
                            }),
                            autoWidth: true,
                            singleSelect: true,
                            disableHeaders: true,
                            trackOver: false,
                            reserveScrollOffset: true,
                            columns: [{
                                header: '专业名称',
                                dataIndex: 'Name'
                            }]
                        }],
                        tbar: [
                        {
                            id: 'subjectName',
                            xtype: 'textfield',
                            allowBlank: false
                        }, {
                            text: '添加',
                            width: 60,
                            handler: function () {
                                var field = Ext.getCmp('subjectName');

                                if (field.isValid()) {
                                    Ext.Ajax.request({
                                        url: WebRoot + '/ProjectSubject/Create',
                                        method: 'post',
                                        params: { Name: field.getValue() },
                                        success: function (resp, opts) {
                                            var data = Ext.util.JSON.decode(resp.responseText);
                                            if (data.errors == 'B000') {
                                                Ext.getCmp('subjectListview').store.reload();
                                            } else {
                                                Ext.Msg.alert('操作失败', data.message);
                                            }
                                        }
                                    });
                                }
                            }
                        }, {
                            text: '删除',
                            width: 60,
                            handler: function () {
                                var listview = Ext.getCmp('subjectListview');
                                var records = listview.getSelectedRecords();
                                if (records == null || records.length == 0) {
                                    Ext.Msg.alert('操作提示', '请选择删除记录');
                                } else {
                                    Ext.Msg.confirm('操作确认', '确定要删除该项目专业吗？', function (btn) {
                                        if (btn == 'yes') {
                                            Ext.Ajax.request({
                                                url: WebRoot + '/ProjectSubject/Delete',
                                                method: 'post',
                                                params: { ID: records[0].get('ID') },
                                                success: function (resp, opts) {
                                                    var data = Ext.util.JSON.decode(resp.responseText);
                                                    if (data.errors == 'B000') {
                                                        Ext.getCmp('subjectListview').store.reload();
                                                    } else {
                                                        Ext.Msg.alert('操作失败', data.message);
                                                    }
                                                }
                                            });
                                        }
                                    });
                                }
                            }
                        }]
                    }]
                }]
            });
        });
    </script>
</body>
</html>
