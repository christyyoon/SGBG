using UnityEngine;
using System.Collections;
using System.IO;

public class delpicture : MonoBehaviour {
	public int delState = 0; // 0->nothing, 1->ready, 2-> delete
	public int delOn = 0;
	public int fcount;
	//public int delnumber;
	//public picture pic = GetComponentInChildren<picture>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){

		//fcount = Directory.GetFiles ("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot", "*", SearchOption.AllDirectories).Length;
		if (delState == 0)
			delState = 1;
		//Debug.Log ("del 1 click");
		//Debug.Log ("delonchek" + delOnCheck);
		else if ((delState == 1)) {
			delState=2;
				}
		//Debug.Log ("mouseon");
	


	}
}