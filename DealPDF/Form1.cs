using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DealPDF
{
    public partial class Form1 : Form
    {
        public string s_d = "";
        public Form1()
        {
            InitializeComponent();
        }


        public int checkX(string s, int index, int last_index)
        {
            if (s[index + 1] == '—' /*&& s[index - 2] == ';'*/) return 0;
            //if (s[index + 1] >= 48 && s[index + 1] <= 57) return 0;
            if(s[last_index +1 ] >= 48 && s[last_index + 1] <= 57) return 0;
            if (s[index - 2] == '.') return 0;
            if (s[index - 2] == ':') return 0;
            return 1;
        }
        public string  replaceX(string s) {
            string s1="";
            int fi = 0;
            int last_fi = 0;
            int startIndex = 0;
            int pi = 0;
            int len = 0;
            len = s.Length;
            //fi = s.IndexOf("\n");
            s1 = s;
            while((fi = s.IndexOf("\n",startIndex ))> 0 && fi > pi )
            {
               // if (s[fi - 2] != '.' && s[fi-2] != ':' )
                if(checkX(s, fi, last_fi) > 0)
                {
                    s1 = s.Substring(0, fi - 1) + " " + s.Substring(fi + 1, s.Length - fi - 1);   
                }
                last_fi = fi;
                s = s1;
                startIndex = fi + 1;
                if (startIndex > s.Length) startIndex = s.Length-1;
                pi = fi+1;
               
            }
        
            s = s1;
            s_d = s;
          
            return s1;    
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            string a_tip= "";
            string str1;
            try
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    //MessageBox.Show((string)iData.GetData(DataFormats.Text));
                    string str = (string)iData.GetData(DataFormats.UnicodeText);
                    replaceX(str);
                    textBox1.Text = s_d;
                }
                else
                {
                    MessageBox.Show("目前剪贴板中数据不可转换为文本", "错误");
                }
            }
            catch (Exception )
            {
                MessageBox.Show("error");
            }


        }

        public static string HttpPost(string url, string postDataStr)
        {

            Encoding myEncoding = Encoding.GetEncoding("UTF-8");  //选择编码字符集
            string data =  postDataStr;
            byte[] bytesToPost = System.Text.Encoding.Default.GetBytes(data); //转换为bytes数据

            string responseResult = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";// "application/json"; //"application/x-www-form-urlencoded";// 
            request.ContentLength = bytesToPost.Length;

            Stream myRequestStream = request.GetRequestStream();
            //StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            //myStreamWriter.Write(postDataStr);
            //myStreamWriter.Close();
            myRequestStream.Write(bytesToPost, 0, bytesToPost.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
           // if(response.StatusCode == HttpStatusCode.OK)
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();


            return retString;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //String str = textBox1.Text;
            // String str2 = HttpPost("https://fanyi.baidu.com/v2transapi?from=en&to=zh", str);
            //textBox1.Text = str2;
            Clipboard.SetText(s_d);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox1.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string x = textBox1.Text;

            if (x[0] == '—') {
                MessageBox.Show("yes");
            }
        }
    }
}
