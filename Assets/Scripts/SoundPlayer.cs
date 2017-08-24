using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

	AudioSource audio;
	void Awake()
	{
		audio = GetComponent<AudioSource>();
	}
	public void PlaySound(AudioClip clip)
	{
		audio.clip = clip;
		audio.Play();
		StartCoroutine(WaitAndEnqueue(clip.length));
	}
	IEnumerator WaitAndEnqueue(float time)
	{
		yield return new WaitForSeconds(time);
		SoundManager.soundPlayers.Enqueue(gameObject);
	}
}
