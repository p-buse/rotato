using UnityEngine;
using System.Collections;

public class ExitGame : MonoBehaviour
{
    public float zoomSpeed = 0.4f;
    public float zoomFactor = 1.2f;
    float originalZoom;

    void Start()
    {
        this.originalZoom = transform.localScale.x;
    }

    void OnMouseUpAsButton()
    {
        Application.Quit();
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
            transform.localScale = Vector3.Lerp(transform.lossyScale, new Vector3(originalZoom, originalZoom, originalZoom), zoomSpeed);
        }
    }
}
