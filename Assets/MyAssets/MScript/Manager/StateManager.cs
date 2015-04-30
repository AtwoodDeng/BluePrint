using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class StateManager : MonoBehaviour {

	static public string BASE_STATE_NAME = "base";

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	
	public StateManager() { s_Instance = this; }
	public static StateManager Instance { get { return s_Instance; } }
	private static StateManager s_Instance;

	protected void OnEnable() {
		BEventManager.Instance.RegisterEvent (EventDefine.OnSubmitInput , OnSubmitInput );
		BEventManager.Instance.RegisterEvent (EventDefine.OnChangeInput , OnChangeInput );
	}
	
	protected void OnDisable() {
		BEventManager.Instance.UnregisterEvent (EventDefine.OnSubmitInput , OnSubmitInput);
		BEventManager.Instance.RegisterEvent (EventDefine.OnChangeInput , OnChangeInput );
	}

	void OnSubmitInput(EventDefine eventName, object sender, EventArgs args)
	{
		MessageEventArgs msg = (MessageEventArgs)args;
		string inputText = msg.GetMessage("text");

		foreach( BlockState _blockState in blockStateList )
		{
			foreach ( UniState _state in _blockState.stateList )
			{
				if ( _state.recieveInput( inputText ) )
				{
					_blockState.StateOver(_state);

					foreach( UniState _for in _state.forwardStateList )
					{
						if ( _for.isPreStatesOver() )
						{
							_for.Active();
							_for.parent.StateActive(_for);
						}
					}
					return;
				}
			}
		}

	}
	
	void OnChangeInput(EventDefine eventName, object sender, EventArgs args)
	{
		MessageEventArgs msg = (MessageEventArgs)args;
		string inputText = msg.GetMessage("text");
	}

	public void Start()
	{
		initState();
	}


	public List<BlockState> blockStateList = new List<BlockState>();
	public Dictionary<String,UniState> stateDict = new Dictionary<string, UniState>();

	public void initState()
	{
		if (blockStateList.Count > 0 )
			return;

		// init all nodes
		XmlDocument xmlDoc = loadXml( sceneXmlUrl());
		XmlNodeList blockList = xmlDoc.SelectSingleNode("doc").SelectNodes("block");
		foreach( XmlElement xele in blockList )
		{
			blockStateList.Add( xmlEle2BlockState(xele));
		}

		// init links in the nodes
		foreach( XmlElement xele in blockList )
		{
			xmlEle2BlockLinks(xele);
		}
		// init links in edges
		XmlNodeList edgeList = xmlDoc.SelectSingleNode("doc").SelectNodes("edge");
		foreach( XmlElement xele in edgeList )
		{
			xmlEle2Link( xele );
		}
		// active init states

	}

	public static BlockState getBlockState(Block block)
	{
		BlockState blockState = getBlockState(block.name);
		blockState.block = block;
		return blockState;
	}
	
	public static BlockState getBlockState(string name)
	{
		s_Instance.initState();
		foreach( BlockState bstate in s_Instance.blockStateList )
		{
			if ( bstate.name.Equals(name ))
				return bstate;
		}
		return null;
	}

	public static BlockState xmlEle2BlockState(XmlElement xblock)
	{
		BlockState resState = new BlockState();
		resState.name = xblock.GetAttribute("name");
		XmlNodeList stateList = xblock.SelectNodes("state");
		foreach( XmlElement xstate in stateList)
		{
			string name = xstate.GetAttribute("name");
			string input = "";
			foreach( XmlElement xele in xstate.ChildNodes)
			{
				if ("input".Equals(xele.Name))
				{
					input = xele.InnerText;
				}
			}
			UniState newState = resState.addState( name , input );
			if (newState != null )
			{
				s_Instance.stateDict.Add(resState.name+"."+name,newState);
			}
		}
		return resState;
	}

	public static void xmlEle2BlockLinks(XmlElement xblock)
	{
		BlockState blockState = getBlockState(xblock.GetAttribute("name"));
		XmlNodeList stateList = xblock.SelectNodes("state");
		foreach( XmlElement xstate in stateList)
		{
			UniState state = null;
			foreach( UniState _state in blockState.stateList )
				if (_state.name.Equals(xstate.GetAttribute("name")))
					state = _state;

			foreach( XmlElement xele in xstate.ChildNodes)
			{
				if ("previous".Equals(xele.Name))
				{
					string[] preList = xele.InnerText.Split(';');
					foreach( string pre in preList )
					{
						string key = pre;
						if (string.IsNullOrEmpty(key))
							continue;
						if ( !key.Contains(".") )
							key = xblock.GetAttribute("name")+"."+key;
						if ( s_Instance.stateDict.ContainsKey( key ))
							state.addPreState(s_Instance.stateDict[key]);
					}
				}
				if ("forward".Equals(xele.Name))
				{
					string[] forList = xele.InnerText.Split(';');
					foreach( string forw in forList )
					{
						string key = forw;
						if (string.IsNullOrEmpty(key))
							continue;
						if ( !key.Contains(".") )
							key = xblock.GetAttribute("name")+"."+key;
						if ( s_Instance.stateDict.ContainsKey( key ))
							state.addForwardState(s_Instance.stateDict[key]);
					}
				}
			}
		}
	}

	public static void xmlEle2Link(XmlElement xedge )
	{
		string from = xedge.SelectSingleNode("from").InnerText;
		string[] toList = xedge.SelectSingleNode("to").InnerText.Split(';');

		UniState fromState = null;
		if ( s_Instance.stateDict.ContainsKey( from ) )
		{
			fromState = s_Instance.stateDict[from];
		}
		if ( fromState == null )
			return;
		foreach( string _to in toList )
		{
			if ( s_Instance.stateDict.ContainsKey( _to ))
			{
				fromState.addForwardState( s_Instance.stateDict[_to]);
			}
		}
	}

	public static XmlDocument loadXml( string url )
	{
		XmlDocument doc = new XmlDocument();  
		doc.Load(url);
		return doc;
	}

	public static string sceneXmlUrl(string scene="")
	{
		if(string.IsNullOrEmpty(scene))
			scene = LogicManager.Instance.name;
		return Application.dataPath + "/StreamingAssets/" + LogicManager.Instance.sceneName +".xml";
	}
}

public class UniState{
	public string name;
	public string input;
	public BlockState parent;
	public List<UniState> preStateList = new List<UniState>();
	public List<UniState> forwardStateList = new List<UniState>();

	public bool isActive
	{
		get{
			return _isActive;
		}
	}
	bool _isActive = false;
	public bool isOver
	{
		get{
			return _isOver;
		}
	}
	bool _isOver = false; 

	public UniState(string _name, string _input, BlockState _parent)
	{
		name = _name;
		input = _input;
		parent = _parent;
	}
	public bool addForwardState( UniState state)
	{
		foreach( UniState _state in forwardStateList )
		{
			if (_state == state )
				return false;
		}

		forwardStateList.Add(state);
		state.addPreState(this);
		return true;
	}
	public bool addPreState( UniState state)
	{
		foreach( UniState _state in preStateList )
		{
			if (_state == state )
				return false;
		}
		preStateList.Add(state);
		state.addForwardState(this);
		return true;
	}
	public void Active()
	{
		_isActive = true;
	}
	public void Disactive()
	{
		_isActive = false;
	}
	public bool isPreStatesOver()
	{
		bool isFinish = true;
		foreach( UniState _sta in preStateList )
		{
			if ( _sta.isOver == false )
				isFinish = false;
		}
		return isFinish;
	}
	public bool checkInput( string _input )
	{
		return _isActive && !_isOver && input.Equals(_input);
	}
	public bool recieveInput( string _input )
	{

		if ( _isActive && !_isOver )
		{
			if ( input.Equals(_input))
			{
				_isOver = true;
				return true;
			}
		}
		return false;
	}
}

public class BlockState{
	public string name;
	public List<UniState> stateList = new List<UniState>();
//	public int index;
	public UniState activeState;
	public Block block;

//	public UniState getState(string input)
//	{
//		if ( index < stateList.Count )
//		{
//			if ( stateList[index].input.Equals(input))
//			{
//				index++;
//				return stateList[index];
//			}
//		}else if ( index >= stateList.Count )
//		{
//			//if reach the last state
//		}
//		return null;
//	}

	public bool checkState(string _input)
	{
//		if ( index < stateList.Count )
//		{
//			if ( stateList[index].input.StartsWith(input))
//			{
//				return true;
//			}
//		}
		if ( activeState != null )
			if ( activeState.input.StartsWith( _input ))
				return true;

		return false;
	}

	public void refresh()
	{
//		index = 0;
		activeState = null;
		stateList.Clear();
	}

	public UniState addState(string name, string input)
	{
		UniState uniState = new UniState( name , input , this);
		stateList.Add(uniState);
		if ( uniState.name.Equals( StateManager.BASE_STATE_NAME ))
		{
			activeState = uniState;
			uniState.Active();
		}
		return uniState;
	}

	public void StateActive(UniState _state)
	{
		if ( block != null && activeState == null)
		{
			activeState = _state;
			block.StateActive();
		}
	}
	public void StateOver(UniState _state)
	{
		if ( block != null )
		{
			block.StateOver();
			activeState = null;
		}
	}
}
