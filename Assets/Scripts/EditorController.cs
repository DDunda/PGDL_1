using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorController : MonoBehaviour {
	[Header("Camera Transforms")]
	public Transform pivot;
	public Camera cam;

	[Header("Movement Options")]
	public float mouseSensitivity = 4;
	public float zoomStrength;

	[Space]
	[Range(-90, 90)]
	public float minPitch;
	[Range(-90, 90)]
	public float maxPitch;

	[Space]
	[Min(0.1f)]
	public float minD;
	[Min(0.1f)]
	public float maxD;

	[Space]
	public float minMoveSpeed = 3;
	public float maxMoveSpeed = 10;

	[Header("Movement Boundaries")]
	public Vector3 boundsOffset;
	public Vector3 minBounds;
	public Vector3 maxBounds;

	[Header("Camera Angle")]
	public Vector2 rot = Vector2.zero;
	public float distance;

	private Vector2 lastMousePos;

	[Header("Editing")]
	public float heightScrollSensitivity;

	private Checkpoint holding = null;

	private void Start()
	{
		lastMousePos = Input.mousePosition;
	}

	private float GetSpeed()
	{
		return Mathf.Lerp(minMoveSpeed, maxMoveSpeed, Mathf.InverseLerp(minD, maxD, distance));
	}

	void CamMovement()
	{
		Vector3 moveAxis = Vector3.zero;

		moveAxis.x += Input.GetAxisRaw("Horizontal");
		moveAxis.z += Input.GetAxisRaw("Vertical");
		moveAxis.y += Input.GetAxisRaw("Fly");

		// Rotate so that it faces camera direction on 2D plane
		moveAxis = Quaternion.Euler(0, rot.x, 0) * moveAxis;

		Vector3 mB = minBounds + boundsOffset;
		Vector3 MB = maxBounds + boundsOffset;

		// Check if moving into wall
		moveAxis = (Vector3.Min(Vector3.Max(pivot.position + moveAxis * 0.1f, mB), MB) - pivot.position) * 10f;

		if (moveAxis.magnitude == 0)
			return;

		pivot.position += moveAxis.normalized * GetSpeed() * Time.deltaTime;
		pivot.position = Vector3.Min(Vector3.Max(pivot.position, mB), MB);
	}

	void CamRotation()
	{
		if (Input.GetMouseButton(1))
		{
			Vector2 mouseDelta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - lastMousePos;
			mouseDelta.y *= -1;
			rot += mouseDelta * mouseSensitivity;
		}
		rot.x = Mathf.Repeat(rot.x, 360);
		rot.y = Mathf.Clamp(rot.y, minPitch, maxPitch);
		pivot.transform.eulerAngles = new Vector3(rot.y, rot.x);
	}

	void CamZoom()
	{
		if (holding != null)
			return;

		distance *= Mathf.Pow(2, -Input.mouseScrollDelta.y * zoomStrength);
		distance = Mathf.Clamp(distance, minD, maxD);
		cam.transform.localPosition = new Vector3(0, 0, -distance);
	}

	void TryEdit()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
		}
	}

	void Update()
    {
		CamRotation();
		CamZoom();
		CamMovement();

		lastMousePos = Input.mousePosition;
	}
}
