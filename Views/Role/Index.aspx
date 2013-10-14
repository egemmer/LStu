<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%
    ViewData["WebRoot"] = (Request.ApplicationPath == "/") ? "" : Request.ApplicationPath;
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>档案管理系统-角色管理</title>
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
                url: WebRoot + '/Role/List',
                fields: [
                'Code',
                'Name',
                'Description'
                ]
            });

            var roleWindow = new Ext.Window({
                width: 400,
                autoHeight: true,
                closable: false,
                resizable: false,
                modal: true,
                items: [
            {
                xtype: 'form',
                layout: 'form',
                id: 'roleForm',
                bodyStyle: 'padding: 10px;',
                items: [
                {
                    xtype: 'textfield',
                    id: 'Code',
                    fieldLabel: '角色码',
                    width: 200
                },
                {
                    xtype: 'textfield',
                    id: 'Name',
                    fieldLabel: '角色名',
                    width: 200
                },
                {
                    xtype: 'textarea',
                    id: 'Description',
                    fieldLabel: '简介',
                    width: 200
                }]
            }],
                buttons: [
            {
                xtype: 'button',
                text: '确定',
                listeners: {
                    click: function () {
                        var basicForm = Ext.getCmp('roleForm').getForm();
                        if (basicForm.isValid()) {
                            basicForm.submit({
                                success: function (form, action) {
                                    store.reload();
                                    roleWindow.hide();
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
                        roleWindow.hide();
                    }
                }
            }]
            });

            var auth_sm = new Ext.grid.CheckboxSelectionModel({ singleSelect: false, dataIndex: 'Code' });

            var authWindow = new Ext.Window({
                width: 500,
                autoHeight: true,
                closable: false,
                resizable: false,
                modal: true,
                items: [
                    {
                        xtype: 'hidden',
                        id: 'Role_Code'
                    },
                    {
                        xtype: 'grid',
                        store: new Ext.data.JsonStore({
                            id: 'authStore',
                            url: WebRoot + '/Permission/List',
                            fields: [
                            'Code',
                            'Name',
                            'Description'
                            ],
                            autoLoad: true
                        }),
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
                        sm: auth_sm,
                        frame: true,
                        columns: [
                                    auth_sm,
                                    { header: "代码", sortable: false, dataIndex: 'Code' },
                                    { header: "功能名称", sortable: false, dataIndex: 'Name' },
                                    { header: "简介", sortable: false, dataIndex: 'Description' }
                             ],
                        minColumnWidth: 85,
                        autoHeight: true,
                        autoscroll: true,
                        enableColumnMove: false
                    }
                ],
                buttons: [
                    {
                        xtype: 'button',
                        text: '确定',
                        listeners: {
                            click: function () {
                                var selected = sm.getSelected();
                                var roleCode = selected.get('Code');
                                var resourceCodes = '';

                                var selections = auth_sm.getSelections();
                                for (var i = 0; i < selections.length; i++) {
                                    resourceCodes += selections[i].get('Code') + ';';
                                }

                                Ext.Ajax.request({
                                    url: WebRoot + '/Permission/Authorize',
                                    method: 'post',
                                    params: { roleCode: roleCode, resourceCodes: resourceCodes },
                                    success: function (r, options) {
                                        var response = Ext.util.JSON.decode(r.responseText);
                                        if (response.success) {
                                            Ext.Msg.alert('操作成功', '已授权');
                                            authWindow.hide();
                                        } else {
                                            Ext.Msg.alert('操作结果', response.message);
                                        }
                                    }
                                });
                            }
                        }
                    }, {
                        xtype: 'button',
                        text: '关闭',
                        listeners: {
                            click: function () {
                                authWindow.hide();
                            }
                        }
                    }
                ]
            });

            var sm = new Ext.grid.CheckboxSelectionModel({ singleSelect: true, dataIndex: 'Code' });

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
                                    { header: "角色码", sortable: false, dataIndex: 'Code' },
                                    { header: "角色名", sortable: false, dataIndex: 'Name' },
                                    { header: "简介", sortable: false, dataIndex: 'Description' }
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
                                        var basicForm = Ext.getCmp('roleForm').getForm();
                                        basicForm.url = WebRoot + '/Role/Create';
                                        basicForm.findField('Code').setReadOnly(false);
                                        basicForm.setValues({ Code: '', Name: '', Description: '' });
                                        roleWindow.show();
                                    }
                                }
                            },
                            {
                                xtype: 'button',
                                text: '修改',
                                listeners: {
                                    click: function () {
                                        if (sm.hasSelection()) {
                                            var basicForm = Ext.getCmp('roleForm').getForm();
                                            basicForm.url = WebRoot + '/Role/Update';
                                            basicForm.findField('Code').setReadOnly(true);
                                            var selected = sm.getSelected();
                                            basicForm.setValues(selected.data);
                                            roleWindow.show();
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
                                            var selected = sm.getSelected();

                                            Ext.Ajax.request({
                                                url: WebRoot + '/Role/Delete',
                                                method: 'post',
                                                params: { code: selected.get('Code') },
                                                success: refresh
                                            });
                                        } else {
                                            Ext.Msg.alert("消息提醒", "请选择删除的记录！");
                                        }
                                    }
                                }
                            }, {
                                xtype: 'button',
                                text: '授权',
                                listeners: {
                                    click: function () {
                                        if (sm.hasSelection()) {
                                            var selected = sm.getSelected();
                                            var roleCode = selected.get('Code');

                                            if (auth_sm.hasSelection()) {
                                                auth_sm.clearSelections();
                                            }

                                            Ext.Ajax.request({
                                                url: WebRoot + '/Permission/HasPermissions',
                                                method: 'post',
                                                params: { roleCode: roleCode },
                                                success: function (r, options) {
                                                    var items = Ext.util.JSON.decode(r.responseText);
                                                    for (var i = 0; i < items.length; i++) {
                                                        var store = auth_sm.grid.store;
                                                        for (var n = 0; n < store.getCount(); n++) {
                                                            var record = store.getAt(n);
                                                            if (record.get('Code') == items[i].Resource_Code) {
                                                                auth_sm.selectRow(n, true);
                                                            }
                                                        }
                                                    }
                                                }
                                            });

                                            authWindow.show();
                                        } else {
                                            Ext.Msg.alert("消息提醒", "请选择一条用户记录！");
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
