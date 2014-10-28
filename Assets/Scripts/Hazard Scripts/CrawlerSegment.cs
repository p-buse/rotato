using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlerSegment : MonoBehaviour {
	//not sure if these are necessary, but whatever
	GameManager gameManager;
	BlockManager blockManager;


	void Start()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
	}
	
	void Update()
	{
		if(!gameManager.gameFrozen)
		{
			if (blockManager.getBlockAt (transform.position.x, transform.position.y)) 
			{
				CrawlerMovement move = transform.GetComponent<CrawlerMovement>();
				move.updateMyBlock(null);
				Destroy (this.gameObject);
			}
		}

	}

	public Int2 GetRoundedPosition()
	{
		return new Int2(transform.position.x, transform.position.y);
	}

	private Vector3 floatToV3(float direction)
	{
		return new Vector3 (Mathf.Cos (direction * Mathf.PI / 2.0f), Mathf.Sin (direction * Mathf.PI / 2.0f),0);
	}
	
}
