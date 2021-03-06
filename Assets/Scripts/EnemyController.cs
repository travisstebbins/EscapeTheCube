﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

	// public variables
	public float speed = -5f;
	public int damage = 1;	
	public int hp = 5;
	public int hitDistance = 1;
	[Range(0,3)]
	public int gravDirection = 0;
	public GameObject appearingPlatform;

	// components
	private Rigidbody2D rb;	
	private PlayerController player;
	private SpriteRenderer rend;

	// helper variables
	private bool isHit;
	private bool kickback;
	private float damageDelay = 0.3f;
	private Vector2 gravity;
	private float damageKickbacktime = 0.1f;
	private List<RaycastHit2D> hits = new List<RaycastHit2D>();	
	private bool flashBegin;
	private bool flashComplete;
	private float flashSpeed = 0.1f;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		isHit = false;
		if (gravDirection == 0 || gravDirection == 2) {
			rb.velocity = new Vector2 (speed, 0);
			//rb.constraints = RigidbodyConstraints2D.FreezePositionY;
		} else {
			rb.velocity = new Vector2 (0, speed);
			//rb.constraints = RigidbodyConstraints2D.FreezePositionX;
		}
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		rb.gravityScale = 0;
		rend = GetComponent<SpriteRenderer> ();
	}
	
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update () {
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
	}
	
	void FixedUpdate () {
		if (!kickback) {
			if (gravDirection == 0 || gravDirection == 2)
				rb.velocity = new Vector2 (speed, rb.velocity.y);
			else
				rb.velocity = new Vector2 (rb.velocity.x, speed);
		}
		switch (gravDirection) {
			case 0:
				gravity = new Vector2 (0, -9.8f);
				break;
			case 1:
				gravity = new Vector2 (-9.8f, 0);
				break;
			case 2:
				gravity = new Vector2 (0, 9.8f);
				break;
			case 3:
				gravity = new Vector2 (9.8f, 0);
				break;
			default:
				gravity = new Vector2 (0, -9.8f);
				break;
		}
		rb.AddForce (gravity * 2);
	}

	/*void Update () {
		hits.Clear ();
		GetComponent<Collider2D>().enabled = false;
		hits.Add ( Physics2D.Raycast (transform.position, Vector2.right, hitDistance, LayerMask.GetMask ("Player")));
		hits.Add ( Physics2D.Raycast (transform.position, Vector2.left, hitDistance, LayerMask.GetMask ("Player")));
		hits.Add ( Physics2D.Raycast (transform.position, Vector2.up, hitDistance, LayerMask.GetMask ("Player")));
		hits.Add ( Physics2D.Raycast (transform.position, Vector2.down, hitDistance, LayerMask.GetMask ("Player")));
		DrawRay (transform.position, new Vector3(hitDistance,0,0), Color.red);
		DrawRay (transform.position, new Vector3(-hitDistance,0,0), Color.red);
		DrawRay (transform.position, new Vector3(0,hitDistance,0), Color.red);
		DrawRay (transform.position, new Vector3(0,-hitDistance,0), Color.red);
		GetComponent<Collider2D>().enabled = true;
		for (int i = 0; i < hits.Count; ++i) {
			if (hits[i].rigidbody.gameObject.CompareTag ("Player"))
					DamagePlayer ();
		}
	}*/

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("TurnaroundTrigger")) {
			speed *= -1;
			Flip ();
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.rigidbody.gameObject.CompareTag ("KillZone")) {
			if (appearingPlatform != null)
				appearingPlatform.SetActive (true);
			Destroy (this.gameObject);
		}
	}

	public void DamagePlayer () {
		if (!player.getIsHit ())
			player.Damage (this);
	}

	public void Damage (PlayerController player) {
		Debug.Log ("enemy damaged");
		hp -= player.damage;
		isHit = true;
		flashBegin = true;
		Vector2 heading = new Vector2 (transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y);
		Vector2 direction = heading / heading.magnitude;
		StartCoroutine (DamageCoroutine (direction, player));
		if (hp <= 0) {
			if (appearingPlatform != null)
				appearingPlatform.SetActive (true);
			Destroy (this.gameObject);
		}
	}

	IEnumerator DamageCoroutine (Vector2 direction, PlayerController player) {
		yield return null;
		Vector2 currentVelocity = rb.velocity;		
		kickback = true;
		Debug.Log ("x velocity: " + Mathf.Round (direction.x) + ", y velocity: " + Mathf.Round (direction.y));
		if (gravDirection == 0 || gravDirection == 2)
			rb.velocity = new Vector2 (direction.x > 0 ? speed > 0 ? speed * player.attackKickback : -speed * player.attackKickback : speed > 0 ? -speed * player.attackKickback : speed * player.attackKickback, 0);
		else
			rb.velocity = new Vector2 (0, direction.y > 0 ? speed > 0 ? speed * player.attackKickback : -speed * player.attackKickback : speed > 0 ? -speed * player.attackKickback : speed * player.attackKickback);
		yield return new WaitForSeconds (damageKickbacktime);
		//rb.velocity = currentVelocity;
		kickback = false;
		yield return new WaitForSeconds (damageDelay);
		isHit = false;
	}

	public bool getIsHit () {
		return isHit;
	}

	void DrawRay( Vector3 start, Vector3 dir, Color color )
	{
		Debug.DrawRay( start, dir, color );
	}

	void Flip () {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
}
