using UnityEngine;
using System.Collections;

public class HighlightManager : MonoBehaviour {

    Player player;
    GameManager gameManager;
    public GameObject selectionHighlightPrefab;
    public GameObject rotationHighlightPrefab;
    GameObject selectionHighlight;
    GameObject rotationHighlight;

    void Awake()
    {
        this.gameManager = FindObjectOfType<GameManager>();
        selectionHighlight = Instantiate(selectionHighlightPrefab) as GameObject;
        if (selectionHighlight == null)
            print("selection highlight was null!");

        rotationHighlight = Instantiate(rotationHighlightPrefab) as GameObject;
        player = FindObjectOfType<Player>();
        
    }

    void Update()
    {
        switch (gameManager.gameState)
        {
            case GameManager.RotationMode.playing:
                {
                    selectionHighlight.SetActive(true);
                    rotationHighlight.SetActive(false);
                    selectionHighlight.transform.position = player.GetRoundedPosition().ToVector2();
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
        }
        
    }
}
