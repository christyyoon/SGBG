﻿using UnityEngine;
using System.Collections;

public class quit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}

	void OnCanvasDown(){
		Application.Quit (); // terminate program.
	}

	void OnMouseDown(){
				if (Input.GetMouseButtonDown (0)) { // left button down
						Application.Quit ();
				}
		}
}
