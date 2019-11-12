using Microsoft.AspNetCore.SignalR;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.Sample.NetCore3.Controllers;
using Senparc.Weixin.Sample.NetCore3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Senparc.Weixin.Sample.NetCore3.Service
{
    public class WeChatHub : Hub
    {
        public static readonly string appId = Config.SenparcWeixinSetting.WeixinAppId;
        public static readonly string OpenId = "oZ23J1XK3gFFHApw9aNxcf33oeJ0"; //伍家烨
        //public MessageBody messageBody;
        //public PostModel postModel;

  

        public void Send(MessageBody body)
        {  
            Clients.All.SendAsync("Recv", body);
            //1、body存到数据库

            //2、body发到微信公众号,post请求
            string text = body.Content.ToString();
            var sasas = CustomApi.SendText(appId, OpenId, text);
            var sss = sasas;
        }

        public void SendtoWeChat(MessageWeChat message )
        {
            
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("哇，有人进来了：{0}", this.Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("靠，有人跑路了：{0}", this.Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }


    }



    public class MessageWeChat
    {
        public int Type { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }


        //public PostModel postModel;
    }
}
