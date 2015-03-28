using UnityEngine;
using System.Collections;
using System;

public class CameraMovement : MonoBehaviour
{
    GameManager gameManager;
    public bool fixedMovement;
    public Vector2 cameraMovement;
    public float followSpeed = 2f;
    Player player;

    private float rightBound;
    private float leftBound;
    private float topBound;
    private float bottomBound;

    Transform topLeft;
    Transform bottomRight;

    void Awake()
    {
        this.gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
        gameManager.PlayerCreated += this.PlayerCreated;
        gameManager.BoundsChanged += this.BoundsChanged;
    }

    void Start()
    {
        BoundsChanged(gameManager, gameManager.topLeft, gameManager.bottomRight);
    }

    private void SetBounds()
    {
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = (vertExtent * Screen.width / Screen.height);
        leftBound = (float)(topLeft.position.x + horzExtent);
        rightBound = (float)(bottomRight.position.x - horzExtent);
        bottomBound = (float)(bottomRight.position.y + vertExtent);
        topBound = (float)(topLeft.position.y - vertExtent);
    }

    public void BoundsChanged(GameManager gm, Transform topLeft, Transform bottomRight)
    {
        this.topLeft = topLeft;
        this.bottomRight = bottomRight;
        this.SetBounds();
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
        ClampPosition();
    }

    private void ClampPosition()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, -10f);
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        transform.position = pos;
    }

    public void PanCamera(Vector2 panVector)
    {
        transform.position += new Vector3(panVector.x, panVector.y, 0f);
        ClampPosition();
    }

    public void ZoomCamera(float zoomDelta)
    {
        // Hypothetical new extents
        float vertExtent = Camera.main.orthographicSize + zoomDelta;
        float horzExtent = (vertExtent * Screen.width / Screen.height);
        if (vertExtent*2 < topLeft.position.y - bottomRight.position.y && vertExtent >= 1f &&
            horzExtent*2 < bottomRight.position.x - topLeft.position.x && horzExtent >= 1f)
        {
            Camera.main.orthographicSize += zoomDelta;
            SetBounds();
        }
    }
}
