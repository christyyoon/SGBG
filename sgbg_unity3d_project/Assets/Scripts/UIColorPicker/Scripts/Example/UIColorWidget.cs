using UnityEngine;
using System.Collections;

public class UIColorWidget : MonoBehaviour {
	[SerializeField]
	private UIWidget widget;

	public void OnColorChange(){
		widget.color = UIColorPicker.current.value;
	}
}
