using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    GameManager gameManager;
    LevelEditor levelEditor;
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
        this.levelEditor = FindObjectOfType<LevelEditor>();
        player = FindObjectOfType<Player>();
        gameManager.PlayerCreated += this.PlayerCreated;
        gameManager.BoundsChanged += this.BoundsChanged;
	}

    void Start()
    {
        SetBounds(gameManager.topLeft, gameManager.bottomRight);
    }

    private void SetBounds(Transform topLeft, Transform bottomRight)
    {
        float vertExtent = Camera.main.camera.orthographicSize;
        float horzExtent = (vertExtent * Screen.width / Screen.height);
        leftBound = (float)(topLeft.position.x + horzExtent);
        rightBound = (float)(bottomRight.position.x - horzExtent);
        bottomBound = (float)(bottomRight.position.y + vertExtent);
        topBound = (float)(topLeft.position.y - vertExtent);
    }

    public void BoundsChanged(GameManager gm, Transform topLeft, Transform bottomRight)
    {
        this.SetBounds(topLeft, bottomRight);
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
        if (levelEditor.editorState != LevelEditor.EditorState.NewLevel)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, -10f);
            pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
            pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
            transform.position = pos;
        }
    }
}
