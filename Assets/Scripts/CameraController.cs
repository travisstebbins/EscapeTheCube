using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public bool centered = true;
	public int scrollDistanceX = 5;
	public int scrollDistanceY = 5;

	private GameObject player;
	private Vector3 startingPos;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (centered)
			transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, transform.position.z);
		else {
			if (player.transform.position.x > transform.position.x + (this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX)) {
				transform.position = new Vector3 (player.transform.position.x - (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX), transform.position.y, transform.position.z);
			}
			else if (player.transform.position.x < transform.position.x - (this.GetComponent<Camera>().orthographicSize * (16.0/9.0)) + scrollDistanceX) {
				if ((player.transform.position.x + (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX)) > 0)
					transform.position = new Vector3 (player.transform.position.x + (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX), transform.position.y, transform.position.z);
				else
					transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			}
			if (transform.position.y <= 0 && player.transform.position.y > transform.position.y + (this.GetComponent<Camera>().orthographicSize - scrollDistanceY)) {
				transform.position = new Vector3 (transform.position.x, player.transform.position.y - (float)(this.GetComponent<Camera>().orthographicSize - scrollDistanceY), transform.position.z);
			}
			else if (player.transform.position.y < transform.position.y - (this.GetComponent<Camera>().orthographicSize - scrollDistanceY)) {
				transform.position = new Vector3 (transform.position.x, player.transform.position.y + (float)(this.GetComponent<Camera>().orthographicSize - scrollDistanceY), transform.position.z);
			}
		}
	}

	public void Reset() {
		transform.position = startingPos;
	}
}
