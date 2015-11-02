using UnityEngine;
using System.Collections;

public class PulseLight : MonoBehaviour {

	private Light l;
	private int direction = 1;
	private float speed = 2f;

	void Start () {
		l = this.GetComponent<Light> ();
	}

	void Update () {
		//l.intensity += speed * direction;
		l.intensity = Mathf.Lerp (l.intensity, direction == 1 ? 8 : 2, Time.deltaTime);
		if (l.intensity >= 7)
			direction *= -1;
		else if (l.intensity <= 3)
			direction *= -1;
	}
}
