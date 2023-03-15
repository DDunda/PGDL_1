using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public uint index = 0;
	public Checkpoint next = null;
	public bool active = false;

	public TMPro.TextMeshPro text;
	public GameObject startIcon;
	public GameObject endIcon;

	public GameObject sprite;
	public SphereCollider pointCollider;

	public void SetHeight(float h)
	{
		pointCollider.center = Vector3.up * h;
		sprite.transform.localPosition = Vector3.up * h;
	}

	public void Show(bool s)
	{
		if (!s)
		{
			sprite.SetActive(false);
			return;
		}

		text.text = $"{index}";
		startIcon.SetActive(false);
		endIcon.SetActive(false);
		text.gameObject.SetActive(false);

		if (index == 0)
		{
			startIcon.SetActive(true);
		} else if (next == null)
		{
			endIcon.SetActive(true);
		} else
		{
			text.gameObject.SetActive(true);
		}
		sprite.SetActive(true);
	}

	public void Disable()
	{
		active = false;
		Show(false);
	}

	public void Enable()
	{
		active = true;
		Show(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!active)
			return;

		Disable();

		if (index == 0)
		{
			RaceController.StartTime();
		}

		if (next == null)
		{
			RaceController.EndTime();
			return;
		}

		next.Enable();
	}
}
