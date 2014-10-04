using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public Int2 GetRoundedPosition()
    {
        return new Int2(transform.position.x, transform.position.y);
    }

}
