using UnityEngine;
using System.Collections;

public class LaserShooter : AbstractBlock {

	float distance;
	LineRenderer laser;
	Vector2 startPoint;
	Vector2 direction;
	static Vector2[] startPoints = {new Vector2(transform.position.x, transform.position.y+0.5f), new Vector2(transform.position.x-0.5f, transform.position.y), new Vector2(transform.position.x, transform.position.y-0.5f), new Vector2(transform.position.x+0.5f, transform.position.y)};
	static Vector2[] directions = {new Vector2 (0, 1), new Vector2 (-1, 0), new Vector2 (0, -1), new Vector2 (1, 0)};

	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotatable() {
		return true;
	}

	void Start() {
		laser = gameObject.GetComponent<LineRenderer>();
		startPoint = startPoints [orientation];
		direction = directions [orientation];
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x-0.5f, transform.position.y), -Vector2.right);
		if (hit.collider != null) {
			distance = hit.distance;
			laser.SetPosition(1, new Vector3(-hit.distance, 0f, 0f));
		}
	}

	void Update() {
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x-0.5f, transform.position.y), -Vector2.right);
		if (hit.collider != null) {
			distance = hit.distance;
			laser.SetPosition(1, new Vector3(-hit.distance-0.5f, 0f, 0f));
		}
	}
}