using UnityEngine;
using System.Collections;

public class CrackedBlock : AbstractBlock {
	//displayed on the model.  if this is 0 and it's rotated again, after the rotation this block will diappear.
	//because this is public, it can be set individually from the unity scene, right?
	public int rotationsLeft;
	Transform number;
	public SpriteRenderer numberDisplay;

	public Sprite[] numberSprites;

	void Start()
	{
		numberDisplay = transform.FindChild ("numberDisplay").GetComponent<SpriteRenderer> ();

	}

	public override bool invalidatesRotation()
	{
		return false;
	}
	
	public override bool isRotatable()
	{
		return true;
	}

	void Update()
	{
		numberDisplay.sprite = numberSprites [rotationsLeft];
		//number.guiText.text = rotationsLeft.ToString();
	}

	/// <summary>
	/// To be called by the BlockManager after a rotation 
	/// decrements rotationsLeft
	/// Destroying this block is left to the BlockManager
	/// </summary>
	public void wasJustRotated(){
		rotationsLeft--;
		//number.guiText.text = rotationsLeft.ToString();
		//how do I do this?
//		numberDisplay.renderer.material.mainTexture = Resources.Load("Sprites/"+rotationsLeft+".png", Texture2D);	// Set the texture.  Must be in Resources folder.

	}
}
