<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%
    ViewData["WebRoot"] = (Request.ApplicationPath == "/") ? "" : Request.ApplicationPath;
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>档案管理系统-我的申请</title>
    <link href="../../Scripts/ExtJS/resources/css/ext-all.css" rel="stylesheet" type="text/css" />
 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/adapter/ext/ext-base.js"></script>
 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/ext-all.js"></script>
 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/Security.js"></script>
</head>
<body>
    <script type="text/javascript">
        Ext.onReady(function () {
            var WebRoot = '<%=ViewData["WebRoot"] %>';
            var pageSize = 15;

            var store = new Ext.data.JsonStore({
                url: WebRoot + '/MyApply/List',
                totalProperty: 'count',
                idProperty: 'ID',
                root: 'items',
                fields: [
                    'ID',
                    'Project_Code',
                    'Project_Name',
                    'Project_Stage',
                    'Project_Subject',
                    'Barcode',
                    'Diagram_Name',
                    'File_Name',
                    'Login_ID',
                    'Audit_Date',
                    'Apply_Date',
                    'Reason',
                    'Status',
                    'StatusDescription'
                ]
            });

            var sm = new Ext.grid.CheckboxSelectionModel({ singleSelect: true, dataIndex: 'ID' });

            var refresh = function () {
                var status = Ext.getCmp('statusCombo').getValue();
                store.baseParams.status = status;

                var disabled = (status != '1');
                Ext.getCmp('download').setDisabled(disabled);

                store.load({
                    params: {
                        start: 0,
                        limit: pageSize
                    }
                });
            }

            new Ext.Viewport({
                layout: 'border',
                items: [
                    {
                        region: 'center',
                        xtype: 'grid',
                        title: '我的申请列表',
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
                                    { header: "项目编码", id: 'Project_Code', dataIndex: 'Project_Code' },
                                    { header: "项目名称", id: 'Project_Name', dataIndex: 'Project_Name' },
                                    { header: "项目阶段", id: 'Project_Stage', dataIndex: 'Project_Stage' },
                                    { header: "项目专业", id: 'Project_Subject', dataIndex: 'Project_Subject' },
                                    { header: "条形码", id: 'Barcode', dataIndex: 'Barcode' },
                                    { header: "图纸名称", id: 'Diagram_Name', dataIndex: 'Diagram_Name' },
                                    { header: "文件名", id: 'File_Name', dataIndex: 'File_Name' },
                                    { header: "状态", id: 'StatusDescription', dataIndex: 'StatusDescription' },
                                    { header: "申请时间", id: 'Apply_Date', dataIndex: 'Apply_Date' },
                                    { header: "审核时间", id: 'Audit_Date', dataIndex: 'Audit_Date' }
                             ],
                        listeners: {
                            render: function (grid) {
                                var store = grid.getStore(); // Capture the Store. 
                                var view = grid.getView(); // Capture the GridView. 
                                grid.tip = new Ext.ToolTip({
                                    target: view.mainBody, // The overall target element. 
                                    delegate: '.x-grid3-row', // Each grid row causes its own seperate show and hide. 
                                    trackMouse: true, // Moving within the row should not hide the tip. 
                                    renderTo: document.body, // Render immediately so that tip.body can be referenced prior to the first show. 
                                    width: 200,
                                    listeners: { // Change content dynamically depending on which element triggered the show. 
                                        beforeshow: function updateTipBody(tip) {
                                            var rowIndex = view.findRowIndex(tip.triggerElement);
                                            tip.body.dom.innerHTML = '申请原因: ' + store.getAt(rowIndex).get('Reason');
                                        }
                                    }
                                });
                            }
                        },
                        minColumnWidth: 85,
                        autoHeight: true,
                        autoscroll: true,
                        enableColumnMove: false,
                        tbar: [{
                            xtype: 'combo',
                            id: 'statusCombo',
                            typeAhead: true,
                            forceSelection: true,
                            triggerAction: 'all',
                            emptyText: '选择审核状态',
                            selectOnFocus: true,
                            editable: false,
                            store: [
                                ['0', '等待审核'],
                                ['1', '审核通过'],
                                ['2', '审核拒绝']
                            ],
                            listeners: {
                                select: refresh,
                                afterRender: function (combo) {
                                    combo.setValue('0');
                                    refresh();
                                }
                            }
                        }],
                        buttons: [
                        {
                            id: 'download',
                            text: '下载',
                            handler:  function () {
                                if (sm.hasSelection()) {
                                    var selected = sm.getSelected();
                                    var status = selected.get('Status');
                                    if (status == 0) {
                                        Ext.Msg.alert('操作提示', '正在审核');
                                    } else if (status == 1) {
                                        window.open(WebRoot + '/MyApply/Download?id=' + selected.get('ID'));
                                    }
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
                    }
                ]
            });

            refresh();
        });
    </script>
</body>
</html>
