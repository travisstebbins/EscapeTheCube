using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prime31;

public class PlayerController : MonoBehaviour {

	public float gravity = -30f;
	public float runSpeed = 8f;
	public float targetJumpHeight = 10f;

	public Text velocityText;

	private CharacterController2D controller;

	void Awake () {
		controller = GetComponent<CharacterController2D> ();
	}

	void Update () {
		Vector3 velocity = controller.velocity;

		if (controller.isGrounded)
			velocity.y = 0;

		if (Input.GetKey (KeyCode.RightArrow)) {
			if (controller.isGrounded)
				velocity.x = runSpeed;
			else
				velocity.x = runSpeed * 0.75f;
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			if (controller.isGrounded)
				velocity.x = -runSpeed;
			else
				velocity.x = -runSpeed * 0.75f;
		} else {
			velocity.x = 0;
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			velocity.y -= 10f;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			velocity.y = Mathf.Sqrt (2f * targetJumpHeight * -gravity);
		}

		velocity.y += gravity * Time.deltaTime;

		controller.move (velocity * Time.deltaTime);
		velocityText.text = "yVel: " + controller.velocity.y;
	}

	void FixedUpdate () {
		if (controller.velocity.y < -float.Epsilon) {
			controller.velocity.y -= 2f;
		}
	}
}

/*
	public float speed = 1f;
	public float jumpForce = 1f;
	public Text velocityText;

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
			rb.MovePosition (new Vector2 (rb.position.x + (horizontal * speed * Time.deltaTime), rb.position.y));
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
		velocityText.text = "yVel: " + rb.velocity.y;
		if (rb.velocity.y < -10f && !doubleGrav && !grounded) {
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
*/