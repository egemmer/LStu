<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%
    ViewData["WebRoot"] = (Request.ApplicationPath == "/") ? "" : Request.ApplicationPath;
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>档案管理系统</title>
    <link rel="stylesheet" href="../../Scripts/ExtJS/resources/css/ext-all.css" />
    <link rel="stylesheet" href="../../Scripts/ExtJS/resources/css/fileuploadfield.css" />
    <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/adapter/ext/ext-base.js"></script>
    <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/ext-all.js"></script>
    <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/fileUpload/FileUploadField.js"></script>
    <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/Security.js"></script>
</head>
<body>
    <script type="text/javascript">
        Ext.onReady(function () {
            var WebRoot = '<%=ViewData["WebRoot"] %>';
            var pageSize = 15;

            var store = new Ext.data.JsonStore({
                url: WebRoot + '/Search/List',
                totalProperty: 'count',
                idProperty: 'Barcode',
                root: 'items',
                fields: [
                'Barcode',
                'Project_Code',
                'Project_Stage',
                'Project_Name',
                'Project_Subject',
                'File_Name',
                'Diagram_Name',
                'Diagram_Code',
                'Diagram_Version',
                'Diagram_Scale',
                'Created_Date',
                'Plotter'
                ]
            });

            var GetProfileFrame = function () {
                var frameWindow = new Ext.Window({
                    title: '新增档案',
                    width: 420,
                    autoHeight: true,
                    resize: false,
                    modal: true,
                    resizable: false,
                    closable: false,
                    items: [{
                        id: 'profileForm',
                        xtype: 'form',
                        frame: true,
                        layout: 'form',
                        url: WebRoot + '/Profile/Create',
                        method: 'post',
                        fileUpload: true,
                        items: [{
                            xtype: 'hidden',
                            name: 'Project_Name'
                        }, {
                            xtype: 'fileuploadfield',
                            fieldLabel: '文档',
                            name: 'UploadFile',
                            allowBlank: false,
                            width: 280
                        }, {
                            xtype: 'combo',
                            fieldLabel: '项目编码',
                            name: 'Project_Code',
                            emptyText: '请选择...',
                            hiddenName: 'Project_Code',
                            valueField: 'Project_Code',
                            displayField: 'Project_Code',
                            allowBlank: false,
                            triggerAction: 'all',
                            mode: 'local',
                            typeAhead: true,
                            allowBlank: false,
                            forceSelection: true,
                            editable: false,
                            width: 280,
                            store: new Ext.data.JsonStore({
                                url: WebRoot + '/Project/List',
                                idProperty: 'Project_Code',
                                fields: ['Project_Code', 'Project_Name'],
                                autoLoad: true
                            }),
                            listeners: {
                                select: function (combo, record, index) {
                                    var basicForm = Ext.getCmp('profileForm').getForm();
                                    basicForm.setValues({ Project_Name: record.get('Project_Name') });
                                }
                            }
                        }, {
                            xtype: 'combo',
                            fieldLabel: '项目阶段',
                            name: 'Project_Stage',
                            emptyText: '请选择...',
                            hiddenName: 'Project_Stage',
                            valueField: 'Project_Stage',
                            displayField: 'Project_Stage',
                            allowBlank: false,
                            triggerAction: 'all',
                            mode: 'local',
                            typeAhead: true,
                            allowBlank: false,
                            forceSelection: true,
                            editable: false,
                            width: 280,
                            store: new Ext.data.JsonStore({
                                url: WebRoot + '/Search/StageRecords',
                                idProperty: 'Name',
                                fields: [{ name: 'Project_Stage', mapping: 'Name'}],
                                autoLoad: true
                            })
                        }, {
                            xtype: 'combo',
                            fieldLabel: '项目专业',
                            name: 'Project_Subject',
                            emptyText: '请选择...',
                            hiddenName: 'Project_Subject',
                            valueField: 'Project_Subject',
                            displayField: 'Project_Subject',
                            allowBlank: false,
                            triggerAction: 'all',
                            mode: 'local',
                            typeAhead: true,
                            allowBlank: false,
                            forceSelection: true,
                            editable: false,
                            width: 280,
                            store: new Ext.data.JsonStore({
                                url: WebRoot + '/Search/SubjectRecords',
                                idProperty: 'Name',
                                fields: [{ name: 'Project_Subject', mapping: 'Name'}],
                                autoLoad: true
                            })
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '图纸名称',
                            name: 'Diagram_Name',
                            allowBlank: false,
                            width: 280
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '图纸编码',
                            name: 'Diagram_Code',
                            allowBlank: false,
                            width: 280
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '图纸比例',
                            name: 'Diagram_Scale',
                            width: 280
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '图纸版本',
                            name: 'Diagram_Version',
                            width: 280
                        }]
                    }],
                    buttons: [{
                        text: '保存',
                        handler: function () {
                            var basicForm = Ext.getCmp('profileForm').getForm();
                            if (basicForm.isValid()) {
                                basicForm.submit({
                                    waitTitle: '请等待',
                                    waitMsg: '正在处理中 ...',
                                    success: function () {
                                        Ext.Msg.alert('操作提示', "保存已成功。");
                                        frameWindow.close();
                                    },
                                    failure: function (form, action) {
                                        Ext.Msg.alert('错误提示', "操作失败。");
                                    }
                                });
                            }
                        }
                    }, {
                        text: '关闭',
                        handler: function () {
                            frameWindow.close();
                        }
                    }]
                });
                return frameWindow;
            }

            var GetApplyFrame = function () {
                var applyFrame = new Ext.Window({
                    title: '申请借阅',
                    width: 360,
                    modal: true,
                    closable: false,
                    resizable: false,
                    items: [{
                        id: 'applyForm',
                        xtype: 'form',
                        frame: true,
                        url: WebRoot + '/Search/Apply',
                        method: 'POST',
                        labelSeparator: '',
                        labelWidth: 80,
                        items: [{
                            xtype: 'hidden',
                            name: 'Barcode'
                        }, {
                            xtype: 'textfield',
                            name: 'Project_Code',
                            width: 230,
                            readOnly: true,
                            fieldLabel: '项目编码'
                        }, {
                            xtype: 'textfield',
                            name: 'Project_Name',
                            width: 230,
                            readOnly: true,
                            fieldLabel: '项目名称'
                        }, {
                            xtype: 'textfield',
                            name: 'Project_Stage',
                            width: 230,
                            readOnly: true,
                            fieldLabel: '项目阶段'
                        }, {
                            xtype: 'textfield',
                            name: 'Project_Subject',
                            width: 230,
                            readOnly: true,
                            fieldLabel: '项目专业'
                        }, {
                            xtype: 'combo',
                            fieldLabel: '审核人',
                            name: 'Audit_User_ID',
                            emptyText: '请选择...',
                            hiddenName: 'Audit_User_ID',
                            valueField: 'Login_ID',
                            displayField: 'Username',
                            allowBlank: false,
                            triggerAction: 'all',
                            mode: 'local',
                            typeAhead: true,
                            allowBlank: false,
                            forceSelection: true,
                            editable: false,
                            width: 230,
                            store: new Ext.data.JsonStore({
                                url: WebRoot + '/Search/FindAuditors',
                                idProperty: 'Login_ID',
                                fields: ['Login_ID', 'Username'],
                                autoLoad: true
                            })
                        }, {
                            xtype: 'textfield',
                            name: 'Diagram_Name',
                            width: 230,
                            readOnly: true,
                            fieldLabel: '图纸名称'
                        }, {
                            xtype: 'textfield',
                            name: 'File_Name',
                            width: 230,
                            readOnly: true,
                            fieldLabel: '文件名'
                        }, {
                            xtype: 'textarea',
                            fieldLabel: '借阅理由',
                            name: 'Reason',
                            allowBlank: false,
                            width: 240
                        }],
                        buttons: [{
                            text: '确定',
                            handler: function () {
                                var basicForm = Ext.getCmp('applyForm').getForm();
                                if (basicForm.isValid()) {
                                    basicForm.submit({
                                        success: function (form, action) {
                                            Ext.Msg.alert('操作提示', action.result.message);
                                            applyFrame.close();
                                        },
                                        failure: function (form, action) {
                                            Ext.Msg.alert('借阅失败', action.result.message);
                                        }
                                    });
                                }
                            }
                        }, {
                            text: '取消',
                            handler: function () {
                                applyFrame.close();
                            }
                        }]
                    }]
                });
                return applyFrame;
            }

            var GetProjectFrame = function () {
                var frameWindow = new Ext.Window({
                    title: '项目新增',
                    width: 360,
                    modal: true,
                    closable: false,
                    resizable: false,
                    items: [{
                        id: 'projectForm',
                        xtype: 'form',
                        frame: true,
                        url: WebRoot + '/Project/Create',
                        method: 'POST',
                        labelSeparator: '',
                        labelWidth: 80,
                        items: [{
                            xtype: 'textfield',
                            name: 'Project_Code',
                            fieldLabel: '项目编码',
                            allowBlank: false,
                            minLength: 4,
                            minLengthText: '项目编码最少4位.',
                            width: 230
                        }, {
                            xtype: 'textfield',
                            name: 'Project_Name',
                            fieldLabel: '项目名称',
                            allowBlank: false,
                            width: 230
                        }, {
                            xtype: 'textfield',
                            name: 'Business_Owner',
                            fieldLabel: '项目所有人',
                            width: 230
                        }],
                        buttons: [{
                            text: '确定',
                            handler: function () {
                                var basicForm = Ext.getCmp('projectForm').getForm();
                                if (basicForm.isValid()) {
                                    basicForm.submit({
                                        success: function (form, action) {
                                            Ext.Msg.alert("操作提示", "项目新增操作成功。");
                                            frameWindow.close();

                                            var treePanel = Ext.getCmp('treePanel');
                                            var root = treePanel.getRootNode();
                                            var loader = treePanel.getLoader();
                                            loader.load(root);
                                        },
                                        failure: function (form, action) {
                                            Ext.Msg.alert('创建失败', action.result.message);
                                        }
                                    });
                                }
                            }
                        }, {
                            text: '取消',
                            handler: function () {
                                frameWindow.close();
                            }
                        }]
                    }]
                });
                return frameWindow;
            }

            var treeMenu = new Ext.menu.Menu({
                items: [{
                    text: '申请借阅',
                    handler: function () {
                        GetApplyFrame().show();
                        var basicForm = Ext.getCmp('applyForm').getForm();
                        basicForm.setValues(contextTreeNode.attributes);
                    }
                }, {
                    text: '新增档案',
                    handler: function () {
                        GetProfileFrame().show();
                        var basicForm = Ext.getCmp('profileForm').getForm();
                        basicForm.setValues(contextTreeNode.attributes);
                    }
                }, {
                    text: '新增项目',
                    handler: function () {
                        GetProjectFrame().show();
                    }
                }]
            });

            var gridMenu = new Ext.menu.Menu({
                items: [
                {
                    text: '申请借阅',
                    handler: function () {
                        GetApplyFrame().show();
                        var basicForm = Ext.getCmp('applyForm').getForm();
                        basicForm.setValues(contextGridRow.data);
                    }
                }, {
                    text: '预览',
                    handler: function () {
                        if (contextGridRow == null || contextGridRow == undefined) {
                            Ext.Msg.alert('操作提示', '右键点击行记录。');
                        } else {
                            window.open(WebRoot + '/Search/Preview?barcode=' + contextGridRow.get('Barcode'));
                        }
                    }
                }]
            });

            var contextTreeNode;
            var contextGridRow;

            var sm = new Ext.grid.CheckboxSelectionModel({ singleSelect: false, dataIndex: 'Barcode' });

            new Ext.Viewport({
                layout: 'border',
                items: [
                {
                    region: 'west',
                    title: 'Navigation',
                    xtype: 'tabpanel',
                    width: 200,
                    activeTab: 0,
                    animScroll: true,
                    enableTabScroll: true,
                    collapsible: true,
                    frame: true,
                    split: true,
                    items: [{
                        id: 'treePanel',
                        title: '普通查询',
                        xtype: 'treepanel',
                        collapsible: true,
                        autoScroll: true,
                        disableSelection: true,
                        loader: new Ext.tree.TreeLoader({
                            dataUrl: WebRoot + '/Search/Childrens',
                            listeners: {
                                beforeload: function (treeLoader, node) {
                                    this.baseParams = node.attributes;
                                }
                            }
                        }),
                        root: new Ext.tree.AsyncTreeNode({ id: 'root', text: '档案', leaf: false }),
                        listeners: {
                            click: function (n) {
                                if (n.attributes.id == 'root' || n.attributes.Order < 2) return;
                                store.baseParams = {
                                    subject: n.attributes.Project_Subject,
                                    stage: n.attributes.Project_Stage,
                                    code: n.attributes.Project_Code,
                                    at: 'G'
                                };
                                store.reload({
                                    params: {
                                        start: 0,
                                        limit: pageSize
                                    }
                                });
                            },
                            contextmenu: function (node, e) {
                                if (!node.isRoot && node.attributes.Order > 1) {
                                    contextTreeNode = node;
                                    e.preventDefault();
                                    treeMenu.showAt(e.getXY());
                                }
                            }
                        }
                    }, {
                        title: '其他查询',
                        width: 200,
                        items: {
                            xtype: 'form',
                            bodyStyle: 'padding: 10px',
                            items: [{
                                xtype: 'displayfield',
                                hideLabel: true,
                                value: '项目编码'
                            }, {
                                xtype: 'textfield',
                                id: 'qProjectCode',
                                hideLabel: true
                            }, {
                                xtype: 'displayfield',
                                hideLabel: true,
                                value: '项目名称'
                            }, {
                                xtype: 'textfield',
                                id: 'qProjectName',
                                hideLabel: true
                            }],
                            buttons: [{
                                text: '查询',
                                handler: function () {
                                    var qProjectCode = Ext.getCmp('qProjectCode').getValue();
                                    var qProjectName = Ext.getCmp('qProjectName').getValue();
                                    store.baseParams = {
                                        name: qProjectName,
                                        code: qProjectCode,
                                        at: 'F'
                                    };
                                    store.reload({
                                        params: {
                                            start: 0,
                                            limit: pageSize
                                        }
                                    });
                                }
                            }]
                        }
                    }]
                }, {
                    region: 'center',
                    xtype: 'panel',
                    items: [{
                        title: '档案列表',
                        xtype: 'grid',
                        store: store,
                        stripeRows: true,
                        disableSelection: true,
                        enableHdMenu: false,
                        columnLines: true,
                        loadMask: true,
                        autoHeight: true,
                        autoScroll: true,
                        enableColumnMove: false,
                        sm: sm,
                        viewConfig: {
                            forceFit: true,
                            enableRowBody: true,
                            showPreview: true
                        },
                        frame: true,
                        columns: [
                            sm,
                            { header: "条形码", sortable: false, dataIndex: 'Barcode', minWidth: 100 },
                            { header: "项目编码", sortable: false, dataIndex: 'Project_Code' },
                            { header: "项目名称", sortable: false, dataIndex: 'Project_Name' },
                            { header: "项目阶段", sortable: false, dataIndex: 'Project_Stage' },
                            { header: "项目专业", sortable: false, dataIndex: 'Project_Subject' },
                            { header: "文件名称", sortable: false, dataIndex: 'File_Name' },
                            { header: "图纸名称", sortable: false, dataIndex: 'Diagram_Name' },
                            { header: "图纸编码", sortable: false, dataIndex: 'Diagram_Code' },
                            { header: "图纸版本", sortable: false, dataIndex: 'Diagram_Version' },
                            { header: "图纸比例", sortable: false, dataIndex: 'Diagram_Scale' },
                            { header: "归档日期", sortable: false, dataIndex: 'Created_Date' },
                            { header: "项目经理", sortable: false, dataIndex: 'Plotter' }
                        ],
                        iconCls: 'icon-grid',
                        listeners: {
                            rowcontextmenu: function (grid, rowIndex, e) {
                                contextGridRow = grid.getStore().getAt(rowIndex);
                                e.preventDefault();
                                gridMenu.showAt(e.getXY());
                            }
                        },
                        buttons: [{
                            text: '申请借阅',
                            handler: function () {
                                if (sm.hasSelection()) {
                                    var frameWindow = new Ext.Window({
                                        title: '批量借阅',
                                        width: 380,
                                        modal: true,
                                        closable: false,
                                        resizable: false,
                                        items:
                                        [{
                                            xtype: 'form',
                                            id: 'applyForBatchForm',
                                            bodyStyle: 'padding:10px',

                                            items:
                                            [{
                                                xtype: 'combo',
                                                fieldLabel: '审核人',
                                                name: 'Audit_User_ID',
                                                emptyText: '请选择...',
                                                hiddenName: 'Audit_User_ID',
                                                valueField: 'Login_ID',
                                                displayField: 'Username',
                                                allowBlank: false,
                                                triggerAction: 'all',
                                                mode: 'local',
                                                typeAhead: true,
                                                allowBlank: false,
                                                forceSelection: true,
                                                editable: false,
                                                width: 230,
                                                store: new Ext.data.JsonStore({
                                                    url: WebRoot + '/Search/FindAuditors',
                                                    idProperty: 'Login_ID',
                                                    fields: ['Login_ID', 'Username'],
                                                    autoLoad: true
                                                })
                                            }, {
                                                xtype: 'textarea',
                                                id: 'reason',
                                                fieldLabel: '借阅理由',
                                                width: 230,
                                                allowBlank: false
                                            }],
                                            buttons:
                                            [{
                                                text: '确定',
                                                handler: function () {
                                                    var items = '';
                                                    var selections = sm.getSelections();
                                                    for (var i = 0; i < selections.length; i++) {
                                                        items += selections[i].get('Barcode') + ';';
                                                    }

                                                    var basicForm = Ext.getCmp('applyForBatchForm').getForm();
                                                    basicForm.submit({
                                                        url: WebRoot + '/Search/ApplyForBatch',
                                                        method: 'POST',
                                                        params: { items: items },
                                                        success: function (form, action) {
                                                            Ext.Msg.alert('操作提示', action.result.message);
                                                            frameWindow.close();
                                                        },
                                                        failure: function (form, action) {
                                                            Ext.Msg.alert('失败提示', action.result.message);
                                                        }
                                                    });
                                                }
                                            }, {
                                                text: '关闭',
                                                handler: function () {
                                                    frameWindow.close();
                                                }
                                            }]
                                        }]
                                    });
                                    frameWindow.show();
                                } else {
                                    Ext.Msg.alert("操作提示", "请选择操作记录！");
                                }
                            }
                        }],
                        bbar: new Ext.PagingToolbar({
                            pageSize: pageSize,
                            store: store,
                            displayInfo: true,
                            displayMsg: '显示 {0} 至 {1} , 共 {2} 条',
                            emptyMsg: '没有记录'
                        })
                    }]
                }]
            });
        });
    </script>
</body>
</html>
