using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawRectangle : MonoBehaviour {

    [SerializeField]
    Vector2 topLeft;
    [SerializeField]
    Vector2 bottomRight;

        public LineRenderer _topLine;
        public LineRenderer _bottomLine;
        public LineRenderer _leftLine;
        public LineRenderer _rightLight;

    void SetTopLeft(Vector2 newTopLeft)
    {
        this.topLeft = newTopLeft;
        RecreateLines();
    }


    void SetButtomRight(Vector2 newBottomRight)
    {
        this.bottomRight = newBottomRight;
        RecreateLines();
    }

    private void RecreateLines()
    {
        DestroyLines();
        

    }

    private void DestroyLines()
    {
        
    }

}
