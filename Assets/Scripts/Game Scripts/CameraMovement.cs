using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    // Here's the game manager.
    // more stuff
	GameManager gameManager;
    public bool fixedMovement;
    public Vector2 cameraMovement;
	public float followSpeed = 2f;
    Player player;

	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
        gameManager.PlayerCreated += this.PlayerCreated;
	}

    void PlayerCreated(GameManager gm, Player p, PlayerMovement pm)
    {
        this.player = p;
    }

    void Update()
    {
        if (player != null && !this.fixedMovement)
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
    }
}
