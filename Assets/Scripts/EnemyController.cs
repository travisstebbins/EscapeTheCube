using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public float speed = -5f;
	public int damage = 1;

	private Rigidbody2D rb;
	private int hp = 5;
	private int hitDistance = 5;
	
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	void FixedUpdate () {
		rb.velocity = new Vector2 (speed, rb.velocity.y);
	}

	void Update () {
		GetComponent<Collider2D> ().enabled = false;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, new Vector2 (speed, 0), hitDistance, LayerMask.GetMask ("Player"));
		GetComponent<Collider2D> ().enabled = true;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("TurnaroundTrigger")) {
			speed *= -1;
		}
	}
}
