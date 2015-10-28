using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	// public variables
	public int hp = 5;
	public float maxSpeed = 20f;
	public float jumpHeight = 10f;	
	public float groundPoundSpeed = 8f;
	public int damage = 1;
	public float meleeAttackDistance = 4f;
	public float attackKickback = 5;
	public float gravMagnitude = 85;
	public Transform groundCheck;
	public float groundRadius = 1.035f;
	public float kickbackTime = 0.002f;
	public float damageDelay = 0.5f;
	public float enemyHitVerticalVelocity = 25f;
	public LayerMask groundLayerMask;

	// components
	private Rigidbody2D rb;
	private Animator anim;
	private CircleCollider2D cColl;
	private BoxCollider2D bColl;

	// helper variables
	private Vector2 startingPos;
	private bool facingRight = true;
	private bool isGrounded = false;
	private bool doubleJump = false;
	private bool onMovingPlatform = false;
	private bool isHit;
	private bool kickback;
	private int gravDirection;
	private GameObject[] fallingPlatforms;

	// unity functions	
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		cColl = GetComponent<CircleCollider2D> ();
		bColl = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();
		Physics2D.gravity = new Vector2 (0, -gravMagnitude);
		startingPos = transform.position;
		fallingPlatforms = GameObject.FindGameObjectsWithTag ("FallingPlatform");
	}

	void FixedUpdate () {
		// check if character is grounded
		isGrounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, groundLayerMask);
		if (isGrounded) {
			doubleJump = false;
			cColl.offset = new Vector2 (0.08f, -1.9f);
			bColl.offset = new Vector2 (0.08f, 0.42f);
			bColl.size = new Vector2 (2.07f, 4.44f);
		} else {
			cColl.offset = new Vector2 (0.08f, 0.04f);
			bColl.offset = new Vector2 (0.08f, 1.12f);
			bColl.size = new Vector2 (2.07f, 2.37f);
		}
		anim.SetBool ("isGrounded", isGrounded);
		if (gravDirection == 0 || gravDirection == 2) {
			float move = Input.GetAxis ("Horizontal");
			anim.SetFloat ("speed", Mathf.Abs (move));
			if (!kickback)
				rb.velocity = new Vector2 (isGrounded ? move * maxSpeed : move * maxSpeed * 0.8f, rb.velocity.y);

			if (gravDirection == 0) {
				anim.SetFloat ("vSpeed", rb.velocity.y);
				if (Input.GetKey (KeyCode.DownArrow))
					rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y - groundPoundSpeed);
				if (move > 0 && !facingRight)
					Flip ();
				else if (move < 0 && facingRight)
					Flip ();
			} else if (gravDirection == 2) {
				anim.SetFloat ("vSpeed", -rb.velocity.y);
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
			if (!kickback)
				rb.velocity = new Vector2 (rb.velocity.x, isGrounded ? move * maxSpeed : move * maxSpeed * 0.8f);

			if (gravDirection == 1) {
				anim.SetFloat ("vSpeed", rb.velocity.x);
				if (Input.GetKey (KeyCode.LeftArrow))
					rb.velocity = new Vector2 (rb.velocity.x - groundPoundSpeed, rb.velocity.y);
				if (move < 0 && !facingRight)
					Flip ();
				else if (move > 0 && facingRight)
					Flip ();
			} else if (gravDirection == 3) {
				anim.SetFloat ("vSpeed", -rb.velocity.x);
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
		if (Input.GetKeyDown (KeyCode.Space)) {
			Vector2 corner1;
			Vector2 corner2;
			if (gravDirection == 0 || gravDirection == 2) {
				if (facingRight) {
					corner1 = new Vector2 (bColl.bounds.max.x, bColl.bounds.max.y);
					corner2 = new Vector2 (bColl.bounds.max.x + meleeAttackDistance, bColl.bounds.min.y);
				}
				else {
					corner1 = new Vector2 (bColl.bounds.min.x, bColl.bounds.max.y);
					corner2 = new Vector2 (bColl.bounds.min.x - meleeAttackDistance, bColl.bounds.min.y);
				}
			}
			else {
				if (facingRight) {
					corner1 = new Vector2 (bColl.bounds.min.x, bColl.bounds.max.y);
					corner2 = new Vector2 (bColl.bounds.max.x, bColl.bounds.max.y + meleeAttackDistance);
				}
				else {
					corner1 = new Vector2 (bColl.bounds.min.x, bColl.bounds.min.y);
					corner2 = new Vector2 (bColl.bounds.max.x, bColl.bounds.min.y - meleeAttackDistance);
				}
			}
			Collider2D coll = Physics2D.OverlapArea (corner1, corner2, LayerMask.GetMask("Enemy"));
			if (coll.gameObject != null)
			{
				if (!coll.gameObject.GetComponent<EnemyController>().getIsHit ())
					coll.gameObject.GetComponent<EnemyController>().Damage (this);
			}
		}
		if (gravDirection == 0) {

			/*if (Input.GetKeyDown (KeyCode.Space)) {
				hits.Clear ();
				Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z), new Vector3 (transform.localScale.x * meleeAttackDistance, 0, 0), Color.red);
				Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (transform.localScale.x * meleeAttackDistance, 0, 0), Color.red);
				Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y - 2f, transform.position.z), new Vector3 (transform.localScale.x * meleeAttackDistance, 0, 0), Color.red);
				hits.Add (Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y - 2f), new Vector2 (transform.localScale.x, 0), meleeAttackDistance, LayerMask.GetMask ("Enemy")));
				hits.Add (Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y), new Vector2 (transform.localScale.x, 0), meleeAttackDistance, LayerMask.GetMask ("Enemy")));
				hits.Add (Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y + 2f), new Vector2 (transform.localScale.x, 0), meleeAttackDistance, LayerMask.GetMask ("Enemy")));
				for (int i = 0; i < hits.Count; ++i) {
					Debug.Log (i);

					if (hits[i].rigidbody.gameObject != null && hits[i].rigidbody.gameObject.CompareTag ("Enemy"))
						Debug.Log ("enemy hit");
				}
				Collider2D coll = Physics2D.OverlapArea (new Vector2 (bColl.bounds.max.x, bColl.bounds.max.y), new Vector2 (bColl.bounds.max.x + meleeAttackDistance, bColl.bounds.min.y), LayerMask.NameToLayer("Enemy"));
				if (coll.gameObject.CompareTag("Enemy")) {
					Debug.Log("enemy hit");
				}
			}*/

			if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.UpArrow)) {
				anim.SetBool ("isGrounded", false);
				if (!isGrounded && !doubleJump)
					doubleJump = true;
				rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));

			}
		} else if (gravDirection == 1) {
			Debug.DrawRay (new Vector3 (transform.position.x + 2f, transform.position.y, transform.position.z), new Vector3 (0, -transform.localScale.x * meleeAttackDistance, 0), Color.red);
			Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (0, -transform.localScale.x * meleeAttackDistance, 0), Color.red);
			Debug.DrawRay (new Vector3 (transform.position.x - 2f, transform.position.y, transform.position.z), new Vector3 (0, -transform.localScale.x * meleeAttackDistance, 0), Color.red);
			if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.RightArrow)) {
				anim.SetBool ("isGrounded", false);
				if (!isGrounded && !doubleJump)
					doubleJump = true;
				rb.velocity = new Vector2 (Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.x), rb.velocity.y);
			}
		} else if (gravDirection == 2) {
			Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z), new Vector3 (-transform.localScale.x * meleeAttackDistance, 0, 0), Color.red);
			Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (-transform.localScale.x * meleeAttackDistance, 0, 0), Color.red);
			Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y - 2f, transform.position.z), new Vector3 (-transform.localScale.x * meleeAttackDistance, 0, 0), Color.red);
			if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.DownArrow)) {
				anim.SetBool ("isGrounded", false);
				if (!isGrounded && !doubleJump)
					doubleJump = true;
				rb.velocity = new Vector2 (rb.velocity.x, -Mathf.Sqrt (2f * jumpHeight * Physics2D.gravity.y));
			}
		} else if (gravDirection == 3) {
			Debug.DrawRay (new Vector3 (transform.position.x + 2f, transform.position.y, transform.position.z), new Vector3 (0, transform.localScale.x * meleeAttackDistance, 0), Color.red);
			Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (0, transform.localScale.x * meleeAttackDistance, 0), Color.red);
			Debug.DrawRay (new Vector3 (transform.position.x - 2f, transform.position.y, transform.position.z), new Vector3 (0, transform.localScale.x * meleeAttackDistance, 0), Color.red);
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
		if (coll.rigidbody.gameObject.CompareTag ("Enemy"))
			coll.rigidbody.gameObject.GetComponent<EnemyController> ().DamagePlayer ();
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

	public void Damage (EnemyController enemy) {
		Debug.Log ("player damaged");
		hp -= enemy.damage;
		isHit = true;
		kickback = true;
		Vector2 heading = transform.position - enemy.transform.position;
		Vector2 direction = heading / heading.magnitude;
		Debug.Log ("x direction: " + direction.x + ", y direction: " + direction.y);
		StartCoroutine (DamageCoRoutine(direction, enemy));
	}
	
	IEnumerator DamageCoRoutine (Vector2 direction, EnemyController enemy) {
		yield return null;
		if (gravDirection == 0 || gravDirection == 2)
			rb.velocity = new Vector2 (-direction.x * (enemy.GetComponent<Rigidbody2D> ().velocity.x < 0 ? enemy.speed : -enemy.speed) * 10, isGrounded ? 0 : gravDirection == 0 ? direction.y * enemyHitVerticalVelocity : -direction.y * enemyHitVerticalVelocity);
		else
			rb.velocity = new Vector2 (isGrounded ? 0 : gravDirection == 1 ? -direction.x * enemyHitVerticalVelocity : direction.x * enemyHitVerticalVelocity, -direction.y * (enemy.GetComponent<Rigidbody2D>().velocity.y < 0 ? enemy.speed : -enemy.speed) * 10);
		yield return new WaitForSeconds (kickbackTime);
		rb.velocity = Vector2.zero;
		kickback = false;
		yield return new WaitForSeconds (damageDelay - kickbackTime);
		isHit = false;
		Debug.Log ("isHit reset");
		/*yield return null;
		yield return new WaitForSeconds (hitKickbackTime);
		kickback = false;
		yield return new WaitForSeconds (damageDelay - attackKickback);
		isHit = false;*/
	}

	public bool getIsHit () {
		return isHit;
	}

	public void KillPlayer () {
		transform.position = startingPos;
		hp = 5;
		rb.velocity = Vector2.zero;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ().Reset ();
		gravDirection = 0;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		Physics2D.gravity = new Vector2 (0, -gravMagnitude);
		ReEnableFallingPlatforms ();
	}

	void ReEnableFallingPlatforms () {
		for (int i = 0; i < fallingPlatforms.Length; ++i) {
			fallingPlatforms[i].GetComponent<FallingPlatformController>().Reset ();
		}
	}
}
