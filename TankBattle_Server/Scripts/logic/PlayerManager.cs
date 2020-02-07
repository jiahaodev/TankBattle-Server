/****************************************************
	文件：PlayerManager.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/08 0:21   	
	功能：玩家管理者
*****************************************************/

using System;
using System.Collections.Generic;

public class PlayerManager
{
    //玩家列表
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    //玩家是否在线
    public static bool IsOnline(string id) {
        return players.ContainsKey(id);
    }

    //获取玩家
    public static Player GetPlayer(string id) {
        return players[id];
    }

    //添加玩家
    public static void AddPlayer(string id,Player player) {
        players.Add(id,player);
    }

    //删除玩家
    public static void RemovePlayer(string id) {
        players.Remove(id);
    }

}

