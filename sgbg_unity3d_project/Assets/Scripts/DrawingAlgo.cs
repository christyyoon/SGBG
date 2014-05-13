using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawingAlgo : MonoBehaviour 
{
	#region painter
	public Texture2D sourceBaseTex;
	private Texture2D baseTex;
	
	public int screenWidth = Screen.width;
	public int screenHeight = Screen.height;
	void  Start ()
	{
		int width = screenWidth * 103 / 100;
		int height = screenHeight * 118 / 100;

		GameObject obj = GameObject.Find ("canvas");

		Debug.Log (screenWidth);
		sourceBaseTex.Resize (width, height);
		
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
				sourceBaseTex.SetPixel (i, j, Color.white);
		
		baseTex = (Texture2D) Instantiate( sourceBaseTex );
		//baseTex.Resize (300, 300);
	}
	
	private Vector2 dragStart;
	private Vector2 dragEnd;
	
	public enum Tool {
		None,
		Line,
		Brush,
		Eraser,
		Vector
	}
	
	private int tool2 = 1;
	
	public Samples AntiAlias = Samples.Samples4;
	
	public Tool tool = Tool.Brush;
	
	public Texture[] toolimgs;
	
	public Texture2D colorCircle;
	
	public float lineWidth = 1;
	
	public float strokeWidth = 1;
	
	public Color col = Color.black;
	
	public Color col2 = Color.black;
	
	public GUISkin gskin;
	
	public LineTool lineTool = new LineTool();
	
	public BrushTool brush = new BrushTool();
	
	public EraserTool eraser = new EraserTool();
	
	public Stroke stroke = new Stroke();
	
	public int zoom = 1;
	
	public ToolType currentTool;
	
	//public Transform paintt1;
	//public GameObject target;
	public palette target;
	public void OnColorChange()
	{
		col = UIColorPicker.current.value;
		//Debug.Log (col);
	}
	BezierPoint[] BezierPoints;
	
	void  OnGUI ()
	{
		GUI.skin = gskin;
		GUILayout.BeginArea ( new Rect(0,0,0,0)/*100,50,100+baseTex.width*zoom,baseTex.height*zoom)*/,"","box");
		//gray box behind the white board
		
		//		GUILayout.BeginArea ( new Rect(0,0,100,baseTex.height*zoom));  //없으면 hardness, size can't control
		//		
		//		//tool2 = GUILayout.Toolbar(tool2,toolimgs,"Tool"); //toolbar img
		//		
		//		//      tool = System.Enum.Parse (Tool,tool2.ToString ());
		//		
		//		// FIXME: Defaults to brush tool, fix enum parse above.
		//		
		tool = Tool.Brush;
		//		//GUILayout.Label ("Drawing Options");
		//		GUILayout.Space (10);
		switch (tool) 
		{
			////		case Tool.Line:
			////			GUILayout.Label ("Size "+ Mathf.Round (lineTool.width*10)/10);
			////			lineTool.width = GUILayout.HorizontalSlider (lineTool.width,0,40);
			////			col = RGBCircle (col,"",colorCircle);
			////			break;
			//			
		case Tool.Brush:
			//GUILayout.Label ("Size "+ Mathf.Round (brush.width*10)/10);
			///brush.width = GUILayout.HorizontalSlider (brush.width,0,40);
			//GUILayout.Label ("Hardness " + Mathf.Round (brush.hardness*10)/10);
			//brush.hardness = GUILayout.HorizontalSlider (brush.hardness,0.1f,50);
			//col = RGBCircle (col,"",colorCircle);
			//col = Color.red;  //color of pen
			//col = UIColorPicker.current.value;
			
			
			break;
			//			
			////		case Tool.Eraser:
			////			GUILayout.Label ("Size "+ Mathf.Round (eraser.width*10)/10);
			////			eraser.width = GUILayout.HorizontalSlider (eraser.width,0,50);
			////			GUILayout.Label ("Hardness " + Mathf.Round (eraser.hardness*10)/10);
			////			eraser.hardness = GUILayout.HorizontalSlider (eraser.hardness,1,50);
			////			break;
		}
		//		
		////		if (tool==Tool.Line) 
		////		{
		////			stroke.enabled = GUILayout.Toggle (stroke.enabled,"Stroke");						
		////			GUILayout.Label ("Stroke Width "+ Mathf.Round (stroke.width*10)/10);						
		////			stroke.width = GUILayout.HorizontalSlider (stroke.width,0,lineWidth);						
		////			GUILayout.Label ("Secondary Color");						
		////			col2 = RGBCircle (col2,"",colorCircle);
		////		}
		//		
		GUILayout.EndArea (); //white board end
		//
		GUI.DrawTexture ( new Rect(screenWidth/3.65f, screenHeight/2.23f ,baseTex.width*zoom,baseTex.height*zoom),baseTex);//white board location		
		//		GUILayout.EndArea ();
	}
	
	private Vector2 preDrag;
	
	void  Update ()
	{	
		
		//paintt1 = GetComponent<paint1>();
		//paintt1.renderer.material.color = UIColorPicker.current.value;
		Rect imgRect = new Rect(screenWidth/3.65f, screenHeight/2.23f ,baseTex.width*zoom,baseTex.height*zoom);
		//DrawTexture하고 맞춰줘야 마우스하고 	선하고 같은 위치에서 그림	
		
		Vector2 mouse = Input.mousePosition;		
		
		mouse.y = Screen.height-mouse.y;
		
		if (Input.GetKeyDown ("t")) 
		{					
			test ();
		}		
		
		if (Input.GetKeyDown ("mouse 0")) 
		{
			Debug.Log(Input.mousePosition.x + " , "+ Input.mousePosition.y);  
			if (imgRect.Contains (mouse)) 
			{
				if (tool==Tool.Vector) 
				{				
					var m2 = mouse-new Vector2(imgRect.x,imgRect.y);					
					
					m2.y = imgRect.height-m2.y;
					
					var bz = new ArrayList (BezierPoints);					
					
					bz.Add (new BezierPoint (m2,m2-new Vector2(50,10),m2+new Vector2(50,10)));
					
					BezierPoints = (BezierPoint[]) bz.ToArray();
					
					DrawBezier (BezierPoints,lineTool.width,col,baseTex);								
				}
				
				dragStart = mouse - new Vector2 (imgRect.x,imgRect.y);				
				
				dragStart.y =imgRect.height-dragStart.y;				
				
				dragStart.x = Mathf.Round (dragStart.x/zoom);				
				
				dragStart.y = Mathf.Round (dragStart.y/zoom);				
				
				//LineStart (mouse - Vector2 (imgRect.x,imgRect.y));
				
				dragEnd = mouse - new Vector2 (imgRect.x,imgRect.y);				
				
				dragEnd.x = Mathf.Clamp (dragEnd.x,0,imgRect.width);
				
				dragEnd.y = imgRect.height-Mathf.Clamp (dragEnd.y,0,imgRect.height);				
				
				dragEnd.x = Mathf.Round ( dragEnd.x/zoom);				
				
				dragEnd.y = Mathf.Round ( dragEnd.y/zoom);								
			} 
			
			else 
			{
				dragStart=Vector3.zero;
			}
		}
		
		if (Input.GetKey ("mouse 0")) 
		{					
			if (dragStart==Vector2.zero) 
			{
				return;								
			}
			
			dragEnd = mouse - new Vector2 (imgRect.x,imgRect.y);			
			
			dragEnd.x = Mathf.Clamp (dragEnd.x,0,imgRect.width);			
			
			dragEnd.y = imgRect.height-Mathf.Clamp (dragEnd.y,0,imgRect.height);			
			
			dragEnd.x = Mathf.Round ( dragEnd.x/zoom);			
			
			dragEnd.y = Mathf.Round ( dragEnd.y/zoom);
			
			if (tool==Tool.Brush) 
			{
				Brush (dragEnd,preDrag);
			}			
			
			if (tool==Tool.Eraser) 
			{
				Eraser (dragEnd,preDrag);
			}
		}		
		
		if (Input.GetKeyUp ("mouse 0") && dragStart != Vector2.zero) 
		{		
			GameObject targetobj = GameObject.Find("pallete");
			target=targetobj.GetComponent<palette>();
			target.clr=col;
			if (tool==Tool.Line) 
			{
				
				dragEnd = mouse - new Vector2 (imgRect.x,imgRect.y);				
				
				dragEnd.x = Mathf.Clamp (dragEnd.x,0,imgRect.width);				
				
				dragEnd.y = imgRect.height-Mathf.Clamp (dragEnd.y,0,imgRect.height);				
				
				dragEnd.x = Mathf.Round ( dragEnd.x/zoom);				
				
				dragEnd.y = Mathf.Round ( dragEnd.y/zoom);				
				
				Debug.Log ("Draw Line");				
				
				NumSamples=AntiAlias;
				
				if (stroke.enabled) 
				{										
					baseTex = DrawLine (dragStart,dragEnd,lineTool.width,col,baseTex,true,col2,stroke.width);					
					
				} 
				else 
				{
					baseTex = DrawLine (dragStart,dragEnd,lineTool.width,col,baseTex);
				}
			}
			
			dragStart=Vector2.zero;
			
			dragEnd=Vector2.zero;
			
			
		}
		
		preDrag = dragEnd;
	}
	
	public void getToolName(ToolType toolName)
	{
		currentTool = toolName;
		Debug.Log (currentTool.ToString () + "Drawing");
		
	}
	void  Brush ( Vector2 p1 ,  Vector2 p2  )
	{
		
		NumSamples=AntiAlias;		
		
		if (p2 == Vector2.zero) 
		{
			p2 = p1;
		}
		
		PaintLine (p1,p2,brush.width,col,brush.hardness,baseTex);
		
		baseTex.Apply ();
	}
	
	void  Eraser ( Vector2 p1 ,  Vector2 p2  )
	{
		NumSamples=AntiAlias;
		
		if (p2 == Vector2.zero) 
		{						
			p2 = p1;
		}
		
		PaintLine (p1,p2,eraser.width,Color.white,eraser.hardness,baseTex);		
		
		baseTex.Apply ();
	}
	
	void  test (){		
		
		float startTime = Time.realtimeSinceStartup;		
		
		var w = 100;				
		var h = 100;
		
		var p1 = new BezierPoint (new Vector2(10,0),new Vector2(5,20),new Vector2(20,0));
		
		var p2 = new BezierPoint (new Vector2(50,10),new Vector2(40,20),new Vector2(60,-10));		
		
		var c = new BezierCurve (p1.main,p1.control2,p2.control1,p2.main);
		
		p1.curve2=c;
		
		p2.curve1=c;
		
		Vector2 elapsedTime = new Vector2 ((Time.realtimeSinceStartup-startTime)*10,0);		
		
		float startTime2 = Time.realtimeSinceStartup;		
		
		for (var i=0;i<w*h;i++) 
		{
			IsNearBezier (new Vector2(Random.value*80,Random.value*30),p1,p2,10);
		}
		
		Vector2 elapsedTime2 = new Vector2 ((Time.realtimeSinceStartup-startTime2)*10,0);
		
		Debug.Log ("Drawing tool " + elapsedTime.ToString () + "  "+ elapsedTime2.ToString ());
		
	}
	
	public class LineTool 
	{				
		public float width = 1;
	}
	
	public class EraserTool 
	{
		public float width = 1;
		
		public float hardness = 1;
	}
	
	public class BrushTool 
	{
		public float width = 10;	//thickness of pen	
		
		public float hardness = 1;		//농도 fo pen
		
		public float spacing = 10;		
	}
	
	public class Stroke 
	{
		public bool enabled= false;
		
		public float width = 1;
	}
	
	
	#endregion
	
	
	#region gui controls
	
	//	
	//	static Color RGBSlider ( Color c ,  string label  )				
	//	{
	//		GUI.color=c;		
	//		
	//		GUILayout.Label (label);		
	//		
	//		GUI.color=Color.red;		
	//		
	//		c.r = GUILayout.HorizontalSlider (c.r,0,1);		
	//		
	//		GUI.color=Color.green;		
	//		
	//		c.g = GUILayout.HorizontalSlider (c.g,0,1);		
	//		
	//		GUI.color=Color.blue;		
	//		
	//		c.b = GUILayout.HorizontalSlider (c.b,0,1);		
	//		
	//		GUI.color=Color.white;
	//		
	//		return c;
	//	}
	//	
	//	static Color  RGBCircle ( Color c ,  string label ,  Texture2D colorCircle  ){		
	//		
	//		var r = GUILayoutUtility.GetAspectRect (1);		
	//		
	//		r.height = r.width -= 15;		
	//		
	//		var r2 = new Rect(r.x + r.width +5,r.y,10,r.height);		
	//		
	//		var hsb = new HSBColor (c);//It is much easier to work with HSB colours in this case
	//		
	//		var cp = new Vector2 (r.x+r.width/2,r.y+r.height/2);
	//		
	//		if (Input.GetMouseButton (0)) 
	//		{
	//			var InputVector= Vector2.zero;			
	//			
	//			InputVector.x = cp.x - Event.current.mousePosition.x;			
	//			
	//			InputVector.y = cp.y - Event.current.mousePosition.y;
	//			
	//			var hyp = Mathf.Sqrt( (InputVector.x * InputVector.x) + (InputVector.y * InputVector.y) );			
	//			
	//			if (hyp <= r.width/2 + 5) 
	//			{
	//				hyp = Mathf.Clamp (hyp,0,r.width/2);				
	//				
	//				float a = Vector3.Angle(new Vector3(-1,0,0), InputVector);
	//				
	//				if (InputVector.y<0) 
	//				{
	//					a = 360 - a;
	//				}
	//				
	//				hsb.h = a / 360;
	//				
	//				hsb.s = hyp / (r.width/2);				
	//				
	//			}
	//		}
	//		
	//		var hsb2 = new HSBColor (c);		
	//		
	//		hsb2.b = 1;		
	//		
	//		var c2 = hsb2.ToColor ();		
	//		
	//		GUI.color = c2;
	//		
	//		hsb.b = GUI.VerticalSlider (r2,hsb.b,1.0f,0.0f,"BWSlider","verticalsliderthumb");
	//		
	//		GUI.color = Color.white * hsb.b;		
	//		
	//		GUI.color = new Color( GUI.color.r, GUI.color.g, GUI.color.b, 1);		
	//		
	//		GUI.Box (r,colorCircle,GUIStyle.none);
	//		
	//		var pos = (new Vector2 (Mathf.Cos (hsb.h*360*Mathf.Deg2Rad),-Mathf.Sin (hsb.h*360*Mathf.Deg2Rad))*r.width*hsb.s/2);
	//		
	//		GUI.color = c;		
	//		
	//		GUI.Box ( new Rect(pos.x-5+cp.x,pos.y-5+cp.y,10,10),"","ColorcirclePicker");		
	//		
	//		GUI.color = Color.white; //그리는곳 바탕색 
	//		
	//		c = hsb.ToColor ();		
	//		
	//		return c;
	//	}
	//	
	#endregion
	
	
	#region drawing
	
	public enum Samples 
	{				
		None,			
		Samples2,
		Samples4,
		Samples8,
		Samples16,
		Samples32,
		RotatedDisc
	}
	
	static Samples NumSamples = Samples.Samples4;
	
	static Texture2D  DrawLine ( Vector2 from ,  Vector2 to ,  float w ,  Color col ,  Texture2D tex  )
	{
		return DrawLine (from,to,w,col,tex,false,Color.black,0);
	}
	
	static Texture2D  DrawLine (Vector2 from, Vector2 to, float w, Color col, Texture2D tex,bool stroke,Color strokeCol, float strokeWidth)
	{
		w = Mathf.Round (w);//It is important to round the numbers otherwise it will mess up with the texture width
		
		strokeWidth = Mathf.Round (strokeWidth);
		
		var extent = w + strokeWidth;		
		
		var stY = Mathf.Clamp (Mathf.Min (from.y,to.y)-extent,0,tex.height);//This is the topmost Y value		
		
		var stX =  Mathf.Clamp (Mathf.Min (from.x,to.x)-extent,0,tex.width);		
		
		var endY = Mathf.Clamp (Mathf.Max (from.y,to.y)+extent,0,tex.height);		
		
		var endX = Mathf.Clamp (Mathf.Max (from.x,to.x)+extent,0,tex.width);//This is the rightmost Y value
		
		strokeWidth = strokeWidth/2;		
		
		var strokeInner = (w-strokeWidth)*(w-strokeWidth);		
		
		var strokeOuter = (w+strokeWidth)*(w+strokeWidth);
		
		var strokeOuter2 = (w+strokeWidth+1)*(w+strokeWidth+1);		
		
		var sqrW = w*w;//It is much faster to calculate with squared values
		
		var lengthX = endX-stX;	
		
		var lengthY = endY-stY;		
		
		var start = new Vector2 (stX,stY);
		
		Color[] pixels = tex.GetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, 0);//Get all pixels
		
		for (int y=0;y<lengthY;y++) 
		{		
			for (int x=0;x<lengthX;x++) 
			{//Loop through the pixels
				var p = new Vector2 (x,y) + start;				
				
				var center = p + new Vector2(0.5f,0.5f);				
				
				float dist = (center-NearestPointStrict(from,to,center)).sqrMagnitude;//The squared distance from the center of the pixels to the nearest point on the line				
				
				if (dist<=strokeOuter2) 
				{
					var samples = Sample (p);
					
					var c = Color.black;
					
					var pc = pixels[y*(int)lengthX+x];					
					
					for (int i=0;i<samples.Length;i++) 
					{//Loop through the samples
						dist = (samples[i]-NearestPointStrict(from,to,samples[i])).sqrMagnitude;//The squared distance from the sample to the line						
						
						if (stroke) 
						{
							if (dist<=strokeOuter && dist >= strokeInner) 
							{
								c+=strokeCol;																
							} 
							else if (dist<sqrW) 
							{
								c+=col;
							} 
							else 
							{	
								c+=pc;	
							}							
							
						} 
						else 
						{														
							if (dist<sqrW) 
							{//Is the distance smaller than the width of the line
								c+=col;
							}
							else 
							{
								c+=pc;//No it wasn't, set it to be the original colour
							}
						}
					}
					
					c /= samples.Length;//Get the avarage colour
					
					pixels[y * (int)lengthX + x] = c;
				}								
			}
		}		
		
		tex.SetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, pixels, 0);		
		
		tex.Apply ();		
		
		return tex;
	}
	
	static Texture2D  Paint ( Vector2 pos ,  float rad ,  Color col ,  float hardness ,  Texture2D tex  )
	{
		var start = new Vector2 (Mathf.Clamp (pos.x-rad,0,tex.width),Mathf.Clamp (pos.y-rad,0,tex.height));		
		
		var width = rad*2;		
		
		var end = new Vector2 (Mathf.Clamp (pos.x+rad,0,tex.width),Mathf.Clamp (pos.y+rad,0,tex.height));		
		
		var widthX = Mathf.Round (end.x-start.x);		
		
		var widthY = Mathf.Round (end.y-start.y);		
		
		var sqrRad = rad*rad;		
		
		var sqrRad2 = (rad+1)*(rad+1);		
		
		Color[] pixels = tex.GetPixels((int)start.x, (int)start.y, (int)widthX, (int)widthY, 0);
		
		for (var y=0;y<widthY;y++) 
		{
			for (var x=0;x<widthX;x++) 
			{
				var p = new Vector2 (x,y) + start;				
				
				var center = p + new Vector2(0.5f,0.5f);
				
				float dist = (center-pos).sqrMagnitude;				
				
				if (dist>sqrRad2) 
				{
					continue;
				}				
				
				var samples = Sample (p);
				
				var c = Color.black;
				
				for (var i=0;i<samples.Length;i++) 
				{
					dist = GaussFalloff (Vector2.Distance(samples[i],pos),rad) * hardness;					
					
					if (dist>0) 											
					{
						c += Color.Lerp (pixels[y*(int)widthX+x],col,dist);
					}
					else 
					{
						c+=pixels[y*(int)widthX+x];
					}
				}
				
				c /= samples.Length;
				
				pixels[y*(int)widthX+x]=c;				
				
			}
		}
		
		tex.SetPixels((int)start.x, (int)start.y, (int)widthX, (int)widthY, pixels, 0);		
		
		return tex;
	}
	
	
	static Texture2D  PaintLine ( Vector2 from ,  Vector2 to ,  float rad ,  Color col ,  float hardness ,  Texture2D tex  )
	{	
		var width = rad*2;
		
		var extent = rad;		
		
		var stY = Mathf.Clamp(Mathf.Min(from.y, to.y) - extent, 0, tex.height);		
		
		var stX = Mathf.Clamp(Mathf.Min(from.x, to.x) - extent, 0, tex.width);		
		
		var endY = Mathf.Clamp(Mathf.Max(from.y, to.y) + extent, 0, tex.height);		
		
		var endX = Mathf.Clamp(Mathf.Max(from.x, to.x) + extent, 0, tex.width);
		
		var lengthX = endX - stX;		
		
		var lengthY = endY - stY;
		
		var sqrRad = rad * rad;		
		
		var sqrRad2 = (rad + 1) * (rad + 1);		
		
		Color[] pixels = tex.GetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, 0);		
		
		var start = new Vector2(stX, stY);
		
		//Debug.Log (widthX + "   "+ widthY + "   "+ widthX*widthY);		
		
		for (int y=0;y<(int)lengthY;y++) 
		{
			for (int x=0;x<(int)lengthX;x++) 
			{			
				var p = new Vector2 (x,y) + start;				
				
				var center = p + new Vector2(0.5f, 0.5f);
				
				float dist = (center-NearestPointStrict(from,to,center)).sqrMagnitude;
				
				if (dist>sqrRad2) 
				{				
					continue;
				}
				
				dist = GaussFalloff (Mathf.Sqrt(dist),rad) * hardness;				
				
				//dist = (samples[i]-pos).sqrMagnitude;				
				
				Color c;				
				
				if (dist>0) 
				{
					c = Color.Lerp(pixels[y * (int)lengthX + x], col, dist);
				} 
				else 
				{
					c = pixels[y * (int)lengthX + x];
				}
				
				pixels[y * (int)lengthX + x] = c;
			}
		}
		
		tex.SetPixels((int)start.x, (int)start.y, (int)lengthX, (int)lengthY, pixels, 0);
		
		return tex;
	}
	
	
	internal class BezierPoint 	
	{
		internal Vector2 main;	
		internal Vector2 control1;//Think of as left	
		internal Vector2 control2;//Right	
		//Rect rect;	
		internal BezierCurve curve1;//Left
		internal BezierCurve curve2;//Right
		
		internal BezierPoint ( Vector2 m ,  Vector2 l ,  Vector2 r  )
		{		
			main = m;
			control1 = l;
			control2 = r;
		}
	}
	
	internal class BezierCurve 
	{
		internal Vector2[] points;
		internal float aproxLength;	
		internal Rect rect;	
		internal Vector2  Get ( float t  )		
		{		
			int t2 = (int) Mathf.Round (t*(points.Length-1));
			return points[t2];		
		}
		
		void  Init ( Vector2 p0 ,  Vector2 p1 ,  Vector2 p2 ,  Vector2 p3  )		
		{		
			Vector2 topleft = new Vector2(Mathf.Infinity,Mathf.Infinity);		
			Vector2 bottomright = new Vector2(Mathf.NegativeInfinity,Mathf.NegativeInfinity);			
			
			topleft.x = Mathf.Min (topleft.x,p0.x);						
			topleft.x = Mathf.Min (topleft.x,p1.x);						
			topleft.x = Mathf.Min (topleft.x,p2.x);						
			topleft.x = Mathf.Min (topleft.x,p3.x);
			
			topleft.y = Mathf.Min (topleft.y,p0.y);
			topleft.y = Mathf.Min (topleft.y,p1.y);
			topleft.y = Mathf.Min (topleft.y,p2.y);						
			topleft.y = Mathf.Min (topleft.y,p3.y);			
			
			bottomright.x = Mathf.Max (bottomright.x,p0.x);
			bottomright.x = Mathf.Max (bottomright.x,p1.x);
			bottomright.x = Mathf.Max (bottomright.x,p2.x);
			bottomright.x = Mathf.Max (bottomright.x,p3.x);
			
			bottomright.y = Mathf.Max (bottomright.y,p0.y);
			bottomright.y = Mathf.Max (bottomright.y,p1.y);
			bottomright.y = Mathf.Max (bottomright.y,p2.y);
			bottomright.y = Mathf.Max (bottomright.y,p3.y);
			
			rect = new Rect(topleft.x,topleft.y,bottomright.x-topleft.x,bottomright.y-topleft.y);
			
			var ps = new List<Vector2>();
			
			var point1  = CubicBezier (0,p0,p1,p2,p3);
			
			var point2  = CubicBezier (0.05f,p0,p1,p2,p3);
			
			var point3  = CubicBezier (0.1f,p0,p1,p2,p3);
			
			var point4  = CubicBezier (0.15f,p0,p1,p2,p3);
			
			var point5  = CubicBezier (0.5f,p0,p1,p2,p3);
			
			var point6  = CubicBezier (0.55f,p0,p1,p2,p3);
			
			var point7  = CubicBezier (0.6f,p0,p1,p2,p3);
			
			aproxLength = Vector2.Distance(point1,point2) + Vector2.Distance (point2,point3) + Vector2.Distance (point3,point4)  + Vector2.Distance (point5,point6)  + Vector2.Distance (point6,point7);
			
			Debug.Log (Vector2.Distance(point1,point2) + "     " + Vector2.Distance(point3,point4) + "   " + Vector2.Distance(point6, point7));
			
			aproxLength*= 4;
			
			float a2 = 0.5f/aproxLength;//Double the amount of points since the approximation is quite bad
			
			for (float i = 0 ;i<1;i+=a2) 
			{			
				ps.Add (CubicBezier (i,p0,p1,p2,p3));
			}
			
			points = ps.ToArray();
		}
		
		internal BezierCurve ( Vector2 main ,  Vector2 control1 ,  Vector2 control2 ,  Vector2 end  )
		{
			Init (main,control1,control2,end);		
		}
	}
	
	static void  DrawBezier ( BezierPoint[] points ,  float rad ,  Color col ,  Texture2D tex  )	
	{	
		rad = Mathf.Round (rad);//It is important to round the numbers otherwise it will mess up with the texture width
		
		if (points.Length<=1) 
			return;
		
		Vector2 topleft = new Vector2(Mathf.Infinity,Mathf.Infinity);
		
		Vector2 bottomright = new Vector2(0,0);
		
		for (int i=0;i<points.Length-1;i++) 		
		{		
			Vector2 main = points[i].main;
			Vector2 control2 = points[i].control2;
			Vector2 control1 = points[i + 1].control1;
			Vector2 main2 = points[i + 1].main;		
			BezierCurve curve = new BezierCurve( main, control2, control1, main2 );		
			points[i].curve2 = curve;		
			points[i+1].curve1 = curve;
			
			topleft.x = Mathf.Min (topleft.x,curve.rect.x);
			
			topleft.y = Mathf.Min (topleft.y,curve.rect.y);
			
			bottomright.x = Mathf.Max (bottomright.x,curve.rect.x+curve.rect.width);
			
			bottomright.y = Mathf.Max (bottomright.y,curve.rect.y+curve.rect.height);
		}
		
		topleft-= new Vector2(rad,rad);
		bottomright+= new Vector2(rad,rad);
		
		var start = new Vector2 (Mathf.Clamp (topleft.x,0,tex.width),Mathf.Clamp (topleft.y,0,tex.height));
		
		var width = new Vector2(Mathf.Clamp(bottomright.x - topleft.x, 0, tex.width - start.x), Mathf.Clamp(bottomright.y - topleft.y, 0, tex.height - start.y));
		
		Color[] pixels = tex.GetPixels((int)start.x, (int)start.y, (int)width.x, (int)width.y, 0);
		
		for (var y=0;y<width.y;y++) 
		{		
			for (var x = 0; x < width.x; x++) 			
			{			
				var p = new Vector2(x+start.x,y+start.y);
				
				if (!IsNearBeziers (p,points,rad+2))
				{
					continue;
				}
				
				var samples = Sample (p);
				
				var c = Color.black;				
				
				var pc = pixels[y*(int)width.x+x];//Previous pixel color				
				
				for (var i=0;i<samples.Length;i++) 
				{
					if (IsNearBeziers (samples[i],points,rad)) 
					{
						c+= col;
					} 
					else 
					{
						c+= pc;
					}
				}
				
				c /= samples.Length;
				
				pixels[y*(int)width.x+x] = c;
			}
		}
		
		tex.SetPixels((int)start.x, (int)start.y, (int)width.x, (int)width.y, pixels, 0);
		tex.Apply ();
	}
	
	static void AddP(List<Vector2> tmpList, Vector2 p, float ix, float iy )	
	{	
		var x = p.x + ix;
		var y = p.y + iy;
		
		tmpList.Add(new Vector2(x, y));
	}
	
	static Vector2[] Sample ( Vector2 p  )
	{
		List<Vector2> tmpList = new List<Vector2>(32);
		
		switch (NumSamples) 
		{
		case Samples.None:
			AddP(tmpList,p,0.5f,0.5f);
			break;
			
		case Samples.Samples2:
			AddP(tmpList,p,0.25f,0.5f);
			AddP(tmpList,p,0.75f,0.5f);
			break;
			
		case Samples.Samples4: 
			AddP(tmpList,p,0.25f,0.5f);
			AddP(tmpList,p,0.75f,0.5f);
			AddP(tmpList,p,0.5f,0.25f);
			AddP(tmpList,p,0.5f,0.75f);
			break;
			
		case Samples.Samples8: 
			AddP(tmpList,p,0.25f,0.5f);
			AddP(tmpList,p,0.75f,0.5f);
			AddP(tmpList,p,0.5f,0.25f);
			AddP(tmpList,p,0.5f,0.75f);
			AddP(tmpList,p,0.25f,0.25f);
			AddP(tmpList,p,0.75f,0.25f);
			AddP(tmpList,p,0.25f,0.75f);
			AddP(tmpList,p,0.75f,0.75f);
			break;
			
		case Samples.Samples16:         
			AddP(tmpList,p,0,0);
			AddP(tmpList,p,0.3f,0);
			AddP(tmpList,p,0.7f,0);
			AddP(tmpList,p,1,0);
			AddP(tmpList,p,0,0.3f);
			AddP(tmpList,p,0.3f,0.3f);
			AddP(tmpList,p,0.7f,0.3f);
			AddP(tmpList,p,1,0.3f);
			AddP(tmpList,p,0,0.7f);
			AddP(tmpList,p,0.3f,0.7f);
			AddP(tmpList,p,0.7f,0.7f);
			AddP(tmpList,p,1,0.7f);		
			AddP(tmpList,p,0,1);		
			AddP(tmpList,p,0.3f,1);
			AddP(tmpList,p,0.7f,1);
			AddP(tmpList,p,1,1);
			break;
			
		case Samples.Samples32:
			AddP(tmpList,p,0,0);
			AddP(tmpList,p,1,0);
			AddP(tmpList,p,0,1);
			AddP(tmpList,p,1,1);
			AddP(tmpList,p,0.2f,0.2f);
			AddP(tmpList,p,0.4f,0.2f);
			AddP(tmpList,p,0.6f,0.2f);
			AddP(tmpList,p,0.8f,0.2f);
			AddP(tmpList,p,0.2f,0.4f);
			AddP(tmpList,p,0.4f,0.4f);
			AddP(tmpList,p,0.6f,0.4f);
			AddP(tmpList,p,0.8f,0.4f);
			AddP(tmpList,p,0.2f,0.6f);
			AddP(tmpList,p,0.4f,0.6f);
			AddP(tmpList,p,0.6f,0.6f);
			AddP(tmpList,p,0.8f,0.6f);		
			AddP(tmpList,p,0.2f,0.8f);
			AddP(tmpList,p,0.4f,0.8f);
			AddP(tmpList,p,0.6f,0.8f);		
			AddP(tmpList,p,0.8f,0.8f);		
			AddP(tmpList,p,0.5f,0);
			AddP(tmpList,p,0.5f,1);
			AddP(tmpList,p,0,0.5f);
			AddP(tmpList,p,1,0.5f);		
			AddP(tmpList,p,0.5f,0.5f);
			break;
			
		case Samples.RotatedDisc:
			AddP(tmpList,p,0,0);
			AddP(tmpList,p,1,0);
			AddP(tmpList,p,0,1);
			AddP(tmpList,p,1,1);
			
			Vector2 pq = new Vector2(p.x + 0.5f, p.y + 0.5f);		
			AddP(tmpList,pq,0.258f,0.965f);//Sin (75°) && Cos (75°)		
			AddP(tmpList,pq,-0.965f,-0.258f);
			AddP(tmpList,pq,0.965f,0.258f);
			AddP(tmpList,pq,0.258f,-0.965f);
			break;
		}
		
		return tmpList.ToArray();	
	}
	
	
	#endregion
	
	#region mathfx
	
	
	static float Hermite ( float start ,   float end ,   float value  )
	{
		return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));	
	}
	
	static float Sinerp ( float start ,   float end ,   float value  )
	{
		return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
	}
	
	static float Coserp ( float start ,   float end ,   float value  )
	{
		return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
	}
	
	static float Berp ( float start ,   float end ,   float value  )
	{
		value = Mathf.Clamp01(value);
		
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1 - value, 2.2f) + value) * (1 + (1.2f * (1 - value)));
		
		return start + (end - start) * value;
	}
	
	static float SmoothStep ( float x ,   float min ,   float max  )
	{
		x = Mathf.Clamp (x, min, max);				
		var v1= (x-min)/(max-min);				
		var v2= (x-min)/(max-min);				
		return -2*v1 * v1 *v1 + 3*v2 * v2;				
	}
	
	static float Lerp ( float start ,   float end ,   float value  )
	{
		return ((1.0f - value) * start) + (value * end);
	}
	
	static Vector3 NearestPoint ( Vector3 lineStart ,   Vector3 lineEnd ,   Vector3 point  )
	{
		var lineDirection= Vector3.Normalize(lineEnd-lineStart);
		var closestPoint= Vector3.Dot((point-lineStart),lineDirection)/Vector3.Dot(lineDirection,lineDirection);
		return lineStart+(closestPoint*lineDirection);
	}
	
	static Vector3 NearestPointStrict ( Vector3 lineStart ,   Vector3 lineEnd ,   Vector3 point  )
	{
		var fullDirection= lineEnd-lineStart;
		var lineDirection= Vector3.Normalize(fullDirection);
		var closestPoint= Vector3.Dot((point-lineStart),lineDirection)/Vector3.Dot(lineDirection,lineDirection);
		return lineStart+(Mathf.Clamp(closestPoint,0.0f,Vector3.Magnitude(fullDirection))*lineDirection);
	}
	
	static Vector2 NearestPointStrict ( Vector2 lineStart ,   Vector2 lineEnd ,   Vector2 point  )
	{
		var fullDirection= lineEnd-lineStart;
		var lineDirection= Normalize(fullDirection);
		var closestPoint= Vector2.Dot((point-lineStart),lineDirection)/Vector2.Dot(lineDirection,lineDirection);
		return lineStart+(Mathf.Clamp(closestPoint,0.0f,fullDirection.magnitude)*lineDirection);
	}
	
	static float Bounce ( float x  )
	{
		return Mathf.Abs(Mathf.Sin(6.28f*(x+1)*(x+1)) * (1-x));
	}
	
	
	// test for value that is near specified float (due to floating point inprecision)
	
	
	// all thanks to Opless for this!
	
	static bool Approx ( float val ,   float about ,   float range  )
	{
		return ( ( Mathf.Abs(val - about) < range) );
	}
	
	// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
	
	
	// compares the square of the distance to the square of the range as this 
	
	
	// avoids calculating a square root which is much slower than squaring the range
	
	
	static bool Approx ( Vector3 val ,   Vector3 about ,   float range  )
	{
		return ( (val - about).sqrMagnitude < range*range);	
	}
	
	static float GaussFalloff ( float distance  ,   float inRadius  )
	{
		return Mathf.Clamp01 (Mathf.Pow (360.0f, -Mathf.Pow (distance / inRadius, 2.5f) - 0.01f));
	}
	
	// CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
	
	
	// This is useful when interpolating eulerAngles and the object
	
	
	// crosses the 0/360 boundary.  The standard Lerp function causes the object
	
	
	// to rotate in the wrong direction and looks stupid. Clerp fixes that.
	
	
	static float Clerp ( float start ,   float end ,   float value  )
	{
		var min= 0.0f;
		var max = 360.0f;
		var half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
		var retval = 0.0f;
		var diff = 0.0f;
		
		if((end - start) < -half)
		{		
			diff = ((max - start)+end)*value;
			retval =  start+diff;
		}
		
		else if((end - start) > half)
		{
			diff = -((max - end)+start)*value;
			retval =  start+diff;		
		}
		
		else retval =  start+(end-start)*value;
		
		return retval;	
	}
	
	
	//======= NEW =========//
	
	
	
	static Vector2 RotateVector(Vector2 vector, float rad)	
	{
		rad *= Mathf.Deg2Rad;
		var res = new Vector2 ((vector.x * Mathf.Cos(rad)) - (vector.y * Mathf.Sin(rad)),(vector.x * Mathf.Sin(rad)) + (vector.y * Mathf.Cos(rad)));
		return res;
	}
	
	static Vector2  IntersectPoint ( Vector2 start1 ,  Vector2 start2 ,  Vector2 dir1 ,  Vector2 dir2  )
	{
		if (dir1.x==dir2.x) 
		{
			return Vector2.zero;
		}
		
		var h1 = dir1.y/dir1.x;
		var h2 = dir2.y / dir2.x;
		
		if (h1==h2) 		
		{
			return Vector2.zero;
		}
		
		var line1 = new Vector2 (h1,start1.y-start1.x*h1);
		var line2 = new Vector2 (h2,start2.y-start2.x*h2);
		
		var y1 = line2.y-line1.y;
		var x1 = line1.x-line2.x;
		
		var x2 = y1 / x1;
		
		var y2 = line1.x*x2 + line1.y;
		return new Vector2(x2,y2);
	}
	
	static Vector2 ThreePointCircle ( Vector2 a1 ,  Vector2 a2 ,  Vector2 a3  )
	{	
		var dir = a2-a1;				
		dir /= 2;
		
		var b1 = a1+dir;
		dir = RotateVector (dir,90);
		
		var l1 = dir;
		dir = a3-a2;
		dir /= 2;		
		
		var b2 = a2 + dir;
		dir = RotateVector (dir,90);
		
		var l2 = dir;
		var p = IntersectPoint(b1, b2, l1, l2);
		return p;
	}
	
	
	//===== Bezier ====== //
	
	
	static Vector2  CubicBezier ( float t ,  Vector2 p0 ,  Vector2 p1 ,  Vector2 p2 ,  Vector2 p3  )
	{
		// FIXME: fix bezier curve algorithm.		
		
		/*      t = Mathf.Clamp01 (t);

        var t2 = 1-t;

    return Mathf.Pow(t2, 3) * p0 + 3 * Mathf.Pow(t2, 2) * t * p1 + 3 * t2 * Mathf.Pow(t, 2) * p2 + Mathf.Pow(t, 3) * p3;

 */
		return Vector2.zero;
	}
	
	static Vector2 NearestPointOnBezier(Vector2 p, BezierCurve c, float accuracy, bool doubleAc)
	{
		float minDist = Mathf.Infinity;
		float minT = 0;
		Vector2 minP = Vector2.zero;
		for (float i = 0 ;i<1;i+=accuracy) 
		{
			var point  = c.Get (i);
			float d = (p - point).sqrMagnitude;
			if (d<minDist) 
			{
				minDist=d;
				minT = i;
				minP = point;
			}
		}
		
		if (!doubleAc) 
		{
			return minP;
		}
		
		float st = Mathf.Clamp01(minT - accuracy);
		
		float en = Mathf.Clamp01 (minT+accuracy);
		
		for (var i=st;i<en;i+=accuracy/10) 
		{
			var point  = c.Get (i);
			float d= (p - point).sqrMagnitude;			
			
			if (d<minDist) 
			{
				minDist=d;
				minT = i;
				minP = point;
			}
		}
		
		return minP;
	}
	
	static bool IsNearBezierTest ( Vector2 p ,  BezierCurve c ,  float accuracy ,  float maxDist  )
	{
		Vector2 prepoint = c.Get (0);
		
		for (float i = accuracy;i<1;i+=accuracy) 
		{
			var point  = c.Get (i);
			float d = (p - point).sqrMagnitude;
			float d2 = (prepoint - point + new Vector2(maxDist,maxDist)).sqrMagnitude;
			
			if (d<=d2*2) 
				return true;
		}
		
		return false;
	}
	
	static Vector2  NearestPointOnBezier ( Vector2 p ,  Vector2 p0 ,  Vector2 p1 ,  Vector2 p2 ,  Vector2 p3  )
	{	
		float minDist = Mathf.Infinity;
		float minT = 0;
		Vector2 minP = Vector2.zero;
		
		for (float i = 0 ;i<1;i+=0.01f) 
		{
			var point  = CubicBezier (i,p0,p1,p2,p3);
			float d = (p - point).sqrMagnitude;
			
			if (d<minDist) 
			{
				minDist=d;
				minT = i;
				minP = point;
			}
		}
		
		float st = Mathf.Clamp01 (minT-0.01f);
		float en = Mathf.Clamp01(minT + 0.01f);
		
		for (var i=st;i<en;i+=0.001f) 
		{
			var point  = CubicBezier (i,p0,p1,p2,p3);
			var d= (p - point).sqrMagnitude;
			
			if (d<minDist) 
			{
				minDist=d;
				minT = i;
				minP = point;
			}
		}
		return minP;	
	}
	
	static bool  IsNearBezier ( Vector2 p ,  BezierPoint point1 ,  BezierPoint point2 ,  float rad  )
	{
		if (point1.curve2 != point2.curve1) 
		{	
			Debug.LogError ("Curves Not The Same");		
			return false;		
		}
		
		BezierCurve curve = point1.curve2;
		
		var r = curve.rect;		
		
		r.x-=rad;		
		
		r.y-=rad;		
		
		r.width+=rad*2;		
		
		r.height+=rad*2;
		
		if (!r.Contains (p)) 
		{
			return false;		
		}
		
		var nearest = NearestPointOnBezier (p,curve,0.1f,false);
		
		var sec = point1.curve2.aproxLength/10;
		
		if ((nearest-p).sqrMagnitude>=(sec*3)*(sec*3)) 
		{
			return false;		
		}
		
		nearest = NearestPointOnBezier (p,curve,0.01f,true);
		
		if ((nearest-p).sqrMagnitude<=rad*rad) 
		{
			return true;
		}
		
		return false;
	}
	
	static bool  IsNearBeziers ( Vector2 p ,  BezierPoint[] points ,  float rad  )
	{
		for (var i=0;i<points.Length-1;i++) 		
		{		
			if (IsNearBezier (p,points[i],points[i+1],rad)) 
			{
				return true;
			}
		}
		return false;
	}
	
	
	//====== End Bezier ========//
	
	
	static Vector2  NearestPointOnCircle ( Vector2 p ,  Vector2 center ,  float w  )
	{
		Vector2 dir = p-center;
		dir = Normalize (dir);
		dir *= w;
		return center+dir;
	}
	
	static Vector2 Normalize(Vector2 p)	
	{	
		float mag = p.magnitude;
		return p/mag;
	}
	
	#endregion
	
	
};