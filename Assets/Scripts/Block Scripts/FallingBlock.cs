using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingBlock : AbstractBlock {

    public float fallClock;
	public bool whichHalf = true;
	public Int2 location;
	public float initiatePush;
	public float pushClock;
	public int pushDirection;

	void Start() {
        fallClock = -1f;
		initiatePush = 0f;
		pushClock = -1f;
        blockManager = GameObject.FindObjectOfType<BlockManager>();
		location = new Int2(this.transform.position.x, this.transform.position.y);
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
			if (fallClock >= 0.0f) {
				initiatePush = 0f;
				fallClock += Time.deltaTime*3;
				if (fallClock >= 1.0f && whichHalf) {
					fallClock -= 1.0f;
					whichHalf = false;
					if (grid.ContainsKey(location) && grid[location] == this) {
						grid.Remove(location);
					}
					location.y--;
					if (!grid.ContainsKey(location) || grid[location] != this) {
						grid.Remove(location);
						grid.Add(location, this);
					}
					Int2 below = new Int2(location.x, location.y-1);
					if (!grid.ContainsKey(below)) {
						grid.Add(below, this);
					}
					else if (grid.ContainsKey(below) && ((grid[below] as FallingBlock == null || (grid[below] as FallingBlock).fallClock < 0.0f))) {
						fallClock = -1.0f;
						transform.position = new Vector3(location.x, location.y, 0.0f);
					}
				}
				else if (fallClock >= 0.5f && !whichHalf) {
					whichHalf = true;
				}
				Int2 beneath = new Int2(location.x, location.y-1);
				if (!whichHalf && (grid.ContainsKey(beneath) && ((grid[beneath] as FallingBlock == null || (grid[beneath] as FallingBlock).fallClock < 0.0f)))) {
					fallClock = -1.0f;
					transform.position = new Vector3(location.x, location.y, 0.0f);
					if (!grid.ContainsKey(location) || grid[location] != this) {
						grid.Remove(location);
						grid.Add(location, this);
					}
					if(grid.ContainsKey(beneath) && grid[beneath] == this) {
						grid.Remove(beneath);
					}
				}
				if (!whichHalf && !grid.ContainsKey(beneath)) {
					grid.Add(beneath, this);
				}
				if (fallClock >= 0.0) {
					transform.position = new Vector3(location.x, location.y-fallClock, 0.0f);
				}
			}
			Int2 belowThis = new Int2(location.x, location.y-1);
			if (fallClock <= 0.0f && ((!grid.ContainsKey(belowThis) || (grid[belowThis] as FallingBlock != null && (grid[belowThis] as FallingBlock).fallClock >= 0.0f)))) {
				fallClock = 0.0f;
			}
			if (initiatePush > 0f && !blockManager.player.GetRoundedPosition().Equals(new Int2 (location.x - pushDirection, location.y))) {
				initiatePush = 0f;
			}
			if (initiatePush >= 0.3f) {
                gameManager.PlaySound("PushBlock");
				initiatePush = 0f;
				pushClock = 0f;
				grid.Add (new Int2(location.x + pushDirection, location.y), this);
			}
			if (pushClock >= 0f) {
				pushClock += Time.deltaTime * 3;
				transform.position = new Vector3(location.x + pushClock * pushDirection, location.y, 0f);
				if (pushClock >= 1f) {
					pushClock = -1f;
					grid.Remove (location);
					location.x += pushDirection;
					transform.position = new Vector3(location.x, location.y, 0f);
				}
			}
		}
	}
}