using UnityEngine;
using System.Collections;

public class UIColorRenderer : MonoBehaviour {

	public void OnColorChange(){
		renderer.material.color = UIColorPicker.current.value;
	}
}
