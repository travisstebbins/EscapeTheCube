using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public float speed = -5f;
	public int damage = 1;	
	public int hp = 5;
	public int hitDistance = 5;
	[Range(0,3)]
	public int gravDirection = 0;

	private Rigidbody2D rb;
	private bool isHit;
	private float damageDelay = 0.3f;
	private Vector2 gravity;
	private PlayerController player;
	private float attackKickbackTime = 0.1f;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		isHit = false;
		if (gravDirection == 0 || gravDirection == 2)
			rb.velocity = new Vector2 (speed, 0);
		else
			rb.velocity = new Vector2 (0, speed);
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
	}
	
	void FixedUpdate () {
		if (gravDirection != 0) {
			rb.gravityScale = 0;
			switch (gravDirection) {
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
			rb.AddForce (gravity);
		}
		if (!isHit) {
			if (gravDirection == 0 || gravDirection == 2)
				rb.velocity = new Vector2 (speed, 0);
			else
				rb.velocity = new Vector2 (0, speed);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("TurnaroundTrigger")) {
			speed *= -1;
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.rigidbody.gameObject.CompareTag ("KillZone"))
			Destroy (this.gameObject);
	}

	public void DamagePlayer () {
		PlayerController player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		if (!player.getIsHit ())
			player.Damage (this);
	}

	public void Damage (PlayerController player) {
		hp -= player.damage;
		isHit = true;
		Vector2 heading = new Vector2 (transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y);
		Vector2 direction = heading / heading.magnitude;
		StartCoroutine (DamageCoroutine (direction));
		if (hp <= 0)
			Destroy (this.gameObject);
	}

	IEnumerator DamageCoroutine (Vector2 direction) {
		yield return null;
		Vector2 currentVelocity = rb.velocity;
		if (gravDirection == 0 || gravDirection == 2)
			rb.AddForce (new Vector2 (player.attackKickback * direction.x, 0));
		else
			rb.AddForce (new Vector2 (0, player.attackKickback * direction.y));
		yield return new WaitForSeconds (attackKickbackTime);
		rb.velocity = currentVelocity;
		yield return new WaitForSeconds (damageDelay);
		isHit = false;
	}

	public bool getIsHit () {
		return isHit;
	}

}
