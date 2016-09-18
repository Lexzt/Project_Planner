using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Roles
{
    eNONE,
    eSENTRY,
    eCHECKER,
    ePASS_OFFICE,
    eCONSOLE,
    eDRIVER,
};

public class Emplacement : MonoBehaviour {

	public string NameOfEmplacement;
	public List<Stick> ListOfSticks;
	public int TotalAmtOfStick;
	public int Pirority;
	public bool StickUnique = false;
    public Roles CurrentRole = Roles.eNONE;

	// Use this for initialization
	void Start () 
	{
		//// Generate total amount of sticks based off time. This is yet to exclude the time that we are to remove.
		//GenerateSticks ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GenerateSticks (GameObject Parent,GameObject StickGameObject,Roles EmplacementType,int index) 
	{
		TotalAmtOfStick = (int)(StaticVars.EndDate - StaticVars.StartDate).TotalHours / StaticVars.StickInHours;
        CurrentRole = EmplacementType; 
		GameObject EmplacementObject = new GameObject (NameOfEmplacement);
		EmplacementObject.transform.parent = Parent.transform;
		ListOfSticks = new List<Stick> ();
		float x = StickGameObject.GetComponent<RectTransform> ().rect.xMax;
		float y = StickGameObject.GetComponent<RectTransform> ().rect.yMax;
		float width = StickGameObject.GetComponent<RectTransform> ().rect.width;
		float height = StickGameObject.GetComponent<RectTransform> ().rect.height;
		for (int i = 0; i < TotalAmtOfStick; i++) 
		{
			GameObject stickObject = Instantiate(StickGameObject) as GameObject;
			stickObject.transform.SetParent (EmplacementObject.transform);
			stickObject.GetComponent<RectTransform> ().position = new Vector3 (StaticVars.xPixelPadding + x + (i * width) + (i * StaticVars.StickPadding), Screen.height - y - StaticVars.yPixelPadding - (index * (height + StaticVars.StickPadding)));

			Stick temp = stickObject.AddComponent<Stick> ();
			temp.TimeStart = StaticVars.StartDate.Add (new TimeSpan ((i * StaticVars.StickInHours) + StaticVars.StartHourOffset, 0, 0));
			temp.TimeEnd = temp.TimeStart.AddHours (StaticVars.StickInHours);
			stickObject.name = temp.TimeStart + " - " + temp.TimeEnd + "|" + NameOfEmplacement;
			temp.GUI = stickObject;
			ListOfSticks.Add (temp);

			stickObject.GetComponent<Button> ().onClick.AddListener(temp.onClick);
		}

		if (StaticVars.EndDate < ListOfSticks [ListOfSticks.Count - 1].TimeEnd) 
		{
			Debug.Log ("We have a unique case for the last stick and first stick!");
			StickUnique = true;
			// We will handle this later.
		}
	}

	public void RemoveStick (DateTime Time)
	{
		foreach (Stick a in ListOfSticks) 
		{
			if (a.TimeStart > Time && Time < a.TimeEnd) 
			{
				Debug.Log ("Removing " + Time.ToString ());
				Destroy (a.GUI);
				ListOfSticks.Remove (a);
				break;
			}
		}
	}
}
