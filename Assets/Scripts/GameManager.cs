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
					if(Input.GetMouseButtonDown(0))
					{
						Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						int x = Mathf.RoundToInt(worldPos.x);
						int y = Mathf.RoundToInt(worldPos.y);
						if(isValidCenter(new Int2(x,y))){
							gameState = RotationMode.frozen;
						}
						
					}
                    break;
                }

            case RotationMode.frozen: //game is frozen, left-click is held, but no rotation is happening
                {
					if(Input.GetMouseButtonUp(0))
					{
						gameState = RotationMode.playing;
					}
					//detect keyboard input, call blockmanager rotation, set gameState to RotationMode.rotating
                    break;
                }

            case RotationMode.rotating:
                {
					//check BlockManager for rotation clock, if it's done set gameState back to RotationMode.frozen
                    break;
                }
        }
    }

	public bool isValidCenter(Int2 xy){
		Int2 playerPos = blockManager.player.GetRoundedPosition();
		int absDx = Mathf.Abs (xy.x - playerPos.x);
		int absDy = Mathf.Abs (xy.y - playerPos.y);
		if ((absDx <= 2 && absDy <= 2) && (absDx == 2 || absDy == 2 || (absDx == 0 && absDy == 0))) 
		{
			return true;
		}
		return false;
	}

}
