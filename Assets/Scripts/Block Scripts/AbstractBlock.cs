using UnityEngine;

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

    protected static GameManager gameManager;
	public Transform blockSprite;
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

	private float heat = 0;

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

    void Awake()
    {
        this.blockSprite = transform.Find("blockSprite");
        if (blockSprite == null)
        {
            Debug.LogError("block: " + gameObject + "at position: " + GetCurrentPosition() + " couldn't find its sprite!");
        }
        orientation = FindRotationAngle(blockSprite);
        AbstractBlock.gameManager = FindObjectOfType<GameManager>();
        
    }

	public Int2 GetCurrentPosition()
	{
		return new Int2(transform.position.x, transform.position.y);
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
		blockSprite.transform.localPosition = (Mathf.Cos(time * Mathf.PI / 2.0f)*startVec + Mathf.Sin(time*Mathf.PI/2.0f)*endVec) + new Vector3(-dx,-dy,0);

		//absolute angle treatment
		blockSprite.transform.eulerAngles = new Vector3(0,0,90.0f*((1.0f-time)*orientation + time*(orientation + direction)));
	}

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
		blockSprite.transform.localPosition = new Vector3(0,0,0);
		orientation += dir;
        blockSprite.transform.eulerAngles = new Vector3(0f, 0f, orientation * 90f);
        // Round FallingBlocks' positions
		if (this as FallingBlock != null) {
			(this as FallingBlock).location = new Int2(this.transform.position.x, this.transform.position.y);
		}
	}

	void Update() {
		if (!gameManager.gameFrozen && heat > 0f) {
			heat -= Time.deltaTime;
		}
	}
}