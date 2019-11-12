using Microsoft.AspNetCore.SignalR;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.Sample.NetCore3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Senparc.Weixin.Sample.NetCore3.Service
{
    
    public class MpCommonHub : Hub
    {
        //public static readonly string appId = Config.SenparcWeixinSetting.WeixinAppId;
        //public static readonly string appSecret = Config.SenparcWeixinSetting.WeixinAppSecret;
        //public static readonly string OpenId = "oZ23J1XK3gFFHApw9aNxcf33oeJ0";

        //获取Access_token
        //管理Access_token，当请求再来的时候检查Access_token是否已经拥有，过期时间是否已经到了
        //根据appid，appsecret发送请求获取result，result里有access_token，过期时间
        //
         
        public void Send(MpBody body)
        {
            //await _hubContext.Clients.All.SendAsync("Notify", $"Home page loaded at: {DateTime.Now}");
            Clients.All.SendAsync("RecvMP", body);
            //1、body存到数据库
            //2、body发到微信公众号,post请求
            //var accessToken = AccessTokenContainer.TryGetAccessToken(appId, appSecret);


        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("真的吗，有人进来了：{0}", this.Context.ConnectionId);
            Classhub.asd = this.Clients;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("真的耶，有人跑路了：{0}", this.Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
