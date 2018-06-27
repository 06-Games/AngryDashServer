using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MyMsgBase : MessageBase
{
    public string map;
};

public class _NetworkManager : NetworkBehaviour
{

    public Terminal terminal;

	public void StartServer()
    {
        GetComponent<NetworkManager>().StartHost();
        player = 0;

        LoadMap();
    }

    public void LoadMap()
    {
        if (ConfigAPI.GetInt("players.limit") < 1)
            ConfigAPI.SetInt("players.limit", GetComponent<NetworkManager>().maxConnections = 50);
        GetComponent<NetworkManager>().maxConnections = ConfigAPI.GetInt("players.limit");

        m_Message = new MyMsgBase();

        string path = Application.dataPath;
#if UNITY_EDITOR
        path = "C:\\Users\\evan\\Documents\\Unity\\Compiller\\Angry Dash Server\\1.0";
#elif UNITY_STANDALONE
        string[] Path = Application.dataPath.Split(new string[2] { "/", "\\" }, System.StringSplitOptions.None);
        path = Application.dataPath.Replace(Path[Path.Length - 1]+"/", "");
#endif
        if (!File.Exists(path + "map.level"))
            File.WriteAllText(path + "map.level", "Blocks {\n1.0; (0.0, 0.0, 0.0); 0; FF0000255; 0\n1.0; (0.0, 5.0, 0.0); 0; FF0000255; 0\n}");

        m_Message.map = File.ReadAllText(path + "map.level");
    }

    public void Disconnect()
    {
        GetComponent<NetworkManager>().StopHost();
        Base.Quit();
    }

    public const short MessageType = MsgType.Highest + 1;
    public MyMsgBase m_Message;

    public int player = 0;
    void Update()
    {
        if(GetComponent<NetworkManager>().numPlayers != player)
        {
            NetworkServer.SendToAll(MessageType, m_Message);

            if(player < GetComponent<NetworkManager>().numPlayers)
                terminal.Message("A player join the game");
            else terminal.Message("A player quit the game");

            player = GetComponent<NetworkManager>().numPlayers;
        }
    }
}
