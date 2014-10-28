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

	void Start() {
		laser = gameObject.GetComponent<LineRenderer>();
		switch (orientation) {
			case 0: {
				startPoint = new Vector2(transform.position.x, transform.position.y+0.5f);
				break;
			}
			case 1: {
				startPoint = new Vector2(transform.position.x-0.5f, transform.position.y);
				break;
			}
			case 2: {
				startPoint = new Vector2(transform.position.x, transform.position.y-0.5f);
				break;
			}
			case 3: {
				startPoint = new Vector2(transform.position.x+0.5f, transform.position.y);
				break;
			}
		}
		laser.SetPosition (0, ((Vector2)transform.position)-startPoint);
		direction = directions [orientation];
		RaycastHit2D hit = Physics2D.Raycast(startPoint, direction);
		if (hit.collider != null) {
			laser.SetPosition(1, new Vector2(direction.x*(hit.distance+0.5f), direction.y*(hit.distance+0.5f)));
		}
	}

	void Update() {
		if (!gameManager.gameFrozen) {
			laser.SetPosition (0, ((Vector2)transform.position)-startPoint);
			RaycastHit2D hit = Physics2D.Raycast(startPoint, direction);
			if (hit.collider != null) {
				laser.SetPosition(1, new Vector2(Mathf.Abs(direction.x)*(hit.point.x-startPoint.x+direction.x*0.5f), Mathf.Abs(direction.y)*(hit.point.y-startPoint.y+direction.y*0.5f)));
				if (hit.collider.gameObject.tag == "Player") {
					gameManager.ResetLevel();
				}
				else if (hit.collider.gameObject.tag == "Block") {
					AbstractBlock blockHit = hit.collider.gameObject.GetComponent<AbstractBlock>();
					blockHit.addHeat();
				}
			}
			else {
				laser.SetPosition(1, new Vector2(direction.x*30.5f, direction.y*30.5f));
			}
		}
	}

	public override void finishRotation(Int2 center, int dir) {
		base.finishRotation (center, dir);
		switch (orientation) {
			case 0: {
				startPoint = new Vector2(transform.position.x, transform.position.y+0.5f);
				break;
			}
			case 1: {
				startPoint = new Vector2(transform.position.x-0.5f, transform.position.y);
				break;
			}
			case 2: {
				startPoint = new Vector2(transform.position.x, transform.position.y-0.5f);
				break;
			}
			case 3: {
				startPoint = new Vector2(transform.position.x+0.5f, transform.position.y);
				break;
			}
		}
		direction = directions [orientation];
	}

	
	public override void AnimateFrameOfRotation (Int2 center, int direction, float time) {
		base.AnimateFrameOfRotation (center, direction, time);
		laser.SetPosition (0, ((Vector2)transform.position)-startPoint);
		laser.SetPosition (1, ((Vector2)transform.position)-startPoint);
	}
}