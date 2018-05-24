using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour {

	public float minTime = 0.05f;
	public float maxTime = 1.2f;
	public GameObject mesh;
	public Material[] material;

	private Light lightSource; 
	private float timer;

	Renderer rend;

	// Use this for initialization
	void Start () {

		lightSource = GetComponent<Light> ();
		timer = Random.Range (minTime, maxTime);
	}
	
	// Update is called once per frame
	void Update () {

		timer -= Time.deltaTime;
		if (timer <= 0) {
			lightSource.enabled = !lightSource.enabled;
		} 


		if (lightSource.enabled) {

			mesh.GetComponent<Renderer> ().sharedMaterial = material [1];
		} 

		else {
			mesh.GetComponent<Renderer>().sharedMaterial = material[0];
		}
	}

}
