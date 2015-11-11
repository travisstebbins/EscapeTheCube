using UnityEngine;
using System.Collections;

public class MemoryOrbController : MonoBehaviour {

	private GameManager gm;
	
	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		if (gm.getOrbCollectedThisLevel ())
			Destroy (this.gameObject);
	}
}
