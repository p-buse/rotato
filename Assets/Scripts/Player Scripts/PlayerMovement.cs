using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 1f;
    public float jumpForce = 600f;
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
    }
	
	void Update ()
    {
        float horizInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        if (horizInput > 0) {
			this.horizontalVelocity = moveSpeed;

			AbstractBlock rightNeighbor;
			if (blockManager.grid.TryGetValue(new Int2(transform.position.x+1, transform.position.y), out rightNeighbor) && rightNeighbor as  FallingBlock != null
			    && !blockManager.grid.ContainsKey(new Int2 (transform.position.x+2, transform.position.y))) {
				FallingBlock rightNeighborFalling = (rightNeighbor as FallingBlock);
				if (rightNeighborFalling.isRotatable()) {
					rightNeighborFalling.initiatePush += Time.deltaTime;
					rightNeighborFalling.pushDirection = 1;
				}
			}
		}
        else if (horizInput < 0) {
            this.horizontalVelocity = -moveSpeed;

			AbstractBlock leftNeighbor;
			if (blockManager.grid.TryGetValue(new Int2(transform.position.x-1, transform.position.y), out leftNeighbor) && leftNeighbor as  FallingBlock != null
			    && !blockManager.grid.ContainsKey(new Int2 (transform.position.x-2, transform.position.y))) {
				FallingBlock leftNeighborFalling = (leftNeighbor as FallingBlock);
				if (leftNeighborFalling.isRotatable()) {
					leftNeighborFalling.initiatePush += Time.deltaTime;
					leftNeighborFalling.pushDirection = -1;
				}
			}
		}
        else {
            this.horizontalVelocity = 0f;

			AbstractBlock rightNeighbor;
			AbstractBlock leftNeighbor;
			if (blockManager.grid.TryGetValue (new Int2(transform.position.x + 1, transform.position.y), out rightNeighbor) && rightNeighbor as FallingBlock != null) {
				(rightNeighbor as FallingBlock).initiatePush = 0f;
			}
			if (blockManager.grid.TryGetValue (new Int2(transform.position.x - 1, transform.position.y), out leftNeighbor) && leftNeighbor as FallingBlock != null) {
				(leftNeighbor as FallingBlock).initiatePush = 0f;
			}
		}
        if (vertInput > 0 && isGrounded() && !beingShot)
            jumping = true;
        else
            jumping = false;
	}

    void FixedUpdate()
    {
        if (!gameManager.gameFrozen)
        {
            if (!beingShot)
            {
                rigidbody2D.gravityScale = 1f;
                rigidbody2D.velocity = new Vector2(horizontalVelocity, rigidbody2D.velocity.y);
                if (jumping == true && isGrounded())
                {
                    rigidbody2D.AddForce(new Vector2(0f, jumpForce));
                    gameManager.PlaySound("Jump");
                    jumping = false;
                }
            }
        }
        else
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.gravityScale = 0f;
        }
    }

    public bool isGrounded()
    {
        return Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Solid"));
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
