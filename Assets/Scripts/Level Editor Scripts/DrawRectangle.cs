using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawRectangle : MonoBehaviour {

    [SerializeField]
    Vector2 topLeft;
    [SerializeField]
    Vector2 bottomRight;
    
    public bool recreateLines = false;
    public Material lineMaterial;

    GameObject _topLine;
    GameObject _bottomLine;
    GameObject _leftLine;
    GameObject _rightLine;

    void Awake()
    {
        RecreateLines();
    }

    void Update()
    {
        if (recreateLines == true)
        {
            RecreateLines();
            recreateLines = false;
        }
    }

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
        LineRenderer topLine = GenerateLine(ref _topLine);
        topLine.SetPosition(0, new Vector2(topLeft.x, topLeft.y));
        topLine.SetPosition(1, new Vector2(bottomRight.x, topLeft.y));
        LineRenderer bottomLine = GenerateLine(ref _bottomLine);
        bottomLine.SetPosition(0, new Vector2(topLeft.x, bottomRight.y));
        bottomLine.SetPosition(1, new Vector2(bottomRight.x, bottomRight.y));
        LineRenderer leftLine = GenerateLine(ref _leftLine);
        leftLine.SetPosition(0, new Vector2(topLeft.x, topLeft.y));
        leftLine.SetPosition(1, new Vector2(topLeft.x, bottomRight.y));
        LineRenderer rightLine = GenerateLine(ref _rightLine);
        rightLine.SetPosition(0, new Vector2(bottomRight.x, topLeft.y));
        rightLine.SetPosition(1, new Vector2(bottomRight.x, bottomRight.y));
        // use world space
        // set first point
        // set second point
    }

    private LineRenderer GenerateLine(ref GameObject lineObject)
    {
        lineObject = new GameObject("rect_line", typeof(LineRenderer));
        lineObject.transform.parent = this.gameObject.transform;
        LineRenderer lineComponent = lineObject.GetComponent<LineRenderer>();
        lineComponent.useWorldSpace = true;
        lineComponent.SetWidth(0.1f, 0.1f);
        lineComponent.SetVertexCount(2);
        lineComponent.material = lineMaterial;
        return lineComponent;
    }

    private void DestroyLines()
    {
        if (_topLine != null)
            Destroy(_topLine);
        if (_bottomLine != null)
            Destroy(_bottomLine);
        if (_leftLine != null)
            Destroy(_leftLine);
        if (_rightLine != null)
            Destroy(_rightLine);
    }

}
