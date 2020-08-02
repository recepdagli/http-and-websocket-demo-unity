using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnGUI()
    {
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 20;
        if (GUI.Button(new Rect((Screen.width/2)-100, (Screen.height/2)-100, 200, 50),"login"))
        {
            SceneManager.LoadScene("login");
        } 
        if (GUI.Button(new Rect((Screen.width/2)-100, (Screen.height/2), 200, 50),"register"))
        {
            SceneManager.LoadScene("register");
        } 
    }
}
