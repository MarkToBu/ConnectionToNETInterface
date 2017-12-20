using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class SelectINfo : Form
    {
        public SelectINfo()
        {
            InitializeComponent();
        }

        private void SelectINfo_Load(object sender, EventArgs e)
        {
            cbStatus.Items.Add("成功");
            cbStatus.Items.Add("失败");
            cbStatus.Items.Add("异常");
        }

        private void SelectAll_Click(object sender, EventArgs e)
        {
            Bind();



             
        }
        protected void Bind(){
            BindToGridView(ConnHelper.SelectAllOrders());
         }

        protected void BindToGridView(DataSet dt)
        {
            if (dataGridView1.DataSource != null) {
                dataGridView1.DataSource = null;
            }
            dataGridView1.DataSource = dt.Tables[0];
            
        }

        private void SelectSelected_Click(object sender, EventArgs e)
        {
           string  orderID = tbOrderID.Text.Trim();
           string  phone = tbPhone.Text.Trim();
           string  amount = tbAmount.Text.Trim();
            string status =cbStatus.SelectedIndex==-1?"": cbStatus.SelectedIndex+1+"";
            MessageBox.Show(status);
            string  dateStart = dtpDateStart.Value.ToString("yyyy-MM-dd").Trim();
           string  dateEnd = dtpDateEnd .Value.AddDays(1).ToString("yyyy-MM-dd").Trim(); ;
            string userID = ConfigurationManager.AppSettings["acount"];

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  id 供货商编号,orderid 订单号,account 手机号, money 金额, starttime 开始时间, endtime 结束时间, remark 备注, state 状态, userid 自己的编号  FROM orders WHERE starttime >= @starttime and endtime <= @endtime");
            List<MySqlParameter> list = new List<MySqlParameter>();

            list.Add(new MySqlParameter("@starttime", dateStart + "%"));
            list.Add(new MySqlParameter("@endtime", dateEnd + "%"));
           

            if (orderID.Length != 0)
            {
                sb.Append(" and orderid=@orderid");
                list.Add(new MySqlParameter("@orderid", orderID));
            }
            if (phone.Length != 0)
            {
                sb.Append(" and account=@account");
                list.Add(new MySqlParameter("@account", phone));
            }
            if (amount.Length != 0)
            {
                sb.Append(" and money=@money");
                list.Add(new MySqlParameter("@money", amount));
            }
            if (status.Length != 0)
            {
                sb.Append(" and state=@state");
                list.Add(new MySqlParameter("@state", status));
            }
            sb.Append(" and  userid=@userid");
            list.Add(new MySqlParameter("@userid", userID));

            // 开始查询
            DataSet ds = ConnHelper.GetDSParam(sb.ToString(), "orders", list.ToArray());
            reState(ds);
            BindToGridView(ds);
            
        }
        private void reState(DataSet dt)
        {
            foreach (DataRow dr in dt.Tables[0].Rows)
            {
                string state = dr["状态"].ToString();
                dr.BeginEdit();
                if (state.Equals("1"))
                {
                    dr["状态"] = "成功";
                }
                else if (state.Equals("2"))
                {
                    dr["状态"] = "失败";
                }
                else if (state.Equals("3"))
                {
                    dr["状态"] = "未知";
                }
                dr.EndEdit();
            }
        }
    }
}
