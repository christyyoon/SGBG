using UnityEngine;
using System.Collections;

public class gdelguide : MonoBehaviour {

	private bool isReady = true;
	private const float TIME_INTERVAL = 0.8f;
	
	void buttonReady(){
		isReady = true;
		//PlayerPrefs.DeleteKey ("isReady");
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	void setTimer(){
		isReady = false;
		Invoke("buttonReady",TIME_INTERVAL);
	}

	void OnCanvasDown(){
		if(isReady == true){
			OnMouseDown();
			Invoke("buttonReady",TIME_INTERVAL);
			//PlayerPrefs.SetInt("isReady",0);
			isReady = false;
		}
	}
	void OnMouseDown()
	{
		this.transform.position = new Vector3 (0.051f, 2.89f, -0.09f);

		for(int i = 1 ; i <= 6 ; i++){
			GameObject pictures = GameObject.Find("picture"+i.ToString());
			if(pictures != null){
				picture picScript = pictures.GetComponent<picture>();
				picScript.setTimer();
			}
			
		}

	}
}
