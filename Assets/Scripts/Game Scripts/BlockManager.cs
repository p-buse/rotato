﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class BlockManager : MonoBehaviour {

    GameManager gameManager;
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
		foreach(Int2 position in neighbors.Keys){

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
        gameManager = GetComponent<GameManager>();
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
        // Subscribe the the PlayerCreated event
        gameManager.PlayerCreated += this.PlayerCreated;
        
	}

    void PlayerCreated(GameManager gameManager, Player player, PlayerMovement playerMovement)
    {
        this.player = player;
    }

    public void AddBlock(Int2 position, AbstractBlock block)
    {
		RemoveBlock(position);
        grid.Add(position, block);
    }

    public void RemoveBlock(Int2 position)
    {
        AbstractBlock alreadyThere;
        if (grid.TryGetValue(position, out alreadyThere))
        {
            grid.Remove(position);
			if (alreadyThere as FallingBlock != null) {
				Int2 above = new Int2(position.x, position.y + 1);
				AbstractBlock check;
				if (grid.TryGetValue(above, out check) && check == alreadyThere) {
					grid.Remove(above);
				}
				Int2 below = new Int2(position.x, position.y - 1);
				if (grid.TryGetValue(below, out check) && check == alreadyThere) {
					grid.Remove(below);
				}
			}
            if (alreadyThere != null) {
                Destroy(alreadyThere.gameObject);
			}
        }
    }

    public void ChangePos(Int2 original, Int2 newPos)
    {
        AbstractBlock alreadyThere;
        if (grid.TryGetValue(original, out alreadyThere))
        {
            grid.Remove(original);
            this.AddBlock(newPos, alreadyThere);
            alreadyThere.transform.position = new Vector3(newPos.x, newPos.y, alreadyThere.transform.position.z);
        }
        else
        {
            Debug.LogWarning("Couldn't find block at: " + original);
        }
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
				if (cracked.shouldBeDead) 
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
		if (currentlyRotating.Count == 0) {
			return true;
		}
		foreach (AbstractBlock currentBlock in currentlyRotating.Values) {
			if (currentBlock.isRotatable()) {
				return false;
			}
		}
		return true;
	}

    public AbstractBlock getBlockAt(Int2 pos)
    {
        return this.getBlockAt(pos.x, pos.y);
    }

	/// <summary>
	/// Gets the block at position (x,y), x and y floats.
	/// for falling blocks, lasers, and mirrors, only returns it if inside its sprite (its collider)
	/// </summary>
	/// <returns>The block, if it exists, or null if it doesn't.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public AbstractBlock getBlockAt(float x, float y)
	{
		Int2 pos = new Int2 (x, y);
		AbstractBlock theBlock;
		if (grid.TryGetValue (pos, out theBlock) && theBlock != null) 
		{
			if(theBlock.isPointInside(x,y))
			{
				return theBlock;
			}
		}
		return null;
	}



    public void DestroyAllBlocks()
    {
        foreach (Int2 pos in this.grid.Keys)
        {
            if (grid[pos] != null)
            {
                Destroy(grid[pos].gameObject);
            }
        }
        this.grid = new Dictionary<Int2, AbstractBlock>();
    }
}
