using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Commons.Events
{
    /// <summary>
    /// 用于弹出等待界面
    /// </summary>
    public class UpdateModel
    {
        public bool IsOpen { get; set; }
    }

    public class UpdateLoadingEvent : PubSubEvent<UpdateModel>
    {

    }
}
