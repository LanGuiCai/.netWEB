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

namespace AsynClient
{
    public partial class Mainform : Form
    {
        private bool isExit = false;
        private delegate void SetListBoxCallBack(string str);
        private SetListBoxCallBack setlistboxcallback;

        private delegate void SetRichTextBoxReceiveCallBack(string str);
        private SetRichTextBoxReceiveCallBack setRichTextBoxReceiveCallBack;

        private TcpClient client;
        private NetworkStream ns;
        private ManualResetEvent allDone = new ManualResetEvent(false);

        public Mainform()
        {
            InitializeComponent();
            setlistboxcallback = new SetListBoxCallBack(SetListBox);
            setRichTextBoxReceiveCallBack = new SetRichTextBoxReceiveCallBack(SetRichTextBoxReceive);
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //声明客户端对象
            client = new TcpClient(AddressFamily.InterNetwork);
            IPAddress[] serverIP = Dns.GetHostAddresses(Dns.GetHostName());

            //采用异步方式
            AsyncCallback requestCallBack = new AsyncCallback(RequestCallBack);
            allDone.Reset();

            //开始连接
            client.BeginConnect(serverIP[2], 51888, requestCallBack, client);
            lbxStatus.Invoke(setlistboxcallback, string.Format("本机终结点：{0}", client.Client.LocalEndPoint));
            lbxStatus.Invoke(setlistboxcallback, "开始与服务器连接");
            allDone.WaitOne();
        }
        private void RequestCallBack(IAsyncResult iar)
        {
            allDone.Set();//允许一个或多个线程执行
            try
            {
                client = (TcpClient)iar.AsyncState;
                client.EndConnect(iar);
                lbxStatus.Invoke(setlistboxcallback, string.Format("与服务器{0}连接成功", client.Client.RemoteEndPoint));
                ns = client.GetStream();
                DataRead dataRead = new DataRead(ns, client.ReceiveBufferSize);
                ns.BeginRead(dataRead.msg, 0, dataRead.msg.Length, ReadCallBack, dataRead);
            }
            catch(Exception e)
            {
                lbxStatus.Invoke(setlistboxcallback, e.Message);
                return;
            }
        }

        private void ReadCallBack(IAsyncResult iar)
        {
            try
            {
                DataRead dataRead = (DataRead)iar.AsyncState;
                int recv = dataRead.ns.EndRead(iar);//处理异步读取结束
                //在文本框里设置得到的数据
                rtbxReceive.Invoke(setRichTextBoxReceiveCallBack, Encoding.UTF8.GetString(dataRead.msg, 0, recv));
                if(isExit==false)
                {
                    dataRead = new DataRead(ns, client.ReceiveBufferSize);
                    ns.BeginRead(dataRead.msg, 0, dataRead.msg.Length, ReadCallBack, dataRead);
                }
            }
            catch(Exception e)
            {
                lbxStatus.Invoke(setlistboxcallback, e.Message);
            }
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            string str = rtbxSend.Text;
            try
            {
                byte[] bytedata = Encoding.UTF8.GetBytes(str + "\r\n");
                ns.BeginWrite(bytedata, 0, bytedata.Length, new AsyncCallback(SendCallBack), ns);
                //强制清空ns缓冲区，发送数据
                ns.Flush();
            }
            catch(Exception err)
            {
                lbxStatus.Items.Add(err.Message);
            }
        }
        private void SendCallBack(IAsyncResult iar)
        {
            try
            {
                ns.EndWrite(iar);
            }
            catch(Exception e)
            {
                lbxStatus.Invoke(setlistboxcallback, e.Message);
            }
        }

        /// <summary>
        /// 将字符串加到列表框中，并且选中最后一条记录
        /// </summary>
        /// <param name="str"></param>
        private void SetListBox(string str)
        {
            lbxStatus.Items.Add(str);
            lbxStatus.SelectedIndex = lbxStatus.Items.Count - 1;
            lbxStatus.ClearSelected();
        }
        /// <summary>
        /// 负责在信息列表中增加输出信息
        /// </summary>
        /// <param name="str"></param>
        private void SetRichTextBoxReceive(string str)
        {
            rtbxReceive.AppendText(str);
        }
    }

    //声明网络流对象和字节数组
    public class DataRead
    {
        public NetworkStream ns;
        public byte[] msg;
        public DataRead(NetworkStream ns,int buffersize)
        {
            this.ns = ns;
            msg = new byte[buffersize];
        }
    }
}
