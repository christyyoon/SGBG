using UnityEngine;
using System.Collections;

public class redoBtn : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnMouseDown(){
		GameObject canvas = GameObject.Find("canvas");
		canvas.SendMessage("OnRedo");
	}
}
