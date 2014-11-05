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
		crawlers = new List<float[]> ();
		GameObject[] crawlerObjects = GameObject.FindGameObjectsWithTag ("Crawler");
		foreach(GameObject crawler in crawlerObjects)
		{

			crawlers.Add(new float[]{crawler.transform.position.x, crawler.transform.position.y});
		}

	}

    public static void WriteXML()
    {
        string path = @"c:\temp\SerializationOverview.xml";
        LevelSkeleton level = new LevelSkeleton();
        level.blocks.Add(new BlockSkeleton(1, 3, 5, 0));
        level.blocks.Add(new BlockSkeleton(3, 2, 5, 0));
        level.playerPosition = new Int2(3, 5);
        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(LevelSkeleton));
        System.IO.StreamWriter file = new System.IO.StreamWriter(path);
        Debug.Log("Wrote level to " + path);
        writer.Serialize(file, level);
        file.Close();
    }

    void Start()
    {
        WriteXML();
    }
}
