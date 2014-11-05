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

		foreach (Int2 pos in grid)
        {
			noRoZones.Add(pos);
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

    public static void WriteXML()
    {
        string path = @"c:\temp\SerializationOverview.xml";
        LevelSkeleton level = new LevelSkeleton();
        level.blocks.Add(new BlockSkeleton("block", new Int2(0, 0), 0));
        level.blocks.Add(new BlockSkeleton("fixed", new Int2(-5, 2), 2));
        level.blocks.Add(new BlockSkeleton("cannon", new Int2(-10, 2), 0));
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
