using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OrbGUIController : MonoBehaviour {

	// public variables
	Sprite orbGUI0;
	Sprite orbGUI1;
	Sprite orbGUI2;
	Sprite orbGUI3;

	// components
	Image img;

	// private variables
	GameManager gm;

	// Use this for initialization
	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>() ;
		img = GetComponent<Image> ();
		SetGUI (gm.getNumOrbs ());
	}
	
	public void SetGUI (int numOrbs) {
		switch (numOrbs) {
			case 0:
				img.sprite = orbGUI0;
				break;
			case 1:
				img.sprite = orbGUI1;
				break;
			case 2:
				img.sprite = orbGUI2;
				break;
			case 3:
				img.sprite = orbGUI3;
				break;
			default:
				img.sprite = orbGUI0;
				break;
		}
	}
}
