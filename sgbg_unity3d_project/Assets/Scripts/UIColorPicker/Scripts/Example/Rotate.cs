using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	public float speed=2;
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up * speed, Space.World);
	}
}
