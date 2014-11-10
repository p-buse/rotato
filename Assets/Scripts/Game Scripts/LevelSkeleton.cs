using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelSkeleton
{

    public List<BlockSkeleton> blocks = new List<BlockSkeleton>();

    public List<Int2> noRoZones = new List<Int2>();

    public Int2 playerPosition;
    public List<Vector2> crawlers = new List<Vector2>();

	//stores block grid
	public void setGrid(Dictionary<Int2, AbstractBlock> grid){

		blocks = new List<BlockSkeleton> ();

		foreach (AbstractBlock block in grid.Values) {
			blocks.Add(block.getSkeleton());
		}
	
	}

	//stores norozone grid
	public void setNoRoZoneGrid(Dictionary<Int2, NoRotationZone> noRoGrid){

		foreach (Int2 pos in noRoGrid.Keys)
        {
			this.noRoZones.Add(pos);
		}
	}

	public void setCrawlers()
	{
		GameObject[] crawlerObjects = GameObject.FindGameObjectsWithTag ("Crawler");
		foreach(GameObject crawler in crawlerObjects)
		{
			crawlers.Add(crawler.transform.position);
		}

	}
}
