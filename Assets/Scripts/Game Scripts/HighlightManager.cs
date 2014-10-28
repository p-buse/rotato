using UnityEngine;
using System.Collections;

public class HighlightManager : MonoBehaviour {

    Player player;
    GameManager gameManager;
    public Color highlightColor;
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
    }

    void Update()
    {
        switch (gameManager.gameState)
        {
            case GameManager.RotationMode.playing:
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
            case GameManager.RotationMode.frozen:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(true);
                    rotationHighlight.transform.position = gameManager.currentRotationCenter.ToVector2();
                    break;
                }
            case GameManager.RotationMode.rotating:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(false);
                    break;
                }
            case GameManager.RotationMode.won:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(false);
                    break;
                }
            case GameManager.RotationMode.lost:
                {
                    selectionHighlight.SetActive(false);
                    rotationHighlight.SetActive(false);
                    break;
                }
        }
        
    }
}
