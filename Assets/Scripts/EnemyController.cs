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
	private float damageDelay = 0.5f;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		isHit = false;
	}
	
	void FixedUpdate () {
		if (!isHit && (gravDirection == 0 || gravDirection == 2))
			rb.velocity = new Vector2 (speed, rb.velocity.y);
		else
			rb.velocity = new Vector2 (rb.velocity.x, speed);
		if (gravDirection != 0) {
			rb.gravityScale = 0;
			rb.AddForce (
				gravDirection == 1 ? new Vector2 (-9.8f, 0) :
				gravDirection == 2 ? new Vector2 (0, 9.8f) :
				gravDirection == 3 ? new Vector2 (9.8f, 0) :
				new Vector2 (0, -9.8f));
		}
	}

	void Update () {
		GetComponent<Collider2D> ().enabled = false;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, new Vector2 (speed, 0), hitDistance, LayerMask.GetMask ("Player"));
		GetComponent<Collider2D> ().enabled = true;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("TurnaroundTrigger")) {
			speed *= -1;
		}
	}

	public void DamagePlayer () {
		PlayerController player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		if (!player.getIsHit ())
			player.Damage (this);
	}

	public void Damage (PlayerController player) {
		Debug.Log ("enemy damaged");
		hp -= player.damage;
		isHit = true;
		Vector2 heading = new Vector2 (transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y);
		Vector2 direction = heading / heading.magnitude;
		StartCoroutine (DamageCoroutine (direction));
	}

	IEnumerator DamageCoroutine (Vector2 direction) {
		yield return null;
		//Vector2 currentPos = transform.position;
		//while ((currentPos - finalPos).sqrMagnitude > float.Epsilon)
			rb.velocity = new Vector2(direction.x * speed, direction.y * speed);
		yield return new WaitForSeconds (damageDelay);
		isHit = false;
	}

	public bool getIsHit () {
		return isHit;
	}

}
