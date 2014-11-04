using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class LevelSkeleton : MonoBehaviour
{

	public List<BlockSkeleton> blocks;

	//for each item in list: item[0] is x position, item[1] is y position:
	public List<int[]> noRoZones; 

	public int[] player;
	public List<int[]> crawlers;

	//stores block grid
	public void setGrid(Dictionary<Int2, AbstractBlock> grid){

		blocks = new List<BlockSkeleton> ();

		foreach (var block in grid.Values) {
			blocks.Add(block.getSkeleton());
		}
	
	}

	//stores norozone grid
	public void setNoRoZoneGrid(HashSet<Int2> grid){

		foreach (var zone in grid) {	
			int[] pos = new int[2];
			pos[0] = zone.x;
			pos[1] = zone.y;

			noRoZones.Add(pos);
		}

	}

	public void setCrawlers()
	{
		crawlers = new List<int[]> ();
		GameObject[] crawlerObjects = GameObject.FindGameObjectsWithTag ("Crawler");
		foreach(GameObject crawler in crawlerObjects)
		{
			crawlers.Add(new int[]{crawler.transform.position.x, crawler.transform.position.y});
		}

	}
	
}
