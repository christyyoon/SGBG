using UnityEngine;
using System.Collections;

public class paint4 : MonoBehaviour {
	public DrawingAlgo algo;
	public GameObject paintobj;
	public palette pal;
	public GameObject palobj;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnCanvasDown(){
		paintobj = GameObject.Find ("pallete/paint4");
		Color color = paintobj.renderer.material.color;
		
		GameObject canvas = GameObject.Find("canvas");
		drawingOnGUI canvasScript = canvas.GetComponent<drawingOnGUI>();
		canvasScript.OnColorChange(color);
		
		GameObject view = GameObject.Find("colorview");
		view.renderer.material.color = color;
		
		GameObject palobj = GameObject.Find("pallete");
		pal = palobj.GetComponent<palette>();
		pal.check=0;
	}

	void OnMouseDown(){
		if (Input.GetMouseButtonDown (0)) { // left button down
			paintobj = GameObject.Find ("pallete/paint4");
			Color color = paintobj.renderer.material.color;
			
			GameObject canvas = GameObject.Find("canvas");
			drawingOnGUI canvasScript = canvas.GetComponent<drawingOnGUI>();
			canvasScript.OnColorChange(color);
			
			GameObject view = GameObject.Find("colorview");
			view.renderer.material.color = color;
			
			GameObject palobj = GameObject.Find("pallete");
			pal = palobj.GetComponent<palette>();
			pal.check=0;
		}
	}
}
