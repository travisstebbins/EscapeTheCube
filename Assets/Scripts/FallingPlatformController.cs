using UnityEngine;
using System.Collections;

public class FallingPlatformController : MonoBehaviour {

	public float delay = 1f;
	public float lifespan = 1f;
	public float fallSpeed = 8f;

	private Rigidbody2D rb;
	private bool falling;
	private float fallTime;
	
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		falling = false;
	}

	/*void Update () {
		if (falling) {
			fallTime = Time.time + lifespan;
			falling = false;
		}
		if (Time.time >= fallTime)
			Destroy (this);
	}*/

	public void Fall () {
		if (!falling)
			StartCoroutine (FallingCoroutine ());
	}

	IEnumerator FallingCoroutine () {		
		falling = true;
		yield return null;
		yield return new WaitForSeconds(delay);
		rb.velocity = new Vector2 (0, -fallSpeed);
		Destroy (this.gameObject, lifespan);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.CompareTag ("KillZone"))
		    Destroy (this.gameObject);
	}
}
