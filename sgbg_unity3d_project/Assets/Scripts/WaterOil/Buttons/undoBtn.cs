using UnityEngine;
using System.Collections;

public class undoBtn : MonoBehaviour {

	private bool isReady = true;
	private const float TIME_INTERVAL = 0.2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void buttonReady(){
		isReady = true;

	}

	public void OnCanvasDown(){

		if(isReady == true){
			GameObject canvas = GameObject.Find("canvas");
			canvas.SendMessage("OnUndo");
			Invoke("buttonReady",TIME_INTERVAL);
			isReady = false;
		}

	}

	void OnMouseDown(){
		if(isReady == true){
			GameObject canvas = GameObject.Find("canvas");
			canvas.SendMessage("OnUndo");
			Invoke("buttonReady",TIME_INTERVAL);
			isReady = false;
		}
	}
}
