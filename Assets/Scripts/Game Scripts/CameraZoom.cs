using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {

    // Here's the game manager.
    // more stuff
	GameManager gameManager;
	Vector3 originalPosition;
    public bool zoomEnabled = false;
    public bool followPlayer = false;
	public float closeZoom = 2f;
	public float farZoom = 7f;
	public float zoomSpeed = 1f;
    Player player;

	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.originalPosition = transform.position;
        player = FindObjectOfType<Player>();
        gameManager.PlayerCreated += this.PlayerCreated;
	}

    void PlayerCreated(GameManager gm, Player p, PlayerMovement pm)
    {
        this.player = p;
    }

    void Update()
    {
        if (player != null)
        {
            if (zoomEnabled)
            {
                if (gameManager.gameFrozen)
                {
                    Int2 rotationCenter = gameManager.currentRotationCenter;
                    Vector3 newLocation = new Vector3(rotationCenter.x, rotationCenter.y, -10f);
                    transform.position = Vector3.Lerp(transform.position, newLocation, zoomSpeed * Time.deltaTime);
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, closeZoom, zoomSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, originalPosition, zoomSpeed * Time.deltaTime);
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, farZoom, zoomSpeed * Time.deltaTime);
                }
            }
            if (followPlayer)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -10f), zoomSpeed * Time.deltaTime);
            }
        }
    }
}
