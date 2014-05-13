using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIColorPicker))]
#else
[CustomEditor(typeof(UIColorPicker), true)]
#endif
public class UIColorPickerEditor : UIWidgetContainerEditor {
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		if (NGUIEditorTools.DrawHeader("Appearance"))
		{
			NGUIEditorTools.BeginContents();
			NGUIEditorTools.DrawProperty("Spectrum", serializedObject, "colorSpectrum");
			NGUIEditorTools.DrawProperty("Thumb", serializedObject, "thumb");
			NGUIEditorTools.DrawProperty("Circular", serializedObject, "circular");
			NGUIEditorTools.EndContents();
		}

		UIColorPicker colorPicker = target as UIColorPicker;
		NGUIEditorTools.DrawEvents("On Value Change", colorPicker, colorPicker.onChange);
		serializedObject.ApplyModifiedProperties();
	}
}
