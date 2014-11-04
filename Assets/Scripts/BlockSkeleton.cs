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
	public int rotationsTillDeath=0;

	/// <summary>
	/// Store the information about this block in this class.
	/// For blocks other than salt and cracked, rotationsTillDeath defaults to 0
	/// </summary>
	/// <param name="type1">Type1.</param>
	/// <param name="x1">The first x value.</param>
	/// <param name="y1">The first y value.</param>
	/// <param name="orientation1">Orientation1.</param>
	/// <param name="rotationsTillDeath1">Rotations till death1.</param>
	public void create(int type1, int x1, int y1, int orientation1, int rotationsTillDeath1)
	{
		type = type1;
		x = x1;
		y = y1;
		orientation = orientation1;
		rotationsTillDeath = rotationsTillDeath1;
	}

}