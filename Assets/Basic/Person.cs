﻿using UnityEngine;
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

	public bool IsRested (DateTime StickStartTiming)
	{
		if ((StickStartTiming - lastStickEndTiming).Hours >= (StaticVars.RestAfterSticks * StaticVars.StickInHours) || 
			(StickStartTiming - lastStickEndTiming).Days > 0) 
		{
			return true;
		}
		return false;
	}

	public void Reset ()
	{
		NoOfSticks = OriginNoOfSticks;
		lastDoneEmplacement = null;
		lastStickEndTiming = new DateTime();
	}

	public int GetHoursSinceLastStick (DateTime currentTime)
	{
		if (lastStickEndTiming == DateTime.MinValue) 
		{
			DateTime tTime = StaticVars.StartDate.AddHours (StaticVars.StickInHours * StaticVars.RestAfterSticks * -1);
			return (int)((currentTime - tTime).TotalHours);
		}
		else
		{
			return (int)(currentTime - lastStickEndTiming).TotalHours;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
