using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour {

	// public variables
	public GameObject pauseMenu;

	// private variables
	bool fadeToBlack;
	Image black;

	void Start () {
		if (Application.loadedLevelName == "MainMenu") {
			black = GameObject.FindGameObjectWithTag ("Black").GetComponent<Image> ();
			black.gameObject.SetActive (false);
		}
	}

	void Update () {
		if (fadeToBlack && Application.loadedLevelName == "MainMenu") {
			black.color = new Color (black.color.r, black.color.g, black.color.b, black.color.a + (2f * Time.deltaTime));
			if(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume > 0)
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume -= (2f * Time.deltaTime);
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (!pauseMenu.activeInHierarchy && Time.timeScale != 0) {
				Time.timeScale = 0;
				pauseMenu.SetActive (true);
			}
			else if (pauseMenu.activeInHierarchy && Time.timeScale == 0) {
				Time.timeScale = 1;
				pauseMenu.SetActive (false);
			}
		}
	}

	// main menu
	public void Play () {
		Time.timeScale = 1;
		black.gameObject.SetActive (true);
		fadeToBlack = true;
		ParticleSystem mainMenuParticles = GameObject.FindGameObjectWithTag ("MainMenuParticles").GetComponent<ParticleSystem> ();
		mainMenuParticles.gameObject.SetActive (false);
		StartCoroutine (PlayCoroutine ());
	}

	IEnumerator PlayCoroutine () {
		yield return null;
		SoundManager sm = GameObject.FindGameObjectWithTag ("SoundManager").GetComponent<SoundManager> ();
		sm.PlaySound (7);
		yield return new WaitForSeconds (10.5f);
		fadeToBlack = false;
		Application.LoadLevel ("TutorialLevel");
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
			Time.timeScale = 1;
			Application.LoadLevel ("TutorialLevel");
			break;
		case 1:
			Time.timeScale = 1;
			Application.LoadLevel ("Level1");
			break;
		case 2:
			Time.timeScale = 1;
			Application.LoadLevel ("Level2");
			break;
		}
	}

	// pause menu
	public void Resume () {
		Time.timeScale = 1;
	}

	public void Restart () {
		Time.timeScale = 1;
		Application.LoadLevel (Application.loadedLevel);
	}

	public void MainMenu () {
		Time.timeScale = 1;
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
