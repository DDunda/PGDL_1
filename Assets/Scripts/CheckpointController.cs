using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
	public GameObject checkpointPrefab;
	public Transform checkpointGroup;
	public int playLayer;
	public int editLayer;
	public static int _playLayer;
	public static int _editLayer;
	public static GameObject _checkpointPrefab;
	public static Transform _checkpointGroup;

	public static Checkpoint first {
		get {
			if (count == 0)
				return null;
			return _checkpoints[0];
		}
	}
	public static Checkpoint last {
		get {
			if (count == 0)
				return null;
			return _checkpoints[count - 1];
		}
	}
	public static int count { get => _checkpoints.Count; }

	private static List<Checkpoint> _checkpoints = new List<Checkpoint>();

	public static Checkpoint[] checkpoints {
		get => _checkpoints.ToArray();
	}

	private void Start()
	{
		_checkpointPrefab = checkpointPrefab;
		_checkpointGroup = checkpointGroup;
		_playLayer = playLayer;
		_editLayer = editLayer;
	}

	public static void SetVisibilityAll(bool visibility)
	{
		foreach (var c in checkpoints) {
			if (visibility) {
				c.GetComponent<Checkpoint>().Enable();
			} else {
				c.GetComponent<Checkpoint>().Disable();
			}
		}
	}

	public static void SetTriggerAll(bool t)
	{
		int l = t ? _playLayer : _editLayer;
		foreach (var c in checkpoints) {
			c.pointCollider.isTrigger = t;
			c.gameObject.layer = l;
		}
	}

	public static void LinkCheckpoints()
	{
		for (int i = 0; i < count - 1; i++) {
			checkpoints[i].next = checkpoints[i + 1];
		}
	}

	public static void CleanOrder()
	{
		for (int i = 0; i < count; i++) {
			checkpoints[i].index = i;
		}
	}

	public static void CleanCheckpoints()
	{
		CleanOrder();
		LinkCheckpoints();
	}

	public static void AppendCheckpoint(Checkpoint c)
	{
		if (count == 0) {
			_checkpoints.Add(c);
			c.index = 0;
			c.next = null;
			c.UpdateIcon();
			return;
		}

		last.next = c;
		c.index = count;
		_checkpoints.Add(c);

		last.UpdateIcon();
		c.UpdateIcon();
	}

	public static Checkpoint AppendCheckpoint(Vector3 v)
	{
		GameObject cgo = Instantiate(_checkpointPrefab, _checkpointGroup);
		cgo.transform.position = v;
		Checkpoint c = cgo.GetComponent<Checkpoint>();

		if (count == 0) {
			_checkpoints.Add(c);
			c.index = 0;
			c.next = null;
			c.UpdateIcon();
			return c;
		}

		Checkpoint pLast = last;
		_checkpoints.Add(c);

		pLast.next = c;
		last.index = count - 1;

		pLast.UpdateIcon();
		last.UpdateIcon();

		return c;
	}

	public static void PrependCheckpoint(Checkpoint c)
	{
		if (count == 0) {
			_checkpoints.Add(c);
			c.index = 0;
			c.next = null;
			c.UpdateIcon();
			return;
		}

		c.next = checkpoints[1];
		_checkpoints.Insert(0, c);

		for (int i = 0; i < count; i++) {
			checkpoints[i].index = i;
			checkpoints[i].UpdateIcon();
		}
	}

	public static Checkpoint PrependCheckpoint(Vector3 v)
	{
		GameObject cgo = Instantiate(_checkpointPrefab, _checkpointGroup);
		cgo.transform.position = v;
		Checkpoint c = cgo.GetComponent<Checkpoint>();

		if (count == 0) {
			_checkpoints.Add(c);
			c.index = 0;
			c.next = null;
			c.UpdateIcon();
			return c;
		}

		c.next = first;
		_checkpoints.Insert(0, c);

		for (int i = 0; i < count; i++) {
			checkpoints[i].index = i;
			checkpoints[i].UpdateIcon();
		}

		return c;
	}

	public static void RemoveCheckpoint(Checkpoint c)
	{
		int i = _checkpoints.FindIndex(x => x == c);

		if (i == -1)
			return;

		_checkpoints.RemoveAt(i);

		if (i > 0)
			_checkpoints[--i].next = c.next;

		for (; i < count; i++) {
			checkpoints[i].index = i;
			checkpoints[i].UpdateIcon();
		}
	}
}
