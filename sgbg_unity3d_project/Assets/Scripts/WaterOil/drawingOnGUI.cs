using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices; // needed to import dll 
using System.IO;

public class drawingOnGUI : MonoBehaviour {

	// don't add file extension ".dll"
	[DllImport("mypaint_dll")]private static extern void mypaint_initWithImageSize(int img_width, int img_height);
	[DllImport("mypaint_dll")]private static extern void mypaint_release();
	
	[DllImport("mypaint_dll")]private static extern System.IntPtr mypaint_draw (float x, float y, float pressure, float xtilt, float ytilt, float dtime);
	[DllImport("mypaint_dll")]private static extern void mypaint_draw_end();

	[DllImport("mypaint_dll")]private static extern void mypaint_setColor(float r, float g, float b);
	[DllImport("mypaint_dll")]private static extern void mypaint_setTool (string toolName);
	[DllImport("mypaint_dll")]private static extern void mypaint_setRadius(float scale);
	[DllImport("mypaint_dll")]private static extern void mypaint_setHardness(float scale);
	
	[DllImport("mypaint_dll")]private static extern System.IntPtr  mypaint_getCanvas ();
	[DllImport("mypaint_dll")]private static extern void mypaint_setCanvas(short[] buffer);

	private Rect textureArea;
	private Vector2 leftTopPosition;
	private int canvasWidth;
	private int canvasHeight;

	private Texture2D texture;
	private short[] buffer;

	public struct UpdateData{
		/* dirty box (to be invalidated tiles) */
		public int x;
		public int y;
		public int width;
		public int height;
		/* pixel data */
		public System.IntPtr bufferPtr;
		public int bufferSize;
	}

	private ToolType currentTool;
	private Color currentColor;
	private float radiusScale;
	private float hardnessScale;

	private bool isStarted = false;
	private bool isCanvasChanged = false;
	private string canvasID;

	//for undo/redo
	private System.Collections.Generic.Stack<CanvasSnapshot> undoStack;
	private System.Collections.Generic.Stack<CanvasSnapshot> redoStack;
	// Use this for initialization
	void Start () {

		if(isStarted == true)
			return;

		canvasID = string.Empty; // to check new canvas or old canvas?

		Debug.Log ("watoeoil start");
		isStarted = true; // prevent to duplicate start method

		/* unity data initializaion*/
		//canvas position, size (GUI texture) are hard-corded. Constants are based on 800 x 600 resolution
		leftTopPosition = new Vector2 (Screen.width*0.17f,Screen.height*0.223f);

		canvasWidth = (int)(Screen.width*0.663f);
		canvasHeight = (int)(Screen.height*0.6f);

		textureArea = new Rect (leftTopPosition.x,
		                       leftTopPosition.y,
		                       canvasWidth,
		                       canvasHeight);

		texture = new Texture2D (canvasWidth, canvasHeight,TextureFormat.RGBA32, false);

		//set texture color to white
		for(int i = 0 ; i < canvasWidth ; i++)
			for(int j = 0 ; j < canvasHeight ; j++)
				texture.SetPixel(i,j,new Color(1.0f,1.0f,1.0f,0.5f));

		texture.Apply (false);

		currentTool = ToolType.pencil; // TODO check this line
		currentColor = new Color (0, 0, 0); // initial drawing color is black
		radiusScale = 0.0f;
		hardnessScale = 0.0f;

		/* mypaint dll initializaion*/
		mypaint_initWithImageSize (canvasWidth, canvasHeight); //initialize mypaint drawing alogrithm
		mypaint_setColor (currentColor.r, currentColor.g, currentColor.b);

		undoStack = new System.Collections.Generic.Stack<CanvasSnapshot>();
		redoStack = new System.Collections.Generic.Stack<CanvasSnapshot>();

		Texture2D deepCopiedTex = Instantiate (texture) as Texture2D;

		System.IntPtr img = mypaint_getCanvas ();
		const int tileSizePixel = 64;
		int tileWidth = Mathf.CeilToInt ((float)canvasWidth / tileSizePixel);
		int tileHeight = Mathf.CeilToInt ((float)canvasHeight / tileSizePixel);
		int tileSize = tileSizePixel * tileSizePixel * 4;// * sizeof(System.UInt16);
		int bufferSize = tileWidth * tileHeight * tileSize; 
		
		short[] buf = new short[bufferSize];

		buffer = new short[bufferSize]; // used in water oil space

		Marshal.Copy(img,buf,0,bufferSize);
		undoStack.Push (new CanvasSnapshot(deepCopiedTex,buf)); // push base canvas image


		OnToolChange (currentTool);
	}

	private void setTextureToMypaint(short[] buf){
//		Color[] canvasRGBA = texture.GetPixels(0);
//		System.UInt16[] rgba = new System.UInt16[canvasWidth*canvasHeight*4];
//		
//		int arrayIndex = 0;
//		for(int i = 0 ; i < canvasRGBA.Length ; i++){
//			rgba[arrayIndex]   = (System.UInt16)(canvasRGBA[i].r*65535/2.0f);
//			rgba[arrayIndex+1] = (System.UInt16)(canvasRGBA[i].g*65535/2.0f);
//			rgba[arrayIndex+2] = (System.UInt16)(canvasRGBA[i].b*65535/2.0f);
//			rgba[arrayIndex+3] = (System.UInt16)(canvasRGBA[i].a*65535/1.5f);
//			arrayIndex += 4;
//		}
		
		mypaint_setCanvas(buf);
	}

	private Vector2 valueToPoint(int index,int row){
		Vector2 point = new Vector2();
		int x = index % row;
		int y = index / row;
		
		point.Set (x, y);
		
		return point;
	}

	private void updateTexture(UpdateData updateData){
		int startX = updateData.x;
		int startY = updateData.y;
		int w = updateData.width;
		int h = updateData.height;

//		if(buffer == null)
//			buffer = new int[canvasWidth*canvasHeight*4]; // 4 means the number of chanels (rgba)

		System.IntPtr img = mypaint_getCanvas ();

		const int tileSizePixel = 64;
		int tileWidth = Mathf.CeilToInt ((float)canvasWidth / tileSizePixel);
		int tileHeight = Mathf.CeilToInt ((float)canvasHeight / tileSizePixel);

		int tileSize = tileSizePixel * tileSizePixel * 4;// * sizeof(System.UInt16);
		int bufferSize = tileWidth * tileHeight * tileSize; 

		if(buffer == null)
			buffer = new short[bufferSize];

//		Debug.Log ("screen width : " + canvasWidth + " , screen height : " + canvasHeight);
//		Debug.Log ("tileWidth : " + tileWidth + " , tileHeight : " + tileHeight);
//		Debug.Log ("tileSize : " + tileSize + " , bufferSize : " + bufferSize);

		Marshal.Copy(img,buffer,0,bufferSize);
		//

		for(int j = startY; j < startY + h; j++)
			for(int i = startX; i < startX + w ; i++){
				//int index = j * canvasWidth * 4 + i * 4;

			int index = 4 * (((
				j / tileSizePixel * tileWidth +
				i / tileSizePixel) * tileSizePixel +
			                  j % tileSizePixel) * tileSizePixel +
			                 i % tileSizePixel);

			//TODO handle array index exception in bottom edge 
			Vector2 indexToXY = valueToPoint(index,canvasWidth);
			
			if(i < 0 || i >= canvasWidth || j < 0 || j >= canvasHeight)
				continue;

			texture.SetPixel(i,j,new Color((float)(System.UInt16)buffer[index]/65535*2.1f, // 2.0f means each chanel is doubled
			                               (float)(System.UInt16)buffer[index+1]/65535*2.1f,
			                               (float)(System.UInt16)buffer[index+2]/65535*2.1f,
			                               (float)(System.UInt16)buffer[index+3]/65535*3.5f)
			                 );
			}

		texture.Apply (false);
	}

	public void OnCanvasDown(Vector2 point, short minDepth){ // TODO change parameter to use depth, size, position
		if(isCanvasChanged == false)
			isCanvasChanged = true;

		Vector2 mousePosition = new Vector2(point.x, Screen.height - point.y);

		if(textureArea.Contains(mousePosition)){
			//TODO set radius , hardness by depth, size input that came from kinect and delta time, tilt
			/*
			 * mypaint_setRadius(...)
			 * mypaint_setHardness() << only if water color, oil brush
			 * 	water color -> 0.4 ~ 1.0
			 * 	oil			-> 0.1 ~ 0.8
			 */
			float depthRatio = ((float)minDepth-15)/70; // 70mm is max hardness , 15mm is possible error range
			float radiusRatio = ((float)minDepth-15)/70;

			if(depthRatio > 1.0f)
				depthRatio = 1.0f;

			if(radiusRatio > 1.0f)
				radiusRatio = 1.0f;

			float hardness = 0;

			if(currentTool == ToolType.waterbrush){
				hardness = 0.1f + 0.6f*depthRatio;
				mypaint_setHardness(hardness);
			}
			else if(currentTool == ToolType.oilbrush){
				hardness = 0.1f + 0.7f*depthRatio;
				mypaint_setHardness(hardness);
			}else if(currentTool == ToolType.pencil){
				radiusRatio = 0;
			}

			mypaint_setRadius(1.0f + 2.0f*radiusRatio);
			//mypaint_setHardness(hardness);

			System.IntPtr updateDataPtr = mypaint_draw(mousePosition.x - leftTopPosition.x,
			                                    canvasHeight - (mousePosition.y - leftTopPosition.y),
			                                    1.0f,0.0f,0.0f,Time.deltaTime);
			
			UpdateData updateData = (UpdateData) Marshal.PtrToStructure(updateDataPtr,typeof(UpdateData));
			updateTexture(updateData);
			
		}
	}

	public void OnCanvasUp(){
		Debug.Log ("OnCanvasUp");
		mypaint_draw_end();

		// notify the change of color to palette
		palette target = GameObject.Find("pallete").GetComponent<palette>();
		target.OnPaintAdd (currentColor);

		//TODO limit maximum number of undo

		//Do deep copy
		Texture2D deepCopiedTex = Instantiate (texture) as Texture2D;
		short[] deepCopidedBuf = buffer.Clone () as short[];

		//push current canvas image to undo stack
		undoStack.Push (new CanvasSnapshot(deepCopiedTex,deepCopidedBuf));

		//clear redo stack
		redoStack.Clear ();
	}

	public void OnCanvasDelete(){
		isCanvasChanged = false;

		mypaint_release ();

		mypaint_initWithImageSize (canvasWidth, canvasHeight); //initialize mypaint drawing alogrithm
		
		mypaint_setColor (currentColor.r, currentColor.g, currentColor.b);
		OnToolChange (currentTool);

		//get image buffer
		System.IntPtr img = mypaint_getCanvas ();
		
		const int tileSizePixel = 64;
		int tileWidth = Mathf.CeilToInt ((float)canvasWidth / tileSizePixel);
		int tileHeight = Mathf.CeilToInt ((float)canvasHeight / tileSizePixel);
		
		int tileSize = tileSizePixel * tileSizePixel * 4;// * sizeof(System.UInt16);
		int bufferSize = tileWidth * tileHeight * tileSize; 
		
		if(buffer == null)
			buffer = new short[bufferSize];
		
		Marshal.Copy(img,buffer,0,bufferSize);
		//end of getting image buffer

		//set color of unity side texture to white
		for(int i = 0 ; i < canvasWidth ; i++)
			for(int j = 0 ; j < canvasHeight ; j++)
				texture.SetPixel(i,j,new Color(1.0f,1.0f,1.0f,1.0f));

		//undo,redo clear
		undoStack.Clear ();
		Texture2D deepCopiedTex = Instantiate (texture) as Texture2D;
		short[] deepCopiedBuf = buffer.Clone () as short[];
		undoStack.Push (new CanvasSnapshot(deepCopiedTex,deepCopiedBuf)); //todo getcanvas
		redoStack.Clear ();

		texture.Apply (false);
	}

	public void OnToolChange(ToolType toolType){
		currentTool = toolType;

		switch (toolType) {
		case ToolType.waterbrush:
			mypaint_setTool("WATER_COLOR_BRUSH");
			break;
		case ToolType.oilbrush:
			mypaint_setTool("OIL_BRUSH");
			break;
		case ToolType.pencil:
			mypaint_setTool("PENCIL");
			break;
		case ToolType.knife:
			mypaint_setTool("KNIFE");
			break;
		case ToolType.eraser:
			mypaint_setTool("ERASER");
			break;
		}

		if (currentTool == ToolType.eraser) // don't change color
			return;
		//set drawing color after setting tool
		mypaint_setColor(currentColor.r, currentColor.g, currentColor.b);
	}

	// this function is called by color picker
	public void OnColorChange(){
		currentColor = UIColorPicker.current.value;
		if (currentTool == ToolType.eraser) // don't change color
			return;
		mypaint_setColor(currentColor.r, currentColor.g, currentColor.b);
	}

	//this function is called by game object like paints, not color picker
	public void OnColorChange(Color color){
		currentColor = color;
		if (currentTool == ToolType.eraser) // don't change color
			return;
		mypaint_setColor(currentColor.r, currentColor.g, currentColor.b);
	}

	public void OnUndo(){

		if(undoStack.Count == 1) // undo stack only have base canvas image
			return;

		CanvasSnapshot previousCanvas = undoStack.Pop ();
		CanvasSnapshot currentCanvas = undoStack.Peek ();

		this.texture = Instantiate (currentCanvas.CanvasTex) as Texture2D; // make clone of texture to perform deep copy
		redoStack.Push (previousCanvas); // there is no need to deep copy. b/c previousCanvas is popped.

		setTextureToMypaint (currentCanvas.CanvasBuf);
		//TODO set timer

		this.texture.Apply (false);
	}

	public void OnRedo(){
		if(redoStack.Count == 0)
			return;
		CanvasSnapshot currentCanvas = redoStack.Pop();

		this.texture = Instantiate (currentCanvas.CanvasTex) as Texture2D;
		undoStack.Push (currentCanvas);

		setTextureToMypaint (currentCanvas.CanvasBuf);
		//TODO set timer

		this.texture.Apply (false);

	}

	public Texture2D GetCanvasTex(){
//		Debug.Log ("10,10 : r : " + texture.GetPixel (10, 10).r + " , g : " + texture.GetPixel (10, 10).g
//						+ " , b : " + texture.GetPixel (10, 10).b + " , a : " + texture.GetPixel (10, 10).a);
		//set transparent pixel to white
		for(int i = 0 ; i < texture.height ; i++){
			for(int j = 0 ; j < texture.width; j++){
				if(texture.GetPixel(j,i).a == 0.5019608f){
					texture.SetPixel(j,i,new Color(255,255,255,255));
				}
			}

		}
		return texture;
	}

	public short[] getCanvasBuf(){
		return buffer;
	}

	public bool isCanvasChange(){
		return isCanvasChanged;
	}

	public string getCanvasID(){
		return canvasID;
	}

	public void init(string fileName){
		Debug.Log ("init");
		Start ();

		//check canvasID, baseImg for undo
		canvasID = fileName.Substring (fileName.Length - 18, 14); // 2014~~~

		string filePath = Application.dataPath + "/galleryData/" + fileName;

		if (File.Exists(filePath))
		{
			Debug.Log (filePath+" loading");
			using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
			{
				for(int i = 0; i < buffer.Length ; i++){
					buffer[i] = reader.ReadInt16();
				}
			}
		}

		//set buffer to mypaint
		mypaint_setCanvas (buffer.Clone() as short[]);

		//refresh canvas by mypaint buffer
		UpdateData ud = new UpdateData();
		ud.x = 0;
		ud.y = 0;
		ud.width = canvasWidth;
		ud.height = canvasHeight;

		updateTexture (ud);

		Texture2D deepCopiedTex = Instantiate (texture) as Texture2D;
		undoStack.Clear ();
		undoStack.Push (new CanvasSnapshot(deepCopiedTex, buffer.Clone() as short[]));
	}
	// Update is called once per frame
	void Update () {
		// mouse drawing - only used for debugging
		if(Input.GetMouseButton(0)){
			Vector2 mousePosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			OnCanvasDown(mousePosition,0);
		}
		else if(textureArea.Contains(Input.mousePosition) && Input.GetMouseButtonUp(0)){ // to stable
			OnCanvasUp();
		}

		if(Input.GetKeyDown(KeyCode.C)){
			mypaint_setTool("SAND");
			mypaint_setColor(87.0f/255.0f,49.0f/255.0f,0f);
		}
		else if(Input.GetKeyDown(KeyCode.D)){
			mypaint_setTool("WATER_COLOR_BRUSH");
			mypaint_setTool("SAND_ERASER");
		}
		else if(Input.GetKeyDown(KeyCode.R)){
			Debug.Log("r");
			radiusScale += 0.002f;
			mypaint_setRadius(radiusScale);
		}
		else if(Input.GetKeyDown(KeyCode.H)){
			hardnessScale += 0.02f;
			Debug.Log("hard : " + hardnessScale);
			mypaint_setHardness(hardnessScale);
		}
		else if (Input.GetKeyDown(KeyCode.U)){
			Debug.Log ("u : " + undoStack.Count +" , r : " + redoStack.Count);
		}
		else if(Input.GetKeyDown(KeyCode.T)){
			OnRedo();
		}
	}

	void OnGUI(){
		GUI.DrawTexture (textureArea, texture);
	}

	void OnDestory(){
		mypaint_release ();
	}
}
