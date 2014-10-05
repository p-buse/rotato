using UnityEngine;
using System.Collections;

public class Block : AbstractBlock {

    void Start()
    {

    }

<<<<<<< HEAD
    /*public override void AnimateFrameOfRotation(Int2 center, string direction, float time)
    {
        throw new System.NotImplementedException();
    }*/

=======
>>>>>>> 0d9ff62ef7f39ffefb3f152f6282260488330536
    public override bool invalidatesRotation()
    {
        return false;
    }

    public override bool isRotable()
    {
        return true;
    }
}
