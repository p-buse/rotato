using UnityEngine;
using System.Collections.Generic;
using System;

public class BlockManager : MonoBehaviour {
    
	public Dictionary<Int2, AbstractBlock> grid; // changed to public so I can see and change it in falling blocks.  Also changed the value type to AbstractBlock.
    private Dictionary<Int2, AbstractBlock> currentlyRotating;
	public Dictionary<Int2, AbstractBlock> justRotated;
    [HideInInspector]
    public Player player;

	//direction: 1 for clockwise, -1 for counterclockwise
    public bool isValidRotation(Int2 center, int direction)
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
			if(neighbors[position].invalidatesRotation()){
				return false;
			}
		}
		
		//Step 3:
		//iterate through neighbors
		foreach(var position in neighbors.Keys){
			if(neighbors[position].isRotatable()){
				//get theoretical new position of block after rotation
				Int2 newPos = neighbors[position].posAfterRotation(center, direction);

				AbstractBlock curNeighbor; //TryGetValue fills this with instance of block if key exists

				//get block at theoretical new position:
				if(neighbors.TryGetValue(newPos, out curNeighbor)){
					// if block currently located at future position will not rotate,
					//conflict, return false
					if(!curNeighbor.isRotatable()){
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
        // Find the player
        player = FindObjectOfType<Player>();
        // we start currently rotating nothing
        currentlyRotating = new Dictionary<Int2, AbstractBlock>();
        // Populate our grid with the blocks in the scene
        grid = new Dictionary<Int2, AbstractBlock>();
        // FindObjectsOfType is an expensive operation, so we only run it once per scene
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

    public void AddBlock(Int2 position, AbstractBlock block)
    {
        if (grid.ContainsKey(position))
        {
            grid.Remove(position);
        }
        grid.Add(position, block);
    }

    public void startRotation(Int2 center)
    {
        // Add the neighbors of the center to our currently rotating blocks
        this.currentlyRotating = getNeighbors(center);

        // And if we're centered on a block, add that one too
        AbstractBlock centerBlock;
        if (grid.TryGetValue(center, out centerBlock))
        {
            this.currentlyRotating.Add(center, centerBlock);
        }
    }

    public void AnimateFrameOfRotation(Int2 center, int direction, float time)
    {
        foreach (AbstractBlock b in currentlyRotating.Values)
        {
            if (b.isRotatable())
            {
                b.AnimateFrameOfRotation(center, direction, time);
            }
        }
    }

	/// <summary>
	/// Called by gameManager when release click: Handles the cracked blocks' health and destruction
	/// </summary>
	/// <param name="justRotated">blocks just rotated.</param>
	public void DecrementCracked(Dictionary<Int2, AbstractBlock> justRotated)
	{
		foreach (Int2 pos in justRotated.Keys) 
		{
			CrackedBlock cracked = justRotated [pos] as CrackedBlock;
			if (cracked != null) {
				//decrement the health of cracked blocks
				cracked.wasJustRotated ();
				//destroy them if necessary
				if (cracked.rotationsLeft < 1) 
				{
					grid.Remove (pos);
					//if this block broke because of the rotation, destroy it and don't keep it in the grid
					Destroy (cracked.gameObject);
					//TODO: add animation here
				} 
			}
		}
	}



	public void finishRotation(Int2 center, int direction)
    {
		
        // Rotate each of the blocks and update our list to match
        // Take each of these blocks out of the grid in turn
        foreach (Int2 pos in currentlyRotating.Keys)
        {
            if (currentlyRotating[pos].isRotatable())
            {
                grid.Remove(pos);
                currentlyRotating[pos].finishRotation(center, direction);
            }
		}

        // Put these rotated blocks back into the grid
		foreach (Int2 pos in currentlyRotating.Keys)
		{
            if (currentlyRotating[pos].isRotatable())
            {
                grid.Add(currentlyRotating[pos].GetCurrentPosition(), currentlyRotating[pos]);
            }
		}
		currentlyRotating = new Dictionary<Int2, AbstractBlock> ();
		justRotated = getNeighbors (center);

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

	public bool rotationEmpty() {
		return currentlyRotating.Count == 0;
		}

	/// <summary>
	/// Gets the block at position (x,y), x and y floats.
	/// for falling blocks, only returns it if inside its sprite
	/// </summary>
	/// <returns>The <see cref="AbstractBlock"/>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public AbstractBlock getBlockAt(float x, float y)
	{
		Int2 pos = new Int2 (x, y);
		AbstractBlock theBlock;
		if (grid.TryGetValue (pos, out theBlock)) 
		{
			if(theBlock as FallingBlock==null)
				return theBlock;
			else if(Mathf.Abs(y - theBlock.transform.position.y)<0.5)
			{
				return theBlock;
			}
		}
		return null;
	}


}
