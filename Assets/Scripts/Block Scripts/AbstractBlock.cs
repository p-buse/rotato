using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractBlock : MonoBehaviour
{
    /// <summary>
    /// Used for "no spin zones."
    /// </summary>
    /// <returns>true if this invalidates the rotation, false otherwise</returns>
    public abstract bool invalidatesRotation();

    /// <summary>
    /// Used for blocks that don't move.
    /// </summary>
    /// <returns>true if the block moves, false otherwise</returns>
    public abstract bool isRotatable();

    // References to important stuff
    protected static GameManager gameManager;
    protected static BlockManager blockManager;
	public Transform blockSprite;
	public SpriteRenderer blockSpriteRenderer;
	public List<CrawlerMovement> crawlers;

    private int _orientation;
    /// <summary>
    /// starts at 0 for 12oclock, 1 for 9 oclock, 2 for 6 oclock, 3 for 3 oclock
    /// orientation will be always constrained between 0 and 3
    /// negative orientations get converted to their positive equivalent
    /// </summary>
    public int orientation
    {
        get
        {
            return _orientation;
        }
        set
        {
            if (value < 0)
            {
                _orientation = value + 4;
            }
            else if (value >= 4)
            {
                _orientation = value % 4;
            }
            else
            {
                _orientation = value;
            }
        }
    }

    /// <summary>
    /// How hot the block is.
    /// Heat increases 2 per second while being lasered (2 here minus 1 in Update()) and decreases 1 per second without a laser.
    /// The player dies on contact with a block with heat 6 or higher, so a block will take 3 seconds to heat up to deadly levels.
    /// The maximum heat is 9, so a block without a laser on it will cool down to safe heat levels in 3 seconds.
    /// </summary>
	[HideInInspector]
	public float heat = 0;

    /// <summary>
    /// Get a particular object's "orientation" given its current rotation in the 2D plane.
    /// </summary>
    /// <param name="obj">The object for which to find the orientation</param>
    /// <returns></returns>
 
    

    void Awake()
    {
        this.blockSprite = transform.Find("blockSprite");
		this.blockSpriteRenderer = blockSprite.GetComponent<SpriteRenderer> ();
        if (blockSprite == null)
        {
            Debug.LogError("block: " + gameObject + "at position: " + GetCurrentPosition() + " couldn't find its sprite!");
        }
        orientation = FindRotationAngle(blockSprite);
        AbstractBlock.gameManager = FindObjectOfType<GameManager>();
        AbstractBlock.blockManager = FindObjectOfType<BlockManager>();
        
    }

    // Using LateUpdate instead of Update to avoid conflicts with blocks' own Update functions
    void LateUpdate()
    {
        if (!gameManager.gameFrozen && heat > 0f)
        {
            heat -= Time.deltaTime;
            if (heat < 0f)
            {
                heat = 0f;
            }
            blockSpriteRenderer.color = new Color(1f, 1f - heat / 9f, 1f - heat / 9f);
        }
    }

	public Int2 GetCurrentPosition()
	{
		return new Int2(transform.position.x, transform.position.y);
	}

	/// <summary>
	/// moves the block's model to be where it should at this stage of the rotation.  Non-rotatable blocks override this.
	/// </summary>
	/// <param name="center">Center.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="time">Time (between 0 and 1)</param>
	public virtual void AnimateFrameOfRotation (Int2 center, int direction, float time)
	{
		int dx = Mathf.RoundToInt(transform.position.x) - center.x;
		int dy = Mathf.RoundToInt(transform.position.y) - center.y;
		int newdx = -1 * direction * dy;
		int newdy = direction * dx;
		Vector3 startVec = new Vector3(dx,dy,0);
		Vector3 endVec = new Vector3(newdx,newdy,0);

		blockSprite.transform.localPosition = (Mathf.Cos(time * Mathf.PI / 2.0f)*startVec + Mathf.Sin(time*Mathf.PI/2.0f)*endVec) + new Vector3(-dx,-dy,0);
		blockSprite.transform.eulerAngles = new Vector3(0,0,90.0f*((1.0f-time)*orientation + time*(orientation + direction)));

        // Update crawlers
		for(int i = 0; i<crawlers.Count;i++)
		{
			crawlers[i].AnimateFrameOfRotation(center, direction, time);
			
		}

	}

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
		blockSprite.transform.localPosition = new Vector3(0,0,0);
		orientation += dir;
        blockSprite.transform.eulerAngles = new Vector3(0f, 0f, orientation * 90f);
        // Round FallingBlocks' positions
		if (this as FallingBlock != null) {
			(this as FallingBlock).location = new Int2(this.transform.position.x, this.transform.position.y);
		}

		for(int i = 0; i<crawlers.Count;i++)
		{
			crawlers[i].finishRotation(center, dir);
			
		}
	}

	// Heat increases 2 per second while being lasered (2 here minus 1 in Update()) and decreases 1 per second without a laser.
	// The player dies on contact with a block with heat 6 or higher, so a block will take 3 seconds to heat up to deadly levels.
	// The maximum heat is 9, so a block without a laser on it will cool down to safe heat levels in 3 seconds.
	public void addHeat() {
		heat += Time.deltaTime * 3;
		if (heat > 9f) {
			heat = 9f;
		}
	}

	void OnCollisionStay2D(Collision2D coll) {
		if (coll.collider.gameObject.tag == "Player" && heat >= 6f && gameManager.gameState == GameManager.GameMode.playing) {
            gameManager.PlaySound("Burnt");
            gameManager.LoseLevel("Burnt by a hot block");
		}
	}

    private int FindRotationAngle(Transform obj)
    {
        int rotationAngle = Mathf.RoundToInt(obj.eulerAngles.z);
        rotationAngle = rotationAngle % 360;
        if (rotationAngle < 0)
        {
            rotationAngle = 360 - rotationAngle;
        }
        return rotationAngle / 90;
    }

	

	

}