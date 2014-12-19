using UnityEngine;
using System.Collections;

public class ButterBlock : AbstractBlock {
    Animator cageAnim;
    Animator rotaterTotAnim;

    void Start()
    {
        this.cageAnim = GetComponent<Animator>();
        if (cageAnim == null)
            Debug.LogWarning("couldn't find cage animation");
        this.rotaterTotAnim = transform.FindChild("rotater-tot").GetComponent<Animator>();
        if (rotaterTotAnim == null)
            Debug.LogWarning("couldn't find rotater tot animation");
    }

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

    void OnTriggerEnter2D(Collider2D coll)
    {
		if (coll.gameObject.tag == "Player" && gameManager.gameState == GameManager.GameMode.playing && heated == 0)
        {
            cageAnim.SetTrigger("openCage");
            rotaterTotAnim.SetTrigger("setFree");
            gameManager.PlaySound("Win");
            gameManager.WinLevel(2f);
        }
		else if (coll.gameObject.tag == "Player" && heated > 0 && gameManager.gameState == GameManager.GameMode.playing) {
			gameManager.PlaySound("Burnt");
			gameManager.LoseLevel("Burnt by a hot block!");
		}
	}
}