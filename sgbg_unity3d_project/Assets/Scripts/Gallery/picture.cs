﻿using UnityEngine;
using System.Collections;
using System.IO;

public class picture : MonoBehaviour {
	public Vector3 receivedScale; // Value of receiver(scale)
	public Vector3 receivedPosition; // Value of receiver(position)
	public galleryobj gal; 
	public GameObject canvasobj; // Declare object
	public GameObject deleteBtn;
	public delpicture del;
	public int check = 1; // Identification
	public int delOnOff=0;
	public int picnumber;
	//public int mustdel=0;

	// Use this for initialization

	void Start () {
		canvasobj = GameObject.Find ("canvas"); // Find gameobject
		gal = canvasobj.GetComponent<galleryobj> (); // Get script
		receivedScale = gal.originScale; // Receive originscale
		receivedPosition = gal.originPosition; // Receive originposition
		picnumber = gal.objnumber;
		Debug.Log ("startpicnum" + picnumber);

	}
	
	// Update is called once per frame
	void Update () {
		//picnumber = gal.objnumber;
		//Debug.Log ("updatepicnum" + picnumber);
		deleteBtn = GameObject.Find("delete");
		del = deleteBtn.GetComponent<delpicture>();
		if (del.delState == 1)
			renderer.material.color = Color.grey;
		else if ((del.delOn==1) && (delOnOff==1) && (del.delState==2)) {
			Debug.Log("picnumbeforedel"+picnumber);
			File.Delete("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture"+(picnumber)+".png");
			del.delState=0;
			Debug.Log("picnumafterdel"+picnumber);
		}

	}

	void OnMouseDown()
	{

		deleteBtn = GameObject.Find("delete");
		del = deleteBtn.GetComponent<delpicture>();
		if ((check == 1) && (del.delState == 1) && (delOnOff == 0)) {
			delOnOff=1;
			renderer.transform.Rotate(0, 0, -7);
			//del.delOnCheck = 2;
			//mustdel = 1;
			del.delOn=1;
			Debug.Log(picnumber);


				}
		else if ((check == 1) && (del.delState == 1) && (delOnOff == 1)) {
			renderer.transform.Rotate(0, 0, 7);
			delOnOff = 0;
			//del.delOnCheck = 1;
			//mustdel=0;
			del.delOn=0;
		}

		// If select object, object is expanded
		else if (check == 1) {
						renderer.transform.position = new Vector3 (0.065f, 1.01f, -0.1f);
						renderer.transform.localScale = new Vector3 (2.33f, 1.75f, 0.01f);
						check = 0;
				}

		// If select expanded object, object is retuned originally
		else if (check == 0) {
						renderer.transform.position = receivedPosition;
						renderer.transform.localScale = receivedScale;
						check = 1;
		}

	}
}