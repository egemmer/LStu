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

            var store = new Ext.data.JsonStore({
                url: WebRoot + '/Department/List',
                fields: ['ID', 'Name', 'Description']
            });

            var sm = new Ext.grid.CheckboxSelectionModel({ singleSelect: true, dataIndex: 'ID' });

            var refresh = function () {
                store.load();
            }

            var departmentAdd = new Ext.Window({
                width: 500,
                autoHeight: true,
                closable: false,
                resizable: false,
                modal: true,
                items: [
                    {
                        xtype: 'form',
                        layout: 'form',
                        id: 'addForm',
                        bodyStyle: 'padding: 10px;',
                        items: [
                            {
                                xtype: 'textfield',
                                fieldLabel: '部门名称',
                                name: 'Name',
                                allowBlank: false,
                                width: 300
                            }, {
                                xtype: 'textarea',
                                fieldLabel: '简介',
                                name: 'Description',
                                width: 300
                            }
                        ]
                    }
                ],
                buttons: [
                    {
                        xtype: 'button',
                        text: '确定',
                        listeners: {
                            click: function () {
                                var basicForm = Ext.getCmp('addForm').getForm();
                                if (basicForm.isValid()) {
                                    basicForm.submit({
                                        url: WebRoot + '/Department/Create',
                                        success: function (form, action) {
                                            store.load();
                                            departmentAdd.hide();
                                        },
                                        failure: function (form, action) {
                                            Ext.Msg.alert('错误提示', action.result.message);
                                        }
                                    });
                                }
                            }
                        }
                    }, {
                        xtype: 'button',
                        text: '关闭',
                        listeners: {
                            click: function () {
                                departmentAdd.hide();
                            }
                        }
                    }
                ]
            });

            var departmentUpdate = new Ext.Window({
                width: 500,
                autoHeight: true,
                closable: false,
                resizable: false,
                modal: true,
                items: [
                    {
                        xtype: 'form',
                        id: 'updateForm',
                        layout: 'form',
                        bodyStyle: 'padding: 10px;',
                        items: [
                            {
                                xtype: 'hidden',
                                id: 'ID'
                            }, {
                                xtype: 'textfield',
                                fieldLabel: '部门名称',
                                id: 'Name',
                                allowBlank: false,
                                width: 300
                            }, {
                                xtype: 'textarea',
                                fieldLabel: '简介',
                                id: 'Description',
                                width: 300
                            }
                        ]
                    }
                ],
                buttons: [
                    {
                        xtype: 'button',
                        text: '确定',
                        listeners: {
                            click: function () {
                                var basicForm = Ext.getCmp('updateForm').getForm();
                                if (basicForm.isValid()) {
                                    basicForm.submit({
                                        url: WebRoot + '/Department/Update',
                                        success: function (form, action) {
                                            store.load();
                                            departmentUpdate.hide();
                                        },
                                        failure: function (form, action) {
                                            Ext.Msg.alert('错误提示', action.result.message);
                                        }
                                    });
                                }
                            }
                        }
                    }, {
                        xtype: 'button',
                        text: '关闭',
                        listeners: {
                            click: function () {
                                departmentUpdate.hide();
                            }
                        }
                    }
                ]
            });

            new Ext.Viewport({
                layout: 'border',
                items: [
                    {
                        region: 'center',
                        xtype: 'grid',
                        store: store,
                        stripeRows: true,
                        loadMask: true,
                        trackMouseOver: false,
                        disableSelection: true,
                        enableHdMenu: false,
                        viewConfig: {
                            forceFit: true,
                            enableRowBody: true,
                            showPreview: true
                        },
                        sm: sm,
                        frame: true,
                        columns: [
                                    sm,
                                    { header: "编号", id: 'ID', sortable: false, dataIndex: 'ID' },
                                    { header: "部门名称", id: 'Name', sortable: false, dataIndex: 'Name' },
                                    { header: "简介", id: 'Description', sortable: false, dataIndex: 'Description' }
                             ],
                        minColumnWidth: 85,
                        autoHeight: true,
                        autoscroll: true,
                        enableColumnMove: false,
                        iconCls: 'icon-grid',
                        listeners: {
                            rowclick: function (s, rowIndex, e) {

                            }
                        },
                        buttons: [
                                {
                                    xtype: 'button',
                                    text: '新增',
                                    listeners: {
                                        click: function () {
                                            Ext.getCmp('addForm').getForm().setValues({ Name: '', Description: '' });
                                            departmentAdd.show();
                                        }
                                    }
                                },
                                {
                                    xtype: 'button',
                                    text: '修改',
                                    listeners: {
                                        click: function () {
                                            if (sm.hasSelection()) {
                                                var selected = sm.getSelected();
                                                Ext.getCmp('updateForm').getForm().setValues(selected.data);
                                                departmentUpdate.show();
                                            } else {
                                                Ext.Msg.alert("消息提醒", "请选择一条修改的记录！");
                                            }
                                        }
                                    }
                                },
                                {
                                    xtype: 'button',
                                    text: '删除',
                                    listeners: {
                                        click: function () {
                                            if (sm.hasSelection()) {
                                                Ext.Msg.confirm("确认检查", "确定要删除吗?", function (b) {
                                                    if (b == 'no') return;
                                                    var selected = sm.getSelected();
                                                    Ext.Ajax.request({
                                                        url: WebRoot + '/Department/Delete',
                                                        method: 'post',
                                                        params: { Department_ID: selected.get('ID') },
                                                        success: refresh
                                                    });
                                                });
                                            } else {
                                                Ext.Msg.alert("消息提醒", "请选择一条删除的记录！");
                                            }
                                        }
                                    }
                                },
                                {
                                    xtype: 'button',
                                    text: '刷新',
                                    listeners: {
                                        click: refresh
                                    }
                                }
                             ]
                    }
                ]
            });

            refresh();
        });
    </script>
</body>
</html>
