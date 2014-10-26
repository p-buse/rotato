﻿using UnityEngine;
using System.Collections;

public class SpikyBlock : MonoBehaviour
{
    static GameManager gameManager;

    void Start()
    {
        SpikyBlock.gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("couldn't find game manager");
        }
        // Set our parent to be the block sprite
        transform.parent.parent = transform.parent.parent.Find("blockSprite");
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!gameManager.gameFrozen)
        {
            // If the player touches the spikes, skewer 'em! 
            if (coll.gameObject.tag == "Player")
            {
                this.SkewerPlayer();
            }
        }
    }

    public void SkewerPlayer()
    {
        gameManager.ResetLevel();
    }
}