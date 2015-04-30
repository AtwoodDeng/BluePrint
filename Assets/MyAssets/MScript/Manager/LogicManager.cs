using UnityEngine;
using System.Collections;

public class LogicManager : MonoBehaviour {
	
	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	public string sceneName;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
