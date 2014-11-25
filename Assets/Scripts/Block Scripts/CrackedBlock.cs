using UnityEngine;
using System.Collections;

public class CrackedBlock : AbstractBlock {
	//displayed on the model.  if this is 0 and it's rotated again, after the rotation this block will diappear.
	//because this is public, it can be set individually from the unity scene, right?
    private int _rotationsLeft;
    public int rotationsLeft
    {
        get
        {
            return _rotationsLeft;
        }
        set
        {
            if (value >= 0 && value <= 5)
                _rotationsLeft = value;
        }
    }

	Transform numberDisplayObject;
	public SpriteRenderer numberDisplay;
	public Sprite[] numberSprites;

	void Start()
	{
		numberDisplayObject = transform.FindChild ("numberDisplay");
		numberDisplay = transform.FindChild ("numberDisplay").GetComponent<SpriteRenderer> ();

	}

	public override string myType ()
	{
		return "Cracked";
	}

	public override void AnimateFrameOfRotation (Int2 center, int direction, float time)
	{
		int dx = Mathf.RoundToInt(transform.position.x) - center.x;
		int dy = Mathf.RoundToInt(transform.position.y) - center.y;
		int newdx = -1 * direction * dy;
		int newdy = direction * dx;
		Vector3 startVec = new Vector3(dx,dy,0);
		Vector3 endVec = new Vector3(newdx,newdy,0);
		
		blockSprite.transform.localPosition = (Mathf.Cos(time * Mathf.PI / 2.0f)*startVec + Mathf.Sin(time*Mathf.PI/2.0f)*endVec) + new Vector3(-dx,-dy,0);
		numberDisplayObject.transform.localPosition = (Mathf.Cos(time * Mathf.PI / 2.0f)*startVec + Mathf.Sin(time*Mathf.PI/2.0f)*endVec) + new Vector3(-dx,-dy,0);

		blockSprite.transform.eulerAngles = new Vector3(0,0,90.0f*((1.0f-time)*orientation + time*(orientation + direction)));
		for(int i = 0; i<crawlers.Count;i++)
		{
			crawlers[i].AnimateFrameOfRotation(center, direction, time);
			
		}
	}

    public override BlockSkeleton getSkeleton()
    {
        return new BlockSkeleton(this.myType(), this.GetCurrentPosition(), this.orientation, this.spikiness, this.rotationsLeft);
    }

	public override void finishRotation(Int2 center, int dir)
	{
		base.finishRotation (center, dir);
		numberDisplayObject.transform.localPosition = new Vector3 (0, 0, 0);
	}

	public override bool invalidatesRotation()
	{
		return false;
	}
	
	public override bool isRotatable()
	{
		return true;
	}

	void Update()
	{
		numberDisplay.sprite = numberSprites [rotationsLeft];
	}

	/// <summary>
	/// To be called by the BlockManager after a rotation 
	/// decrements rotationsLeft
	/// Destroying this block is left to the BlockManager
	/// </summary>
	public void wasJustRotated(){

		if(rotationsLeft ==1)
		{
			for(int i=0;i<crawlers.Count;i++ )
			{
				CrawlerMovement c = crawlers[i];
				c.myBlock = null;
				c.falling = true;
			}
		}

		rotationsLeft--;

	}
}
