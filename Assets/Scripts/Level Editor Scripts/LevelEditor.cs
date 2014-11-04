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
    NoRotationManager noRoMan;
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

    List<Rect> guiRects;


    void Awake()
    {
        this.gameManager = GetComponent<GameManager>();
        this.toolImages = new Texture[] { this.point, this.line, this.rect };
        this.player = FindObjectOfType<Player>();
        this.blockManager = FindObjectOfType<BlockManager>();
        this.noRoMan = FindObjectOfType<NoRotationManager>();
        guiRects = new List<Rect>();

        // Get the images for our brushes
        this.brushImages = new Texture[brushes.Length];
        for (int i = 0; i < brushes.Length; i++)
        {
            brushImages[i] = brushes[i].image;
        }
    }

    bool MouseInGUI()
    {
        // Because different coordinate systems for GUI and world
        Vector2 pos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        foreach (Rect r in guiRects)
        {
            if (r.Contains(pos))
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        Vector3 mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Int2 mouseWorldPos = new Int2(mouseVector.x, mouseVector.y);

        if (gameManager.gameState == GameManager.GameMode.editing)
        {
            if (!MouseInGUI())
            {
                switch (toolMode)
                {
                    case ToolMode.point:
                        {
                            if (currentBrush.isPlayer)
                            {
                                if (Input.GetMouseButton(0))
                                {
                                    // If there's not a block where we're trying to place the player
                                    if (!blockManager.grid.ContainsKey(mouseWorldPos))
                                    {
                                        player = FindObjectOfType<Player>();
                                        if (player == null)
                                        {
                                            GameObject p = Instantiate(currentBrush.prefab, mouseWorldPos.ToVector2(), Quaternion.identity) as GameObject;
                                        }
                                        else
                                        {
                                            player.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, player.transform.position.z);
                                        }
                                    }
                                }
                            }
                            else if (currentBrush.isButter)
                            {
                                if (Input.GetMouseButton(0))
                                {
                                    ButterBlock butter = FindObjectOfType<ButterBlock>();
                                    if (butter == null)
                                    {
                                        GameObject b = Instantiate(currentBrush.prefab, mouseWorldPos.ToVector2(), Quaternion.identity) as GameObject;
                                        blockManager.AddBlock(mouseWorldPos, b.GetComponent<ButterBlock>());
                                    }
                                    else
                                    {
                                        blockManager.ChangePos(butter.GetCurrentPosition(), mouseWorldPos);
                                    }
                                }
                            }
                            else if (currentBrush.isCrawler)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    Instantiate(currentBrush.prefab, mouseWorldPos.ToVector2(), Quaternion.identity);
                                }
                            }
                            else if (currentBrush.isNoRotationZone)
                            {
                                if (Input.GetMouseButton(0))
                                {
                                    if (noRoMan.AddZone(mouseWorldPos))
                                    {
                                        Instantiate(currentBrush.prefab, mouseWorldPos.ToVector2(), Quaternion.identity);
                                    }
                                }
                                if (Input.GetMouseButton(1))
                                {
                                    noRoMan.RemoveZone(mouseWorldPos);
                                }
                            }
                            // Brush is a block
                            else
                            {
                                if (Input.GetMouseButton(0))
                                {
                                    if (player != null && !mouseWorldPos.Equals(player.GetRoundedPosition()))
                                    {
                                        GameObject b = Instantiate(currentBrush.prefab, mouseWorldPos.ToVector2(), Quaternion.identity) as GameObject;
                                        AbstractBlock theBlock = b.GetComponent<AbstractBlock>();
                                        blockManager.AddBlock(mouseWorldPos, theBlock);
                                    }
                                }
                                else if (Input.GetMouseButton(1))
                                {
                                    blockManager.RemoveBlock(mouseWorldPos);
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
            Rect brushRect = new Rect(0, Screen.height - boxHeight, Screen.width, boxHeight);
            Rect toolRect = new Rect(0, 0, boxWidth, boxHeight);
            Rect playRect = new Rect(Screen.width - boxWidth, 0, boxWidth, boxHeight);
            guiRects.Add(brushRect);
            guiRects.Add(toolRect);
            guiRects.Add(playRect);

            // Different brushes
            GUILayout.BeginArea(brushRect);
            this.currentBrushNumber = GUILayout.Toolbar(currentBrushNumber, brushImages, GUILayout.MaxHeight(boxHeight), GUILayout.MaxWidth(Screen.width));
            GUILayout.EndArea();

            // Different tools
            GUILayout.BeginArea(toolRect);
            this.toolMode = (ToolMode)GUILayout.Toolbar((int)toolMode, toolImages, GUILayout.MaxWidth(boxWidth), GUILayout.MaxHeight(boxHeight));
            GUILayout.EndArea();

            // Play button
            GUILayout.BeginArea(playRect);
            if (GUILayout.Button("Play"))
            {
                gameManager.gameState = GameManager.GameMode.playing;
            }
            GUILayout.EndArea();
        }
    }
}
