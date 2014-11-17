using UnityEngine;
using System.Collections;

public class LaserShooter : AbstractBlock {

	LineRenderer laser;
	public Vector2 startPoint;
	public Vector2 direction;
	static Vector2[] directions = {new Vector2 (0, 1), new Vector2 (-1, 0), new Vector2 (0, -1), new Vector2 (1, 0)};

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
		return blockSprite.collider2D.bounds.Contains (new Vector3 (x, y, 0));
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

	
	public override void AnimateFrameOfRotation (Int2 center, int direction, float time) {
		base.AnimateFrameOfRotation (center, direction, time);
		laser.SetPosition (0, ((Vector2)transform.position)-startPoint);
		laser.SetPosition (1, ((Vector2)transform.position)-startPoint);
		blockSprite.collider2D.enabled = false;
	}

	private Vector2 floatToV2(float direction)
	{
		return new Vector2 (-1.0f*Mathf.Sin (direction * Mathf.PI / 2.0f), Mathf.Cos (direction * Mathf.PI / 2.0f));
	}
}