using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
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
		public bool  canSprint;
		public bool  canJump;
		public bool  slopeSliding;
		public bool  airMovement;
		 
		public float walkSpeed;
		public float sprintSpeed;
		public float slopeSpeed;
		 
		public float jumpForce;
		public float airMultiplier;
		public int airJumpsMax;

		public float gravity;
		 
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
	public string _levelName;
	public static Levels levels { 
		get {
			if (_levels == null) LoadFile();
			return _levels;
		}
	}
	public static LevelRoutes currentLevel { get => _levels[levelName]; }
	private static Levels _levels = null;
	public static string levelName;

	public static Modifiers GetModifiers()
	{
		var fpc = Controller.fpController;

		return new()
		{
			canSprint = fpc.canSprint,
			canJump = fpc.canJump,
			slopeSliding = fpc.willSlideOnSlopes,
			airMovement = fpc.canSprint,

			walkSpeed = fpc.walkSpeed,
			sprintSpeed = fpc.sprintSpeed,
			slopeSpeed = fpc.slopeSpeed,

			jumpForce = fpc.jumpForce,
			airJumpsMax = fpc.airJumpsMax,
			airMultiplier = fpc.airMultiplier,
			gravity = fpc.gravity,

			maxSlopeAngle = Controller.charController.slopeLimit
		};
	}

	public static Hash128 GetHash()
	{
		Hash128 hash = CheckpointController.checkpoints.Select(c => c.transform.position).Aggregate(new Hash128(), (a, c) => {
			a.Append(c.x);
			a.Append(c.y);
			a.Append(c.z);
			return a;
		});

		Modifiers m = GetModifiers();

		int bools = (m.canSprint ? 1 : 0) | (m.canJump ? 2 : 0) | (m.slopeSliding ? 4 : 0) | (m.airMovement ? 8 : 0);

		hash.Append(bools);
		hash.Append(m.walkSpeed);
		hash.Append(m.sprintSpeed);
		hash.Append(m.slopeSpeed);
		hash.Append(m.jumpForce);
		hash.Append(m.airJumpsMax);
		hash.Append(m.airMultiplier);
		hash.Append(m.gravity);
		hash.Append(m.maxSlopeAngle);

		return hash;
	}

	public static bool RouteExists(out Hash128 hash)
	{
		hash = GetHash();

		return currentLevel.Contains(hash);
	}

	public static RouteData GetRouteData()
	{
		return new()
		{
			hash = GetHash(),
			checkpoints = (from s in CheckpointController.checkpoints select s.transform.position).ToArray(),
			spawnpointPos = Controller.spawnpoint.position,
			spawnpointAngle = Controller.ToEuler(Controller.spawnpoint.rotation),
			bestTimes = RaceController.times.ToArray(),
			modifiers = GetModifiers()
		};
	}

	public static void SetRoute(Hash128 hash)
	{
		foreach(var c in CheckpointController.checkpoints) {
			Destroy(c.gameObject);
		}

		RaceController.times.Clear();

		if (!currentLevel.Contains(hash))
			return;

		RouteData r = currentLevel[hash];

		Controller.spawnpoint.position = r.spawnpointPos;
		Controller.spawnpoint.rotation = Quaternion.Euler(r.spawnpointAngle);

		RaceController.times.UnionWith(r.bestTimes);

		Controller.fpController.canSprint         = r.modifiers.canSprint;
		Controller.fpController.canJump           = r.modifiers.canJump;
		Controller.fpController.willSlideOnSlopes = r.modifiers.slopeSliding;
		Controller.fpController.canMoveInAir      = r.modifiers.airMovement;
		Controller.fpController.walkSpeed	      = r.modifiers.walkSpeed;
		Controller.fpController.sprintSpeed	      = r.modifiers.sprintSpeed;
		Controller.fpController.slopeSpeed	      = r.modifiers.slopeSpeed;
		Controller.fpController.jumpForce	      = r.modifiers.jumpForce;
		Controller.fpController.airJumpsMax	      = r.modifiers.airJumpsMax;
		Controller.fpController.airMultiplier     = r.modifiers.airMultiplier;
		Controller.fpController.gravity		      = r.modifiers.gravity;

		Controller.charController.slopeLimit      = r.modifiers.maxSlopeAngle;

		foreach (var c in r.checkpoints) {
			CheckpointController.AppendCheckpoint(c);
		}
	}

	public static void SaveRoute()
	{
		Hash128 hash = GetHash();

		if (!currentLevel.Contains(hash)) {
			currentLevel[hash] = GetRouteData();
		} else {
			currentLevel[hash].bestTimes = RaceController.times.ToArray();
		}

		levels.lastLevel = levelName;
		levels.lastRoute = hash;
	}

	public static void SaveFile()
	{
		File.WriteAllText($"{Application.persistentDataPath}/{fileName}", JsonUtility.ToJson(levels));
	}

	public static void LoadFile()
	{
		if (!File.Exists($"{Application.persistentDataPath}/{fileName}")) {
			_levels = new();
			return;
		}
		_levels = JsonUtility.FromJson<Levels>(File.ReadAllText($"{Application.persistentDataPath}/{fileName}"));
	}

	private void Start()
	{
		levelName = _levelName;
		LoadFile();
		if(!levels.Contains(levelName)) {
			levels[levelName] = new() { levelName = levelName };
			SaveFile();
		}
	}
}
