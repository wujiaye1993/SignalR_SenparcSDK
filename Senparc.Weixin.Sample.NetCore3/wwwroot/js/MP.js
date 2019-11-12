"use strict";
/*创建了一个 SignalR 的 connection 对象*/
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/MpCommonHub")
    .build();

/*connection 绑定了一个事件，该事件的名称和服务器 Send 方法中第一个参数的值相呼应*/
/*通过这种绑定，客户端就可以接收到服务器推送过来的消息*/
connection.on("RecvMP", function (data) {
    var li = document.createElement("li");
    li = $(li).text(data.wxMpId + "：" + data.wxAppSecret)
    $("#msgList").append(li);
});

connection.start()
    .then(function () {
        console.log("SignalR 已连接");
    }).catch(function (err) {
        console.log(err);
    });

/*反之，通过 connection.invoke("send",xxx)，也可以将消息发送到服务器端的 Send 方法中*/
$(document).ready(function () {
    $("#btnSend").on("click", function () {
        var wxMpId = $("#wxMpId").val();
        var wxAppSecret = $("#wxAppSecret").val();
        //var customMessageFromWeixin = $("#customMessageFromWeixin").val();
        console.log(wxMpId + ":" + wxAppSecret);
        connection.invoke("send", { "Type": 0, "WxMpId": wxMpId, "WxAppSecret": wxAppSecret });
        console.log("已调用connection.invoke send ");
    });
});