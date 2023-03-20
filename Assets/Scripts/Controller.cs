using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	// Scene references;
	public GameObject _freeCamObj;
	public GameObject _editorUI;
	public GameObject _playUI;
	public LevelController _level;

	private static Transform _spawnpoint;
	private static GameObject _player;
	private static Transform _playerCheckpointOrigin;
	private static GameObject _playerCamObj;
	private static PlayerMovement _playerMovement;
	private static PlayerCam _playerCamScript;
	private static Camera _freeCam;
	private static TMPro.TextMeshProUGUI _timer;

	// Using these find commands means we have to link less objects in the editor
	public static GameObject freeCamObj;
	public static GameObject editorUI;
	public static GameObject playUI;
	public static LevelController level;
	public static Transform editorCamPivot => freeCamObj.transform.parent;
	public static Transform spawnpoint => _spawnpoint = _spawnpoint ?? GameObject.FindGameObjectWithTag("Spawnpoint").transform;
	public static GameObject player => _player = _player            ?? GameObject.FindGameObjectWithTag("Player");
	public static Transform playerCheckpointOrigin => _playerCheckpointOrigin = _playerCheckpointOrigin ?? player.transform.Find("CheckpointPosition");
	public static GameObject playerCamObj => _playerCamObj = _playerCamObj                              ?? player.transform.Find("PlayerCam").gameObject;
	public static PlayerMovement playerMovement => _playerMovement = _playerMovement     ?? player.GetComponent<PlayerMovement>();
	public static PlayerCam playerCamScript => _playerCamScript = _playerCamScript ?? playerCamObj.GetComponent<PlayerCam>();
	public static Camera freeCam => _freeCam = _freeCam                              ?? freeCamObj.GetComponent<Camera>();
	public static TMPro.TextMeshProUGUI timer => _timer = _timer ?? playUI.transform.Find("Timer").GetComponent<TMPro.TextMeshProUGUI>();

	public static Vector2 ToEuler(Quaternion @this)
	{
		Vector2 v;
		float x = @this.eulerAngles.x;
		v.x = x < 180f ? x : x - 360f;
		v.y = @this.eulerAngles.y;
		return v;
	}

	public void Start()
	{
		freeCamObj = _freeCamObj;
		editorUI = _editorUI;
		playUI = _playUI;
		level = _level;
	}
}
