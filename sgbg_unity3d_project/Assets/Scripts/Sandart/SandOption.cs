using UnityEngine;
using System.Collections;

public class SandOption : MonoBehaviour {

	public Texture2D sandSpreadTex;
	public Texture2D sandEraserTex;

	private bool isReady = true;
	private const float TIME_INTERVAL = 0.5f;


	ToolType sandOption;

	// Use this for initialization
	void Start () {
		this.gameObject.renderer.material.mainTexture = sandSpreadTex;
		sandOption = ToolType.sand;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void buttonReady(){
		isReady = true;
		
	}

	void OnCanvasDown(){
		if(isReady == true){
			OnMouseDown();
			Invoke("buttonReady",TIME_INTERVAL);
			isReady = false;
		}
	}

	void OnMouseDown(){
		if(sandOption == ToolType.sand){
			sandOption = ToolType.sandEraser;
			this.gameObject.renderer.material.mainTexture = sandEraserTex;
		}
		else{
			sandOption = ToolType.sand;
			this.gameObject.renderer.material.mainTexture = sandSpreadTex;
		}

		GameObject sandCanvas = GameObject.Find ("canvas");
		Sandart sandCanvasScript = sandCanvas.GetComponent<Sandart> ();
		sandCanvasScript.OnToolChange (sandOption);
	}
}
