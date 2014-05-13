using UnityEngine;
using System.Collections;

//select the tools one at a time
//tool -> drawing algorithm (canvas)

public class toolkit : MonoBehaviour {
	
	MonoBehaviour previousTool = null;
	ToolType previousToolName;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SelectToolName (ToolType toolName)  //get toolname
	{
		previousToolName = toolName;
		
		Debug.Log (previousToolName.ToString());
		
//		GameObject DrawingAlgo = GameObject.Find ("Main Camera");
//		DrawingAlgo.SendMessage ("getToolName", toolName);
		GameObject canvas = GameObject.Find ("canvas");
		canvas.SendMessage ("OnToolChange",previousToolName);
	}
	
	public void SelectTool(MonoBehaviour tool) //get tool object
	{
		if (previousTool != null) // first call to this function
		{
			previousTool.transform.Translate (0.02f, 0, 0); //deselected 
		}
		
		tool.transform.Translate (-0.02f, 0, 0); //selected
		
		previousTool = tool;
		
	}
}