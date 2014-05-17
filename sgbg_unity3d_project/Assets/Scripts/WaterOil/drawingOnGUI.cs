using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices; // needed to import dll 
//using System.Windows.Forms;

public class drawingOnGUI : MonoBehaviour {

	// don't add file extension ".dll"
	[DllImport("mypaint_dll")]private static extern int applyTexture(System.IntPtr texPtr);

	[DllImport("mypaint_dll")]private static extern void mypaint_initWithImageSize(int img_width, int img_height);
	[DllImport("mypaint_dll")]private static extern void mypaint_release();
	
	[DllImport("mypaint_dll")]private static extern System.IntPtr mypaint_draw (float x, float y, float pressure, float xtilt, float ytilt, float dtime);
	[DllImport("mypaint_dll")]private static extern void mypaint_draw_end();

	[DllImport("mypaint_dll")]private static extern void mypaint_setColor(float r, float g, float b);
	[DllImport("mypaint_dll")]private static extern void mypaint_setTool (string toolName);
	[DllImport("mypaint_dll")]private static extern void mypaint_setRadius(float scale);
	[DllImport("mypaint_dll")]private static extern void mypaint_setHardness(float scale);
	
	[DllImport("mypaint_dll")]private static extern System.IntPtr  mypaint_getCanvas ();
	[DllImport("mypaint_dll")]private static extern void mypaint_setCanvas(System.UInt16[] buffer);

	private Rect textureArea;
	private Vector2 leftTopPosition;
	private int width;
	private int height;

	private Texture2D texture;
	private int[] buffer;

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
	private System.Collections.Generic.Stack<Texture2D> undoStack;
	private System.Collections.Generic.Stack<Texture2D> redoStack;
	// Use this for initialization
	void Start () {
		/* unity data initializaion*/
		//canvas position, size (GUI texture) are hard-corded. Constants are based on 800 x 600 resolution
		leftTopPosition = new Vector2 (Screen.width*0.17f,Screen.height*0.223f);

		width = (int)(Screen.width*0.663f);
		height = (int)(Screen.height*0.6f);

		textureArea = new Rect (leftTopPosition.x,
		                       leftTopPosition.y,
		                       width,
		                       height);

		texture = new Texture2D (width, height,TextureFormat.RGBA32, false);

		//set texture color to white
		for(int i = 0 ; i < width ; i++)
			for(int j = 0 ; j < height ; j++)
				texture.SetPixel(i,j,new Color(1.0f,1.0f,1.0f,0.5f));

		texture.Apply (false);

		currentTool = ToolType.pencil; // TODO check this line
		currentColor = new Color (0, 0, 0); // initial drawing color is black
		radiusScale = 1.0f;
		hardnessScale = 1.0f;

		undoStack = new System.Collections.Generic.Stack<Texture2D>();
		undoStack.Push (Instantiate(this.texture) as Texture2D); // push base canvas image
		redoStack = new System.Collections.Generic.Stack<Texture2D>();

		/* mypaint dll initializaion*/
		mypaint_initWithImageSize (width, height); //initialize mypaint drawing alogrithm

		mypaint_setColor (currentColor.r, currentColor.g, currentColor.b);
		OnToolChange (currentTool);
	}

	private void setTextureToMypaint(){
		Color[] canvasRGBA = texture.GetPixels(0);
		System.UInt16[] rgba = new System.UInt16[width*height*4];
		
		int arrayIndex = 0;
		for(int i = 0 ; i < canvasRGBA.Length ; i++){
			rgba[arrayIndex]   = (System.UInt16)(canvasRGBA[i].r*65535/2.0f);
			rgba[arrayIndex+1] = (System.UInt16)(canvasRGBA[i].g*65535/2.0f);
			rgba[arrayIndex+2] = (System.UInt16)(canvasRGBA[i].b*65535/2.0f);
			rgba[arrayIndex+3] = (System.UInt16)(canvasRGBA[i].a*65535/1.5f);
			arrayIndex += 4;
		}
		
		mypaint_setCanvas(rgba);
	}

	private void updateTexture(UpdateData updateData){
		int startX = updateData.x;
		int startY = updateData.y;
		int w = updateData.width;
		int h = updateData.height;

		if(buffer == null)
			buffer = new int[width*height*4]; // 4 means the number of chanels (rgba)

		System.IntPtr img = mypaint_getCanvas ();
		Marshal.Copy(img,buffer,0,width*height*4);

		for(int j = startY; j < startY + h; j++)
			for(int i = startX; i < startX + w ; i++){
				int index = j * width * 4 + i * 4;

			//TODO handle array index exception in edge 

			texture.SetPixel(i,j,new Color((float)buffer[index]/65535*2.0f, // 2.0f means each chanel is doubled
			                               (float)buffer[index+1]/65535*2.0f,
			                               (float)buffer[index+2]/65535*2.0f,
			                               (float)buffer[index+3]/65535*1.5f)
			                 );
			}

		texture.Apply (false);
	}

	public void OnCanvasDown(Vector2 point){ // TODO change parameter to use depth, size, position
		Vector2 mousePosition = new Vector2(point.x, Screen.height - point.y);

		if(textureArea.Contains(mousePosition)){
			//TODO set radius , hardness by depth, size input that came from kinect and delta time, tilt
			System.IntPtr updateDataPtr = mypaint_draw(mousePosition.x - leftTopPosition.x,
			                                    height - (mousePosition.y - leftTopPosition.y),
			                                    1.0f,0.0f,0.0f,1/10);
			
			UpdateData updateData = (UpdateData) Marshal.PtrToStructure(updateDataPtr,typeof(UpdateData));
			updateTexture(updateData);
			
		}
	}

	public void OnCanvasUp(){
		mypaint_draw_end();

		// notify the change of color to palette
		palette target = GameObject.Find("pallete").GetComponent<palette>();
		//target.clr = currentColor;
		target.OnPaintAdd (currentColor);

		//push current canvas image to undo stack
		//TODO limit maximum number of undo
		undoStack.Push (Instantiate(this.texture) as Texture2D);
		Debug.Log ("undo stack push " + undoStack.Count);
		//clear redo stack
		redoStack.Clear ();
	}

	public void OnCanvasDelete(){
		mypaint_release ();

		mypaint_initWithImageSize (width, height); //initialize mypaint drawing alogrithm
		
		mypaint_setColor (currentColor.r, currentColor.g, currentColor.b);
		OnToolChange (currentTool);

		//set color of unity side texture to white
		for(int i = 0 ; i < width ; i++)
			for(int j = 0 ; j < height ; j++)
				texture.SetPixel(i,j,new Color(1.0f,1.0f,1.0f,1.0f));

		//undo,redo clear
		undoStack.Clear ();
		undoStack.Push (Instantiate (this.texture) as Texture2D);
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
		Texture2D previousCanvas = undoStack.Pop ();
		Texture2D currentCanvas = undoStack.Peek ();

		this.texture = Instantiate (currentCanvas) as Texture2D; // make clone of texture performing deep copy
		redoStack.Push (previousCanvas); // there is no need to deep copy. b/c previousCanvas is popped.

		//TODO set mypaint canvas
		setTextureToMypaint ();
		//TODO timer

		this.texture.Apply (false);

	}

	public void OnRedo(){
		if(redoStack.Count == 0)
			return;
		Texture2D currentCanvas = redoStack.Pop();

		this.texture = Instantiate (currentCanvas) as Texture2D;
		undoStack.Push (currentCanvas);

		//TODO set mypaint canvas
		setTextureToMypaint ();
		//TODO timer

		this.texture.Apply (false);

	}
	// Update is called once per frame
	void Update () {
		// mouse drawing - only used for debugging
		if(Input.GetMouseButton(0)){
			Vector2 mousePosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			OnCanvasDown(mousePosition);
		}
		else if(textureArea.Contains(Input.mousePosition) && Input.GetMouseButtonUp(0)){ // to stable
			OnCanvasUp();
		}

		if(Input.GetKeyDown(KeyCode.C)){
			//setTextureToMypaint();
		}
		else if(Input.GetKeyDown(KeyCode.R)){
			Debug.Log("r");
			radiusScale += 0.002f;
			mypaint_setRadius(radiusScale);
		}
		else if(Input.GetKeyDown(KeyCode.H)){
			hardnessScale += 0.02f;
			mypaint_setHardness(hardnessScale);
		}
		else if (Input.GetKeyDown(KeyCode.U)){
			OnUndo ();
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
