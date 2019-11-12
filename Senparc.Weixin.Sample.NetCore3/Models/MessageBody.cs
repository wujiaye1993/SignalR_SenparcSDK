using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Senparc.Weixin.Sample.NetCore3.Models
{
    public class MessageBody
    {
        /*
         * 消息类，负责SignalR需要传递后端前端的信息
         */
        
        //客服名
        public string ToKefuName { get; set; }
        //公众号集合
        public List<string> MP_AccountList { get; set; }
        //数据类型
        public int Type { get; set; }
        //开发者微信号
        public string ToUserName { get; set; }
        //发送方账号
        public string FromUserName { get; set; }
        //消息创建时间，整型
        public DateTimeOffset CreateTime { get; set; }
        //消息类型，文本为text
        public string MsgType { get; set; }
        //消息内容
        public string Content { get; set; }
        //发信息的人，由于前端的需求，在SignalR中重复使用，所以不能分开两个变量，发送消息的人只能是唯一变量 
        //所以可以 textName = toUserName, textName = FromUserName 完成需求
        public string TextName { get; set; }
        public string UserName { get; set; }

        //消息id，64位整型
        public long MsgId { get; set; }
        //public string CustomMessageFromWeixin { get; set; }
    }
}
