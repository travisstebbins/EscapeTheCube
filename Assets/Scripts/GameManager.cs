using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	// private variables
	private int checkPoint = 0;
	private Transform checkPointPosition;
	private int numOrbs = 0;
	private bool orbCollectedThisLevel = false;
	private SoundManager sm;
	private Image ending1;
	private Image ending2;
	private bool fadeEnding1;
	private bool fadeEnding2;
	private bool endGame = false;
	private OrbGUIController ogc;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (this);
	}

	void Start () {
		sm = GameObject.FindGameObjectWithTag ("SoundManager").GetComponent<SoundManager> ();
		ending1 = GameObject.FindGameObjectWithTag ("Ending1").GetComponent<Image>();
		ending2 = GameObject.FindGameObjectWithTag ("Ending2").GetComponent<Image>();
	}

	void Update () {
		if (fadeEnding1) {
			ending1.color = new Color (ending1.color.r, ending1.color.g, ending1.color.b, ending1.color.a + (2f * Time.deltaTime));
			if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume > 0)
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume -= 0.5f;
		}
		if (fadeEnding2) {
			ending2.color = new Color (ending2.color.r, ending2.color.g, ending2.color.b, ending2.color.a + (2f * Time.deltaTime));
			if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume > 0)
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume -= 0.5f;
		}
	}

	void OnLevelWasLoaded (int level) {
		if (level == 0)
			resetCheckPoint ();
		sm = GameObject.FindGameObjectWithTag ("SoundManager").GetComponent<SoundManager> ();
		ending1 = GameObject.FindGameObjectWithTag ("Ending1").GetComponent<Image>();
		ending2 = GameObject.FindGameObjectWithTag ("Ending2").GetComponent<Image>();
		ogc = GameObject.FindGameObjectWithTag ("OrbGUI").GetComponent<OrbGUIController> ();
		if (ogc != null)
			Debug.Log ("has ogc");
	}

	public void resetCheckPoint () {
		checkPoint = 0;
		checkPointPosition = null;
	}

	public void setCheckPoint (GameObject c) {
		if (c.GetComponent<CheckPoint>().checkPoint >= checkPoint) {
			checkPoint = c.GetComponent<CheckPoint>().checkPoint;
			Debug.Log ("check point " + checkPoint + " set");
			checkPointPosition = c.transform;
			Debug.Log ("check point position x: " + checkPointPosition.position.x + ", check point posiiton y: " + checkPointPosition.position.y);
		}
	}

	public void EndGame () {
		if (!endGame) {
			endGame = true;
			Debug.Log ("EndGame called");
			if (numOrbs == 0 || numOrbs == 1) {
				StartCoroutine (EndGameCoroutine (0));
			} else if (numOrbs == 2) {
				StartCoroutine (EndGameCoroutine (1));
			} else if (numOrbs == 3) {
				StartCoroutine (EndGameCoroutine (2));
			}
		}
	}

	IEnumerator EndGameCoroutine (int endCase) {
		yield return null;
		if (endCase == 0) {
			Debug.Log ("EndGame Case 0");
			if (sm == null)
				Debug.Log ("SoundManager null");
			else {
				Debug.Log ("SoundManager present");
				sm.PlaySound (3);
			}
			yield return new WaitForSeconds (3f);
			Application.LoadLevel ("MainMenu");
		} else if (endCase == 1) {
			Debug.Log ("EndGame Case 1");
			sm.PlaySound (5);
			yield return new WaitForSeconds (7f);
			sm.PlaySound (4);
			if (ending1 == null)
				Debug.Log ("ending1 null");
			fadeEnding1 = true;
			yield return new WaitForSeconds (10f);
			Application.LoadLevel ("MainMenu");					
		} else if (endCase == 2) {
			Debug.Log ("EndGame Case 2");
			sm.PlaySound (6);
			yield return new WaitForSeconds (7f);
			sm.PlaySound (4);
			if (ending1 == null)
				Debug.Log ("ending1 null");
			fadeEnding1 = true;
			yield return new WaitForSeconds (5f);
			if (ending2 == null)
				Debug.Log ("ending2 null");
			fadeEnding2 = true;
			yield return new WaitForSeconds (5f);
			Application.LoadLevel ("MainMenu");
		}
	}

	public int getCheckPoint () {
		return checkPoint;
	}

	public Transform getCheckPointPosition () {
		return checkPointPosition;
	}

	public void incrementNumOrbs () {
		ogc = GameObject.FindGameObjectWithTag ("OrbGUI").GetComponent<OrbGUIController> ();
		Debug.Log ("incrementNumOrbs called");
		numOrbs++;
		if (ogc != null)
			ogc.SetGUI (numOrbs);
		else
			Debug.Log ("doesn't have ogc");
		Debug.Log ("numOrbs: " + numOrbs);
	}

	public int getNumOrbs () {
		return numOrbs;
	}

	public void setOrbCollectedThisLevel (bool collected) {
		orbCollectedThisLevel = collected;
	}

	public bool getOrbCollectedThisLevel () {
		return orbCollectedThisLevel;
	}

}
