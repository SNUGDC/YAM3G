using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
		SoundManager.usingPlayers.Remove(gameObject);
		SoundManager.soundPlayers.Enqueue(gameObject);
	}
	public void StopSound()
	{
		audio.Stop();
		StopAllCoroutines();
		SoundManager.usingPlayers.Remove(gameObject);
		SoundManager.soundPlayers.Enqueue(gameObject);
	}
	public void FadeOutSound(float duration)
	{
		StopAllCoroutines();
		StartCoroutine(FadeOut(duration));
	}
	IEnumerator FadeOut(float duration)
	{
		yield return audio.DOFade(0,duration)
			.SetEase(Ease.Linear)
			.WaitForCompletion();

		audio.Stop();
		audio.volume = 1;
		SoundManager.usingPlayers.Remove(gameObject);
		SoundManager.soundPlayers.Enqueue(gameObject);
	}
}
