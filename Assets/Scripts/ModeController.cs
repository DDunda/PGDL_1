using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModeController : MonoBehaviour {

    public enum GameMode {
        NONE,
        EDIT,
        PLAY
    };

	public GameMode startMode;
	public KeyCode toggleModeKey;

	private GameMode _mode = GameMode.NONE;

	public void SetMode(GameMode mode)
	{
		if (mode == _mode)
			return;

		_mode = mode;
		switch (mode)
		{
			case GameMode.EDIT:
				Controller.playController.enabled = false;
				Controller.playerMovement.enabled = false;
				Controller.playUI.SetActive(false);
				Controller.playerCamObj.SetActive(false);

				Controller.editorController.enabled = true;
				break;

			case GameMode.PLAY:
				Controller.editorController.enabled = false;

				Controller.playController.enabled = true;
				Controller.playerMovement.enabled = true;
				Controller.playUI.SetActive(true);
				Controller.playerCamObj.SetActive(true);
				break;
		}
	}

	public void ToggleMode()
	{
		SetMode(_mode switch { GameMode.EDIT => GameMode.PLAY, GameMode.PLAY => GameMode.EDIT, _ => GameMode.EDIT });
	}

	void Start()
    {
		foreach(var c in GameObject.FindGameObjectsWithTag("Checkpoint").Select(x => x.GetComponent<Checkpoint>()).OrderBy(x => x.index))
		{
			CheckpointController.AppendCheckpoint(c);
		}
		Controller.spawnpoint.position = Controller.player.transform.position;
		Controller.spawnpoint.rotation = Controller.playerCamObj.transform.rotation;
		RaceController.RespawnPlayer();
		SetMode(startMode);
	}

	private void Update()
	{
		if(Input.GetKeyDown(toggleModeKey)) {
			ToggleMode();
		}
	}
}