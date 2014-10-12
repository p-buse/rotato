﻿using UnityEngine;
using System.Collections;

public class Block : AbstractBlock {
		
    public override bool invalidatesRotation()
    {
        return false;
    }

    public override bool isRotatable()
    {
        return true;
    }
}