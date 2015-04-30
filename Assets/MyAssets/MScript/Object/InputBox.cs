using UnityEngine;
using System.Collections;
using System;

public class InputBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

//	protected void OnEnable() {
//		BEventManager.Instance.RegisterEvent (EventDefine.OnBackClick ,OnBackClick );
//	}
//	
//	protected void OnDisable() {
//		BEventManager.Instance.UnregisterEvent (EventDefine.OnBackClick, OnBackClick);
//	}

	public UIInput input;
	public void OnSubmit () {
		MessageEventArgs msg = new MessageEventArgs();
		msg.AddMessage("text",input.value);
		BEventManager.Instance.PostEvent(EventDefine.OnSubmitInput, msg);
		input.value = "";
	}

	public void OnChange () {
		MessageEventArgs msg = new MessageEventArgs();
		msg.AddMessage("text",input.value);
		BEventManager.Instance.PostEvent(EventDefine.OnChangeInput, msg);
	}
}
