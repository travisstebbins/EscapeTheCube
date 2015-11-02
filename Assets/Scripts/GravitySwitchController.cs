using UnityEngine;
using System.Collections;

public class GravitySwitchController : MonoBehaviour {

	[Range (0,3)]
	public int gravDirection = 0;
	public float particleSpeed = 2f;

	private ParticleSystem particleSys;
	private ParticleSystem.Particle[] particles;

	void Start () {
		particleSys = GetComponentInChildren<ParticleSystem> ();
		switch (gravDirection) {
		case 0 :	particleSys.startColor = new Color(0, 0, 255);
					break;
		case 1 : 	particleSys.startColor = new Color(255, 255, 0);
					break;
		case 2 : 	particleSys.startColor = new Color(255, 0, 0);
					break;
		case 3 : 	particleSys.startColor = new Color(0, 255, 0);
					break;
		}
	}

	void LateUpdate () {
		InitializeIfNeeded ();
		int numParticlesAlive = particleSys.GetParticles (particles);
		for (int i = 0; i < numParticlesAlive; ++i) {
			switch (gravDirection) {
			case 0: particles[i].velocity = new Vector3 (0, 0, -particleSpeed);
				break;
			case 1: particles[i].velocity = new Vector3 (-particleSpeed, 0, 0);
				break;
			case 2: particles[i].velocity = new Vector3 (0, 0, particleSpeed);
				break;
			case 3: particles[i].velocity = new Vector3 (particleSpeed, 0, 0);
				break;
			}
		}
		particleSys.SetParticles (particles, numParticlesAlive);
	}

	void InitializeIfNeeded () {
		if (particleSys == null)
			particleSys = GetComponentInChildren<ParticleSystem> ();
		if (particles == null || particles.Length < particleSys.maxParticles)
			particles = new ParticleSystem.Particle[particleSys.maxParticles];
	}

}
