using UnityEngine;
using System.Collections;

public class ClickableButton : MonoBehaviour {
    public int levelToLoad;
    public float zoomSpeed = 0.4f;
    public float zoomFactor = 1.2f;
    float originalZoom;
    public bool activateOnKeyPress = false;
    public KeyCode keyToPress = KeyCode.Escape;

    void Start()
    {
        this.originalZoom = transform.localScale.x;
    }

    void OnMouseUpAsButton()
    {
        Application.LoadLevel(levelToLoad);
    }

    void Update()
    {
        if (this.activateOnKeyPress && Input.GetKeyUp(keyToPress))
        {
            OnMouseUpAsButton();
        }
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (GetComponent<Collider2D>().bounds.Contains(mouse))
        {
            transform.localScale = Vector3.Lerp(transform.lossyScale, new Vector3(zoomFactor, zoomFactor, zoomFactor), zoomSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.lossyScale, new Vector3(originalZoom, originalZoom, originalZoom), zoomSpeed);
        }
    }
}
