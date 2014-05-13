using UnityEngine;
using System.Collections;

public class colorviewer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void getColor(Color color)
	{
		Color currentColor = color;
		
		Debug.Log ("colorviewer"+" "+color.r + "," + color.g + "," + color.b);
	}
}
