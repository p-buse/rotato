using UnityEngine;
using System.Collections.Generic;
using System;

public class BlockManager : MonoBehaviour {
    
	public Dictionary<Int2, AbstractBlock> grid; // changed to public so I can see and change it in falling blocks.  Also changed the value type to AbstractBlock.
    Player player;

	//direction: 1 for clockwise, -1 for counterclockwise
    private bool isValidRotation(Int2 center, int direction)
    {
        // Check if a rotation zone is valid

		/* Step 0: check if the center is at an edge, return false if it is
		 * 
		 * Step 1:  get neighbors of center
		 * Step 2:  call invalidatesRotation on neighbors, return false if any return false
		 * Step 3:  for each neighbor w/ isRotatble true, check if their updated position 
		 * 			contains a block w/ isRotatable false
		 * 			if either return false, return false
		 * Step 4:  return true
		 */


		//Step 1:
		//get neighbors
		Dictionary<Int2,AbstractBlock> neighbors = getNeighbors(center);


		//Step 2:
		//iterate through neighbors
		foreach(var position in neighbors.Keys){

			//if neighbor invalidates rotation, return false:
			if(!neighbors[position].invalidatesRotation()){
				return false;
			}
		}
		
		//Step 3:
		//iterate through neighbors
		foreach(var position in neighbors.Keys){
			if(neighbors[position].isRotable()){
				//get theoretical new position of block after rotation
				Int2 newPos = neighbors[position].posAfterRotation(center, direction);

				AbstractBlock curNeighbor; //TryGetValue fills this with instance of block if key exists

				//get block at theoretical new position:
				if(neighbors.TryGetValue(newPos, out curNeighbor)){
					// if block currently located at future position will not rotate,
					//conflict, return false
					if(!curNeighbor.isRotable()){
						return false;
					}

				}
			}
		}

		//Step 4:
        return true;
    }

	// Awake is called before Start, and triggers even if the object is deactivated
	void Awake ()
    {
        // Get the player
        player = FindObjectOfType<Player>();
        // Populate our grid with the blocks in the scene
        // FindObjectsOfType is an expensive operation, so we only run it once per scene
        grid = new Dictionary<Int2, AbstractBlock>();
        AbstractBlock[] blocks = GameObject.FindObjectsOfType<AbstractBlock>();
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

	//returns a dictionary contining the non-empty neighbors of a center, not including the center
	private Dictionary<Int2, AbstractBlock> getNeighbors(Int2 center){

		Dictionary<Int2, AbstractBlock> neighbors = new Dictionary<Int2, AbstractBlock>();
		Int2 position;

		//iterate from (center-1,center-1) to (center+1,center+1)
		for (int i=-1; i<2; i++) {
			for (int j=-1; j<2; j++){

				position = new Int2(center.x + i, center.y + j);

				if(grid.ContainsKey(position)){
					neighbors.Add(position, grid[position]);
				}
			}
		}

		//make sure we don't return center as a neighbor:
		if(neighbors.ContainsKey(center)){
			neighbors.Remove(center);
		}

		return neighbors;

	}
}
