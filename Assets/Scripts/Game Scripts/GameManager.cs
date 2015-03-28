using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // References to other stuff
    WinAndLoseText winAndLoseText;
    BlockManager blockManager;
    NoRotationManager noRotationManager;
    SoundManager soundManager;
    [HideInInspector]
    public PlayerMovement playerMovement;
    [HideInInspector]
    public Player player;
    HighlightManager highlightManager;

    // Salt stuff
    [HideInInspector]
    public int veggiesFreed = 0;
	int totalVeggies;

    // Winning and losing
    public float winOrLoseCountdownTime = 2f;
    float resetClock = 0f;

    // Rotation stuff
    public float secondsToRotate = 1f;
    [HideInInspector]
    public Int2 currentRotationCenter;
    int currentRotationDirection = 0;
    int rotationsSinceFreezing = 0;
    bool rotationEmpty;
    float rotationClock = 0f;
	int rotations = 0;
    

    // Setting the player when one is created
    public delegate void PlayerCreatedHandler(GameManager gameManager, Player player, PlayerMovement playerMovement);
    public event PlayerCreatedHandler PlayerCreated;
    public void CreatePlayer(Player newPlayer, PlayerMovement newPlayerMovement)
    {
        if (newPlayer == null)
            print("new player null");
        if (newPlayerMovement == null)
            print("new player movement null");
        this.player = newPlayer;
        this.playerMovement = newPlayerMovement;
        this.PlayerCreated(this, newPlayer, newPlayerMovement);
    }

    // Gamemode stuff
    public enum GameMode { playing, frozen, rotating, won, lost, editing, paused };
    public GameMode gameState = GameMode.playing;
    // Used for returning from pause menu
    GameMode lastState = GameMode.playing;
    /// <summary>
    /// True if our gamestate is "playing", false otherwise
    /// </summary>
    public bool gameFrozen
    {
        get
        {
            if (gameState == GameMode.playing)
                return false;
            else
                return true;
        }
    }

    // Editor stuff
    public bool canEdit;

    // Cursor
    public GameObject cursorPrefab;

    // Input stuff
    InputManager inputManager;
    public InputManager.CapturedInput currentInput
    {
        get
        {
            return inputManager.current;
        }
    }

    // Bounding box of the level
    [HideInInspector]
    public Transform topLeft;
    [HideInInspector]
    public Transform bottomRight;

    public PhysicsMaterial2D noFrictionMaterial;

    public delegate void BoundsChangedHandler(GameManager gm, Transform topLeft, Transform bottomRight);
    public event BoundsChangedHandler BoundsChanged;

	


    void Awake()
    {
        if (FindObjectOfType<MusicManager>() == null)
        {
            new GameObject("Music Manager", typeof(MusicManager));
        }
        this.winAndLoseText = GetComponent<WinAndLoseText>();
        this.inputManager = GetComponent<InputManager>();
        this.soundManager = GetComponent<SoundManager>();
        this.blockManager = GetComponent<BlockManager>();
        this.noRotationManager = GetComponent<NoRotationManager>();
        this.highlightManager = GetComponent<HighlightManager>();
        this.playerMovement = FindObjectOfType<PlayerMovement>();
        this.player = FindObjectOfType<Player>();
		this.totalVeggies = FindObjectsOfType<Salt>().Length;
        Instantiate(cursorPrefab);
        this.topLeft = transform.FindChild("topLeft");
        this.bottomRight = transform.FindChild("bottomRight");
        SetupEdgeCollidersOnWorldBounds();
		if (!canEdit) {
			GameData.instance.AddLevel(Application.loadedLevel, totalVeggies);
		}
    }

    private void SetupEdgeCollidersOnWorldBounds()
    {
        float verticalExtentOfWorldBounds = (topLeft.transform.position.y - bottomRight.transform.position.y) * 2;
        GameObject leftEdge = new GameObject("leftEdge", typeof(EdgeCollider2D));
        leftEdge.transform.position = topLeft.transform.position;
        leftEdge.transform.parent = topLeft;
        leftEdge.layer = LayerMask.NameToLayer("Solid");
        leftEdge.transform.eulerAngles = new Vector3(0f, 0f, -90f);
        leftEdge.transform.localScale = new Vector3(verticalExtentOfWorldBounds, 1f);
        leftEdge.GetComponent<EdgeCollider2D>().sharedMaterial = noFrictionMaterial;
        GameObject rightEdge = new GameObject("rightEdge", typeof(EdgeCollider2D));
        rightEdge.transform.position = bottomRight.transform.position;
        rightEdge.transform.parent = bottomRight;
        rightEdge.layer = LayerMask.NameToLayer("Solid");
        rightEdge.transform.eulerAngles = new Vector3(0f, 0f, -90f);
        rightEdge.transform.localScale = new Vector3(verticalExtentOfWorldBounds, 1f);
        rightEdge.GetComponent<EdgeCollider2D>().sharedMaterial = noFrictionMaterial;
    }

    public void PlaySound(string soundName, float volume = 1f)
    {
        volume *= MusicManager.instance.fxVolume;
        soundManager.PlayClip(soundName, volume);
    }

    public bool MouseIsInRotatableArea()
    {
        return this.ValidCenterToClick(currentRotationCenter);
    }

    public bool MouseIsInNoRoZone()
    {
        return noRotationManager.hasNoRotationZone(currentRotationCenter);
    }

    IEnumerator ShakeObject(GameObject obj, float timeToShake, float shakeIntensity)
    {
        Vector3 originalPosition = obj.transform.position;
        for (float t = 0f; t <= timeToShake; t += Time.deltaTime)
        {
            obj.transform.position = new Vector3(
                obj.transform.position.x + Random.Range(-shakeIntensity, shakeIntensity),
                obj.transform.position.y + Random.Range(-shakeIntensity, shakeIntensity),
                obj.transform.position.z);
            yield return null;
        }
        obj.transform.position = originalPosition;
    }

    public void SetTopLeftPos(Vector2 pos)
    {
        this.topLeft.transform.position = pos;
        if (BoundsChanged != null)
        {
            BoundsChanged(this, topLeft, bottomRight);
        }
    }

    public void SetBottomRightPos(Vector2 pos)
    {
        this.bottomRight.transform.position = pos;
        if (BoundsChanged != null)
        {
            BoundsChanged(this, topLeft, bottomRight);
        }
    }

    void Update()
    {
        if (currentInput.escapePressed)
        {
            if (gameState != GameMode.paused)
            {
                lastState = gameState;
                gameState = GameMode.paused;
            }
            else
            {
                gameState = lastState;
            }
        }
        Time.timeScale = 1f;
        switch (gameState)
        {
            case GameMode.playing:
                {
                    CheckOutsideWorld();
                    this.currentRotationCenter = GetMousePosition();
					

                    if (Input.GetMouseButtonDown(0))
                    {
                        //print (worldPos.x+", "+worldPos.y);
                        //print (blockManager.getBlockAt(worldPos.x, worldPos.y));
                        if (ValidCenterToClick(currentRotationCenter))
                        {
                            PlaySound("EnterRotation");
                            gameState = GameMode.frozen;
                            rotationsSinceFreezing = 0;
                        }

                    }
                    break;
                }

            case GameMode.frozen: //game is frozen, left-click is held, but no rotation is happening
                {
                    if (!Input.GetMouseButton(0))
                    {
                        // We're exiting freeze mode
                        if (this.rotationClock <= 0f)
                        {
                            PlaySound("ExitRotation");
                            gameState = GameMode.playing;

                            // Check if we've actually rotated something
                            if (rotationsSinceFreezing % 4 != 0 && !rotationEmpty)
                            {
                                blockManager.DecrementCracked(blockManager.justRotated);
								rotations++;
                                UpdateSalt();
                            }
                            rotationsSinceFreezing = 0;
                        }
                    }
                }
                // If we're not already rotating
                if (rotationClock <= 0f && Input.GetMouseButton(0))
                {
                    // Rotate right!
                    if (currentInput.rightPressed)
                    {
                        if (blockManager.isValidRotation(currentRotationCenter, -1))
                        {
                            PlaySound("RotateRight");
                            blockManager.startRotation(currentRotationCenter);
                            rotationClock = 1f;
                            currentRotationDirection = -1;
                            rotationEmpty = blockManager.rotationEmpty();
                            gameState = GameMode.rotating;
                        }
                        else
                        {
                            ShakeHighlighting();
                        }
                    }
                    // Rotate left!
                    else if (currentInput.leftPressed)
                    {
                        if (blockManager.isValidRotation(currentRotationCenter, 1))
                        {
                            PlaySound("RotateLeft");
                            blockManager.startRotation(currentRotationCenter);
                            rotationClock = 1f;
                            currentRotationDirection = 1;
                            rotationEmpty = blockManager.rotationEmpty();
                            gameState = GameMode.rotating;
                        }
                        else
                        {
                            ShakeHighlighting();
                        }
                    }
                }

                break;

            case GameMode.rotating:
                {
                    //check rotation clock, if it's done set gameState back to RotationMode.frozen
                    if (rotationClock > 0)
                        rotationClock -= Time.deltaTime / secondsToRotate;
                    // If we're done rotating, finish the rotation
                    if (rotationClock <= 0)
                    {
                        blockManager.finishRotation(currentRotationCenter, currentRotationDirection);
                        rotationsSinceFreezing += currentRotationDirection;
                        if (Mathf.Abs(rotationsSinceFreezing) == 4)
                        {
                            rotationsSinceFreezing = 0;
                        }
                        gameState = GameMode.frozen;
                    }
                    // If we're not done rotating, then keep animating the rotation
                    else
                    {
                        blockManager.AnimateFrameOfRotation(currentRotationCenter, currentRotationDirection, 1f - rotationClock);
                    }
                    break;
                }

            case GameMode.won:
                {
                    resetClock -= Time.deltaTime;
                    if (resetClock <= 0f)
                    {
                        GoToNextLevel();
                    }
                    break;
                }
            case GameMode.lost:
                {
                    resetClock -= Time.deltaTime;
                    if (resetClock <= 0f)
                    {
                        ResetLevel();
                    }
                    break;
                }

            case GameMode.paused:
                {
                    Time.timeScale = 0f;
                    break;
                }
        }
    }

    public Int2 GetMousePosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.y);
        return new Int2(x, y);
    }

    private void CheckOutsideWorld()
    {
        if (player != null)
        {
            if (player.transform.position.x < topLeft.position.x ||
                player.transform.position.x > bottomRight.position.x ||
                player.transform.position.y > topLeft.position.y ||
                player.transform.position.y < bottomRight.position.y)
            {
                PlaySound("Fall");
                LoseLevel("Outside the world!");
            }
        }
    }

    private void ShakeHighlighting()
    {
        StartCoroutine(ShakeObject(highlightManager.rotationHighlight, 0.2f, 0.3f));
        PlaySound("Error");
    }

    private bool ValidCenterToClick(Int2 rotationCenter)
    {
        return playerMovement != null && player != null &&
                                    isValidCenter(rotationCenter) && playerMovement.isGrounded()
                                    && !playerMovement.beingShot && !playerInNoRoZone();
    }

    private void UpdateSalt()
    {
		Salt[] salt = FindObjectsOfType<Salt> ();
        for (int i = 0; i < salt.Length; i++)
        {
            Salt current = salt[i];
	        current.justRotated();
	        current.field.text = "" + current.rotationsBeforeRemove;
	        if (current.rotationsBeforeRemove == 0)
	        {
                PlaySound("VeggieDeath");
	            blockManager.grid.Remove(current.GetCurrentPosition());
	            Destroy(current.gameObject);
	        }
        }
    }

    public bool isValidCenter(Int2 xy)
    {
        if (xy == null)
            return false;

        if (noRotationManager.hasNoRotationZone(xy))
        {
            return false;
        }

        // Check that the given center is exactly two blocks away from the player
        if (blockManager.player == null)
            return true;
        Int2 playerPos = blockManager.player.GetRoundedPosition();
        int absDx = Mathf.Abs(xy.x - playerPos.x);
        int absDy = Mathf.Abs(xy.y - playerPos.y);
        if ((absDx <= 2 && absDy <= 2) && (absDx == 2 || absDy == 2 || (absDx == 0 && absDy == 0)))
        {
            return true;
        }
        return false;
    }

    public bool playerInNoRoZone()
    {
        if (blockManager.player == null)
            return false;
        Int2 playerPos = blockManager.player.GetRoundedPosition();
        if (noRotationManager.hasNoRotationZone(playerPos))
        {
            return true;
        }
        return false;
    }

    public void WinLevel(float winTime = -1f)
    {
        if (gameState == GameMode.playing)
        {
            if (winTime < 0f)
                resetClock = winOrLoseCountdownTime;
            else
                resetClock = winTime;
            gameState = GameMode.won;
            winAndLoseText.WinLevel();
			if (!canEdit) {
				GameData.instance.ChangeUnlockedLevel(Application.loadedLevel);
				GameData.instance.ChangeBestVeggies(Application.loadedLevel, veggiesFreed);
				GameData.instance.ChangeBestRotations(Application.loadedLevel, rotations);
			}
        }
    }

    public void LoseLevel(string reasonForLosing)
    {
        if (gameState == GameMode.playing)
        {
            resetClock = winOrLoseCountdownTime;
            gameState = GameMode.lost;
            player.FrenchFryify();
            winAndLoseText.LoseLevel(reasonForLosing);
        }
    }

    public void GoToNextLevel()
    {
        int loadedLevel = Application.loadedLevel;
        
        if (loadedLevel < Application.levelCount - 1)
        {
            Application.LoadLevel(loadedLevel + 1);
        }
        else
        {
            Application.LoadLevel(0);
        }
    }

    public void ResetLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

	public void addSalt() {
		veggiesFreed++;
	}

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.richText = true;
        int boxWidth = Screen.width / 4;
        int boxHeight = Screen.height / 8;
        switch (gameState)
        {
            default:
                {
					if (!canEdit) {
                    	GUILayout.BeginArea(new Rect(0, 0, boxWidth*2, boxHeight));
                    	GUILayout.Label("Veggies freed: " + veggiesFreed + "/" + totalVeggies + "\tRotations used: " + rotations);
                    	GUILayout.EndArea();
					}
                    break;
                }
            case GameMode.lost:
                {
                    break;
                }
            case GameMode.won:
                {
                    break;
                }
            case GameMode.paused:
                {
                    GUILayout.BeginArea(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "", "box");
                    if (GUILayout.Button("Return to Game"))
                    {
                        gameState = lastState;
                    }
                    if (GUILayout.Button("Main Menu"))
                    {
                        ReturnToMainMenu();
                    }
                    if (!canEdit)
                    {
                        if (GUILayout.Button("Restart Current Level"))
                        {
                            ResetLevel();
                        }
                    }
					if (GUILayout.Button ("Skip Level"))
					{
                        GameData.instance.ChangeUnlockedLevel(Application.loadedLevel);
						GoToNextLevel();
					}
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Music Volume ");
                    MusicManager.instance.musicVolume = GUILayout.HorizontalSlider(MusicManager.instance.musicVolume, 0f, 1f);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Game Volume ");
                    MusicManager.instance.fxVolume= GUILayout.HorizontalSlider(MusicManager.instance.fxVolume, 0f, 1f);
                    GUILayout.EndHorizontal();
                    //currentScroll = GUILayout.BeginScrollView(currentScroll);
                    //foreach(string campaignName in campaignManager.GetCampaigns())
                    //{
                    //    GUILayout.Label(campaignName);
                    //    foreach(string levelName in campaignManager.GetLevelsInCampaign(campaignName))
                    //    {
                    //        if (GUILayout.Button(levelName))
                    //        {
                    //            levelEditor.LoadLevelFromPath(levelName);
                    //        }
                    //    }
                    //}
                    //GUILayout.EndScrollView();
                    GUILayout.EndArea();
                    break;
                }
            case GameMode.editing:
                {
                    break;
                }
        }
    }

    public void ReturnToMainMenu()
    {
        Application.LoadLevel(1);
    }

}
