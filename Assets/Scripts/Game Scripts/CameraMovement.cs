using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    GameManager gameManager;
    public bool fixedMovement;
    public Vector2 cameraMovement;
	public float followSpeed = 2f;
    Player player;

    private float rightBound;
 private float leftBound;
 private float topBound;
 private float bottomBound;

	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
        gameManager.PlayerCreated += this.PlayerCreated;
	}

    void Start()
    {
     float vertExtent = Camera.main.camera.orthographicSize / 2;  
     float horzExtent = (vertExtent * Screen.width / Screen.height) / 2;
     leftBound = (float)(gameManager.topLeft.position.x + horzExtent);
     rightBound = (float)(gameManager.bottomRight.position.x - horzExtent);
     bottomBound = (float)(gameManager.bottomRight.position.y + vertExtent);
     topBound = (float)(gameManager.topLeft.position.y - vertExtent);
    }


    void PlayerCreated(GameManager gm, Player p, PlayerMovement pm)
    {
        this.player = p;
    }

    void Update()
    {
        if (player != null && !gameManager.gameFrozen && !this.fixedMovement)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                -10f), followSpeed * Time.deltaTime);
        }
        else if (!gameManager.gameFrozen && this.fixedMovement)
        {
            transform.Translate(cameraMovement * Time.deltaTime);
        }
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, -10f);
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        transform.position = pos;
    }
}
