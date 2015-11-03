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

}
