﻿/****************************************************
	文件：NetManager.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/07 22:06   	
	功能：网络通信管理者
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public class NetManager
{
    //监听socket
    public static Socket listenfd;
    //客户端socket  及  状态消息
    public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
    //Select的检查表
    private static List<Socket> checkRead = new List<Socket>();
    //ping间隔
    public static long pingInterval = 30;

    /// <summary>
    /// 启动网络监听
    /// </summary>
    /// <param name="listenPort"></param>
    public static void StartNetLoop(int listenPort)
    {
        //启动socket端口监听
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAdr = IPAddress.Parse("0.0.0.0");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, listenPort);
        listenfd.Listen(0);
        Console.WriteLine("[服务器]启动成功");
        //循环读取
        while (true)
        {
            ResetCheckRead();
            Socket.Select(checkRead, null, null, 1000);
            //检查可读对象
            for (int i = checkRead.Count - 1; i >= 0; i--)
            {
                Socket s = checkRead[i];
                if (s == listenfd)
                {
                    ReadListenfd(s);
                }
                else
                {
                    ReadClientfd(s);
                }
            }
            //定时消息
            Timer();
        }
    }


    //填充checkRead列表
    private static void ResetCheckRead()
    {
        checkRead.Clear();
        checkRead.Add(listenfd);
        foreach (ClientState state in clients.Values)
        {
            checkRead.Add(state.socket);
        }
    }


    //读取Listenfd
    private static void ReadListenfd(Socket listenfd)
    {
        try
        {
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            state.lastPingTime = GetTimeStamp();
            clients.Add(clientfd, state);
            Console.WriteLine("Accept " + clientfd.RemoteEndPoint.ToString());
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Accept fail" + ex.ToString());
        }
    }


    //读取Clientfd
    private static void ReadClientfd(Socket clientfd)
    {
        ClientState state = clients[clientfd];
        ByteArray readBuff = state.readBuffer;
        //接收
        int count = 0;
        //缓冲区不够，清除，若依旧不够，只能返回
        //缓冲区长度只有1024，单条协议超过缓冲区长度时会发生错误，根据需要调整长度
        if (readBuff.remain <= 0)
        {
            OnReceiveData(state);
            readBuff.MoveBytes();
        }
        if (readBuff.remain <= 0)
        {
            Console.WriteLine("Receive fail , maybe msg length > buff capacity");
            Close(state);
            return;
        }

        try
        {
            count = clientfd.Receive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0);
        }
        catch (Exception e)
        {
            Console.WriteLine("Receive SocketException " + e.ToString());
            Close(state);
            return;
        }

        //客户端关闭
        if (count <= 0)
        {
            Console.WriteLine("Socket Close " + clientfd.RemoteEndPoint.ToString());
            Close(state);
            return;
        }

        //消息处理
        readBuff.writeIdx += count;
        //处理二进制消息
        OnReceiveData(state);
        //移动缓冲区
        readBuff.CheckAndMoveBytes();
    }


    //数据处理
    private static void OnReceiveData(ClientState state)
    {
        ByteArray readBuff = state.readBuffer;
        if (readBuff.length <= 2)
        {
            return;
        }
        Int16 bodyLength = readBuff.ReadInt16();
        if (readBuff.length < bodyLength)
        {
            return; //消息不完整
        }
        //解析协议名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "")
        {
            Console.WriteLine("OnReceiveData MsgBase.DecodeName fail");
            Close(state);
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();

        //分发消息
        MethodInfo mi = typeof(MsgHandler).GetMethod(protoName);
        object[] o = {state,msgBase};
        if (mi != null)
        {
            mi.Invoke(null,o);
        }
        else
        {
            Console.WriteLine("OnReceiveData Invoke fail " + protoName);
        }

        //继续读取消息
        if (readBuff.length > 2)
        {
            OnReceiveData(state);
        }
    }


    //发送消息
    public static void Send(ClientState state, MsgBase msg)
    {
        if (state == null)
        {
            return;
        }
        if (!state.socket.Connected)
        {
            return;
        }
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
        try
        {
            state.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("Socket Close on BeginSend" + e.ToString());
        }
    }


    //关闭连接
    private static void Close(ClientState state)
    {
        //连接关闭通知
        MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
        object[] ob = { state };
        mei.Invoke(null, ob);

        //关闭
        state.socket.Close();
        clients.Remove(state.socket);
    }

    //定时器任务
    private static void Timer()
    {
        //消息分发
        MethodInfo mei = typeof(EventHandler).GetMethod("OnTimer");
        object[] ob = { };
        mei.Invoke(null, ob);
    }

    //获取时间戳
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}

