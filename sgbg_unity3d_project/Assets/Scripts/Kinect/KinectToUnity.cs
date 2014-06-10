﻿using UnityEngine;
using System.Collections;
using System.IO;

//[RequireComponent(typeof(Renderer))]
public class KinectToUnity : MonoBehaviour {

	private static KinectToUnity _instance;
	
	public DepthWrapper dw;
	public GUIText text; // GUI text to show the minimum value of depth (for testing)
	public GUITexture guiT;
	
	private short minDepth; // minimum depth in depth buffer
	private Vector2 minPoint;
	private short[] depthSnapshot;
	private Vector2[] ROIVertex = new Vector2[2]; // store leftTop, rightBottom point
	private int pointCount = 0;
	
	private bool isDetected = false;
	
	private short errorRange = 15; // range of vibration degree
	
	private Vector2 prevMinPtr;
	
	#region
	// Use this for initialization
	void Start () {
		prevMinPtr = new Vector2 (0, 0);
	}

	public static KinectToUnity Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<KinectToUnity>();

				if(!_instance){
					GameObject container = new GameObject();
					container.name = "KinectToUnityContainer";
					_instance = container.AddComponent(typeof(KinectToUnity)) as KinectToUnity;
				}
				//Tell unity not to destroy this object when loading a new scene!
				//DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}
	
	void Awake() 
	{
		if(_instance == null)
		{
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if(this != _instance)
				Destroy(this.gameObject);
		}
	}

	#endregion
	
	private void arrayCopy(short[] des, short[] src){// for deep copy
		for (int i = 0; i < des.Length; i++) {
			des[i] = src[i];
		}
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.S)) { // key S means snapshot
			Debug.Log ("S key pressed!");
			
			// store a snapshot of depth image
			//depthSnapshot = (short[]) dw.depthImg.Clone(); // deep copy
			depthSnapshot = new short[320*240];
			arrayCopy(depthSnapshot,dw.depthImg);
			
			//initiate ROI vertics manually
			ROIVertex[0].Set(55,3);//left top
			ROIVertex[1].Set(310,195);//right bottom
			
			//initiate number of vertex
			pointCount = 2;
		}
		
		if (dw.pollDepth())
		{
			Debug.Log("poll depth");
			
			if(pointCount == 2){ //two point(LT, RB) were detected
				callGameObjByKinect(dw.depthImg);
			}

		}
	}

	public void Run(){
			if (Input.GetKeyDown (KeyCode.S)) { // key S means snapshot
			Debug.Log ("S key pressed!");
			
			// store a snapshot of depth image
			//depthSnapshot = (short[]) dw.depthImg.Clone(); // deep copy
			depthSnapshot = new short[320*240];
			arrayCopy(depthSnapshot,dw.depthImg);
			
			//initiate ROI vertics manually
			ROIVertex[0].Set(55,3);//left top
			ROIVertex[1].Set(310,195);//right bottom
			
			pointCount = 2;
			//initiate number of vertex
		}
		
		if (dw.pollDepth())
		{
			Debug.Log("poll depth");
			
			if(pointCount == 2){ //two point(LT, RB) were detected
				callGameObjByKinect(dw.depthImg);
			}else {
				
			}
			
			if(depthSnapshot != null){
				//text.guiText.text += "\nsnapshot";
			}
			//tex.SetPixels32(convertPlayersToCutout(dw.segmentations));
			//tex.Apply(false);
		}
	}
	
	private Vector2 findPointInROI(short[] depthBuf){
		for (int i = (int)ROIVertex[0].y ; i <= (int)ROIVertex[1].y ; i++) {
			for(int j = (int)ROIVertex[0].x ; j <= (int)ROIVertex[1].x ; j++) {
				int pix = pointToValue(j,i,320);
				minDepth = 0; // initialize mindepth per frame
				minPoint.Set (0, 0);
				// 's' key was pressed and two canvas vertex(leftTop, rightBottom) are not detected
				// restrict depth range from 800mm to 1000mm b/c IR noise (kinect depth accruacy decreases with increasing distance from the sensor) 
				if (depthSnapshot != null && pointCount == 2 && depthBuf [pix] >= 800 && depthSnapshot [pix] <= 1000) { // pointCount != 2 -> two points were detected
					
					Vector2 currentPtr = valueToPoint (pix, 320);
					
					if (currentPtr.x > ROIVertex [0].x && currentPtr.x < ROIVertex [1].x &&
					    currentPtr.y > ROIVertex [0].y && currentPtr.y < ROIVertex [1].y) {
						
						short depthDifference = (short)(depthSnapshot [pix] - depthBuf [pix]);
						
						if (minDepth < depthDifference) {// find greatest depth diff 
							//Debug.Log("point detected "+depthSnapshot[pix]+","+depthBuf[pix]);
							
							minPoint = currentPtr;
							minDepth = depthDifference;
						}
					}
					
				}
			}
		}
		float x,y;
		
		x = 1 - (minPoint.x-ROIVertex[0].x) / (ROIVertex[1].x - ROIVertex[0].x); 
		y = (minPoint.y-ROIVertex[0].y) / (ROIVertex[1].y - ROIVertex[0].y);
		return new Vector2 (x, y);
	}
	
	
	private int pointToValue(int x, int y, int row){
		return y * row + x;
	}
	private Vector2 valueToPoint(int index,int row){
		Vector2 point = new Vector2();
		int x = index % row;
		int y = index / row;
		
		point.Set (x, y);
		
		return point;
	}
	
	GameObject findGameObject(float x, float y){
		//Ray ray = camera.ScreenPointToRay(new Vector3(x,y,0));  
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(x,y,0));  
		RaycastHit hitObj;
		
		if (Physics.Raycast (ray, out hitObj, Mathf.Infinity)) {
			string hitObjName = hitObj.transform.name;
			
			return GameObject.Find (hitObjName);
		} else
			return null;
	}
	
	GameObject findColorSelector(float x, float y){
		if(UICamera.currentCamera == null) // current scene is not a water-oil space
			return null;

		Ray ray = UICamera.currentCamera.ScreenPointToRay(new Vector3(x,y,0));  
		RaycastHit hitObj;
		
		if (Physics.Raycast (ray, out hitObj, Mathf.Infinity)) {
			string hitObjName = hitObj.transform.name;
			
			return GameObject.Find (hitObjName);
		} else
			return null;
	}
	
	private void callGameObjByKinect(short[] depthBuf)
	{
		minDepth = 0; // initialize mindepth per frame
		minPoint.Set (0, 0);

		//find a min coordinate in ROI 
		for (int pix = 0; pix < depthBuf.Length; pix += 2) {
			
			// 's' key was pressed and two canvas vertex(leftTop, rightBottom) are not detected
			// restrict depth range from 800mm to 1000mm b/c IR noise (kinect depth accruacy decreases with increasing distance from the sensor) 
			if (depthSnapshot != null && pointCount == 2 && depthBuf [pix] >= 700 && depthSnapshot [pix] <= 1000) { // pointCount != 2 -> two points were detected
				
				Vector2 currentPtr = valueToPoint (pix, 320);
				
				if (currentPtr.x > ROIVertex [0].x && currentPtr.x < ROIVertex [1].x &&
				    currentPtr.y > ROIVertex [0].y && currentPtr.y < ROIVertex [1].y) {
					
					short depthDiff = (short)(depthSnapshot [pix] - depthBuf [pix]);
					
					if (minDepth < depthDiff) {// find greatest depth diff 
						//Debug.Log("point detected "+depthSnapshot[pix]+","+depthBuf[pix]);
						
						minPoint = currentPtr;
						minDepth = depthDiff;
					}
				}
				
			}
		}
		
		//valid input!//
		if(pointCount == 2 && minDepth > errorRange){
			
			if(isDetected == false) // for canvas up event
				isDetected = true;
			
			float x,y;
			
			//find x,y ratio
			x = (minPoint.x-ROIVertex[0].x) / (ROIVertex[1].x - ROIVertex[0].x); 
			y = (minPoint.y-ROIVertex[0].y) / (ROIVertex[1].y - ROIVertex[0].y); // b/c kinect look at the canvas in reverse
			
			// coordinates transformation from kinect to unity
			y = 1 - y;
			
			// for debuging
			if(guiT != null)
				guiT.transform.position = new Vector3(x,y,0f); 

			// adjust calibration manually 
			//y -= 0.06f; 

			//for debugging
			text.guiText.text = "spandex pos : "+ x + "," + y;

			//real valid input
			Vector2 worldPoint = new Vector2(Screen.width*x, Screen.height*y);
			
			//2. find game object by kinect input(2D coordinates)
			GameObject gameObject = findGameObject (worldPoint.x, worldPoint.y);
			GameObject colorSelector = findColorSelector(worldPoint.x, worldPoint.y);

			Debug.Log (gameObject.name);
			
			//3. call the onSpandexDown method
			if (gameObject != null){
				if(gameObject.name == "canvas"){
					//TODO Sandart
					drawingOnGUI canvasScript = gameObject.GetComponent<drawingOnGUI>();
					canvasScript.OnCanvasDown(worldPoint,minDepth); // TODO complete params
				}
				else if(colorSelector != null){
					GameObject.Find ("Control - Circular Color Picker").SendMessage("OnCanvasDown",worldPoint);
				}
				else
					gameObject.SendMessage ("OnCanvasDown");
				Debug.Log ("gameobject found : " + gameObject.name);
			}
			
			prevMinPtr = worldPoint; // for canvas up event
		}
		else if(pointCount == 2 && minDepth < errorRange && isDetected == true){ // End of canvas down
			isDetected = false;
			
			if(findGameObject(prevMinPtr.x,prevMinPtr.y).name == "canvas") // 'spandex canvas up' on 'drawing canvas'
				GameObject.Find("canvas").SendMessage("OnCanvasUp");
			
			prevMinPtr.Set (0,0);
		}
		
		
	}
}