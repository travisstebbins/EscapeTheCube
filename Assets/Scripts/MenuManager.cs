using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	// public variables
	public GameObject pauseMenu;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Time.timeScale = 0;
			pauseMenu.SetActive (true);
		}
	}

	// main menu
	public void Play () {
		Application.LoadLevel ("Level1");
	}

	public void LevelSelect () {

	}

	public void Options () {

	}

	public void Quit () {
		Application.Quit ();
	}

	public void LoadLevel (int l) {
		switch (l) {
		case 0:
			Application.LoadLevel ("Tutorial");
			break;
		case 1:
			Application.LoadLevel ("Level1");
			break;
		case 2:
			Application.LoadLevel ("Level2");
			break;
		}
	}

	// pause menu
	public void Resume () {
		Time.timeScale = 1;
	}

	public void Restart () {
		Application.LoadLevel (Application.loadedLevel);
	}

	public void MainMenu () {
		Application.LoadLevel ("MainMenu");
	}

	private bool muted = false;
	private float audioLevel = AudioListener.volume;
	public void Sound () {
		if (!muted) {
			muted = true;
			audioLevel = AudioListener.volume;
			AudioListener.volume = 0;
		} else {
			muted = false;
			AudioListener.volume = audioLevel;
		}
	}

}
