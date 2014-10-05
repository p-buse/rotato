﻿using UnityEngine;

public abstract class AbstractBlock : MonoBehaviour
{

    protected GameManager gameManager;

	//blockModel needs to be created, not sure how to start that in prefab world.
	public Transform blockSprite;

	public float orientation; //starts at 0, +1 = 1 90-degree turn ccw ?  we can tweak what this means. 
	//in analog of position, probably want this to be discrete, while model has continuous EulerAngles instead

    void Start()
    {
        this.gameManager = GameObject.FindObjectOfType<GameManager>();
        this.blockSprite = transform.Find("blockSprite");
    }

	/// <summary>
	/// moves the block's model to be where it should at this stage of the rotation.  StaticBlocks override this? Currently this method
	/// is both here and in the Block script
	/// </summary>
	/// <param name="center">Center.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="time">Time.</param>
	public virtual void AnimateFrameOfRotation (Int2 center, int direction, float time)
	{
		int dx = Mathf.RoundToInt(transform.position.x) - center.x;
		int dy = Mathf.RoundToInt(transform.position.y) - center.y;
		int newdx = -1 * direction * dy;
		int newdy = direction * dx;
		Vector3 startVec = new Vector3(dx,dy,0);
		Vector3 endVec = new Vector3(newdx,newdy,0);
		
		//I'm pretty sure this is right...
		blockSprite.transform.position = 0.5f*(Mathf.Cos(time * Mathf.PI / 2.0f)*startVec + Mathf.Sin(time*Mathf.PI/2.0f)*endVec) + new Vector3(-dx,-dy,0);
		
		//the above is absolute, this is relative... not sure how I could do this (facing) absolutely
		//model.transform.rotation += (direction*Time.deltaTime*90.0);
	}


    /// <summary>
    /// Will the block rotate?
    /// </summary>
    /// <returns>True if it will. DuH!!!!!!!</returns>
    public abstract bool isRotable();
    public abstract bool invalidatesRotation();


	//not sure if this should be in AbstractBlock or just individual blocks.  Probably here.
	/// <summary>
	/// computes and returns the destination Int2
	/// </summary>
	/// <returns> The destination Int2 of this block after this rotation</returns>
	/// <param name="center">The center position.</param>
	/// <param name="dir">Dir.</param>
	public virtual Int2 posAfterRotation(Int2 center, int dir)
	{
		int dx = Mathf.RoundToInt(transform.position.x) -center.x;
		int dy = Mathf.RoundToInt(transform.position.y) - center.y;
		int newx = center.x - (dir*dy);
		int newy = center.y + (dir*dx);
		return new Int2(newx, newy);
	}

	/// <summary>
	/// Finishes the rotation by snapping the block's transform to the destination, updating its orientation, and recentering its model.
	/// </summary>
	/// <param name="center">Center.</param>
	/// <param name="dir">Dir.</param>
	public virtual void finishRotation(Int2 center, int dir)
	{
		Int2 end = posAfterRotation (center, dir);
		transform.position = new Vector3 (end.x, end.y, 0);
		blockSprite.transform.position = new Vector3(0,0,0);
		orientation += dir;
	}

}