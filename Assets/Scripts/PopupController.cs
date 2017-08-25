using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class PopupController : MonoBehaviour {

	public static bool isForcedPopup = false;
	public static bool isActive
	{
		get { return instance.gameObject.active; }
		private set { instance.gameObject.active = value; }
	}
	static PopupController instance;

	public Image background;
	public Button buttonRetry, buttonExit;
	public Text textRenderer, textRetry, textExit;
	public PopupController()
	{
		instance = this;
	}
	void Awake()
	{
		instance.textRenderer = textRenderer;
	}
	public static void Initiate()
	{
		InitText();
		isForcedPopup = false;
		isActive = false;
	}
	static void InitText()
	{
		instance.textRenderer.text = 
			"YAM3G\n"
			+"~Yet Another Match-3 Game~\n"
			+"\n"
			+"Design : "
			+"Park JaeYeong\n"
			+"Develop : "
			+"Son MyeongZin\n"
			+"\n"
			+"Special Thanks to\n"
			+"SNUGDC\n"
			+"&& Lee SeongChan";
	}
	public static void SwitchPopup()
	{
		if(!isForcedPopup)
		{
			isActive = !isActive;
			SoundManager.PlaySound(SoundType.Click);
		}
	}
	public void SwitchPopupNonStatic()
	{
		if(!isForcedPopup)
		{
			isActive = !isActive;
		}
	}
	public static void ForcedPopup(string text)
	{
		instance.textRenderer.text = text;
		isActive = true;
		isForcedPopup = true;
	}
	public void ExitGame()
	{
		SaveLoad.Save();
		Application.Quit();
	}
	public static void FinaleAction(float duration)
	{
		Debug.Log("StartFinale");
		Color[] colors = new Color[6];
		instance.buttonRetry.interactable = false;
		instance.buttonExit.interactable = false;

		var c1 = instance.background.color;
		instance.background.color = new Color(c1.r,c1.g,c1.b,0);
		var c2 = instance.buttonRetry.image.color;
		instance.buttonRetry.image.color = new Color(c2.r,c2.g,c2.b,0);
		var c3 = instance.textRetry.color;
		instance.textRetry.color = new Color(c3.r,c3.g,c3.b,0);
		var c4 = instance.buttonExit.image.color;
		instance.buttonExit.image.color = new Color(c4.r,c4.g,c4.b,0);
		var c5 = instance.textExit.color;
		instance.textExit.color = new Color(c5.r,c5.g,c5.b,0);
		var c6 = instance.textRenderer.color;
		instance.textRenderer.color = new Color(c6.r,c6.g,c6.b,0);

		DOTween.Sequence()
			.Join(instance.background.DOFade(0.75f,duration).SetEase(Ease.Linear))
			.Join(instance.buttonRetry.image.DOFade(1,duration).SetEase(Ease.Linear))
			.Join(instance.textRetry.DOFade(1,duration).SetEase(Ease.Linear))
			.Join(instance.buttonExit.image.DOFade(1,duration).SetEase(Ease.Linear))
			.Join(instance.textExit.DOFade(1,duration).SetEase(Ease.Linear))
			.Join(instance.textRenderer.DOFade(1,duration).SetEase(Ease.Linear))
			.OnComplete(()=>{
				instance.buttonRetry.interactable = true;
				instance.buttonExit.interactable = true;
			})
			;
	}
}
