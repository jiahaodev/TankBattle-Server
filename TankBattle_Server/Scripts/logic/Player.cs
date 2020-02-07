/****************************************************
	文件：Player.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/07 22:48   	
	功能：玩家类
*****************************************************/
using System;

public class Player
{
    public string id = "";

    public ClientState state; 

    public Player(ClientState state) {
        this.state = state;
    }

    //坐标
    public int x;
    public int y;
    public int z;
    //在哪个房间
    public int roomId = -1;
    //阵营
    public int camp = 1;
    //坦克生命值
    public int hp = 100;

    //数据库数据
    public PlayerData data;

    //发送消息
    public void Send(MsgBase msgBase) {
        NetManager.Send(state,msgBase);
    }

}

