/****************************************************
	文件：SysMsgHandler.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/07 23:53   	
	功能：消息处理者
*****************************************************/

public partial class MsgHandler
{

    public static void MsgPing(ClientState state,MsgBase magBase) {
        state.lastPingTime = NetManager.GetTimeStamp();
        MsgPong msgPong = new MsgPong();
        NetManager.Send(state,msgPong);
    }
}

