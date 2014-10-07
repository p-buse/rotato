using UnityEngine;
using System.Collections;

public class Highlighting : MonoBehaviour {

    Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        transform.position = player.GetRoundedPosition().ToVector2();
    }
}
