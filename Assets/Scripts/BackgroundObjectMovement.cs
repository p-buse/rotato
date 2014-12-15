using UnityEngine;
using System.Collections;

public class BackgroundObjectMovement : MonoBehaviour {
	private Camera camera;
	public float moveRatioX;
	public float moveRatioY;
	private Vector3 lastCameraPos;
	public Vector3 baseMovement;
	public float baseRotation;

	void Awake()
	{
		this.camera = FindObjectOfType<Camera>();
		lastCameraPos = camera.transform.position;
	}


	void Update () {

		Vector3 CameraMoved = camera.transform.position-lastCameraPos;

		//right now this is settable for different ratios in x and y directions.  is that necessary?
		Vector3 myMove = new Vector3 (-moveRatioX*CameraMoved.x, -moveRatioY*CameraMoved.y, 0);

		transform.Translate (myMove);
		lastCameraPos = camera.transform.position;

		if (baseMovement != Vector3.zero)
			transform.Translate (baseMovement * Time.deltaTime);

		transform.eulerAngles += new Vector3 (0, 0, baseRotation * Time.deltaTime*180f);
	}
}
