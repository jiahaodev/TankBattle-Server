/****************************************************
	文件：ClientState.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/07 22:09   	
	功能：客户端连接状态
*****************************************************/
using System.Net.Sockets;


public class ClientState
{
    public Socket socket;
    public ByteArray readBuffer = new ByteArray();
    public long lastPingTime = 0;
    //public Player player;
}

