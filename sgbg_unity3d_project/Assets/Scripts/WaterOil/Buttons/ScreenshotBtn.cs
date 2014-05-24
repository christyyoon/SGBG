using UnityEngine;
using System.Collections;
using System.IO;

public class ScreenshotBtn : MonoBehaviour {

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
		// TODO set Timer or wait until Canvas Up event
	}

	void OnMouseDown(){
		/*step
		 * 1. get canvas image (Texture2D)
		 * 2. encode the image to png file
		 * 3. save file to specific directory
		 */

		//1. get canvas image (Texture2D)
		GameObject canvas = GameObject.Find ("canvas");
		drawingOnGUI canvasScript = canvas.GetComponent<drawingOnGUI> ();

		Texture2D canvasTex = canvasScript.GetCanvasTex ();

		//2. encode the image to png file
		byte[] canvasPng = canvasTex.EncodeToPNG();

		string galleryPath = Application.dataPath + "/galleryData/";
		//3. save file to specific directory
		int fcount = Directory.GetFiles (galleryPath, "*.png", SearchOption.AllDirectories).Length; // Count the number of file(파일개수)
		string[] files = Directory.GetFiles (galleryPath, "*.png", SearchOption.AllDirectories); // String array(save screenshot file)
		int limit = 3;

		// if file number reached at limit number, then delete the oldest file
		if (fcount == limit){
			string filename = files[0].Substring(files[0].Length - 18 , 14); // 'a.png' -> 'a'

			File.Delete (files [0]);

			if(File.Exists (galleryPath + filename + ".data")){

				string[] data = Directory.GetFiles (galleryPath, filename+".data", SearchOption.AllDirectories);
				string[] buf = Directory.GetFiles (galleryPath, filename+".buf", SearchOption.AllDirectories);

				File.Delete (data [0]);
				File.Delete (buf [0]);
			}
		}
		
		File.WriteAllBytes (galleryPath + System.DateTime.Now.ToString("yyyyMMddHHmmss")+".png", canvasPng);

	}
}
