using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingBlock : AbstractBlock {

	float fallClock = -1.0f;
	BlockManager blockManager = GameObject.FindObjectOfType<BlockManager>();
	bool whichHalf = true;
	Int2 location;

	void Start() {
		location = new Int2(this.transform.position.x, this.transform.position.y);
	}
	
	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotable() {
		return fallClock < 0.0f;
	}

	void Update() {
		if (!gameManager.rotateMode) {
			Dictionary<Int2, AbstractBlock> grid = blockManager.grid;
			if (fallClock >= 0.0f) {
				fallClock += Time.deltaTime*3;
				if (fallClock >= 1.0f && whichHalf) {
					fallClock -= 1.0f;
					whichHalf = false;
					if (grid.ContainsKey(location) && grid[location] == this) {
						grid.Remove(location);
					}
					location.y--;
					if (grid.ContainsKey(location) && grid[location] != this) {
						grid.Remove(location);
						grid.Add(location, this);
					}
					Int2 below = new Int2(location.x, location.y-1);
					if (!grid.ContainsKey(below) && !below.Equals(blockManager.player.GetRoundedPosition())) {
						grid.Add(below, this);
					}
					else if (grid[below] as FallingBlock == null || (grid[below] as FallingBlock).fallClock < 0.0f || below.Equals(blockManager.player.GetRoundedPosition())) {
						fallClock = -1.0f;
						transform.position = new Vector3(location.x, location.y, 0.0f);
					}
				}
				else if (fallClock >= 0.5f && !whichHalf) {
					whichHalf = true;
				}
				if (fallClock >= 0.0) {
					transform.position = new Vector3(location.x, location.y-fallClock, 0.0f);
				}
			}
			Int2 below = new Int2(location.x, location.y-1);
			else if ((!grid.ContainsKey(below) || (grid[below] as FallingBlock != null && (grid[below] as FallingBlock).fallClock >= 0.0f)) && !below.Equals(blockManager.player.GetRoundedPosition())) {
				fallClock = 0.0f;
			}
		}
	}
}