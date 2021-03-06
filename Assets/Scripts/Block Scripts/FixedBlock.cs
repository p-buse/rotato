﻿using UnityEngine;
using System.Collections;

public class FixedBlock : AbstractBlock {
	
	public override bool invalidatesRotation() {
		return false;
	}
	
	public override bool isRotatable() {
		return false;
	}

	public override string myType ()
	{
		return "Fixed";
	}

	public override void AnimateFrameOfRotation (Int2 center, int direction, float time) {}

	public override Int2 posAfterRotation(Int2 center, int dir) {
		return new Int2 (transform.position.x, transform.position.y);
	}

    public override void finishRotation(Int2 center, int dir)
    {
        
    }
}