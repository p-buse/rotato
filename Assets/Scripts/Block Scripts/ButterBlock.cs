using UnityEngine;
using System.Collections;

public class ButterBlock : AbstractBlock {

	public override string myType ()
	{
		return "Butter";
	}

	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotatable() {
		return false;
	}
	
	public override void AnimateFrameOfRotation (Int2 center, int direction, float time) {}
	
	public override Int2 posAfterRotation(Int2 center, int dir) {
		return new Int2 (transform.position.x, transform.position.y);
	}

    public override void finishRotation(Int2 center, int dir)
    {
        
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
		if (coll.gameObject.tag == "Player" && gameManager.gameState == GameManager.GameMode.playing && heat < 0.38f)
        {
            gameManager.PlaySound("Win");
            gameManager.WinLevel();
        }
    }
}