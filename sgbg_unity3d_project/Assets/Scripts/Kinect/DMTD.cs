using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices; // needed to import dll 

public class DMTD : MonoBehaviour {

	[DllImport("DMTD")]private static extern void fnDMTDCreate();
	[DllImport("DMTD")]private static extern void fnDMTDDestroy();

	// Use this for initialization
	void Start () {
		fnDMTDCreate ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
