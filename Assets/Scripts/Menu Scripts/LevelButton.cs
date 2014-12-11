using UnityEngine;
using System.Collections;

public class LevelButton : MonoBehaviour {

	public bool isTutorial;
	public int levelIndexInBuildOrder;
	public int levelIndexInGame;
	public float zoomSpeed = 0.4f;
	public float zoomFactor = 1.5f;
	float originalZoom;
	int bestVeggies;
	int maxVeggies;
	int bestRotations;
	TypogenicText levelText;
	SpriteRenderer square;

	void Start () {
		this.originalZoom = transform.localScale.x;
		levelText = transform.Find("Text").GetComponent<TypogenicText>();
		square = transform.Find("Square").GetComponent<SpriteRenderer>();
		GameDataSkeleton.LevelDataSkeleton thisLevel;
		if (!isTutorial) {
			if (GameData.instance.TryGetLevel(levelIndexInBuildOrder, out thisLevel)) {
				bestVeggies = thisLevel.yourBestVeggies;
				maxVeggies = thisLevel.veggieCount;
				bestRotations = thisLevel.fewestRotations;
				string rotations = "";
				if (bestRotations == int.MaxValue) {
					rotations = "?";
				}
				else {
					rotations = bestRotations + "";
				}
				levelText.Text = "Level " + levelIndexInGame + "\nVeggies: " + bestVeggies + "/" + maxVeggies + "\nRotations: " + rotations;
			}
			else {
				bestVeggies = 0;
				maxVeggies = 0;
				bestRotations = -1;
				levelText.Text = "Level " + levelIndexInGame;
			}
			if (levelIndexInBuildOrder > GameData.instance.GetUnlockedLevel()) {
				square.color = new Color(square.color.r * 0.5f, square.color.g * 0.5f, square.color.b * 0.5f);
			}
		}
		else {
			levelText.Text = "Tutorial";
		}
	}
	void Update() {
		Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (collider2D.bounds.Contains(mouse))
		{
			transform.localScale = Vector3.Lerp(transform.lossyScale, new Vector3(zoomFactor, zoomFactor, zoomFactor), zoomSpeed);
		}
		else
		{
			transform.localScale = Vector3.Lerp(transform.lossyScale, new Vector3(originalZoom, originalZoom, originalZoom), zoomSpeed);
		}
	}
	
	void OnMouseUpAsButton() {
		if (levelIndexInBuildOrder <= GameData.instance.GetUnlockedLevel()) {
			Application.LoadLevel(levelIndexInBuildOrder);
		}
	}
}
