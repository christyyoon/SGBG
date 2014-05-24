using UnityEngine;
using System.Collections;

public class Sandart : MonoBehaviour {
	
	public Texture2D sandTex;
	
	private Rect textureArea;
	private Vector2 leftTopPosition;
	private int width;
	private int height;
	
	private Texture2D baseTex;
	
	// Use this for initialization
	void Start () {
		//canvas position, size (GUI texture) are hard-corded. Constants are based on 800 x 600 resolution
		leftTopPosition = new Vector2 (Screen.width*0.17f,Screen.height*0.223f);
		
		width = (int)(Screen.width*0.663f);
		height = (int)(Screen.height*0.6f);

		textureArea = new Rect (leftTopPosition.x,
		                        leftTopPosition.y,
		                        width,
		                        height);
		
		baseTex = new Texture2D (width, height,TextureFormat.RGBA32, false);
		
		//set texture color to white
		for(int i = 0 ; i < width ; i++)
			for(int j = 0 ; j < height ; j++)
				baseTex.SetPixel(i,j,new Color(1.0f,1.0f,1.0f,0.0f));
		
		baseTex.Apply (false);
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){ // TODO is mouse contained in texture area
			
			Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
			int xCanvas = (int)(mousePosition.x - leftTopPosition.x);
			int yCanvas = (int)(height - (mousePosition.y - leftTopPosition.y));
			
			dropSand (xCanvas,yCanvas);
		}
		
	}
	
	private void dropSand(int x, int y){
		int alpha = sandTex.width / 2;
		int beta = sandTex.height / 2;
		
		for(int i = y - beta; i < y + beta; i++)
		for(int j = x - alpha ; j < x + alpha ; j++){
			Color sandPixClr = sandTex.GetPixel(j - x + alpha, i - y + beta);
			Color basePixClr = baseTex.GetPixel(j,i);

			//sand art alogrithm
			if(basePixClr.a != 0){
				continue;
			}
			baseTex.SetPixel(j,i,sandPixClr);
		}
		
		baseTex.Apply (false);
	}
	
	void OnMouseDown(){
		
	}
	
	void OnGUI(){
		GUI.DrawTexture (textureArea, baseTex);
		
	}
}
