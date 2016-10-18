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
	public int NoOfSticks;
	public Batch Parent;
	public List<Roles> ListOfRoles = new List<Roles>();

	public DateTime lastStickEndTiming;



	public Person (string tName, string tIC, DateTime tDOB, JSONNode tRoles)
	{
		Name = tName;
		IC = tIC;
		DOB = tDOB;
		ListOfRoles = new List<Roles> ();
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
		for (int i = 0; i < tRoles.Count; i++) 
		{
			ListOfRoles.Add (StaticVars.RolesParseJson(tRoles [i].Value));
		}
	}

	public bool IsSpecialRole ()
	{
		foreach (Roles tRole in ListOfRoles) 
		{
			if (tRole == Roles.eCONSOLE || tRole == Roles.eDRIVER || tRole == Roles.ePASS_OFFICE) 
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
		if ((StickStartTiming - lastStickEndTiming).Hours >= (StaticVars.RestAfterSticks * StaticVars.StickInHours)) 
		{
			return true;
		}
		return false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
