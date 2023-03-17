using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorController : MonoBehaviour
{
	[Header("Editor objects")]
	public Transform pivot;
	public bool freecamMode = true;
	public Camera freeCam;
	public GameObject freeCamObj;
	public GameObject playerCam;
	public GameObject player;
	public PlayerMovement playerMovement;
	public GameObject UI;
	public Transform spawnpoint;

	[Header("Keybinds")]
	public KeyCode modiferKey = KeyCode.LeftControl;
	public KeyCode deleteKey = KeyCode.Delete;
	public KeyCode toggleKey;
	public KeyCode addKey;
	public KeyCode respawnKey;

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

	[Header("Freecam Movement Boundaries")]
	public Vector3 boundsOffset;
	public Vector3 minBounds;
	public Vector3 maxBounds;

	[Header("Freecam Angle")]
	public Vector2 rot = Vector2.zero;
	public float distance;

	private Vector2 lastMousePos;

	[Header("Freecam Editing")]
	public float heightIncrement = 0.25f;
	public LayerMask checkpointLayer;

	private Checkpoint _holding = null;
	private Vector3 _pickupOffset;
	private Plane _dragPlane;
	private float _height;

	[Header("Player Editing")]
	public Transform checkpointOrigin;

	private void Start()
	{
		lastMousePos = Input.mousePosition;
	}

	private void OnEnable()
	{
		CheckpointController.SetVisibilityAll(true);
		CheckpointController.SetTriggerAll(false);

		UI.SetActive(true);
		SetFreeCam(freecamMode);
	}

	private void OnDisable()
	{
		UI.SetActive(false);
		freeCamObj.SetActive(false);

		_holding = null;
	}

	private float GetSpeed()
	{
		return Mathf.Lerp(minMoveSpeed, maxMoveSpeed, Mathf.InverseLerp(minD, maxD, distance));
	}

	void SetFreeCam(bool enable = true)
	{
		freecamMode = enable;

		freeCamObj.SetActive(enable);
		playerCam.SetActive(!enable);
		playerMovement.enabled = !enable;
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
		if (Input.GetMouseButton(1)) {
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
		if (_holding != null)
			return;

		distance *= Mathf.Pow(2, -Input.mouseScrollDelta.y * zoomStrength);
		distance = Mathf.Clamp(distance, minD, maxD);
		freeCam.transform.localPosition = new Vector3(0, 0, -distance);
	}

	void TryEdit()
	{
		Ray ray = freeCam.ScreenPointToRay(Input.mousePosition);
		if (Input.GetKeyDown(deleteKey)) {
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, checkpointLayer)) {
				Destroy(hit.transform.gameObject);
			}
		}
		if (Input.GetMouseButtonDown(0)) {
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) {
				if ((1 << hit.transform.gameObject.layer & checkpointLayer) != 0) {
					_holding = hit.transform.gameObject.GetComponent<Checkpoint>();
				} else if(Input.GetKey(modiferKey)) {
					_holding = CheckpointController.PrependCheckpoint(hit.point);
				} else {
					_holding = CheckpointController.AppendCheckpoint(hit.point);
				}

				_dragPlane = new Plane(Vector3.up, hit.point);
				_pickupOffset = hit.point - _holding.transform.position;
				_height = _holding.GetHeight().height;
			}
		} else if (Input.GetMouseButtonUp(0)) {
			_holding = null;
		} else if (_holding) {
			if (_dragPlane.Raycast(ray, out float d)) {
				Vector3 click = ray.GetPoint(d);
				_holding.Set2DPos(click - _pickupOffset);
			}

			_height = _height + Input.mouseScrollDelta.y * heightIncrement;
			if (_holding.SetHeight(_height))
				_height = Mathf.Max(0, _height);
		}
	}

	void FreecamUpdate()
	{
		CamRotation();
		CamZoom();
		CamMovement();

		TryEdit();

		if(Input.GetKeyDown(toggleKey)) {
			SetFreeCam(false);
		}
	}

	void PlayercamUpdate()
	{
		if(Input.GetKeyDown(addKey)) {
			if(Input.GetKey(modiferKey)) {
				CheckpointController.PrependCheckpoint(checkpointOrigin.position);
			} else {
				CheckpointController.AppendCheckpoint(checkpointOrigin.position);
			}
		}
		if(Input.GetKeyDown(respawnKey)) {
			if (Input.GetKey(modiferKey)) {
				spawnpoint.position = player.transform.position;
				spawnpoint.rotation = playerMovement.orientation.transform.rotation;
			} else {
				player.transform.position = spawnpoint.transform.position;
				player.transform.rotation = spawnpoint.transform.rotation;
			}
		}
		if (Input.GetKeyDown(toggleKey)) {
			SetFreeCam(true);
		}
	}

	void Update()
	{
		if (freecamMode) {
			FreecamUpdate();
		} else {
			PlayercamUpdate();
		}

		lastMousePos = Input.mousePosition;
	}
}
