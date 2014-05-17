using UnityEngine;
using System.Collections;

public class pencil : MonoBehaviour {
	ToolType toolType = ToolType.pencil;
	// Use this for initialization
	void Start () {
		GameObject toolkit = GameObject.Find ("toolkit");
		toolkit.SendMessage ("SelectTool",this); //send the object
		toolkit.SendMessage ("SelectToolName", toolType);	//send toolname
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnMouseDown()
	{
		Debug.Log ("pencil onMouseDown");
		GameObject toolkit = GameObject.Find ("toolkit");
		toolkit.SendMessage ("SelectTool",this); //send the object
		toolkit.SendMessage ("SelectToolName", toolType);	//send toolname
	}
	


}
