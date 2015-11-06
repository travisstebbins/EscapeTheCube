using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour {

	public float attackDelay = 2f;
	public int damage = 1;
	public int hp = 6;
	public float damageDelay = 0.5f;
	public GameObject appearingPlatform;

	private Animator anim;
	private SpriteRenderer rend;
	private PlayerController player;

	private bool facingRight = false;
	private bool isAttacking = false;
	private int attackCounter = 0;
	private const float attackTime = 2.66667f;
	private bool isHit = false;
	private bool flashBegin;
	private bool flashComplete;
	private float flashSpeed = 0.1f;


	void Start () {
		anim = GetComponent<Animator> ();
		rend = GetComponent<SpriteRenderer> ();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
	}

	void Update () {
		if (!isAttacking) {
			isAttacking = true;
			StartCoroutine(AttackCoroutine());
		}
		if (attackCounter >= 1) {
			Flip ();
			attackCounter = 0;
		}
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
		flashBegin = true;
		StartCoroutine (DamageCoroutine ());
		if (hp <= 0) {
			if (appearingPlatform != null)
				appearingPlatform.SetActive (true);
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

