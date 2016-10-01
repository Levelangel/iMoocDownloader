using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;

namespace imooc_download
{
    public partial class Form1 : Form
    {
        private List<Class> classes = new List<Class>();
        delegate void SetControlsCallBack(object obj);
        Thread thGetinfo,thDownloadFile;
        private bool isTimeOut = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("网址不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            thGetinfo = new Thread(PullClassInfo);
            timer1.Enabled = true;
            button1.Text = "抓取中";
            thGetinfo.Start();
        }

        private void PullClassInfo()
        {            
            string content = GetUrlContent(textBox1.Text);
            if (content == "error")
            {
                isTimeOut = false;
                SetText("查询");
                MessageBox.Show("网址解析错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetText(GetClassName(content));
            if (lblClassName.Text == "")
            {
                isTimeOut = false;
                SetText("查询");
                MessageBox.Show("网址格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Thread.Sleep(10);
            classes = GetClass(content);
            SetControlEnable(1);
            MessageBox.Show("视频链接抓取完毕！可以开始下载了^_^", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetText("查询");
            //SetControlEnable();
        }

        private void SetText(object text)
        {
            if (lblClassName.InvokeRequired || button1.InvokeRequired)
            {
                SetControlsCallBack d = SetText;
                Invoke(d, text);
            }
            else
            {
                if (text.ToString() == "查询")
                {
                    button1.Text = text.ToString();
                }
                else
                {
                    lblClassName.Text = text.ToString();
                }
            }
        }

        private void SetControlEnable(object obj)
        {
            if (groupBox2.InvokeRequired || button3.InvokeRequired)
            {
                SetControlsCallBack d = SetControlEnable;
                Invoke(d,obj);
            }
            else
            {
                groupBox2.Enabled = true;
                button3.Enabled = true;
                timer1.Enabled = false;
            }
        }

        private void ShowNowFile(object fileName)
        {
            if (lblDownloading.InvokeRequired)
            {
                SetControlsCallBack d = ShowNowFile;
                Invoke(d, fileName);
            }
            else
            {
                lblDownloading.Text = fileName.ToString();
            }
        }

        private void SetProgressBarMaxValue(object value)
        {
            if (progressBar1.InvokeRequired)
            {
                SetControlsCallBack d = SetProgressBarMaxValue;
                Invoke(d, value);
            }
            else
            {
                progressBar1.Maximum = int.Parse(value.ToString());
            }
        }

        private void ProgressBarStep(object obj)
        {
            if (progressBar1.InvokeRequired)
            {
                SetControlsCallBack d = ProgressBarStep;
                Invoke(d, obj);
            }
            else
            {
                progressBar1.PerformStep();
            }
        }

        private void SetProgressValue(object obj)
        {
            if (progressBar1.InvokeRequired)
            {
                SetControlsCallBack d = SetProgressValue;
                Invoke(d, obj);
            }
            else
            {
                progressBar1.Value = int.Parse(obj.ToString());
            }
        }

        private void Lock(object isLock)
        {
            if (groupBox1.InvokeRequired || groupBox2.InvokeRequired || button3.InvokeRequired)
            {
                SetControlsCallBack d = Lock;
                Invoke(d, isLock);
            }
            else
            {
                if (isLock.ToString() == "0")
                {
                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;
                }
                else
                {
                    groupBox1.Enabled = true;
                    button3.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 抓取网页内容
        /// </summary>
        /// <param name="url">网页地址</param>
        /// <param name="charset">字符集</param>
        /// <returns>网页内容，失败返回 error</returns>
        private string GetUrlContent(string url,string charset = "UTF-8")
        {
            if (url.Trim() == "")
            {
                return "";
            }
            while (true)
            {
                try
                {
                    //generate http request
                    WebRequest req = WebRequest.Create(url);
                    //use GET method to get url's html
                    req.Method = "GET";
                    req.Timeout = 3000;
                    //use request to get response
                    WebResponse resp = req.GetResponse();
                    if (((HttpWebResponse)resp).StatusCode != HttpStatusCode.OK)
                        continue;
                    string htmlCharset = charset;
                    //use songtaste's html's charset GB2312 to decode html
                    //otherwise will return messy code
                    Encoding htmlEncoding = Encoding.GetEncoding(htmlCharset);
                    StreamReader sr = new StreamReader(resp.GetResponseStream(), htmlEncoding);
                    //read out the returned html
                    string respHtml = sr.ReadToEnd();
                    return respHtml;
                }
                catch (Exception)
                {
                    return "error";
                }
            }
        }

        /// <summary>
        /// 从网页内容获取课程信息
        /// </summary>
        /// <param name="content">网页内容</param>
        /// <returns></returns>
        private List<Class> GetClass(string content)
        {
            List<Class> classes = new List<Class>();
            while (content.Contains("<a href='/video/"))
            {
                string temp = "";
                int t = content.IndexOf("<a href='/video/", 0);
                content = content.Substring(t);
                string strTmp = content;
                strTmp = strTmp.Substring(strTmp.IndexOf("</i>"));
                strTmp = strTmp.Substring(strTmp.IndexOf("</i>"));
                strTmp = strTmp.Substring(5);
                strTmp = strTmp.Substring(0, strTmp.IndexOf("<button"));
                strTmp = strTmp.Trim();
                strTmp = strTmp.Replace("  ", "");
                strTmp = strTmp.Replace("\r\n", "");
                strTmp = strTmp.Substring(0,strTmp.IndexOf("("));
                strTmp = strTmp.Trim();
                strTmp = strTmp.Replace("/", "-");
                strTmp = strTmp.Replace("\\", "-");
                strTmp = strTmp.Replace("*", "-");
                strTmp = strTmp.Replace("|", "-");
                strTmp = strTmp.Replace("<", "-");
                strTmp = strTmp.Replace(">", "-");
                content = content.Substring(16);
                int i = 0;
                while (content[i] != '\'')
                {
                    temp += content[i];
                    i += 1;
                }
                Class myclass = new Class();
                myclass.Title = strTmp;
                myclass.ClassNumber = temp;
                myclass.Url = GetClassUrl(temp);
                classes.Add(myclass);
            }
            return classes;
        }

        /// <summary>
        /// 获取课程名字
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetClassName(string content)
        {
            string tmp = content;
            if (tmp.Contains("<h2 class=\"l\">"))
            {
                tmp = tmp.Substring(tmp.IndexOf("<h2 class=\"l\">"));
                tmp = tmp.Substring(14);
                tmp = tmp.Substring(0, tmp.IndexOf("</h2>"));
                return tmp;
            }
            return "";
        }

        /// <summary>
        /// 获取课程视频地址
        /// </summary>
        /// <param name="classNumber">课程视频号</param>
        /// <returns></returns>
        private ClassUrl GetClassUrl(string classNumber)
        {
            string url = "http://www.imooc.com/course/ajaxmediainfo/?mid=" + classNumber + "&mode=flash";
            string strJson = GetUrlContent(url);
            ClassUrl classUrl = new ClassUrl();
            if (strJson.Contains("http"))
            {
                string tmpUrl = strJson.Substring(strJson.IndexOf("http", StringComparison.Ordinal));
                tmpUrl = tmpUrl.Substring(0, tmpUrl.IndexOf("\",\"", StringComparison.Ordinal));
                tmpUrl = tmpUrl.Replace("\\","");
                classUrl.UrlL = tmpUrl;
                tmpUrl = strJson.Substring(strJson.IndexOf("http", StringComparison.Ordinal));
                tmpUrl = tmpUrl.Substring(tmpUrl.IndexOf("http", 1, StringComparison.Ordinal));
                tmpUrl = tmpUrl.Substring(0, tmpUrl.IndexOf("\",\"", StringComparison.Ordinal));
                tmpUrl = tmpUrl.Replace("\\", "");
                classUrl.UrlM = tmpUrl;
                tmpUrl = strJson.Substring(strJson.IndexOf("http", StringComparison.Ordinal));
                tmpUrl = tmpUrl.Substring(tmpUrl.IndexOf("http", 1, StringComparison.Ordinal));
                tmpUrl = tmpUrl.Substring(tmpUrl.IndexOf("http", 1, StringComparison.Ordinal));
                tmpUrl = tmpUrl.Substring(0, tmpUrl.IndexOf("\"],\"", StringComparison.Ordinal));
                tmpUrl = tmpUrl.Replace("\\", "");
                classUrl.UrlH = tmpUrl;
                return classUrl;
            }          
            return null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            groupBox2.Enabled = false;
            button3.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "")
            {
                MessageBox.Show("下载路径不可为空！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            string path = textBox2.Text.Trim();
            if (!Directory.Exists(path))
            {
                if (DialogResult.Yes ==
                    MessageBox.Show("输入的文件夹【" + path + "】不存在，是否要创建？", "询问", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,"错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            //下载文件开始
            thDownloadFile = new Thread(DownloadFiles);
            thDownloadFile.Start();
        }

        private void DownloadFiles()
        {
            Lock(0);
            string path = textBox2.Text;
            string classname = lblClassName.Text;
            classname = classname.Replace("/","-");
            classname = classname.Replace("\\", "-");
            classname = classname.Replace("<", "-");
            classname = classname.Replace(">", "-");
            classname = classname.Replace("?", "-");
            classname = classname.Replace("|", "-");
            classname = classname.Replace("*", "-");
            if (!Directory.Exists(path + "/" + classname))
            {
                Directory.CreateDirectory(path + "/" + classname);
            }
            foreach (Class @class in classes)
            {
                SetProgressValue(0);
                ShowNowFile(@class.Title + ".mp4");
                string filePath = path + "\\" + classname + "\\" + @class.Title + ".mp4.tmp";
                if (File.Exists(filePath.Substring(0, filePath.IndexOf(".tmp"))))
                {
                    continue;
                }
                if (radioButton1.Checked)
                {
                    HttpDownloadFile(@class.Url.UrlL, filePath);
                }
                if (radioButton2.Checked)
                {
                    HttpDownloadFile(@class.Url.UrlM, filePath);
                }
                if (radioButton3.Checked)
                {
                    HttpDownloadFile(@class.Url.UrlH, filePath);
                }
                File.Move(filePath, filePath.Substring(0,filePath.IndexOf(".tmp")));
                //HttpDownloadFile(@class.Url.UrlH, path + "/" + classname + "/" + @class.Title + ".mp4");
            }
            MessageBox.Show("下载完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Lock(1);
            //HttpDownloadFile(classes[0].Url.UrlH, path + "\\" + classname + "\\1.mp4.tmp");
        }

        /// <summary>
        /// Http下载文件
        /// </summary>
        private void HttpDownloadFile(string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            long totalBytes = response.ContentLength; 
            if (totalBytes == 0)
            {
                return;
            }
            long times = totalBytes/1024 + 1;
            SetProgressBarMaxValue(times);
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, bArr.Length);
            ProgressBarStep(2);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, bArr.Length);
                ProgressBarStep(2);
            }
            stream.Close();
            responseStream.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "选择视频下载的文件夹";
            folderBrowserDialog1.SelectedPath = "";
            folderBrowserDialog1.ShowNewFolderButton = true;
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            thGetinfo.Abort();
            thGetinfo = null;
            button1.Text = "查询";
            if (isTimeOut)
            {
                MessageBox.Show("拉取课程章节信息超时！请重试", "抱歉", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            timer1.Enabled = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    }
}
