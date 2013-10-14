<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%
    ViewData["WebRoot"] = (Request.ApplicationPath == "/") ? "" : Request.ApplicationPath;
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
 <meta http-equiv="pragma" content="no-cache"/>   
 <meta http-equiv="cache-control" content="no-cache, must-revalidate"/>   
 <meta http-equiv="expires" content="0"/> 
 <title>CAD 自动归档系统</title>
 <link href="../../Scripts/ExtJS/resources/css/ext-all.css" rel="stylesheet" type="text/css" />

 <link href="../../Content/Site.css" rel="stylesheet" type="text/css" />

 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/adapter/ext/ext-base.js"></script>
 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/ExtJS/ext-all.js"></script>
 <script language="javascript" type="text/javascript" src="<%=ViewData["WebRoot"] %>/Scripts/Security.js"></script>
 <script language="javascript" type="text/javascript">
     var WebRoot = '<%=ViewData["WebRoot"] %>';

     var changePasswordDialog;

     Ext.onReady(function () {
         Ext.QuickTips.init();

         changePasswordDialog = new Ext.Window({
             title: "密码修改",
             resizable: false,
             frame: true,
             closeAction: "hide",
             maximizable: false,
             closable: true,
             modal: false,
             bodyStyle: "padding:4px",
             width: 310,
             items: [
             {
                 id: 'mainform',
                 xtype: 'form',
                 layout: 'form',
                 bodyStyle: 'padding: 10px;',
                 lableWidth: 39,
                 defaults: { xtype: "textfield", width: 120, inputType: 'password' },
                 items: [
                 { fieldLabel: "原密码", id: 'oldPassword', allowBlank: false, blankText: '不允许为空' },
                 { fieldLabel: "新密码", id: 'newPassword', allowBlank: false, blankText: '不允许为空' },
                 {
                     fieldLabel: '确认密码',
                     id: 'confirmPassword',
                     allowBlank: false,
                     blankText: '不允许为空',
                     invalidText: '密码不一致',
                     validator: function (value) {
                         var newPassword = Ext.get('newPassword').getValue();
                         return newPassword == value;
                     }
                 }]
             }],
             buttons:
             [
                {
                    text: "确定",
                    handler: function () {
                        // 检查新密码和确认密码是否一致
                        var basicForm = Ext.getCmp('mainform').getForm();
                        if (basicForm.isValid()) {
                            basicForm.submit({
                                url: WebRoot + '/Account/ChangePassword',
                                success: function (form, action) {
                                    Ext.Msg.alert('操作提示', '修改已经成功.');
                                    changePasswordDialog.hide();
                                    Ext.getCmp('mainform').getForm().reset();
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert('错误提示', action.result.message);
                                }
                            });
                        }
                    }
                }, {
                    text: "取消",
                    handler: function () {
                        changePasswordDialog.hide();
                        Ext.getCmp('mainform').getForm().reset();
                    }
                }
             ]
         });

     });
 </script>
</head>

<body>
    <div class="page">
        <div id="header">
            <div id="title">
                <h1>CAD 自动归档系统</h1>
            </div>
              
            <div id="logindisplay">
                <% Html.RenderPartial("LogOnUserControl"); %> [ <a href="javascript:void(0);" onclick="changePasswordDialog.show()">修改密码</a> ]
            </div> 
            
            <div id="menucontainer">
            
                <ul id="menu">
                <% 
                    foreach (var entity in ViewData["authorities"] as List<LStu.Models.ResourceDTO>)
                    {
                        %>
                        <li><a href='<%=ViewData["WebRoot"] + "/" + entity.Url %>' target="mainFrame"><%=entity.Name%></a></li>
                        <%
                    }
                 %>
                </ul>
            </div>
        </div>

        <div id="main">
           <iframe name="mainFrame" frameborder="0" height="100%" width="100%" src='<%=ViewData["WebRoot"] + "/Search" %>' marginheight="1" marginwidth="1" id="mainFrame" scrolling="auto" style="height:520px;"></iframe>
           <%--<script type="text/javascript" language="javascript">
               window.moveTo(0, 0);
               window.resizeTo(screen.availWidth, screen.availHeight);

               var mainFrame = document.getElementById('mainFrame');
               mainFrame.width = screen.availWidth - 6;
           </script>--%>
        </div>
    </div>
</body>
</html>
