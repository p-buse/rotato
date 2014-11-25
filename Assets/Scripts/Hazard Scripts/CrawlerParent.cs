using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlerParent : MonoBehaviour {
	GameManager gameManager;
	BlockManager blockManager;
	public int clinging=0;
	public int moving=1;
	//public AbstractBlock startBlock;
	
	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
//		if(startBlock ==null)
//		{
//			Collision2D hit = Physics2D.Raycast ((Vector2)transform.position, -1.0f*floatToV2 (clinging), 0.15f);
//			if(hit!=null)
//			{
//				startBlock = blockManager.getBlockAt(hit.transform.position);
//			}
//			this.startBlock = blockManager.getBlockAt(hitPos.x, hitPos.y);
//			print (startBlock.transform.position);
		//}
		//print (startBlock.transform.position);
		CrawlerMovement[] segments = transform.GetComponentsInChildren<CrawlerMovement> ();
		for(int i = 0; i<segments.Length;i++)
		{
			//print ("found a CrawlerMovement");
			segments[i].blockManager = blockManager;
			segments[i].gameManager = gameManager;
			segments[i].clinging = this.clinging;
			segments[i].moving = this.moving;
			//segments[i].falling = false;
			segments[i].updateMyBlock(segments[i].getMyBlock());
			//print (segments[i].myBlock);
			segments[i].transform.eulerAngles = Vector3.zero;
			//segments[i].transform.parent= null;
		}
	}
	
	void Update()
	{
		if(!gameManager.gameFrozen && transform.childCount ==0)
		{
			Destroy (this.gameObject);
		}
	}

	public Vector2 floatToV2(float direction)
	{
		return new Vector2 (-1.0f*Mathf.Sin (direction * Mathf.PI / 2.0f), Mathf.Cos (direction * Mathf.PI / 2.0f));
	}


}
