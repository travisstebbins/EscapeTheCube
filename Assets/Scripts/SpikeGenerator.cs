using UnityEngine;
using System.Collections;

public class SpikeGenerator : MonoBehaviour {

	public Sprite [] sprites;

	// Use this for initialization
	void Start () {
		int i = Random.Range (0, sprites.Length);
		GetComponent<SpriteRenderer> ().sprite = sprites [i];
		float scale = Random.Range (0.75f, 1.25f);
		transform.localScale = new Vector3 (scale, scale, scale);
	}
}
