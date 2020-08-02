using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class main2 : MonoBehaviour
{
    private string data = "";
    private string auth_token = "";
    private bool onguistart = false;
    private Vector2 scrollPosition = Vector2.zero;
    

    private List<Texture2D> list_img;
    private List<string> list_names;
    private root root_class;

    private Task taskWebConnect;

    bool temp = false;
    // Start is called before the first frame update
    void Start()
    {
        list_img = new List<Texture2D>();
        list_names = new List<string>();
        auth_token = PlayerPrefs.GetString("auth_token","");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void check_data()
    {
        data = "";
        onguistart = false;
        taskWebConnect = Task.Run(() => {
            DoClientWebSocket();
        });

        StartCoroutine(GetTexture()); 
       
    }
    
    IEnumerator GetTexture() 
    {
        UnityWebRequest www; 
        yield return new WaitUntil(() => data != "");

        root_class = root.CreateFromJSON(data);

        foreach(var user in root_class.users)
        {
            www = UnityWebRequestTexture.GetTexture(user.avatar);
            yield return www.SendWebRequest();
            

            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                list_img.Add(myTexture);
                list_names.Add(user.username);
            }
        }
        onguistart = true;
    }
    void OnGUI()
    {
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = GUI.skin.textField.fontSize= 20;
        if (GUI.Button(new Rect((Screen.width/2)-50, (Screen.height/2)-50, 200, 50),"start"))
        {
            InvokeRepeating("check_data", 0.0f, 10.0f);
        } 
        if (GUI.Button(new Rect((Screen.width/2)-50, (Screen.height/2), 200, 50),"stop"))
        {
            CancelInvoke();
        } 
        if (GUI.Button(new Rect((Screen.width/2)-50, (Screen.height/2)+50, 200, 50),"back to main"))
        {
            CancelInvoke();
            SceneManager.LoadScene("main1");
        } 
        scrollPosition = GUI.BeginScrollView(new Rect(0,0, Screen.width, Screen.height), scrollPosition, new Rect(0, 0, Screen.width, list_img.Count*100));
            float height = 0;
            int count = 0;
            foreach(Texture2D texture2D in list_img)
            {
                GUI.DrawTexture(new Rect(0, height, 100, 100),texture2D);
                GUI.Label(new Rect(100,height+50,100,100),list_names[count]);
                height += 100;
                count += 1;
            }     
        

        GUI.EndScrollView();
    }

    /////////////////////////WEBSOCKET/////////////////////////////////
    private async Task DoClientWebSocket()
    {
    using (ClientWebSocket ws = new ClientWebSocket())
    {
        Uri serverUri = new Uri("ws://167.71.14.193:8080/");

        var source = new CancellationTokenSource();
        source.CancelAfter(5000);

        await ws.ConnectAsync(serverUri, source.Token);
        var count = 0;

        while (ws.State == WebSocketState.Open && count <= 2)
        { 
            count += 1;
            string msg ="";
            if(count == 1){
                msg = "{\"cmd\": \"auth\", \"token\":\""+auth_token+"\"}";
          
            }
            else if (count == 2)
            {
                msg = "{\"cmd\": \"getusers\"}";
            }
            ArraySegment<byte> bytesToSend =
                      new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
             await ws.SendAsync(bytesToSend, WebSocketMessageType.Text,
                               true, source.Token);

             var receiveBuffer = new byte[1024];

            var offset = 0;
            var dataPerPacket = 10; 
            while (true)
            {
              ArraySegment<byte> bytesReceived = new ArraySegment<byte>(receiveBuffer, offset, dataPerPacket);
              WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived,source.Token);
              
              offset += result.Count;
              if (result.EndOfMessage)
                    break;
            }
            Debug.Log("Complete response: "+count.ToString()+
                                Encoding.UTF8.GetString(receiveBuffer, 0,
                                                            offset));
            if(count == 2)
            {
                data = Encoding.UTF8.GetString(receiveBuffer, 0, offset);
            }
            }
        }
    }
    /////////////////////////WEBSOCKET/////////////////////////////////
}







[System.Serializable]
public class root    {
    public bool status;
    public List<users> users;

    public static root CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<root>(jsonString);
    }
}
[System.Serializable]
public class users    {
    public string username;
    public string avatar;
}