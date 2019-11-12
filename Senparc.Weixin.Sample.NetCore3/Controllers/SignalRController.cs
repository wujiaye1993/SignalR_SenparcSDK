using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.Entities.Request;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler;
using Senparc.Weixin.Sample.NetCore3.Models;
using Senparc.Weixin.Sample.NetCore3.Service;

namespace Senparc.Weixin.Sample.NetCore3.Controllers
{
    public class SignalRController : Controller
    {
        
        public static readonly string Token = Config.SenparcWeixinSetting.Token;//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = Config.SenparcWeixinSetting.EncodingAESKey;//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = Config.SenparcWeixinSetting.WeixinAppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。

        readonly Func<string> _getRandomFileName = () => SystemTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);
        //public RequestMessageText requestMessageText;

        SenparcWeixinSetting _senparcWeixinSetting;

        //注入_hubContext，注册管道
        private readonly IHubContext<SingleHub> _hubContext;

        public SignalRController(IHubContext<SingleHub> hubContext, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            _hubContext = hubContext;
            _senparcWeixinSetting = senparcWeixinSetting.Value;
        }

        //构建顾客消息对象
        MessageBody body = new MessageBody();
        //SingleHub hub = new SingleHub();

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Get")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }


        [HttpPost]
        [ActionName("Index")]
        public async Task<ActionResult> Post(PostModel postModel)
        {
            /* 异步请求请见 WeixinAsyncController（推荐） */

/*            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }*/

            #region 打包 PostModel 信息

            postModel.Token = Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致（必须提供）

            #endregion

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制（实际最大限制 99999）
            //注意：如果使用分布式缓存，不建议此值设置过大，如果需要储存历史信息，请使用数据库储存
            var maxRecordCount = 10;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。

            MemoryStream stream = new MemoryStream();
            await Request.Body.CopyToAsync(stream);
            //var messageHandler1 = new CustomMessageHandler(Request.Body, postModel, maxRecordCount);
            var messageHandler = new CustomMessageHandler(stream, postModel, maxRecordCount); //在这里走消息流程，断点自己看

            
            
            #region 设置消息去重设置

            /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
             * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的 RequestMessage */
            messageHandler.OmitRepeatedMessage = true;//默认已经是开启状态，此处仅作为演示，也可以设置为 false 在本次请求中停用此功能

            #endregion
            
            try
            {
                messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）

                //messageHandler.Execute();//执行微信处理过程（关键）

                //这里调用WeCHatHub类的send方法建立SignalR管道，前端-服务器-微信(CustomApi发送客服消息，白名单)，微信-服务器-前端(Post)要用同个hub实例管道对象


                //拿到请求消息内容
                var textMessageFromWeixin = messageHandler.RequestMessage as RequestMessageText; //仿照写法，自己搜
                                                                                                 //把 contentFromWeixin 放到 SignalR 管道中（SingleHub），再由管道发送到前端显示
                                                                                                 //body.CustomMessageFromWeixin = contentFromWeixin;

                //赋给SingleHub管道里的字段，构建顾客消息对象
                //body.CustomMessageFromWeixin = contentFromWeixin;
                #region 该方法获得的数据
                /*
                 * 1、ToUserName     公众号AppId
                 * 2、FromUserName   发送方帐号（一个OpenID）
                 * 3、CreateTime	    消息创建时间 （整型）
                 * 4、MsgType	    消息类型，文本为text
                 * 5、Content	    文本消息内容
                 * 6、MsgId	        消息id，64位整型
                 */
                #endregion

                #region 发到前端需要的数据
                /*
                 * 由于发信息给微信不经过这里，所以只负责处理来自微信的消息
                 * 发到前端需要的数据，                       
                 * 1、公众号（Name） ToUserName（AppId）
                 * 2、客户信息（Name） FromUserName（OpenId） （间接获得）
                 * 3、客服信息（Name） OrchardCore.User （间接获得）
                 * 4、消息体    Content
                 * 5、创建时间  Create 
                 */
                #endregion


                var contentFromWeixin = textMessageFromWeixin.Content;
                var gzhAppId = textMessageFromWeixin.ToUserName;   //开发者微信号，这里该执行根据AppId查公众号名
                var FromUserOpenId  = textMessageFromWeixin.FromUserName;   //发送发账号，OpenId，后善：这里该执行根据OpenId查询微信用户信息，存储到数据库
                var CreateTime = textMessageFromWeixin.CreateTime;  //DateTimeOffset 类型
                var MsgType = textMessageFromWeixin.MsgType;        //消息类型，用于判断执行的处理方法
                var MsgId = textMessageFromWeixin.MsgId;        //消息id，64位，用于区分消息记录
                //该条信息记录对象存储到数据库

                var username = "小微同学";  //假设根据OpenId查找得到的客户名
                body.CustomerName = username; //前端显示的客户名
                body.ToUserName = AppId; //1、设定发给的公众号，后善：查WeChatMP表根据AppId获取Name
                body.FromUserName = FromUserOpenId;   //2、设定消息来自的人，即客户，后善：改根据OpenId获取客户的微信用户信息的Name，这需找该OpenId是否重复，不重复
                //创建客户对象并存储用户信息（完善Id，Name等）到数据库，重复直接根据OpenId获取客户的微信用户信息的Name
                //只要给公众号发消息，就能获取你的OpenId（OpenId是公众号中普通用户的唯一标识，可以用来获取用户的信息）
                body.ToKefuName = "客服1号" ;  //3、设定消息发给谁，即客服，后善：获取客服集合队伍，出栈，获取客服Id（OrchardUserId），根据Id获取客服Name
                body.Content = contentFromWeixin;   //4、消息体
                body.CreateTime = CreateTime.ToString("yyyyMMdd-HHmmss");   //5、创建时间 ，完成

                body.TextName = username;       //设定显示在前端当前发信息的人，在该方法时，为微信的客户
                
                //在Controller里使用管道输送
                await _hubContext.Clients.All.SendAsync("Recv", body);

                //张磊提到的使用静态类方法创建同一个实例调用
                //await Classhub.asd.All.SendAsync("Recv", body);

                //hub.Send(body);
                messageHandler.SaveResponseMessageLog();//记录 Response 日志（可选）

                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                //return new WeixinResult(messageHandler);//v0.8+
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
            }
            catch (Exception ex)
            {
                #region 异常处理
                WeixinTrace.Log("MessageHandler错误：{0}", ex.Message);

                using (TextWriter tw = new StreamWriter(ServerUtility.ContentRootMapPath("~/App_Data/Error_" + _getRandomFileName() + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                }
                return Content("");
                #endregion
            }
        }



        public async Task<IActionResult> Index()
        {
            //有这个方法FromUserName传不过去
            //await _hubContext.Clients.All.SendAsync("Recv", body);
            return View();
        }
    }
}