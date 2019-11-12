"use strict";
/*创建了一个 SignalR 的 connection 对象*/
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/SingleHub")
    .build();

/*connection 绑定了一个事件，该事件的名称和服务器 Send 方法中第一个参数的值相呼应*/
/*通过这种绑定，客户端就可以接收到服务器推送过来的消息，即客户发送消息给客服，从这里定义显示到前端*/
connection.on("Recv", function (body) {
    var li = document.createElement("li");
    li = $(li).text("发给公众号：" + body.toUserName + "\n"
        + "消息来自：" + body.fromUserName + "\n"
        + "发给客服：" + body.toKefuName + "\n"
        + "创建时间：" + body.createTime + "\n"
        + "消息内容：" + body.content)
    $("#msgList").append(li);
});

connection.start()
    .then(function () {
        console.log("SignalR 已连接");
    }).catch(function (err) {
        console.log(err);
    });

/*反之，通过 connection.invoke("send",xxx)，也可以将消息发送到服务器端的 Send 方法中，即客服发送消息给客户*/
$(document).ready(function () {
    $("#btnSend").on("click", function () {
        var textName = $("#textName").val();
        var content = $("#content").val();
        //var customMessageFromWeixin = $("#customMessageFromWeixin").val();
        console.log(textName + ":" + content);
        connection.invoke("send", { "Type": 0, "TextName": textName, "Content": content});
        console.log("已调用connection.invoke send ");
    });
});