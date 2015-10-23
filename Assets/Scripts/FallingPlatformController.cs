using UnityEngine;
using System.Collections;

public class FallingPlatformController : MonoBehaviour {

	public float delay = 1f;
	public float lifespan = 1f;
	public float fallSpeed = 8f;

	private Rigidbody2D rb;
	private bool falling;
	private float fallTime = 99999f;
	private Vector2 startPos;
	private bool timeMeasured;
	
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.gravityScale = 0;
		falling = false;
		startPos = transform.position;
		timeMeasured = false;
	}

	void Update () {
		if (!timeMeasured && falling) {
			fallTime = Time.time + delay + lifespan;
			timeMeasured = true;
		}
		if (falling && Time.time >= fallTime) {
			this.gameObject.SetActive (false);
			falling = false;
		}
	}

	public void Fall () {
		if (!falling)
			StartCoroutine (FallingCoroutine ());
	}

	IEnumerator FallingCoroutine () {		
		falling = true;
		yield return null;
		yield return new WaitForSeconds(delay);
		rb.velocity = new Vector2 (0, -fallSpeed);
		//this.gameObject.SetActive (false);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.CompareTag ("KillZone"))
			this.gameObject.SetActive (false);
	}

	public void Reset () {
		StopAllCoroutines ();
		transform.position = startPos;
		falling = false;
		timeMeasured = false;
		this.gameObject.SetActive (true);
		rb.velocity = Vector2.zero;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
	}

}
