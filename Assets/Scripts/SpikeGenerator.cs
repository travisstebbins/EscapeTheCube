using UnityEngine;
using System.Collections;

public class SpikeGenerator : MonoBehaviour {

	public Sprite [] sprites;

	// Use this for initialization
	void Awake () {
		int i = Random.Range (0, sprites.Length);
		GetComponent<SpriteRenderer> ().sprite = sprites [i];
	}
}
