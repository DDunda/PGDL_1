using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController : MonoBehaviour {

    public enum GameMode {
        NONE,
        EDIT,
        PLAY
    };

	public GameMode startMode;

	[Header("Edit Mode")]
	public EditorController editorController;
	public GameObject editorUI;
	public GameObject editorCamera;

	[Header("Play Mode")]
	public RaceController playController;
	public GameObject playUI;
	public GameObject playCamera;
	public GameObject player;
	public GameObject spawnpoint;

	private GameMode _mode = GameMode.NONE;

	public void SetMode(GameMode mode)
	{
		if (mode == _mode)
			return;

		_mode = mode;
		switch (mode)
		{
			case GameMode.EDIT:
				player.SetActive(false);

				editorUI.SetActive(true);
				editorCamera.SetActive(true);

				playUI.SetActive(false);
				playCamera.SetActive(false);

				foreach (var c in GameObject.FindGameObjectsWithTag("Checkpoint"))
				{
					c.GetComponent<Checkpoint>().Show(true);
				}
				break;

			case GameMode.PLAY:
				player.transform.position = spawnpoint.transform.position;
				player.transform.rotation = spawnpoint.transform.rotation;
				player.SetActive(true);

				editorUI.SetActive(false);
				editorCamera.SetActive(false);

				playUI.SetActive(true);
				playCamera.SetActive(true);

				RaceController.Restart();
				break;
		}
	}

	void Start()
    {
		SetMode(startMode);
	}
}
