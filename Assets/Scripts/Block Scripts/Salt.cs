using UnityEngine;
using System.Collections;

public class Salt : AbstractBlock {

	public int rotationsBeforeRemove;
	public TextMesh field;
	Player player;
	Int2 location;

	void Start() {
		player = GameObject.FindObjectOfType<Player>();
		location = new Int2(transform.position.x, transform.position.y);
		field = GetComponent<TextMesh>();
		field.text = "" + rotationsBeforeRemove;
        gameManager.PlayerCreated += this.PlayerCreated;
	}

    void PlayerCreated(GameManager gm, Player p, PlayerMovement pm)
    {
        this.player = p;
    }

	void Update() {
		if (player != null && player.GetRoundedPosition().Equals(location)) {
            gameManager.PlaySound("GetSalt");
            blockManager.grid.Remove(location);
			Destroy(this.gameObject);
			gameManager.addSalt();
		}
	}

	/// <summary>
	/// To be called by the GameManager after a rotation 
	/// decrements rotationsBeforeRemove
	/// Destroying this block is left to the GameManager
	/// </summary>
	public void justRotated(){
		if(rotationsBeforeRemove ==1)
		{
			for(int i=0;i<crawlers.Count;i++ )
			{
				CrawlerMovement c = crawlers[i];
				c.myBlock = null;
				c.falling = true;
			}
		}
		rotationsBeforeRemove--;
		
	}

	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotatable() {
		return false;
	}

	public override string myType ()
	{
		return "Salt";
	}
	
	public override void AnimateFrameOfRotation (Int2 center, int direction, float time) {}
	
	public override Int2 posAfterRotation(Int2 center, int dir) {
		return new Int2 (transform.position.x, transform.position.y);
	}
	
	public override void finishRotation(Int2 center, int dir)
	{
		
	}
}