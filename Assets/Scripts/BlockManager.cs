using UnityEngine;
using System.Collections.Generic;
using System;

public class BlockManager : MonoBehaviour {
    private Dictionary<Int2, Block> grid;
    Player player;

    private bool isValidRotation(Vector2 center, int direction)
    {
        // Check if a rotation zone is valid
        return false;
    }

	// Awake is called before Start, and triggers even if the object is deactivated
	void Awake ()
    {
        // Get the player
        player = FindObjectOfType<Player>();
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

    void Update()
    {
        Int2[] rotateZone = new Int2[1];
        foreach (Int2 position in rotateZone)
        {
            // Rotate the blocks here
        }
    }
	
}
