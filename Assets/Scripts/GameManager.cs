using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	// private variables
	private int checkPoint = 0;
	private Transform checkPointPosition;

	void Awake () {
		if (instance == null)
			instance = this;
		else
			Destroy (gameObject);
		DontDestroyOnLoad (this);
	}

	public void resetCheckPoint () {
		checkPoint = 0;
	}

	public void setCheckPoint (GameObject c) {
		if (c.GetComponent<CheckPoint>().checkPoint >= checkPoint) {
			checkPoint = c.GetComponent<CheckPoint>().checkPoint;
			Debug.Log ("check point " + checkPoint + " set");
			checkPointPosition = c.transform;
			Debug.Log ("check point position x: " + checkPointPosition.position.x + ", check point posiiton y: " + checkPointPosition.position.y);
		}
	}

	public int getCheckPoint () {
		return checkPoint;
	}

	public Transform getCheckPointPosition () {
		return checkPointPosition;
	}

	void OnLevelWasLoaded (int l) {
		checkPoint = 0;
		checkPointPosition = null;
	}

}
