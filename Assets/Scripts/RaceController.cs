using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
	public KeyCode modiferKey = KeyCode.LeftControl;
	public KeyCode respawnKey;
	static float time = 0;
	static bool racing = false;
	public static SortedSet<float> times = new();

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
		times.Add(time);

		LevelController.SaveRoute();
		LevelController.SaveFile();
	}

	public static void RespawnPlayer()
	{
		Controller.charController.enabled = false;
		Controller.player.transform.position = Controller.spawnpoint.transform.position;
		Controller.charController.enabled = true;
		Vector2 v = Controller.ToEuler(Controller.spawnpoint.rotation);
		Restart();
	}

	private void OnEnable()
	{
		CheckpointController.SetTriggerAll(true);
		Restart();

		times.Clear();

		if (LevelController.RouteExists(out Hash128 hash)) {
			times.UnionWith(LevelController.currentLevel[hash].bestTimes);
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(respawnKey)) {
			if (Input.GetKey(modiferKey)) {
				Controller.spawnpoint.position = Controller.player.transform.position;
				Controller.spawnpoint.rotation = Controller.playerCamObj.transform.rotation;
			} else {
				RespawnPlayer();
			}
		}

		if (time == -1) {
			time = 0;
			Controller.timer.text = "Time: 0.000s";
		}

		if(!racing) return;

		time += Time.deltaTime;
		Controller.timer.text = "Time: " + (time >= 60 ? Mathf.FloorToInt(time / 60).ToString() + "m " : "") + $"{time % 60:0.000}s";
	}
}
