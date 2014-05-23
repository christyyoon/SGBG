using UnityEngine;
using System.Collections;

public class paint1 : MonoBehaviour {
	public GameObject drawingobj; // Drawing object
	public DrawingAlgo algo; // Drawing code
	
	public GameObject paintobj; // Paint object
	
	public palette pal; // Pallete code
	public GameObject palobj; // pallete object
	
	public GameObject view; // Colorviewer object
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseDown()
	{
		drawingobj = GameObject.Find("Main Camera");
		algo = drawingobj.GetComponent<DrawingAlgo>();
		paintobj = GameObject.Find ("pallete/paint1");
		algo.col = paintobj.renderer.material.color; // Set drawing color to paint object's color
		
		palobj = GameObject.Find ("pallete");
		pal = palobj.GetComponent<palette>();
		pal.check=0; // Send message(paint object is selected) to pallete
		
		view = GameObject.Find("colorview");
		view.renderer.material.color = algo.col; // Change colorviewers's color to drawing color
		
	}
}
