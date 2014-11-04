using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlerSegment : MonoBehaviour {
	//not sure if these are necessary, but whatever
	GameManager gameManager;
	BlockManager blockManager;
	CrawlerMovement move;

	void Start()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
		this.move = transform.GetComponent<CrawlerMovement>();
	}
	
	void Update()
	{
		if(!gameManager.gameFrozen)
		{
			if (blockManager.getBlockAt (transform.position.x, transform.position.y)) 
			{
				dieSafely();
			}
			if(move.myBlock!=null){
				if(move.myBlock.heat>=6)
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
			//kill player
			gameManager.LoseLevel("Eaten by a Crawler!");
			
		}
		
	}

	/// <summary>
	/// Dies safely.
	/// </summary>
	public void dieSafely()
	{
		move.updateMyBlock(null);
		Destroy (this.gameObject);
	}
}
