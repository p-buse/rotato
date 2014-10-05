using UnityEngine;
using System.Collections;

public class Block : AbstractBlock {

    void Start()
    {

    }

    public override bool invalidatesRotation()
    {
        return false;
    }

    public override bool isRotable()
    {
        return true;
    }
}
