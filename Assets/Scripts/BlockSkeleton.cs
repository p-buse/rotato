using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class BlockSkeleton : MonoBehaviour {

	public int type;
	public int x;
	public int y;
	public int orientation;

	public void create(int type1, int x1, int y1, int orientation1)
	{
		type = type1;
		x = x1;
		y = y1;
		orientation = orientation1;
	}

}