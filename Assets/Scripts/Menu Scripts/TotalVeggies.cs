using UnityEngine;
using System.Collections;

public class TotalVeggies : MonoBehaviour {

	int totalVeggies = 0;
	int currentVeggies = 0;
	TypogenicText text;

	void Start() {
		text = GetComponent<TypogenicText>();
	}

	void Update() {
		text.Text = "Veggies rescued: " + currentVeggies + "/" + totalVeggies;
	}

	public void addVeggieCounts(int veggies, int maxVeggies) {
		totalVeggies += maxVeggies;
		currentVeggies += veggies;
	}
}