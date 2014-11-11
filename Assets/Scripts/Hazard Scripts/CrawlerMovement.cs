using UnityEngine;
using System.Collections;

public class CrawlerMovement : MonoBehaviour
{
	//this crawler segment has radius 0.1
	
	public float moveSpeed = 1f;
	
	GameManager gameManager;
	BlockManager blockManager;
	
	
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
	
	
	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
		this.crawlerSprite = transform.Find ("crawlerSprite");
		
	}
	
	void FixedUpdate ()
	{
		
		if(!gameManager.gameFrozen)
		{
			if(isGrounded())
			{
				falling = false;
			}
			
			hasBelow = hasBlockBelow ();
			
			if(falling && !hasBelow)
			{
				updateMyBlock (null);
				transform.Translate(0, -fallSpeed*Time.deltaTime, 0);
			}
			
			//landing behavior
			if(falling && hasBlockBelow())
			{
				clinging = 0;
				if(moving!=1)
					moving=3;
				updateMyBlock(getMyBlock());
				falling = false;
			}
			
			if(!falling) //crawling
			{
				
				updateMyBlock(getMyBlock());
				transform.Translate(floatToV3(moving)*Time.deltaTime);
				//when the crawler bumps a block, cling to it and move up it
				if(bumpedBlockForward())
				{
					updateMyBlock(forwardBlock());
					if(forwardBlock() is LaserShooter && forwardBlock().orientation == (moving+5)%4)
					{
						//						float change = 1f/3f;
						//						moving = (moving + change + 4)%4;
						//						clinging = (clinging + change + 4)%4;
					}
					else
					{
						float change = clinging - moving;
						moving = (moving + change + 4)%4;
						clinging = (clinging + change + 4)%4;
					}
					
				}
				else 
				{
					if(myBlock!=null)
					{
						float relx = transform.position.x - myBlock.transform.position.x;
						float rely = transform.position.y - myBlock.transform.position.y;
						if(!isGrounded() &&  reachedEdge())//(Mathf.Abs(relx)>=0.505f && Mathf.Abs (rely)>=0.505f))
						{
							//turn down for what
							//transform.Translate(0.10f*floatToV3 (moving));
							transform.Translate(-0.05f*floatToV3 (clinging));
							float change = moving - clinging ;
							float oldMoving = moving;
							moving = (moving +4 + change)%4;
							clinging =(clinging + 4 + change)%4;
							updateMyBlock(getMyBlock());
							
						}
					}
				}
			}
			
		}
	}
	
	private bool reachedEdge()
	{
		Vector2 check = (Vector2)transform.position - 0.15f * floatToV2 (clinging);// 0.01f * floatToV2 (moving);
		//if there's no block right in front of you
		if(!bumpedBlockForward())
		{
			if(!blockManager.getBlockAt(check.x, check.y))
			{
				return true;
			}
		}
		return false;
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
		Vector3 blockPos = transform.position + 0.12f * floatToV3 (moving);
		
		return blockManager.getBlockAt (blockPos.x, blockPos.y)!=null;
	}
	
	
	AbstractBlock forwardBlock()
	{
		Vector3 blockPos = transform.position + 0.12f * floatToV3 (moving);
		
		return blockManager.getBlockAt (blockPos.x, blockPos.y);
	}
	
	bool isGrounded()
	{
		Vector3 belowMe = transform.position - 0.12f * floatToV3 (clinging);
		return blockManager.getBlockAt (belowMe.x, belowMe.y) != null;
	}
	
	AbstractBlock getMyBlock()
	{
		Vector3 blockPos = transform.position - 0.12f * floatToV3 (clinging);
		return blockManager.getBlockAt (blockPos.x, blockPos.y);
	}
	
	
	bool hasBlockBelow()
	{
		return Physics2D.Raycast (transform.position, new Vector2(0,-1), 0.12f, 1 << LayerMask.NameToLayer ("Solid"));
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
	
	public void MoveFrameOfRotation(Vector2 center, int direction, float time)
	{
		float dx = transform.position.x -center.x;
		float dy = transform.position.y - center.y;
		float newdx = -1 * direction * dy;
		float newdy = direction * dx;
		Vector3 startVec = new Vector3(dx,dy,0);
		Vector3 endVec = new Vector3(newdx,newdy,0);
		
		transform.position = (Mathf.Cos(time * Mathf.PI / 2.0f)*startVec + Mathf.Sin(time*Mathf.PI/2.0f)*endVec) + new Vector3(-dx,-dy,0);
		
		transform.eulerAngles = new Vector3(0,0,90.0f*((1.0f-time)*clinging + time*(clinging + direction)));
	}
	
	public void finishRotation(Int2 center, int dir)
	{
		transform.position =  posAfterRotation (center, dir);
		crawlerSprite.transform.localPosition = Vector3.zero;
		crawlerSprite.transform.eulerAngles = Vector3.zero;//or (0,0,clinging * 90f) if not rot. symmetric;
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
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		nextClingingVec = collision.contacts [0].normal;
	}
	
	//relpaces need for "crawling behavior" in update()?
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
	
	
}



