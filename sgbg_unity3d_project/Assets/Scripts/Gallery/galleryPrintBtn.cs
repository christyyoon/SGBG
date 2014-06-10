using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices; // needed to import dll 
using System.IO;

public class galleryPrintBtn : MonoBehaviour {
	
	private bool isReady = true;
	const float TIME_INTERVAL = 2.0f;
	public bool onPrint = false;
	private string filePath = string.Empty;
	
	[DllImport("PRINT_DLL")]private static extern void fnPrint_PngFilePrint( string csFileName );
	
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

	public void setFileName(string fp){
		filePath = fp;
	}
	
	void OnMouseDown(){
	
		// read
		if(filePath == string.Empty)
			return;

		fnPrint_PngFilePrint (filePath);
		onPrint = false;
		filePath = string.Empty;

//		Debug.Log ("print funct start");
//		//1. get canvas image (Texture2D)
//		GameObject canvas = GameObject.Find ("canvas");
//		drawingOnGUI canvasScript = canvas.GetComponent<drawingOnGUI> ();
//		Texture2D canvasTex;
//		
//		if(canvasScript == null){
//			Sandart sandartCanvasScript = canvas.GetComponent<Sandart> ();
//			canvasTex = sandartCanvasScript.GetCanvasTex();
//		}else
//			canvasTex = canvasScript.GetCanvasTex ();
//		
//		byte[] canvasPng = canvasTex.EncodeToPNG();
//		
//		string galleryPath = Application.dataPath + "/galleryData/";
//		
//		File.WriteAllBytes (galleryPath + "temp.png", canvasPng);
//		
//		fnPrint_PngFilePrint (galleryPath + "temp.png");
//		
//		File.Delete (galleryPath + "temp.png");
//		
//		Debug.Log ("Print Ok!" );
	}
}
