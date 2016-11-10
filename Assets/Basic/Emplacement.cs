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
	eARMOURER,
};

public class Emplacement : MonoBehaviour {

	public string NameOfEmplacement;
	public List<Stick> ListOfSticks;
	public int TotalAmtOfStick;
	public int Pirority;
	//public bool StickUnique = false;
    public Roles CurrentRole = Roles.eNONE;
	private bool AllAssigned = false;
	public bool Easy;

	// Use this for initialization
	void Start () 
	{
		//// Generate total amount of sticks based off time. This is yet to exclude the time that we are to remove.
		//GenerateSticks ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsSpecialRole ()
	{
		if (CurrentRole == Roles.eCONSOLE || CurrentRole == Roles.eDRIVER || CurrentRole == Roles.ePASS_OFFICE) 
		{
			return true;
		}
		return false;
	}

	public int NumberOfSticksUnAssigned()
	{
		int Counter = 0;
		foreach (Stick val in ListOfSticks) 
		{
			if (val.Assigned == false) 
			{
				Counter++;
			}
		}
		return Counter;
	}

	public void SetAllAssigned ()
	{
		foreach (Stick val in ListOfSticks) 
		{
			if (val.Assigned == true) 
			{
				continue;
			}
			else
			{
				AllAssigned = false;
				break;
			}
		}
		AllAssigned = true;
	}

	public bool GetAllAssigned ()
	{
		foreach (Stick val in ListOfSticks) 
		{
			if (val.Assigned == true) 
			{
				continue;
			}
			else
			{
				return false;
			}
		}
		return true;
	}

	public void GenerateSticks (GameObject Parent,GameObject StickGameObject,Roles EmplacementType,int tPirority,int index) 
	{
		Pirority = tPirority;
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
			// First Stick
			if (i == 0) 
			{
				temp.TimeStart = StaticVars.StartDate.Add (new TimeSpan ((i * StaticVars.StickInHours), 0, 0));
				temp.TimeEnd = temp.TimeStart.AddHours (StaticVars.StickInHours + StaticVars.StartHourOffset);
			}
			else if(i == TotalAmtOfStick - 1)
			{
				temp.TimeStart = ListOfSticks[i - 1].TimeEnd;
				temp.TimeEnd = temp.TimeStart.AddHours (StaticVars.StickInHours - StaticVars.StartHourOffset);
			}
			else
			{
				//Debug.Log (ListOfSticks [i - 1].TimeEnd + " - " + StaticVars.StartDate.Add (new TimeSpan ((i * StaticVars.StickInHours), 0, 0)));
				temp.TimeStart = ListOfSticks[i - 1].TimeEnd;
				temp.TimeEnd = temp.TimeStart.AddHours (StaticVars.StickInHours);
			}

			//temp.TimeEnd = temp.TimeStart.AddHours (StaticVars.StickInHours);
			temp.StepIndex = i;
			temp.Parent = this;
			stickObject.name = temp.TimeStart + " - " + temp.TimeEnd + "|" + NameOfEmplacement;
			temp.GUI = stickObject;
			ListOfSticks.Add (temp);

			stickObject.GetComponent<Button> ().onClick.AddListener(temp.onClick);
		}

//		if (StaticVars.EndDate < ListOfSticks [ListOfSticks.Count - 1].TimeEnd) 
//		{
//			Debug.Log ("We have a unique case for the last stick and first stick!");
//			StickUnique = true;
//			// We will handle this later.
//		}
	}

	public void RemoveStick (DateTime TimeStart, DateTime TimeEnd)
	{
		//List<Stick> RebuildList = new List<Stick> ();
		foreach (Stick a in ListOfSticks) 
		{
			if (a.TimeStart >= TimeStart && a.TimeEnd <= TimeEnd) 
			{
				//Debug.Log ("Removing " + a.TimeStart.ToString () + " - "+ a.TimeEnd.ToString());
				//Destroy (a.GUI);
				//ToRemoveSticks.Add (a);
				a.State = StickState.DISABLED;
				a.GUI.GetComponent<Image> ().color = Color.gray;
				Debug.Log (a.GUI.transform.GetChild(0).name);
				a.GUI.transform.GetChild(0).GetComponent<Text> ().text = "";
			}
//			else
//			{
//				RebuildList.Add (a);
//			}
		}
		//ListOfSticks = RebuildList;
	}

	public void Reset ()
	{
		foreach (Stick stick in ListOfSticks) 
		{
			stick.Reset ();
		}
	}

	public bool IsSpecialEmplacement ()
	{
		if (CurrentRole == Roles.eCONSOLE || 
			CurrentRole == Roles.eDRIVER || 
			CurrentRole == Roles.ePASS_OFFICE || 
			CurrentRole == Roles.eARMOURER) 
		{
			return true;
		}
		else
		{
			return false;
		}
		return false;
	}
}
