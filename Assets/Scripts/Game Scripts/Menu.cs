using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    public KeyCode quitKey = KeyCode.Escape;

    void Update()
    {
        if (Input.GetKey(quitKey))
            Application.Quit();
    }


    void OnGUI()
    {
        GUI.BeginGroup(new Rect(0, 100, 500, 500));
        for (int i = 0; i < Application.levelCount; i++)
        {
            if (GUILayout.Button("Load the Level: " + i.ToString()))
            {
                Application.LoadLevel(i);
            }
        }
        GUI.EndGroup();
    }
	
}
