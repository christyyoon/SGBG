﻿using UnityEngine;
using System.Collections;
using System.IO;

/* Delete about selected picture in the gallery */
public class delPicture : MonoBehaviour {

	private bool isReady = true;
	private const float TIME_INTERVAL = 0.5f;
	
	public int state=0; // 0-> Activate deletion, 1-> Execute deletion
	public int fcount; // Count the number of file
	
	// Use this for initialization
	void Start () {
		if(PlayerPrefs.GetInt("isReady") == 0){
			isReady = false;
			Invoke("buttonReady",TIME_INTERVAL);
		}
		GameObject.Find("deletepresent").renderer.enabled=false; // Hide deletion's guide
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void buttonReady(){
		isReady = true;
		PlayerPrefs.DeleteKey ("isReady");
	}

	void OnCanvasDown(){
		if(isReady == true){
			isReady = false;
			Invoke("buttonReady",TIME_INTERVAL);
			PlayerPrefs.SetInt("isReady",0);
			OnMouseDown();
		}
	}

	void OnMouseDown()
	{
		fcount = Directory.GetFiles (Application.dataPath+"/galleryData", "*.png", SearchOption.AllDirectories).Length; // Count the number of file(파일개수)
		
		if (state == 0) // When push the delete button firstly
		{
			state = 1; // Because of execute deletion, when button is pushed again next time.
			GameObject.Find("deletepresent").renderer.enabled=true; // Present deletion's guide because of select picture about deletion
		}
		
		else if (state == 1) // When push delete button again, execute deletion
		{
			
			for(int i=1; i<=fcount; i++) // Examine selected picture as fcount
			{
				picture picturecode = GameObject.Find("picture"+i.ToString()).GetComponent<picture>(); // Get object's script(picture.cs)
				
				if(picturecode.delOn==1) // If object is selected as deletion
				{
					picturecode.DeletePic(); // Execute delete function
					picturecode.delOn=0; // Because deletion was executed
				}
				
			}
			
			state = 0; // Deletion was completed
			
			GameObject.Find("deletepresent").renderer.enabled=false; // Hide deletion's guide because of delete's completion

			Application.LoadLevel ("gallery"); // Restart gallery scene because of realignment


//			galleryObjGenerator frameScript =  GameObject.Find("frame").GetComponent<galleryObjGenerator>();
//			frameScript.updateGallery();

			
		}
		
	}
}