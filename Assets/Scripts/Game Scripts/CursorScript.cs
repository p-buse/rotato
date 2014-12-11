using UnityEngine;
using System.Collections;

public class CursorScript : MonoBehaviour
{
    public Sprite cursorNeutral;
    public Sprite cursorNo;
    public Sprite cursorRotatable;
    enum CursorState { NEUTRAL = 0, ROTATABLE = 1, NO = 2, HIDDEN = 3 };
    CursorState cursorState;
    GameManager gameManager;
    SpriteRenderer cursorSprite;

    void Awake()
    {
        this.gameManager = FindObjectOfType<GameManager>();
        this.cursorSprite = GetComponentInChildren<SpriteRenderer>();
        this.cursorState = 0;
    }
    void Update()
    {
        this.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (gameManager.gameState == GameManager.GameMode.editing || gameManager.gameState == GameManager.GameMode.paused)
        {
            Screen.showCursor = true;
            this.ChangeCursorState(CursorState.HIDDEN);
        }
        else if (gameManager.gameState == GameManager.GameMode.playing)
        {
            Screen.showCursor = false;
            if (gameManager.MouseIsInRotatableArea())
            {
                this.ChangeCursorState(CursorState.ROTATABLE);
            }
            else if (gameManager.MouseIsInNoRoZone())
            {
                this.ChangeCursorState(CursorState.NO);
            }
            else
            {
                this.ChangeCursorState(CursorState.NEUTRAL);
            }
        }
        else
        {
            this.ChangeCursorState(CursorState.HIDDEN);
        }
    }

    void ChangeCursorState(CursorState newCursorState)
    {
        this.cursorState = newCursorState;
        switch (this.cursorState)
        {
            case CursorState.NEUTRAL: cursorSprite.sprite = cursorNeutral; break;
            case CursorState.ROTATABLE: cursorSprite.sprite = cursorRotatable; break;
            case CursorState.NO: cursorSprite.sprite = cursorNo; break;
            case CursorState.HIDDEN: cursorSprite.sprite = null; break;
        }
    }

    void OnDestroy()
    {
        Screen.showCursor = true;
    }
}
