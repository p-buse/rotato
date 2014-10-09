using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	GameManager gameManager;
	Vector3 originalPosition;
    public bool zoomEnabled = false;
	public float closeZoom = 2f;
	public float farZoom = 7f;
	public float zoomSpeed = 1f;

	void Awake()
	{
		this.gameManager = FindObjectOfType<GameManager>();
		this.originalPosition = transform.position;
	}

    void Update()
    {
        if (zoomEnabled)
        {
            if (gameManager.gameFrozen)
            {
                Int2 rotationCenter = gameManager.currentRotationCenter;
                Vector3 newLocation = new Vector3(rotationCenter.x, rotationCenter.y, -10f);
                transform.position = Vector3.Lerp(transform.position, newLocation, zoomSpeed * Time.deltaTime);
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, closeZoom, zoomSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, originalPosition, zoomSpeed * Time.deltaTime);
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, farZoom, zoomSpeed * Time.deltaTime);
            }
        }
    }
}
