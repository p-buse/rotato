using UnityEngine;
using System.Collections;

public class ClickableButton : MonoBehaviour {
    public int levelToLoad;
    float zoomSpeed = 0.4f;
    float zoomFactor = 1.2f;

    void OnMouseUpAsButton()
    {
        Application.LoadLevel(levelToLoad);
    }

    void Update()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (collider2D.bounds.Contains(mouse))
        {
            transform.localScale = Vector3.Lerp(transform.lossyScale, new Vector3(zoomFactor, zoomFactor, zoomFactor), zoomSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.lossyScale, new Vector3(1f, 1f, 1f), zoomSpeed);
        }
    }
}
