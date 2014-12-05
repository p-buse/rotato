using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlerParent : MonoBehaviour {
	GameManager gameManager;
	BlockManager blockManager;
	public int clinging=0;
	public int moving=1;
	
	void Start()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();

		CrawlerMovement[] children = transform.GetComponentsInChildren<CrawlerMovement> ();
		for(int i = 0; i<children.Length;i++)
		{
			//print ("found a CrawlerMovement");
			children[i].blockManager = blockManager;
			children[i].gameManager = gameManager;
			children[i].clinging = this.clinging;
			children[i].moving = this.moving;
			//segments[i].falling = false;

			setBlock(children[i]);
			//children[i].updateMyBlock(children[i].getMyBlock());

			//print (segments[i].myBlock);
			//segments[i].transform.eulerAngles = Vector3.zero;
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

	//so can put parent at 3/4 of the way in one direction, 1/2 in other
	void setBlock(CrawlerMovement child)
	{
		Vector3 pos = child.transform.position - 0.26f * floatToV3 (clinging);
		AbstractBlock block = blockManager.getBlockAt (pos.x, pos.y);
		if(block!=null)
		{
			child.updateMyBlock(block);
		}

	}

	public Vector3 floatToV3(float direction)
	{
		return new Vector3 (-1.0f*Mathf.Sin (direction * Mathf.PI / 2.0f), Mathf.Cos (direction * Mathf.PI / 2.0f),0);
	}


}
