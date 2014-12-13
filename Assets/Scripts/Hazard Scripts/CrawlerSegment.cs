using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlerSegment : MonoBehaviour {
	//not sure if these are necessary, but whatever
	GameManager gameManager;
	BlockManager blockManager;
	CrawlerMovement move;
	
	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
		this.move = transform.GetComponent<CrawlerMovement>();
	}
	
	void Update()
	{
		if(!gameManager.gameFrozen)
		{
			//Vector2 top = new Vector2(transform.position.x, transform.position.y)+0.1f*move.floatToV2(move.clinging);
			if (blockManager.getBlockAt (transform.position.x, transform.position.y)!=null) 
			{
//				print("block at "+transform.position.x+", "+transform.position.y);
//				print ("my last moving number: "+move.moving);
//				print ("my last moving vector: "+move.floatToV2(move.moving));
//				Debug.DrawLine(transform.position, transform.position + move.floatToV3(move.clinging));
				dieSafely();
			}
			if(move.myBlock!=null){
				if(move.myBlock.heated > 0)
				{
					dieSafely();
				}
			}
		}
		
	}
	
	void OnCollisionEnter2D(Collision2D coll) 
	{
		if (coll.gameObject.tag == "Player") 
		{
            gameManager.PlaySound("Crunch", 1.2f);
			//kill player
			gameManager.LoseLevel("Eaten by a Crawler!");
			
		}
		
	}
	
	/// <summary>
	/// Dies safely.
	/// </summary>
	public void dieSafely()
	{
        gameManager.PlaySound("CrawlerDeath", 0.2f);
		move.updateMyBlock(null);
		Destroy (this.gameObject);
	}
}
