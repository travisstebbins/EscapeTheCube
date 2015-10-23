using UnityEngine;
using System.Collections;

public class MyCharacterController2D : MonoBehaviour {

	// public variables
	public int hp = 5;
	public float maxSpeed = 20f;
	public float jumpHeight = 10f;	
	public float groundPoundSpeed = 8f;
	public float gravMagnitude = 85;
	public Transform groundCheck;
	public float groundRadius = 1.035f;
	public LayerMask groundLayerMask;

	// components
	private Rigidbody2D rb;
	private Animator anim;

	// helper variables
	private Vector2 startingPos;
	private bool facingRight = true;
	private bool isGrounded = false;
	private bool doubleJump = false;
	private bool onMovingPlatform = false;
	private int gravDirection;

	// unity functions	
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		Physics2D.gravity = new Vector2 (0, -gravMagnitude);
		startingPos = transform.position;
	}

	void FixedUpdate () {
		// check if character is grounded
		isGrounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, groundLayerMask);
		if (isGrounded)
			doubleJump = false;
		anim.SetBool ("isGrounded", isGrounded);
		if (gravDirection == 0 || gravDirection == 2) {
			float move = Input.GetAxis ("Horizontal");
			anim.SetFloat ("speed", Mathf.Abs (move));
			rb.velocity = new Vector2 (isGrounded ? move * maxSpeed : move * maxSpeed * 0.8f, rb.velocity.y);

			if (gravDirection == 0) {
				if (Input.GetKey (KeyCode.DownArrow))
					rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y - groundPoundSpeed);
				if (move > 0 && !facingRight)
					Flip ();
				else if (move < 0 && facingRight)
					Flip ();
			} else if (gravDirection == 2) {
				if (Input.GetKey (KeyCode.UpArrow))
					rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y + groundPoundSpeed);
				if (move < 0 && !facingRight)
					Flip ();
				else if (move > 0 && facingRight)
					Flip ();
			}			

		} else if (gravDirection == 1 || gravDirection == 3) {
			float move = Input.GetAxis ("Vertical");
			anim.SetFloat ("speed", Mathf.Abs (move));
			rb.velocity = new Vector2 (rb.velocity.x, isGrounded ? move * maxSpeed : move * maxSpeed * 0.8f);

			if (gravDirection == 1) {
				if (Input.GetKey (KeyCode.LeftArrow))
					rb.velocity = new Vector2 (rb.velocity.x - groundPoundSpeed, rb.velocity.y);
				if (move < 0 && !facingRight)
					Flip ();
				else if (move > 0 && facingRight)
					Flip ();
			} else if (gravDirection == 3) {
				if (Input.GetKey (KeyCode.RightArrow))
					rb.velocity = new Vector2 (rb.velocity.x + groundPoundSpeed, rb.velocity.y);
				if (move > 0 && !facingRight)
					Flip ();
				else if (move < 0 && facingRight)
					Flip ();
			}
		}

		onMovingPlatform = Physics2D.OverlapCircle (groundCheck.position, groundRadius, LayerMask.GetMask ("MovingPlatform"));
		if (onMovingPlatform) {
			Vector2 movingPlatformVelocity = Physics2D.OverlapCircle (groundCheck.position, groundRadius, LayerMask.GetMask ("MovingPlatform")).GetComponent<MovingPlatformController>().getVelocity();
			rb.velocity = new Vector2 (movingPlatformVelocity.x + rb.velocity.x, movingPlatformVelocity.y + rb.velocity.y);
		}
	}

	void Update () {
		if (gravDirection == 0) {
			if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.UpArrow)) {
				anim.SetBool ("isGrounded", false);
				if (!isGrounded && !doubleJump)
					doubleJump = true;
				rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));
			}
		} else if (gravDirection == 1) {
			if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.RightArrow)) {
				anim.SetBool ("isGrounded", false);
				if (!isGrounded && !doubleJump)
					doubleJump = true;
				rb.velocity = new Vector2 (Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.x), rb.velocity.y);
			}
		} else if (gravDirection == 2) {
			if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.DownArrow)) {
				anim.SetBool ("isGrounded", false);
				if (!isGrounded && !doubleJump)
					doubleJump = true;
				rb.velocity = new Vector2 (rb.velocity.x, -Mathf.Sqrt (2f * jumpHeight * Physics2D.gravity.y));
			}
		} else if (gravDirection == 3) {
			if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.LeftArrow)) {
				anim.SetBool ("isGrounded", false);
				if (!isGrounded && !doubleJump)
					doubleJump = true;
				rb.velocity = new Vector2 (-Mathf.Sqrt (2f * jumpHeight * Physics2D.gravity.x), rb.velocity.y);
			}
		}
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (coll.CompareTag ("GravitySwitch")) {			
			int direction1 = gravDirection;
			gravDirection = coll.gameObject.GetComponent<GravitySwitchController>().gravDirection;
			switch (gravDirection) {
			case 0 :
				Physics2D.gravity = new Vector2 (0, -gravMagnitude);
				RotatePlayer (direction1, gravDirection);
				break;
			case 1 :
				Physics2D.gravity = new Vector2 (-gravMagnitude, 0);
				RotatePlayer (direction1, gravDirection);
				break;
			case 2 :
				Physics2D.gravity = new Vector2(0, gravMagnitude);
				RotatePlayer (direction1, gravDirection);
				break;
			case 3:
				Physics2D.gravity = new Vector2(gravMagnitude, 0);
				RotatePlayer (direction1, gravDirection);
				break;
			}
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		//if (hit.rigidbody.gameObject.CompareTag ("Enemy"))
		//hit.rigidbody.gameObject.GetComponent<EnemyController> ().DamagePlayer ();
		if (coll.gameObject.CompareTag ("KillZone")) {
			KillPlayer();
		}

		if (coll.rigidbody.gameObject.CompareTag ("FallingPlatform")) {
			coll.rigidbody.gameObject.GetComponent<FallingPlatformController>().Fall ();
		}
	}

	// my functions

	void RotatePlayer (int direction1, int direction2) {
		int startRotation;
		int endRotation;
		switch (direction1) {
		case 0:
			startRotation = 0;
			break;
		case 1:
			startRotation = 270;
			break;
		case 2:
			startRotation = 180;
			break;
		case 3:
			startRotation = 90;
			break;
		default:
			startRotation = 0;
			break;
		}
		switch (direction2) {
		case 0:
			endRotation = 0;
			break;
		case 1:
			endRotation = 270;
			break;
		case 2:
			endRotation = 180;
			break;
		case 3:
			endRotation = 90;
			break;
		default:
			endRotation = 0;
			break;
		}
		if (startRotation < endRotation) {
			Transform from = transform;
			Transform to = from;
			to.rotation = Quaternion.Euler (new Vector3(0,0,endRotation));
			transform.rotation = Quaternion.Slerp (from.rotation, to.rotation, Time.time * 0.1f);
			/*while (transform.rotation.eulerAngles.z < endRotation) {
				transform.rotation = Quaternion.Euler (0, 0, transform.rotation.eulerAngles.z + rotateSpeed);
				if (transform.rotation.eulerAngles.z >= endRotation) {
					transform.rotation = Quaternion.Euler (0, 0, endRotation);
					break;
				}
			}*/
		}
		else if (startRotation > endRotation) {
			Transform from = transform;
			Transform to = from;
			to.rotation = Quaternion.Euler (new Vector3(0,0,endRotation));
			transform.rotation = Quaternion.Slerp (from.rotation, to.rotation, Time.time * 0.1f);
			/*while (transform.rotation.eulerAngles.z > endRotation) {
				transform.rotation = Quaternion.Euler (0, 0, transform.rotation.eulerAngles.z - rotateSpeed);
				if (transform.rotation.eulerAngles.z <= endRotation) {
					transform.rotation = Quaternion.Euler (0, 0, endRotation);
					break;
				}
			}*/
		}
	}

	void Flip () {
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	public void KillPlayer () {
		transform.position = startingPos;
		hp = 5;
		rb.velocity = Vector2.zero;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ().Reset ();
		gravDirection = 0;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		Physics2D.gravity = new Vector2 (0, -gravMagnitude);
	}
}
