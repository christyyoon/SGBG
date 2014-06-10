using UnityEngine;
using System.Collections;

public class waddguide : MonoBehaviour {

	private bool isReady = true;
	private const float TIME_INTERVAL = 0.8f;

	void buttonReady(){
		isReady = true;
		//PlayerPrefs.DeleteKey ("isReady");
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	void setTimer(){
		isReady = false;
		Invoke("buttonReady",TIME_INTERVAL);
	}

	void OnCanvasDown(){
		if(isReady == true){
			OnMouseDown();
			Invoke("buttonReady",TIME_INTERVAL);
			//PlayerPrefs.SetInt("isReady",0);
			isReady = false;
		}
	}

	void OnMouseDown()
	{
		GameObject.Find ("guideline").transform.position = new Vector3 (-0.0002642766f, 0, 1.92f);
		GameObject.Find ("guideline").SendMessage ("setTimer");
	}
}
