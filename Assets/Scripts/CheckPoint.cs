using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

	public int checkPoint;

	public void Awake () {
		DontDestroyOnLoad (this);
	}

}
