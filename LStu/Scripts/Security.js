Ext.onReady(function () {

    new Ext.KeyMap(document, [
    {
        key: Ext.EventObject.BACKSPACE,
        fn: function (key, e) {
            var t = e.target.tagName;
            if (t !== "INPUT" && t !== "TEXTAREA") {
                e.stopEvent();
            }
        }
    }]);

    Ext.Ajax.defaultHeaders = {
        'Request-By': 'Ext' //标识ajax请求   
    };

    Ext.Ajax.on('requestcomplete', function (conn, response, options) {
        var data = Ext.util.JSON.decode(response.responseText);
        if (typeof data == 'object' && data.signOut == true) {
            alert("登入超时,系统将自动跳转到登陆页面,请重新登入!");
            top.window.location.href = data.redirect;
        }
    });
});