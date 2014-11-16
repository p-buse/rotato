using UnityEngine;
using System.Collections;

public class SpikyBlock : MonoBehaviour
{
    static GameManager gameManager;
    public SpriteRenderer spikeSprite;

    void Awake()
    {
        SpikyBlock.gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("couldn't find game manager");
        }
        spikeSprite = transform.parent.GetComponentInChildren<SpriteRenderer>();
    }

    public void SetSpikeDirection(string direction)
    {
        //switch(direction)
        //{
        //    case "left":
        //        transform.parent.
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!gameManager.gameFrozen)
        {
            // If the player touches the spikes, skewer 'em! 
            if (coll.gameObject.tag == "Player")
            {
                gameManager.PlaySound("Skewer");
                this.SkewerPlayer();
            }
        }
    }

    public void SkewerPlayer()
    {
        gameManager.LoseLevel("Skewered by spikes!");
    }
}
