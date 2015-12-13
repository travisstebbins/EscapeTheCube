using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;

public class OrbGUIController : MonoBehaviour {

	// public variables
	public Sprite orbGUI0;
	public Sprite orbGUI1;
	public Sprite orbGUI2;
	public Sprite orbGUI3;

	// components
	Image img;
	ParticleSystem darkParticles;
	ParticleSystem lightParticles;

	// private variables
	GameManager gm;

	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>() ;
		img = GetComponent<Image> ();
		darkParticles = GameObject.FindGameObjectWithTag ("OrbGUI_DarkParticles").GetComponent<ParticleSystem> ();
		lightParticles = GameObject.FindGameObjectWithTag ("OrbGUI_LightParticles").GetComponent<ParticleSystem> ();
		SetGUI (gm.getNumOrbs ());
	}
	
	public void SetGUI (int numOrbs) {
		Debug.Log ("SetGUI: " + numOrbs);
		SerializedObject so1 = new SerializedObject(darkParticles);
		SerializedObject so2 = new SerializedObject(lightParticles);
		switch (numOrbs) {
			case 0:
				img.sprite = orbGUI0;				
				so1.FindProperty("ShapeModule.arc").floatValue = 360;
				so1.ApplyModifiedProperties();				
				so2.FindProperty("ShapeModule.arc").floatValue = 0;
				so2.ApplyModifiedProperties();
				lightParticles.gameObject.SetActive (false);
				break;
			case 1:
				img.sprite = orbGUI1;
				so1.FindProperty("ShapeModule.arc").floatValue = 240;
				so1.ApplyModifiedProperties();
				lightParticles.gameObject.SetActive (true);
				so2.FindProperty("ShapeModule.arc").floatValue = 120;
				so2.ApplyModifiedProperties();
				break;
			case 2:
				img.sprite = orbGUI2;
				so1.FindProperty("ShapeModule.arc").floatValue = 120;
				so1.ApplyModifiedProperties();
				so2.FindProperty("ShapeModule.arc").floatValue = 240;
				so2.ApplyModifiedProperties();
				break;
			case 3:
				img.sprite = orbGUI3;
				so1.FindProperty("ShapeModule.arc").floatValue = 0;
				so1.ApplyModifiedProperties();
				darkParticles.gameObject.SetActive (false);
				so2.FindProperty("ShapeModule.arc").floatValue = 360;
				so2.ApplyModifiedProperties();
				break;
			default:
				img.sprite = orbGUI0;
				break;
		}
	}
}
