using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class Block : MonoBehaviour {

	public enum BState
	{
		Wait,
		Open
	};
	public string name;
	public BState mState = BState.Wait;

	public Uni2DSprite black;
	public Uni2DSprite background;
	public Uni2DSprite obj;
	public Animator objAnimator;
	public Camera camera;
	private int _layer;
	public tk2dTextMesh TipsText;
	public tk2dTextMesh TipsCoverText;
	
	public BlockState blockState;

	public GameObject shade;

	protected void OnEnable() {
//		BEventManager.Instance.RegisterEvent (EventDefine.OnSubmitInput , OnSubmitInput );
		BEventManager.Instance.RegisterEvent (EventDefine.OnChangeInput , OnChangeInput );
	}
	
	protected void OnDisable() {
//		BEventManager.Instance.UnregisterEvent (EventDefine.OnSubmitInput , OnSubmitInput);
		BEventManager.Instance.RegisterEvent (EventDefine.OnChangeInput , OnChangeInput );
	}

	// Use this for initialization
	void Awake () {
		_layer = this.gameObject.layer;
		TipsText.text = "";
		TipsCoverText.text = "";

		camera.cullingMask = (int)Math.Pow(2.0,(double)_layer);

		blockState = StateManager.getBlockState(this);
		this.StateActive();

		shade.SetActive( true );

	}
	
	// Update is called once per frame
	void Update () {

	}

	int layer2ID (string layer)
	{
		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty it = tagManager.GetIterator();
		while (it.NextVisible(true))
		{
			if(it.name.StartsWith("User Layer"))
			{
				if (it.type == "string")
				{
					if (it.stringValue.Equals(layer))
					{
						int id = int.Parse( it.name.Substring(it.name.Length-2));
//						int mask = (int)Math.Pow(2.0,(double)id);
						return id;
					}
				}

			}
		}
		return 0;
	}

//	void OnSubmitInput(EventDefine eventName, object sender, EventArgs args)
//	{
//		MessageEventArgs msg = (MessageEventArgs)args;
//		if (mState == BState.Open )
//		{
//			UniState state = blockState.getState(msg.GetMessage("text"));
//			if (state != null)
//			{
//				objAnimator.SetTrigger("next");
//			}
//		}
//
//	}

	public void StateActive()
	{
		if ( blockState != null && blockState.activeState != null )
		{
			TipsText.text = blockState.activeState.input;
			objAnimator.SetTrigger(blockState.activeState.name);
			Debug.Log("setTrigger"+blockState.activeState.name);
			TipsCoverText.text = "";
		}
		else
		{
			TipsText.text = "";
			TipsCoverText.text = "";
		}
	}
	public void StateOver()
	{
		TipsText.text = "";
		TipsCoverText.text = "";
	}

	void OnChangeInput(EventDefine eventName, object sender, EventArgs args)
	{
		MessageEventArgs msg = (MessageEventArgs)args;
		string input = msg.GetMessage("text");
		if (blockState.checkState(input))
		{
			TipsCoverText.text = input;
		}
		
	}

	string blank( int number )
	{
		if ( number <= 0 )
			return "";
		return blank ( number - 1 ) + " ";
	}
}
