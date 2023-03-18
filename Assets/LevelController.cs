using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.Json;
using System.IO;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	public interface IHasKey<T>
	{
		T key { get; }
	}

	[Serializable]
	public abstract class SerializableDictionary<TKey, TVal> where TVal : IHasKey<TKey> {
		public abstract List<TVal> list {
			get;
			set;
		}

		public TVal this[TKey key] {
			get {
				int i = list.FindIndex(x => x.key.Equals(key));

				return i == -1 ? default(TVal) : list[i];
			}
			set {
				int i = list.FindIndex(x => x.key.Equals(key));

				if (i == -1) {
					Add(value);
				} else {
					list[i] = value;
				}
			}
		}
		public TKey[] Keys => (from x in list select x.key).ToArray();
		public bool Contains(TKey key) => Keys.Contains(key);
		public void Add(TVal val) => list.Add(val);
	}

	[Serializable]
	public struct Modifiers
	{
		public float walkSpeed;
		public float sprintSpeed;
		public float groundDrag;

		public float jumpForce;
		public float jumpCooldown;
		public float airMultiplier;
		public int airJumpsMax;

		public float maxSlopeAngle;

	}

	[Serializable]
	public class RouteData : IHasKey<Hash128>
	{
		public Hash128 hash;
		public Vector3[] checkpoints;
		public Vector3 spawnpointPos;
		public Vector2 spawnpointAngle;
		public float[] bestTimes;
		public Modifiers modifiers;

		public Hash128 key => hash;
	}

	[Serializable]
	public class LevelRoutes : SerializableDictionary<Hash128, RouteData>, IHasKey<string>
	{
		public string levelName;
		public List<RouteData> routes = new();

		public string key => levelName;

		public override List<RouteData> list {
			get => routes;
			set => routes = value;
		}
	}

	[Serializable]
	public class Levels : SerializableDictionary<string, LevelRoutes>
	{
		public List<LevelRoutes> levels = new();
		public string lastLevel;
		public Hash128 lastRoute;

		public override List<LevelRoutes> list {
			get => levels;
			set => levels = value;
		}
	}

	private static string fileName = "routes.json";
	public string levelName;
	public static Levels levels { 
		get {
			if (_levels == null) LoadAllRoutes();
			return _levels;
		}
	}
	private static Levels _levels = null;

	public RouteData GetRouteData()
	{
		RouteData d = new()
		{
			checkpoints = (from s in CheckpointController.checkpoints select s.transform.position).ToArray(),
			spawnpointPos = Controller.spawnpoint.position,
			spawnpointAngle = Controller.ToEuler(Controller.spawnpoint.rotation),
			bestTimes = RaceController.times.ToArray(),
			modifiers = new()
			{
				walkSpeed = Controller.playerMovement.walkSpeed,
				sprintSpeed = Controller.playerMovement.sprintSpeed,
				groundDrag = Controller.playerMovement.groundDrag,

				jumpForce = Controller.playerMovement.jumpForce,
				jumpCooldown = Controller.playerMovement.jumpCooldown,
				airMultiplier = Controller.playerMovement.airMultiplier,
				airJumpsMax = Controller.playerMovement.airJumpsMax,

				maxSlopeAngle = Controller.playerMovement.maxSlopeAngle
			}
		};

		Hash128 hash = d.checkpoints.Aggregate(new Hash128(), (a, c) => {
			a.Append(c.x);
			a.Append(c.y);
			a.Append(c.z);
			return a;
		});

		hash.Append(d.modifiers.walkSpeed);
		hash.Append(d.modifiers.sprintSpeed);
		hash.Append(d.modifiers.groundDrag);
		hash.Append(d.modifiers.jumpForce);
		hash.Append(d.modifiers.jumpCooldown);
		hash.Append(d.modifiers.airMultiplier);
		hash.Append(d.modifiers.airJumpsMax);
		hash.Append(d.modifiers.maxSlopeAngle);

		d.hash = hash;

		return d;
	}

	public void SetRoute(Hash128 hash)
	{
		foreach(var c in CheckpointController.checkpoints) {
			Destroy(c.gameObject);
		}

		RaceController.times.Clear();

		if (!levels.Contains(levelName))
			return;
		if (!levels[levelName].Contains(hash))
			return;

		RouteData r = levels[levelName][hash];

		Controller.spawnpoint.position = r.spawnpointPos;
		Controller.spawnpoint.rotation = Quaternion.Euler(r.spawnpointAngle);

		RaceController.times.UnionWith(r.bestTimes);

		Controller.playerMovement.walkSpeed     = r.modifiers.walkSpeed;
		Controller.playerMovement.sprintSpeed   = r.modifiers.sprintSpeed;
		Controller.playerMovement.groundDrag    = r.modifiers.groundDrag;
		Controller.playerMovement.jumpForce     = r.modifiers.jumpForce;
		Controller.playerMovement.jumpCooldown  = r.modifiers.jumpCooldown;
		Controller.playerMovement.airMultiplier = r.modifiers.airMultiplier;
		Controller.playerMovement.airJumpsMax   = r.modifiers.airJumpsMax;
		Controller.playerMovement.maxSlopeAngle = r.modifiers.maxSlopeAngle;

		foreach (var c in r.checkpoints) {
			CheckpointController.AppendCheckpoint(c);
		}
	}

	public void SaveRoute()
	{
		if(!levels.Contains(levelName)) {
			levels[levelName] = new() { levelName = levelName };
		}

		RouteData r = GetRouteData();
		levels[levelName][r.hash] = r;

		levels.lastLevel = levelName;
		levels.lastRoute = r.hash;
	}

	public static void SaveAllRoutes()
	{
		File.WriteAllText($"{Application.persistentDataPath}/{fileName}", JsonUtility.ToJson(levels));
	}

	public static void LoadAllRoutes()
	{
		if (!File.Exists($"{Application.persistentDataPath}/{fileName}")) {
			_levels = new();
			return;
		}
		_levels = JsonUtility.FromJson<Levels>(File.ReadAllText($"{Application.persistentDataPath}/{fileName}"));
	}

	private void Start()
	{
		LoadAllRoutes();
	}

	private void OnDestroy()
	{
		SaveRoute();
		SaveAllRoutes();
	}
}
