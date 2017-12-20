using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static WindowsFormsApp1.JsonParser;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 定义队列
        private ConcurrentQueue<OrdersItem> query = new ConcurrentQueue<OrdersItem>();

        //定义委托
        private delegate void updateListDele(string str, string threadID);

        // 线程运行标志
        private static bool flag = true;

        private void UpdateList(string str, string threadID)
        {
            lock (this)
            {

                if (rtbOrderInfo.Lines.Length >= 1000)
                {
                    rtbOrderInfo.Clear();
                }
                //加了个同步锁，具体的更新UI的逻辑
                rtbOrderInfo.AppendText(System.DateTime.Now + "::线程ID " + threadID + "----->" + str + "\n");
                rtbOrderInfo.ScrollToCaret();
            }
        }




        private void Button1_Click(object sender, EventArgs e)
        {
            AcountSet ac = new AcountSet
            {
                StartPosition = FormStartPosition.CenterParent
            };
            ac.ShowDialog();

        }
        ///<summary> 
        ///返回*.exe.config文件中appSettings配置节的value项  
        ///</summary> 
        ///<param name="strKey"></param> 
        ///<returns></returns> 
        public static string GetAppConfig(string strKey)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }

        //开始按钮
        private void Button2_Click(object sender, EventArgs e)
        {

            if (GetAppConfig("acount") == "" || GetAppConfig("password") == "" || GetAppConfig("URL") == "")
            {
                MessageBox.Show("请先进行初始化配置");
                return;
            }
            flag = true;
            // 获取订单 (一个线程)
            ThreadPool.QueueUserWorkItem(new WaitCallback(GetOrderWork));

            // 处理订单（10个线程）
            for (int i = 0; i < 10; i++)
            {
                // 启动十个线程来获取
                ThreadPool.QueueUserWorkItem(new WaitCallback(OrderProcessingWork));
            }

            button2.Enabled = false;
            button3.Enabled = true;









            //MessageBox.Show(ConfigurationManager.AppSettings["acount"]);

            //string key = "ChannelId" + ConfigurationManager.AppSettings["acount"]
            //+ "Secretkey" + ConfigurationManager.AppSettings["password"];
            //string Hmac = GetMD5(key);

            ////ChannelId = 10019 & Hmac = d9210d9bc9631140e9c3cf0d27bbba17 & Version = 0.1 & GoodsType = TelGoods

            ////HttpUtils.OpenReadWithHttps(string URL, string strPostdata, string strEncoding);
            ////string url = "ChannelId="+ConfigurationManager.AppSettings["acount"]
            ////            + "&Hmac="+Hmac+ "&Version=0.1&GoodsType = TelGoods";
            //string data = PostData(ConfigurationManager.AppSettings["acount"], Hmac, 0.1 + "", "TelGoods");


            //string jsonData = HttpUtils.OpenReadWithHttps(ConfigurationManager.AppSettings["URL"], data, "utf-8");
            //rtbOrderInfo.Text = jsonData;
            //MessageBox.Show(data);

            //Root root = JsonConvert.DeserializeObject<Root>(jsonData);

            ////MessageBox.Show(root.Data.ToString());
            ////MessageBox.Show(root.Message);
            ////MessageBox.Show(root.Result);

            //List<OrdersItem> orderitems = root.Data.Orders;
            //foreach (var itmes in orderitems) {
            //    StringBuilder builder = null;
            //    builder.Append(itmes.OPType + itmes.Account + itmes.Location + itmes.GoodsName);

            //    MessageBox.Show(builder.ToString());
            //}




        }


        //传递数据Hmac
        private string PostData(string acount, string Hmac, string version, string goodstype) {

            string url = "ChannelId=" + acount + "&Hmac=" + Hmac + "&Version=" + version + "&GoodsType=" + goodstype;

            return url;
        }

        //使用MD5加密字符串
        public string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = Encoding.Default.GetBytes(myString.Trim());    //tbPass为输入密码的文本框  

            //string Hmac = md5.ComputeHash(result).ToString();
            ;
            string Hmac = BitConverter.ToString(md5.ComputeHash(result)).Replace("-", "");
            //MessageBox.Show(Hmac);
            return Hmac;
        }


        //暂停按钮
        private void Button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button3.Enabled = false;
            flag = false;
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {

        }



        private void Button4_Click(object sender, EventArgs e)
        {
            SelectINfo sinfo = new SelectINfo
            {
                StartPosition = FormStartPosition.CenterParent
            };
            sinfo.ShowDialog();
        }

        private void Button5_Click(object sender, EventArgs e)
        {


        }



        //获取订单的方法
        private List<OrdersItem> GetOrderList()
        {
            //string Hmac = "ChannelId"+ApplicationConfig.ID+"Secretkey" + ApplicationConfig.KEY;
            string key = "ChannelId" + ConfigurationManager.AppSettings["acount"] + "Secretkey" + ConfigurationManager.AppSettings["password"];
            string Hmac = GetMD5(key);
            string data = PostData(ConfigurationManager.AppSettings["acount"], Hmac, 0.1 + "", "TelGoods");
            // JSON 解析
            string jsonData = HttpUtils.OpenReadWithHttps(ConfigurationManager.AppSettings["URL"], data, "utf-8");


            Root root = JsonConvert.DeserializeObject<Root>(jsonData);


            if (root.Result.Equals("Success"))
            {
                rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "请求订单成功", GetThreadID() });
                return root.Data.Orders;
            }

            // 如果出现错误
            rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { root.Message, GetThreadID() });
            return null;
        }

        //方法——获取当前线程的id
        public static string GetThreadID()
        {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }


        private void GetOrderWork(object o)
        {
            List<OrdersItem> list;
            while (flag)
            {
                Thread.Sleep(1000); // 防止卡死
                if (query.Count >= 10)
                {
                    continue;
                }
                // 队列订单不足 10 进行获取
                list = GetOrderList();
                if (list == null || list.Count == 0)
                {
                    rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "请求的订单数为 0", GetThreadID() });
                    continue;
                }
                rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "请求的订单数为" + list.Count, GetThreadID() });
                // 加入队列
                foreach (OrdersItem order in list)
                {
                    query.Enqueue(order);
                }
                // 清空列表
                list.Clear();
            }
        }


        // 本笔订单处理完成后继续从队列中获取新订单
        /// </summary>
        private void OrderProcessingWork(object o)
        {
            rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "线程启动....处理订单中", GetThreadID() });

            while (flag)
            {
                Thread.Sleep(100);  // 防卡死
                // 从队列获取订单
                OrdersItem order;
                query.TryDequeue(out order);
                if (order == null)
                {
                    continue;
                }
                else if (order.OrderId.Length == 0)
                {
                    rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "出现空订单，已跳过", GetThreadID() });
                    continue;
                }
                rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "正在处理订单" + order.OrderId, GetThreadID() });
                // 首先进行订单确认
                if (ConfirmOrder(order))
                {
                    // 入库
                    utils.SQLDMl.InsertOrder(order);

                    // 处理订单，返平台结果
                    ReturnResult(order);
                }
            }

            rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "线程停止....", GetThreadID() });
        }

        /// <summary>
        /// 确认订单是否有效
        /// 
        /// TODO
        /// </summary>
        private bool ConfirmOrder(OrdersItem order)
        {
            string querenURL = "http://api.dev.zonghejiaofei.net/api/Channel/RechargeConfirm";
            string pwdKey = "ChannelId" + GetAppConfig("acount") + "OrderId" + order.OrderId + "Secretkey" + GetAppConfig("password");
            string reqData = "ChannelId=" + GetAppConfig("acount") + "&OrderId=" + order.OrderId + "&Hmac=" +GetMD5(pwdKey);
            //string result = HttpUtils.postRequest(querenURL, reqData);

             

            //从接口上发送请求和data
            string jsonData = HttpUtils.OpenReadWithHttps(querenURL,reqData,"utf-8");
            // JSON 解析
            ConfirmEntity confirmResult = JsonConvert.DeserializeObject<ConfirmEntity>(jsonData);

            if (confirmResult.Result.Equals("Success"))
            {
                // TODO
                // 返回 false 应该是表示订单已处理
                if (confirmResult.Data.Res)
                {
                    rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "订单确认成功，状态：" + confirmResult.Data.Res,GetThreadID() });
                    return true;
                }
                else
                {
                    rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "订单确认成功，状态：" + confirmResult.Data.Res + "; 已跳过处理", GetThreadID() });
                    return false;
                }
            }

            rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "订单确认异常，错误信息：" + confirmResult.Message,GetThreadID() });
            return false;
        }

        /// <summary>
        /// 返平台结果
        /// </summary>
        private void ReturnResult(OrdersItem order)
        {
           // ConfigurationManager.AppSettings["acount"] + "Secretkey" + ConfigurationManager.AppSettings["password"];
            string state = "Success";
            string url = "http://api.dev.zonghejiaofei.net/api/Channel/Result";
            string key = "ChannelId" + ConfigurationManager.AppSettings["acount"] + "OrderId" + order.OrderId + "Secretkey" + ConfigurationManager.AppSettings["password"] + "State" + state;

            string reqData = "ChannelId=" + ConfigurationManager.AppSettings["acount"] + "&OrderId=" + order.OrderId + "&State=" + state + "&Hmac=" + GetMD5(key);

            string result = HttpUtils.OpenReadWithHttps(url, reqData, "utf-8");
            
            // JSON 解析
            NetSpaceEntity netSpaceResult = JsonConvert.DeserializeObject<NetSpaceEntity>(result);

            if (netSpaceResult.Result.Equals("Success"))
            {
                rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "订单处理成功，状态标记为" + netSpaceResult.Data.State,  GetThreadID() });
                // 更新数据库 状态
                utils.SQLDMl.UpdateOrder(order, netSpaceResult.Data.State, "");
                return;
            }

            rtbOrderInfo.Invoke(new updateListDele(UpdateList), new object[] { "订单处理错误，异常为：" + netSpaceResult.Message,GetThreadID() });
        }

        // 启动后执行，初始化数据，如果保存的话
    

    }
}
