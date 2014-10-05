using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    BlockManager blockManager;
    enum RotationMode { playing, frozen, rotating };
    RotationMode gameState = RotationMode.playing;
    public bool rotationHappening
    {
        get
        {
            if (gameState == RotationMode.playing)
                return false;
            else
                return true;
        }
    }

    void Awake()
    {
        this.blockManager = FindObjectOfType<BlockManager>();
    }

    public void RegisterClick(float clickx, float clicky)
    {
    }

    void Update()
    {
        switch (gameState)
        {
            case RotationMode.playing:
                {
                    break;
                }
            case RotationMode.frozen:
                {
                    break;
                }
            case RotationMode.rotating:
                {
                    break;
                }
        }
    }

}
