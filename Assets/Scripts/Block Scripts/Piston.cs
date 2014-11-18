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

	private Int2 nextBlock;
	private Int2 lastBlock;


	bool isReturning;

	void Start(){

		blockManager = GameObject.FindObjectOfType<BlockManager>();

		start = GetCurrentPosition();

		setDir(dir);

		clock = 0f;

		isReturning = false;
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

			lastBlock = new Int2(this.transform.position.x + -1f*xMult,this.transform.position.y + -1f*yMult);
			nextBlock = new Int2(this.transform.position.x + 1f*xMult,this.transform.position.y + 1f*yMult);

			//time to check if we have to turn around:
			if(clock >= 1f){

				//remove last block:
				grid.Remove(lastBlock);

				clock = 0f;

				//if arrived at start position:
				if(GetCurrentPosition().Equals(start) && isReturning == true){

					isReturning = false;

					switchDir();
					//snap to grid:
					//blockSprite.position = new Vector3(start.x, start.y, 0);

					return;
				}

				//if something there:
				if(grid.ContainsKey(nextBlock)){

					isReturning = true;

					//snap to perfect grid for this frame:
					//blockSprite.position = new Vector3(this.transform.position.x, this.transform.position.x, 0);

					//switch direction:
					switchDir();

					//add last block:
					grid.Add(lastBlock,this);

					nextBlock = lastBlock;

					//Int2 pos = new Int2(blockSprite.transform.position.x,blockSprite.transform.position.y);
					//this.transform.position = new Vector3(pos.x, pos.y,0);

					return;
					   
				}

				//add next block:
				grid.Add(nextBlock, this);

				return;

			}

			//blockSprite.transform.position = new Vector3(blockSprite.transform.position.x + xMult*Time.deltaTime, blockSprite.transform.position.y + yMult*Time.deltaTime, 0);
			this.transform.position = new Vector3(blockSprite.transform.position.x + xMult*Time.deltaTime, blockSprite.transform.position.y + yMult*Time.deltaTime, 0);


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