using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour
{

    enum EditorState { idle, drawing };
    EditorState editorState = EditorState.idle;

	enum ToolMode { point = 0, select = 1 };
    ToolMode toolMode = ToolMode.point;
    public Texture point;
    public Texture line;
    public Texture rect;
	public Texture select;
    Texture[] toolImages;

    GameManager gameManager;
    BlockManager blockManager;
    NoRotationManager noRoMan;
    Player player;
	AbstractBlock selectedBlock;
	bool selectedPlayer;

	public GameObject selectionHighlightPrefab;
	GameObject selectionHighlight;



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
        this.toolImages = new Texture[] { this.point, this.select};
        this.player = FindObjectOfType<Player>();
        this.blockManager = FindObjectOfType<BlockManager>();
        this.noRoMan = FindObjectOfType<NoRotationManager>();
		this.selectionHighlight = Instantiate (selectionHighlightPrefab) as GameObject;
		selectionHighlight.SetActive (false);
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
						selectionHighlight.SetActive(false);
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
										blockManager.player = GetComponent<Player>();
										gameManager.player = GetComponent<Player>();
										gameManager.playerMovement = GetComponent<PlayerMovement>();
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

					//Select Mode!
					case ToolMode.select:
					{
						//release left click = select this block 
						//click elsewhere = unselect block, move selected cursor there
						if(Input.GetMouseButtonUp(0))
						{
							
							selectedBlock = blockManager.getBlockAt(mouseWorldPos.x,mouseWorldPos.y);
							selectedPlayer = false;
							selectionHighlight.SetActive(true);
							selectionHighlight.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, selectionHighlight.transform.position.z);

							if(selectedBlock ==null && player.GetRoundedPosition().x == mouseWorldPos.x && player.GetRoundedPosition().y == mouseWorldPos.y)
							{
								selectedPlayer = true;
							}
							
						}

						//if have a thing
						if(selectedBlock!=null || selectedPlayer)
						{
						//hold right click = drag this thing around
							if(Input.GetMouseButton(1)&&blockManager.getBlockAt(mouseWorldPos.x,mouseWorldPos.y)==null && !mouseWorldPos.Equals(player.GetRoundedPosition()))
						    {
								//if holding block and there's no block or player there, 
								if(selectedBlock !=null )
								{
									blockManager.grid.Remove(selectedBlock.GetCurrentPosition());
									selectedBlock.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y,0);
									selectionHighlight.transform.position = selectedBlock.transform.position;

									blockManager.grid.Add (new Int2(mouseWorldPos.x, mouseWorldPos.y), selectedBlock);
									if (selectedBlock as LaserShooter != null) {
										(selectedBlock as LaserShooter).setFireDirection();
									}
									else if (selectedBlock as MirrorBlock != null) {
										(selectedBlock as MirrorBlock).stopFiring();
									}

	 							}
								else if(selectedPlayer)
								{
									player.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y,0);
									selectionHighlight.transform.position = player.transform.position;
								}
							}
							//rotate ccw
							else if(selectedBlock !=null && Input.GetKeyDown(KeyCode.Q))
							{
								selectedBlock.orientation += 1;
								selectedBlock.blockSprite.transform.eulerAngles = new Vector3(0f, 0f, selectedBlock.orientation * 90f);
								if (selectedBlock as LaserShooter != null) {
									(selectedBlock as LaserShooter).setFireDirection();
									
								}
								else if (selectedBlock as MirrorBlock != null) {
									(selectedBlock as MirrorBlock).stopFiring();
								}
							}
							else if(selectedBlock !=null && Input.GetKeyDown(KeyCode.E))
							{
								selectedBlock.orientation -= 1;
								selectedBlock.blockSprite.transform.eulerAngles = new Vector3(0f, 0f, selectedBlock.orientation * 90f);
								if (selectedBlock as LaserShooter != null) {
									(selectedBlock as LaserShooter).setFireDirection();
								}
								else if (selectedBlock as MirrorBlock != null) {
									(selectedBlock as MirrorBlock).stopFiring();
								}
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
				selectionHighlight.SetActive(false);
                gameManager.gameState = GameManager.GameMode.playing;
            }
            GUILayout.EndArea();
        }
    }
}
