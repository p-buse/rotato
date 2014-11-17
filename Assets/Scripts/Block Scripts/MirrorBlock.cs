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