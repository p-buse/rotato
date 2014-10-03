using UnityEngine;
using System.Collections.Generic;
using System;

public class BlockManager : MonoBehaviour {
    private Dictionary<Int2, Block> grid;

	// Awake is called before Start, and triggers even if the object is deactivated
	void Awake ()
    {
        
        // Populate our grid with the blocks in the scene
        // FindObjectsOfType is an expensive operation, so we only run it once per scene
        grid = new Dictionary<Int2, Block>();
        Block[] blocks = GameObject.FindObjectsOfType<Block>();
        foreach (Block b in blocks)
        {
            Int2 blockPosition = new Int2(b.transform.position.x, b.transform.position.y);
            // Detect overlapping blocks
            if (!grid.ContainsKey(blockPosition))
            {
                grid.Add(blockPosition, b);
            }
            else
            {
                Debug.LogError(String.Format("Detected two or more blocks at: {0}", blockPosition));
            }
        }
	}
	
}
