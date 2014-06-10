using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices; // needed to import dll 

public class DMTD : MonoBehaviour {
	
	private static DMTD _instance;
	
	struct StSendData{
		public int		iSymbolID;
		public float	fSymbolSize;
		public float	fSymbolDepth;
		public float	fSymbolPosX;
		public float	fSymbolPosY;
		public float	fAngle;
	}
	
	int				iRecvCnt;
	StSendData[]	stRecvData = new StSendData[10];
	[DllImport("DMTD")]private static extern void fnDMTDCreate();
	[DllImport("DMTD")]private static extern void fnDMTDDestroy();
	[DllImport("DMTD")]private static extern bool fnDMTD_GetMarkerChanged();
	[DllImport("DMTD")]private static extern int fnDMTD_GetMarkerPos( StSendData[] stRecvData );
	
	private bool isDetected = false;
	private Vector2 prevMinPtr;
	
	private bool isOn = false;
	private bool isOnCanvas = false;

	private const int MAX_POINT = 3;
	
	
	// Use this for initialization
	void Start () {
		fnDMTDCreate ();
		isOn = true;

	}
	
	// Update is called once per frame
	void Update () {
		if(isOn){
			if (fnDMTD_GetMarkerChanged ()) {
				iRecvCnt = fnDMTD_GetMarkerPos( stRecvData );
				//Debug.Log( "Get Data!! - " + iRecvCnt )

				if(iRecvCnt > MAX_POINT) // limit point
					iRecvCnt = MAX_POINT;

				for(int i = 0 ; i < iRecvCnt ; i++){
					//Debug.Log ("pos : " + stRecvData[i].fSymbolPosX + " , " + stRecvData[0].fSymbolPosY);
					//Debug.Log ("ID : " + stRecvData[i].iSymbolID);
					callGameObjByKinect(stRecvData[i]);
				}

				// Run
//				if(iRecvCnt != 0)
//					callGameObjByKinect(stRecvData);
//				else{
				if(iRecvCnt == 0 && isDetected == true){
					isDetected = false;
					
					if(findGameObject(prevMinPtr.x,prevMinPtr.y).name == "canvas"){ // 'spandex canvas up' on 'drawing canvas'
						GameObject.Find("canvas").SendMessage("OnCanvasUp");
					}
					
					prevMinPtr.Set (0,0);
				}
//				}

			}
			if(Input.GetKeyDown(KeyCode.T))
				fnDMTDDestroy();
		}
		
		if(Input.GetKeyDown(KeyCode.S)){
			fnDMTDCreate();
			isOn = true;
		}
		
	}
	
	public static DMTD Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<DMTD>();
				
				if(!_instance){
					GameObject container = new GameObject();
					container.name = "DMTDContainer";
					_instance = container.AddComponent(typeof(DMTD)) as DMTD;
				}
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad(_instance.gameObject);
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
			Screen.SetResolution(1200,900,true);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
//			if(this != _instance)
//				Destroy(this.gameObject);
		}
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
	
	private void callGameObjByKinect(StSendData kinectInput)
	{
		
		if(isDetected == false) // for canvas up event
			isDetected = true;
		
		float x,y;
		
		//find x,y ratio
		x = kinectInput.fSymbolPosX; 
		y = kinectInput.fSymbolPosY; 
		
		// coordinates transformation from kinect to unity
		y = 1 - y;
		
		// adjust calibration manually 
		//y -= 0.03f; 
		
		//real valid input
		Vector2 worldPoint = new Vector2(Screen.width*x, Screen.height*y);
		
		//2. find game object by kinect input(2D coordinates)
		GameObject gameObject = findGameObject (worldPoint.x, worldPoint.y);
		GameObject colorSelector = findColorSelector(worldPoint.x, worldPoint.y);
		
		//Debug.Log (gameObject.name);
		
		//3. call the onSpandexDown method
		if (gameObject != null){
			if(gameObject.name == "canvas"){
				isOnCanvas = true;
				//TODO Sandart
				drawingOnGUI canvasScript = gameObject.GetComponent<drawingOnGUI>();
				if(canvasScript!= null)
					canvasScript.OnCanvasDown(worldPoint,(short)kinectInput.fSymbolDepth); // TODO complete params
				else{
					Sandart sandScript = gameObject.GetComponent<Sandart>();
					sandScript.OnCanvasDown(worldPoint,(short)kinectInput.fSymbolDepth);
				}

			}
			else if(colorSelector != null){
				GameObject.Find ("Control - Circular Color Picker").SendMessage("OnCanvasDown",worldPoint);
				if(isOnCanvas == true){
					isOnCanvas = false;
					GameObject.Find ("canvas").SendMessage("OnCanvasUp");
				}
			}
			else{
				if(isOnCanvas == true){
					isOnCanvas = false;
					GameObject.Find ("canvas").SendMessage("OnCanvasUp");
				}
				gameObject.SendMessage ("OnCanvasDown");
			}
			Debug.Log ("gameobject found : " + gameObject.name);

		}

		prevMinPtr = worldPoint; // for canvas up event
		
		//		else if(pointCount == 2 && minDepth < errorRange && isDetected == true){ // End of canvas down
		//			isDetected = false;
		//			
		//			if(findGameObject(prevMinPtr.x,prevMinPtr.y).name == "canvas") // 'spandex canvas up' on 'drawing canvas'
		//				GameObject.Find("canvas").SendMessage("OnCanvasUp");
		//			
		//			prevMinPtr.Set (0,0);
		//		}
	}
	
	//	void OnDestroy(){
	//		fnDMTDDestroy();
	//	}
	
	void OnApplicationQuit() {
		//fnDMTDDestroy();
	}
	
}
