using UnityEngine;
using System.Collections;

public class MirrorBlock : AbstractBlock {
	
	LineRenderer laser;
	public Vector2 direction;
	static Vector2[] directions = {new Vector2 (0, 1), new Vector2 (-1, 0), new Vector2 (0, -1), new Vector2 (1, 0)};
	bool firing = false;
	float fireTime = 0f;
	
	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotatable() {
		return true;
	}

	public override string myType ()
	{
		return "Mirror";
	}

	void Start() {
		laser = gameObject.GetComponent<LineRenderer>();
		laser.SetPosition (0, Vector2.zero);
		laser.SetPosition (1, Vector2.zero);
	}
	
	void Update() {
		if (!gameManager.gameFrozen) {
			laser.SetPosition (0, Vector2.zero);
			laser.SetPosition (1, Vector2.zero);
			if (fireTime > 0.1f && firing) {
				RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
				if (hit.collider != null) {
					laser.SetPosition(1, new Vector2(Mathf.Abs(direction.x)*(hit.point.x-transform.position.x), Mathf.Abs(direction.y)*(hit.point.y-transform.position.y)));
					if (hit.collider.gameObject.tag == "Player" && gameManager.gameState == GameManager.GameMode.playing) {
						gameManager.PlaySound("Lasered");
						gameManager.LoseLevel("Evaporated by a laser");
					}
					else if (hit.collider.gameObject.GetComponent<CrawlerSegment>() != null) {
						hit.collider.gameObject.GetComponent<CrawlerSegment>().dieSafely();
					}
					else if (hit.collider.gameObject.tag == "Block") {
						AbstractBlock blockHit = hit.collider.gameObject.GetComponent<AbstractBlock>();
						blockHit.addHeat((int)(Mathf.Abs(direction.x) * (2 + direction.x) + Mathf.Abs(direction.y) * (1 - direction.y)));
					}
					else if (hit.collider.gameObject.tag == "Sprite") {
						AbstractBlock blockHit = hit.collider.gameObject.transform.parent.gameObject.GetComponent<AbstractBlock>();
						blockHit.addHeat((int)(Mathf.Abs(direction.x) * (2 + direction.x) + Mathf.Abs(direction.y) * (1 - direction.y)));
					}
				}
				else {
					laser.SetPosition(1, new Vector2(direction.x*30.5f, direction.y*30.5f));
				}
			}
			fireTime -= Time.deltaTime;
			if (fireTime < 0f) {
				fireTime = 0f;
			}
		}
	}

	public override bool isPointInside(float x, float y)
	{
		return blockSprite.collider2D.bounds.Contains (new Vector3 (x, y, 0));
	}

	public override void addHeat(int source) {
		if (source == (orientation + 2) % 4) {
			fireTime += Time.deltaTime * 2;
			if (fireTime > 0.2f) {
				fireTime = 0.2f;
			}
			direction = directions[(orientation + 3) % 4];
			firing = true;
		}
		else if (source == (orientation + 1) % 4) {
			fireTime += Time.deltaTime * 2;
			if (fireTime > 0.2f) {
				fireTime = 0.2f;
			}
			direction = directions[orientation];
			firing = true;
		}
		else {
			base.addHeat(orientation);
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
		//float angle = (Mathf.Atan2 (relVec.y, relVec.x)*2f/Mathf.PI + 3f)%4;
		float angle = (Vector3.Angle(new Vector3(1,0,0), relVec)+360f)%360;
		//print ("to this block you are at an angle of " + angle);
		float topLeftCorner = (135f + 90f * orientation) % 360;
		float ccwCorner = (225f + 90f * orientation) % 360;
		float cwCorner = (315f + 90f * orientation) % 360;
		//first side? (ccw of beam)
		if (angleIsBetween(angle, topLeftCorner,ccwCorner))
		{
			return (orientation + 1f)%4;
		}
		//bottom?
		if (angleIsBetween(angle, ccwCorner,cwCorner))
		{
			return (orientation + 2f)%4f;
		}
		//cw side of beam?
		if (angleIsBetween(angle, cwCorner,topLeftCorner))
		{
			return (orientation + 3.5f)%4;
		}
		//print ("somethingweird");
		return 1f;
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

		//print ("This shouldn't happen");
		return false;
	}

	public override void finishRotation(Int2 center, int dir) {
		base.finishRotation(center, dir);
		stopFiring();
		blockSprite.collider2D.enabled = true;
	}

	public void stopFiring() {
		fireTime = 0f;
		laser.SetPosition(1, Vector2.zero);
	}

	public override void AnimateFrameOfRotation (Int2 center, int direction, float time) {
		base.AnimateFrameOfRotation (center, direction, time);
		laser.SetPosition (0, Vector2.zero);
		laser.SetPosition (1, Vector2.zero);
		blockSprite.collider2D.enabled = false;
	}
}