using UnityEngine;
using System.Collections;

public class GetSandartSpace : MonoBehaviour {

	// Use this for initialization
	void Start () {
		string dataname = PlayerPrefs.GetString ("art"); // Receive datafile's name
		
		/* If datafile's name is not empty, load datafile to wateroil scene  */
		if (PlayerPrefs.HasKey("art")){
			Debug.Log("dataname : " + dataname);
			GameObject canvas = GameObject.Find("canvas");
			
			Sandart canvasScript = canvas.GetComponent<Sandart>();
			
			canvasScript.init(dataname+".buf");
		}
		
		PlayerPrefs.DeleteKey ("art");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
