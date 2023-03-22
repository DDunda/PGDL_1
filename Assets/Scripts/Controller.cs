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
	private static FirstPersonController _fpController;
	private static CharacterController _charController;
	private static Camera _freeCam;
	private static TMPro.TextMeshProUGUI _timer;
	private static GameObject _modifierUI;
	private static ModifierController _modifierController;
	private static EditorController _editorController;
	private static RaceController _playController;

	// Using these find commands means we have to link less objects in the editor
	public static GameObject freeCamObj;
	public static GameObject editorUI;
	public static GameObject playUI;
	public static LevelController level;
	public static Transform editorCamPivot => freeCamObj.transform.parent;
	public static Transform spawnpoint => _spawnpoint;
	public static GameObject player => _player;
	public static Transform playerCheckpointOrigin => _playerCheckpointOrigin;
	public static GameObject playerCamObj => _playerCamObj;
	public static FirstPersonController fpController => _fpController;
	public static CharacterController charController => _charController;
	public static Camera freeCam => _freeCam;
	public static TMPro.TextMeshProUGUI timer => _timer;
	public static GameObject modifierUI => _modifierUI;
	public static ModifierController modifierController => _modifierController;
	public static EditorController editorController => _editorController;
	public static RaceController playController => _playController;

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

		_modifierController = gameObject.GetComponent<ModifierController>();
		_editorController = gameObject.GetComponent<EditorController>();
		_playController = gameObject.GetComponent<RaceController>();

		_spawnpoint = GameObject.FindGameObjectWithTag("Spawnpoint").transform;
		_player = GameObject.FindGameObjectWithTag("Player");
		_playerCheckpointOrigin = player.transform.Find("CheckpointPosition");
		_playerCamObj = player.transform.Find("PlayerCam").gameObject;
		_fpController = player.GetComponent<FirstPersonController>();
		_charController = player.GetComponent<CharacterController>();
		_freeCam = freeCamObj.GetComponent<Camera>();
		_timer = playUI.transform.Find("Timer").GetComponent<TMPro.TextMeshProUGUI>();
		_modifierUI = editorUI.transform.Find("ModifierUI").gameObject;
	}
}
