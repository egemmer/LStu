<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>档案管理系统-用户管理</title>
    <link rel="stylesheet" href="Scripts/ExtJS/resources/css/ext-all.css" />
    <script language="javascript" type="text/javascript" src="Scripts/ExtJS/adapter/ext/ext-base.js"></script>
    <script language="javascript" type="text/javascript" src="Scripts/ExtJS/ext-all.js"></script>
    <script language="javascript" type="text/javascript" src="Scripts/Security.js"></script>
</head>
<body>
    <script type="text/javascript">
        Ext.onReady(function () {

            var store = new Ext.data.JsonStore({
                url: '/Project/List',
                fields: [
                'ID',
                'Project_Code',
                'Project_Name',
                'Project_Date',
                'Business_Owner'
                ]
            });

            var addWindow = new Ext.Window({
                width: 500,
                authHeight: true,
                closable: false,
                resizable: false,
                modal: true,
                items: [
                {
                    xtype: 'form',
                    id: 'addForm',
                    url: '/Project/Create',
                    bodyStyle: 'padding: 10px;',
                    items: [
                                { xtype: 'textfield', id: 'Project_Code', width: 250, fieldLabel: '项目编码', allowBlank: false },
                                { xtype: 'textfield', id: 'Project_Name', width: 250, fieldLabel: '项目名称', allowBlank: false },
                                { xtype: 'datefield', id: 'Project_Date', width: 250, fieldLabel: '项目日期', allowBlank: false, format: 'Y-m-d' },
                                { xtype: 'textfield', id: 'Business_Owner', width: 250, fieldLabel: '项目业主' }
                            ]
                }],
                buttons: [
                    {
                        xtype: 'button',
                        text: '确定',
                        listeners: {
                            click: function () {
                                var basicForm = Ext.getCmp('addForm').getForm();
                                if (basicForm.isValid()) {
                                    basicForm.submit({
                                        success: function (form, action) {
                                            store.load();
                                            addWindow.hide();
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
                                addWindow.hide();
                            }
                        }
                    }
                ]
            }
            );

            var updateWindow = new Ext.Window({
                width: 500,
                authHeight: true,
                closable: false,
                resizable: false,
                modal: true,
                items: [
                {
                    xtype: 'form',
                    id: 'updateForm',
                    url: '/Project/Update',
                    bodyStyle: 'padding: 10px;',
                    items: [
                                { xtype: 'hidden', id: 'ID' },
                                { xtype: 'textfield', id: 'Project_Code', width: 250, fieldLabel: '项目编码', readOnly: true },
                                { xtype: 'textfield', id: 'Project_Name', width: 250, fieldLabel: '项目名称', allowBlank: false },
                                { xtype: 'datefield', id: 'Project_Date', width: 250, fieldLabel: '项目日期', allowBlank: false, format: 'Y-m-d' },
                                { xtype: 'textfield', id: 'Business_Owner', width: 250, fieldLabel: '项目业主' }
                            ]
                }],
                buttons: [
                    {
                        xtype: 'button',
                        text: '确定',
                        listeners: {
                            click: function () {
                                var basicForm = Ext.getCmp('updateForm').getForm();
                                if (basicForm.isValid()) {
                                    basicForm.submit({
                                        success: function (form, action) {
                                            store.load();
                                            updateWindow.hide();
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
                                updateWindow.hide();
                            }
                        }
                    }
                ]
            });

            var sm = new Ext.grid.CheckboxSelectionModel({ singleSelect: true, dataIndex: 'ID' });

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
                        viewConfig: {
                            forceFit: true
                        },
                        sm: sm,
                        frame: true,
                        columns: [
                                    sm,
                                    { header: "ID", width: 85, sortable: true, dataIndex: 'ID' },
                                    { header: "项目编码", width: 85, sortable: true, dataIndex: 'Project_Code' },
                                    { header: "项目名称", width: 85, sortable: true, dataIndex: 'Project_Name' },
                                    { header: "项目日期", width: 75, sortable: false, dataIndex: 'Project_Date' },
                                    { header: "项目业主", width: 75, sortable: false, dataIndex: 'Business_Owner' }
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
                                        addWindow.show();
                                    }
                                }
                            },
                            {
                                xtype: 'button',
                                text: '修改',
                                listeners: {
                                    click: function () {
                                        if (sm.hasSelection()) {
                                            var basicForm = Ext.getCmp('updateForm').getForm();
                                            var selected = sm.getSelected();
                                            basicForm.setValues(selected.data);
                                            updateWindow.show();
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
                                            Ext.Msg.confirm("确认检查", "确定要删除吗?<br/>删除项目, 则项目下所有资料全部删除。", function (b) {
                                                if (b == 'no') return;
                                                var selected = sm.getSelected();
                                                Ext.Ajax.request({
                                                    url: '/Project/Delete',
                                                    method: 'post',
                                                    params: { ID: selected.get('ID') },
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
