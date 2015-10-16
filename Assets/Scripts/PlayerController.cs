using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prime31;

public class PlayerController : MonoBehaviour {

	public float gravity = -30f;
	public float runSpeed = 8f;
	public float targetJumpHeight = 10f;
	public Text hpText;
	public Text winText;
	
	private CharacterController2D controller;
	private int hp = 5;
	private int gravDirection;
	private bool doubleJump;
	private Rigidbody2D rb;
	private bool isHit = false;
	private float damageDelay = 0.5f;
	private Vector2 startingPos;

	void Awake () {
		controller = GetComponent<CharacterController2D> ();
		gravDirection = 0;
		doubleJump = false;
		rb = GetComponent<Rigidbody2D> ();
		rb.gravityScale = 0;
		controller.onControllerCollidedEvent += onCollisionEnter2D;
		startingPos = transform.position;
	}

	void Start () {		
		hpText.text = "HP: " + hp;
		winText.enabled = false;
	}

	void Update () {
		Vector3 velocity = controller.velocity;

		if (gravDirection == 0 || gravDirection == 2) {
			if (controller.isGrounded) {
				velocity.y = 0;
				doubleJump = false;
			}
			
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
				switch (gravDirection) {
				case 0:
					velocity.y -= 10f;
					break;
				case 2:
					velocity.y += 10f;
					break;
				}
			}
			
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				switch (gravDirection) {
				case 0: 
					if (controller.isGrounded || !doubleJump) {
						if (!controller.isGrounded)
							doubleJump = true;
						velocity.y = Mathf.Sqrt (2f * targetJumpHeight * -gravity);						
					}
					break;
				case 2: 
					if (controller.isGrounded || !doubleJump) {
						if (!controller.isGrounded)
							doubleJump = true;
						velocity.y = -Mathf.Sqrt (2f * targetJumpHeight * gravity);
					}
					break;
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
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				if (controller.isGrounded)
					velocity.y = -runSpeed;
				else
					velocity.y = -runSpeed * 0.75f;
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
		hp -= enemy.damage;
		hpText.text = "HP: " + hp;
		isHit = true;
		StartCoroutine (DamageCoRoutine());
	}

	IEnumerator DamageCoRoutine () {
		yield return null;
		yield return new WaitForSeconds (damageDelay);
		isHit = false;
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (coll.CompareTag ("GravitySwitch")) {
			gravDirection = coll.gameObject.GetComponent<GravitySwitchController>().gravDirection;
			switch (gravDirection) {
				case 0 :
				case 1 :
					if (gravity > 0)
						gravity *= -1;
					break;
				case 2 : 
				case 3 :
					if (gravity < 0)
						gravity *= -1;
					break;
			}
		}
		if (coll.CompareTag ("Exit")) {
			winText.enabled = true;
		}
	}

	void onCollisionEnter2D (RaycastHit2D hit) {
		if (hit.rigidbody.gameObject.CompareTag ("Enemy"))
			hit.rigidbody.gameObject.GetComponent<EnemyController> ().DamagePlayer ();
		if (hit.rigidbody.gameObject.CompareTag ("KillZone")) {
			KillPlayer();
		}
		if (hit.rigidbody.gameObject.CompareTag ("MovingPlatform")) {
			rb.velocity = hit.rigidbody.gameObject.GetComponent<MovingPlatformController>().getVelocity ();
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
	}
}