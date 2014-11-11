using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Reset Key
    public KeyCode resetKey = KeyCode.Escape;

    // References to other stuff
    BlockManager blockManager;
    NoRotationManager noRotationManager;
    SoundManager soundManager;
    [HideInInspector]
    public PlayerMovement playerMovement;
    [HideInInspector]
    public Player player;

    // Salt stuff
    [HideInInspector]
    public Salt[] salt;
    [HideInInspector]
    public int saltSoFar = 0;

    // Winning and losing
    public float winOrLoseCountdownTime = 2f;
    float resetClock = 0f;
    string reasonForLosing = "";

    // Rotation stuff
    public float secondsToRotate = 1f;
    [HideInInspector]
    public Int2 currentRotationCenter;
    int currentRotationDirection = 0;
    int rotationsSinceFreezing = 0;
    bool rotationEmpty;
    float rotationClock = 0f;

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
    public enum GameMode { playing, frozen, rotating, won, lost, editing };
    //[HideInInspector]
    public GameMode gameState = GameMode.playing;
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

    void Awake()
    {
        this.soundManager = GetComponent<SoundManager>();
        this.blockManager = GetComponent<BlockManager>();
        this.noRotationManager = GetComponent<NoRotationManager>();

        this.salt = GameObject.FindObjectsOfType<Salt>();
        this.playerMovement = FindObjectOfType<PlayerMovement>();
        this.player = FindObjectOfType<Player>();
    }

    public void PlaySound(string soundName)
    {
        soundManager.PlayClip(soundName);
    }

    void Update()
    {
        if (Input.GetKey(resetKey))
        {
            ResetLevel();
        }
        switch (gameState)
        {
            case GameMode.playing:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        int x = Mathf.RoundToInt(worldPos.x);
                        int y = Mathf.RoundToInt(worldPos.y);
                        this.currentRotationCenter = new Int2(x, y);
                        if (playerMovement != null && player != null &&
                            isValidCenter(currentRotationCenter) && playerMovement.isGrounded() 
                            && !playerMovement.beingShot && !playerInNoRoZone())
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
                                UpdateSalt();
                            }
                            rotationsSinceFreezing = 0;
                        }
                    }
                }
                // If we're not already rotating
                if (rotationClock <= 0f)
                {
                    // Rotate right!
                    if (Input.GetAxis("Horizontal") > 0 && blockManager.isValidRotation(currentRotationCenter, -1))
                    {
                        PlaySound("RotateRight");
                        blockManager.startRotation(currentRotationCenter);
                        rotationClock = 1f;
                        currentRotationDirection = -1;
                        rotationEmpty = blockManager.rotationEmpty();
                        gameState = GameMode.rotating;
                    }
                    // Rotate left!
                    else if (Input.GetAxis("Horizontal") < 0 && blockManager.isValidRotation(currentRotationCenter, 1))
                    {
                        PlaySound("RotateLeft");
                        blockManager.startRotation(currentRotationCenter);
                        rotationClock = 1f;
                        currentRotationDirection = 1;
                        rotationEmpty = blockManager.rotationEmpty();
                        gameState = GameMode.rotating;
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

            case GameMode.editing:
                {
                    // The LevelEditor script will detect this and knows what to do
                    break;
                }

        }
    }

    private void UpdateSalt()
    {
        for (int i = 0; i < salt.Length; i++)
        {
            Salt current = salt[i];
            if (current != null)
            {
                current.rotationsBeforeRemove--;
                current.field.text = "" + current.rotationsBeforeRemove;
                if (current.rotationsBeforeRemove == 0)
                {
                    blockManager.grid.Remove(current.GetCurrentPosition());
                    Destroy(current.gameObject);
                    salt[i] = null;
                }
            }

        }
    }

    public bool isValidCenter(Int2 xy)
    {

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

    public void WinLevel()
    {
        if (gameState == GameMode.playing)
        {
            resetClock = winOrLoseCountdownTime;
            gameState = GameMode.won;
        }
    }

    public void LoseLevel(string reasonForLosing)
    {
        if (gameState == GameMode.playing)
        {
            this.reasonForLosing = reasonForLosing;
            resetClock = winOrLoseCountdownTime;
            gameState = GameMode.lost;
            player.FrenchFryify();
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

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.richText = true;
        int boxWidth = Screen.width / 4;
        int boxHeight = Screen.height / 8;
        switch (gameState)
        {
            case GameMode.playing:
                {
                    GUILayout.BeginArea(new Rect(0, 0, boxWidth, boxHeight));
                    GUILayout.Label("Salt: " + saltSoFar);
                    GUILayout.EndArea();
                    break;
                }
            case GameMode.lost:
                {
                    GUI.Label(new Rect(0, 0, boxWidth, boxHeight), "<size=" + boxHeight + ">YOU LOSE!!!</size>", style);
                    GUI.Label(new Rect(0,boxHeight, boxWidth, boxHeight * 2), "<size=" + boxHeight + ">" + reasonForLosing + "</size>", style);
                    break;
                }
            case GameMode.won:
                {
                    GUI.Label(new Rect(0, 0, boxWidth, boxHeight), "<size=" + boxHeight + ">YOU WIN!!!</size>", style);
                    break;
                }
        }
    }

}
