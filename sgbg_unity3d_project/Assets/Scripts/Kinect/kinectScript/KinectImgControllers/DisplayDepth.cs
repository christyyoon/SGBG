using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent(typeof(Renderer))]
public class DisplayDepth : MonoBehaviour {
	
	public DepthWrapper dw;
	public GUIText text; // GUI text to show the minimum value of depth (for testing)
	public GUITexture guiT;
	
	private short minDepth; // minimum depth in depth buffer
	private Vector2 minPoint;
	private Texture2D tex;
	private short[] depthSnapshot;
	private Vector2[] ROIVertex = new Vector2[2]; // store leftTop, rightBottom point
	private int pointCount = 0;
	
	private bool isDetected = true;

	private bool isStable = true;
	private bool swOn = false;
	private short outputCount = 0;
	private StreamWriter sw;
	private short errorRange = 12; // range of vibration degree


	private ArrayList actionData = new ArrayList();

	#region
	// Use this for initialization
	void Start () {
		tex = new Texture2D(320,240,TextureFormat.ARGB32,false);
		//renderer.material.mainTexture = tex;
		//minPoint.Set (0, 0); // initialize the min coordinate
	}
	#endregion
	
	private void arrayCopy(short[] des, short[] src){// for deep copy
		for (int i = 0; i < des.Length; i++) {
			des[i] = src[i];
		}
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
//		Debug.Log ("mouse pos  : " + Input.mousePosition.x + "," + Input.mousePosition.y);
			
		}
		if (Input.GetKeyDown (KeyCode.S)) { // key S means snapshot
			Debug.Log ("S key pressed!");
			
			// store a snapshot of depth image
			//depthSnapshot = (short[]) dw.depthImg.Clone(); // deep copy
			depthSnapshot = new short[320*240];
			arrayCopy(depthSnapshot,dw.depthImg);
			
			//initiate ROI vertics
			ROIVertex[0].Set(55,3);//left top
			ROIVertex[1].Set(310,195);//right bottom


			//initiate number of vertex
			pointCount = 2;


			
		}

		if (dw.pollDepth())
		{
			//Debug.Log();
			
			if(pointCount == 2){ //two point(LT, RB) were detected
				//drawRect(img,ROIVertex[0],ROIVertex[1]);
//				Vector2 screenPoint = findPointInROI(dw.depthImg);
//				if(screenPoint != null)
//					guiT.transform.position = new Vector3(screenPoint.x,screenPoint.y,0f);
				callGameObjByKinect(dw.depthImg);
			}else {

			}
			
			//tex.SetPixels32(img);
			
			//text.guiText.text ="minimum depth value : " + minDepth.ToString()+"\n"+
			//	"x : " + minPoint.y +", y : " + minPoint.x;
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
		Ray ray = camera.ScreenPointToRay(new Vector3(x,y,0));  
		RaycastHit hitObj;
		
		if (Physics.Raycast (ray, out hitObj, Mathf.Infinity)) {
			string hitObjName = hitObj.transform.name;
			
			return GameObject.Find (hitObjName);
		} else
			return null;
	}

	GameObject findColorSelector(float x, float y){
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
			//Color32[] img = new Color32[depthBuf.Length];
			minDepth = 0; // initialize mindepth per frame
			minPoint.Set (0, 0);
	
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
	
//		if(minDepth <= errorRange){ // stable state
//			if(isDetected == false){
//				Debug.Log("stable state");
//			}
//			isDetected = true;
//		}else{
//			
//		}
//		
//		if (minDepth > 30 && isDetected == true && pointCount < 2) {
//			Debug.Log((pointCount + 1)+" point detected");
//			ROIVertex [pointCount++] = minPoint;
//			
//			isDetected = false; // until canvas state turn back to stable state
//		} 
		//test//
		if(pointCount == 2 && minDepth > errorRange){
			float x,y;

			//find x,y ratio
			x = (minPoint.x-ROIVertex[0].x) / (ROIVertex[1].x - ROIVertex[0].x); 
			y = (minPoint.y-ROIVertex[0].y) / (ROIVertex[1].y - ROIVertex[0].y); // b/c kinect look at the canvas in reverse

			// coordinates transformation from kinect to unity
			y = 1 - y;

			if(guiT != null)
				guiT.transform.position = new Vector3(x,y,0f); 

			//y -= 0.06f; // adjust calibration manually 

			text.guiText.text = "spandex pos : "+x+ "," + y;

			Vector2 worldPoint = new Vector2(Screen.width*x, Screen.height*y);

			// TODO PERFOMANCE
			//2. find game object by kinect input(2D coordinates)
			GameObject gameObject = findGameObject (worldPoint.x, worldPoint.y);
			GameObject colorSelector = findColorSelector(worldPoint.x, worldPoint.y);

			if(colorSelector != null){
				GameObject.Find ("Control - Circular Color Picker").SendMessage("OnCanvasDown",worldPoint);
			}
			
			//3. call the onSpandexDown method
			if (gameObject != null){
				if(gameObject.name == "canvas")
					gameObject.SendMessage ("OnCanvasDown",worldPoint);
				else
					gameObject.SendMessage ("OnMouseDown");
				//Debug.Log ("gameobject found : " + gameObject.name);
			}


		}
		else if(pointCount == 2 && minDepth < errorRange){
			GameObject.Find("canvas").SendMessage("OnCanvasUp");
		}
	
 	
	}
}