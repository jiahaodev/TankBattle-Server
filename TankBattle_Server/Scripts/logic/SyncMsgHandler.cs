/****************************************************
	文件：SyncMsgHandler.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/08 14:26   	
	功能：（战场）同步消息处理
*****************************************************/
using System;

public partial class MsgHandler
{

    //同步位置协议
    public static void MsgSyncTank(ClientState state, MsgBase msgBase)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBase;
        Player player = state.player;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            return;
        }
        if (room.status != Room.Status.FIGHT)
        {
            return;
        }
        //防作弊检测
        if (Math.Abs(player.x - msg.x) > 5
            || Math.Abs(player.y - msg.y) > 5
            || Math.Abs(player.z - msg.z) > 5
            )
        {
            Console.WriteLine("疑似作弊 " + player.id);
        }

        //更新消息
        player.x = msg.x;
        player.y = msg.y;
        player.z = msg.z;
        player.ex = msg.ex;
        player.ey = msg.ey;
        player.ez = msg.ez;
        //广播
        msg.id = player.id;
        room.Broadcast(msg);
    }


    //开火协议
    public static void MsgFire(ClientState state, MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        Player player = state.player;
        if (player == null)
        {
            return;
        }
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            return;
        }
        if (room.status != Room.Status.FIGHT)
        {
            return;
        }
        //广播
        msg.id = player.id;
        room.Broadcast(msg);
    }


    //击中协议
    public static void MsgHit(ClientState state,MsgBase msgBase) {
        MsgHit msg = (MsgHit)msgBase;
        Player player = state.player;
        if (player == null)
        {
            return;
        }
        Player targetPlayer = PlayerManager.GetPlayer(msg.targetId);
        if (targetPlayer == null)
        {
            return;
        }

        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            return;
        }
        if (room.status != Room.Status.FIGHT)
        {
            return;
        }
        //发送者校验
        if (player.id != msg.id)
        {
            return;
        }
        //更新状态
        int damage = 25;
        targetPlayer.hp -= damage;
        //广播
        msg.id = player.id;
        msg.hp = player.hp;
        msg.damage = damage;
        room.Broadcast(msg);
    }

}

