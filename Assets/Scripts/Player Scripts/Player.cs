using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioListener))]
public class Player : MonoBehaviour {
    GameManager gameManager;
	BlockManager blockManager;
    SpriteRenderer playerSprite;

    void Start()
    {
        gameManager.CreatePlayer(this, this.gameObject.GetComponent<PlayerMovement>());
    }

    void Awake()
    {

        this.playerSprite = transform.Find("playerSprite").GetComponent<SpriteRenderer>();
        if (playerSprite == null)
        {
            Debug.LogError("couldn't find player's sprite!");
        }
        this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
        
        // Get rid of all other audio listeners in the scene
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener al in listeners)
        {
            if (al.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
                Destroy(al);
        }
    }

    void Update()
    {
		Int2 position = this.GetRoundedPosition();
		Dictionary<Int2, AbstractBlock> grid = blockManager.grid;
        if (this.CrushedByBlock(grid, position))
        {
            gameManager.PlaySound("Burnt"); // yes i know it doesn't match
            gameManager.LoseLevel("Crushed by falling blocks");
        }
    }

	void OnCollisionStay2D(Collision2D coll) {
		if (coll.collider.gameObject.tag == "Sprite" && coll.collider.gameObject.transform.parent.gameObject.GetComponent<AbstractBlock>().heat >= 6f && gameManager.gameState == GameManager.GameMode.playing) {
			gameManager.PlaySound("Burnt");
			gameManager.LoseLevel("Burnt by a hot block");
		}
	}
	
	public void FrenchFryify()
    {
        Color starting = playerSprite.color;
        starting.a = 0f;
        playerSprite.color = starting;
        starting = transform.Find("frenchFries").GetComponent<SpriteRenderer>().color;
        starting.a = 1f;
        transform.Find("frenchFries").GetComponent<SpriteRenderer>().color = starting;
    }

    bool CrushedByBlock(Dictionary<Int2, AbstractBlock> grid, Int2 position)
    {
        Int2 above = new Int2 (position.x, position.y + 1);
	    Int2 below = new Int2 (position.x, position.y - 1);
        return (grid.ContainsKey(above) && grid[above] as FallingBlock != null && !(grid[above] as FallingBlock).whichHalf && 
		 	(!grid.ContainsKey(new Int2(above.x, above.y+1)) || grid[new Int2(above.x, above.y+1)] != grid[above]) && 
		 	grid.ContainsKey(below) && (grid[below] as FallingBlock == null || (grid[below] as FallingBlock).fallClock < 0.0f) && !gameManager.gameFrozen);
    }
    public Int2 GetRoundedPosition()
    {
        return new Int2(transform.position.x, transform.position.y);
    }
}
