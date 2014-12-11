using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlerBoss : MonoBehaviour {
	//not sure if these are necessary, but whatever
	GameManager gameManager;
	BlockManager blockManager;
	float moveClock = 0f;
	
	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();

	}
	
	void Update()
	{
		if(!gameManager.gameFrozen)
		{
			moveClock+=Time.deltaTime;
			transform.localPosition += Time.deltaTime*(new Vector3(Mathf.Sin (moveClock*Mathf.PI/2f),0,0));
			transform.localPosition += 0.05f*Time.deltaTime*Vector3.right;
			//Vector2 top = new Vector2(transform.position.x, transform.position.y)+0.1f*move.floatToV2(move.clinging);
			AbstractBlock block = blockManager.getBlockAt (transform.position.x, transform.position.y);
			if (block!=null) 
			{
				//				print("block at "+transform.position.x+", "+transform.position.y);
				//				print ("my last moving number: "+move.moving);
				//				print ("my last moving vector: "+move.floatToV2(move.moving));
				//				Debug.DrawLine(transform.position, transform.position + move.floatToV3(move.clinging));
				blockManager.RemoveBlock(new Int2(block.transform.position.x, block.transform.position.y));
			}

		}
		
	}
	
	void OnCollisionEnter2D(Collision2D coll) 
	{
		if(!gameManager.gameFrozen)
		{
			if (coll.gameObject.tag == "Player") 
			{
				//kill player
				gameManager.LoseLevel("Nom nom nom... you were a tasty snack for the UberWorm");
				
			}
			if (coll.gameObject.tag == "Block") 
			{
				blockManager.RemoveBlock(new Int2(coll.gameObject.transform.position.x, coll.gameObject.transform.position.y));
			}

			Destroy (coll.gameObject);
			AbstractBlock mirrorsAndLasers = coll.gameObject.GetComponentInParent<AbstractBlock> ();
			Destroy (mirrorsAndLasers.gameObject);
		}
		
	}
	

}
