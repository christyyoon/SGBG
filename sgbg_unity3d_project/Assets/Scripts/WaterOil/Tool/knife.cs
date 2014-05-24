using UnityEngine;
using System.Collections;

public class knife : MonoBehaviour {
	ToolType toolType = ToolType.knife;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCanvasDown(){
		GameObject toolkit = GameObject.Find ("toolkit");
		toolkit.SendMessage ("SelectTool",this); //send the object
		toolkit.SendMessage ("SelectToolName", toolType);	//send toolname
	}

	void OnMouseDown()
	{
		GameObject toolkit = GameObject.Find ("toolkit");
		toolkit.SendMessage ("SelectTool",this); //send the object
		toolkit.SendMessage ("SelectToolName", toolType);	//send toolname
	}
	
}
