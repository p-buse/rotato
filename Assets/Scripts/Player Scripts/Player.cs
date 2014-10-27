using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    GameManager gameManager;
	BlockManager blockManager;

    void Start()
    {
        this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
    }

    void Update()
    {
		Int2 position = this.GetRoundedPosition();
		Dictionary<Int2, AbstractBlock> grid = blockManager.grid;
        if (this.CrushedByBlock(grid, position))
        {
            gameManager.ResetLevel();
        }
    }

    bool CrushedByBlock(Dictionary<Int2, AbstractBlock> grid, Int2 position)
    {
        Int2 above = new Int2 (position.x, position.y + 1);
	    Int2 below = new Int2 (position.x, position.y - 1);
        return (grid.ContainsKey(above) && grid[above] as FallingBlock != null && !(grid[above] as FallingBlock).whichHalf && 
		 	(!grid.ContainsKey(new Int2(above.x, above.y+1)) || grid[new Int2(above.x, above.y+1)] != grid[above]) && 
		 	grid.ContainsKey(below) && (grid[below] as FallingBlock == null || (grid[below] as FallingBlock).fallClock < 0.0f) && !gameManager.gameFrozen);
    }
    public Int2 GetRoundedPosition()
    {
        return new Int2(transform.position.x, transform.position.y);
    }
}
