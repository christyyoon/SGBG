using UnityEngine;
using System.Collections;

public class waterbrush : MonoBehaviour {
	ToolType toolType = ToolType.waterbrush;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		GameObject toolkit = GameObject.Find ("toolkit");
		toolkit.SendMessage ("SelectTool",this); //send the object
		toolkit.SendMessage ("SelectToolName", toolType);	//send toolname
	}

}
