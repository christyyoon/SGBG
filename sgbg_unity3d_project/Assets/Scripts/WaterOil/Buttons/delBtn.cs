using UnityEngine;
using System.Collections;

public class delBtn : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCanvasDown(){
		GameObject canvas = GameObject.Find("canvas");
		canvas.SendMessage("OnCanvasDelete");
	}

	void OnMouseDown(){
		GameObject canvas = GameObject.Find("canvas");
		canvas.SendMessage("OnCanvasDelete");
	}
}
