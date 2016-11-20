using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

public class Stick : MonoBehaviour, IPointerClickHandler {

	public DateTime TimeStart;
	public DateTime TimeEnd;
	public Person DutyPersonal;
	public Emplacement Parent;
	public bool Assigned = false;
	public GameObject GUI;
	public StickState State = StickState.ENABLED;
	public int StepIndex = 0;
	public bool Unique = false;
	public Stick Neighbour = null;

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
		Data.NoOfSticks--;
		Data.lastDoneEmplacement = Parent;
		Data.ListOfSticks.Add (this);
		// I need to update the ui here.
		GUI.transform.GetChild(0).GetComponent<Text>().text = Data.name;
	}

	public void AssignPersonWithoutChecks (Person Data)
	{
		DutyPersonal = Data;
		Assigned = true;
		Data.lastStickEndTiming = TimeEnd;
		Data.lastDoneEmplacement = Parent;
		Data.ListOfSticks.Add (this);
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

	public void OnPointerClick(PointerEventData eventData)
	{
//		if (eventData.button == PointerEventData.InputButton.Left) 
//		{
//		}
//		else if (eventData.button == PointerEventData.InputButton.Middle)
//		{
//
//		}
//		else if (eventData.button == PointerEventData.InputButton.Right)
//		{
//			Debug.Log ("Right click!");
//		}

		if(NamePanel.Instance.AutoCompleteBox.activeInHierarchy == false)
		{
			if (eventData.button == PointerEventData.InputButton.Left) 
			{
				NamePanel.Instance.AutoCompleteBox.SetActive(true);
				NamePanel.Instance.AutoCompleteBox.transform.FindChild("Name Panel").FindChild("InputField").GetComponent<InputField>().text = "";
				NamePanel.Instance.SelectedStick = this;
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				Debug.Log ("Right click!");
				if (State == StickState.ENABLED) 
				{
					State = StickState.DISABLED;
					gameObject.GetComponent<Image> ().color = new Color(0.2f,0.2f,0.2f,1);
				}
				else if(State == StickState.DISABLED)
				{
					State = StickState.ENABLED;
					gameObject.GetComponent<Image> ().color = Color.white;
				}
			}
		}
	}

	public void Reset ()
	{
		DutyPersonal = null;
		Assigned = false;
		GUI.transform.GetChild(0).GetComponent<Text>().text = "";
	}

	public void SwapReset()
	{
		DutyPersonal.NoOfSticks++;
		DutyPersonal.ListOfSticks.Remove(this);
		DutyPersonal = null;
		Assigned = false;
		GUI.transform.GetChild(0).GetComponent<Text>().text = "";
	}
}
