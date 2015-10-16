using UnityEngine;
using System.Collections;

public class GravitySwitchController : MonoBehaviour {

	[Range (0,3)]
	public int gravDirection = 0;

	void Awake () {
		switch (gravDirection) {
		case 0 :	this.GetComponent<SpriteRenderer>().color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.5f);
					break;
		case 1 : 	this.GetComponent<SpriteRenderer>().color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f);
					break;
		case 2 : 	this.GetComponent<SpriteRenderer>().color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);
					break;
		case 3 : 	this.GetComponent<SpriteRenderer>().color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f);
					break;
		}
	}

}
