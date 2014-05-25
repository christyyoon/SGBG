using UnityEngine;
using System.Collections;
using System.IO;

/* Create picture object in the gallery */
public class galleryObjGenerator : MonoBehaviour {
	
	public int fcount; // Count the number of file
	public int limit=6; // Limit number of picture in the gallery
	public Texture2D texturepng; // Picture's material texture
	public Vector3 originScale; // Object's first scale value
	public Vector3 originPosition; // Object's first position value
	public string[] files; // String array(save screenshot file)
	public string filename; // png file's name


	// Use this for initialization
	public IEnumerator Start () {

		fcount = Directory.GetFiles (Application.dataPath+"/galleryData", "*.png", SearchOption.AllDirectories).Length; // Count the number of file(파일개수)
		files = Directory.GetFiles (Application.dataPath + "/galleryData", "*.png", SearchOption.AllDirectories); // String array(save screenshot file)



		for (int i=1; i<=fcount; i++) { // Create object number of fcount 


			yield return new WaitForSeconds(0.1f); // Start delay
			
			filename = Path.GetFileNameWithoutExtension(files[fcount-i]); // png file's name
			
			WWW www = new WWW("file://"+Application.dataPath+"/galleryData/"+filename+".png"); // Download png image through the WWW
			texturepng = www.texture; // Apply recieved image to material texture
			
			GameObject picture = GameObject.CreatePrimitive (PrimitiveType.Cube); // Create dynamic object
			
			picture.transform.localScale = new Vector3 (0.57f, 0.42f, 0.01f); // Define size of object

			// Define gap of object
			if(i<=3)
				picture.transform.position = new Vector3(0.63f*i-1.21f, 1.27f, 0);
			else
				picture.transform.position = new Vector3(0.63f*(i-3)-1.21f, 0.7f, 0);

			picture.transform.Rotate(0, 0, 180); // Define rotation of object
			picture.renderer.material.mainTexture = texturepng; // Apply texture to object's material
			picture.name = "picture"+ i.ToString(); // Determine object name
			
			
			originScale = new Vector3 (0.57f, 0.42f, 0.01f); // Save the origin scale

			// Save the origin position
			if(i<=3)
				originPosition = new Vector3(0.63f*i-1.21f, 1.27f, 0);
			else
				originPosition = new Vector3(0.63f*(i-3)-1.21f, 0.7f, 0);
			
			picture.AddComponent<picture>(); // Add picture.cs to object
			
			yield return new WaitForSeconds(0.1f); // End delay
			
		}
		
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
	}
	
}