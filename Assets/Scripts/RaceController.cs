using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;
	public KeyCode respawnKey;
	static float time = 0;
	static bool racing = false;

	public static GameObject player {
		get {
			_player = _player ?? GameObject.FindGameObjectWithTag("Player");
			return _player;
		}
	}
	public static GameObject spawnpoint {
		get {
			_spawnpoint = _spawnpoint ?? GameObject.FindGameObjectWithTag("Spawnpoint");
			return _spawnpoint;
		}
	}

	private static GameObject _player = null;
	private static GameObject _spawnpoint = null;

	public static void Restart()
	{
		time = -1;
		racing = false;

		if (CheckpointController.count == 0)
			return;

		foreach(var c in CheckpointController.checkpoints) {
			c.Disable();
		}

		CheckpointController.first.Enable();
	}

	public static void StartTime()
	{
		if(CheckpointController.count == 0) return;
		time = 0;
		racing = true;
	}

	public static void EndTime()
	{
		if(CheckpointController.count == 0) return;
		CheckpointController.first.Enable();
		racing = false;
	}

	public static void RespawnPlayer()
	{
		player.transform.position = spawnpoint.transform.position;
		player.transform.rotation = spawnpoint.transform.rotation;
		Restart();
	}

	private void OnEnable()
	{
		CheckpointController.SetTriggerAll(true);
		Restart();
	}

	void Update()
	{
		if(Input.GetKeyDown(respawnKey)) {
			RespawnPlayer();
		}

		if (time == -1) {
			time = 0;
			text.text = "Time: 0.000s";
		}

		if(!racing) return;

		time += Time.deltaTime;
		text.text = "Time: " + (time >= 60 ? Mathf.FloorToInt(time / 60).ToString() + "m " : "") + $"{time % 60:0.000}s";
	}
}
