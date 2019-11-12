using Microsoft.AspNetCore.SignalR;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.Sample.NetCore3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Senparc.Weixin.Sample.NetCore3.Service
{
    public class SingleHub : Hub
    {
        /*        private SingleHub()
                {
                    Console.WriteLine("SingleHub初始化");
                }

                public static SingleHub instance = null;
                //10.定义一个字段，用来进行测试
                public string name;
                //3.定义返回值类为单例类型的静态方法
                public static SingleHub getInstance(string n)
                {
                    //4.判断静态变量instance是否为空
                    if (instance == null)
                    {
                        //5.如果为空，就创建实例
                        instance = new SingleHub();
                        //6.给单例类中的字段进行赋值
                        instance.name = n;
                    }
                    //7.返回此变量
                    return instance;
                }*/



        public static readonly string appId = Config.SenparcWeixinSetting.WeixinAppId;
        public static readonly string appSecret = Config.SenparcWeixinSetting.WeixinAppSecret;
        public static readonly string OpenId = "oZ23J1XK3gFFHApw9aNxcf33oeJ0";

        /*
         此方法只负责客服发信息到微信
             */
        public void Send(MessageBody body)
        {
            //await _hubContext.Clients.All.SendAsync("Notify", $"Home page loaded at: {DateTime.Now}");
            //放在上面有没有可能发送消息的时候假设时间间隔短没有存到数据库？
            Clients.All.SendAsync("Recv", body);

            //获取当前的客服、顾客、公众号、消息、时间

            //1、body存到数据库
            //2、body发到微信公众号,post请求

            //var accessToken = AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            string text = body.Content.ToString();
            var sasas = CommonApi.GetToken("wxc3a7e87e3d3cb81a", "7f331ad801e1cf978024c31c5f0a5b9f");
            CustomApi.SendText("wxc3a7e87e3d3cb81a", OpenId, text);


            //放在下面发送消息会延迟
            //Clients.All.SendAsync("Recv", body);
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

        //8.如果这个类中还有其他的字段，那么直接在下面写出来即可
        //如下:
        //public string Name;
        //public int Age;



    }
}
