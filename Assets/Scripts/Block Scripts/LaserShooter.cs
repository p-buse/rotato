using UnityEngine;
using System.Collections;

public class LaserShooter : AbstractBlock {

	LineRenderer laser;
	public Vector2 startPoint;
	public Vector2 direction;
	static Vector2[] directions = {new Vector2 (0, 1), new Vector2 (-1, 0), new Vector2 (0, -1), new Vector2 (1, 0)};
	public float orient;

	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotatable() {
		return true;
	}

	public override string myType ()
	{
		return "Laser";
	}

	void Start() {
		laser = gameObject.GetComponent<LineRenderer>();
		setFireDirection ();
	}

	void Update() {
		orient = orientation;
		if (!gameManager.gameFrozen) {
			laser.SetPosition (0, startPoint - ((Vector2)transform.position));
			RaycastHit2D hit = Physics2D.Raycast(startPoint, direction);
			if (hit.collider != null) {
				laser.SetPosition(1, new Vector2(Mathf.Abs(direction.x)*(hit.point.x-startPoint.x+direction.x*0.5f), Mathf.Abs(direction.y)*(hit.point.y-startPoint.y+direction.y*0.5f)));
				if (hit.collider.gameObject.tag == "Player" && gameManager.gameState == GameManager.GameMode.playing) {
                    gameManager.PlaySound("Lasered");
                    gameManager.LoseLevel("Evaporated by a laser");
				}
				else if (hit.collider.gameObject.GetComponent<CrawlerSegment>() != null) {
					hit.collider.gameObject.GetComponent<CrawlerSegment>().dieSafely();
				}
				else if (hit.collider.gameObject.tag == "Block") {
					AbstractBlock blockHit = hit.collider.gameObject.GetComponent<AbstractBlock>();
					blockHit.addHeat(orientation);
				}
				else if (hit.collider.gameObject.tag == "Sprite") {
					AbstractBlock blockHit = hit.collider.gameObject.transform.parent.gameObject.GetComponent<AbstractBlock>();
					blockHit.addHeat(orientation);
				}
			}
			else {
				laser.SetPosition(1, new Vector2(direction.x*30.5f, direction.y*30.5f));
			}
		}
	}

	public override bool isPointInside(float x, float y)
	{
		return blockSprite.collider2D.OverlapPoint(new Vector2 (x, y));
	}

	public override void finishRotation(Int2 center, int dir) {
		base.finishRotation(center, dir);
		setFireDirection();
		blockSprite.collider2D.enabled = true;
	}

	public void setFireDirection() {
		startPoint = (Vector2)transform.position + 0.5f*floatToV2 (orientation);

		direction = floatToV2(orientation);
		laser.SetPosition (0, startPoint - ((Vector2)transform.position));
		RaycastHit2D hit = Physics2D.Raycast(startPoint, direction);
		if (hit.collider != null) {
			laser.SetPosition(1, new Vector2(Mathf.Abs(direction.x)*(hit.point.x-startPoint.x+direction.x*0.5f), Mathf.Abs(direction.y)*(hit.point.y-startPoint.y+direction.y*0.5f)));
		}
		else {
			laser.SetPosition(1, new Vector2(direction.x*30.5f, direction.y*30.5f));
		}

	}

	/// <summary>
	/// Given a relative vector, returns the float in [0,4) 
	/// corresponding to the normal vector to this block's side in that direction
	/// </summary>
	/// <returns>The proper cling float.</returns>
	/// <param name="relVec">Rel vec.</param>
	public override float relVecToClingFloat(Vector3 relVec)
	{
		float angle = (Vector3.Angle(new Vector3(1,0,0), relVec)+360f)%360;
		if(relVec.y<0)
		{
			angle = 360 - angle;
		}
		float tip = (90f + 90f * orientation) % 360;
		float ccwCorner = (225f + 90f * orientation) % 360;
		float cwCorner = (315f + 90f * orientation) % 360;
		//first side? (ccw of beam)
		if (angleIsBetween(angle, tip,ccwCorner))
	    {
			//print (" on side 1");
			return (orientation + 2f/3f)%4;
		}
		//bottom?
		if (angleIsBetween(angle, ccwCorner,cwCorner))
		{
			//print ("hit bottom");
			return (orientation + 2f)%4f;
		}
		//cw side of beam?
		if (angleIsBetween(angle, cwCorner,tip))
		{
			//print ("hi");
			//print (" on side -1");
			return (orientation + 10f/3f)%4;
		}
		//print ("something weird");
		return 1f;
	}
	
	public override void AnimateFrameOfRotation (Int2 center, int direction, float time) 
	{
		base.AnimateFrameOfRotation (center, direction, time);
		laser.SetPosition (0, ((Vector2)transform.position)-startPoint);
		laser.SetPosition (1, ((Vector2)transform.position)-startPoint);
		blockSprite.collider2D.enabled = false;
	}

	private Vector2 floatToV2(float direction)
	{
		return new Vector2 (-1.0f*Mathf.Sin (direction * Mathf.PI / 2.0f), Mathf.Cos (direction * Mathf.PI / 2.0f));
	}

	private Vector3 floatToV3(float direction)
	{
		return new Vector3 (-1.0f*Mathf.Sin (direction * Mathf.PI / 2.0f), Mathf.Cos (direction * Mathf.PI / 2.0f),0);
	}
	/// <summary>
	/// returns whether the given float is in [start, end)
	/// handles wrapping by assuming you give them in ccw order
	/// (so it should actually work for floats or degrees or radians)
	/// </summary>
	/// <returns>The <see cref="System.Boolean"/>.</returns>
	/// <param name="myAngle">My angle.</param>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	private bool angleIsBetween(float myAngle, float start, float end)
	{
		//print ("Is "+myAngle+" between "+start+" and "+end+"?");
		if(start<end)
			return start<=myAngle && myAngle<end;
		if(end<start)
			return start<=myAngle || end>myAngle;
	
		//print ("Is "+myAngle+" between "+start+" and "+end+"?");
		return false;
	}
}