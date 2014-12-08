using UnityEngine;
using System.Collections;

public class CursorScript : MonoBehaviour
{

    enum CursorState { NEUTRAL = 0, ROTATABLE = 1, NO = 2, HIDDEN = 3 };
    CursorState cursorState;
    GameManager gameManager;
    Animator animator;

    void Awake()
    {
        this.gameManager = FindObjectOfType<GameManager>();
        this.animator = GetComponentInChildren<Animator>();
        this.cursorState = 0;
    }
    void Update()
    {
        if (gameManager.gameState == GameManager.GameMode.editing)
        {
            Screen.showCursor = true;
        }
        else
        {
            Screen.showCursor = false;
        }
        this.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!gameManager.gameFrozen)
        {
            if (gameManager.MouseIsInRotatableArea())
            {
                this.ChangeCursorState(1);
            }
            else if (gameManager.MouseIsInNoRoZone())
            {
                this.ChangeCursorState(2);
            }
            else
            {
                this.ChangeCursorState(0);
            }
        }
        else
        {
            this.ChangeCursorState(3);
        }
    }

    void ChangeCursorState(int newCursorState)
    {
        this.cursorState = (CursorState)newCursorState;
        this.animator.SetInteger("cursorState", (int)this.cursorState);
    }

    void OnDestroy()
    {
        Screen.showCursor = true;
    }

}
