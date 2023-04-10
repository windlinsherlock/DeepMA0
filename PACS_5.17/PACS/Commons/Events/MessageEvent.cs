using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Commons.Events
{
    /// <summary>
    /// 用于组件、页面通信
    /// </summary>
    public class MessageModel
    {
        public string Filter { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }
    }

    public class MessageEvent : PubSubEvent<MessageModel>
    {
    }
}
