using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed = 1f;
	public float jumpForce = 1f;

	private Rigidbody2D rb;
	private bool grounded = false;
	private bool doubleJump = false;
	private Vector3 prevPos;
	private bool doubleGrav = false;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis ("Horizontal");
		if (grounded)
			transform.Translate (new Vector3 (horizontal * speed, 0, 0) * Time.deltaTime);
		if (Input.GetKeyDown (KeyCode.Space) && (grounded || !doubleJump)) {
			rb.AddForce(new Vector2(0, jumpForce));
			if (grounded) {
				grounded = false;
				rb.velocity = (transform.position - prevPos) / Time.deltaTime;
			}
			else
				doubleJump = true;
		}
		if (rb.velocity.y < 0 && !doubleGrav) {
			Physics2D.gravity = Physics2D.gravity * 2.0f;
			doubleGrav = true;
		}
		prevPos = transform.position;
	}

	void FixedUpdate () {
		if (rb.velocity.y < 0 && !doubleGrav) {
			Physics2D.gravity = Physics2D.gravity * 2.0f;
			doubleGrav = true;
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.gameObject.CompareTag("Ground")) {
			grounded = true;
			doubleJump = false;
			if (doubleGrav) {
				Physics2D.gravity = Physics2D.gravity / 2.0f;
				doubleGrav = false;
			}
		}
	}
}
