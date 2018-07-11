using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MyMsgBase : MessageBase
{
    public string map;
};
public class ServInfo : MessageBase
{
    public byte[] icon;
    public string Name;
    public int player;
    public int maxPlayer;
};
public class MsgID
{
    public const short AskForServerInfo = 500;
    public const short SendServerInfo = 501;
    public const short AskForServerMap = 502;
    public const short SendServerMap = 503;
}

public class _NetworkManager : NetworkBehaviour
{

    public Terminal terminal;
    public Sprite DefaultIcon;
    public TextAsset 
        DefaultMap;

	public void StartServer()
    {
        LoadMap();

        //GetComponent<NetworkManager>().StartHost();
        NetworkServer.RegisterHandler(MsgID.AskForServerInfo, RequestInfo);
        NetworkServer.RegisterHandler(MsgID.AskForServerMap, MapRequestReceive);
        player = 0;
    }
    void RequestInfo(NetworkMessage netMsg)
    {
        ServInfo info = new ServInfo();
#if UNITY_EDITOR
        string path = "C:\\Users\\evan\\Documents\\Unity\\Compiller\\Angry Dash Server\\"+Application.version+"\\";
#elif UNITY_STANDALONE
        string[] Path = Application.dataPath.Split(new string[2] { "/", "\\" }, System.StringSplitOptions.None);
        string path = Application.dataPath.Replace(Path[Path.Length - 1], "");
#endif
        if (string.IsNullOrEmpty(ConfigAPI.GetString("server.icon")))
            ConfigAPI.SetString("server.icon", "icon.png");
        if (!File.Exists(path + ConfigAPI.GetString("server.icon")))
            File.WriteAllBytes(path + ConfigAPI.GetString("server.icon"), DefaultIcon.texture.EncodeToPNG());
        info.icon = File.ReadAllBytes(path + ConfigAPI.GetString("server.icon"));
        info.maxPlayer = ConfigAPI.GetInt("players.limit");
        info.player = player;
        if (string.IsNullOrEmpty(ConfigAPI.GetString("server.name")))
            ConfigAPI.SetString("server.name", "Angry Dash Server");
        info.Name = ConfigAPI.GetString("server.name");
        NetworkServer.SendToAll(MsgID.SendServerInfo, info);
    }

    public void LoadMap()
    {
        if (ConfigAPI.GetInt("players.limit") < 1 | !ConfigAPI.ParamExist("players.limit"))
            ConfigAPI.SetInt("players.limit", 50);
        GetComponent<NetworkManager>().maxConnections = ConfigAPI.GetInt("players.limit");

        if (ConfigAPI.GetInt("server.port") < 500 | ConfigAPI.GetInt("server.port") > 65535 | !ConfigAPI.ParamExist("server.port"))
            ConfigAPI.SetInt("server.port", 7777);
        if (ConfigAPI.GetInt("server.port") != GetComponent<NetworkManager>().networkPort)
        {
            GetComponent<NetworkManager>().networkPort = ConfigAPI.GetInt("server.port");
            //GetComponent<NetworkManager>().matchPort = ConfigAPI.GetInt("server.port");

            GetComponent<NetworkManager>().StopHost();
            GetComponent<NetworkManager>().StartHost();
        }


        m_Message = new MyMsgBase();

        string path = Application.dataPath;
#if UNITY_EDITOR
        path = "C:\\Users\\evan\\Documents\\Unity\\Compiller\\Angry Dash Server\\"+Application.version+"\\";
#elif UNITY_STANDALONE
        string[] Path = Application.dataPath.Split(new string[2] { "/", "\\" }, System.StringSplitOptions.None);
        path = Application.dataPath.Replace(Path[Path.Length - 1], "");
#endif
        if (!File.Exists(path + "map.level"))
            File.WriteAllText(path + "map.level", DefaultMap.text);

        m_Message.map = File.ReadAllText(path + "map.level");

        if (!ConfigAPI.ParamExist("map.reloadForPlayers"))
            ConfigAPI.SetBool("map.reloadForPlayers", true);
        if (ConfigAPI.GetBool("map.reloadForPlayers"))
            NetworkServer.SendToAll(MsgID.SendServerMap, m_Message);
    }

    public void Disconnect()
    {
        GetComponent<NetworkManager>().StopHost();
        Base.Quit();
    }
    
    public MyMsgBase m_Message;

    public int player = 0;
    void MapRequestReceive(NetworkMessage netMsg)
    {
        NetworkServer.SendToAll(MsgID.SendServerMap, m_Message);
        player = player + 1;
        terminal.Message("A player join the game");
    }

    private void Update()
    {
        if (player > GetComponent<NetworkManager>().numPlayers)
        {
            player = player - 1;
            terminal.Message("A player quit the game");
        }
    }
}
