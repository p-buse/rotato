using UnityEngine;
using System.Collections;

public class CrawlerMovement : MonoBehaviour
{
	//this crawler segment has radius 0.1
	
	public float moveSpeed = 1f;
	
	public GameManager gameManager;
	public BlockManager blockManager;
	
	
	public bool falling = true;
	public bool hasBelow = false;
	
	//ccw rotation like blocks:
	//0 mod 4 = (0,1) up-pointing vector
	//1 mod 4 = (-1,0) left-pointing vector
	//2 mod 4 = (0,-1) down-pointing vector
	//3 mod 4 = (1,0) right-pointing vector
	public float moving;  //current crawling direction, {0,1,2,3}
	public float clinging; //what side of block is the crawler on? in {0,1,2,3}
	//private Vector2 movingVec;
	//private Vector2 clingingVec;
	public float fallSpeed = 1.5f;
	public AbstractBlock myBlock; //which block am I on?
	public Transform crawlerSprite;

	//will use this for non-90-degree turns
	private Vector2 nextClingingVec;
	public float moveTimer;
	
	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
		this.crawlerSprite = transform.Find ("crawlerSprite");
		//moveTimer = 0f;
		
	}
	
	void FixedUpdate ()
	{
		
		if(!gameManager.gameFrozen)
		{

			crawlerSprite.transform.eulerAngles = new Vector3(0,0,90f*clinging);
			//crawling 'left'
//			if((moving - clinging+4)%4==1)
//				crawlerSprite.transform.localScale.x = -1f;
//			else
//				crawlerSprite.transform.localScale.x = 1f;

			if(falling && isGrounded())
			{
				falling = false;
			}
			//landing behavior
			if(falling && hasBelow)
			{
				clinging = 0;
				if(moving!=1)
					moving=3;
				updateMyBlock(getMyBlock());
				falling = false;
			}
			
			hasBelow = hasBlockBelow ();

			//falling
			if(falling && !hasBlockBelow())
			{
				updateMyBlock (null);
				transform.Translate(0, -fallSpeed*Time.deltaTime, 0);
			}
			

			
			//if(!falling) //crawling
			if(myBlock!=null)
			{
				moveTimer+=Time.deltaTime;
				float amt= Mathf.Sin (moveTimer*2.0f*Mathf.PI);
				//if(Mathf.Sin (moveTimer*2.0f*Mathf.PI)<0)
				//	amt = 0;
				crawlerSprite.transform.localPosition = -0.05f*amt*floatToV3(moving);
				//transform.Translate((0.5f+0.5f*Mathf.Sin (moveTimer*2.0f*Mathf.PI)*Mathf.Sin (moveTimer*2.0f*Mathf.PI))*floatToV3(moving)*Time.deltaTime);
				transform.Translate(floatToV3(moving)*Time.deltaTime);
				//print(myBlock.relVecToClingFloat(transform.position - myBlock.transform.position));
				//when the crawler bumps a block, cling to it and move up it
				if(bumpedBlockForward())
				{
					updateMyBlock(forwardBlock());
			
					float change = myBlock.relVecToClingFloat(transform.position - myBlock.transform.position) - clinging;
					//print ("bumped forward, change = "+change);
					//float change = clinging - moving;
					moving = (moving + change + 4)%4;
					clinging = (clinging + change + 4)%4;

				}
				else 
				{
					if(myBlock!=null)
					{
//						float relx = transform.position.x - myBlock.transform.position.x;
//						float rely = transform.position.y - myBlock.transform.position.y;
						//if( myBlock.relVecToClingFloat(transform.position - myBlock.transform.position)!= clinging && !isGrounded())
						if( !isGrounded())// && reachedEdge())
						{

							//turn down for what
							//transform.Translate(-0.02f*floatToV3 (clinging));
							//transform.Translate(0.08f*floatToV3 (moving));
							if(isGrounded()){
								updateMyBlock(getMyBlock());
							}
							//Vector3 nextFramePosition = transform.position + Time.deltaTime*floatToV3(moving);
							float change = myBlock.relVecToClingFloat(transform.position - myBlock.transform.position) - clinging;
							//print(change);
							//float change = moving - clinging ;
							//float oldMoving = moving;
							moving = (moving +4 + change)%4;
							clinging =(clinging + 4 + change)%4;
						}
					}
				}
			}
			
		}
	}

	public bool reachedEdge()
	{
		Vector3 pos = transform.position -0.15f*floatToV3 (clinging) + 0.02f*floatToV3 (moving);
		return blockManager.getBlockAt (pos.x, pos.y)==null;
	}

	/// <summary>
	/// updates myBlock field
	/// </summary>
	/// <param name="block">Block.</param>
	public void updateMyBlock(AbstractBlock block)
	{
		if(myBlock!=null)
			myBlock.crawlers.Remove (this);
		myBlock = block;
		if(myBlock!=null)
			myBlock.crawlers.Add (this);
	}
	
	bool bumpedBlockForward()
	{
		Vector3 blockPos = transform.position + 0.15f * floatToV3 (moving);
		
		return blockManager.getBlockAt (blockPos.x, blockPos.y)!=null;
	}
	
	
	AbstractBlock forwardBlock()
	{
		Vector3 blockPos = transform.position + 0.15f * floatToV3 (moving);
		
		return blockManager.getBlockAt (blockPos.x, blockPos.y);
	}

	/// <summary>
	/// Returns whether the crawler has anything at its feet to cling to
	/// </summary>
	/// <returns><c>true</c>, if grounded, <c>false</c> otherwise.</returns>
	bool isGrounded()
	{
		Vector3 atMyFeet = transform.position - 0.13f * floatToV3 (clinging);
		return blockManager.getBlockAt (atMyFeet.x, atMyFeet.y) != null;
	}
	
	public AbstractBlock getMyBlock()
	{
		Vector3 blockPos = transform.position - 0.15f * floatToV3 (clinging);
		return blockManager.getBlockAt (blockPos.x, blockPos.y);
	}
	
	/// <summary>
	/// used for resolving falling:
	/// Returns the block at (x,y-0.12)
	/// </summary>
	/// <returns><c>true</c>, if there's a block below <c>false</c> otherwise.</returns>
	bool hasBlockBelow()
	{	
		//Vector3 belowMe = transform.position + (new Vector3 (0, -0.2f, 0));
		//return blockManager.getBlockAt (belowMe.x, belowMe.y) != null;
		return Physics2D.Raycast (transform.position, new Vector2(0,-1), 0.15f, 1 << LayerMask.NameToLayer ("Solid"));
	}
	
	public void AnimateFrameOfRotation(Int2 center, int direction, float time)
	{
		float dx = transform.position.x -center.x;
		float dy = transform.position.y - center.y;
		float newdx = -1 * direction * dy;
		float newdy = direction * dx;
		Vector3 startVec = new Vector3(dx,dy,0);
		Vector3 endVec = new Vector3(newdx,newdy,0);
		
		crawlerSprite.transform.localPosition = (Mathf.Cos(time * Mathf.PI / 2.0f)*startVec + Mathf.Sin(time*Mathf.PI/2.0f)*endVec) + new Vector3(-dx,-dy,0);
		
		crawlerSprite.transform.eulerAngles = new Vector3(0,0,90.0f*((1.0f-time)*clinging + time*(clinging + direction)));
	}

	
	public void finishRotation(Int2 center, int dir)
	{
		transform.position =  posAfterRotation (center, dir);
		crawlerSprite.transform.localPosition = Vector3.zero;

		moving = (moving + dir + 4)%4;
		clinging = (clinging + dir + 4)%4;
		
	}
	
	public Vector3 posAfterRotation(Int2 center, int dir)
	{
		float dx = transform.position.x -center.x;
		float dy = transform.position.y - center.y;
		float newx = center.x - (dir*dy);
		float newy = center.y + (dir*dx);
		return new Vector3(newx, newy,0);
	}
	
	
	public Vector2 floatToV2(float direction)
	{
		return new Vector2 (-1.0f*Mathf.Sin (direction * Mathf.PI / 2.0f), Mathf.Cos (direction * Mathf.PI / 2.0f));
	}
	
	public Vector3 floatToV3 (float direction)
	{
		return new Vector3 (-1.0f*Mathf.Sin (direction * Mathf.PI / 2.0f), Mathf.Cos (direction * Mathf.PI / 2.0f),0);
	}
	
//	void OnCollisionEnter2D(Collision2D collision)
//	{
//		nextClingingVec = collision.contacts [0].normal;
//	}
	
	//replaces need for "crawling behavior" in update()?
	//	void OnCollisionStay2D(Collision2D info)
	//	{
	//		print ("Collided");
	//		if(myBlock.gameObject != info.gameObject)
	//		{
	//			Vector2 normal = info.contacts[0].normal;
	//			if(info.gameObject.tag == "Block")
	//			{
	//				updateMyBlock(info.gameObject.GetComponent<AbstractBlock>());
	//				Quaternion.FromToRotation(Vector2.up, normal);
	//				transform.Translate(transform.right*Time.deltaTime);
	//				Debug.DrawRay(info.contacts[0].point, normal, Color.white);
	//			}
	//		}
	//		else if(myBlock.gameObject == info.gameObject)
	//		{
	//			transform.Translate(transform.right*Time.deltaTime);
	//			//TODO: if this takes me off the block, cling to its next side
	//		}
	//	}
	
//	public float nextMoving()
//	{
//
//	}

}



