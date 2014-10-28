using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float secondsToRotate = 1f;
    public KeyCode rotateRightKey = KeyCode.E;
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode resetKey = KeyCode.R;

    BlockManager blockManager;
    NoRotationManager noRotationManager;
    SoundManager soundManager;
    PlayerMovement playerMovement;
    [HideInInspector]
    public Salt[] salt;

    public float winOrLoseCountdownTime = 2f;
    float resetClock = 0f;
    string reasonForLosing = "";

    public Int2 currentRotationCenter;
    int currentRotationDirection = 0;
    int rotationsSinceFreezing = 0;
    
    [HideInInspector]
    public int saltSoFar = 0;
    bool rotationEmpty;
    float rotationClock = 0f;
    public enum RotationMode { playing, frozen, rotating, won, lost };
    [HideInInspector]
    public RotationMode gameState = RotationMode.playing;
    

    public bool gameFrozen
    {
        get
        {
            if (gameState == RotationMode.playing)
                return false;
            else
                return true;
        }
    }

    void Awake()
    {
        this.soundManager = FindObjectOfType<SoundManager>();
        this.blockManager = FindObjectOfType<BlockManager>();
        this.playerMovement = FindObjectOfType<PlayerMovement>();
        this.salt = GameObject.FindObjectsOfType<Salt>();
        this.noRotationManager = FindObjectOfType<NoRotationManager>();
    }

    public void PlaySound(string soundName)
    {
        soundManager.PlayClip(soundName);
    }


    public void RegisterClick(float clickx, float clicky)
    {
    }

    void Update()
    {
        if (Input.GetKey(resetKey))
        {
            ResetLevel();
        }
        switch (gameState)
        {
            case RotationMode.playing:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        int x = Mathf.RoundToInt(worldPos.x);
                        int y = Mathf.RoundToInt(worldPos.y);
                        this.currentRotationCenter = new Int2(x, y);
                        if (isValidCenter(currentRotationCenter) && playerMovement.isGrounded() && !playerMovement.beingShot && !playerInNoRoZone())
                        {
                            PlaySound("EnterRotation");
                            gameState = RotationMode.frozen;
                            rotationsSinceFreezing = 0;
                        }

                    }
                    break;
                }

            case RotationMode.frozen: //game is frozen, left-click is held, but no rotation is happening
                {
                    if (!Input.GetMouseButton(0))
                    {
                        if (this.rotationClock <= 0f)
                        {
                            {
                                PlaySound("ExitRotation");
                                gameState = RotationMode.playing;
                                if (rotationsSinceFreezing % 4 != 0)
								blockManager.handleCracked(blockManager.justRotated);
                                }
                                if (rotationsSinceFreezing != 0 && !rotationEmpty)
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
                                rotationsSinceFreezing = 0;
                            }
                        }
                    }
                    // If we're not already rotating
                    if (rotationClock <= 0f)
                    {
                        // Rotate right!
                        if (Input.GetKey(rotateRightKey) && blockManager.isValidRotation(currentRotationCenter, -1))
                        {
                            PlaySound("RotateRight");
                            blockManager.startRotation(currentRotationCenter);
                            rotationsSinceFreezing -= 1;
                            rotationClock = 1f;
                            currentRotationDirection = -1;
                            rotationEmpty = blockManager.rotationEmpty();
                            gameState = RotationMode.rotating;
                        }
                        // Rotate left!
                        else if (Input.GetKey(rotateLeftKey) && blockManager.isValidRotation(currentRotationCenter, 1))
                        {
                            PlaySound("RotateLeft");
                            blockManager.startRotation(currentRotationCenter);
                            rotationsSinceFreezing += 1;
                            rotationClock = 1f;
                            currentRotationDirection = 1;
                            rotationEmpty = blockManager.rotationEmpty();
                            gameState = RotationMode.rotating;
                        }
                    }

                    break;

            case RotationMode.rotating:
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
                        gameState = RotationMode.frozen;
                    }
                    else
                    {
                        blockManager.AnimateFrameOfRotation(currentRotationCenter, currentRotationDirection, 1f - rotationClock);
                    }
                    break;
                }

            case RotationMode.won:
                {
                    resetClock -= Time.deltaTime;
                    if (resetClock <= 0f)
                    {
                        GoToNextLevel();
                    }
                    break;
                }
            case RotationMode.lost:
                {
                    resetClock -= Time.deltaTime;
                    if (resetClock <= 0f)
                    {
                        ResetLevel();
                    }
                    break;
                }

        }
    }

    public bool isValidCenter(Int2 xy)
    {

        if (noRotationManager.hasNoRotationZone(xy))
        {
            return false;
        }

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

        Int2 playerPos = blockManager.player.GetRoundedPosition();
        if (noRotationManager.hasNoRotationZone(playerPos))
        {
            return true;
        }
        return false;

    }

    public void WinLevel()
    {
        if (gameState == RotationMode.playing)
        {
            playerMovement.enabled = false;
            resetClock = winOrLoseCountdownTime;
            gameState = RotationMode.won;
        }
    }

    public void LoseLevel(string reasonForLosing)
    {
        if (gameState == RotationMode.playing)
        {
            this.reasonForLosing = reasonForLosing;
            playerMovement.enabled = false;
            resetClock = winOrLoseCountdownTime;
            gameState = RotationMode.lost;
        }
    }

    void GoToNextLevel()
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
        switch (gameState)
        {
            case RotationMode.playing:
                {
                    GUI.Label(new Rect(0, -5, 100, 50), "Salt: " + saltSoFar);
                    break;
                }
            case RotationMode.lost:
                {
                    GUI.Label(new Rect(Screen.width/2,Screen.height/2 - 200,200,200),"<size=100>YOU LOSE!!!</size>", style);
                    GUI.Label(new Rect(Screen.width/2, Screen.height/2, 200, 200),"<size=30>" + reasonForLosing + "</size>", style);
                    break;
                }
            case RotationMode.won:
                {
                    GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 - 200, 200, 200), "<size=100>You WIN!!</size>", style);
                    break;
                }
        }
    }

}
