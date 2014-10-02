using UnityEngine;
using System.Collections;

public class StaticBlock : Block {

    public override void RotateTo(int x, int y, string direction)
    {
        // Rotation code here
    }

    public override bool invalidatesRotation()
    {
        return false;
    }
}
