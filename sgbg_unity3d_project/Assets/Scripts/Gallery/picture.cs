using UnityEngine;
using System.Collections;
using System.IO;

/* Dynamic objects's script component */
public class picture : MonoBehaviour {

	public Vector3 receivedScale; // Value of receiver(scale)
	public Vector3 receivedPosition; // Value of receiver(position)
	public string receivedFilename; // value of receiver(png file name)

	public galleryobj gal; // Canvas's script(galleryobj.cs)
	public GameObject canvasobj; // Canvas object

	public delpicture delPicCode; // Delete's script(delpicture.cs)
	public GameObject delobj; // Delete object

	public int check = 1; // OnMouseDown() identification ex) 1-> select picture about deletion, zoom the picture 0-> deselect picture, return picture 
	public int delOn = 0; // selected picture about deletetion identification ex) 0-> nothing 1-> permit to select of deletion
	
	// Use this for initialization
	
	void Start () {
		
		canvasobj = GameObject.Find ("canvas"); // Find gameobject
		gal = canvasobj.GetComponent<galleryobj> (); // Get script

		delobj = GameObject.Find ("delete");
		delPicCode = delobj.GetComponent<delpicture> ();

		receivedScale = gal.originScale; // Receive originscale
		receivedPosition = gal.originPosition; // Receive originposition
		receivedFilename = gal.filename;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* Function about delete png image in certain route */
	public void DeletePic()
	{
		File.Delete (Application.dataPath + "/screenshot/" + receivedFilename + ".png");
	}

	void OnMouseDown()
	{

		if ((check == 1) && (delPicCode.state == 1)) // When delete button is activated, and picture is selected
		{
			renderer.material.color = Color.gray; // Picture's color is darkend because of selection's expression.
			delOn = 1; // Permit to select of deletion
			check = 0; // Provide chance to deselction
		}

		else if ((check == 0) && (delPicCode.state == 1)) // When delete button is activated, and picture is deselected
		{
			renderer.material.color = Color.white; // Picture's color is brightened because of deselection's expression.
			delOn=0; // Cancel to select of deletion
			check=1; // Provide chance to selection
		}

		else if (check == 1) // When select picture, picture is expanded
		{
			renderer.transform.position = new Vector3 (0.065f, 1.01f, -0.1f); // Apply expanded picturs's position
			renderer.transform.localScale = new Vector3 (2.33f, 1.75f, 0.01f); // Apply expanded picture,s scale
			check = 0; // Provide chance to deselction
		}

		else if (check == 0) // When select expanded picture, pictue is returned original size and position
		{
			renderer.transform.position = receivedPosition; // Return to original position
			renderer.transform.localScale = receivedScale; // Return to original scale
			check=1; // Provide chance to selction
		}
		
	}
}
