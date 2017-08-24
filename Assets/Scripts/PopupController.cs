using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PopupController : MonoBehaviour {

	public static bool isForcedPopup = false;
	public static bool isActive
	{
		get { return instance.gameObject.active; }
		private set { instance.gameObject.active = value; }
	}
	static PopupController instance;
	public Text textRenderer;
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
			+"Designed by Park JY\n"
			+"Developed by Son MZ\n"
			+"\n"
			+"Special Thanks to\n"
			+"SNUGDC && Lee SC";
	}
	public static void SwitchPopup()
	{
		if(!isForcedPopup)
		{
			isActive = !isActive;
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
		Application.Quit();
	}
}
