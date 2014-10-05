using UnityEngine;
using System.Collections;

public class Block : AbstractBlock {

    void Start()
    {

    }

    /*public override void AnimateFrameOfRotation(Int2 center, string direction, float time)
    {
        throw new System.NotImplementedException();
    }*/

    public override bool invalidatesRotation()
    {
        return false;
    }

    public override bool isRotable()
    {
        return true;
    }
}
