using UnityEngine;
using System.Collections;
using System.IO;

public class galleryobj : MonoBehaviour {
	public int fcount; // Count the number of file
	public int limit=3; // Limit number of file in the gallery
	//public int ncount=1; // Count the file name number ex) capture2.png --> ncount=2
	private Texture2D texturepng; // Picture's material texture

	public Vector3 originScale; // Object's first scale value
	public Vector3 originPosition; // Object's first position value
	public int objnumber;

	// Use this for initialization
	IEnumerator Start () {

		fcount = Directory.GetFiles ("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot", "*", SearchOption.AllDirectories).Length; // Count the number of file(파일개수)

		//if (fcount > limit) // Just read file by limit number 
		//	fcount = limit;

		for (int i=1; i<=fcount; i++) { // Create object number of fcount 

			yield return new WaitForSeconds(0.5f); // Start delay

			/* If there is not exist capture(ncount).png file, then increase ncount.
			   Because we have to seek next file.
			   This source seek file by asc. 
			while(File.Exists("C:\\Users\\Micky\\Documents\\last\\screenshot\\capture" + ncount + ".png")==false)
			{
				ncount++;
			}*/

			WWW www = new WWW("file://"+@"C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture"+(fcount-i+1)+".png"); // Download data in web through the WWW
			texturepng = www.texture; // Apply recieved image to material texture
			//ncount++; // After apply png file to texture, then increase ncount(If not, Gallery create same png file) 

			GameObject picture = GameObject.CreatePrimitive (PrimitiveType.Cube); // Create dynamic object

			picture.transform.localScale = new Vector3 (0.5f, 0.4f, 0.01f); // Define size of object
			picture.transform.position = new Vector3 (0.68f*i-1.31f, 1.31f, 0); // Define gap of object
			picture.transform.Rotate(0, 0, 180); // Define rotation of object

			picture.renderer.material.mainTexture = texturepng; // Apply texture to object's material
			picture.name = "picture"+ i.ToString(); // Determine object name

			originScale = new Vector3 (0.5f, 0.4f, 0.01f);
			originPosition = new Vector3 (0.68f*i-1.31f, 1.31f, 0);

			picture.AddComponent<picture>(); // Add picture.cs to object
			objnumber=fcount-i+1;
			yield return new WaitForSeconds(0.5f); // End delay
				}


	}



	
	// Update is called once per frame
	void Update () {
	}
}
