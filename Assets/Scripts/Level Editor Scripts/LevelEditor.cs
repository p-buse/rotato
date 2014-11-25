using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class LevelEditor : MonoBehaviour
{
    public Texture pointTexture;
    public Texture selectTexture;
    public GameObject selectionHighlightPrefab;
    public Brush[] brushes;

    GameObject selectionHighlight;

	enum ToolMode { point = 0, select = 1 };
    ToolMode toolMode = ToolMode.point;
    
    string currentLevelName;
    Texture[] toolImages;

    GameManager gameManager;
    BlockManager blockManager;
    NoRotationManager noRoMan;
    Player player;
	AbstractBlock selectedBlock;
	bool selectedPlayer;

    string levelsPath;

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
    Texture[] brushImages;
    int currentBrushNumber = 0;
    Brush currentBrush
    {
        get
        {
            return brushes[currentBrushNumber];
        }
    }

    

    HashSet<Rect> guiRects;
    Dictionary<string, GameObject> nameToBlockPrefab;
    struct SpecialPrefabs
    {
        public GameObject playerPrefab;
        public GameObject crawlerPrefab;
        public GameObject noRoPrefab;
        public GameObject spikesPrefab;
    }
    SpecialPrefabs specialPrefabs;

    bool awaitingConfirmation = false;
    string confirmationMessage = "";

    bool loadMenu = false;

    Vector2 loadMenuScrollPosition = Vector2.zero;

    GameObject ghostlyBlock;
    private int _currentOrientation;
    public int currentOrientation
    {
        get
        {
            return _currentOrientation;
        }
        set
        {
            if (value < 0)
            {
                _currentOrientation = value + 4;
            }
            else if (value >= 4)
            {
                _currentOrientation = value % 4;
            }
            else
            {
                _currentOrientation = value;
            }
        }
    }

    void Awake()
    {
        this.currentLevelName = "Untitled";
        this.gameManager = GetComponent<GameManager>();
        this.toolImages = new Texture[] { this.pointTexture, this.selectTexture};
        this.player = FindObjectOfType<Player>();
        this.blockManager = FindObjectOfType<BlockManager>();
        this.noRoMan = FindObjectOfType<NoRotationManager>();
		this.selectionHighlight = Instantiate (selectionHighlightPrefab) as GameObject;
		selectionHighlight.SetActive (false);
        guiRects = new HashSet<Rect>();
        nameToBlockPrefab = new Dictionary<string,GameObject>();
        gameManager.PlayerCreated += this.PlayerCreated;
        this.levelsPath = Application.dataPath + "/Levels/";
        if (!Directory.Exists(levelsPath))
        {
            Directory.CreateDirectory(levelsPath);
        }
        this.UpdateGhostlyBlock();
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
            nameToBlockPrefab[brushes[i].name] = brushes[i].prefab;

            // Add special prefabs
            if (brushes[i].isCrawler)
                specialPrefabs.crawlerPrefab = brushes[i].prefab;
            if (brushes[i].isNoRotationZone)
                specialPrefabs.noRoPrefab = brushes[i].prefab;
            if (brushes[i].isPlayer)
                specialPrefabs.playerPrefab = brushes[i].prefab;
            if (brushes[i].isSpikez)
                specialPrefabs.spikesPrefab = brushes[i].prefab;
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
        DrawGhostlyBlock(mouseWorldPos);
        if (gameManager.gameState == GameManager.GameMode.editing && !this.awaitingConfirmation && !this.loadMenu)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentOrientation -= 1;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                currentOrientation += 1;
            }
            if (!MouseInGUI())
            {
                switch (toolMode)
                {
                    case ToolMode.point:
                        {
                            selectionHighlight.SetActive(false);
                            if (currentBrush.isPlayer)
                            {
                                PaintPlayer(mouseWorldPos);
                            }
                            else if (currentBrush.isButter)
                            {
                                PaintButter(mouseWorldPos);
                            }
                            else if (currentBrush.isCrawler)
                            {
                                PaintCrawler(mouseWorldPos);
                            }
                            else if (currentBrush.isNoRotationZone)
                            {
                                PaintNoRoZone(mouseWorldPos);
                            }
                            else if (currentBrush.isSpikez)
                            {
                                PaintSpikes(mouseWorldPos);
                            }
                            // Brush is a block
                            else
                            {
                                PaintBlock(mouseWorldPos);
                            }
                            break;
                        }

                    //Select Mode!
                    case ToolMode.select:
                        {
                            //release left click = select this block 
                            //click elsewhere = unselect block, move selected cursor there
                            if (Input.GetMouseButtonUp(0))
                            {

                                selectedBlock = blockManager.getBlockAt(mouseWorldPos.x, mouseWorldPos.y);
                                selectedPlayer = false;
                                selectionHighlight.SetActive(true);
                                selectionHighlight.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, selectionHighlight.transform.position.z);

                                if (player != null && selectedBlock == null && player.GetRoundedPosition().x == mouseWorldPos.x && player.GetRoundedPosition().y == mouseWorldPos.y)
                                {
                                    selectedPlayer = true;
                                }

                            }

                            //if have a thing
                            if (selectedBlock != null || selectedPlayer)
                            {
                                //hold right click = drag this thing around
                                if (Input.GetMouseButton(1) && blockManager.getBlockAt(mouseWorldPos.x, mouseWorldPos.y) == null && (player == null || !mouseWorldPos.Equals(player.GetRoundedPosition())))
                                {
                                    //if holding block and there's no block or player there, 
                                    if (selectedBlock != null)
                                    {
                                        blockManager.grid.Remove(selectedBlock.GetCurrentPosition());
                                        selectedBlock.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
                                        selectionHighlight.transform.position = selectedBlock.transform.position;

                                        blockManager.grid.Add(new Int2(mouseWorldPos.x, mouseWorldPos.y), selectedBlock);
                                        if (selectedBlock as LaserShooter != null)
                                        {
                                            (selectedBlock as LaserShooter).setFireDirection();
                                        }
                                        else if (selectedBlock as MirrorBlock != null)
                                        {
                                            (selectedBlock as MirrorBlock).stopFiring();
                                        }

                                    }
                                    else if (selectedPlayer)
                                    {
                                        player.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
                                        selectionHighlight.transform.position = player.transform.position;
                                    }
                                }
                                //rotate ccw
                                else if (selectedBlock != null && Input.GetKeyDown(KeyCode.A))
                                {
                                    selectedBlock.orientation += 1;
                                    selectedBlock.blockSprite.transform.eulerAngles = new Vector3(0f, 0f, selectedBlock.orientation * 90f);
                                    if (selectedBlock as LaserShooter != null)
                                    {
                                        (selectedBlock as LaserShooter).setFireDirection();

                                    }
                                    else if (selectedBlock as MirrorBlock != null)
                                    {
                                        (selectedBlock as MirrorBlock).stopFiring();
                                    }
                                }
                                else if (selectedBlock != null && Input.GetKeyDown(KeyCode.D))
                                {
                                    selectedBlock.orientation -= 1;
                                    selectedBlock.blockSprite.transform.eulerAngles = new Vector3(0f, 0f, selectedBlock.orientation * 90f);
                                    if (selectedBlock as LaserShooter != null)
                                    {
                                        (selectedBlock as LaserShooter).setFireDirection();
                                    }
                                    else if (selectedBlock as MirrorBlock != null)
                                    {
                                        (selectedBlock as MirrorBlock).stopFiring();
                                    }
                                }
								else if (selectedBlock != null && Input.GetKeyDown(KeyCode.W)) {
									if (selectedBlock as Salt != null) {
										(selectedBlock as Salt).rotationsBeforeRemove++;
										(selectedBlock as Salt).field.text = "" + (selectedBlock as Salt).rotationsBeforeRemove;
									}
									else if (selectedBlock as CrackedBlock != null) {
										(selectedBlock as CrackedBlock).rotationsLeft++;
									}
								}
								else if (selectedBlock != null && Input.GetKeyDown(KeyCode.S)) {
									if (selectedBlock as Salt != null) {
										(selectedBlock as Salt).rotationsBeforeRemove--;
										(selectedBlock as Salt).field.text = "" + (selectedBlock as Salt).rotationsBeforeRemove;
									}
									else if (selectedBlock as CrackedBlock != null) {
										(selectedBlock as CrackedBlock).rotationsLeft--;
									}
								}
                            }
                            break;

                        }
                }
            }
        }
        else
        {
            HideGhostlyBlock();
        }
    }

    private void UpdateGhostlyBlock()
    {
        if (!currentBrush.isCrawler && !currentBrush.isPlayer)
        {
            if (ghostlyBlock != null)
            {
                Destroy(ghostlyBlock);
            }
            this.ghostlyBlock = Instantiate(currentBrush.prefab) as GameObject;
            Component[] components = this.ghostlyBlock.GetComponentsInChildren<Component>();
            // Destroy everything except an object's sprites
            foreach (Component comp in components)
            {
                SpriteRenderer spriteComponent = comp as SpriteRenderer;
                if (spriteComponent == null && !comp.Equals(comp.transform))
                {
                    Destroy(comp);
                }
            }
            // Make it *ghostly* by setting alpha value on the sprite
            SpriteRenderer sr = ghostlyBlock.GetComponentInChildren<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.3f);
        }
    }

    private void HideGhostlyBlock()
    {
        if (ghostlyBlock != null)
            ghostlyBlock.transform.position = new Vector3(0f, 0f, -1000f);
    }

    private void DrawGhostlyBlock(Int2 mouseWorldPos)
    {
        if (ghostlyBlock != null)
        {
            if (!currentBrush.isCrawler && !currentBrush.isPlayer && !MouseInGUI() && toolMode == ToolMode.point)
            {
                this.ghostlyBlock.transform.position = mouseWorldPos.ToVector2();
                this.ghostlyBlock.transform.eulerAngles = new Vector3(0f, 0f, currentOrientation * 90f);
            }
            else
            {
                this.ghostlyBlock.transform.position = new Vector3(0f, 0f, -1000f);
            }
        }
    }


    private void PaintSpikes(Int2 mouseWorldPos)
    {
        if (Input.GetMouseButton(0))
        {
            AbstractBlock blockAtMouse = blockManager.getBlockAt(mouseWorldPos);
            if (blockAtMouse != null)
            {
                blockAtMouse.AddSpike(currentBrush.prefab, currentOrientation);
            }
        }
        else if (Input.GetMouseButton(1))
        {
            AbstractBlock blockAtMouse = blockManager.getBlockAt(mouseWorldPos);
            if (blockAtMouse != null)
            {
                blockAtMouse.RemoveSpike(currentOrientation);
            }
        }
    }

    private void PaintBlock(Int2 mouseWorldPos)
    {
        if (Input.GetMouseButton(0))
        {
            if (player == null || !mouseWorldPos.Equals(player.GetRoundedPosition()))
            {
                AddBlock(mouseWorldPos, currentBrush.prefab, currentOrientation);
            }
        }
        else if (Input.GetMouseButton(1))
        {
            blockManager.RemoveBlock(mouseWorldPos);
        }
    }

    private void PaintNoRoZone(Int2 mouseWorldPos)
    {
        if (Input.GetMouseButton(0))
        {
            noRoMan.AddNoRoZone(mouseWorldPos, currentBrush.prefab);
        }
        if (Input.GetMouseButton(1))
        {
            noRoMan.RemoveNoRoZone(mouseWorldPos);
        }
    }

    private void PaintCrawler(Int2 mouseWorldPos)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(currentBrush.prefab, mouseWorldPos.ToVector2(), Quaternion.identity);
        }
    }

    private void PaintButter(Int2 mouseWorldPos)
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

    private void PaintPlayer(Int2 mouseWorldPos)
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

    private AbstractBlock AddBlock(Int2 pos, GameObject blockPrefab, int orientation, List<int> spikiness = null)
    {
        GameObject b = Instantiate(blockPrefab, pos.ToVector2(), Quaternion.identity) as GameObject;
        AbstractBlock theBlock = b.GetComponent<AbstractBlock>();
        if (theBlock == null)
            Debug.LogError("couldn't get AbstractBlock component of: " + blockPrefab);
        blockManager.AddBlock(pos, theBlock);
        if (spikiness != null)
        {
            foreach (int spikeDirection in spikiness)
            {
                theBlock.AddSpike(specialPrefabs.spikesPrefab, spikeDirection);
            }
            theBlock.spikiness = spikiness;
        }
        theBlock.orientation = orientation;
        theBlock.blockSprite.transform.eulerAngles = new Vector3(0f, 0f, theBlock.orientation * 90f);
        return theBlock;
    }

    void OnGUI()
    {
        // Setup our various measurements and boxes.
        // We do this every frame to adjust for changing window sizes.
        float boxWidth = Screen.width / 3;
        float boxHeight = Screen.height / 10;
        Rect brushRect = new Rect(0, Screen.height - boxHeight, Screen.width, boxHeight);
        Rect toolRect = new Rect(0, 0, boxWidth, boxHeight);
        Rect playEditRect = new Rect(Screen.width - boxWidth, 0, boxWidth, boxHeight);
        Rect saveLoadRect = new Rect(Screen.width - boxWidth, boxHeight * 2, boxWidth, boxHeight * 2);
        Rect loadRect = new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2);
        SetupGUIRects(brushRect, toolRect, playEditRect, saveLoadRect);
        if (!awaitingConfirmation)
        {
            if (gameManager.gameState == GameManager.GameMode.editing)
            {
                if (!loadMenu)
                {
                    DrawBrushes(boxHeight, brushRect);
                    DrawTools(boxWidth, boxHeight, toolRect);
                    DrawPlayButton(playEditRect);
                    DrawSaveLoadButtons(saveLoadRect);
                }
                else
                {
                    DrawLoadMenu(loadRect);
                }
            }
            else if (gameManager.gameState == GameManager.GameMode.playing && gameManager.canEdit)
            {
                DrawEditButton(playEditRect);
            }
            else if (gameManager.gameState == GameManager.GameMode.playing && !gameManager.canEdit)
            {
                DrawSkipButton(playEditRect);
            }
        }
        else
        {
            Rect confirmationRect = new Rect(Screen.width * 0.375f, Screen.height * 0.375f, Screen.width / 4, Screen.height / 4);
            DrawConfirmationWindow(confirmationRect);
        }
    }

    private void DrawLoadMenu(Rect loadRect)
    {
        GUILayout.BeginArea(loadRect, "", "box");
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("X"))
        {
            loadMenu = false;
        }
        GUILayout.EndHorizontal();
        this.loadMenuScrollPosition = GUILayout.BeginScrollView(loadMenuScrollPosition);
        foreach (string levelName in this.GetLevelNames())
        {
            if (GUILayout.Button(levelName))
            {
                LoadLevel(levelName);
                loadMenu = false;
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawConfirmationWindow(Rect confirmationRect)
    {
        GUILayout.BeginArea(confirmationRect, "", "box");
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.Label(confirmationMessage);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Yes"))
        {
            this.confirmationMessage = "";
            this.awaitingConfirmation = false;
            this.SaveLevel(currentLevelName, true);
        }
        if (GUILayout.Button("No"))
        {
            this.confirmationMessage = "";
            this.awaitingConfirmation = false;
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawSkipButton(Rect playEditRect)
    {
        GUILayout.BeginArea(playEditRect);
        if (GUILayout.Button("Skip"))
        {
            gameManager.GoToNextLevel();
        }
        GUILayout.EndArea();
    }

    private void DrawEditButton(Rect playEditRect)
    {
        GUILayout.BeginArea(playEditRect);
        if (GUILayout.Button("Edit"))
        {
            gameManager.gameState = GameManager.GameMode.editing;
        }
        GUILayout.EndArea();
    }

    private void DrawSaveLoadButtons(Rect saveLoadRect)
    {
        GUILayout.BeginArea(saveLoadRect, "", "box");
        this.currentLevelName = GUILayout.TextField(this.currentLevelName);
        if (GUILayout.Button("Save"))
        {
            SaveLevel(this.currentLevelName);
        }
        if (GUILayout.Button("Load"))
        {
            this.loadMenu = true;
        }
        GUILayout.EndArea();
    }

    private void DrawPlayButton(Rect playEditRect)
    {
        // Play/Edit button
        GUILayout.BeginArea(playEditRect);
        if (GUILayout.Button("Play"))
        {
            SaveLevel(currentLevelName, true);
            selectionHighlight.SetActive(false);
            gameManager.gameState = GameManager.GameMode.playing;
        }
        GUILayout.EndArea();
    }

    private void DrawTools(float boxWidth, float boxHeight, Rect toolRect)
    {
        // Different tools
        GUILayout.BeginArea(toolRect);
        this.toolMode = (ToolMode)GUILayout.Toolbar((int)toolMode, toolImages, GUILayout.MaxWidth(boxWidth), GUILayout.MaxHeight(boxHeight));
        GUILayout.EndArea();
    }

    private void DrawBrushes(float boxHeight, Rect brushRect)
    {
        // Different brushes
        GUILayout.BeginArea(brushRect);
        int lastBrush = currentBrushNumber;
        this.currentBrushNumber = GUILayout.Toolbar(currentBrushNumber, brushImages, GUILayout.MaxHeight(boxHeight), GUILayout.MaxWidth(Screen.width));
        if (lastBrush != currentBrushNumber)
        {
            UpdateGhostlyBlock();
            this.toolMode = ToolMode.point;
        }
        GUILayout.EndArea();
    }

    private void SetupGUIRects(params Rect[] rects)
    {
        // Clear our GUI rectangles, then add our current ones
        guiRects = new HashSet<Rect>();
        foreach (Rect r in rects)
        {
            guiRects.Add(r);
        }
    }

    void ConfirmOverWrite(string confirmationMessage)
    {
        this.awaitingConfirmation = true;
        this.confirmationMessage = confirmationMessage;
    }

    private string PathToLevel(string levelName)
    {
        return this.levelsPath + levelName + ".xml";
    }

    void SaveLevel(string levelName, bool overwrite = false)
    {
        string levelPath = PathToLevel(levelName);
        if (!File.Exists(levelPath) || overwrite)
        {
            if (this.player != null)
            {
                LevelSkeleton currentLevel = this.ConvertCurrentLevelToSkeleton();
                LevelEditor.WriteXML(currentLevel, levelPath);
            }
            else
            {
                Debug.LogWarning("Can't save level if no player placed!");
            }
        }
        else
        {
            ConfirmOverWrite("Overwrite " + levelName + "?");
        }
    }

    private void LoadLevel(string levelName)
    {
        string levelPath = PathToLevel(levelName);
        LevelSkeleton loadedLevel = ReadXML(levelPath);
        if (loadedLevel != null)
        {
            this.LoadLevelFromSkeleton(loadedLevel);
        }
        this.currentLevelName = levelName;
    }

    public void ResetLevel()
    {
        LoadLevel(currentLevelName);
    }

    void LoadLevelFromSkeleton(LevelSkeleton skeleton)
    {
        // Add our blocks
        blockManager.DestroyAllBlocks();
        foreach(BlockSkeleton blockSkelly in skeleton.blocks)
        {
            GameObject newBlock;
            if (nameToBlockPrefab.TryGetValue(blockSkelly.name, out newBlock))
            {
                AbstractBlock currentBlock = AddBlock(blockSkelly.position, newBlock, blockSkelly.orientation, blockSkelly.spikiness);
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
        if (player != null)
            Destroy(player.gameObject);
        Instantiate(specialPrefabs.playerPrefab, skeleton.playerPosition.ToVector2(), Quaternion.identity);

        // Add crawlers
        GameObject[] crawlers = GameObject.FindGameObjectsWithTag("Crawler");
        foreach (GameObject c in crawlers)
        {
            Destroy(c);
        }

        foreach (Vector2 newCrawlerPosition in skeleton.crawlers)
        {
            Instantiate(specialPrefabs.crawlerPrefab, newCrawlerPosition, Quaternion.identity);
        }

        // Add noRoZones
        noRoMan.ClearNoRotationZones();
        foreach (Int2 noRoZone in skeleton.noRoZones)
        {
            noRoMan.AddNoRoZone(noRoZone, specialPrefabs.noRoPrefab);
        }
    }

    LevelSkeleton ConvertCurrentLevelToSkeleton()
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
        writer.Serialize(file, level);
        file.Close();
        Debug.Log("Wrote level to " + path);
    }

    public static LevelSkeleton ReadXML(string path)
    {
       XmlSerializer deserializer = new XmlSerializer(typeof(LevelSkeleton));
       try
       {
           TextReader textReader = new StreamReader(path);
           LevelSkeleton loadedLevel;
           loadedLevel = (LevelSkeleton)deserializer.Deserialize(textReader);
           textReader.Close();
           return loadedLevel;
       }
       catch (FileNotFoundException)
       {
           Debug.LogWarning("Couldn't read file from " + path);
           return null;
       }
    }

    string[] GetLevelNames()
    {
        string[] levelPaths = Directory.GetFiles(levelsPath, "*.xml");
        for (int i = 0; i < levelPaths.Length; i++)
        {
            levelPaths[i] = levelPaths[i].Replace(levelsPath, "");
            levelPaths[i] = levelPaths[i].Replace(".xml", "");
        }
        return levelPaths;
    }
}
