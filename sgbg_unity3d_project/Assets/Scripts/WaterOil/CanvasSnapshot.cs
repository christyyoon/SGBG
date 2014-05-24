using UnityEngine;
using System.Collections;

public class CanvasSnapshot {

	private Texture2D canvasTex;
	private short[] canvasBuf;

	public CanvasSnapshot(Texture2D tex, short[] buf){
		this.canvasTex = tex;
		this.canvasBuf = buf;
	}

	public Texture2D CanvasTex {
		get {
			return canvasTex;
		}
	}

	public short[] CanvasBuf {
		get {
			return canvasBuf;
		}
	}
}
