using UnityEngine;
using System.Collections;

public class DeadlyCamera : MonoBehaviour
{

    public Vector2 cameraMovement;
    public bool enableMovement;
    Collider2D playerCollider;
    GameManager gameManager;

    void Start()
    {
        this.playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        this.gameManager = FindObjectOfType<GameManager>();
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
        if (gameManager.gameState == GameManager.RotationMode.playing && !GeometryUtility.TestPlanesAABB(cameraView, playerCollider.bounds))
        {
            gameManager.PlaySound("Fall");
            gameManager.LoseLevel("Fell out of the world");
        }
    }
}
