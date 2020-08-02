using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class login : MonoBehaviour
{
    private string results =" ";

    private string status = "";

    private string username = "";
    private string password = "";

    private login_token user;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("auth_token","");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator get_auth_token(string url) {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log("ERR");
        }
        else {
            results = www.downloadHandler.text;
            Debug.Log(results);
            
            user = login_token.CreateFromJSON(results);
            Debug.Log(user.token);
            if(user.status)
            {
                Debug.Log("login success");
                PlayerPrefs.SetString("auth_token",user.token);
                SceneManager.LoadScene("main2");
            }
        }
    }
    private void OnGUI() {
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize =GUI.skin.textField.fontSize= 20;
        username = GUI.TextField(new Rect((Screen.width/2)-200, (Screen.height/2)-125, 400, 100), username, 25);
        password = GUI.TextField(new Rect((Screen.width/2)-200, (Screen.height/2), 400, 100), password, 25);

        if (GUI.Button(new Rect((Screen.width/2)-100, (Screen.height/2)+125, 200, 50),"login"))
        {
            StartCoroutine(get_auth_token("http://167.71.14.193:8081/auth?username="+username+";password="+password+""));
        } 
        if (GUI.Button(new Rect((Screen.width/2)-100, (Screen.height/2)+175, 200, 50),"back to main"))
        {
            SceneManager.LoadScene("main1");
        } 
    }
}
[System.Serializable]
public class login_token    {
    public bool status;
    public string token;

    public static login_token CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<login_token>(jsonString);
    }
}