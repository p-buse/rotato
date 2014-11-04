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

    GameManager gameManager;
    BlockManager blockManager;
    Player player;

    [System.Serializable]
    public class Brush
    {
        public string name;
        public Texture image;
        public GameObject prefab;
        public bool isPlayer;
        public bool isButter;
        public bool isNoRotationZone;
        public bool isCrawler;
    }

    public Brush[] brushes;
    Texture[] brushImages;
    int currentBrushNumber = 0;
    Brush currentBrush
    {
        get
        {
            return brushes[currentBrushNumber];
        }
    }


    void Awake()
    {
        this.gameManager = GetComponent<GameManager>();
        this.toolImages = new Texture[] { this.point, this.line, this.rect };
        this.player = FindObjectOfType<Player>();
        this.blockManager = FindObjectOfType<BlockManager>();

        // Get the images for our brushes
        this.brushImages = new Texture[brushes.Length];
        for (int i = 0; i < brushes.Length; i++)
        {
            brushImages[i] = brushes[i].image;
        }
    }

    void Update()
    {
        Vector3 mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Int2 mousePos = new Int2(mouseVector.x, mouseVector.y);

        if (gameManager.gameState == GameManager.GameMode.editing)
        {
            if (Input.GetMouseButton(0))
            {
                switch (toolMode)
                {
                    case ToolMode.point:
                        {
                            if (currentBrush.isPlayer)
                            {
                            }
                            else if (currentBrush.isButter)
                            {
                            }
                            else if (currentBrush.isCrawler)
                            {
                            }
                            else if (currentBrush.isNoRotationZone)
                            {
                            }
                            // Brush is a block
                            else
                            {
                                if (player != null && mousePos != player.GetRoundedPosition())
                                {
                                    GameObject b = Instantiate(currentBrush.prefab, mousePos.ToVector2(), Quaternion.identity) as GameObject;
                                    AbstractBlock theBlock = b.GetComponent<AbstractBlock>();
                                    blockManager.AddBlock(mousePos, theBlock);
                                }
                            }
                            break;
                        }
                }
            }
        }
    }

    void OnGUI()
    {
        if (gameManager.gameState == GameManager.GameMode.editing)
        {
            float boxWidth = Screen.width / 3;
            float boxHeight = Screen.height / 10;
            // Different brushes
            GUILayout.BeginArea(new Rect(0, Screen.height - boxHeight, boxWidth, boxHeight));
            this.currentBrushNumber = GUILayout.Toolbar(currentBrushNumber, brushImages, GUILayout.MaxHeight(boxHeight), GUILayout.MaxWidth(boxWidth));
            GUILayout.EndArea();

            // Different tools
            GUILayout.BeginArea(new Rect(0, 0, boxWidth, boxHeight));
            this.toolMode = (ToolMode)GUILayout.Toolbar((int)toolMode, toolImages, GUILayout.MaxWidth(boxWidth), GUILayout.MaxHeight(boxHeight));
            GUILayout.EndArea();

            // Play button
            GUILayout.BeginArea(new Rect(Screen.width - boxWidth, 0, boxWidth, boxHeight));
            if (GUILayout.Button("Play"))
            {
                gameManager.gameState = GameManager.GameMode.playing;
            }
            GUILayout.EndArea();
        }
    }
}
