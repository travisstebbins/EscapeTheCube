using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

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

}
