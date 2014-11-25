using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 1f;
    public float jumpForce = 600f;
    private Vector2 conservedMovement;
    public bool grounded;
    private bool _beingShot = false;
    public bool beingShot
    {
        get
        {
            return _beingShot;
        }
        set
        {
            _beingShot = value;
        }
    }

    GameManager gameManager;
	BlockManager blockManager;

    bool jumping = false;
    float horizontalVelocity = 0f;
    Transform groundCheck;

    void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
        this.groundCheck = transform.Find("groundCheck");
        this.conservedMovement = Vector2.zero;
    }
	
	void Update ()
    {
        float horizInput = Input.GetAxis("Horizontal");
        if (horizInput > 0) {
			this.horizontalVelocity = moveSpeed;

            AddPushRight();
		}
        else if (horizInput < 0) {
            this.horizontalVelocity = -moveSpeed;

            AddPushLeft();
		}
        else {
            this.horizontalVelocity = 0f;

            ResetPush();
		}
        this.grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Solid"));
        if (Input.GetButtonDown("Vertical") && grounded && !gameManager.gameFrozen)
        {
            this.jumping = true;
        }
	}

    private void ResetPush()
    {
        AbstractBlock rightNeighbor;
        AbstractBlock leftNeighbor;
        if (blockManager.grid.TryGetValue(new Int2(transform.position.x + 1, transform.position.y), out rightNeighbor) && rightNeighbor as FallingBlock != null)
        {
            (rightNeighbor as FallingBlock).initiatePush = 0f;
        }
        if (blockManager.grid.TryGetValue(new Int2(transform.position.x - 1, transform.position.y), out leftNeighbor) && leftNeighbor as FallingBlock != null)
        {
            (leftNeighbor as FallingBlock).initiatePush = 0f;
        }
    }

    private void AddPushLeft()
    {
        AbstractBlock leftNeighbor;
        if (blockManager.grid.TryGetValue(new Int2(transform.position.x - 1, transform.position.y), out leftNeighbor) && leftNeighbor as FallingBlock != null
            && !blockManager.grid.ContainsKey(new Int2(transform.position.x - 2, transform.position.y)))
        {
            FallingBlock leftNeighborFalling = (leftNeighbor as FallingBlock);
            if (leftNeighborFalling.isRotatable())
            {
                leftNeighborFalling.initiatePush += Time.deltaTime;
                leftNeighborFalling.pushDirection = -1;
            }
        }
    }

    private void AddPushRight()
    {
        AbstractBlock rightNeighbor;
        if (blockManager.grid.TryGetValue(new Int2(transform.position.x + 1, transform.position.y), out rightNeighbor) && rightNeighbor as FallingBlock != null
            && !blockManager.grid.ContainsKey(new Int2(transform.position.x + 2, transform.position.y)))
        {
            FallingBlock rightNeighborFalling = (rightNeighbor as FallingBlock);
            if (rightNeighborFalling.isRotatable())
            {
                rightNeighborFalling.initiatePush += Time.deltaTime;
                rightNeighborFalling.pushDirection = 1;
            }
        }
    }

    void FixedUpdate()
    {
        if (!gameManager.gameFrozen)
        {
            rigidbody2D.gravityScale = 1f;
            if (!conservedMovement.Equals(Vector2.zero))
            {
                rigidbody2D.velocity = conservedMovement;
                conservedMovement = Vector2.zero;
            }
            if (!beingShot)
            {
                rigidbody2D.velocity = new Vector2(horizontalVelocity, rigidbody2D.velocity.y);
                if (jumping == true)
                {
                    rigidbody2D.AddForce(new Vector2(0f, jumpForce));
                    gameManager.PlaySound("Jump");
                    jumping = false;
                }
            }
        }
        else
        {
            if (conservedMovement.Equals(Vector2.zero))
            {
                conservedMovement = rigidbody2D.velocity;
            }
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.gravityScale = 0f;
        }
    }

    public bool isGrounded()
    {
        return this.grounded;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (this.beingShot && coll.gameObject.tag == "Block")
        {
            gameManager.PlaySound("HitWall");
            this.beingShot = false;
        }
    }
}
