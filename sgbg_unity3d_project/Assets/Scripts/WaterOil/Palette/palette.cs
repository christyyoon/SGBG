using UnityEngine;
using System.Collections;
using System.IO;

public class palette : MonoBehaviour {
	
	public GameObject[] paint;// Create new GameObject[8];
	public Color clr; // When draw line and finish, that color(OnMouseUp())
	public Color palleteClr; // Receiver of clr
	public int allTrueCaseCnt=0; // Count value, when all paint object exist
	public int check=1; // Identification(if paint is selected of not selected) 1 -> not selected 0 -> selected

	public string dataname; // Working file's data file name(NULL or wateroil or sandart)
	public StreamReader reader; // File reader
	public FileInfo datafile; // File info

	public int countOfPaint; // Data file's paint count
	public string unionOfRGBA; // Data file's union of paint's rgba
	public string[] rgba; // Paint's rgba array

	public float r; // rgb(r)
	public float g; // rgb(g)
	public float b; // rgb(b)
	public float a; // rgb(a)

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
	void Start () {

		paint= new GameObject[8]; // Create paint object(8)

		// Set paint object's state(initialization)
		for (int i=0; i<8; i++) {
			paint[i] = GameObject.Find("pallete/paint"+i.ToString()); // Find paint object
			paint[i].renderer.enabled=false; // Set enabled false
		}

		dataname = PlayerPrefs.GetString ("art"); // Receive datafile's name

		/* If datafile's name is not empty, load datafile to wateroil scene  */
		if (dataname != string.Empty)
		{
			datafile = new FileInfo (Application.dataPath + "/screenshot/" + dataname + ".data"); // Datefile

			reader = datafile.OpenText (); // Open datafile by text
	
			reader.ReadLine (); // Read first line(wateroil or sandart)

			countOfPaint = System.Convert.ToInt32 (reader.ReadLine ()); // Read paint's count and convert to integer

			/* Activate paint object and set color */
			for (int i=0; i<countOfPaint; i++)
			{
				unionOfRGBA = reader.ReadLine (); // Data file's union of paint's rgba
				rgba = unionOfRGBA.Split (); // Split union of paint's rgba and distribute to array

				// set rgba string to rgba value
				r = (float)System.Convert.ToDouble (rgba [0]);
				g = (float)System.Convert.ToDouble (rgba [1]);
				b = (float)System.Convert.ToDouble (rgba [2]);
				a = (float)System.Convert.ToDouble (rgba [3]);

				paint [i].renderer.enabled = true; // Activate paint object
				paint [i].renderer.material.color = new Color (r, g, b, a); // Set paint's color

			}

			PlayerPrefs.DeleteKey ("art"); // Delete art's information

		}

	}
	
	
	// Update is called once per frame
	void Update () {

		int clrChange = 0; // Identification(change color) 0 -> not change 1 -> color is changed
		int paintChange = 0; // Identification 0 -> paint color is empty 1 -> paint color is filled

		// If color is changed
		if (palleteClr != clr) {

			clrChange=1; // Color is changed
			palleteClr=clr; // change palleteClr by updated color

			// When all paint object is filled by color and paint object is not selected
			if((paint[0].renderer.enabled==true) && (paint[1].renderer.enabled==true) && (paint[2].renderer.enabled==true) && (paint[3].renderer.enabled==true) && (paint[4].renderer.enabled==true) && (paint[5].renderer.enabled==true) && (paint[6].renderer.enabled==true) && (paint[7].renderer.enabled==true) && (check==1))
			{
	
				paint[allTrueCaseCnt%8].renderer.material.color=palleteClr; //Refilled paint color order by asc
				allTrueCaseCnt++;
		
			}

			// when paint is not filled by color(wateroil scene is opened firstly)
			for(int i=0; i<8; i++)
			{
				if((paint[i].renderer.enabled == false) && (check==0)) // If not empty paint selected
					break;

				else if(paint[i].renderer.enabled == false) // If paint is empty
				{
					paint[i].renderer.enabled=true; // Activate paint object
					paint[i].renderer.material.color=palleteClr; // Set color to paint object
					paintChange=1; // Paint color is filled
				}

				if(clrChange == paintChange) // For once update, once change, so two identification is true, then break
					break;
			}

			check=1; // Because user select paint object, check state is changed 0

		}


	}
}