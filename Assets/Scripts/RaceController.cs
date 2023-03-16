using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;
	static float time = 0;
	static bool racing = false;

	public static void Restart()
	{
		time = 0;
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

	void Update()
	{
		if(!racing) return;

		time += Time.deltaTime;
		text.text = $"Time: {time:0.000}s";
	}
}
