using UnityEngine;
using System.Collections;
using System.IO;

/* Create picture object in the gallery */
public class galleryobj : MonoBehaviour {

	public int fcount; // Count the number of file
	public int limit=3; // Limit number of picture in the gallery
	public Texture2D texturepng; // Picture's material texture
	public Vector3 originScale; // Object's first scale value
	public Vector3 originPosition; // Object's first position value

	// Use this for initialization
	public IEnumerator Start () {

		fcount = Directory.GetFiles ("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot", "*", SearchOption.AllDirectories).Length; // Count the number of file(파일개수)
	

		/* Code about deletion picture randomly */

		// Examine png file's name as fcount ex) fcount = 2, (capture1.png, capture2.png) -> normal (capture1.png, capture3.png) -> abnormal
		for (int j=1; j<=fcount; j++) 
		{

			// If png file not exist about correspond number of file
			if(File.Exists("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + j + ".png")==false)
			{
				// Examine next number file as remained file number(fcount-examined file count)
				for(int k=1; k<=fcount-j+1; k++)
				{
					int jk = j+k;

					// If next number's file exist
					if(File.Exists("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (j+k) + ".png")==true) 

						// Move next number's file to (next-1) number's file
						File.Move("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (j+k) + ".png", "C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (j+k-1) + ".png");

					// If there not exist next number's file
					else if(File.Exists("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (j+k) + ".png")==false)
					{

						// If certain number's file is filled by another number's file name, then stop
						while(File.Exists("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (j+k-1) + ".png")==false)
						{
							jk=jk+1; // Because find nextfile, if not exist then find next file again........

							// If find file, then change file name
							if(File.Exists("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (jk) + ".png"))
								File.Move("C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (jk) + ".png", "C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture" + (j+k-1) + ".png");
						}
					}
				}
			}
		}


		for (int i=1; i<=fcount; i++) { // Create object number of fcount 
			
			yield return new WaitForSeconds(0.5f); // Start delay
			
			WWW www = new WWW("file://"+@"C:\\Users\\Micky\\Documents\\last\\artbox_Data\\screenshot\\capture"+(fcount-i+1)+".png"); // Download data through the WWW
			texturepng = www.texture; // Apply recieved image to material texture
			
			GameObject picture = GameObject.CreatePrimitive (PrimitiveType.Cube); // Create dynamic object
			
			picture.transform.localScale = new Vector3 (0.5f, 0.4f, 0.01f); // Define size of object
			picture.transform.position = new Vector3 (0.68f*i-1.31f, 1.31f, 0); // Define gap of object
			picture.transform.Rotate(0, 0, 180); // Define rotation of object
			
			picture.renderer.material.mainTexture = texturepng; // Apply texture to object's material
			picture.name = "capture"+ (fcount-i+1).ToString(); // Determine object name
			
			originScale = new Vector3 (0.5f, 0.4f, 0.01f); // Save the origin scale
			originPosition = new Vector3 (0.68f*i-1.31f, 1.31f, 0); // Save the origin position

			picture.AddComponent<picture>(); // Add picture.cs to object
	
			yield return new WaitForSeconds(0.5f); // End delay

		}
			
	}

	

	// Update is called once per frame
	void Update () {

	}

}