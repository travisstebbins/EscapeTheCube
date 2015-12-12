using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
	public float rotateSpeed = 7f;
	public LayerMask groundLayerMask;
	public Light playerLight;
	public Light playerGlowLight;
	public float lightPulseSpeed = 10f;
	public GameObject gameOverScreen;
	public LayerMask enemyLayerMask;
	public AudioClip heartBeat;
	public float fadeSpeed = 2f;

	// components
	private Rigidbody2D rb;
	private Animator anim;
	private BoxCollider2D bColl;
	private GameManager gm;
	private AudioSource audioSource;
	private Image black;
	private SpriteRenderer rend;

	// helper variables
	private bool facingRight = true;
	private bool isGrounded = false;
	private bool doubleJump = false;
	private bool onMovingPlatform = false;
	private bool isHit;
	private bool kickback;
	private int gravDirection;
	private bool rotating = false;
	private Quaternion newRotation;
	private GameObject[] fallingPlatforms;
	private int lightPulseDirection = 1;
	private bool isDead = false;
	private bool fadeToBlack = false;
	private bool flashBegin;
	private bool flashComplete;
	private float flashSpeed = 0.1f;
	private float gravitySwitchDelay = 0.05f;

	// unity functions	
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		bColl = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();
		Physics2D.gravity = new Vector2 (0, -gravMagnitude);
		fallingPlatforms = GameObject.FindGameObjectsWithTag ("FallingPlatform");
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		if (gm != null)
			Debug.Log ("has game manager");
		if (gm.getCheckPointPosition() != null)
			Debug.Log ("spawn check poing position x: " + gm.getCheckPointPosition ().position.x + ", spawn check point position y: " + gm.getCheckPointPosition ().position.y);
		if (gm.getCheckPointPosition() != null)
			transform.position = gm.getCheckPointPosition ().position;
		if (Application.loadedLevelName == "TutorialLevel")
			anim.SetBool ("hasSword", false);
		else
			anim.SetBool ("hasSword", true);
		black = GameObject.FindGameObjectWithTag ("Black").GetComponent<Image> ();
		audioSource = GetComponent<AudioSource> ();
		anim.SetBool ("dead", false);
	}

	void Awake () {
		rend = GetComponent<SpriteRenderer> ();
	}

	void FixedUpdate () {
		if (!isDead && !fadeToBlack) {
			// check if character is grounded
			isGrounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, groundLayerMask);
			if (isGrounded) {
				doubleJump = false;
			}
			anim.SetBool ("isGrounded", isGrounded);
			if (gravDirection == 0 || gravDirection == 2) {
				float move = Input.GetAxis ("Horizontal");
				anim.SetFloat ("speed", Mathf.Abs (move));
				if (!kickback)
					rb.velocity = new Vector2 (isGrounded ? move * maxSpeed : move * maxSpeed * 0.8f, rb.velocity.y);

				if (gravDirection == 0) {
					anim.SetFloat ("vSpeed", rb.velocity.y);
					if (Input.GetKey (KeyCode.DownArrow)) {
						if (!isGrounded) {
							rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y - groundPoundSpeed);
						}
					}
					if (move > 0 && !facingRight)
						Flip ();
					else if (move < 0 && facingRight)
						Flip ();
				} else if (gravDirection == 2) {
					anim.SetFloat ("vSpeed", -rb.velocity.y);
					if (Input.GetKey (KeyCode.UpArrow))
						if (!isGrounded) {
							rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y + groundPoundSpeed);
						}
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
				Vector2 movingPlatformVelocity = Physics2D.OverlapCircle (groundCheck.position, groundRadius, LayerMask.GetMask ("MovingPlatform")).GetComponent<MovingPlatformController> ().getVelocity ();
				rb.velocity = new Vector2 (movingPlatformVelocity.x + rb.velocity.x, movingPlatformVelocity.y + rb.velocity.y);
			}
		}
	}

	void Update () {
		if (fadeToBlack) {
			black.color = new Color (black.color.r, black.color.g, black.color.b, black.color.a + (fadeSpeed * Time.deltaTime));
			if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume > 0)
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume -= 0.5f;
		}
		if (!isDead && !fadeToBlack) {
			if (flashBegin) {
				rend.color = new Color(rend.color.r, rend.color.g - flashSpeed, rend.color.b - flashSpeed);
				if (rend.color.g <= 0 || rend.color.b <= 0) {
					flashBegin = false;
					flashComplete = true;
				}
			}
			if (flashComplete) {
				rend.color = new Color(rend.color.r, rend.color.g + flashSpeed, rend.color.b + flashSpeed);
				if (rend.color.g >= 1 || rend.color.b >= 1) {
					rend.color = new Color(1,1,1);
					flashComplete = false;
				}
			}
			if (hp == 1) {
				playerLight.intensity = Mathf.Lerp (playerLight.intensity, lightPulseDirection == 1 ? 8 : 2, lightPulseSpeed * Time.deltaTime);
				if (playerLight.intensity >= 7)
					lightPulseDirection *= -1;
				else if (playerLight.intensity <= 3)
					lightPulseDirection *= -1;
			}
			if (rotating)
				Rotation ();
			if (Input.GetKeyDown (KeyCode.Space)) {
				anim.SetTrigger ("attack");
				Vector2 corner1 = Vector2.zero;
				Vector2 corner2 = Vector2.zero;
				if (gravDirection == 0 || gravDirection == 2) {
					if (facingRight) {
						corner1 = new Vector2 (bColl.bounds.max.x, bColl.bounds.max.y);
						corner2 = new Vector2 (bColl.bounds.max.x + meleeAttackDistance, bColl.bounds.min.y);
					} else {
						corner1 = new Vector2 (bColl.bounds.min.x, bColl.bounds.max.y);
						corner2 = new Vector2 (bColl.bounds.min.x - meleeAttackDistance, bColl.bounds.min.y);
					}
				} else {
					if (gravDirection == 1) {
						if (facingRight) {
							corner1 = new Vector2 (bColl.bounds.min.x, bColl.bounds.min.y);
							corner2 = new Vector2 (bColl.bounds.max.x, bColl.bounds.min.y - meleeAttackDistance);
						} else {
							corner1 = new Vector2 (bColl.bounds.min.x, bColl.bounds.max.y);
							corner2 = new Vector2 (bColl.bounds.max.x, bColl.bounds.max.y + meleeAttackDistance);
						}
					}
					else if (gravDirection == 3) {
						if (facingRight) {
							corner1 = new Vector2 (bColl.bounds.min.x, bColl.bounds.max.y);
							corner2 = new Vector2 (bColl.bounds.max.x, bColl.bounds.max.y + meleeAttackDistance);
						} else {
							corner1 = new Vector2 (bColl.bounds.min.x, bColl.bounds.min.y);
							corner2 = new Vector2 (bColl.bounds.max.x, bColl.bounds.min.y - meleeAttackDistance);
						}
					}
				}
				Collider2D coll = Physics2D.OverlapArea (corner1, corner2, enemyLayerMask);
				if (coll.gameObject != null) {
					if (coll.gameObject.CompareTag("Enemy")) {
						if (!coll.gameObject.GetComponent<EnemyController> ().getIsHit ())
							coll.gameObject.GetComponent<EnemyController> ().Damage (this);
					}
					if (coll.gameObject.CompareTag("Boss")) {
					    if (!coll.gameObject.GetComponent<Level1BossController>().getIsHit ())
							coll.gameObject.GetComponent<Level1BossController>().Damage (this);
					}
				}
			}
			if (gravDirection == 0) {

				if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.UpArrow)) {
					anim.SetBool ("isGrounded", false);
					if (!isGrounded && !doubleJump) {
						doubleJump = true;
						anim.SetTrigger ("jump");
					}
					rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));
				}
				if (Input.GetKey (KeyCode.DownArrow)) {
					if (isGrounded) {
						anim.SetBool ("crouch", true);
					}
				}
				if (Input.GetKeyUp (KeyCode.DownArrow)) {
					if (isGrounded) {
						anim.SetBool ("crouch", false);
					}
				}
			} else if (gravDirection == 1) {
				if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.RightArrow)) {
					anim.SetBool ("isGrounded", false);
					if (!isGrounded && !doubleJump) {
						doubleJump = true;
						anim.SetTrigger ("jump");
					}
					rb.velocity = new Vector2 (Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.x), rb.velocity.y);
				}
				if (Input.GetKey (KeyCode.LeftArrow)) {
					if (isGrounded) {
						anim.SetBool ("crouch", true);
					}
				}
				if (Input.GetKeyUp (KeyCode.LeftArrow)) {
					if (isGrounded) {
						anim.SetBool ("crouch", false);
					}
				}
			} else if (gravDirection == 2) {
				if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.DownArrow)) {
					anim.SetBool ("isGrounded", false);
					if (!isGrounded && !doubleJump) {
						doubleJump = true;
						anim.SetTrigger ("jump");
					}
					rb.velocity = new Vector2 (rb.velocity.x, -Mathf.Sqrt (2f * jumpHeight * Physics2D.gravity.y));
				}
				if (Input.GetKey (KeyCode.UpArrow)) {
					if (isGrounded) {
						anim.SetBool ("crouch", true);
					}
				}
				if (Input.GetKeyUp (KeyCode.UpArrow)) {
					if (isGrounded) {
						anim.SetBool ("crouch", false);
					}
				}
			} else if (gravDirection == 3) {
				if ((isGrounded || !doubleJump) && Input.GetKeyDown (KeyCode.LeftArrow)) {
					anim.SetBool ("isGrounded", false);
					if (!isGrounded && !doubleJump) {
						doubleJump = true;
						anim.SetTrigger ("jump");
					}
					rb.velocity = new Vector2 (-Mathf.Sqrt (2f * jumpHeight * Physics2D.gravity.x), rb.velocity.y);
				}
				if (Input.GetKey (KeyCode.RightArrow)) {
					if (isGrounded) {
						anim.SetBool ("crouch", true);
					}
				}
				if (Input.GetKeyUp (KeyCode.RightArrow)) {
					if (isGrounded) {
						anim.SetBool ("crouch", false);
					}
				}
			}
		}
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (coll.CompareTag ("GravitySwitch")) {
			//int direction1 = gravDirection;
			gravDirection = coll.gameObject.GetComponent<GravitySwitchController> ().gravDirection;
			/*if (Mathf.Abs (direction1 - gravDirection) == 1 || Mathf.Abs (direction1 - gravDirection) == 3) {
				Debug.Log ("gravity assist");
				rb.velocity = new Vector2 (rb.velocity.x + (Physics2D.gravity.x * 1000), rb.velocity.y + (Physics2D.gravity.y * 1000));
			}*/
			StartCoroutine (GravitySwitchCoroutine (gravDirection));
		} else if (coll.CompareTag ("MemoryOrb")) {
			Destroy (coll.gameObject);
			hp = 5;
			playerLight.range = 50;
			playerLight.transform.position = new Vector3 (playerLight.transform.position.x, playerLight.transform.position.y, -30);
			playerLight.color = new Color (1f, 1f, 1f);
			playerGlowLight.intensity = 6;
			gm.setOrbCollectedThisLevel(true);
			gm.incrementNumOrbs();
		} else if (coll.CompareTag ("Health")) {
			if (coll.gameObject.GetComponent<HealthController> ().getHasHealth ()) {
				if (hp < 5) {
					AddHealth (coll.gameObject.GetComponent<HealthController> ().healthAmount);
					coll.gameObject.GetComponent<HealthController> ().setNoHeartSprite ();
				}
			}
		} else if (coll.CompareTag ("CheckPoint")) {
			Debug.Log ("check point " + coll.gameObject.GetComponent<CheckPoint> ().checkPoint + "triggered");
			gm.setCheckPoint (coll.gameObject);
		} else if (coll.CompareTag ("Exit")) {
			fadeToBlack = true;
			gm.resetCheckPoint();
			gm.setOrbCollectedThisLevel(false);
			StartCoroutine (ExitCoroutine());
		} else if (coll.CompareTag ("Sword")) {
			anim.SetBool ("hasSword", true);
			Destroy (coll.gameObject);
		}
	}

	IEnumerator GravitySwitchCoroutine (int gravDirection) {
		yield return null;
		yield return new WaitForSeconds (gravitySwitchDelay);
		switch (gravDirection) {
			case 0:
				Physics2D.gravity = new Vector2 (0, -gravMagnitude);
				RotatePlayer (gravDirection);
				break;
			case 1:
				Physics2D.gravity = new Vector2 (-gravMagnitude, 0);
				RotatePlayer (gravDirection);
				break;
			case 2:
				Physics2D.gravity = new Vector2 (0, gravMagnitude);
				RotatePlayer (gravDirection);
				break;
			case 3:
				Physics2D.gravity = new Vector2 (gravMagnitude, 0);
				RotatePlayer (gravDirection);
				break;
		}
	}

	IEnumerator ExitCoroutine () {
		yield return null;
		if (Application.loadedLevel == 3)
			gm.EndGame ();
		else {
			audioSource.PlayOneShot (heartBeat);
			yield return new WaitForSeconds (3f);
			Application.LoadLevel (Application.loadedLevel + 1);
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll != null) {
			if (coll.rigidbody.gameObject.CompareTag ("Enemy"))
				coll.rigidbody.gameObject.GetComponent<EnemyController> ().DamagePlayer ();
			if (coll.rigidbody.gameObject.CompareTag ("Boss"))
				coll.rigidbody.gameObject.GetComponent<Level1BossController> ().DamagePlayer ();
			if (coll.gameObject.CompareTag ("KillZone")) {
				KillPlayer ();
			}
			if (coll.rigidbody.gameObject.CompareTag ("FallingPlatform")) {
				coll.rigidbody.gameObject.GetComponent<FallingPlatformController> ().Fall ();
			}
		}
	}

	// my functions

	void RotatePlayer (int direction) {
		int endRotation;
		switch (direction) {
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
		newRotation = Quaternion.Euler (new Vector3 (0, 0, endRotation));
		rotating = true;
	}

	void Rotation () {
		if (Mathf.Abs (transform.rotation.eulerAngles.z - newRotation.eulerAngles.z) < 10 * float.Epsilon) {
			transform.rotation = newRotation;
			rotating = false;
		}
		else
			transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
	}

	void Flip () {
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void AddHealth (int healthAmount) {
		hp += healthAmount;
		playerLight.range += 8;
		playerLight.transform.position = new Vector3 (playerLight.transform.position.x, playerLight.transform.position.y, playerLight.transform.position.z - 5);
		playerLight.color = new Color (playerLight.color.r, playerLight.color.g + 0.1f, playerLight.color.b + 0.1f);
		playerGlowLight.intensity += 1;
	}

	public void Damage (EnemyController enemy) {
		anim.SetTrigger ("hit");
		hp -= enemy.damage;
		playerLight.range -= 8;
		playerLight.transform.position = new Vector3 (playerLight.transform.position.x, playerLight.transform.position.y, playerLight.transform.position.z + 5);
		playerLight.color = new Color (playerLight.color.r, playerLight.color.g - 0.1f, playerLight.color.b - 0.1f);
		playerGlowLight.intensity -= 1;
		flashBegin = true;
		isHit = true;
		kickback = true;
		Vector2 heading = transform.position - enemy.transform.position;
		Vector2 direction = heading / heading.magnitude;
		StartCoroutine (DamageCoRoutine(direction, enemy));
		if (hp <= 0)
			KillPlayer ();
	}

	public void Damage (Level1BossController boss) {
		anim.SetTrigger ("hit");
		hp -= boss.damage;
		playerLight.range -= 8;
		playerLight.transform.position = new Vector3 (playerLight.transform.position.x, playerLight.transform.position.y, playerLight.transform.position.z + 5);
		playerLight.color = new Color (playerLight.color.r, playerLight.color.g - 0.1f, playerLight.color.b - 0.1f);
		playerGlowLight.intensity -= 1;
		flashBegin = true;
		isHit = true;
		kickback = true;
		Vector2 heading = transform.position - boss.transform.position;
		Vector2 direction = heading / heading.magnitude;
		StartCoroutine (DamageCoRoutine(direction, boss));
		if (hp <= 0)
			KillPlayer ();
	}
	
	IEnumerator DamageCoRoutine (Vector2 direction, EnemyController enemy) {
		yield return null;
		if (gravDirection == 0 || gravDirection == 2)
			rb.velocity = new Vector2 (-direction.x * (enemy.GetComponent<Rigidbody2D> ().velocity.x < 0 ? enemy.speed : -enemy.speed) * 10, isGrounded ? 0 : gravDirection == 0 ? direction.y * enemyHitVerticalVelocity : -direction.y * enemyHitVerticalVelocity);
		else
			rb.velocity = new Vector2 (isGrounded ? 0 : gravDirection == 1 ? -direction.x * enemyHitVerticalVelocity : direction.x * enemyHitVerticalVelocity, -direction.y * (enemy.GetComponent<Rigidbody2D>().velocity.y < 0 ? enemy.speed : -enemy.speed) * 10);
		yield return new WaitForSeconds (kickbackTime);
		rb.velocity = new Vector2 (0, rb.velocity.y);
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

	IEnumerator DamageCoRoutine (Vector2 direction, Level1BossController boss) {
		yield return null;
		rb.velocity = new Vector2 (direction.x * 50, isGrounded ? 0 : direction.y * enemyHitVerticalVelocity);
		yield return new WaitForSeconds (kickbackTime);
		rb.velocity = new Vector2 (0, rb.velocity.y);
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
		/*isHit = false;
		kickback = false;
		StopAllCoroutines ();
		transform.position = startingPos;
		hp = 5;
		hpText.text = "HP: " + hp;
		healthBar.GetComponent<Image> ().sprite = healthBarSprites [hp];
		rb.velocity = Vector2.zero;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ().Reset ();
		gravDirection = 0;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		Physics2D.gravity = new Vector2 (0, -gravMagnitude);
		ReEnableFallingPlatforms ();*/
		if (!isDead) {
			anim.SetTrigger ("die");
			anim.SetBool ("dead", true);
		}
		isDead = true;
		StartCoroutine (KillPlayerCoroutine());
	}

	IEnumerator KillPlayerCoroutine () {
		yield return null;
		yield return new WaitForSeconds (0.25f);
		bColl.size = new Vector2 (6.19f, 1.9f);
		bColl.offset = new Vector2 (-0.5f, -0.3f);
		yield return new WaitForSeconds (1f);
		Time.timeScale = 0;
		gameOverScreen.SetActive (true);
	}

	void ReEnableFallingPlatforms () {
		for (int i = 0; i < fallingPlatforms.Length; ++i) {
			fallingPlatforms[i].GetComponent<FallingPlatformController>().Reset ();
		}
	}
}