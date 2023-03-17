using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFloor : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		RaceController.RespawnPlayer();
	}
}
