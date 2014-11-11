using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingBlock : AbstractBlock {

    public float fallClock;
	public float initiatePush;
	public float pushClock;
	public int pushDirection;
	float pushStart;

	void Start() {
        fallClock = -1f;
		initiatePush = 0f;
		pushClock = -1f;
		pushStart = 0f;
        blockManager = GameObject.FindObjectOfType<BlockManager>();
	}
	
	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotatable() {
		return fallClock < 0f && pushClock < 0f;
	}

	public override string myType ()
	{
		return "Falling";
	}

	void Update() {
		if (!gameManager.gameFrozen) {
			Dictionary<Int2, AbstractBlock> grid = blockManager.grid;
			AbstractBlock check = null;
			Int2 above = GetCurrentPosition();
			above.y++;
			Int2 below = GetCurrentPosition();
			below.y--;
			if (fallClock >= 0f) {
				initiatePush = 0f;
				fallClock += Time.deltaTime*3;
				if (fallClock >= 1.0f) {
					fallClock = 0f;
					transform.position = new Vector3(GetCurrentPosition().x, GetCurrentPosition().y, 0f);
					if (grid.TryGetValue(below, out check) && (check as FallingBlock == null || (check as FallingBlock).fallClock < 0f)) {
						fallClock = -1f;
						if (grid.TryGetValue(above, out check) && check == this) {
							grid.Remove(above);
						}
					}
				}
				if (!grid.TryGetValue(GetCurrentPosition(), out check)) {
					grid.Add(GetCurrentPosition(), this);
				}
				if (fallClock >= 0f) {
					if (transform.position.y > Mathf.RoundToInt(transform.position.y)) {
						if (!grid.TryGetValue(above, out check)) {
							grid.Add(above, this);
						}
						transform.position = new Vector3(GetCurrentPosition().x, GetCurrentPosition().y-fallClock+1f, 0f);
					}
					else {
						if (grid.TryGetValue(above, out check) && check == this) {
							grid.Remove(above);
						}
						if (!grid.TryGetValue(below, out check)) {
							grid.Add(below, this);
						}
						transform.position = new Vector3(GetCurrentPosition().x, GetCurrentPosition().y-fallClock, 0f);
						if (grid.TryGetValue(below, out check) && (check as FallingBlock == null || (check as FallingBlock).fallClock < 0f)) {
							transform.position = new Vector3(GetCurrentPosition().x, GetCurrentPosition().y, 0f);
							fallClock = -1f;
							if (grid.TryGetValue(above, out check) && check == this) {
								grid.Remove(above);
							}
							if (grid.TryGetValue(below, out check) && check == this) {
								grid.Remove(below);
							}
						}
					}
				}
			}
			if (fallClock <= 0.0f && (!grid.TryGetValue(below, out check) || (check as FallingBlock != null && (check as FallingBlock).fallClock >= 0f))) {
				fallClock = 0.0f;
			}
			if (initiatePush > 0f && !blockManager.player.GetRoundedPosition().Equals(new Int2 (GetCurrentPosition().x - pushDirection, GetCurrentPosition().y))) {
				initiatePush = 0f;
			}
			if (initiatePush >= 0.3f) {
                gameManager.PlaySound("PushBlock");
				initiatePush = 0f;
				pushClock = 0f;
				pushStart = GetCurrentPosition().x;
				grid.Add (new Int2(GetCurrentPosition().x + pushDirection, GetCurrentPosition().y), this);
			}
			if (pushClock >= 0f) {
				pushClock += Time.deltaTime * 3;
				transform.position = new Vector3(pushStart + pushClock * pushDirection, GetCurrentPosition().y, 0f);;
				if (pushClock >= 1f) {
					pushClock = -1f;
					Int2 previous = GetCurrentPosition();
					previous.x -= pushDirection;
					grid.Remove (previous);
					transform.position = new Vector3(GetCurrentPosition().x, GetCurrentPosition().y, 0f);
				}
			}
		}
	}
}