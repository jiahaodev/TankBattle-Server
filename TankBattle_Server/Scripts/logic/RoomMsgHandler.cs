/****************************************************
	文件：RoomMsgHandler.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/08 1:24   	
	功能：游戏房间相关消息处理
*****************************************************/
using System;

public partial class MsgHandler {

    //查询战绩
    public static void MsgGetAchieve(ClientState state, MsgBase msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        Player player = state.player;
        if (player == null) return;

        msg.win = player.data.win;
        msg.lost = player.data.lost;

        player.Send(msg);
    }


    //请求房间列表
    public static void MsgGetRoomList(ClientState state, MsgBase msgBase)
    {
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;
        Player player = state.player;
        if (player == null) return;

        player.Send(RoomManager.ToMsg());
    }

    //创建房间
    public static void MsgCreateRoom(ClientState state, MsgBase msgBase)
    {
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        Player player = state.player;
        if (player == null) return;
        //已经在房间里
        if (player.roomId >= 0)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        //创建
        Room room = RoomManager.AddRoom();
        room.AddPlayer(player.id);

        msg.result = 0;
        player.Send(msg);
    }

    //进入房间
    public static void MsgEnterRoom(ClientState state, MsgBase msgBase)
    {
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        Player player = state.player;
        if (player == null) return;
        //已经在房间里
        if (player.roomId >= 0)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        //获取房间
        Room room = RoomManager.GetRoom(msg.id);
        if (room == null)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        //进入
        if (!room.AddPlayer(player.id))
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        //返回协议	
        msg.result = 0;
        player.Send(msg);
    }


    //获取房间信息
    public static void MsgGetRoomInfo(ClientState state, MsgBase msgBase)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        Player player = state.player;
        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            player.Send(msg);
            return;
        }

        player.Send(room.ToMsg());
    }

    //离开房间
    public static void MsgLeaveRoom(ClientState state, MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        Player player = state.player;
        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }

        room.RemovePlayer(player.id);
        //返回协议
        msg.result = 0;
        player.Send(msg);
    }

}
