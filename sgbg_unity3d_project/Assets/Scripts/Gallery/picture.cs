using UnityEngine;
using System.Collections;
using System.IO;

/* Dynamic objects's script component */
public class picture : MonoBehaviour
{
	
	public Vector3 receivedScale; // Value of receiver(scale)
	public Vector3 receivedPosition; // Value of receiver(position)
	
	public string receivedFilename; // value of receiver(png file name)
	public StreamReader reader; // File reader
	public FileInfo datafile; // File info
	public string nextScene; // Identification(sort of work ex> warteroil or sandart)
	
	public galleryObjGenerator gal; // Frame's script(galleryObjGenerator.cs)
	public GameObject frameobj; // Frame object
	
	public delPicture delPicCode; // Delete's script(delPicture.cs)
	public GameObject delobj; // Delete object
	
	public int check = 1; // OnMouseDown() identification ex) 1-> select picture about deletion, zoom the picture 0-> deselect picture, return picture 
	public int delOn = 0; // selected picture about deletetion identification ex) 0-> nothing 1-> permit to select of deletion
	
	/*
     * Example of data file
     * 
     * -----------------------
     * wateroil
     * 3
     * 0.962 0.863 0.888 1.000
     * 0.738 0.709 0.948 1.000
     * 0.650 0.844 0.980 1.000
     *------------------------
     *
     */
	
	// Use this for initialization
	
	void Start()
	{
		
		frameobj = GameObject.Find("frame"); // Find gameobject
		gal = frameobj.GetComponent<galleryObjGenerator>(); // Get script
		
		delobj = GameObject.Find("delete");
		delPicCode = delobj.GetComponent<delPicture>();
		
		receivedScale = gal.originScale; // Receive originscale
		receivedPosition = gal.originPosition; // Receive originposition
		receivedFilename = gal.filename; // Receive filename(png)
		
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
	
	/* Function about delete png image in certain route */
	public void DeletePic()
	{
		File.Delete(Application.dataPath + "/galleryData/" + receivedFilename + ".png");

		if (File.Exists(Application.dataPath + "/galleryData/" + receivedFilename + ".data"))
			File.Delete(Application.dataPath + "/galleryData/" + receivedFilename + ".data");

		if (File.Exists(Application.dataPath + "/galleryData/" + receivedFilename + ".buf"))
			File.Delete(Application.dataPath + "/galleryData/" + receivedFilename + ".buf");
		
	}
	
	void OnMouseDown()
	{
		
		// When delete button is activated, and picture is selected(working space)
		if ((check == 1) && (File.Exists(Application.dataPath + "/galleryData/" + receivedFilename + ".data")) && (delPicCode.state == 1))
		{
			renderer.material.color = Color.gray; // Picture's color is darkend because of selection's expression.
			delOn = 1; // Permit to select of deletion
			check = 0; // Provide chance to deselction
		}
		
		// When delete button is activated, and picture is deselected(working space)
		else if ((check == 0) && (File.Exists(Application.dataPath + "/galleryData/" + receivedFilename + ".data")) && (delPicCode.state == 1))
		{
			renderer.material.color = Color.white; // Picture's color is brightened because of deselection's expression.
			delOn = 0; // Cancel to select of deletion
			check = 1; // Provide chance to selection
		}
		
		// If png file involve data file(working file)
		else if ((check == 1) && (File.Exists(Application.dataPath + "/galleryData/" + receivedFilename + ".data")))
		{
			datafile = new FileInfo(Application.dataPath + "/galleryData/" + receivedFilename + ".data"); // Data file
			reader = datafile.OpenText(); // Open data file by text
			
			nextScene = reader.ReadLine(); // Read first line(wateroil or sandart)
			
			if (nextScene == "wateroil") // If wateroil datafile
			{
				PlayerPrefs.SetString("art", receivedFilename); // Send datafile's name
				Application.LoadLevel("wateroil"); // switch to wateroil scene
			}
			else if (nextScene == "sandart") // If sandart datafile
			{
				PlayerPrefs.SetString("art", receivedFilename); // Send datafile's name
				Application.LoadLevel("sandart"); // switch to sandart scene
			}
			
			
		}
		
		else if ((check == 1) && (delPicCode.state == 1)) // When delete button is activated, and picture is selected(screenshot)
		{
			renderer.material.color = Color.gray; // Picture's color is darkend because of selection's expression.
			delOn = 1; // Permit to select of deletion
			check = 0; // Provide chance to deselction
		}
		
		else if ((check == 0) && (delPicCode.state == 1)) // When delete button is activated, and picture is deselected(screenshot)
		{
			renderer.material.color = Color.white; // Picture's color is brightened because of deselection's expression.
			delOn = 0; // Cancel to select of deletion
			check = 1; // Provide chance to selection
		}
		
		else if (check == 1) // When select picture, picture is expanded(screenshot)
		{
			renderer.transform.position = new Vector3(0.05f, 0.96f, -0.1f); // Apply expanded picturs's position
			renderer.transform.localScale = new Vector3(1.83f, 1.16f, 0.01f); // Apply expanded picture,s scale
			check = 0; // Provide chance to deselction
		}
		
		else if (check == 0) // When select expanded picture, pictue is returned original size and position(screenshot)
		{
			renderer.transform.position = receivedPosition; // Return to original position
			renderer.transform.localScale = receivedScale; // Return to original scale
			check = 1; // Provide chance to selction
		}
		
	}
}