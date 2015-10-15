using UnityEngine;
using System.Collections;

public class GravitySwitchController : MonoBehaviour {

	public int gravDirection = 0;

	void Awake () {
		switch (gravDirection) {
		case 0 : this.GetComponent<SpriteRenderer>().color = Color.blue;
			break;
		case 1 : this.GetComponent<SpriteRenderer>().color = Color.yellow;
			break;
		case 2 : this.GetComponent<SpriteRenderer>().color = Color.red;
			break;
		case 3 : this.GetComponent<SpriteRenderer>().color = Color.green;
			break;
		}
	}

}
