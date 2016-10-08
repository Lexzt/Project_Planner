﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Stick : MonoBehaviour {

	public DateTime TimeStart;
	public DateTime TimeEnd;
	public Person DutyPersonal;
	public Emplacement Parent;
	public bool Assigned = false;
	public GameObject GUI;
	public StickState State = StickState.ENABLED;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AssignPerson (Person Data)
	{
		DutyPersonal = Data;
		Assigned = true;
		Data.lastStickEndTiming = TimeEnd;
		Data.NoOfSticks++;

		// I need to update the ui here.
		GUI.transform.GetChild(0).GetComponent<Text>().text = Data.name;
	}

	public void DefineTimeStart(DateTime Start, int NoOfSticks)
	{
		TimeStart = Start;
		TimeEnd = TimeStart.AddHours (StaticVars.StickInHours * NoOfSticks);
	}

	public void onClick()
	{
		State += 1;
		if (State >= StickState.LAST) 
		{
			State = StickState.ENABLED;
		}
	}
}
