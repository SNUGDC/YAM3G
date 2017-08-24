using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour {

	public static bool isActive
	{
		get { return instance.gameObject.active; }
		private set { instance.gameObject.active = value; }
	}
	static PopupController instance;
	public PopupController()
	{
		instance = this;
	}

	public static void SwitchPopup()
	{
		isActive = !isActive;
	}
	public void SwitchPopupNonStatic()
	{
		isActive = !isActive;
	}
	public static void Inactivate()
	{
		isActive = false;
	}
	public static void Activate()
	{
		isActive = true;
	}
}
