using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Person : MonoBehaviour {

	public string Name;
	public string IC;
	public DateTime DOB;
	public Texture2D Photo;
	public int NoOfSticks = 0;
	public Color color;
	public int OriginNoOfSticks = 0;
	public Batch Parent;
	public List<Roles> ListOfRoles = new List<Roles>();
	public List<Stick> ListOfSticks = new List<Stick>();
	public Emplacement lastDoneEmplacement = null;
	public DateTime lastStickEndTiming = new DateTime();

	public Person (string tName, string tIC, DateTime tDOB, JSONNode tRoles)
	{
		int id = GetInstanceID ();
		color.b = 0xFF & id;
		color.g = 0xFF & id>>8;
		color.r = 0xFF & id>>16;

		Name = tName;
		IC = tIC;
		DOB = tDOB;
		ListOfRoles = new List<Roles> ();
		lastStickEndTiming = new DateTime ();
		for (int i = 0; i < tRoles.Count; i++) 
		{
			ListOfRoles.Add (StaticVars.RolesParseJson(tRoles [i].ToString()));
		}
		ListOfSticks = new List<Stick> ();
	}

	public void Set (string tName, string tIC, DateTime tDOB, JSONNode tRoles)
	{
		int id = GetInstanceID ();
		color.b = 0xFF & id;
		color.g = 0xFF & id>>8;
		color.r = 0xFF & id>>16;

		Name = tName;
		IC = tIC;
		DOB = tDOB;
		ListOfRoles = new List<Roles> ();
		lastStickEndTiming = new DateTime ();
		for (int i = 0; i < tRoles.Count; i++) 
		{
			ListOfRoles.Add (StaticVars.RolesParseJson(tRoles [i].Value));
		}
		ListOfSticks = new List<Stick> ();
	}

	public JSONNode ToJSON ()
	{
		JSONNode node = new JSONClass ();
		node ["Name"] = Name;
		node ["IC"] = IC;
		node ["IC"] = IC;
		node ["DOB"] = DOB.ToString();

		JSONNode roleArray = new JSONArray ();
		for(int i = 0; i < ListOfRoles.Count; i++) 
		{
			roleArray [i] = StaticVars.RolesParseJson(ListOfRoles [i]);
		}

		node.Add ("Roles",roleArray);
		return node;
	}

	public bool IsSpecialRole ()
	{
		foreach (Roles tRole in ListOfRoles) 
		{
			if (tRole == Roles.eCONSOLE || 
				tRole == Roles.eDRIVER || 
				tRole == Roles.ePASS_OFFICE || 
				tRole == Roles.eARMOURER) 
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		return false;
	}

	public bool IsRested (DateTime StickStartTiming, DateTime StickEndTiming)
	{
		foreach (Stick lastStick in ListOfSticks) 
		{
			int HoursDiff = 0;
			int FirstStartDiff = (int)(StickStartTiming - lastStick.TimeStart).TotalHours;
			// The new stick starts AFTER the old stick starts.
			//Debug.Log ("0: " + StickStartTiming.ToString() + " - " + lastStick.TimeEnd.ToString());
			if (FirstStartDiff > 0) 
			{
				// The new stick is starting after the old stick starts. 
				int EndDiff = (int)(StickStartTiming - lastStick.TimeEnd).TotalHours;
				//Debug.Log ("1: " + EndDiff + " - " + StickStartTiming.ToString() + " - " + lastStick.TimeEnd.ToString());
				if (EndDiff > 0) 
				{
					// This means the old stick, ends before your new stick starts. 
					HoursDiff = Mathf.Abs(EndDiff);
				}
			}
			else if(FirstStartDiff < 0)
			{
				int EndDiff = (int)(StickEndTiming - lastStick.TimeStart).TotalHours;
				//Debug.Log ("2: " + EndDiff + " - " + StickStartTiming.ToString() + lastStick.TimeEnd.ToString());
				if (EndDiff < 0) 
				{
					// This means my new sticks, ends before my old stick starts.
					HoursDiff = Mathf.Abs(EndDiff);
				}
			}
			//Debug.Log ("3: " + HoursDiff);
			if (HoursDiff < StaticVars.RestAfterSticks * StaticVars.StickInHours) 
			{
				return false;
			}
			HoursDiff = 0;
		}
		return true;
	}

	public bool IsRestedExcludingSticks (DateTime StickStartTiming, DateTime StickEndTiming, params Stick[] ExcludeSticks)
	{
		foreach (Stick lastStick in ListOfSticks) 
		{
			bool Ignore = false;
			for(int i = 0; i < ExcludeSticks.Length; i++)
			{
				if(lastStick.TimeStart == ExcludeSticks[i].TimeStart && lastStick.TimeEnd == ExcludeSticks[i].TimeEnd)
				{
					Ignore = true;
				}
			}

			if(Ignore == false)
			{
				int HoursDiff = 0;

				// 
				int FirstStartDiff = (int)(StickStartTiming - lastStick.TimeStart).TotalHours;
				// The new stick starts AFTER the old stick starts.
				if (FirstStartDiff > 0) 
				{
					// The new stick is starting after the old stick starts. 
					int EndDiff = (int)(StickStartTiming - lastStick.TimeEnd).TotalHours;
					//Debug.Log ("1: " + EndDiff + " - " + StickStartTiming.ToShortTimeString() + " - " + lastStick.TimeEnd.ToShortTimeString());
					if (EndDiff > 0) 
					{
						// This means the old stick, ends before your new stick starts. 
						HoursDiff = Mathf.Abs(EndDiff);
						//if(HoursDiff
					}
				}
				else if(FirstStartDiff < 0)
				{
					int EndDiff = (int)(StickEndTiming - lastStick.TimeStart).TotalHours;
					//Debug.Log ("2: " + EndDiff + " - " + StickStartTiming.ToLocalTime() + lastStick.TimeEnd.ToLocalTime());
					if (EndDiff < 0) 
					{
						// This means my new sticks, ends before my old stick starts.
						HoursDiff = Mathf.Abs(EndDiff);
					}
				}
				//Debug.Log ("3: " + HoursDiff);
				if (HoursDiff < StaticVars.RestAfterSticks * StaticVars.StickInHours) 
				{
					return false;
				}
				HoursDiff = 0;
			}
		}
		return true;
		//}
		//return false;
	}

	public void Reset ()
	{
		NoOfSticks = OriginNoOfSticks;
		lastDoneEmplacement = null;
		lastStickEndTiming = new DateTime();
		ListOfSticks = new List<Stick> ();
	}

	public int GetHoursSinceLastStick (DateTime currentTime)
	{
		if (lastStickEndTiming == DateTime.MinValue) 
		{
			DateTime tTime = StaticVars.StartDate.AddHours (StaticVars.StickInHours * StaticVars.RestAfterSticks * -1);
			return Mathf.Abs((int)((currentTime - tTime).TotalHours));
		}
		else
		{
			return Mathf.Abs((int)(currentTime - lastStickEndTiming).TotalHours);
		}
	}

	public void AllReset ()
	{
		NoOfSticks = 0;
		OriginNoOfSticks = 0;
		ListOfSticks = new List<Stick>();
		lastDoneEmplacement = null;
		lastStickEndTiming = new DateTime();
	}

	public void RemoveMe()
	{
		Parent.ListOfPeople.Remove(this);
		Destroy(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		UnityEngine.Random.State prevState = UnityEngine.Random.state;
		UnityEngine.Random.InitState(GetInstanceID ());
		int id = UnityEngine.Random.Range (0, 0xFFFFFF);
		UnityEngine.Random.state = prevState;
		//Debug.Log (id);

		color.a = 1.0f;
		color.b = (0xF0 & id)/255.0f;
		color.g = (0xF0 & id>>8)/255.0f;
		color.r = (0xF0 & id>>16)/255.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
