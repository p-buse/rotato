using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelEditorModel : MonoBehaviour {

    Dictionary<Int2, LevelBlock> grid = new Dictionary<Int2, LevelBlock>();

    public void AddBlockAt(Int2 positionToAdd, LevelBlock newBlock)
    {
        if (!grid.ContainsKey(positionToAdd))
        {
            grid[positionToAdd] = newBlock;
        }
        else
        {
            throw new Exception("Block already exists at: " + positionToAdd);
        }
    }

    public void ChangeBlockAt(Int2 positionToChange, LevelBlock newBlock)
    {
        if (grid.ContainsKey(positionToChange))
        {
            grid[positionToChange] = newBlock;
        }
        else
        {
            throw new Exception("No block already at: " + positionToChange);
        }
    }
}
