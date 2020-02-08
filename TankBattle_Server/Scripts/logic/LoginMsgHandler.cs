/****************************************************
	文件：LoginMsgHandler.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/08 0:33   	
	功能：登陆消息处理
*****************************************************/

public partial class MsgHandler
{
    //注册协议处理
    public static void MsgRegister(ClientState state,MsgBase msgBase) {
        MsgRegister msg = (MsgRegister)msgBase;
        if (DbManager.Register(msg.id,msg.pw))
        {
            DbManager.CreatePlayer(msg.id);
            msg.result = 0;
        }
        else
        {
            msg.result = 1;
        }
        NetManager.Send(state,msg);
    }

    //登陆协议处理
    public static void MsgLogin(ClientState state,MsgBase msgBase) {
        MsgLogin msg = (MsgLogin)msgBase;

        //密码校验
        if (!DbManager.CheckPassword(msg.id,msg.pw))
        {
            msg.result = 1;
            NetManager.Send(state,msg);
        }
        //不允许再次登陆
        if (state.player != null)
        {
            msg.result = 1;
            NetManager.Send(state,msg);
            return;
        }
        //如果已经登陆，踢下线
        if (PlayerManager.IsOnline(msg.id))
        {
            Player other = PlayerManager.GetPlayer(msg.id);
            MsgKick msgKick = new MsgKick();
            msgKick.reason = 0;
            other.Send(msgKick);
            //断开连接
            NetManager.Close(other.state);
        }
        //获取玩家数据
        PlayerData playerData = DbManager.GetPlayerData(msg.id);
        if (playerData == null)
        {
            msg.result = 1;
            NetManager.Send(state, msg);
            return;
        }
        //构建Player
        Player player = new Player(state);
        player.id = msg.id;
        player.data = playerData;
        PlayerManager.AddPlayer(msg.id, player);
        state.player = player;
        //返回协议
        msg.result = 0;
        player.Send(msg);
    }

}

