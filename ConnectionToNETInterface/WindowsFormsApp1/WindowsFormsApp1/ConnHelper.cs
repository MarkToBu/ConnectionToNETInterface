using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace WindowsFormsApp1
{
    class ConnHelper
    {
        public static MySqlConnection GetSqlConnection()
        {
            string myConnectionString = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;
            MySqlConnection myConnection = new MySqlConnection(myConnectionString);
            return myConnection;
        }
        public static  DataSet SelectAllOrders()
        {
            //string myConnectionString = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;

            
            MySqlConnection conn = GetSqlConnection();
            conn.Open();
            MySqlCommand cmd=conn.CreateCommand();
            cmd.CommandText = "select * from orders";

           MySqlDataAdapter myAdt = new MySqlDataAdapter(cmd);

            DataSet ds = new DataSet();
           myAdt.Fill(ds); 
            conn.Close();
            return  ds;
        }


        #region 执行数据库命令方法
        /// <summary>
        /// 执行数据库命令方法
        /// </summary>
        /// <param name="M_str_sqlstr" 执行语句></param>
        public static void Getcom(string M_str_sqlstr)
        {
            MySqlConnection sqlcon = GetSqlConnection();
            sqlcon.Open();
            MySqlCommand sqlcom = new MySqlCommand(M_str_sqlstr, sqlcon);
            sqlcom.ExecuteNonQuery();
            sqlcom.Dispose();
            sqlcon.Close();
            sqlcon.Dispose();
        }

        #endregion

        /// <summary>
        /// 执行数据库命令方法
        /// 
        /// 使用参数化查询，防止 SQL 注入
        /// </summary>
        /// <param name="M_str_sqlstr" 执行语句></param>
        public static void GetcomPara(string M_str_sqlstr, MySqlParameter[] parameter)
        {
            MySqlConnection sqlcon = GetSqlConnection();
            sqlcon.Open();
            MySqlCommand sqlcom = new MySqlCommand(M_str_sqlstr, sqlcon);
            sqlcom.Parameters.AddRange(parameter);
            sqlcom.ExecuteNonQuery();
            sqlcom.Dispose();
            sqlcon.Close();
            sqlcon.Dispose();
        }

        #region 创建数据集的方法
        /// <summary>
        /// 创建数据集的方法
        /// </summary>
        /// <param name="M_Str_sqlstr" 数据库执行语句></param>
        /// <param name="M_str_table" 数据集表名（不是数据库表名）></param>
        /// <returns></returns>
        public static DataSet Getds(string M_Str_sqlstr, string M_str_table)
        {
            MySqlConnection sqlcon = GetSqlConnection();
            MySqlDataAdapter sqlds = new MySqlDataAdapter(M_Str_sqlstr, sqlcon);
            DataSet myds = new DataSet();
            sqlds.Fill(myds, M_str_table);
            return myds;
        }
        #endregion

        /// <summary>
        /// 创建数据集的方法
        /// 
        /// 使用参数化查询，防止 SQL 注入
        /// </summary>
        /// <param name="M_Str_sqlstr" 数据库执行语句></param>
        /// <param name="M_str_table" 数据集表名（不是数据库表名）></param>
        /// <returns></returns>
        public static DataSet GetDSParam(string M_Str_sqlstr, string M_str_table, MySqlParameter[] parameter)
        {
            MySqlConnection sqlcon = GetSqlConnection();
            MySqlDataAdapter sqlds = new MySqlDataAdapter(M_Str_sqlstr, sqlcon);
            sqlds.SelectCommand.Parameters.AddRange(parameter);
            DataSet myds = new DataSet();
            sqlds.Fill(myds);
            return myds;
        }

        #region 创建数据阅读器的方法
        /// <summary>
        /// 创建数据阅读器的方法
        /// </summary>
        /// <param name="M_str_sqlstr" 数据库执行语句></param>
        /// <returns></returns>
        public static MySqlDataReader Getread(string M_str_sqlstr)
        {
            MySqlConnection sqlcon = GetSqlConnection();
            MySqlCommand sqlcom = new MySqlCommand(M_str_sqlstr, sqlcon);
            sqlcon.Open();

            MySqlDataReader sqlreader = sqlcom.ExecuteReader(CommandBehavior.CloseConnection);
            return sqlreader;
        }
        #endregion
    }
}
