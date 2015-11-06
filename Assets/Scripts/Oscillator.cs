using UnityEngine;
using System.Collections;

public class Oscillator : MonoBehaviour {

	// public variables
	public Vector2 oscillateDistance;
	public float oscillateSpeed;

	// private variables
	private Vector2 startingPos;
	private float xStep;
	private float yStep;
	private int direction = 1;

	void Start () {
		startingPos = transform.position;
	}

	void Update () {
		//if (transform.position.x <= startingPos.x + oscillateDistance.x || transform.position.y <= startingPos.y + oscillateDistance.y) {
			transform.position = new Vector2 (Mathf.Lerp (transform.position.x, startingPos.x + (direction * oscillateDistance.x), Time.deltaTime * oscillateSpeed),
			                                  Mathf.Lerp (transform.position.y, startingPos.y + (direction * oscillateDistance.y), Time.deltaTime * oscillateSpeed));
		//transform.position = new Vector2 (transform.position.x + (direction * oscillateDistance.x * Time.deltaTime * oscillateSpeed), transform.position.y + (direction * oscillateDistance.y * Time.deltaTime * oscillateSpeed));
		if (transform.position.x < startingPos.x - (0.8f * oscillateDistance.x) || transform.position.x > startingPos.x + (0.8f * oscillateDistance.x)
		    || transform.position.y < startingPos.y - (0.8f * oscillateDistance.y) || transform.position.y > startingPos.y + (0.8f * oscillateDistance.y))
			direction *= -1;
	}
}
