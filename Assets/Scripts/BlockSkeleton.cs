using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BlockSkeleton {

    public string name;
    public Int2 position;
	public int orientation;

    public BlockSkeleton(string name, Int2 position, int orientation)
    {
        this.name = name;
        this.position = position;
        this.orientation = orientation;
    }

    public BlockSkeleton()
    {
    }

}