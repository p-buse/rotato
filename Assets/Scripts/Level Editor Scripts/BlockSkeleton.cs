using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BlockSkeleton {

    public string name;
    public Int2 position;
    public int orientation;
    public int rotationsTillDeath = 0;
    public List<int> spikiness;

    public BlockSkeleton(string name, Int2 position, int orientation, List<int> spikiness, int rotationsTillDeath = 0)
    {
        this.name = name;
        this.position = position;
        this.orientation = orientation;
        this.rotationsTillDeath = rotationsTillDeath;
        this.spikiness = spikiness;
    }

    public BlockSkeleton()
    {
    }

}