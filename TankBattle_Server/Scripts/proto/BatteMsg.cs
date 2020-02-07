/****************************************************
	文件：BatteMsg.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/08 0:30   	
	功能：战斗相关消息
*****************************************************/

public class MsgMove : MsgBase
{
    public MsgMove() { protoName = "MsgMove"; }
    //坐标
    public int x = 0;
    public int y = 0;
    public int z = 0;
}


public class MsgAttack : MsgBase
{
    public MsgAttack() { protoName = "MsgAttack"; }

    public string desc = "127.0.0.1:6543";
}

