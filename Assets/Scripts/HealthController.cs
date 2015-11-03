using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

	// public variables
	public int healthAmount = 1;
	public Sprite heartSprite;
	public Sprite noHeartSprite;

	// private variables
	private SpriteRenderer rend;
	private ParticleSystem particles;

	void Start () {
		rend = GetComponent<SpriteRenderer> ();
		particles = GetComponentInChildren<ParticleSystem> ();
	}

	public void setHeartSprite () {
		rend.sprite = heartSprite;
		particles.Play ();
	}

	public void setNoHeartSprite () {
		rend.sprite = noHeartSprite;
		particles.Stop ();
	}
}
