using UnityEngine;
using System.Collections;

public class CrackedBlock : AbstractBlock {
	//displayed on the model.  if this is 0 and it's rotated again, after the rotation this block will diappear.
	//because this is public, it can be set individually from the unity scene, right?
    [SerializeField]
    int rotationsLeft = 5;
    Animator anim;
    public bool shouldBeDead
    {
        get
        {
            return (rotationsLeft < 1);
        }
    }


    void Start()
    {
        this.anim = GetComponent<Animator>();
        this.anim.SetInteger("rotationsLeft", this.rotationsLeft);
    }

    public void IncrementRotationsLeft()
    {
        if (rotationsLeft < 5)
        {
            rotationsLeft++;
            this.anim.SetInteger("rotationsLeft", this.rotationsLeft);
        }
    }
    public void DecrementRotationsLeft()
    {
        if (rotationsLeft > 0)
        {
            rotationsLeft--;
            this.anim.SetInteger("rotationsLeft", this.rotationsLeft);
        }
    }

    public void SetRotationsLeft(int newRotationsLeft)
    {
        this.rotationsLeft = newRotationsLeft;
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        this.anim.SetInteger("rotationsLeft", this.rotationsLeft);
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
		blockSprite.transform.eulerAngles = new Vector3(0,0,90.0f*((1.0f-time)*orientation + time*(orientation + direction)));
		for(int i = 0; i<crawlers.Count;i++)
		{
			crawlers[i].AnimateFrameOfRotation(center, direction, time);
			
		}
	}

	public override bool invalidatesRotation()
	{
		return false;
	}
	
	public override bool isRotatable()
	{
		return true;
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

		DecrementRotationsLeft();

	}
}
