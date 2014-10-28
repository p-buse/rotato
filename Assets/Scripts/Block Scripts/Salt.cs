using UnityEngine;
using System.Collections;

public class Salt : AbstractBlock {

	public int rotationsBeforeRemove;
	public TextMesh field;
	Player player;
	Int2 location;

	void Start() {
		player = GameObject.FindObjectOfType<Player>();
		location = new Int2(transform.position.x, transform.position.y);
		field = GetComponent<TextMesh>();
		field.text = "" + rotationsBeforeRemove;
	}

	void Update() {
		if (player.GetRoundedPosition().Equals(location)) {
			Salt[] salt = gameManager.salt;
			for (int i = 0; i < salt.Length; i++) {
				if (salt[i] == this) {
					salt[i] = null;
					break;
				}
			}
            blockManager.grid.Remove(location);
			Destroy(this.gameObject);
			gameManager.saltSoFar++;
		}
	}
	
	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotatable() {
		return false;
	}
	
	public override void AnimateFrameOfRotation (Int2 center, int direction, float time) {}
	
	public override Int2 posAfterRotation(Int2 center, int dir) {
		return new Int2 (transform.position.x, transform.position.y);
	}
	
	public override void finishRotation(Int2 center, int dir)
	{
		
	}
}