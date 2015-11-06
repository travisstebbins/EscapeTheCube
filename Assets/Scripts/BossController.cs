using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour {

	public float attackDelay = 2.5f;
	public int damage = 1;
	public int hp = 6;
	public float damageDelay = 0.5f;

	private Animator anim;
	private PlayerController player;

	private bool facingRight = false;
	private bool isAttacking = false;
	private int attackCounter = 0;
	private const float attackTime = 2.66667f;
	private bool isHit = false;


	void Start () {
		anim = GetComponent<Animator> ();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
	}

	void Update () {
		if (!isAttacking) {
			isAttacking = true;
			StartCoroutine(AttackCoroutine());
		}
		if (attackCounter >= 2) {
			Flip ();
			attackCounter = 0;
		}
	}

	IEnumerator AttackCoroutine () {
		yield return null;
		anim.SetTrigger ("attack");
		attackCounter++;
		yield return new WaitForSeconds (attackTime + attackDelay);
		isAttacking = false;
	}

	void Flip () {
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	public void DamagePlayer () {
		if (!player.getIsHit ())
			player.Damage (this);
	}

	public void Damage (PlayerController player) {
		Debug.Log ("enemy damaged");
		hp -= player.damage;
		isHit = true;
		StartCoroutine (DamageCoroutine ());
		if (hp <= 0) {
			Destroy (this.gameObject);
		}
	}
	
	IEnumerator DamageCoroutine () {
		yield return null;
		yield return new WaitForSeconds (damageDelay);
		isHit = false;
	}

	public bool getIsHit () {
		return isHit;
	}

}

