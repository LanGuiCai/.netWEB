using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AsynServer
{
    public partial class MainForm : Form
    {
        private bool isExit = false;
        System.Collections.ArrayList clientlist = new System.Collections.ArrayList();

        TcpListener listetner;
        private delegate void SetListBoxCallBack(string str);
        private SetListBoxCallBack setlistboxcallback;

        private delegate void SetRichTextBoxCallBack(string str);
        private SetRichTextBoxCallBack setrichtextboxcallback;

        private delegate void SetComboxCallback(string str);
        private SetComboxCallback setcomboxcallback;

        private delegate void RemoveComboxCallBack(DataReadWrite datareadwrite);
        private RemoveComboxCallBack removecomboxcallback;

        private ManualResetEvent allDone = new ManualResetEvent(false);

        public MainForm()
        {
            InitializeComponent();
            setlistboxcallback = new SetListBoxCallBack(SetListBox);
            setrichtextboxcallback = new SetRichTextBoxCallBack(SetReceiveText);
            setcomboxcallback = new SetComboxCallback(SetComboBox);
            removecomboxcallback = new RemoveComboxCallBack(RemoveComboxItems);
        }
        //开始监听
        private void btnStart_Click(object sender, EventArgs e)
        {
            Thread myThread = new Thread(new ThreadStart(AcceptConnection));
            myThread.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }
        private void AcceptConnection()
        {
            //根据DNS协议解析IP地址
            IPAddress[] ip = Dns.GetHostAddresses(Dns.GetHostName());
            listetner = new TcpListener(ip[2], 51888);
            listetner.Start();
            while(isExit==false)
            {
                try
                {
                    allDone.Reset();
                    AsyncCallback callback = new AsyncCallback(AcceptTcpClientCallBack);
                    lbxStatus.Invoke(setlistboxcallback, "开始等待连接");
                    listetner.BeginAcceptTcpClient(callback, listetner);
                    allDone.WaitOne();
                }
                catch(Exception e)
                {
                    lbxStatus.Invoke(setlistboxcallback, e.Message);
                    break;
                }
            }
        }
        private void AcceptTcpClientCallBack(IAsyncResult iar)
        {
            try
            {
                allDone.Set();
                TcpListener mylistener = (TcpListener)iar.AsyncState;
                TcpClient client = mylistener.EndAcceptTcpClient(iar);//异步传入连接
                lbxStatus.Invoke(setlistboxcallback, "已经接收客户连接：" + client.Client.RemoteEndPoint);

                cbxSelectClient.Invoke(setcomboxcallback, client.Client.RemoteEndPoint.ToString());
                DataReadWrite datareadwrite = new DataReadWrite(client);
                clientlist.Add(datareadwrite);

                SendString(datareadwrite, "服务器已连接");
                datareadwrite.ns.BeginRead(datareadwrite.read, 0, datareadwrite.read.Length, ReadCallBack, datareadwrite);
            }
            catch(Exception e)
            {
                lbxStatus.Invoke(setlistboxcallback, e.Message);
            }
        }
        /// <summary>
        /// 数据发送
        /// </summary>
        /// <param name="datarearwrite"></param>
        /// <param name="str"></param>
        private void SendString(DataReadWrite datarearwrite,string str)
        {
            try
            {
                datarearwrite.write = Encoding.UTF8.GetBytes(str + "\r\n");
                datarearwrite.ns.BeginWrite(datarearwrite.write, 0, datarearwrite.write.Length, new AsyncCallback(SendCallBack), datarearwrite);
                datarearwrite.ns.Flush();

                lbxStatus.Invoke(setlistboxcallback, string.Format("向{0}发送：{1}", datarearwrite.client.Client.RemoteEndPoint, str));
            }
            catch(Exception e)
            {
                lbxStatus.Invoke(setlistboxcallback, e.Message);
            }
        }
        /// <summary>
        /// 数据读取
        /// </summary>
        /// <param name="iar"></param>
        private void ReadCallBack(IAsyncResult iar)
        {
            try
            {
                DataReadWrite datareadwrite = (DataReadWrite)iar.AsyncState;
                int recv = datareadwrite.ns.EndRead(iar);
                rtbxReceive.Invoke(setrichtextboxcallback, string.Format("来自{0}:{1}",
                    datareadwrite.client.Client.RemoteEndPoint, Encoding.UTF8.GetString(datareadwrite.read, 0, recv)));
                if(isExit==false)
                {
                    datareadwrite.InitReadArray();
                    datareadwrite.ns.BeginRead(datareadwrite.read, 0, datareadwrite.read.Length,
                        ReadCallBack, datareadwrite);
                }
            }
            catch(Exception e)
            {
                lbxStatus.Invoke(setlistboxcallback, e.Message);
            }
        }
        /// <summary>
        /// 处理断开连接的回调函数
        /// </summary>
        /// <param name="iar"></param>
        private void SendCallBack(IAsyncResult iar)
        {
            DataReadWrite datareadwrite = (DataReadWrite)iar.AsyncState;
            try
            {
                datareadwrite.ns.EndWrite(iar);
            }
            catch(Exception e)
            {
                lbxStatus.Invoke(setlistboxcallback, e.Message);
                cbxSelectClient.Invoke(removecomboxcallback, datareadwrite);
            }
        }
        /// <summary>
        /// 移除列表操作
        /// </summary>
        /// <param name="datareadwrite"></param>
        private void RemoveComboxItems(DataReadWrite datareadwrite)
        {
            int index = clientlist.IndexOf(datareadwrite);
            cbxSelectClient.Items.RemoveAt(index);
        }
        /// <summary>
        /// 设置状态信息
        /// </summary>
        /// <param name="str"></param>
        private void SetListBox(string str)
        {
            lbxStatus.Items.Add(str);
            lbxStatus.SelectedIndex = lbxStatus.Items.Count - 1;
            lbxStatus.ClearSelected();
        }
        /// <summary>
        /// 显示获取消息
        /// </summary>
        /// <param name="str"></param>
        private void SetReceiveText(string str)
        {
            rtbxReceive.AppendText(str);
        }
        /// <summary>
        /// 添加选择客户端
        /// </summary>
        /// <param name="obj"></param>
        private void SetComboBox(object obj)
        {
            cbxSelectClient.Items.Add(obj);
        }

        //发送信息
        private void btnSend_Click(object sender, EventArgs e)
        {
            int index = cbxSelectClient.SelectedIndex;
            //异常提示
            if(index==-1)
            {
                MessageBox.Show("请选接收方");
            }
            else
            {
                DataReadWrite obj = (DataReadWrite)clientlist[index];
                SendString(obj, rtbxSend.Text);
                rtbxSend.Clear();
            }
        }
        //断开连接
        private void btnStop_Click(object sender, EventArgs e)
        {
            //设置标志
            isExit = true;

            allDone.Set();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
    }
    /// <summary>
    /// 处理数据
    /// </summary>
    class DataReadWrite
    {
        public TcpClient client;
        public NetworkStream ns;
        public byte[] read;
        public byte[] write;
        public DataReadWrite(TcpClient client)
        {
            this.client = client;
            ns = client.GetStream();
            read = new byte[client.ReceiveBufferSize];
            write = new byte[client.SendBufferSize];
        }

        public void InitReadArray()
        {
            read = new byte[client.ReceiveBufferSize];
        }
        public void InitWriteArray()
        {
            write = new byte[client.SendBufferSize];
        }
    }
}
