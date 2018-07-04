using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class Terminal : MonoBehaviour {

    public InputField IF;
    public Text txt;
    public _NetworkManager Net;
    public GameObject textObject;
    GameObject Viewport;
    public Color32[] textColor;

    List<Messages> MessagesList = new List<Messages>();

    void Start()
    {
        Viewport = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        ConfigAPI.Reload();
        Net.StartServer();
        Message("Server start");
    }

    public void CommandEnter()
    {
        if (string.IsNullOrEmpty(IF.text))
            Message("Please enter a command", 1);
        else
        {
            string cmd = IF.text.Split(new string[1] { " " }, System.StringSplitOptions.None)[0].ToLower();
            string[] args = IF.text.Replace(cmd + " ", "").Split(new string[1] { " " }, System.StringSplitOptions.None);

            if (cmd == "stop")
            {
                Message("Stopping server ...");
                Net.Disconnect();
                Message("The server stopped");
            }
            else if (cmd == "color")
            {
                bool done = false;
                Color32[] newColor = new Color32[textColor.Length];
                try
                {
                    for(int i = 0; i < args.Length; i++)
                        newColor[i] = Base.HexToColor(args[i]);
                    done = true;
                }
                catch { Message("Syntax error, all data need to be hex : color {main} {info} {error}", 2); }

                if(done)
                {
                    textColor = newColor;
                    for(int i = 0; i < MessagesList.Count; i++)
                    {
                        MessagesList[i].textObject.color = textColor[MessagesList[i].type];
                    }
                    Message("The colors have been changed");
                }
            }
            else if (cmd == "reload")
            {
                ConfigAPI.Reload();
                Message("The config file has been reload");
                Net.LoadMap();
                Message("The map has been reload");
            }
            else if (cmd == "info")
            {
                if(Net.player > 1)
                    Message(Net.player + " players connected", 1);
                else Message(Net.player + " player connected", 1);
                Message("", 1);
                Message("Angry Dash Server v" + Application.version, 1);
                Message("Coded by EvanG", 1);
                Message("©"+ DateTime.Now.Year+  " 06Games. All rights reserved.", 1);
            }
            else Message(IF.text + " : unkown command", 1);
        }

        IF.text = "";
    }

    public void Message(string msg, int type = 0)
    {
        string DT = "";
        if (type == 0)
            DT = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";

        if (type == 0)
            Log.LogNewMessage(msg, true);

        GameObject newText = Instantiate(textObject, Viewport.transform);
        Messages newMessages = new Messages();
        newMessages.text = DT + msg;
        newMessages.textObject = newText.GetComponent<Text>();
        newMessages.type = type;

        if (MessagesList.Count > 100)
        {
            Destroy(MessagesList[0].textObject.gameObject);
            MessagesList.Remove(MessagesList[0]);
        }

        newMessages.textObject.text = newMessages.text;
        newMessages.textObject.color = textColor[type];
        MessagesList.Add(newMessages);
        transform.GetChild(0).GetComponent<ScrollRect>().verticalScrollbar.value = 0;
    }

}

[Serializable]
public class Messages
{
    public string text;
    public int type;
    public Text textObject;
}
