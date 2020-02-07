/****************************************************
	文件：DbManager.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/02/07 20:38   	
	功能：数据库操作管理者
*****************************************************/

using System;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;


public class DbManager
{

    public static MySqlConnection mysql;
    private static JavaScriptSerializer Js = new JavaScriptSerializer();


    /// <summary>
    /// 连接数据库
    /// </summary>
    /// <param name="db"></param>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="user"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public static bool Connect(string db, string ip, int port, string user, string pw)
    {
        //创建MySqlConnection对象
        mysql = new MySqlConnection();
        //连接参数
        string s = string.Format(
            "Database={0};Data Source={1};port={2};User Id={3};Password={4}",
            db, ip, port, user, pw);
        mysql.ConnectionString = s;
        //连接
        try
        {
            mysql.Open();
            Console.WriteLine("[数据库]connect succ!");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库]connect fail, " + e.Message);
            return false;
        }
    }

    //判定安全字符串
    //防止SQL注入
    private static bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }


    #region account表相关操作
    /// <summary>
    /// 用户id是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsAccountExist(string id)
    {
        //防sql注入
        if (!DbManager.IsSafeString(id))
        {
            return false;
        }
        //sql语句
        string sql = string.Format("select * from account where id='{0}';", id);
        //查询
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] IsSafeString err, " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 注释账户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public static bool Register(string id, string pw)
    {
        //防sql注入
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[数据库] Register fail, id not safe");
            return false;
        }
        if (!DbManager.IsSafeString(pw))
        {
            Console.WriteLine("[数据库] Register fail, pw not safe");
            return false;
        }
        //用户id是否已经存在
        if (IsAccountExist(id))
        {
            Console.WriteLine("[数据库] Register fail, id exist");
            return false;
        }
        //写入数据库User表
        string sql = string.Format("insert into account set id ='{0}' ,pw ='{1}';", id, pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] Register fail " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 检测用户名与密码是否匹配
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public static bool CheckPassword(string id, string pw)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[数据库] CheckPassword fail, id not safe");
            return false;
        }
        if (!DbManager.IsSafeString(pw))
        {
            Console.WriteLine("[数据库] CheckPassword fail, pw not safe");
            return false;
        }
        string sql = string.Format("select * from account where id = '{0}' and pw='{1}';", id, pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] CheckPassword err, " + e.Message);
            return false;
        }
    }
    #endregion


    #region Player表相关操作
    /// <summary>
    /// 创建Player账户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool CreatePlayer(string id)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[数据库] CreatePlayer fail, id not safe");
            return false;
        }
        PlayerData playerData = new PlayerData();
        string data = Js.Serialize(playerData);
        string sql = string.Format("insert into player set id ='{0}',data='{1}';", id, data);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] CreatePlayer err, " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 获取玩家数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static PlayerData GetPlayerData(string id)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[数据库] GetPlayerData fail, id not safe");
            return null;
        }

        string sql = string.Format("select * from player where id ='{0}';", id);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return null;
            }
            dataReader.Read();
            string data = dataReader.GetString("data");
            //反序列化
            PlayerData playerData = Js.Deserialize<PlayerData>(data);
            dataReader.Close();
            return playerData;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] GetPlayerData fail, " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// 更新玩家数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public static bool UpdatePlayerData(string id, PlayerData playerData)
    {
        string data = Js.Serialize(playerData);

        string sql = string.Format("update player set data='{0}' where id ='{1}';", data, id);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] UpdatePlayerData err, " + e.Message);
            return false;
        }
    }
    #endregion

}