using UnityEngine;
using System.Collections;

public class UIColorRenderer : MonoBehaviour {

	public void OnMouseDown(){
		Debug.Log ("hi");
		animation.Play ("hi");
		}

	public void OnColorChange(){
		renderer.material.color = UIColorPicker.current.value;
	}
}
