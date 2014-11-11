using UnityEngine;
using System.Collections;

public class Spinny : MonoBehaviour {
    public float spinSpeed;

    void OnMouseOver()
    {
        transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * spinSpeed));
    }
}
