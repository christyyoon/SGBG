using UnityEngine;
using System.Collections;
using System.IO;

public class screenshot : MonoBehaviour {
	
	//private int count = 1; // Number of file
	private int limit = 3; // Limit number of file in screenshot
	private int fcount; // Count the number of file
	private string[] files; // String array(save screenshot file)

	// Use this for initialization
	void Start () {

		// If screenshot directory not exist, create directory
		if (!Directory.Exists (Application.dataPath + "/screenshot")) {
						Directory.CreateDirectory (Application.dataPath + "/screenshot");
				}
	}
	
	// Update is called once per frame
	void Update () {

		
	}
	
	void OnMouseDown() {

		fcount = Directory.GetFiles (Application.dataPath+"/screenshot", "*", SearchOption.AllDirectories).Length; // Count the number of file(파일개수)
		files = Directory.GetFiles (Application.dataPath + "/screenshot", "*", SearchOption.AllDirectories); // String array(save screenshot file)

		// if file number reached at limit number, then delete the oldest file
		if (fcount == limit)
			File.Delete (files [0]);

		// Capture screenshot name by datetime
		Application.CaptureScreenshot (Application.dataPath + "/screenshot/"+System.DateTime.Now.ToString("yyyyMMddHHmmss")+".png");

	}
}
