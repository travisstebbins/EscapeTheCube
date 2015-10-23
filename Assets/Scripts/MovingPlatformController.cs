using UnityEngine;
using System.Collections;

public class MovingPlatformController : MonoBehaviour {

	public float speed = 10f;
	public Vector2 moveDistance = new Vector2 (-15f, 0f);
	public float pause = 1.5f;

	private Vector2 startingPos;
	private Vector2 startingVel;
	private Rigidbody2D rb;
	private bool pauseComplete;
	private bool reachedEnd;
	
	void Start () {
		pauseComplete = false;
		reachedEnd = false;
		startingPos = transform.position;
		rb = GetComponent<Rigidbody2D> ();
		startingVel = new Vector2 (speed * (moveDistance.x > 0 ? 1 : (moveDistance.x < 0 ? -1 : 0)), speed * (moveDistance.y > 0 ? 1 : (moveDistance.y < 0 ? -1 : 0)));
		rb.velocity = startingVel;
	}

	void Update () {
		if (moveDistance.x < 0) {
			if (transform.position.x <= startingPos.x + moveDistance.x && !reachedEnd) {
				reachedEnd = true;
				transform.position = new Vector2 (startingPos.x + moveDistance.x, transform.position.y);
				rb.velocity = Vector2.zero;
				StartCoroutine (Wait ());
			} else if (transform.position.x >= startingPos.x && !reachedEnd) {
				reachedEnd = true;
				transform.position = new Vector2 (startingPos.x, transform.position.y);
				rb.velocity = Vector2.zero;
				StartCoroutine (Wait ());
			}
			if (pauseComplete) {					
				startingVel = new Vector2 (startingVel.x * -1, startingVel.y);
				rb.velocity = startingVel;
				pauseComplete = false;
				reachedEnd = false;
			}
		} else if (moveDistance.x > 0) {
			if (transform.position.x >= startingPos.x + moveDistance.x && !reachedEnd) {
				reachedEnd = true;
				transform.position = new Vector2 (startingPos.x + moveDistance.x, transform.position.y);
				rb.velocity = Vector2.zero;
				StartCoroutine(Wait());
			} else if (transform.position.x <= startingPos.x && !reachedEnd) {
				reachedEnd = true;
				transform.position = new Vector2 (startingPos.x, startingPos.y);
				rb.velocity = Vector2.zero;
				StartCoroutine (Wait ());
			}
			if (pauseComplete) {
				startingVel = new Vector2 (startingVel.x * -1, startingVel.y);
				rb.velocity = startingVel;
				pauseComplete = false;
				reachedEnd = false;
			}
		}
	}

	IEnumerator Wait() {
		yield return null;
		yield return new WaitForSeconds (pause);
		pauseComplete = true;
	}

	public Vector3 getVelocity() {
		return rb.velocity;
	}
}
