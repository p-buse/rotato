using UnityEngine;
using System.Collections;

public class SpikyBlock : MonoBehaviour
{
    static GameManager gameManager;

    void Awake()
    {
        SpikyBlock.gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("couldn't find game manager");
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        // If the player touches the spikes, skewer 'em! 
        if (coll.gameObject.tag == "Player")
        {
            this.SkewerPlayer();
        }
    }

    public void SkewerPlayer()
    {
        gameManager.ResetLevel();
    }
}
