using UnityEngine;
using System.Collections;

public class HighlightManager : MonoBehaviour {

    Player player;
    GameManager gameManager;
    public Color highlightColor;

    // Highlight pulsing
    public float highlightPulseStep = 0.01f;
    public float highlightPulseMax = .3f;
    public float highlightPulseMin = .15f;
    /// <summary>
    /// flips between 1 and -1 to make the pulsing go up and down
    /// </summary>
    float highlightMultiplier = 1f;
    PlayerMovement playerMovement;
    public GameObject selectionHighlightPrefab;
    public GameObject rotationHighlightPrefab;
    GameObject selectionHighlight;
    SpriteRenderer[] selectionSquares;
    GameObject rotationHighlight;

    void Awake()
    {
        this.gameManager = FindObjectOfType<GameManager>();
        selectionHighlight = Instantiate(selectionHighlightPrefab) as GameObject;
        if (selectionHighlight == null)
            print("selection highlight was null!");
        selectionSquares = selectionHighlight.GetComponentsInChildren<SpriteRenderer>();

        rotationHighlight = Instantiate(rotationHighlightPrefab) as GameObject;
        player = FindObjectOfType<Player>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        gameManager.PlayerCreated += this.PlayerCreated;
    }

    void PlayerCreated(GameManager gm, Player p, PlayerMovement pm)
    {
        this.player = p;
        this.playerMovement = pm;
    }

    void Update()
    {
        PulseHighlightColor();
        if (player == null || playerMovement == null)
            return;
        switch (gameManager.gameState)
        {
            case GameManager.GameMode.playing:
                {
                    if (gameManager.playerInNoRoZone() || playerMovement.beingShot || !playerMovement.isGrounded())
                    {
                        selectionHighlight.SetActive(false);
                    }
                    else
                    {
                        selectionHighlight.SetActive(true);
                    }
                    rotationHighlight.SetActive(false);
                    selectionHighlight.transform.position = player.GetRoundedPosition().ToVector2();
                    // Highlight each square individually based on whether it's a valid center of rotation
                    foreach (SpriteRenderer square in selectionSquares)
                    {
                        Int2 squarePos = new Int2(square.transform.position.x, square.transform.position.y);
                        if (!gameManager.isValidCenter(squarePos))
                        {
                            square.color = Color.clear;
                        }
                        else
                        {
                            square.color = highlightColor;
                        }
                    }
                    break;
                }
            case GameManager.GameMode.frozen:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(true);
                    rotationHighlight.transform.position = gameManager.currentRotationCenter.ToVector2();
                    break;
                }
            case GameManager.GameMode.rotating:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(false);
                    break;
                }
            case GameManager.GameMode.won:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(false);
                    break;
                }
            case GameManager.GameMode.lost:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(false);
                    break;
                }
            case GameManager.GameMode.editing:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(false);
                    break;
                }
        }
        
    }

    private void PulseHighlightColor()
    {
        float currentAlphaValue = highlightColor.a;
        if (currentAlphaValue > highlightPulseMax)
        {
            highlightMultiplier = -1f;
        }
        else if (currentAlphaValue < highlightPulseMin)
        {
            highlightMultiplier = 1f;
        }
        float newAlphaValue = currentAlphaValue + (highlightPulseStep * highlightMultiplier);
        this.highlightColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, newAlphaValue);
    }
}
