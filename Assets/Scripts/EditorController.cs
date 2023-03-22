using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class EditorController : MonoBehaviour
{
	[Header("Keybinds")]
	public KeyCode modiferKey = KeyCode.LeftControl;
	public KeyCode deleteKey = KeyCode.Delete;
	public KeyCode toggleKey;
	public KeyCode addKey;
	public KeyCode respawnKey;

	[Header("Movement Options")]
	public bool freecamMode = true;
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
	public float posAccuracy = 16;
	public LayerMask checkpointLayer;

	private Checkpoint _holding = null;
	private Vector3 _pickupOffset;
	private Plane _dragPlane;
	private float _height;

	private void Start()
	{
		lastMousePos = Input.mousePosition;
	}

	private void OnEnable()
	{
		CheckpointController.SetVisibilityAll(true);
		CheckpointController.SetTriggerAll(false);

		Controller.editorUI.SetActive(true);
		SetFreeCam(freecamMode);
	}

	private void OnDisable()
	{
		Controller.editorUI.SetActive(false);
		Controller.freeCamObj.SetActive(false);

		_holding = null;
	}

	private float GetSpeed()
	{
		return Mathf.Lerp(minMoveSpeed, maxMoveSpeed, Mathf.InverseLerp(minD, maxD, distance));
	}

	void SetFreeCam(bool enable = true)
	{
		freecamMode = enable;

		Controller.freeCamObj.SetActive(enable);
		Controller.playerCamObj.SetActive(!enable);
		Controller.playerMovement.enabled = !enable;
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
		moveAxis = (Vector3.Min(Vector3.Max(Controller.editorCamPivot.position + moveAxis * 0.1f, mB), MB) - Controller.editorCamPivot.position) * 10f;

		if (moveAxis.magnitude == 0)
			return;

		Controller.editorCamPivot.position += moveAxis.normalized * GetSpeed() * Time.deltaTime;
		Controller.editorCamPivot.position = Vector3.Min(Vector3.Max(Controller.editorCamPivot.position, mB), MB);
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
		Controller.editorCamPivot.eulerAngles = new Vector3(rot.y, rot.x);
	}

	void CamZoom()
	{
		if (_holding != null)
			return;

		distance *= Mathf.Pow(2, -Input.mouseScrollDelta.y * zoomStrength);
		distance = Mathf.Clamp(distance, minD, maxD);
		Controller.freeCam.transform.localPosition = new Vector3(0, 0, -distance);
	}

	Vector3 Snap(Vector3 v)
	{
		v.x = Mathf.Round(v.x * posAccuracy) / posAccuracy;
		v.y = Mathf.Round(v.y * posAccuracy) / posAccuracy;
		v.z = Mathf.Round(v.z * posAccuracy) / posAccuracy;
		return v;
	}

	void TryEdit()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		Ray ray = Controller.freeCam.ScreenPointToRay(Input.mousePosition);
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
					_holding = CheckpointController.PrependCheckpoint(Snap(hit.point));
				} else {
					_holding = CheckpointController.AppendCheckpoint(Snap(hit.point));
				}

				_dragPlane = new Plane(Vector3.up, hit.point);
				_pickupOffset = hit.point - _holding.transform.position;
				_height = _holding.GetHeight().height;
			}
		} else if (Input.GetMouseButtonUp(0) || (_holding && !Input.GetMouseButton(0))) {
			_holding = null;
		} else if (_holding) {
			if (_dragPlane.Raycast(ray, out float d)) {
				Vector3 click = ray.GetPoint(d);
				_holding.Set2DPos(click - _pickupOffset);
			}

			_height = _height + Input.mouseScrollDelta.y * heightIncrement;
			if (_holding.SetHeight(_height))
				_height = Mathf.Max(0, _height);

			_holding.transform.position = Snap(_holding.transform.position);
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
			if (Input.GetKey(modiferKey)) {
				CheckpointController.PrependCheckpoint(Snap(Controller.playerCheckpointOrigin.position));
			} else {
				CheckpointController.AppendCheckpoint(Snap(Controller.playerCheckpointOrigin.position));
			}
		}
		if(Input.GetKeyDown(respawnKey)) {
			if (Input.GetKey(modiferKey)) {
				Controller.spawnpoint.position = Snap(Controller.player.transform.position);
				Controller.spawnpoint.rotation = Controller.playerCamObj.transform.rotation;
			} else {
				Controller.player.transform.position = Controller.spawnpoint.transform.position;
				Vector2 v = Controller.ToEuler(Controller.spawnpoint.rotation);
				Controller.playerCamScript.xRotation = v.x;
				Controller.playerCamScript.yRotation = v.y;
			}
		}
		if (Input.GetKeyDown(toggleKey)) {
			SetFreeCam(true);
		}
	}

	public void SaveUnplayed()
	{
		Hash128 hash;
		if (LevelController.RouteExists(out hash)) return;

		RaceController.times.Clear();

		LevelController.levels.lastLevel = LevelController.levelName;
		LevelController.levels.lastRoute = hash;
		LevelController.currentLevel[hash] = LevelController.GetRouteData();
		LevelController.SaveFile();
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
