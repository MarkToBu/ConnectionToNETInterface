using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using static WindowsFormsApp1.JsonParser;

namespace WindowsFormsApp1.utils
{
    class SQLDMl
    {
        /// <summary>
        /// 拼接 MySqlParameter 数组
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MySqlParameter[] GetParameterArray(string[] name, object[] data)
        {
            List<MySqlParameter> list = new List<MySqlParameter>();
            for (int i = 0; i < name.Length; i++)
            {
                list.Add(new MySqlParameter(name[i], data[i]));
            }

            return list.ToArray();
        }

        public static void InsertOrder(OrdersItem order)
        {
            // 这里应该不会有 SQL 注入问题
            string sql = "INSERT INTO orders(orderid,account,money,starttime,endtime,remark,state,userid) VALUES (@orderid,@account,@money,@starttime,@endtime,@remark,@state,@userid)";
            MySqlParameter[] sqlParameter = GetParameterArray(new string[] { "@orderid", "@account", "@money", "@starttime", "@endtime", "@remark", "@state", "@userid" },
                new object[] { order.OrderId, order.Account, order.Amount, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "", "3", ConfigurationManager.AppSettings["acount"] });

            ConnHelper.GetcomPara(sql, sqlParameter);
        }
        /// <summary>
        /// 更新订单的状态信息
        /// 
        /// 更新最后的时间和状态，根据订单编号
        /// </summary>
        /// <param name="order"></param>
        public static void UpdateOrder(OrdersItem order, string state, string remark)
        {
            // 这里应该不会有 SQL 注入问题
            string sql = "UPDATE orders SET endtime=@endtime,remark=@remark,state=@state WHERE orderid=@orderid";
            MySqlParameter[] sqlParameter = GetParameterArray(new string[] { "@endtime", "@remark", "@state", "@orderid" },
                            new object[] { DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), remark, state, order.OrderId });

            ConnHelper.GetcomPara(sql, sqlParameter);


            //string sql = "UPDATE orders SET account=@account,money=@money,endtime=@endtime,remark=@remark,state=@state WHERE orderid=@orderid";
            //MySqlParameter[] sqlParameter = getParameterArray(new string[] { "@account", "@money", "@endtime", "@remark", "@state", "@orderid" },
            //new object[] { order.Account, order.Amount, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "", state, order.OrderId });
        }
    }
}