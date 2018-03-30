using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour {

	public Image noteImage;
	public AudioClip pickupSound;
	public AudioClip putAwaySound;
	public AudioSource SoundSource;
	public GameObject ToBeDestroyed;
	public Text paragraph;
	private bool hasPlayedAudio;
	private float counter = 3.0f;

	void Update()
	{
		if (noteImage.enabled == true) {
			counter -= Time.deltaTime;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.CompareTag ("Player") && hasPlayedAudio == false && vp_Input.GetButton ("Interact")) {
			SoundSource.PlayOneShot (pickupSound);
			noteImage.enabled = true;
			paragraph.enabled = true;
			hasPlayedAudio = true;
		}
		if (hasPlayedAudio == true && vp_Input.GetButton("Interact") && counter < 0)
		{
			noteImage.enabled = false;
			paragraph.enabled = false;
			Destroy (ToBeDestroyed);
		}
		
	}

	void Start () 
	{
		noteImage.enabled = false;
		paragraph.enabled = false;
	}
}
