using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicPlayer : MonoBehaviour {

	AudioSource audio;
	void Awake()
	{
		audio = GetComponent<AudioSource>();
	}
	public void PlayMusic(AudioClip clip)
	{
		audio.clip = clip;
		audio.Play();
	}
	public void ChangeMusic(AudioClip clip)
	{
		audio.Stop();
		audio.clip = clip;
		audio.Play();
	}
	public void FadeOutMusic(float duration)
	{
		audio.DOFade(0,duration).SetEase(Ease.Linear);
	}
	public void FadeInMusic(float duration)
	{
		audio.DOFade(1,duration).SetEase(Ease.Linear);
	}
}
