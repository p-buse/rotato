using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class LevelEditor : MonoBehaviour
{
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

    string path = @"c:\temp\SerializationOverview.xml";

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
        public bool isSpikez;
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
    Dictionary<string, GameObject> nameToBlockPrefabs;
    GameObject playerPrefab;
    GameObject crawlerPrefab;
    GameObject noRoPrefab;
    GameObject spikesPrefab;


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
        nameToBlockPrefabs = new Dictionary<string,GameObject>();
        gameManager.PlayerCreated += this.PlayerCreated;
    }

    void PlayerCreated(GameManager gm, Player p, PlayerMovement pm)
    {
        this.player = p;
    }

    void Start()
    {
        this.brushImages = new Texture[brushes.Length];
        for (int i = 0; i < brushes.Length; i++)
        {
            // Set the images for our brushes
            brushImages[i] = brushes[i].image;

            if (!brushes[i].isCrawler && !brushes[i].isPlayer && !brushes[i].isNoRotationZone && !brushes[i].isSpikez)
            {
                // Set the names of our blocks
                AbstractBlock block = brushes[i].prefab.GetComponent<AbstractBlock>();
                if (block == null)
                    Debug.LogError("Couldn't get block component of prefab: " + brushes[i].prefab);
                brushes[i].name = block.myType();
            }

            // Add blocks to our dictionary
            nameToBlockPrefabs[brushes[i].name] = brushes[i].prefab;
            if (brushes[i].isCrawler)
                crawlerPrefab = brushes[i].prefab;
            if (brushes[i].isNoRotationZone)
                noRoPrefab = brushes[i].prefab;
            if (brushes[i].isPlayer)
                playerPrefab = brushes[i].prefab;
            if (brushes[i].isSpikez)
                spikesPrefab = brushes[i].prefab;
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
                                        Instantiate(currentBrush.prefab, mouseWorldPos.ToVector2(), Quaternion.identity);
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
                                    if (blockManager.getBlockAt(mouseWorldPos) == null)
                                    {
                                        GameObject b = Instantiate(currentBrush.prefab, mouseWorldPos.ToVector2(), Quaternion.identity) as GameObject;
                                        blockManager.AddBlock(mouseWorldPos, b.GetComponent<ButterBlock>());
                                    }
	                            }
	                            else
	                            {
                                    if (blockManager.getBlockAt(mouseWorldPos) == null)
                                    {
                                        blockManager.ChangePos(butter.GetCurrentPosition(), mouseWorldPos);
                                    }
	                            }
	                        }
                            else if (Input.GetMouseButton(1))
                            {
                                blockManager.RemoveBlock(mouseWorldPos);
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
	                            if (player == null || !mouseWorldPos.Equals(player.GetRoundedPosition()))
	                            {
                                    AddBlock(mouseWorldPos, currentBrush.prefab, 0);
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

							if(player != null && selectedBlock ==null && player.GetRoundedPosition().x == mouseWorldPos.x && player.GetRoundedPosition().y == mouseWorldPos.y)
							{
								selectedPlayer = true;
							}
							
						}

						//if have a thing
						if(selectedBlock!=null || selectedPlayer)
						{
						//hold right click = drag this thing around
							if(Input.GetMouseButton(1)&&blockManager.getBlockAt(mouseWorldPos.x,mouseWorldPos.y)==null && (player == null || !mouseWorldPos.Equals(player.GetRoundedPosition())))
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
							else if(selectedBlock !=null && (Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Mouse ScrollWheel") > 0))
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
							else if(selectedBlock !=null && (Input.GetKeyDown(KeyCode.E) || Input.GetAxis("Mouse ScrollWheel") < 0))
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

    private AbstractBlock AddBlock(Int2 pos, GameObject blockPrefab, int orientation)
    {
        GameObject b = Instantiate(blockPrefab, pos.ToVector2(), Quaternion.identity) as GameObject;
        AbstractBlock theBlock = b.GetComponent<AbstractBlock>();
        if (theBlock == null)
            Debug.LogError("couldn't get AbstractBlock component of: " + blockPrefab);
        blockManager.AddBlock(pos, theBlock);
        theBlock.orientation = orientation;
        theBlock.blockSprite.transform.eulerAngles = new Vector3(0f, 0f, theBlock.orientation * 90f);
        return theBlock;
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
            Rect editRect = new Rect(Screen.width - boxWidth, boxHeight *2, boxWidth, boxHeight *2);
            guiRects.Add(brushRect);
            guiRects.Add(toolRect);
            guiRects.Add(playRect);
            guiRects.Add(editRect);

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

            GUILayout.BeginArea(editRect);
            if (GUILayout.Button("Save"))
            {
                LevelSkeleton currentLevel = this.ConvertLevelToSkeleton();
                LevelEditor.WriteXML(currentLevel, this.path);
            }
            if (GUILayout.Button("Load"))
            {
                LevelSkeleton loadedLevel = ReadXML(this.path);
                this.LoadLevel(loadedLevel);
            }
            GUILayout.EndArea();
        }
    }

    void LoadLevel(LevelSkeleton skeleton)
    {
        // Add our blocks
        blockManager.DestroyAllBlocks();
        foreach(BlockSkeleton blockSkelly in skeleton.blocks)
        {
            GameObject newBlock;
            if (nameToBlockPrefabs.TryGetValue(blockSkelly.name, out newBlock))
            {
                print(newBlock);
                AbstractBlock currentBlock = AddBlock(blockSkelly.position, newBlock, blockSkelly.orientation);

                if (currentBlock as CrackedBlock != null)
                {
                    CrackedBlock currentBlockIfItsCracked = currentBlock as CrackedBlock;
                    currentBlockIfItsCracked.rotationsLeft = blockSkelly.rotationsTillDeath;
                }
            }
            else
            {
                print("couldn't find block with name: " + blockSkelly.name);
            }
        }

        // Add player
        Destroy(FindObjectOfType<Player>().gameObject);
        Instantiate(playerPrefab, skeleton.playerPosition.ToVector2(), Quaternion.identity);

        // Add crawlers
        GameObject[] crawlers = GameObject.FindGameObjectsWithTag("Crawler");
        foreach (GameObject c in crawlers)
        {
            Destroy(c);
        }

        foreach (Vector2 newCrawlerPosition in skeleton.crawlers)
        {
            Instantiate(crawlerPrefab, newCrawlerPosition, Quaternion.identity);
        }

        // Add noRoZones
        noRoMan.ClearNoRotationZones();
        foreach (Int2 noRoZone in skeleton.noRoZones)
        {
            if (noRoMan.AddZone(noRoZone))
            {
                Instantiate(noRoPrefab, noRoZone.ToVector2(), Quaternion.identity);
            }
        }
    }

    LevelSkeleton ConvertLevelToSkeleton()
    {
        LevelSkeleton skelly = new LevelSkeleton();
        skelly.setGrid(blockManager.grid);
        skelly.setNoRoZoneGrid(noRoMan.noRotationZones);
        skelly.setCrawlers();
        if (player != null)
        {
            skelly.playerPosition = player.GetRoundedPosition();
        }
        return skelly;
    }

    public static void WriteXML(LevelSkeleton level, string path)
    {
        XmlSerializer writer = new XmlSerializer(typeof(LevelSkeleton));
        System.IO.StreamWriter file = new System.IO.StreamWriter(path);
        Debug.Log("Wrote level to " + path);
        writer.Serialize(file, level);
        file.Close();
    }

    public static LevelSkeleton ReadXML(string path)
    {
       XmlSerializer deserializer = new XmlSerializer(typeof(LevelSkeleton));
       TextReader textReader = new StreamReader(path);
       LevelSkeleton loadedLevel;
       loadedLevel = (LevelSkeleton)deserializer.Deserialize(textReader);
       textReader.Close();
       return loadedLevel;
    }
}
