using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public int index = 0;
	[HideInInspector]
	public Checkpoint next = null;
	public bool active = false;

	public TMPro.TextMeshPro text;
	public GameObject startIcon;
	public GameObject endIcon;

	public GameObject sprite;
	public SphereCollider pointCollider;

	public bool isFirst { get => this == CheckpointController.first; }
	public bool isLast { get => this == CheckpointController.last; }

	public void SetHeight(float h)
	{
		pointCollider.center = Vector3.up * h;
		sprite.transform.localPosition = Vector3.up * h;
	}

	public void UpdateIcon()
	{
		text.text = $"{index}";

		startIcon.SetActive(false);
		endIcon.SetActive(false);
		text.gameObject.SetActive(false);

		if(CheckpointController.count == 1)
		{
			return; // Empty icon when both start and end
		} else if (isFirst)
		{
			startIcon.SetActive(true);
		} else if (isLast)
		{
			endIcon.SetActive(true);
		} else
		{
			text.gameObject.SetActive(true);
		}
	}

	public void Show(bool s)
	{
		if (s) UpdateIcon();

		sprite.SetActive(s);
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

		if (isLast)
		{
			RaceController.EndTime();
			return;
		}

		if (isFirst)
		{
			RaceController.StartTime();
		}

		next.Enable();
	}

	private void OnDestroy()
	{
		CheckpointController.RemoveCheckpoint(this);
	}
}
