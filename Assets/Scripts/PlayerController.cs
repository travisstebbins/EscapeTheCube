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

	void Awake () {
		controller = GetComponent<CharacterController2D> ();
	}

	void Start () {		
		text.text = "HP: " + hp;
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
	}

	void FixedUpdate () {
		if (controller.velocity.y < -float.Epsilon) {
			controller.velocity.y -= 2f;
		}
	}

	public void Damage (int d) {
		hp -= d;
		text.text = "HP: " + hp;
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (coll.CompareTag ("GravitySwitch")) {
			Debug.Log ("gravity switch enter");
			gravity *= -1;
			targetJumpHeight *= -1;
		}
	}

	void CnTriggerExit2D (Collider2D coll) {
		if (coll.CompareTag ("GravitySwitch")) {
			Debug.Log ("gravity switch exit");
			gravity *= -1;
		}
	}
}