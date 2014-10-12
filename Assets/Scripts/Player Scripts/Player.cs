using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public KeyCode resetButton = KeyCode.R;
    Plane[] cameraView;
    GameManager gameManager;
	BlockManager blockManager;

    void Start()
    {
        cameraView = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        this.gameManager = FindObjectOfType<GameManager>();
		this.blockManager = FindObjectOfType<BlockManager>();
    }

    void Update()
    {
		Int2 position = this.GetRoundedPosition();
		Int2 above = new Int2 (position.x, position.y + 1);
		Int2 below = new Int2 (position.x, position.y - 1);
		Dictionary<Int2, AbstractBlock> grid = blockManager.grid;
        if (Input.GetKeyDown(resetButton) || !GeometryUtility.TestPlanesAABB(cameraView, this.collider2D.bounds) || 
		    (grid.ContainsKey(above) && grid[above] as FallingBlock != null && !(grid[above] as FallingBlock).whichHalf && 
		 	(!grid.ContainsKey(new Int2(above.x, above.y+1)) || grid[new Int2(above.x, above.y+1)] != grid[above]) && 
		 	grid.ContainsKey(below) && (grid[below] as FallingBlock == null || (grid[below] as FallingBlock).fallClock < 0.0f) && !gameManager.gameFrozen))
        {
            gameManager.ResetLevel();
        }
    }
    public Int2 GetRoundedPosition()
    {
        return new Int2(transform.position.x, transform.position.y);
    }
}
