<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%
    ViewData["WebRoot"] = (Request.ApplicationPath == "/") ? "" : Request.ApplicationPath;
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>档案管理系统</title>
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
                url: WebRoot + '/Archive/List',
                fields: [
                'Barcode',
                'Project_Code',
                'Project_Name',
                'Project_Stage',
                'Project_Subject',
                'File_Name',
                'Business_Owner',
                'Diagram_Name',
                'Diagram_Code',
                'Diagram_Version',
                'Diagram_Scale',
                'Plotter'
                ]
            });

            var VIEW_DATA = [];

            var itemIndexOf = function (barcode) {
                var index = -1;
                for (var i = 0; i < VIEW_DATA.length; i++) {
                    if (VIEW_DATA[i] == barcode) {
                        index = i;
                        break;
                    }
                }
                return index;
            };

            var applyItem = function (barcode) {
                if (itemIndexOf(barcode) > -1) return;
                VIEW_DATA.push(barcode);
            };

            var removeItem = function (barcode) {
                var items = [];
                for (var i = 0; i < VIEW_DATA.length; i++) {
                    var c = VIEW_DATA.pop();
                    if (c != barcode) items.push(c);
                }
                for (var i = 0; i < items.length; i++) {
                    VIEW_DATA.push(items[i]);
                }
            };

            var refresh = function () {
                var baseParams = { items: '' };
                for (var i = 0; i < VIEW_DATA.length; i++) {
                    baseParams.items = baseParams.items + ';' + VIEW_DATA[i];
                }

                store.baseParams = baseParams;
                store.reload();
            };

            var GetTemporaryUpdate = function () {
                var temporaryUpdate = new Ext.Window({
                    title: '档案修改',
                    width: 500,
                    autoHeight: true,
                    modal: true,
                    resizable: false,
                    closable: false,
                    items: [{
                        id: 'temporaryForm',
                        xtype: 'form',
                        frame: true,
                        url: WebRoot + '/Archive/Update',
                        method: 'post',
                        labelSeparator: '',
                        labelWidth: 100,
                        items: [{
                            xtype: 'hidden',
                            name: 'Barcode'
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '项目编码',
                            name: 'Project_Code',
                            minLength: 4,
                            minLengthText: '项目编码最少4位.',
                            allowBlank: false,
                            width: 300
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '项目名称',
                            name: 'Project_Name',
                            width: 300
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
                            width: 300,
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
                            width: 300,
                            store: new Ext.data.JsonStore({
                                url: WebRoot + '/Search/SubjectRecords',
                                idProperty: 'Name',
                                fields: [{ name: 'Project_Subject', mapping: 'Name'}],
                                autoLoad: true
                            })
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '项目所有人',
                            name: 'Business_Owner',
                            width: 300
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '图纸名称',
                            name: 'Diagram_Name',
                            allowBlank: false,
                            width: 300
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '图纸编码',
                            name: 'Diagram_Code',
                            allowBlank: false,
                            width: 300
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '图纸版本',
                            name: 'Diagram_Version',
                            width: 300
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '图纸比例',
                            name: 'Diagram_Scale',
                            width: 300
                        }, {
                            xtype: 'textfield',
                            fieldLabel: '文件名',
                            name: 'File_Name',
                            width: 300
                        }],
                        buttons: [{
                            text: '保存',
                            handler: function () {
                                var basicForm = Ext.getCmp('temporaryForm').getForm();
                                if (basicForm.isValid()) {
                                    basicForm.submit({
                                        success: function (form, action) {
                                            Ext.Msg.alert('操作提示', '记录修改成功.');
                                            refresh();
                                            temporaryUpdate.close();
                                        },
                                        failure: function (form, action) {
                                            Ext.Msg.alert('错误提示', action.result.message);
                                        }
                                    });
                                }
                            }
                        }, {
                            text: '关闭',
                            handler: function () {
                                temporaryUpdate.close();
                            }
                        }]
                    }]
                });
                return temporaryUpdate;
            }

            var sm = new Ext.grid.CheckboxSelectionModel({ singleSelect: true, dataIndex: 'Barcode' });

            var showTemporary = function () {
                if (sm.hasSelection()) {
                    var temporaryUpdate = GetTemporaryUpdate();
                    var selected = sm.getSelected();
                    Ext.getCmp('temporaryForm').getForm().setValues(selected.data);
                    temporaryUpdate.show();
                } else {
                    Ext.Msg.alert("操作提示", "请选择操作记录！");
                }
            }

            var execute = function (barcode, c) {
                if (barcode == null || barcode.length == 0) return;
                Ext.Ajax.request({
                    url: WebRoot + '/Archive/Execute',
                    method: 'POST',
                    params: { barcode: barcode, handler: c },
                    success: function (r, options) {
                        var responseContext = Ext.util.JSON.decode(r.responseText);
                        if (responseContext.errors == 'B000') {
                            println('成功 - [条形码: ' + barcode + ']');
                            removeItem(barcode);
                            refresh();
                        }
                        else if (responseContext.errors == 'B008') { // 项目编码不存在。
                            Ext.Msg.confirm('失败提示', '项目编码不存在，是否新增项目编码?', function (ok) {
                                if (ok == 'yes') {
                                    execute(barcode, 'B008');
                                }
                            });
                        }
                        else {
                            var errors = responseContext.errors;
                            println('失败 - [条形码: ' + barcode + '], Error Code: ' + errors + ', 原因: ' + responseContext.message);
                            if (errors != 'B001' && errors != 'B002') {
                                applyItem(barcode);
                                refresh();
                            }
                        }
                    }
                });
            }

            var println = function (append) {
                var console = Ext.getCmp('console');
                var text = console.getValue();
                console.setValue(new Date().format('Y-m-d H:i:s') + ' - ' + append + '\r\n' + text);
            }

            var formSubmit = function () {
                var barcode = Ext.getCmp('barcode').getValue();
                if (barcode == null || barcode == '') {
                    Ext.Msg.alert('操作提示', '请输入档案条码.');
                    return;
                }

                Ext.getCmp('barcode').setValue('');
                var autoOption = Ext.getCmp('autoOption').getValue();

                if (autoOption == true) {
                    if (barcode != null && barcode.length > 0) {
                        execute(barcode);
                    } else {
                        Ext.Msg.alert('操作提示', '请输入档案条码.');
                    }
                } else {
					if (barcode != null && barcode.length > 0) {
                        Ext.Ajax.request({
							url: WebRoot + '/Archive/IsExisted',
							method: 'POST',
							params: { barcode: barcode },
							success: function (r, options) {
								var responseContext = Ext.util.JSON.decode(r.responseText);

								if (responseContext.existed) {
									applyItem(barcode);
									refresh();
								} else {
									Ext.Msg.alert('操作提示', '没有找到档案条码的档案信息.');
								}
							}
						});
                    } else {
                        Ext.Msg.alert('操作提示', '请输入档案条码.');
                    }
                }
            }

            new Ext.Viewport({
                layout: 'border',
                items: [
                {
                    region: 'north',
                    title: '待归档文档档记录查询',
                    xtype: 'form',
                    height: 80,
                    viewConfig: {
                        forceFit: true
                    },
                    frame: true,
                    bodyStyle: 'padding: 10px;',
                    iconCls: 'icon-form',
                    layout: 'column',
                    items: [
                    {
                        width: 320,
                        layout: 'form',
                        labelWidth: 75,
                        items: {
                            id: 'barcode',
                            xtype: 'textfield',
                            fieldLabel: '条形码',
                            width: 220,
                            emptyText: '请输入...'
                        }
                    }, {
                        xtype: 'button',
                        text: '归档',
                        width: 80,
                        handler: formSubmit
                    }, {
                        width: 320,
                        layout: 'form',
                        labelWidth: 75,
                        items: {
                            id: 'autoOption',
                            xtype: 'checkbox',
                            boxLabel: '自动归档',
                            width: 220
                        }
                    }]
                }, {
                    region: 'center',
                    title: '待归档档案记录列表',
                    xtype: 'grid',
                    height: 230,
                    store: store,
                    sm: sm,
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
                    frame: true,
                    columns: [
                                    sm,
                                    { header: "条形码", sortable: false, dataIndex: 'Barcode' },
                                    { header: "项目编码", sortable: false, dataIndex: 'Project_Code' },
                                    { header: "项目名称", sortable: false, dataIndex: 'Project_Name' },
                                    { header: "项目阶段", sortable: false, dataIndex: 'Project_Stage' },
                                    { header: "项目专业", sortable: false, dataIndex: 'Project_Subject' },
                                    { header: "图纸名称", sortable: false, dataIndex: 'Diagram_Name' },
                                    { header: "图纸编码", sortable: false, dataIndex: 'Diagram_Code' },
                                    { header: "图纸版本", sortable: false, dataIndex: 'Diagram_Version' },
                                    { header: "图纸比例", sortable: false, dataIndex: 'Diagram_Scale' },
                                    { header: "文件名", sortable: false, dataIndex: 'File_Name' }
                             ],
                    minColumnWidth: 60,
                    autoHeight: true,
                    autoscroll: true,
                    enableColumnMove: false,
                    iconCls: 'icon-grid',
                    buttons:
                    [{
                        xtype: 'button',
                        text: '归档',
                        listeners: {
                            click: function () {
                                if (sm.hasSelection()) {
                                    var selected = sm.getSelected();
                                    var barcode = selected.get('Barcode');
                                    execute(barcode);
                                } else {
                                    Ext.Msg.alert("操作提示", "请选择操作记录！");
                                }
                            }
                        }
                    }, {
                        xtype: 'button',
                        text: '修改',
                        listeners: { click: showTemporary }
                    }]
                }, {
                    region: 'south',
                    id: 'console',
                    xtype: 'textarea',
                    height: 120,
                    editable: false,
                    allowBlank: true,
                    readOnly: true
                }]
            });

            Ext.EventManager.addListener('barcode', 'keyup', function (e) {
                if (e.getKey() == Ext.EventObject.ENTER) {
                    formSubmit();
                }
            });

            Ext.getCmp("barcode").focus(true, 100);
        });
    </script>
</body>
</html>
