using UnityEngine;
using System.Collections;

public class BackgroundObjectMovement : MonoBehaviour {
	private GameManager gameManager;
	public float moveRatioX;
	public float moveRatioY;
	private Vector3 lastPlayerPos;

	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		lastPlayerPos = FindObjectOfType<Player> ().transform.position;
	}


	void Update () {
		Vector3 playerMoved = gameManager.player.transform.position-lastPlayerPos;

		//right now this is settable for different ratios in x and y directions.  is that necessary?
		Vector3 myMove = new Vector3 (-moveRatioX*playerMoved.x, -moveRatioY*playerMoved.y, 0);

		transform.Translate (myMove);
		lastPlayerPos = gameManager.player.transform.position;
	}
}
