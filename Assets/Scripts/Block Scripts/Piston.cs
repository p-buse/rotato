using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piston : AbstractBlock {

	public Direction dir; 
	public enum Direction { Right = 0, Left = 2, Up = 1, Down = 3 };

	private Int2 start;

	private float xMult;
	private float yMult;

	void Awake(){
		start = new Int2(this.transform.position.x, this.transform.position.y);

		switch (dir) {
			case Direction.Right:
				xMult = 1f;
				yMult = 0f;
				break;
			case Direction.Left:
				xMult = -1f;
				yMult = 0f;
				break;
			case Direction.Up:
				xMult = 0f;
				yMult = 1f;
				break;
			case Direction.Down:
				xMult = 0f;
				yMult = -1f;
				break;
		}
	}


	public override bool invalidatesRotation ()
	{
		return true;
	}


	public override bool isRotatable ()
	{
		return false;
	}

	public override string myType ()
	{
		return "Block";
	}

	void Update(){

		if(!gameManager.gameFrozen) {
			//get the grid to edit as piston moves:
			Dictionary<Int2, AbstractBlock> grid = blockManager.grid;

		}
	}
}