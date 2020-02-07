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

    //临时数据，如：坐标
    public int x;
    public int y;
    public int z;
    //数据库数据
    public PlayerData data;

    //发送消息
    public void Send(MsgBase msgBase) {
        NetManager.Send(state,msgBase);
    }

}

