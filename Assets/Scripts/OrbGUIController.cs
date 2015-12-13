using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OrbGUIController : MonoBehaviour {

	// public variables
	public Sprite orbGUI0;
	public Sprite orbGUI1;
	public Sprite orbGUI2;
	public Sprite orbGUI3;

	// components
	Image img;

	// private variables
	GameManager gm;

	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>() ;
		img = GetComponent<Image> ();
		SetGUI (gm.getNumOrbs ());
	}
	
	public void SetGUI (int numOrbs) {
		Debug.Log ("SetGUI: " + numOrbs);
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
