using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour
{

    enum EditorState { idle, drawing };
    EditorState editorState = EditorState.idle;
    enum ToolMode { point = 0, line = 1, rect = 2 };
    ToolMode toolMode = ToolMode.point;
    public Texture point;
    public Texture line;
    public Texture rect;
    Texture[] toolImages;
    [System.Serializable]
    public class Brush
    {
        public string name;
        public Texture image;
        public GameObject prefab;
    }

    public Brush[] brushes;
    Texture[] brushImages;
    int currentPrefab = 0;


    void Awake()
    {
        this.toolImages = new Texture[] { this.point, this.line, this.rect };

        // Get the images for our brushes
        this.brushImages = new Texture[brushes.Length];
        for (int i = 0; i < brushes.Length; i++)
        {
            brushImages[i] = brushes[i].image;
        }
    }

    void OnGUI()
    {
        float boxWidth = Screen.width / 3;
        float boxHeight = Screen.height / 10;
        // Different brushes
        GUILayout.BeginArea(new Rect(0, Screen.height - boxHeight, boxWidth, boxHeight));
            this.currentPrefab = GUILayout.Toolbar(currentPrefab, brushImages, GUILayout.MaxHeight(boxHeight), GUILayout.MaxWidth(boxWidth));
        GUILayout.EndArea();
        
        // Different tools
        GUILayout.BeginArea(new Rect(0, 0, boxWidth, boxHeight));
            this.toolMode = (ToolMode)GUILayout.Toolbar((int)toolMode, toolImages, GUILayout.MaxWidth(boxWidth), GUILayout.MaxHeight(boxHeight));
        GUILayout.EndArea();
    }
}
