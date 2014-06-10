using UnityEngine;
using System.Collections;
using System.IO;

public class SaveSandartSpace : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(!Directory.Exists(Application.dataPath + "/galleryData")){
			Directory.CreateDirectory(Application.dataPath + "/galleryData");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCanvasDown(){
		OnMouseDown ();
	}
	
	void OnMouseDown(){
		// file : screenshot, paint + scene, drawing data
		
		/* Step
		 * 1. sence name
		 * 2. paint
		 * 3. 
		 */
		GameObject canvas = GameObject.Find ("canvas");
		Sandart canvasScript = canvas.GetComponent<Sandart> ();
		
		if(canvasScript.isCanvasChange() == false) // no change in canvas
			return;
		
		string galleryPath = Application.dataPath + "/galleryData/";
		
		string canvasID = canvasScript.getCanvasID ();
		
		string fileName = string.Empty;
		
		if(canvasID == string.Empty)// new art space
			fileName = galleryPath + System.DateTime.Now.ToString ("yyyyMMddHHmmss");
		else 						// old art space
			fileName = galleryPath + canvasID;
		
		
		// screenshot
		int fcount = Directory.GetFiles (galleryPath, "*.png", SearchOption.AllDirectories).Length; // Count the number of file(파일개수)
		string[] files = Directory.GetFiles (galleryPath, "*.png", SearchOption.AllDirectories); // String array(save screenshot file)
		
		int limit = 6;
		
		// if file number reached at limit number, then delete the oldest file
		if (fcount == limit && canvasID == string.Empty){
			string filename = files[0].Substring(files[0].Length - 18 , 14); // 'C:\user\20140512123123.png' -> '20140512123123'
			
			File.Delete (files [0]); // delete png
			
			if(File.Exists (galleryPath + filename + ".data")){
				
				string[] data = Directory.GetFiles (galleryPath, filename+".data", SearchOption.AllDirectories);
				string[] buf = Directory.GetFiles (galleryPath, filename+".buf", SearchOption.AllDirectories);
				
				File.Delete (data [0]);
				File.Delete (buf [0]);
			}
		}
		
		Application.CaptureScreenshot (fileName+".png");
		
		// scence
		StreamWriter sw = new StreamWriter (File.Open(fileName+".data", FileMode.Create));
		sw.WriteLine ("sandart");
		sw.Close ();
		
		// drawing data
		
		short[] canvasBuf = canvasScript.getCanvasBuf ();
		
		using(BinaryWriter writer = new BinaryWriter(File.Open(fileName+".buf", FileMode.Create))){
			foreach(short elem in canvasBuf){
				writer.Write(elem);
			}
		}
		Debug.Log ("working spaced saved!");
	}
}
