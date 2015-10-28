using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prime31;
using System.Collections.Generic;

public class Archive_PlayerController : MonoBehaviour {

	struct CharacterRaycastOrigins {
		public Vector2 topLeft;
		public Vector2 bottomLeft;
		public Vector2 topRight;
		public Vector2 bottomRight;
		public Vector2 topCenter;
		public Vector2 bottomCenter;
	}

	public float gravity = -30f;
	public float runSpeed = 8f;
	public float rotateSpeed = 30f;
	public float targetJumpHeight = 10f;
	public int damage = 1;
	public float meleeDamageDistance = 10f;
	public float attackKickback = 1000f;
	public Text hpText;
	public Text winText;
	
	private Archive_CharacterController2D controller;
	private int hp = 5;
	private int gravDirection;
	private bool doubleJump;
	private Rigidbody2D rb;
	private bool isHit = false;
	private float damageDelay = 1f;
	private Vector2 startingPos;
	private GameObject[] fallingPlatforms;
	private Vector2 currentDirection;
	private Vector2 direction;
	private float hitKickbackTime = 0.1f;
	private bool kickback;
	private int spriteDirection;
	private SpriteRenderer rend;
	private Animator anim;
	private BoxCollider2D boxCollider;
	CharacterRaycastOrigins raycastOrigins;

	void Awake () {
		controller = GetComponent<Archive_CharacterController2D> ();
		gravDirection = 0;
		doubleJump = false;
		rb = GetComponent<Rigidbody2D> ();
		rb.gravityScale = 0;
		controller.onControllerCollidedEvent += onCollisionEnter2D;
		startingPos = transform.position;
		rend = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator> ();
		spriteDirection = 1;
		boxCollider = GetComponent<BoxCollider2D> ();
	}

	void Start () {		
		hpText.text = "HP: " + hp;
		winText.enabled = false;
		kickback = false;
		fallingPlatforms = GameObject.FindGameObjectsWithTag ("FallingPlatform");
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
			/*GetComponent<Collider2D>().enabled = false;
			RaycastHit2D hit = Physics2D.Raycast (new Vector2(transform.position.x, transform.position.y - 2), currentDirection, meleeDamageDistance, LayerMask.GetMask("Enemy"));
			GetComponent<Collider2D>().enabled = true;
			if (hit.collider.gameObject.CompareTag ("Enemy")) {
				EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
				Debug.Log ("enemy hit");
				if (!enemy.getIsHit ())
					enemy.Damage (this);
			}*/
			Physics2D.OverlapArea (new Vector2 (boxCollider.bounds.max.x, boxCollider.bounds.max.y), new Vector2 (boxCollider.bounds.max.x + meleeDamageDistance, boxCollider.bounds.min.y), LayerMask.NameToLayer("Enemy"));
		}

		Vector3 velocity = controller.velocity;

		if (kickback) {			
			Vector2 directionNorm = direction/direction.magnitude;
			velocity.x = directionNorm.x * runSpeed * 2;
			velocity.y = directionNorm.y * runSpeed * 2;
			controller.move (velocity * Time.deltaTime);
		}

		if (gravDirection == 0 || gravDirection == 2) {
			if (controller.isGrounded) {
				velocity.y = 0;
				doubleJump = false;
				anim.SetBool ("jump", false);
			}
			
			if (Input.GetKey (KeyCode.RightArrow)) {
				if (controller.isGrounded)
					velocity.x = runSpeed;
				else
					velocity.x = runSpeed * 0.75f;
				currentDirection = new Vector2(1, 0);
				spriteDirection = 1;
				SpriteFlip();
				anim.SetBool ("walking", true);

			} else if (Input.GetKey (KeyCode.LeftArrow)) {
				if (controller.isGrounded)
					velocity.x = -runSpeed;
				else
					velocity.x = -runSpeed * 0.75f;
				currentDirection = new Vector2(-1, 0);
				spriteDirection = -1;
				SpriteFlip();
				anim.SetBool ("walking", true);
			} else {
				velocity.x = 0;
				anim.SetBool ("walking", false);
			}

			if (gravDirection == 0) {
				if (Input.GetKey (KeyCode.DownArrow)) {
					velocity.y -= 10f;
				}
				else if (Input.GetKeyDown (KeyCode.UpArrow)) {
					if (controller.isGrounded || !doubleJump) {
						if (!controller.isGrounded)
							doubleJump = true;
						velocity.y = Mathf.Sqrt (2f * targetJumpHeight * -gravity);
						anim.SetBool ("jump", true);
					}
				}
			}

			if (gravDirection == 2) {
				if (Input.GetKey (KeyCode.UpArrow)) {
					velocity.y += 10f;
				}
				else if (Input.GetKeyDown (KeyCode.DownArrow)) {
					if (controller.isGrounded || !doubleJump) {
						if (!controller.isGrounded)
							doubleJump = true;
						velocity.y = -Mathf.Sqrt (2f * targetJumpHeight * gravity);
						anim.SetBool ("jump", true);
					}
				}
			}

			velocity.y += gravity * Time.deltaTime;

		} else {
			if (controller.isGrounded) {
				velocity.x = 0;
				doubleJump = false;
			}
			
			if (Input.GetKey (KeyCode.UpArrow)) {
				if (controller.isGrounded)
					velocity.y = runSpeed;
				else
					velocity.y = runSpeed * 0.75f;
				currentDirection = new Vector2(0, 1);
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				if (controller.isGrounded)
					velocity.y = -runSpeed;
				else
					velocity.y = -runSpeed * 0.75f;
				currentDirection = new Vector2(0, -1);
			} else {
				velocity.y = 0;
			}

			if (gravDirection == 1) {
				if (Input.GetKey (KeyCode.LeftArrow))
					velocity.x -= 10f;
				else if (Input.GetKeyDown (KeyCode.RightArrow)) {
					if (controller.isGrounded || !doubleJump) {
						if (!controller.isGrounded)
							doubleJump = true;
						velocity.x = Mathf.Sqrt (2f * targetJumpHeight * -gravity);						
					}
				}
			}

			if (gravDirection == 3) {
				if (Input.GetKey (KeyCode.RightArrow))
					velocity.x += 10f;
				else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					if (controller.isGrounded || !doubleJump) {
						if (!controller.isGrounded)
							doubleJump = true;
						velocity.x = -Mathf.Sqrt (2f * targetJumpHeight * gravity);
					}
				}
			}	
			velocity.x += gravity * Time.deltaTime;
		}

		controller.move (velocity * Time.deltaTime);

	}

	void FixedUpdate () {
		if (controller.velocity.y < -float.Epsilon && gravDirection == 0) {
			controller.velocity.y -= 2f;
		} else if (controller.velocity.y > float.Epsilon && gravDirection == 2) {
			controller.velocity.y += 2f;
		} else if (controller.velocity.x < -float.Epsilon && gravDirection == 1) {
			controller.velocity.x -= 2f;
		} else if (controller.velocity.x > float.Epsilon && gravDirection == 3) {
			controller.velocity.x += 2f;
		}
		if (!controller.isGrounded)
			rb.velocity = Vector2.zero;
	}

	public void Damage (EnemyController enemy) {
		Debug.Log ("player damaged");
		hp -= enemy.damage;
		hpText.text = "HP: " + hp;
		isHit = true;
		kickback = true;
		direction = transform.position - enemy.transform.position;
		StartCoroutine (DamageCoRoutine(enemy));
	}

	IEnumerator DamageCoRoutine (EnemyController enemy) {
		yield return null;
		if (gravDirection == 0 || gravDirection == 2)
			controller.velocity = new Vector3 (enemy.speed, 0, 0);
		else
			controller.velocity = new Vector3 (0, enemy.speed, 0);
		float moveEnd = Time.time + 0.1f;
		while (Time.time <= moveEnd)
			controller.move (controller.velocity * Time.deltaTime);
		yield return new WaitForSeconds (0.1f);
		controller.velocity = Vector2.zero;
		yield return new WaitForSeconds (damageDelay);
		isHit = false;
		Debug.Log ("isHit reset");
		/*yield return null;
		yield return new WaitForSeconds (hitKickbackTime);
		kickback = false;
		yield return new WaitForSeconds (damageDelay - attackKickback);
		isHit = false;*/
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (coll.CompareTag ("GravitySwitch")) {			
			int direction1 = gravDirection;
			gravDirection = coll.gameObject.GetComponent<GravitySwitchController>().gravDirection;
			switch (gravDirection) {
				case 0 : 
				case 1 :
					if (gravity > 0) {
						gravity *= -1;
					}
					RotatePlayer(direction1, gravDirection);
					break;
				case 2 : 
				case 3 :
					if (gravity < 0) {
						gravity *= -1;
					}
					RotatePlayer(direction1, gravDirection);
					break;
			}
		}
		if (coll.CompareTag ("Exit")) {
			winText.enabled = true;
		}
	}

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
		Debug.Log ("start rotation: " + startRotation);
		Debug.Log ("end rotation: " + endRotation);
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

	void onCollisionEnter2D (RaycastHit2D hit) {
		//if (hit.rigidbody.gameObject.CompareTag ("Enemy"))
			//hit.rigidbody.gameObject.GetComponent<EnemyController> ().DamagePlayer ();
		if (hit.rigidbody.gameObject.CompareTag ("KillZone")) {
			KillPlayer();
		}
		if (hit.rigidbody.gameObject.CompareTag ("MovingPlatform")) {
			rb.velocity = hit.rigidbody.gameObject.GetComponent<MovingPlatformController>().getVelocity ();
		}
		if (hit.rigidbody.gameObject.CompareTag ("FallingPlatform")) {
			hit.rigidbody.gameObject.GetComponent<FallingPlatformController>().Fall ();
		}
	}

	public int getGravDirection () {
		return gravDirection;
	}

	public bool getIsHit () {
		return isHit;
	}

	public void KillPlayer () {
		transform.position = startingPos;
		hp = 5;
		hpText.text = "HP: " + hp;
		controller.velocity = Vector2.zero;
		rb.velocity = Vector2.zero;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ().Reset ();
		gravDirection = 0;
		if (gravity > 0)
			gravity *= -1;
		winText.enabled = false;
		isHit = false;
		ReEnableFallingPlatforms ();
	}

	void ReEnableFallingPlatforms () {
		for (int i = 0; i < fallingPlatforms.Length; ++i) {
			fallingPlatforms[i].GetComponent<FallingPlatformController>().Reset ();
		}
	}

	void SpriteFlip () {
		if ((spriteDirection > 0 && transform.localScale.x < 0) || (spriteDirection < 0 && transform.localScale.x > 0))
			transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

	void PrimeRaycastOrigins () {
		var modifiedBounds = boxCollider.bounds;
		raycastOrigins.topLeft = new Vector2 (modifiedBounds.min.x, modifiedBounds.max.y);
		raycastOrigins.bottomLeft = new Vector2 (modifiedBounds.min.x, modifiedBounds.min.y);
		raycastOrigins.topRight = new Vector2 (modifiedBounds.max.x, modifiedBounds.max.y);
		raycastOrigins.bottomRight = new Vector2 (modifiedBounds.max.x, modifiedBounds.min.y);
		raycastOrigins.topCenter = new Vector2 (modifiedBounds.center.x, modifiedBounds.max.y);
		raycastOrigins.bottomCenter = new Vector2 (modifiedBounds.center.x, modifiedBounds.min.y);
	}
}