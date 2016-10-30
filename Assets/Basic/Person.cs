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
	public int OriginNoOfSticks = 0;
	public Batch Parent;
	public List<Roles> ListOfRoles = new List<Roles>();
	public List<Stick> ListOfSticks = new List<Stick>();
	public Emplacement lastDoneEmplacement = null;
	public DateTime lastStickEndTiming = new DateTime();

	public Person (string tName, string tIC, DateTime tDOB, JSONNode tRoles)
	{
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
//		if ((StickStartTiming - lastStickEndTiming).Hours >= (StaticVars.RestAfterSticks * StaticVars.StickInHours) || 
//			(StickStartTiming - lastStickEndTiming).Days > 0) 
//		{
		foreach (Stick lastStick in ListOfSticks) 
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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
