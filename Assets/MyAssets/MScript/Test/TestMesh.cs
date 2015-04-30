using UnityEngine;
using System.Collections;


public class TestMesh : MonoBehaviour {

	public Material mMaterial;
	public Renderer mRender;
	public Vector2 mOffset;
	public Vector2 mScale;
	// Use this for initialization
	void Start () {
		mRender = GetComponent<Renderer>();
		if (mRender!= null)
		{
			mMaterial = mRender.material;
			mOffset = mMaterial.mainTextureOffset;
			mScale = mMaterial.mainTextureScale;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (mMaterial != null )
		{
			mMaterial.mainTextureOffset = mOffset;
			mMaterial.mainTextureScale = mScale;
		}
	}

	void OnGUI(){
		GUILayout.Label("offset"+mMaterial.mainTextureOffset.ToString());
		GUILayout.Label("scale"+mMaterial.mainTextureScale.ToString());
	}
}
