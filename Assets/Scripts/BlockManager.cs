using UnityEngine;
using System.Collections.Generic;
using System;

public class BlockManager : MonoBehaviour {
    public Dictionary<Int2, AbstractBlock> grid; // changed to public so I can see and change it in falling blocks.  Also changed the value type to AbstractBlock.
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
        grid = new Dictionary<Int2, AbstractBlock>();
        Block[] blocks = GameObject.FindObjectsOfType<AbstractBlock>();
        foreach (AbstractBlock b in blocks)
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
