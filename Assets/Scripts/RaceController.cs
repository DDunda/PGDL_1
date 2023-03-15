using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;
	static float time = 0;
	static bool racing = false;

	public static Checkpoint first;
	public static Checkpoint last;

	public static void Restart()
	{
		foreach (var c in GameObject.FindGameObjectsWithTag("Checkpoint"))
		{
			c.GetComponent<Checkpoint>().Disable();
		}

		first.Enable();
		time = 0;
		racing = false;
	}

	public static void StartTime()
	{
		time = 0;
		racing = true;
	}

	public static void EndTime()
	{
		first.Enable();
		racing = false;
	}

	public static void AddCheckPoint(Checkpoint c)
	{
		last.next = c;
		last.endIcon.SetActive(false);
		last.text.gameObject.SetActive(true);
		c.index = last.index + 1;
		last = c;
	}

	private void Start()
	{
		foreach (var c in GameObject.FindGameObjectsWithTag("Checkpoint"))
		{
			var chk = c.GetComponent<Checkpoint>();
			if(chk.index == 0)
			{
				first = chk;
			}
			if(chk.next == null)
			{
				last = chk;
			}
		}
	}

	void Update()
	{
		if(!racing) return;

		time += Time.deltaTime;
		text.text = $"Time: {time:0.000}s";
	}
}
