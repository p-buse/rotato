using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonBlock : AbstractBlock
{
    static PlayerMovement playerMovement;
    public float shootDelayTime = 1f;
    public float shootCooldownTime = 1f;
    [SerializeField]
    float shootCoolDown = 0f;
    public float shootForce = 300f;
    [SerializeField]
    float shotClock = 0f;
    bool shootPlayer = false;
    static Dictionary<int, Vector2> orientationToShootVectors = new Dictionary<int, Vector2>()
    {
        {0, new Vector2(1, 1)},
        {1, new Vector2(-1, 1)},
        {2, new Vector2(-1, -1)},
        {3, new Vector2(1, -1)}
    };

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (shootCoolDown > 0f)
        {
            shootCoolDown -= Time.deltaTime;
        }
        // We're waiting to shoot the player
        if (this.shotClock > 0f)
        {
            //Hold the player in the cannon
            playerMovement.transform.position = this.transform.position;
            // Count down to our shot
            shotClock -= Time.deltaTime;
            // We've reached shot time!
            if (shotClock <= 0f)
            {
                // Fire the player at the next FixedUpdate
                this.shootPlayer = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (shootPlayer)
        {
            ShootPlayer();
            shootPlayer = false;
        }
    }

    public void ShootPlayer()
    {
        Vector2 shotVector = orientationToShootVectors[this.orientation] * shootForce;
        playerMovement.rigidbody2D.AddForce(shotVector);
        shootCoolDown = shootCooldownTime;
    }

    public override bool invalidatesRotation()
    {
        return false;
    }

    public override bool isRotatable()
    {
        return true;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!gameManager.gameFrozen)
        {
            // If the player's not already in the cannon and we've cooled down and the thing we touched is a player
            if (shotClock <= 0f && shootCoolDown <= 0f && coll.gameObject.tag == "Player")
            {
                // Player enters the cannon
                shotClock = shootDelayTime;
                playerMovement.beingShot = true;
            }
        }
    }
}
