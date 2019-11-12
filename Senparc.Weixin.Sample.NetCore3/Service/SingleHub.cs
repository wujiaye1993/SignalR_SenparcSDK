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
        public async Task SendAsync(MessageBody body)
        {
            if (body.Content==""|body.FromUserName==""|body.ToUserName=="")
            {
                return;
            }

            #region 获取当前的客服、顾客、公众号、消息、时间
            //获取当前的客服、顾客、公众号、消息、时间
            //1、获取当前客服，即登录的OrchardCore.User 对象，这个只在记录的时候有用，记录一条消息记录中的客服信息
            //2、回复的客户，从前端获取客户OpenId或Id，获取Id或者OpenId要查表得到Name，看看能不能从直接从前端获得Name，记录一条消息记录中的客户信息
            //3、所在公众号，从前端获得公众号的AppId
            //4、消息，从前端获得
            //5、获取当前时间
            #endregion
            //1、该条消息记录存储到数据库，该条信息记录是客服发给客户，创 MsgSToC 对象，（必要）按照下面获取。
            //一条消息记录有 CreateTime、ToUserName（客户OpenId）、FromUserName（公众号AppId）、客服Id、Content 
            //2、发到微信公众号，其转发到指定客户

            //1、获取当前客服Id和Name，即登录的OrchardCore.User 对象
            var kefuName = "客服1号";  
            var kefuId = 001;
            //2、回复的客户，从前端获取OpenId
            var OpenId =  body.ToUserName;
            //3、所在公众号，从前端获取公众号的AppId
            var AppId = body.FromUserName;
            //4、消息内容，从前端获得
            var Content = body.Content;
            //5、获取当前时间
            //var time = DateTimeOffset.Now;  
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //前端需要的数据
            body.CreateTime = time;
            

            //await _hubContext.Clients.All.SendAsync("Notify", $"Home page loaded at: {DateTime.Now}");
            //放在上面有没有可能发送消息的时候假设时间间隔短没有存到数据库？
            await Clients.All.SendAsync("Recv", body);

            //1、body存到数据库
            //2、body发到微信公众号,post请求

            //string text = body.Content.ToString();
            //查AppId找到Secret，获取AccessToken方法，后期可以包装该方法，把CommonApi.GetToken包在里面
            var sasas = CommonApi.GetToken(AppId, "7f331ad801e1cf978024c31c5f0a5b9f");
            var AccessToken = sasas.access_token;

            CustomApi.SendText(AccessToken, OpenId, Content);

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
