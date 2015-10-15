using UnityEngine;
using System.Collections;

public class MovingPlatformController : MonoBehaviour {

	public float speed = 5f;
	public Vector2 moveDistance = new Vector2 (-15f, 0f);

	private Vector2 startingPos;
	private Rigidbody2D rb;
	
	void Start () {
		startingPos = transform.position;
		rb = GetComponent<Rigidbody2D> ();
		rb.velocity = new Vector2 (speed * (moveDistance.x > 0 ? 1 : (moveDistance.x < 0 ? -1 : 0)), speed * (moveDistance.y > 0 ? 1 : (moveDistance.y < 0 ? -1 : 0)));
	}

	void Update () {
		if (moveDistance.x < 0) {
			if (transform.position.x <= startingPos.x + moveDistance.x) {
				transform.position = new Vector2(startingPos.x + moveDistance.x, transform.position.y);
				rb.velocity = new Vector2 (rb.velocity.x * -1, rb.velocity.y);
			}
			else if (transform.position.x >= startingPos.x) {
				transform.position = new Vector2(startingPos.x, transform.position.y);
				rb.velocity = new Vector2 (rb.velocity.x * -1, rb.velocity.y);
			}
		}
	}

	public Vector3 getVelocity() {
		return rb.velocity;
	}
}
