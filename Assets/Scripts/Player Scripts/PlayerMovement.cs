﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 1f;
    public float jumpForce = 600f;

    GameManager gameManager;

    bool jumping = false;
    float horizontalVelocity = 0f;
    Transform groundCheck;

    void Awake()
    {
        this.gameManager = FindObjectOfType<GameManager>();
        this.groundCheck = transform.Find("groundCheck");
    }
	
	void Update ()
    {
        float horizInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        if (horizInput > 0)
            this.horizontalVelocity = moveSpeed;
        else if (horizInput < 0)
            this.horizontalVelocity = -moveSpeed;
        else
            this.horizontalVelocity = 0f;
        if (vertInput > 0)
            jumping = true;
        else
            jumping = false;
	}

    void FixedUpdate()
    {
        if (!gameManager.gameFrozen)
        {
            rigidbody2D.gravityScale = 1f;
            rigidbody2D.velocity = new Vector2(horizontalVelocity, rigidbody2D.velocity.y);
            if (jumping == true && isGrounded())
            {
                rigidbody2D.AddForce(new Vector2(0f, jumpForce));
                jumping = false;
            }
        }
        else
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.gravityScale = 0f;
        }
    }

    bool isGrounded()
    {
        return Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Solid"));
    }
}