using UnityEngine;

public abstract class AbstractBlock : MonoBehaviour
{

    protected GameManager gameManager;

	//blockModel needs to be created, not sure how to start that in prefab world.
	public blockModel model;

	public float orientation; //starts at 0, +1 = 1 90-degree turn ccw ?  we can tweak what this means. 
	//in analog of position, probably want this to be discrete, while model has continuous EulerAngles instead

    void Start()
    {
        this.gameManager = GameObject.FindObjectOfType<GameManager>();
    }

	/// <summary>
	/// moves the block's model to be where it should at this stage of the rotation.  StaticBlocks override this? Currently this method
	/// is both here and in the Block script
	/// </summary>
	/// <param name="center">Center.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="time">Time.</param>
	public abstract void AnimateFrameOfRotation (Int2 center, string direction, float time)
	{
		int dx = transform.position.x-center.x;
		int dy = transform.position.y-center.y;
		int newdx = –1.0*direction*dy;
		int newdy = direction*dx;
		Vector3 startVec = Vector3(dx,dy,0);
		Vector3 endVec = Vector3(newdx,newdy,0);
		
		//I'm pretty sure this is right...
		model.transform.position = 0.5*(Mathf.Cos(t*Mathf.PI/2.0)*startVec + Mathf.Sin(t*Mathf.PI/2.0)*endVec) + Vector3(-dx,-dy,0);
		
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
	public static Int2 posAfterRotation(Int2 center, int dir)
	{
		int dx = transform.position.x-center.x;
		int dy = transform.position.y-center.y;
		int newx = center.x – (dir*dy);
		int newy = center.y + (dir*dx);
		return new Int2(newx, newy);
	}

	/// <summary>
	/// Finishes the rotation by snapping the block's transform to the destination, updating its orientation, and recentering its model.
	/// </summary>
	/// <param name="center">Center.</param>
	/// <param name="dir">Dir.</param>
	public static void finishRotation(Int2 center, int dir)
	{
		Int2 end = posAfterRotation (center, dir);
		Transform.position = Vector3 (end.x, end.y, 0);
		model.transform.position = Vector3(0,0,0);
		orientation += dir;
	}

}