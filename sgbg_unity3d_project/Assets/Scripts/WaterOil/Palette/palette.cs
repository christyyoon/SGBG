using UnityEngine;
using System.Collections;

public class palette : MonoBehaviour {
	
	public GameObject[] paint;//= new GameObject[8];
	public Color clr;
	public Color temp;
	public Color temp2;
	public int allTrueCaseCnt=0;
	public int check=1;

	// added by dmHyeon
	private int paintCount;
	private GameObject[] paints;
	private int nextToPaint;
	//private int nextToPaint = 0;

	//

	// Use this for initialization
	void Start () {
		//initialize all paint game objects.
		paints= new GameObject[8];
		for (int i=0; i<8; i++) {
			paints[i] = GameObject.Find("pallete/paint"+i.ToString());
			paints[i].renderer.enabled=false;
		}
		paintCount = 0;
		nextToPaint = 0;
		//Debug.Log ("initial clr"+clr);
		//Debug.Log ("initial temp"+temp);
		//temp = clr;
		
	}
	
	
	// Update is called once per frame
	void Update () {
		//paint [0].renderer.enabled = true;
		//paint [0].renderer.material.color = clr;
//		int clrChangeCnt = 0;
//		int paintCnt = 0;
//		//check = 1;
//		//Debug.Log("update clr"+clr);
//		//Debug.Log("update temp"+temp);
//		if (temp != clr) {
//			clrChangeCnt++;
//			//temp2=paint[(allTrueCaseCnt-1)%8].renderer.material.color;
//			temp=clr;
//			//Debug.Log("temp!=clr");
//			if((paint[0].renderer.enabled==true) && (paint[1].renderer.enabled==true) && (paint[2].renderer.enabled==true) && (paint[3].renderer.enabled==true) && (paint[4].renderer.enabled==true) && (paint[5].renderer.enabled==true) && (paint[6].renderer.enabled==true) && (paint[7].renderer.enabled==true) && (check==1))
//			{
//	
//				paint[allTrueCaseCnt%8].renderer.material.color=temp;
//				allTrueCaseCnt++;
//		
//				//Debug.Log("alltrue"+allTrueCaseCnt);
//
//			}
//
//			for(int i=0; i<8; i++)
//			{
//				if(paint[i].renderer.enabled==false)
//				{
//					paint[i].renderer.enabled=true;
//					paint[i].renderer.material.color=temp;
//					paintCnt++;
//				}
//				
//				if(clrChangeCnt==paintCnt)
//					break;
//			}
//
//			check=1;
//
//		}
	}

	public void OnPaintAdd(Color color){
		/*	1. compare color parameter to color of all paint game objects
		 	2. if same color is found, then end adding paint process
		 	3. else add color to paint game object
		 */
		Debug.Log ("OnPaintAdd");
		for (int i = 0; i < paintCount; i++) {
			if(color == paints[i].renderer.material.color)
				return; // already a paint have the color
		}

		if(paints[nextToPaint].renderer.enabled == false){
			paints[nextToPaint].renderer.enabled = true;
			paintCount++;
		}

		paints [nextToPaint].renderer.material.color = color;

		nextToPaint++;
		nextToPaint %= 8;
	}
}