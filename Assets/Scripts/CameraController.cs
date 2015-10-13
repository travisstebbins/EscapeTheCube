using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public bool centered = true;
	public int scrollDistance = 5;

	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (centered)
			transform.position = new Vector3 (player.transform.position.x, transform.position.y, transform.position.z);
		else {
			if (player.transform.position.x > transform.position.x + (this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistance)) {
				transform.position = new Vector3 (player.transform.position.x - (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistance), transform.position.y, transform.position.z);
			}
			else if (player.transform.position.x < transform.position.x - (this.GetComponent<Camera>().orthographicSize * (16.0/9.0)) + scrollDistance) {
				transform.position = new Vector3 (player.transform.position.x + (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistance), transform.position.y, transform.position.z);
			}
		}
	}
}
