using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class register : MonoBehaviour
{
    private string results =" ";

    private string status = "";

    private string username = "";
    private string password = "";
    
    private register_status user_register;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator get_register_status(string url) {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log("ERR");
        }
        else {
            results = www.downloadHandler.text;
            Debug.Log(results);
            
            user_register = register_status.CreateFromJSON(results);
            if(user_register.status)
            {
                SceneManager.LoadScene("login");
            }
        }
    }
    private void OnGUI() {
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize =GUI.skin.textField.fontSize= 20;
        username = GUI.TextField(new Rect((Screen.width/2)-200, (Screen.height/2)-125, 400, 100), username, 25);
        password = GUI.TextField(new Rect((Screen.width/2)-200, (Screen.height/2), 400, 100), password, 25);

        if (GUI.Button(new Rect((Screen.width/2)-100, (Screen.height/2)+125, 200, 50),"register"))
        {
            StartCoroutine(get_register_status("http://167.71.14.193:8081/register?username="+username+";password="+password+""));
        } 
        if (GUI.Button(new Rect((Screen.width/2)-100, (Screen.height/2)+175, 200, 50),"back to main"))
        {
            SceneManager.LoadScene("main1");
        } 

    }
}
[System.Serializable]
public class register_status    {
    public bool status;

    public static register_status CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<register_status>(jsonString);
    }
}
