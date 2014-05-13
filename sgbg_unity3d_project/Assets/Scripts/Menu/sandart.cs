using UnityEngine;
using System.Collections;

public class sandart : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	void OnMouseDown(){
		if (Input.GetMouseButtonDown (0)) { // left button down
			Application.LoadLevel ("sandart");
		}
	}

}