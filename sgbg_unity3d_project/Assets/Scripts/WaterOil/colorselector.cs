using UnityEngine;
using System.Collections;

public class colorselector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()  //click the color
	{
		int r = Random.Range (0, 256);
		int g = Random.Range (0, 256);
		int b = Random.Range (0, 256);


		Color color = new Color (r, g, b);

		GameObject pallete = GameObject.Find ("pallete");
		pallete.SendMessage ("getColor", color);

		GameObject colorviewer = GameObject.Find ("colorviewer");
		colorviewer.SendMessage ("getColor", color);

	}

}
