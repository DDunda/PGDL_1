using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorController))]
class EditorControllerEditor : Editor {

	Vector3 Slider(int id, Vector3 position, Vector3 axis, float size = 0.05f, float snap = 0.5f)
	{
		Vector3 bo = ((EditorController)target).boundsOffset;
		return Handles.Slider(id, position, bo, axis, HandleUtility.GetHandleSize(position + bo) * size, Handles.DotHandleCap, snap);
	}

	protected virtual void OnSceneGUI()
	{
		EditorController cam = (EditorController)target;

		if (cam == null)
		{
			return;
		}

		Vector3 center = (cam.minBounds + cam.maxBounds) / 2;
		Vector3 size = cam.maxBounds - cam.minBounds;

		Handles.DrawWireCube(center + cam.boundsOffset, size);
		cam.maxBounds.y = Mathf.Max(cam.minBounds.y, Slider(0,new Vector3(center.x, cam.maxBounds.y, center.z), Vector3.up  ).y);
		cam.minBounds.y = Mathf.Min(cam.maxBounds.y, Slider(1,new Vector3(center.x, cam.minBounds.y, center.z), Vector3.down).y);

		cam.maxBounds.x = Mathf.Max(cam.minBounds.x, Slider(2,new Vector3(cam.maxBounds.x, center.y, center.z), Vector3.right).x);
		cam.minBounds.x = Mathf.Min(cam.maxBounds.x, Slider(3,new Vector3(cam.minBounds.x, center.y, center.z), Vector3.left ).x);

		cam.maxBounds.z = Mathf.Max(cam.minBounds.z, Slider(4,new Vector3(center.x, center.y, cam.maxBounds.z), Vector3.forward).z);
		cam.minBounds.z = Mathf.Min(cam.maxBounds.z, Slider(5,new Vector3(center.x, center.y, cam.minBounds.z), Vector3.back   ).z);

		cam.boundsOffset = Handles.PositionHandle(cam.boundsOffset, Quaternion.identity);

		//Handles.Label(cam.minBounds, "Min");
		//Handles.Label(cam.maxBounds, "Max");

		if (cam.minBounds.x > cam.maxBounds.x)
		{
			var x = cam.minBounds.x;
			cam.minBounds.x = cam.maxBounds.x;
			cam.maxBounds.x = x;
		}
		if (cam.minBounds.y > cam.maxBounds.y)
		{
			var y = cam.minBounds.y;
			cam.minBounds.y = cam.maxBounds.y;
			cam.maxBounds.y = y;
		}
		if (cam.minBounds.x > cam.maxBounds.x)
		{
			var z = cam.minBounds.z;
			cam.minBounds.z = cam.maxBounds.z;
			cam.maxBounds.z = z;
		}
	}
}