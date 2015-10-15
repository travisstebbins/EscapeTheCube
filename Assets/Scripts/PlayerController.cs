using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prime31;

public class PlayerController : MonoBehaviour {

	public float gravity = -30f;
	public float runSpeed = 8f;
	public float targetJumpHeight = 10f;

	public Text text;
	
	private CharacterController2D controller;
	private int hp = 5;
	private int gravDirection;
	private bool doubleJump;

	void Awake () {
		controller = GetComponent<CharacterController2D> ();
		gravDirection = 0;
		doubleJump = false;
	}

	void Start () {		
		text.text = "HP: " + hp;
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
	}

	public void Damage (int d) {
		hp -= d;
		text.text = "HP: " + hp;
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
	}

	public int getGravDirection () {
		return gravDirection;
	}
}