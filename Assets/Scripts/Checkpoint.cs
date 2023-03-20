using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public LayerMask groundLayer;
	public LayerMask playerLayer;

	public bool isFirst { get => this == CheckpointController.first; }
	public bool isLast { get => this == CheckpointController.last; }

	private List<Collider> _inside = new();

	private (bool onGround, Vector3 v) GetPointUnderneath()
	{
		RaycastHit hit;
		Vector3 origin = pointCollider.center + transform.position;

		float[] surfaces = (from s in Physics.RaycastAll(new Vector3(origin.x, origin.y + 20, origin.z), Vector3.down, 20, groundLayer)
						   where _inside.Contains(s.collider) && s.point.y > origin.y
						   orderby s.point.y descending
						   select s.point.y).ToArray();

		// Check if inside of an object; we want to warp to the surface
		if (surfaces.Length > 0) {
			return (true, new(transform.position.x, surfaces[0], transform.position.z));
		}

		if (!Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, groundLayer))
			return (false, new(transform.position.x, 0, transform.position.z));

		return (true, hit.point);
	}

	public (bool onGround, float height) GetHeight()
	{
		(bool onGround, Vector2 v) = GetPointUnderneath();

		if(!onGround) return (false, v.y);

		return (true, Mathf.Max(0, transform.position.y - v.y));
	}

	public bool SetHeight(float h)
	{
		(bool onGround, Vector2 v) = GetPointUnderneath();

		Vector3 p = transform.position;

		if (!onGround) {
			p.y = h;
			transform.position = p;
			return false;
		}

		p.y = v.y + Mathf.Max(0, h);
		transform.position = p;
		return true;
	}

	public void Set2DPos(Vector3 p)
	{
		transform.position = new(p.x, transform.position.y, p.z);
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

		if (((1 << other.gameObject.layer) & playerLayer) == 0)
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

	private void OnCollisionEnter(Collision collision)
	{
		_inside.Add(collision.collider);
	}

	private void OnCollisionExit(Collision collision)
	{
		_inside.Remove(collision.collider);
	}
}
