using UnityEngine;
using System.Collections;
using System.IO;

public class screenshot : MonoBehaviour {

	private int count = 1; // Number of file
	private int limit = 3;// Limit number of file in screenshot
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	
	}

	void OnMouseDown() {

		// Create screenshot folder in data file
		Directory.CreateDirectory ("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot");

		// If file exists, increase count
		while (File.Exists ("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + count + ".png"))
			count++;

		// If file not exists
		if (File.Exists ("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + count + ".png") == false) {

			// If file number is over than limit number
			if(count>limit){

				// Delete capture1.png because of limit
				File.Delete("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture1.png");

				// Change orgin number to (origin-1) number ex) capture2->capture1, capture3->capture2
				for(int i=2; i<=limit; i++)
					File.Move("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + i + ".png", "C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (i-1) + ".png");

				Application.CaptureScreenshot ("..\\artbox_Data\\screenshot\\capture"+(count-1)+".png");

			}

			else
				Application.CaptureScreenshot("..\\artbox_Data\\screenshot\\capture"+(count)+".png");
		}
	}
}
