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
        /// <summary>
        /// 存储处理后的文本（全局临时变量）
        /// </summary>
        private string _processedText = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        #region 文本换行处理核心逻辑
        /// <summary>
        /// 检查当前换行位置是否需要保留（不需要替换为空格）
        /// </summary>
        /// <param name="content">原文</param>
        /// <param name="currentLineBreakIndex">当前换行符索引</param>
        /// <param name="lastLineBreakIndex">上一个换行符索引</param>
        /// <returns>0=不处理 1=需要处理</returns>
        private int CheckNeedReplace(string content, int currentLineBreakIndex, int lastLineBreakIndex)
        {
            // 换行后是破折号 → 不处理
            if (content[currentLineBreakIndex + 1] == '—')
                return 0;

            // 上一个换行后是数字 → 不处理
            if (char.IsDigit(content[lastLineBreakIndex + 1]))
                return 0;

            // 换行前两位是 . 或 : → 不处理
            if (content[currentLineBreakIndex - 2] == '.' || content[currentLineBreakIndex - 2] == ':')
                return 0;

            // 符合条件 → 需要替换换行
            return 1;
        }

        /// <summary>
        /// 智能移除文本中不必要的换行，替换为空格
        /// </summary>
        /// <param name="originalText">原始文本</param>
        /// <returns>处理后文本</returns>
        private string RemoveUnnecessaryLineBreaks(string originalText)
        {
            if (string.IsNullOrEmpty(originalText))
                return string.Empty;

            string tempText = originalText;
            int textLength = tempText.Length;

            int lastLineBreakIndex = 0;
            int startSearchIndex = 0;
            int previousPosition = 0;

            // 循环查找所有换行符并处理
            while (startSearchIndex < textLength)
            {
                // 查找下一个换行符
                int currentLineBreakIndex = tempText.IndexOf('\n', startSearchIndex);

                // 没有找到换行 → 退出循环
                if (currentLineBreakIndex == -1 || currentLineBreakIndex <= previousPosition)
                    break;

                // 检查是否需要替换
                if (CheckNeedReplace(tempText, currentLineBreakIndex, lastLineBreakIndex) > 0)
                {
                    // 把换行符替换成空格
                    tempText = tempText.Substring(0, currentLineBreakIndex - 1) + " " +
                               tempText.Substring(currentLineBreakIndex + 1);
                }

                // 更新索引
                lastLineBreakIndex = currentLineBreakIndex;
                startSearchIndex = currentLineBreakIndex + 1;
                previousPosition = currentLineBreakIndex + 1;
            }

            // 保存到全局变量
            _processedText = tempText;
            return tempText;
        }
        #endregion


        #region HTTP POST 请求工具方法
        /// <summary>
        /// 发送UTF-8编码的POST请求（备用接口）
        /// </summary>
        public static string SendPostRequest(string url, string postData)
        {
            try
            {
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                // 创建请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                request.ContentLength = postBytes.Length;

                // 写入请求数据
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(postBytes, 0, postBytes.Length);
                }

                // 读取响应结果
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            string currentText = textBox1.Text;

            if (!string.IsNullOrEmpty(currentText) && currentText[0] == '—')
            {
                MessageBox.Show("开头是破折号！", "检测结果");
            }
        }
        /// <summary>
        /// 从剪贴板读取文本 → 处理 → 显示到文本框
        /// </summary>
        private void btnProcessClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                // 获取剪贴板数据
                IDataObject clipboardData = Clipboard.GetDataObject();

                if (clipboardData.GetDataPresent(DataFormats.Text))
                {
                    // 读取文本
                    string originalText = (string)clipboardData.GetData(DataFormats.UnicodeText);

                    // 处理文本
                    string resultText = RemoveUnnecessaryLineBreaks(originalText);

                    // 显示结果
                    textBox1.Text = resultText;
                }
                else
                {
                    MessageBox.Show("剪贴板中没有可读取的文本", "处理失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 将处理后的文本复制回剪贴板
        /// </summary>
        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_processedText))
            {
                Clipboard.SetText(_processedText);
                MessageBox.Show("已复制到剪贴板！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("暂无处理后的文本", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 清空文本框
        /// </summary>
        private void btnClearTextBox_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Focus();
        }
    }
}
