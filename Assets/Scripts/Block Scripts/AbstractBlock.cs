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
    [HideInInspector]
	public Transform blockSprite;
	protected SpriteRenderer blockSpriteRenderer;
    [HideInInspector]
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
	[HideInInspector]
	public int heated = 0;
	float heatClock = 0f;

    /// <summary>
    /// Get a particular object's "orientation" given its current rotation in the 2D plane.
    /// </summary>
    /// <param name="obj">The object for which to find the orientation</param>
    /// <returns></returns>

    public abstract string myType();

    public List<int> spikiness;

    void Awake()
    {
        this.blockSprite = transform.Find("blockSprite");
        if (blockSprite == null)
        {
            Debug.LogError("block: " + gameObject + "at position: " + GetCurrentPosition() + " couldn't find its sprite!");
        }
		this.blockSpriteRenderer = blockSprite.GetComponent<SpriteRenderer> ();
        if (blockSpriteRenderer == null)
        {
            Debug.LogError("block: " + gameObject + "at position: " + GetCurrentPosition() + " couldn't find its sprite renderer!");
        }
        orientation = FindRotationAngle(blockSprite);
        AbstractBlock.gameManager = FindObjectOfType<GameManager>();
        AbstractBlock.blockManager = FindObjectOfType<BlockManager>();
        SetupSpikes();
    }

    public void SetupSpikes()
    {
        this.spikiness = new List<int>();
        SpikyBlock[] spikes = GetComponentsInChildren<SpikyBlock>();
        foreach (SpikyBlock spike in spikes)
        {
            int spikeDirection = FindRotationAngle(spike.transform);
            if (!spikiness.Contains(spikeDirection))
            {
                spikiness.Add(spikeDirection);
                spike.SetupSpike();
            }
            else
            {
                Debug.LogError("Duplicate spikes on block at: " + GetCurrentPosition());
            }
        }
    }

    public bool RemoveSpike(int spikeDirection)
    {
        SpikyBlock[] spikes = GetComponentsInChildren<SpikyBlock>();
        foreach (SpikyBlock spike in spikes)
        {
            if (FindRotationAngle(spike.transform.parent) == spikeDirection)
            {
                Destroy(spike.transform.parent.gameObject);

                if (!spikiness.Remove(spikeDirection))
                    Debug.LogError("Couldn't remove spike from array with orientation: " + spikeDirection);
                return true;
            }
        }
        return false;
    }

    public void AddSpike(GameObject spikePrefab, int spikeDirection)
    {
        if (spikeDirection >= 0 && spikeDirection <= 3)
        {
            if (!spikiness.Contains(spikeDirection))
            {
                spikiness.Add(spikeDirection);
                GameObject newSpike = Instantiate(spikePrefab, transform.position, Quaternion.identity) as GameObject;
                newSpike.transform.eulerAngles = new Vector3(0f, 0f, spikeDirection * 90f);
                newSpike.transform.parent = this.gameObject.transform;
                SpikyBlock spikeComponent = newSpike.GetComponentInChildren<SpikyBlock>();
                if (spikeComponent != null)
                {
                    spikeComponent.SetupSpike();
                }
                else
                {
                    Debug.LogError("Spike at: " + GetCurrentPosition() + " has no spike component!");
                }
            }
        }
        else
        {
            Debug.LogError("Invalid spike direction: " + spikeDirection + "! Direction must be an integer between 0 and 3 (inclusive).");
        }
    }


    // Using LateUpdate instead of Update to avoid conflicts with blocks' own Update functions
    void LateUpdate()
    {
        if (!gameManager.gameFrozen && heat > 0f)
        {
			heatClock += Time.deltaTime * 4;
			if (heated == 0) {
	            heat -= Time.deltaTime;
	            if (heat < 0f)
	            {
	                heat = 0f;
	            }
			}
			else {
				heated--;
			}
		}
		blockSpriteRenderer.color = new Color(1f, 1f - heat * (2.1f + 0.4f * Mathf.Sin(heatClock * Mathf.PI)), 1f - heat * (2.1f + 0.4f * Mathf.Cos(heatClock * Mathf.PI)));
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

		for(int i = 0; i<crawlers.Count;i++)
		{
			crawlers[i].finishRotation(center, dir);
			
		}
	}

	// Heat increases 2 per second while being lasered (3 here minus 1 in Update()) and decreases 1 per second without a laser.
	// The player dies on contact with a block with heat 6 or higher, so a block will take 3 seconds to heat up to deadly levels.
	// The maximum heat is 9, so a block without a laser on it will cool down to safe heat levels in 3 seconds.
	public virtual void addHeat(int source) {
		heat += Time.deltaTime;
		heated = 2;
		if (heat > 0.4f) {
			heat = 0.4f;
		}
	}

	void OnCollisionStay2D(Collision2D coll) {
		if (coll.collider.gameObject.tag == "Player" && heated > 0 && gameManager.gameState == GameManager.GameMode.playing) {
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

	public virtual BlockSkeleton getSkeleton(){
        return new BlockSkeleton(this.myType(), this.GetCurrentPosition(), this.orientation, this.spikiness);
	}

	//non-square blocks override this to use their sprite's collider instead
	/// <summary>
	/// Checks whether (x,y) is inside this block's collider
	/// </summary>
	/// <returns><c>true</c>, if point is inside <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public virtual bool isPointInside(float x, float y)
	{
		return collider2D.bounds.Contains(new Vector2(x, y));
	}

	/// <summary>
	/// Given a relative vector to a crawler segment, returns the float in [0,4) 
	/// corresponding to the normal vector to this block's side in that direction
	/// </summary>
	/// <returns>The proper cling float.</returns>
	/// <param name="relVec">Rel vec.</param>
	public virtual float relVecToClingFloat(Vector3 relVec)
	{
		return Mathf.RoundToInt(Mathf.Atan2 (relVec.y, relVec.x)*2f/Mathf.PI +3f)%4;
	}



}