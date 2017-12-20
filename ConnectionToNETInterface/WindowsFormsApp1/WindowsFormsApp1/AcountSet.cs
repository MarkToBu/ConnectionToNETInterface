using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Xml;

namespace WindowsFormsApp1
{
    public partial class AcountSet : Form
    {
        public AcountSet()
        {
            InitializeComponent();
        }


         //保存用户账号、密码和地址到winform的运行 配置文件
        private void Button1_Click(object sender, EventArgs e)
        {
            string acount = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string Url = textBox3.Text.Trim();
            if ((textBox1.Text.Trim() != "") && (textBox2.Text.Trim() != "") && (textBox3.Text.Trim() != ""))
            {

                SetInfo(acount, password, Url);
                this.Close();
            }
            else { 
                MessageBox.Show("文本框不能为空，", "警告", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        private void SetInfo(string acount,string password ,string Url) {


            //添加引用，调用配置文件
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
             
            config.AppSettings.Settings["acount"].Value =acount.Trim();
            config.AppSettings.Settings["password"].Value = password.Trim();
            config.AppSettings.Settings["URL"].Value = Url.Trim();
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");



            //修改保存后的文件名以生成执行档的名称开头。

            //config.Save(ConfigurationSaveMode.Modified);
            //UpdateConfig("acount", "likangwen");



        }

        private void AcountSet_Load(object sender, EventArgs e)
        {
            textBox1.Text = ConfigurationManager.AppSettings["acount"];
            textBox2.Text = ConfigurationManager.AppSettings["password"];
            textBox3.Text = ConfigurationManager.AppSettings["URL"];
        }
        //修改配置文件中的账号名和密码
        //private void UpdateConfig(string name, string Xvalue)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(Application.ExecutablePath + ".config");
        //    XmlNode node = doc.SelectSingleNode(@"//add[@key='" + name + "']");
        //    XmlElement ele = (XmlElement)node;
        //    ele.SetAttribute("value", Xvalue);
        //    doc.Save(Application.ExecutablePath + ".config");
        //    MessageBox.Show("成功");
        //}
    }
}
