using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIColorPicker : UIWidgetContainer {
	public List<EventDelegate> onChange = new List<EventDelegate>();
	[SerializeField]
	private bool circular;
	[SerializeField]
	private UITexture colorSpectrum;
	[SerializeField]
	private UISprite thumb;
	static public UIColorPicker current;
	[HideInInspector][SerializeField] protected Color mValue = Color.white;

	private void Start(){
		value = GetColor();
	}

	public Color value
	{
		get{return mValue;}
		set{
			if (mValue != value){
				Color before = this.value;
				mValue = value;
				if (before != this.value){
					if (NGUITools.GetActive(this) && EventDelegate.IsValid(onChange)){
						current = this;
						EventDelegate.Execute(onChange);
						current = null;
					}
				}
			}
		}
	}

	private void OnPress(bool pressed){
		value = GetColor();
		UpdateThumbPosition ();
	}
	
	private void OnDrag(Vector2 delta){
		value = GetColor();
		UpdateThumbPosition ();
	}

	public void OnCanvasDown(Vector2 position){
		value = GetColor();

		Debug.Log ("color picker oncanvasdown");

		Vector2 pos = (Vector2)position;
		//Vector2 pos = UICamera.lastTouchPosition;
		pos.x = Mathf.Clamp01(pos.x / Screen.width);
		pos.y = Mathf.Clamp01(pos.y / Screen.height);
		thumb.transform.position =UICamera.currentCamera.ViewportToWorldPoint(pos);
		if (circular) {
			float length = thumb.transform.localPosition.magnitude;
			if (length > colorSpectrum.localSize.x * 0.5f-thumb.localSize.x*0.5f) {
				thumb.transform.localPosition = Vector3.ClampMagnitude (thumb.transform.localPosition, colorSpectrum.localSize.x * 0.5f-thumb.localSize.x*0.5f);
			}
		} else {
			
			thumb.transform.localPosition=new Vector2(Mathf.Clamp(thumb.transform.localPosition.x,-colorSpectrum.localSize.x*0.5f+thumb.localSize.x*0.5f,colorSpectrum.localSize.x*0.5f-thumb.localSize.x*0.5f),
			                                          Mathf.Clamp(thumb.transform.localPosition.y,-colorSpectrum.localSize.y*0.5f+thumb.localSize.y*0.5f,colorSpectrum.localSize.y*0.5f-thumb.localSize.y*0.5f));
			
		}
	}

	private Color GetColor(){
		Vector2 spectrumScreenPosition = UICamera.mainCamera.WorldToScreenPoint(colorSpectrum.transform.position);
		Vector2 thumbScreenPosition = UICamera.mainCamera.WorldToScreenPoint (thumb.transform.position);
		Vector2 position=thumbScreenPosition-spectrumScreenPosition+colorSpectrum.localSize*0.5f;
		Texture2D texture = colorSpectrum.mainTexture as Texture2D;
		position = new Vector2 ((position.x/colorSpectrum.localSize.x), (position.y / colorSpectrum.localSize.y) );
		return texture.GetPixelBilinear (position.x,position.y);
	}

	private void UpdateThumbPosition(){
		Vector2 pos = UICamera.lastTouchPosition;
		pos.x = Mathf.Clamp01(pos.x / Screen.width);
		pos.y = Mathf.Clamp01(pos.y / Screen.height);
		thumb.transform.position =UICamera.currentCamera.ViewportToWorldPoint(pos);
		if (circular) {
			float length = thumb.transform.localPosition.magnitude;
			if (length > colorSpectrum.localSize.x * 0.5f-thumb.localSize.x*0.5f) {
				thumb.transform.localPosition = Vector3.ClampMagnitude (thumb.transform.localPosition, colorSpectrum.localSize.x * 0.5f-thumb.localSize.x*0.5f);
			}
		} else {

			thumb.transform.localPosition=new Vector2(Mathf.Clamp(thumb.transform.localPosition.x,-colorSpectrum.localSize.x*0.5f+thumb.localSize.x*0.5f,colorSpectrum.localSize.x*0.5f-thumb.localSize.x*0.5f),
			                                          Mathf.Clamp(thumb.transform.localPosition.y,-colorSpectrum.localSize.y*0.5f+thumb.localSize.y*0.5f,colorSpectrum.localSize.y*0.5f-thumb.localSize.y*0.5f));

		}
	}
}
