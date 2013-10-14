<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%
    ViewData["WebRoot"] = (Request.ApplicationPath == "/") ? "" : Request.ApplicationPath;
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>档案管理系统-用户管理</title>
    <link rel="stylesheet" href="../../Scripts/ExtJS/resources/css/ext-all.css" />
    <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/adapter/ext/ext-base.js"></script>
    <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/ext-all.js"></script>
    <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/Security.js"></script>
</head>
<body>
    <script type="text/javascript">
        Ext.onReady(function () {
            var WebRoot = '<%=ViewData["WebRoot"] %>';

            var store = new Ext.data.JsonStore({
                url: WebRoot + '/User/List',
                fields: [
                'Login_ID',
                'Username',
                'Department_ID',
                'Department_Name',
                'Role_Code',
                'Role_Name'
                ]
            });

            var GetUserWindow = function () {
                var userWindow = new Ext.Window({
                    width: 500,
                    autoHeight: true,
                    closable: false,
                    resizable: false,
                    modal: true,
                    items: [
            {
                xtype: 'form',
                layout: 'form',
                id: 'userForm',
                bodyStyle: 'padding: 10px;',
                items: [
                            {
                                xtype: 'textfield',
                                id: 'Login_ID',
                                fieldLabel: '登录帐号',
                                allowBlank: false
                            },
                            {
                                xtype: 'textfield',
                                id: 'Username',
                                fieldLabel: '用户名',
                                allowBlank: false
                            },
                            {
                                xtype: 'combo',
                                fieldLabel: '部门',
                                emptyText: '请选择...',
                                hiddenName: 'Department_ID',
                                triggerAction: 'all',
                                valueField: 'ID',
                                displayField: 'Name',
                                mode: 'local',
                                typeAhead: true,
                                allowBlank: false,
                                forceSelection: true,
                                store: new Ext.data.JsonStore({
                                    url: WebRoot + '/Department/List',
                                    idProperty: 'ID',
                                    fields: ['ID', 'Name'],
                                    autoLoad: true
                                })
                            },
                            {
                                xtype: 'combo',
                                fieldLabel: '角色',
                                emptyText: '请选择...',
                                hiddenName: 'Role_Code',
                                triggerAction: 'all',
                                valueField: 'Code',
                                displayField: 'Name',
                                mode: 'local',
                                typeAhead: true,
                                allowBlank: false,
                                forceSelection: true,
                                store: new Ext.data.JsonStore({
                                    url: WebRoot + '/Role/List',
                                    idProperty: 'Code',
                                    fields: ['Code', 'Name'],
                                    autoLoad: true
                                })
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
                                var basicForm = Ext.getCmp('userForm').getForm();
                                if (basicForm.isValid()) {
                                    basicForm.submit({
                                        success: function (form, action) {
                                            store.load();
                                            userWindow.close();
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
                                userWindow.close();
                            }
                        }
                    }
                ]
                });
                return userWindow;
            };

            var sm = new Ext.grid.CheckboxSelectionModel({ singleSelect: true, dataIndex: 'Login_ID' });

            var refresh = function () {
                store.load();
            }

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
                                    { header: "登录帐号", width: 85, sortable: false, dataIndex: 'Login_ID' },
                                    { header: "用户名", width: 85, sortable: false, dataIndex: 'Username' },
                                    { header: "部门", width: 85, sortable: false, dataIndex: 'Department_Name' },
                                    { header: "角色", width: 75, sortable: false, dataIndex: 'Role_Name' }
                             ],
                        minColumnWidth: 85,
                        autoHeight: true,
                        autoscroll: true,
                        enableColumnMove: false,
                        iconCls: 'icon-grid',
                        buttons: [
                            {
                                xtype: 'button',
                                text: '新增',
                                listeners: {
                                    click: function () {
                                        GetUserWindow().show();
                                        var basicForm = Ext.getCmp('userForm').getForm();
                                        basicForm.url = WebRoot + '/User/Create';
                                    }
                                }
                            },
                            {
                                xtype: 'button',
                                text: '修改',
                                listeners: {
                                    click: function () {
                                        if (sm.hasSelection()) {
                                            GetUserWindow().show();
                                            var basicForm = Ext.getCmp('userForm').getForm();
                                            var selected = sm.getSelected();
                                            basicForm.url = WebRoot + '/User/Update';
                                            basicForm.setValues(selected.data);
                                        } else {
                                            Ext.Msg.alert("消息提醒", "请选择一条修改的记录！");
                                        }
                                    }
                                }
                            }, {
                                xtype: 'button',
                                text: '删除',
                                listeners: {
                                    click: function () {
                                        if (sm.hasSelection()) {
                                            Ext.Msg.confirm("确认检查", "确定要删除吗?", function (b) {
                                                if (b == 'no') return;
                                                var selected = sm.getSelected();
                                                Ext.Ajax.request({
                                                    url: WebRoot + '/User/Delete',
                                                    method: 'post',
                                                    params: { Login_ID: selected.get('Login_ID') },
                                                    success: refresh
                                                });
                                            });
                                        } else {
                                            Ext.Msg.alert("消息提醒", "请选择一条删除的记录！");
                                        }
                                    }
                                }
                            }, {
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
