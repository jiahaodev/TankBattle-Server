/****************************************************
	文件：RoomManager.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/08 1:23   	
	功能：游戏房间管理者
*****************************************************/
using System;
using System.Collections.Generic;


public class RoomManager
{
    //最大id（当前最大的房间id）
    private static int maxId = 0;
    //房间列表
    public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();

    //创建房间
    public static Room AddRoom()
    {
        maxId++;
        Room room = new Room();
        room.id = maxId;
        rooms.Add(room.id, room);
        return room;
    }

    //删除房间
    public static bool RemoveRoom(int id)
    {
        rooms.Remove(id);
        return true;
    }

    //获取房间
    public static Room GetRoom(int id)
    {
        if (rooms.ContainsKey(id))
        {
            return rooms[id];
        }
        return null;
    }

    //生成MsgGetRoomList协议
    public static MsgBase ToMsg()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        int count = rooms.Count;
        msg.rooms = new RoomInfo[count];
        //rooms
        int i = 0;
        foreach (Room room in rooms.Values)
        {
            RoomInfo roomInfo = new RoomInfo();
            //赋值
            roomInfo.id = room.id;
            roomInfo.count = room.playerIds.Count;
            roomInfo.status = (int)room.status;

            msg.rooms[i] = roomInfo;
            i++;
        }
        return msg;
    }
}

