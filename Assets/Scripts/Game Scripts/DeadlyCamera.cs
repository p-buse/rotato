using UnityEngine;
using System.Collections;

public class DeadlyCamera : MonoBehaviour
{

    public Vector2 cameraMovement;
    public bool enableMovement;
    Player player;
    Collider2D playerCollider;
    GameManager gameManager;

    void Start()
    {
        this.player = FindObjectOfType<Player>(); ;
        this.playerCollider = this.player.GetComponent<Collider2D>();
        this.gameManager = FindObjectOfType<GameManager>();
        gameManager.PlayerCreated += this.PlayerCreated;
    }

    void PlayerCreated(GameManager gm, Player p, PlayerMovement pm)
    {
        this.player = p;
        this.playerCollider = p.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameFrozen && this.enableMovement)
        {
            transform.Translate(cameraMovement * Time.deltaTime);
        }
        Plane[] cameraView = GeometryUtility.CalculateFrustumPlanes(camera);
        // If the player is outside the frame, reset the level
        if (gameManager.gameState == GameManager.GameMode.playing && !GeometryUtility.TestPlanesAABB(cameraView, playerCollider.bounds))
        {
            gameManager.PlaySound("Fall");
            gameManager.LoseLevel("Fell out of the world");
        }
    }
}
