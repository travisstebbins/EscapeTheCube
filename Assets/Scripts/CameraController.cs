using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public bool centered = true;
	public bool automateScrollDistances = true;
	public float scrollDistanceX = 5f;
	public float scrollDistanceY = 5f;

	private GameObject player;
	private Vector3 startingPos;

	void Awake () {
		Time.timeScale = 1;
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		startingPos = transform.position;
		if (automateScrollDistances) {
			scrollDistanceX = (0.9f) * (16.0f/9.0f) * this.GetComponent<Camera>().orthographicSize;
			scrollDistanceY = (0.9f) * this.GetComponent<Camera>().orthographicSize;
		}
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
				if (Application.loadedLevelName == "Level1") {
					if ((player.transform.position.x + (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX)) > 0)
						transform.position = new Vector3 (player.transform.position.x + (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX), transform.position.y, transform.position.z);
					else
						transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
				}
				else if (Application.loadedLevelName == "Level2") {
					if ((player.transform.position.x + (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX)) > -56.5f)
						transform.position = new Vector3 (player.transform.position.x + (float)(this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX), transform.position.y, transform.position.z);
					else
						transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
				}
			}
			if (player.transform.position.y > transform.position.y + (this.GetComponent<Camera>().orthographicSize - scrollDistanceY)) {
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
