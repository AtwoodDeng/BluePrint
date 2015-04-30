using UnityEngine;
using System.Collections;

public class MTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public UIInput mtext;
	public void OnSubmit () {
		Debug.Log("submit:"+mtext.value );
	}

	public void OnChange() {
		Debug.Log("change:"+mtext.value );
	}

	public void OnClick() {
		Debug.Log("click:"+mtext.label.text );
	}

	void OnGUI () {
		GUILayout.Label(mtext.label.text );
	}
}
