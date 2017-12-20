using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApp1
{
    public class JsonParser
    {
                //每个订单对象
        public class OrdersItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string OPType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Account { get; set; }
            /// <summary>
            /// 山东 烟台
            /// </summary>
            public string Location { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Amount { get; set; }
            /// <summary>
            /// 山东烟台移动10元
            /// </summary>
            public string GoodsName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int GoodsId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string OrderId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Type { get; set; }
        }
                  

           //订单组
        public class Data
        {
            /// <summary>
            /// 
            /// </summary>
            public List<OrdersItem> Orders { get; set; }
        }
           
          //每个订单
        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public Data Data { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Result { get; set; }
            /// <summary>
            /// 操作成功
            /// </summary>
            public string Message { get; set; }
        }
    }
     
}
