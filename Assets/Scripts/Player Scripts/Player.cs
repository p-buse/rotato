using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public KeyCode resetButton = KeyCode.R;
    Plane[] cameraView;
    GameManager gameManager;

    void Start()
    {
        cameraView = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        this.gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(resetButton) || !GeometryUtility.TestPlanesAABB(cameraView, this.collider2D.bounds))
        {
            gameManager.ResetLevel();
        }
    }
    public Int2 GetRoundedPosition()
    {
        return new Int2(transform.position.x, transform.position.y);
    }
}
