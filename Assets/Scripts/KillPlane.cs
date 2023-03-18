using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		RaceController.RespawnPlayer();
	}
}
