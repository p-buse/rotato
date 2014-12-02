using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    struct CapturedInput
    {
        public bool left;
        public bool right;
        public bool jumpPressed;
        public bool jumpHeld;
    }
    CapturedInput currentInput;
    public float groundMoveSpeed = 1f;
    public float airMoveSpeed = 2f;
    public float jumpAcceleration = 8f;
    public float groundFriction = 10f;
    public float airFriction = 10f;
    public float maxHorizontalSpeed = 10f;
    public float gravityScale = 1f;
    public AnimationCurve jumpCurve;
    public int maxJumpTicks = 12;
    private int currentJumpTicks = 0;
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

    CapturedInput GetInput()
    {
        CapturedInput inputFrame = new CapturedInput();
        float horizInput = Input.GetAxis("Horizontal");
        if (horizInput > 0)
        {
            inputFrame.right = true;
            inputFrame.left = false;
        }
        else if (horizInput < 0)
        {
            inputFrame.left = true;
            inputFrame.right = false;
        }
        else
        {
            inputFrame.left = false;
            inputFrame.right = false;
        }
        inputFrame.jumpPressed = Input.GetButtonDown("Vertical");
        inputFrame.jumpHeld = Input.GetButton("Vertical");
        return inputFrame;
    }
	
	void Update ()
    {
        currentInput = GetInput();
        if (currentInput.right)
        {
            AddPushRight();
        }
        else if (currentInput.left)
        {
            AddPushLeft();
        }
        else
        {
            ResetPush();
        }
        if (jumping)
        {
            if (!currentInput.jumpHeld)
            {
                jumping = false;
            }
        }

        if (currentInput.jumpPressed && grounded)
        {
            gameManager.PlaySound("Jump");
            this.jumping = true;
            this.currentJumpTicks = 0;
        }
        
	}

    void FixedUpdate()
    {
        this.grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Solid"));
        if (!gameManager.gameFrozen)
        {
            rigidbody2D.gravityScale = this.gravityScale;
            if (!conservedMovement.Equals(Vector2.zero))
            {
                rigidbody2D.velocity = conservedMovement;
                conservedMovement = Vector2.zero;
            }
            if (!beingShot)
            {
                if (currentInput.left)
                {
                    if (grounded)
                    {
                        //rigidbody2D.velocity.AddForce(-groundMoveSpeed, 0f);
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x - groundMoveSpeed, rigidbody2D.velocity.y);
                    }
                    else
                    {
                        //rigidbody2D.AddForce(new Vector2(-airMoveSpeed, 0f)); 
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x - airMoveSpeed, rigidbody2D.velocity.y);
                    }
                }
                if (currentInput.right)
                {
                    if (grounded)
                    {
                        //rigidbody2D.AddForce(new Vector2(groundMoveSpeed, 0f));
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x + groundMoveSpeed, rigidbody2D.velocity.y);
                    }
                    else
                    {
                        //rigidbody2D.AddForce(new Vector2(-airMoveSpeed, 0f)); 
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x + airMoveSpeed, rigidbody2D.velocity.y);
                    }
                }
                if (grounded)
                {
                    rigidbody2D.velocity = new Vector2(Mathf.Lerp(rigidbody2D.velocity.x, 0f, Time.deltaTime * groundFriction), rigidbody2D.velocity.y);
                }
                else
                {
                    rigidbody2D.velocity = new Vector2(Mathf.Lerp(rigidbody2D.velocity.x, 0f, Time.deltaTime * airFriction), rigidbody2D.velocity.y);
                }
                rigidbody2D.velocity = new Vector2(Mathf.Clamp(rigidbody2D.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed), rigidbody2D.velocity.y);
                if (jumping == true)
                {
                    currentJumpTicks += 1;
                    if (currentJumpTicks >= maxJumpTicks)
                    {
                        jumping = false;
                    }
                    else
                    {
                        //rigidbody2D.AddForce(new Vector2(0f, jumpAcceleration * (maxJumpTicks - currentJumpTicks))); // "Scaled" jump
                        //rigidbody2D.AddForce(new Vector2(0f, jumpAcceleration)); // "Accelerate" jump
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpAcceleration); //"Linear" jump
                    }
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
