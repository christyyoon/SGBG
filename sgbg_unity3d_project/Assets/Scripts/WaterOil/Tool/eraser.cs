using UnityEngine;
using System.Collections;

public class eraser : MonoBehaviour {
	ToolType toolType = ToolType.eraser;
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
//		if (Input.GetMouseButtonDown (0)) {
//			Debug.Log ("e click");
			GameObject toolkit = GameObject.Find ("toolkit");
			toolkit.SendMessage ("SelectTool",this); //send the object
			toolkit.SendMessage ("SelectToolName", toolType);	//send toolname
		//}
		
	}
}
