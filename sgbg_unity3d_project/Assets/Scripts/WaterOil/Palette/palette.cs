using UnityEngine;
using System.Collections;
using System.IO;

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

	private bool isStarted = false;
	//

	// Use this for initialization
	void Start () {
		if(isStarted == true)
			return ;
		isStarted = false;

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

	}

	public void init(string fileName){
		isStarted = true; // don't excute start method after this method

		//fileName : 2014~~~~~.data
		FileInfo datafile = new FileInfo (Application.dataPath + "/galleryData/" + fileName); // Datefile
		
		StreamReader reader = datafile.OpenText (); // Open datafile by text
		
		reader.ReadLine (); // Read first line(wateroil or sandart)
		
		int countOfPaint = System.Convert.ToInt32 (reader.ReadLine ()); // Read paint's count and convert to integer
		
		/* Activate paint object and set color */
		for (int i=0; i<countOfPaint; i++)
		{
			string unionOfRGBA = reader.ReadLine (); // Data file's union of paint's rgba
			string[] rgba = unionOfRGBA.Split (); // Split union of paint's rgba and distribute to array
			
			// set rgba string to rgba value
			float r = (float)System.Convert.ToDouble (rgba [0]);
			float g = (float)System.Convert.ToDouble (rgba [1]);
			float b = (float)System.Convert.ToDouble (rgba [2]);
			float a = (float)System.Convert.ToDouble (rgba [3]);
			
			paints [i].renderer.enabled = true; // Activate paint object
			paints [i].renderer.material.color = new Color (r, g, b, a); // Set paint's color
			
		}

		paintCount = countOfPaint;
		nextToPaint = paintCount%8;


	}

	public Color[] getPaintsColor(){
		Color[] paintsColor = new Color[paintCount];

		for (int i = 0; i < paintCount; i++) {
			GameObject paint = GameObject.Find("pallete/paint" + i.ToString());
			paintsColor[i] = paint.renderer.material.color;
		}

		return paintsColor;
	}

	public void OnPaintAdd(Color color){
		/*	1. compare color parameter to color of all paint game objects
		 	2. if same color is found, then end adding paint process
		 	3. else add color to paint game object
		 */
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