using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance = null;

	// public variables
	public AudioClip footsteps;
	public AudioClip rustling;
	public AudioClip thudFall;
	public AudioClip biteSound;
	public AudioClip orb;
	public AudioClip flatline;
	public AudioClip heartbeat;
	public AudioClip introSounds;

	// components
	private AudioSource source;

	void Awake () {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad(this);
	}
	
	void Start () {
		source = GetComponent<AudioSource> ();
	}

	public void PlaySound (int s) {
		switch (s) {
		case 0 : if (footsteps != null) source.PlayOneShot(footsteps);
			break;
		case 1 : if (rustling != null) source.PlayOneShot(rustling);
			break;
		case 2 : if (thudFall != null) source.PlayOneShot(thudFall);
			break;
		case 3 : if (biteSound != null) source.PlayOneShot(biteSound);
			break;
		case 4 : if (orb != null) source.PlayOneShot(orb);
			break;
		case 5 : if (flatline != null) source.PlayOneShot(flatline);
			break;
		case 6 : if (heartbeat != null) source.PlayOneShot(heartbeat);
			break;
		}
	}
}
