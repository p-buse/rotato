using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    public KeyCode quitKey = KeyCode.Escape;
    public Texture levelEditorButton;
    public Texture playButton;


    void Update()
    {
        if (Input.GetKey(quitKey))
            Application.Quit();
    }


    void OnGUI()
    {
        Rect topScreen = new Rect(0, 0, Screen.width, Screen.height / 2);
        Rect bottomScreen = new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2);

        GUILayout.BeginArea(topScreen);
        if (GUILayout.Button(levelEditorButton))
        {
            Application.LoadLevel(1);
        }
        GUILayout.EndArea();
        GUILayout.BeginArea(bottomScreen);
        if (GUILayout.Button(playButton))
        {
            Application.LoadLevel(2);
        }
        GUILayout.EndArea();
    }
        
	
}
