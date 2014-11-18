using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piston : AbstractBlock {

	public Direction dir; 
	public enum Direction { Right = 0, Left = 2, Up = 1, Down = 3 };

	private Int2 start;

	private float xMult;
	private float yMult;

	private float clock;

	void Awake(){

		blockManager = GameObject.FindObjectOfType<BlockManager>();

		start = GetCurrentPosition();

		setDir(dir);

		clock = 1f;
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
		return "Piston";
	}

	void Update(){

		if(!gameManager.gameFrozen) {

			clock += Time.deltaTime;

			//get the grid to edit as piston moves:
			Dictionary<Int2, AbstractBlock> grid = blockManager.grid;

			//time to check if we have to turn around:
			if(clock >= 1f){

				//remove last block:
				Int2 lastBlock = new Int2(this.transform.position.x + -1*xMult,this.transform.position.y + -1*yMult);
				grid.Remove(lastBlock);

				//if arrived at start position:
				if(GetCurrentPosition().Equals(start)){
					switchDir();
					//snap to grid:
					blockSprite.position = new Vector3(start.x, start.y, 0);
					this.transform.position = new Vector3(start.x, start.y, 0);

					return;
				}

				//if something there:
				var nextBlock = new Int2((this.transform.position.x + xMult), (this.transform.position.y + yMult));

				if(grid.ContainsKey(nextBlock)){

					//snap to perfect grid for this frame:
					blockSprite.position = new Vector3(this.transform.position.x, this.transform.position.x, 0);

					//switch direction:
					switchDir();
					return;
					   
				}

				//add next block:
				grid.Add(nextBlock, this);

				//snap gameobject transform to current block:
				this.transform.position = new Vector3(this.transform.position.x + 1*xMult, this.transform.position.y + 1*yMult, 0);

				return;

			}

			blockSprite.transform.position += new Vector3(xMult*clock, yMult*clock, 0);



		}
	}

	void setDir(Direction dir){

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
		this.dir = dir;
	
	}

	void switchDir(){

		switch (this.dir) {
		case Direction.Right:
			setDir(Direction.Left);
			break;
		case Direction.Left:
			setDir(Direction.Right);
			break;
		case Direction.Up:
			setDir(Direction.Down);
			break;
		case Direction.Down:
			setDir(Direction.Up);
			break;
		}
	}
}