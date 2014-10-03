using UnityEngine;
using System.Collections;

public class StaticBlock : Block {

    public override void RotateTo(Int2 center, string direction)
    {
        // Rotation code here
    }

    public override bool invalidatesRotation()
    {
        return false;
    }
}
