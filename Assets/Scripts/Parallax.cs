﻿using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	// public variables
	public float speed = 0.1f;

	// private variables
	private Vector3 startingPos;
	private Vector3 offset;
	private GameObject player;

	void Start () {
		startingPos = transform.position;
		player = GameObject.FindGameObjectWithTag ("Player");
		offset = transform.position - player.transform.position;
		//offset = transform.position - player.transform.position;
	}

	void Update () {
		//transform.position = offset;
		transform.position = new Vector3 (startingPos.x - (player.transform.position.x * speed), startingPos.y, startingPos.z);
		//transform.position = (player.transform.position * speed) + offset;
	}
}
