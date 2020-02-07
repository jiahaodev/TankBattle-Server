/****************************************************
	文件：EventHandler.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/08 0:00   	
	功能：事件处理者
*****************************************************/

public partial class EventHandler
{

    public static void OnDisconnect(ClientState state) {
        if (state.player != null)
        {
            //to  do
            //保存数据、移除玩家socket
        }
    }


    public static void OnTimer()
    {
        CheckPing();
    }


    //Ping检查
    public static void CheckPing()
    {
        //现在的时间戳
        long timeNow = NetManager.GetTimeStamp();
        //遍历，删除
        foreach (ClientState s in NetManager.clients.Values)
        {
            if (timeNow - s.lastPingTime > NetManager.pingInterval * 4)
            {
                Console.WriteLine("Ping Close " + s.socket.RemoteEndPoint.ToString());
                NetManager.Close(s);
                return;
            }
        }
    }


}
