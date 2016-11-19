using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Weighted_Randomizer;

[System.Serializable]
public class BatchClass
{
    public BatchClass(int tBatchAmount, int tNoOfPeronals, string tBatchName, bool tXZ = false)
    {
        BatchStick = new List<int[]>();
        BatchStick.Add(new int[] { tBatchAmount, tNoOfPeronals });
        NoOfPersonals = tNoOfPeronals;
        BatchName = tBatchName;
        XZ = tXZ;
    }
    public string BatchName;
    // It will be a list of 2 int array. 
    // So the first index, 0, will represent List of Personal for that amount of person.
    // the second index, 1, will be the amount of personal with that stick. 
    public List<int[]> BatchStick = new List<int[]>();
    public int NoOfPersonals;
    public bool XZ = false;
    public Batch BatchPersonalData;
}

[System.Serializable]
public class StepClass
{
    public string StepName = "";
    public List<Stick> ListOfSticks = new List<Stick>();
    public DateTime StartStepTime;
    public DateTime EndStepTime;
}

public class ChangiAirBaseEast : Base
{
    public DateTime StartDate;
    public DateTime EndDate;

    public GameObject BarObject;

    public GameObject ParentObject;
    public GameObject StickGameObject;

    public GameObject SearchInputField;

    private JSONNode root = null;

    public List<BatchClass> FinalizedList = new List<BatchClass>();

    public string dataFile = "";

    void Start()
    {
        Debug.Log("No Of Hours: " + (StaticVars.EndDate - StaticVars.StartDate).TotalHours);

        // Creation of Key Gameobjects, So when I parent them properly.
        GameObject BatchParents = new GameObject("Batches");
        GameObject EmplacementParents = new GameObject("Emplacements");

        // Parse the JSON to a JSONNode to use
        TextAsset Json = Resources.Load(dataFile) as TextAsset;
        root = JSON.Parse(Json.text);
        #region Batch Parsing
			/*
			* Here, I create the people object based off the JSON.
			* This allows it to be dynamic to shuffle around.
			*/
			for (int j = 0; j < root["Batches"].Count; j++)
			{
				GameObject BatchObj = new GameObject();
				BatchObj.transform.parent = BatchParents.transform;
				Batch tempBatch = BatchObj.AddComponent<Batch>();
				tempBatch.BatchName = root["Batches"][j]["BatchName"];
				tempBatch.BatchNo = j + 1;
				tempBatch.DoEasy = root["Batches"][j]["DoEasy"].AsBool;
				BatchObj.name = root["Batches"][j]["BatchName"];
				tempBatch.BaseParent = this.gameObject;
				base.Batches.Add(tempBatch);

				for (int i = 0; i < root["Batches"][j]["Personnels"].Count; i++)
				{
					//Debug.Log("Array: " + root["Batches"][j]["Personnels"][i]["Name"]);
					GameObject Person = new GameObject();
					Person.transform.parent = BatchObj.transform;
					Person tempPerson = Person.AddComponent<Person>();
					tempPerson.Set(root["Batches"][j]["Personnels"][i]["Name"],
						root["Batches"][j]["Personnels"][i]["IC"],
						new DateTime(root["Batches"][j]["Personnels"][i]["DOB"].AsInt),
						root["Batches"][j]["Personnels"][i]["Roles"]);
					tempPerson.Parent = tempBatch;
					Person.name = tempPerson.Name;
					tempBatch.AddPersonal(tempPerson);
				}
				GetComponent<UserManagementSystem> ().AddBatchData (BatchObj);
			}
		GetComponent<UserManagementSystem> ().DrawUI();

        #endregion

        //		#region ICT Creation
        //		if(StaticVars.NumberOfICT > 0)
        //		{
        //			GameObject ICTObject = new GameObject("ICT");
        //			ICTObject.transform.parent = BatchParents.transform;
        //			Batch ICTBatch = ICTObject.AddComponent<Batch>();
        //			ICTBatch.ICT = true;
        //			base.Batches.Add(ICTBatch);
        //			for(int i = 0; i < StaticVars.NumberOfICT; i++)
        //			{
        //				GameObject Person = new GameObject();
        //				Person.transform.parent = ICTObject.transform;
        //				Person tempPerson = Person.AddComponent<Person>();
        //				tempPerson.Set("ICT " + (i + 1), 
        //					"", 
        //					new DateTime(),
        //					root["ICT"]["Roles"]);
        //				tempPerson.NoOfSticks = StaticVars.NumberOfStickforICTWeekDay;
        //				tempPerson.OriginNoOfSticks = StaticVars.NumberOfStickforICTWeekDay;
        //				tempPerson.Parent = ICTBatch;
        //				Person.name = tempPerson.Name;
        //				ICTBatch.AddPersonal(tempPerson);
        //			}
        //		}
        //		#endregion

        #region Define Emplacements
			/*
			* This section of code runs through the JSON and based off whats defined in the json,
			* parses it into the object to create for the randomization.
			*/
			for (int i = 0; i < root["Emplacements"].Count; i++)
			{
				GameObject EmpObj = new GameObject();
				EmpObj.transform.parent = EmplacementParents.transform;

				Emplacement Emp1 = EmpObj.AddComponent<Emplacement>();
				Emp1.NameOfEmplacement = root["Emplacements"][i]["Name"];
				Emp1.Easy = root["Emplacements"][i]["Easy"].AsBool;

				EmpObj.name = Emp1.NameOfEmplacement;
				Emp1.GenerateSticks(ParentObject, StickGameObject, StaticVars.RolesParseJson(root["Emplacements"][i]["Role"]), root["Emplacements"][i]["Pirority"].AsInt, i);
				
				base.Emplacements.Add(Emp1);
				for(int j = 0; j < root["Emplacements"][i]["RemoveStick"].Count; j++)
				{
					DateTime parsedDateStart;
					DateTime parsedDateEnd;
 					if (DateTime.TryParseExact(root["Emplacements"][i]["RemoveStick"][j]["RemoveStart"].Value, "yyyy, M, dd, H, mm, ss", null, System.Globalization.DateTimeStyles.None, out parsedDateStart) && 
					 	DateTime.TryParseExact(root["Emplacements"][i]["RemoveStick"][j]["RemoveEnd"].Value, "yyyy, M, dd, H, mm, ss", null, System.Globalization.DateTimeStyles.None, out parsedDateEnd))
					{
						Emp1.RemoveStick(parsedDateStart,parsedDateEnd);
					}
					else
					{
						Debug.Log("Failed parse " + root["Emplacements"][i]["RemoveStick"][j]["RemoveStart"].Value + " - " + root["Emplacements"][i]["RemoveStick"][j]["RemoveEnd"].Value);
					}
				}
			}
        #endregion

        ParentObject.transform.localScale = new Vector3(0.7f, 0.7f, 0f);
        ParentObject.transform.localPosition = new Vector3(-115f, 0f, 0f);

        // #region Removing of Sticks
		// 	/*
		// 	* Here, I remove the sticks for the specific time and emplacements.
		// 	* Current, I am hard coding it, to remove based off real emplacements. 
		// 	* However, In the end, I need to think of a way to soft code it, or parse it in via the JSON.
		// 	*/
		// 	// Viper 1 Sentry
		// 	base.Emplacements[0].RemoveStick(new DateTime(2016, 8, 15, 14, 00, 00), new DateTime(2016, 8, 17, 14, 00, 00));

		// 	// Viper 1 Checker
		// 	base.Emplacements[1].RemoveStick(new DateTime(2016, 8, 15, 14, 00, 00), new DateTime(2016, 8, 16, 6, 00, 00));
		// 	base.Emplacements[1].RemoveStick(new DateTime(2016, 8, 16, 9, 00, 00), new DateTime(2016, 8, 16, 15, 00, 00));
		// 	base.Emplacements[1].RemoveStick(new DateTime(2016, 8, 16, 18, 00, 00), new DateTime(2016, 8, 17, 14, 00, 00));

		// 	// UVSS
		// 	base.Emplacements[6].RemoveStick(new DateTime(2016, 8, 15, 14, 00, 00), new DateTime(2016, 8, 16, 6, 00, 00));
		// 	base.Emplacements[6].RemoveStick(new DateTime(2016, 8, 16, 9, 00, 00), new DateTime(2016, 8, 16, 15, 00, 00));
		// 	base.Emplacements[6].RemoveStick(new DateTime(2016, 8, 16, 18, 00, 00), new DateTime(2016, 8, 17, 14, 00, 00));

		// 	// Python Sentry
		// 	base.Emplacements[2].RemoveStick(new DateTime(2016, 8, 15, 18, 00, 00), new DateTime(2016, 8, 16, 6, 00, 00));
		// 	base.Emplacements[2].RemoveStick(new DateTime(2016, 8, 16, 18, 00, 00), new DateTime(2016, 8, 17, 6, 00, 00));
		// 	base.Emplacements[2].RemoveStick(new DateTime(2016, 8, 16, 9, 00, 00), new DateTime(2016, 8, 16, 15, 00, 00));
		// 	base.Emplacements[2].RemoveStick(new DateTime(2016, 8, 17, 9, 00, 00), new DateTime(2016, 8, 17, 14, 00, 00));

		// 	// Python Checker
		// 	base.Emplacements[3].RemoveStick(new DateTime(2016, 8, 15, 18, 00, 00), new DateTime(2016, 8, 16, 6, 00, 00));
		// 	base.Emplacements[3].RemoveStick(new DateTime(2016, 8, 16, 18, 00, 00), new DateTime(2016, 8, 17, 6, 00, 00));

		// 	// Viper 2 Sentry
		// 	base.Emplacements[4].RemoveStick(new DateTime(2016, 8, 15, 18, 00, 00), new DateTime(2016, 8, 17, 12, 00, 00));

		// 	GenerateDictionary();
        // #endregion

        #region ICT Handling
        //int TotalNumberOfSticksCoveredByICT = root["ICT"]["NumberOfICT"].AsInt * root["ICT"]["NumberOfStickforICTWeekDay"].AsInt;
        #endregion

        #region Calculate Sticks
			CalculateSticks(true);
        #endregion

        #region Calculate Steps
        	CalculateSteps();
        #endregion

        // Reset all emplacement to NIL before we try to assign.
        Reset();
    }

    /*
	 * This function does handling for the InputField such that, it will tell me the closet value
	 * when compared to the dictionary.
	 */
    string[] GenerateDictionary()
    {
        List<string> OutputDictionary = new List<string>();
        for (int i = 0; i < Batches.Count; i++)
        {
            for (int j = 0; j < Batches[i].ListOfPeople.Count; j++)
            {
                //Debug.Log ("Batch: " + Batches [i].BatchName + " - " + Batches [i].ListOfPeople [j].Name);
                OutputDictionary.Add(Batches[i].ListOfPeople[j].Name);
            }
        }
        return OutputDictionary.ToArray();
    }

    /*
	 * Resets all emplacement and personal, GUI and Data included.
	 */
    public void Reset()
    {
        foreach (Emplacement emp in base.Emplacements)
        {
            emp.Reset();
        }

        foreach (Batch batch in base.Batches)
        {
            batch.Reset();
        }
    }

    /*
	 * The main function to assign sticks.
	 */
    public void AssignSticks()
    {
        OrderSteps();

        AssignPass(false, true);
        AssignPass(true, true);
        AssignPass(false, false);

       // DebugEmplacements();
       // DebugPersonel();

        while (PossibilityCheck() == false)
        {
            Debug.Log("Loop!");

            Reset();
            AssignPass(false, true);
            AssignPass(true, true);
            AssignPass(false, false);
        }

		#region LastPass
			// This is the last pass, where we will Assign specially with the new style.
			List<StepClass> ListOfStepsWithUnassignedSticks = new List<StepClass>();
			List<int> NumberOfSticksUnassigned = new List<int>();
			List<Person> UnassignedPeople = new List<Person>();
			GenerateUnassignedStepsAndSticks(ref ListOfStepsWithUnassignedSticks, ref NumberOfSticksUnassigned);
			GenerateUnassignedPersonels(ref UnassignedPeople);

			// Debug.Log(UnassignedPeople.Count);

			// // Here, I need to do an important check, 
			// // To check if there is someone to be able to do at that timeslot based off the number of sticks left avaliable on that time.
			// // The amount of sticks left unassigned in a step is in the list of int
			// // And the step is in the list.
			// // So, I just need to cycle through them.
			// if (ListOfStepsWithUnassignedSticks.Count == NumberOfSticksUnassigned.Count)
			// {
			// 	List<int> ListOfPeopleWhoCanDoThatStep = new List<int>();
			// 	for (int i = 0; i < ListOfStepsWithUnassignedSticks.Count; i++)
			// 	{
			// 		int Counter = 0;
			// 		foreach (Person personal in UnassignedPeople)
			// 		{
			// 			if (personal.IsRested(ListOfStepsWithUnassignedSticks[i].StartStepTime, ListOfStepsWithUnassignedSticks[i].EndStepTime))
			// 			{
			// 				//Debug.Log(personal.Name);
			// 				Counter++;
			// 			}
			// 		}
			// 		//Debug.Log(ListOfStepsWithUnassignedSticks[i].StartStepTime + " has " + NumberOfSticksUnassigned[i] + " sticks and " + Counter + " people who can do it.");
			// 		if (Counter >= NumberOfSticksUnassigned[i])
			// 		{
			// 			//Debug.Log(ListOfStepsWithUnassignedSticks[i].StartStepTime + " has no problem");
			// 		}
			// 		else
			// 		{
			// 			//Debug.Log(ListOfStepsWithUnassignedSticks[i].StartStepTime + " has problem " + NumberOfSticksUnassigned[i] + " - " + Counter);
			// 		}
			// 	}
			// }
			// else
			// {
			// 	Debug.Log("Smth wrong!!");
			// }

			int WhileHold = 0;
			while (WhileHold != 5)
			{
				// Now, for the last part, I need to check how many people can do that specific step.
				for (int i = 0; i < ListOfStepsWithUnassignedSticks.Count; i++)
				{
					List<Person> NumberOfPeopleWhoCanDoThatStick = new List<Person>();
					for (int j = 0; j < ListOfStepsWithUnassignedSticks[i].ListOfSticks.Count; j++)
					{
						Stick tempData = ListOfStepsWithUnassignedSticks[i].ListOfSticks[j];
						if (tempData.Assigned == false && tempData.Unique == true)
						{
							foreach (Person personal in UnassignedPeople)
							{
								if (personal.IsRested(tempData.TimeStart, tempData.TimeEnd) &&
									personal.ListOfRoles.Contains(tempData.Parent.CurrentRole) &&
									personal.NoOfSticks - 1 >= 0)
								{
									NumberOfPeopleWhoCanDoThatStick.Add(personal);
								}
							}
							break;
						}
					}

					//Debug.Log (ListOfStepsWithUnassignedSticks[i].StartStepTime + " - " + NumberOfPeopleWhoCanDoThatStick.Count + " - " + NumberOfSticksUnassigned [i]);
					if (NumberOfPeopleWhoCanDoThatStick.Count == NumberOfSticksUnassigned[i])
					{
						//Debug.Log (ListOfStepsWithUnassignedSticks [i].StartStepTime + " has " + NumberOfSticksUnassigned [i] + " unassigned sticks, and " + NumberOfPeopleWhoCanDoThatStick.Count + " people can do it");
						//					foreach (Person people in NumberOfPeopleWhoCanDoThatStick) 
						//					{
						//						Debug.Log ("Person are " + people.Name);
						//					}

						for (int j = 0; j < ListOfStepsWithUnassignedSticks[i].ListOfSticks.Count; j++)
						{
							Stick tempData = ListOfStepsWithUnassignedSticks[i].ListOfSticks[j];
							if (tempData.Assigned == false && tempData.Unique == true)
							{
								int index = Random.Range(0, NumberOfPeopleWhoCanDoThatStick.Count);
								tempData.AssignPerson(NumberOfPeopleWhoCanDoThatStick[index]);
								NumberOfPeopleWhoCanDoThatStick.RemoveAt(index);
								NumberOfSticksUnassigned[i]--;
							}
						}
					}

					if (NumberOfSticksUnassigned[i] == 0)
					{
						ListOfStepsWithUnassignedSticks.RemoveAt(i);
						NumberOfSticksUnassigned.RemoveAt(i);
					}
				}

				// The idea is that, if all the emplacement is a ssigned, break the loop.
				bool AllAssigned = true;
				foreach (Emplacement emp in base.Emplacements)
				{
					if (emp.GetAllAssigned() == false)
					{
						AllAssigned = false;
						break;
					}
				}
				if (AllAssigned == true)
				{
					Debug.Log("Breaking loop cause all assigned!");
					break;
				}

				WhileHold++;
			}
        #endregion

        #region Check if can fill up Anymore
			/*
			* This check based off all the sticks left, if there is a personal who can only do at that specific timeslot.
			*/
			List<Person> UnassignedPeopleCheck = new List<Person>();
			foreach (Batch batch in base.Batches)
			{
				foreach (Person personal in batch.ListOfPeople)
				{
					if (personal.NoOfSticks > 0)
					{
						UnassignedPeopleCheck.Add(personal);
					}
				}
			}

			foreach (Person unassignedPerson in UnassignedPeopleCheck)
			{
				List<StepClass> UnassignedSteps = new List<StepClass>();
				for (int i = 0; i < base.Steps.Count; i++)
				{
					for (int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
					{
						Stick tempData = base.Steps[i].ListOfSticks[j];
						if (tempData.Assigned == false)
						{
							// Store the step
							UnassignedSteps.Add(base.Steps[i]);
							break;
						}
					}
				}

				List<StepClass> StepsHeCanUse = new List<StepClass>();
				for (int i = 0; i < UnassignedSteps.Count; i++)
				{
					if (unassignedPerson.IsRested(UnassignedSteps[i].StartStepTime, UnassignedSteps[i].EndStepTime) &&
						unassignedPerson.NoOfSticks - 1 >= 0)
					{
						//Debug.Log("Adding step " + UnassignedSteps[i].StartStepTime);
						StepsHeCanUse.Add(UnassignedSteps[i]);
					}
				}
				//Debug.Log(unassignedPerson.Name + " - " + UnassignedSteps.Count + " - " + StepsHeCanUse.Count);

				if (StepsHeCanUse.Count == 1)
				{
					List<Stick> UnassignedStickForStep = new List<Stick>();
					foreach (Stick value in StepsHeCanUse[0].ListOfSticks)
					{
						if (value.Assigned == false)
						{
							UnassignedStickForStep.Add(value);
						}
					}
					int index = Random.Range(0, UnassignedStickForStep.Count);
					//Debug.Log(unassignedPerson.Name + " - " + UnassignedStickForStep[index].TimeStart + " - " + UnassignedStickForStep[index].Parent.NameOfEmplacement);
					UnassignedStickForStep[index].AssignPerson(unassignedPerson);
				}
			}
        #endregion

        // #region Single pass Swap Check
		// 	List<StepClass> ListOfStepsWithUnassignedSticksForSmartSwap = new List<StepClass>();
		// 	for (int i = 0; i < base.Steps.Count; i++)
		// 	{
		// 		for (int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
		// 		{
		// 			if (base.Steps[i].ListOfSticks[j].Assigned == false && base.Steps[i].ListOfSticks[j].Unique == true)
		// 			{
		// 				ListOfStepsWithUnassignedSticksForSmartSwap.Add(base.Steps[i]);
		// 				break;
		// 			}
		// 		}
		// 	}

		// 	List<StepClass> ListOfStepsWithUnique = new List<StepClass>();
		// 	for (int i = 0; i < base.Steps.Count; i++)
		// 	{
		// 		for (int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
		// 		{
		// 			if (base.Steps[i].ListOfSticks[j].Unique == true)
		// 			{
		// 				ListOfStepsWithUnique.Add(base.Steps[i]);
		// 				break;
		// 			}
		// 		}
		// 	}

		// 	List<Person> SmartSwapPerson = new List<Person>();
		// 	foreach (Batch batch in base.Batches)
		// 	{
		// 		foreach (Person personal in batch.ListOfPeople)
		// 		{
		// 			if (personal.NoOfSticks == 1)
		// 			{
		// 				SmartSwapPerson.Add(personal);
		// 			}
		// 		}
		// 	}

		// 	foreach (Person personal in SmartSwapPerson)
		// 	{
		// 		foreach (StepClass step in ListOfStepsWithUnassignedSticksForSmartSwap)
		// 		{
		// 			// Make sure he cannot do the stick at that time.
		// 			if (personal.IsRested(step.StartStepTime, step.EndStepTime) == false)
		// 			{
		// 				// Now I need to find the right time to swap for him.
		// 				// So I need to check all the other steps, and see what time I can put him in.
		// 				// Also, Since its for unique stick only, I have to check unique for the stick too.
		// 				List<StepClass> ListOfTimeHeCanDo = new List<StepClass>();
		// 				for (int i = 0; i < ListOfStepsWithUnique.Count; i++)
		// 				{
		// 					if (personal.IsRested(ListOfStepsWithUnique[i].StartStepTime, ListOfStepsWithUnique[i].EndStepTime))
		// 					{
		// 						ListOfTimeHeCanDo.Add(base.Steps[i]);
		// 					}
		// 				}

		// 				Debug.Log("Personal: " + personal.Name);
		// 				foreach (StepClass whatHeCanDo in ListOfTimeHeCanDo)
		// 				{
		// 					Debug.Log(whatHeCanDo.StartStepTime + " - " + whatHeCanDo.EndStepTime);
		// 					for(int j = 0; j < whatHeCanDo.ListOfSticks.Count; j++)
		// 					{
		// 						// Take note, Have not done check for good emplacement yet. 
		// 						if(whatHeCanDo.ListOfSticks[j].Unique == true && whatHeCanDo.ListOfSticks[j].Assigned == true)
		// 						{
		// 							Debug.Log(whatHeCanDo.ListOfSticks[j].Parent.NameOfEmplacement + " - " + whatHeCanDo.ListOfSticks[j].DutyPersonal.Name);
		// 							Person OffStickPersonel = whatHeCanDo.ListOfSticks[j].DutyPersonal;
		// 							if(OffStickPersonel.IsRested(step.StartStepTime, step.EndStepTime))
		// 							{
		// 								whatHeCanDo.ListOfSticks[j].SwapReset();
		// 								for(int k = 0; k < step.ListOfSticks.Count; k++)
		// 								{
		// 									if(step.ListOfSticks[k].Assigned == false)
		// 									{
		// 										step.ListOfSticks[k].AssignPerson(OffStickPersonel);
		// 									}
		// 								}
		// 							}
		// 						}
		// 					}
		// 				}
		// 			}
		// 			else
		// 			{
		// 				Debug.Log("Assign " + personal.Name + " timing " + step.StartStepTime.ToLongTimeString());
		// 			}
		// 		}
		// 	}
		// #endregion

       // DebugEmplacements();
       // DebugPersonel();

		#region Swap Check 2 Sticks
			UnassignedPeople = new List<Person>();
			GenerateUnassignedPersonels(ref UnassignedPeople);

			List<Stick> UnassignedSticks = new List<Stick>();
			GenerateUnassignedSticks(ref UnassignedSticks);
			foreach(Stick otherStick in UnassignedSticks)
			{
				//Debug.Log(UnassignedSticks.Count + " - " + otherStick.TimeStart.ToString() + " - " + otherStick.TimeEnd.ToString());
				for(int i = 0; i < UnassignedPeople.Count; i++)
				{
					//Debug.Log(UnassignedPeople.Count + UnassignedPeople[i].Name);
					if(UnassignedPeople[i].NoOfSticks == 1)
					{
						for(int j = 0; j < UnassignedPeople[i].ListOfSticks.Count; j++)
						{
							Stick tempStick = UnassignedPeople[i].ListOfSticks[j];
							if(tempStick.Unique == true)
							{
								//Debug.Log("Swapping Reset! " + tempStick.TimeStart.ToString() + " - " + tempStick.TimeEnd.ToString() + " \\ " + tempStick.Parent.NameOfEmplacement);
								tempStick.SwapReset();
								// I need to find someone to take over the tempStick duties before I exit this loop.
								// Prefer if the person can swap without doing multiple checks.
								bool exit = false; 
								for(int k = 0; k < base.Steps.Count; k++)
								{
									for(int l = 0; l < base.Steps[k].ListOfSticks.Count; l++)
									{
										if(exit == false)
										{
											Stick otherPersonDuty = base.Steps[k].ListOfSticks[l];
											if(otherPersonDuty.Assigned == true)
											{
												if(otherPersonDuty.Unique == false && otherPersonDuty.Neighbour != null)
												{
													Stick[] tempArray = new Stick[2]{otherPersonDuty,otherPersonDuty.Neighbour};
													// Debug.Log("1:" + otherPersonDuty.DutyPersonal.Name + " - " +otherPersonDuty.TimeStart + " \\" + otherPersonDuty.TimeEnd + " - " + otherPersonDuty.Parent.NameOfEmplacement);
													// Debug.Log("2: " + 	otherPersonDuty.DutyPersonal.IsRested(tempStick.TimeStart,tempStick.TimeEnd) + " - " + 
													// 	otherPersonDuty.Neighbour.DutyPersonal.IsRested(tempStick.TimeStart,tempStick.TimeEnd) + " - " + 
													// 	UnassignedPeople[i].IsRested(otherPersonDuty.TimeStart,otherPersonDuty.TimeEnd) + " - " +  
													// 	otherPersonDuty.DutyPersonal.IsRestedExcludingSticks(otherStick.TimeStart,otherStick.TimeEnd, tempArray));
													// Debug.Log("3: " + otherPersonDuty.DutyPersonal.Name + " - " + UnassignedPeople[i].Name);
													// Debug.Log("4: " + otherStick.TimeStart + " - " + otherStick.TimeEnd);
													// Debug.Log("5: " + tempStick.TimeStart + " - " + tempStick.TimeEnd);
													if(	otherPersonDuty.DutyPersonal.IsRestedExcludingSticks(tempStick.TimeStart,tempStick.TimeEnd, tempArray) && 
														otherPersonDuty.Neighbour.DutyPersonal.IsRestedExcludingSticks(tempStick.TimeStart,tempStick.TimeEnd, tempArray) &&
														UnassignedPeople[i].IsRested(otherPersonDuty.TimeStart,otherPersonDuty.TimeEnd) && 
														otherPersonDuty.DutyPersonal.IsRestedExcludingSticks(otherStick.TimeStart,otherStick.TimeEnd, tempArray))
													{
														// Debug.Log("2: " + otherPersonDuty.DutyPersonal.Name + " - " +otherPersonDuty.TimeStart + " \\" + otherPersonDuty.TimeEnd + " - " + otherPersonDuty.Parent.NameOfEmplacement);
														Person OtherDutyPerson = otherPersonDuty.DutyPersonal;
														otherPersonDuty.SwapReset();
														otherPersonDuty.Neighbour.SwapReset();
														otherPersonDuty.AssignPerson(UnassignedPeople[i]);
														otherPersonDuty.Neighbour.AssignPerson(UnassignedPeople[i]);
														tempStick.AssignPerson(OtherDutyPerson);
														otherStick.AssignPerson(OtherDutyPerson);
														exit = true;
														break;
													}
												} 
											}
										}
									}
								}
								break;
							}
						}
					}
				}

				//AssignPass(false, false);
			}
		#endregion

//        #region Debug to check final reuslts
//        foreach (Emplacement emp in base.Emplacements)
//			{
//				emp.SetAllAssigned();
//				if (emp.GetAllAssigned() == false && !emp.IsSpecialRole())
//				{
//					Debug.Log(emp.NameOfEmplacement + " has " + emp.NumberOfSticksUnAssigned() + " sticks");
//				}
//			}
//
//			foreach (Batch batch in base.Batches)
//			{
//				foreach (Person personal in batch.ListOfPeople)
//				{
//					if (personal.NoOfSticks > 0)
//					{
//						Debug.Log(personal.name + " has " + personal.NoOfSticks + " left");
//					}
//				}
//			}
//        #endregion
    }

    // Handles the keypress for assignment of sticks and reset primarily.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
			CalculateSticks (true);
			CalculateSteps ();
            AssignSticks();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

		if (Input.GetKeyDown (KeyCode.U)) 
		{
			JSONNode root = new JSONClass ();
			JSONNode batchArray = new JSONArray ();
			foreach (Batch tempBatch in base.Batches) 
			{
				batchArray.Add (tempBatch.ToJSON ());
			}
			root.Add ("Batches", batchArray);
			Debug.Log (root.ToString ());

			JSONNode empRoot = new  JSONClass();
			foreach(Emplacement emp in base.Emplacements)
			{
				empRoot.Add(emp.NameOfEmplacement,emp.ToJSON());
			}
			root.Add("Emplacements", empRoot);
			Debug.Log(root.ToString());
		}
    }

    /*
	 * This function calculates the amount of sticks based off the number given to it,
	 * and assigns it automatically with the Xin Jiao System.
	 */
    void CalculateSticks(bool MaxXZ = true)
    {
		int NoOfSticks = 0;
		foreach(Emplacement emp in base.Emplacements)
		{
			if(emp.IsSpecialRole() == false)
			{
				foreach(Stick value in emp.ListOfSticks)
				{
					if(value.State == StickState.ENABLED)
					{
						NoOfSticks++;
					}
				}
			}
		}
		Debug.Log("Number of sticks: " + NoOfSticks);

		foreach (Batch tempBatch in base.Batches) 
		{
			tempBatch.AllReset ();
		}

        #region Calcuation of sticks
			//int NoOfSticks = 34;
			//bool MaxXZ = true;

			// First, We need to pull out the personal who are not standing sticks.
			// Aka, driver, console. This will be soft coded in the future so that they can choose who to pull.

			// To do this, we create a new json node, so we dont override it by accident.
			JSONNode subNode = JSON.Parse(root.ToString());

			Debug.Log("Subnode: " + subNode.ToString());
			// Now, we start by removing the drivers and console personal.
			for (int j = 0; j < subNode["Batches"].Count; j++)
			{
				List<JSONNode> indexToRemove = new List<JSONNode>();
				for (int i = 0; i < subNode["Batches"][j]["Personnels"].Count; i++)
				{
					//Debug.Log(subNode ["Batches"] [j] ["Personnels"] [i] ["Name"]+ " - " + subNode["Batches"][j]["Personnels"][i]["Roles"][0].Value);
					if (subNode["Batches"][j]["Personnels"][i]["Roles"][0].Value == "Driver")
					{
						Debug.Log("Driver detected! " + subNode["Batches"][j]["Personnels"][i]["Name"]);
						indexToRemove.Add(subNode["Batches"][j]["Personnels"][i]);
					}
					else if (subNode["Batches"][j]["Personnels"][i]["Roles"][0].Value == "Console")
					{
						Debug.Log("Console detected! " + subNode["Batches"][j]["Personnels"][i]["Name"]);
						indexToRemove.Add(subNode["Batches"][j]["Personnels"][i]);
					}
					else if (subNode["Batches"][j]["Personnels"][i]["Roles"][0].Value == "Giro")
					{
						Debug.Log("Giro detected! " + subNode["Batches"][j]["Personnels"][i]["Name"]);
						indexToRemove.Add(subNode["Batches"][j]["Personnels"][i]);
					}
					else if (subNode["Batches"][j]["Personnels"][i]["Roles"][0].Value == "Armorer")
					{
						Debug.Log("Armorer detected! " + subNode["Batches"][j]["Personnels"][i]["Name"]);
						indexToRemove.Add(subNode["Batches"][j]["Personnels"][i]);
					}
				}

				for (int k = 0; k < indexToRemove.Count; k++)
				{
					Debug.Log("Removing " + indexToRemove[k].ToString());
					subNode["Batches"][j]["Personnels"].Remove(indexToRemove[k]);
				}

				if (subNode["Batches"][j]["Personnels"].Count == 0)
				{
					Debug.Log("Removing batch!");
					//subNode["Batches"].Remove (subNode ["Batches"] [j]);
				}
			}

			Debug.Log("Subnode: " + subNode.ToString());
			Debug.Log("Root: " + root.ToString());

			// Now that I have the subnode, which have the latest information.
			// I know which personal I can use. I can now assign the dictionary of lists.
			// The idea is to have an array of ints, for example
			// In my camp, I have 4 batches that I can assign stick. But it might not be 4. 
			// So, based off the json, I can then think of how many batches I can use.
			int CurrentNoOfBatches = subNode["Batches"].Count;

			// Currently, I am going to hard code the variable of number of stick for xz.
			int XZBaseStick = StaticVars.MaxNoPerMountWeekDay;

			// Define the stick count list, not the finalize number
			List<BatchClass> StickCount = new List<BatchClass>();

			// I will force add the base stick for them first. 
			// Then, I will start the loop from 1 to ignore the xz.

			// I will now random my first stick first.
			// Then, I will use recursion and algo to find the proper number of stick per batch.
			// So now, I have the base stick for amount of personals, which are still random
			// I need to link the amount of sticks, to the amount of people per batch. 
			for (int i = 0; i < CurrentNoOfBatches; i++)
			{
				if (i == 0)
				{
					//Debug.Log (XZBaseStick + " - " + subNode ["Batches"] [i] ["Personnels"].Count);
					StickCount.Add(new BatchClass(XZBaseStick, subNode["Batches"][i]["Personnels"].Count, subNode["Batches"][i]["BatchName"], true));
				}
				else
				{
					//Debug.Log (XZBaseStick - i + " - "+ subNode ["Batches"] [i] ["Personnels"].Count);
					StickCount.Add(new BatchClass(XZBaseStick - i, subNode["Batches"][i]["Personnels"].Count, subNode["Batches"][i]["BatchName"]));
				}
			}

			// Now, Since I have assignd the total amount of stick per batch, I need to ensure it is valid.
			// I will do this using the recursion loop.
			// However, I need to fill up the json. I will do that tomorrow night.
			// Recursion is going to be dangerous. I have not taught of the full algo, so I am going to write pseudo code first.

			// I will now calculate the total amount of sticks.
			// This is the first case scenario, We will assume they have all the same amount of sticks.
			// This will attempt to calculate if they are the same. If not, we need to do recursive.
			int TotalSticks = 0;
			foreach (BatchClass temp in StickCount)
			{
				int val = temp.BatchStick[0][0] * temp.BatchStick[0][1];
				//Debug.Log(temp.BatchName + " - " + val + " - " + temp.NoOfPersonals);
				TotalSticks += val;
			}
			Debug.Log("Here:" + TotalSticks);

			// Now, I need to calculate the diff in sticks. Which is the most important value.
			// This value determines if we increase or decrease the value. 
			int diff = NoOfSticks - TotalSticks;
			//Debug.Log(NoOfSticks + " - " + TotalSticks);
			List<List<int[]>> CurrentConfiguration = new List<List<int[]>>();
			foreach (BatchClass temp in StickCount)
			{
				//Debug.Log(temp.BatchStick[0][0] * temp.BatchStick[0][1]);
				CurrentConfiguration.Add(temp.BatchStick);
			}

			bool loop = true;
			int breakpoint = 0;

			Debug.Log("Diff: " + diff);
			// Here, I do my condition checks. 
			if (diff == 0)
			{
				// Yay! We dont need to calculate anymore
				//Debug.Log("Finalized");
				loop = false;

			}
			else
			{
				// Okay, So we are now cant return the value yet. Cause our configuration is wrong.
				// We need to change a specific one.
				foreach (BatchClass temp in StickCount)
				{
					if (Mathf.Abs(diff) == temp.NoOfPersonals && temp.XZ != MaxXZ)
					{
						Debug.Log("Diff == BatchNo!");
						// We can increase that batch total amount of stick, 
						if (diff > 0)
						{
							temp.BatchStick[0][1]++;
							loop = false;
							break;
						}
						else if (diff < 0)
						{
							temp.BatchStick[0][1]--;
							loop = false;
							break;
						}
						//Random.Range(0,temp.NoOfPersonals);
					}
				}

				// Okay, so when we reach here, it means that the avail manpower does not equals to the diff sticks.
				// So with this, It means, we have to have uneven stick for one batch.

				// Okay, This means we need the value is negative. 
				// This means we have to too much stick with our current configuration.
				// So we have to decrease our previous configuration. 
				while (loop && breakpoint != 10)
				{
					int sticksLeft = Mathf.Abs(diff);
					for (int i = StickCount.Count - 1; i > 0; i--)
					{
						BatchClass temp = StickCount[i];
						if (temp.XZ != MaxXZ)
						{
							// This is for max xz
							//Debug.Log(sticksLeft + " - " + temp.BatchName + " - " + temp.NoOfPersonals + " - " + diff);
							if (sticksLeft > temp.NoOfPersonals && temp.NoOfPersonals > 0)
							{
								// Now, I will minus the amount of sticks left.
								sticksLeft -= temp.NoOfPersonals;
								if (diff < 0)
								{
									//Debug.Log("Minus");
									// Now, I reconfigure the sticks for when we need less manpower.
									temp.BatchStick[0][0]--;
								}
								else if (diff > 0)
								{
									//Debug.Log("Plus");
									// Now, I reconfigure the sticks for when we need more manpower.
									temp.BatchStick[0][0]++;
								}
							}
							else if (temp.NoOfPersonals > 0)
							{
								//Debug.Log("No of sticks left = " + sticksLeft);
								// This means we do not have enough manpower. 
								// This means we need to uneven stick.
								int PreviousAmt = temp.BatchStick[0][0];
								temp.BatchStick = new List<int[]>();

								if (diff < 0)
								{
									int FirstAmt = temp.NoOfPersonals - sticksLeft;
									int SecAmt = temp.NoOfPersonals - FirstAmt;
									//Debug.Log("First Second " + FirstAmt + " - " + SecAmt + " - " + PreviousAmt);
									temp.BatchStick.Add(new int[] { PreviousAmt - 1, SecAmt });
									temp.BatchStick.Add(new int[] { PreviousAmt, FirstAmt });
								}
								else
								{
									int FirstAmt = temp.NoOfPersonals - sticksLeft;
									int SecAmt = temp.NoOfPersonals - FirstAmt;
									//Debug.Log("First Second " + FirstAmt + " - " + SecAmt + " - " + PreviousAmt);
									temp.BatchStick.Add(new int[] { PreviousAmt + 1, SecAmt });
									temp.BatchStick.Add(new int[] { PreviousAmt, FirstAmt });
								}
								break;
							}
						}
						else
						{
							// This is for not max xz

						}
					}
					breakpoint++;
					int count = 0;
					foreach (BatchClass countStick in StickCount)
					{
						foreach (int[] val in countStick.BatchStick)
						{
							count += val[0] * val[1];
							Debug.Log("(" + countStick.BatchName + ")" + val[1] + " People do " + val[0] * val[1] + " - " + val[0] + " each");
							if (val[1] == 0 && val[0] == 0)
							{
								StickCount.Remove(countStick);
								break;
							}
						}
					}
					Debug.Log(breakpoint + " Times - Count: " + count);
					if (count != NoOfSticks)
					{
						diff = NoOfSticks - count;
						continue;
					}
					else
					{
						FinalizedList = StickCount;
						loop = false;
					}
				}
			}

			int finalCount = 0;
			foreach (BatchClass temp in StickCount)
			{
				//Debug.Log (temp.BatchName);
				foreach (int[] val in temp.BatchStick)
				{
					finalCount += val[0] * val[1];
					//Debug.Log (val[0] * val[1]);
				}

				foreach (Batch batchData in Batches)
				{
					if (batchData.BatchName == temp.BatchName)
					{
						temp.BatchPersonalData = batchData;
						batchData.ClassData = temp;
						//Debug.Log(temp.BatchStick.Count + " - " + temp.BatchName);
						if (temp.BatchStick.Count > 1)
						{
							for (int NoOfIndex = 0; NoOfIndex < temp.BatchStick.Count; NoOfIndex++)
							{
								int NoOfLoops = 0;
								int LoopNo = temp.BatchStick[NoOfIndex][1];
								while (NoOfLoops != LoopNo)
								{
									int RandomIndex = Random.Range(0, batchData.ListOfPeople.Count);
									if (!batchData.ListOfPeople[RandomIndex].IsSpecialRole() && batchData.ListOfPeople[RandomIndex].NoOfSticks == 0)
									{
										if (temp.BatchStick[NoOfIndex][1] > 0)
										{
											batchData.ListOfPeople[RandomIndex].NoOfSticks = temp.BatchStick[NoOfIndex][0];
											batchData.ListOfPeople[RandomIndex].OriginNoOfSticks = temp.BatchStick[NoOfIndex][0];
											temp.BatchStick[NoOfIndex][1]--;
											NoOfLoops++;
										}
									}
								}
							}
						}
						else
						{
							foreach (Person personal in batchData.ListOfPeople)
							{
								if (!personal.IsSpecialRole())
								{
									//Debug.Log("Setting " + personal.Name + " to " + temp.BatchStick[0][0]);
									personal.NoOfSticks = temp.BatchStick[0][0];
									personal.OriginNoOfSticks = personal.NoOfSticks;
								}
							}
						}
					}
				}
			}
			Debug.Log("Count: " + finalCount);
        #endregion
    }

    void CalculateSteps()
    {
        #region Steps Calulations
			base.Steps = new List<StepClass>();
			int TotalAmtOfStick = (int)(StaticVars.EndDate - StaticVars.StartDate).TotalHours / StaticVars.StickInHours;
			for (int i = 0; i < TotalAmtOfStick; i++)
			{
				StepClass tempStep = new StepClass();
				foreach (Emplacement emp in base.Emplacements)
				{
					if (!emp.IsSpecialRole())
					{
						foreach (Stick stick in emp.ListOfSticks)
						{
							if (stick.Assigned == false && stick.StepIndex == i && stick.State == StickState.ENABLED)
							{
								tempStep.ListOfSticks.Add(stick);
								break;
							}
						}
					}
				}
				tempStep.StartStepTime = tempStep.ListOfSticks[0].TimeStart;
				tempStep.EndStepTime = tempStep.ListOfSticks[0].TimeEnd;
				tempStep.StepName = tempStep.StartStepTime.ToString() + " - " + tempStep.EndStepTime.ToString();
				base.Steps.Add(tempStep);
			}

			foreach (Emplacement emp in base.Emplacements)
			{
				//foreach(Stick stick in emp.ListOfSticks)
				for (int i = 0; i < emp.ListOfSticks.Count; i++)
				{
					Stick stick = emp.ListOfSticks[i];
					if(stick.State == StickState.ENABLED)
					{
						if ((stick.TimeEnd - stick.TimeStart).Hours % 3 != 0)
						{
							stick.Unique = true;
						}

						if (i + 1 == emp.ListOfSticks.Count)
						{
							stick.Unique = true;
						}

						if (i + 1 != emp.ListOfSticks.Count && (i - 1 >= 0))
						{
							Stick NextStick = emp.ListOfSticks[i + 1];
							Stick PrevStick = emp.ListOfSticks[i - 1];
							
							if (NextStick.TimeStart != stick.TimeEnd && PrevStick.TimeEnd != stick.TimeStart)
							{
								stick.Unique = true;
							}

							if(NextStick.State != StickState.ENABLED  && PrevStick.State != StickState.ENABLED)
							{
								stick.Unique = true;
							}
						}

						if (i == 0)
						{
							if (i + 1 != emp.ListOfSticks.Count)
							{
								Stick NextStick = emp.ListOfSticks[i + 1];
								if (NextStick.TimeStart != stick.TimeEnd && NextStick.State == StickState.ENABLED)
								{
									stick.Unique = true;
								}
							}
						}
					}
				}
			}
        #endregion
    }

    public void OrderSteps()
    {
        for (int i = 0; i < base.Steps.Count; i++)
        {
            // Yay Order works!
            for (int k = 0; k < base.Steps[i].ListOfSticks.Count; k++)
            {
                if (base.Steps[i].ListOfSticks[k].Parent.Pirority != 0)
                {
                    for (int l = 0; l < base.Steps[i].ListOfSticks.Count; l++)
                    {
                        if (base.Steps[i].ListOfSticks[k].Parent.Pirority < base.Steps[i].ListOfSticks[l].Parent.Pirority)
                        {
                            Stick tempHold = base.Steps[i].ListOfSticks[k];
                            base.Steps[i].ListOfSticks[k] = base.Steps[i].ListOfSticks[l];
                            base.Steps[i].ListOfSticks[l] = tempHold;
                        }
                    }
                }
            }
        }
    }

    public void AssignPass(bool UniqueStatus, bool Easy)
    {
        for (int i = 0; i < base.Steps.Count; i++)
        {
            for (int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
            {
                //Debug.Log(base.Steps[i].ListOfSticks[j].Parent.NameOfEmplacement + " - " +base.Steps[i].StartStepTime.ToString() + " - " + base.Steps[i].EndStepTime.ToString());
                Stick tempData = base.Steps[i].ListOfSticks[j];
				if(tempData.State == StickState.ENABLED)
				{
	                if (UniqueStatus != true && Easy == true)
	                {
	                    if (tempData.Assigned == false && tempData.Unique != true && tempData.Parent.Easy == true)
	                    {
	                        //Debug.Log("Assign 2");
	                        Assign(tempData, 2);
	                    }
	                }
	                else if (UniqueStatus == true && Easy == true)
	                {
	                    if (tempData.Assigned == false && tempData.Unique == true && tempData.Parent.Easy == true)
	                    {
	                        //Debug.Log("Assign 1");
	                        Assign(tempData, 1);
	                    }
	                }
	                else if (UniqueStatus == false && Easy != true)
	                {
	                    if (tempData.Assigned == false && tempData.Unique != true)
	                    {
	                        //Debug.Log("Assign 2");
	                        Assign(tempData, 2);
	                    }
	                }
				}
            }
        }
    }

    public void DebugEmplacements()
    {
        foreach (Emplacement emp in base.Emplacements)
        {
            emp.SetAllAssigned();
            if (emp.GetAllAssigned() == false && !emp.IsSpecialRole())
            {
                Debug.Log(emp.NameOfEmplacement + " has " + emp.NumberOfSticksUnAssigned() + " sticks");
				foreach(Stick temp in emp.ListOfSticks)
				{
					if(temp.Assigned == false)
					{
						Debug.Log("Time: " + temp.TimeStart.ToString() + " - " + temp.TimeEnd.ToString());
					}
				}
            }
        }
    }

    public void DebugPersonel()
    {
        foreach (Batch batch in base.Batches)
        {
            foreach (Person personal in batch.ListOfPeople)
            {
                if (personal.NoOfSticks > 0)
                {
                    Debug.Log(personal.name + " has " + personal.NoOfSticks + " left");
                }
            }
        }
    }

    public bool PossibilityCheck()
    {
        List<StepClass> ListOfStepsWithUnassignedSticks = new List<StepClass>();
        List<int> NumberOfSticksUnassigned = new List<int>();
        List<Person> UnassignedPeople = new List<Person>();
        GenerateUnassignedStepsAndSticks(ref ListOfStepsWithUnassignedSticks, ref NumberOfSticksUnassigned);
        GenerateUnassignedPersonels(ref UnassignedPeople);

        //Debug.Log(UnassignedPeople.Count);

        // Here, I need to do an important check, 
        // To check if there is someone to be able to do at that timeslot based off the number of sticks left avaliable on that time.
        // The amount of sticks left unassigned in a step is in the list of int
        // And the step is in the list.
        // So, I just need to cycle through them.
        if (ListOfStepsWithUnassignedSticks.Count == NumberOfSticksUnassigned.Count)
        {
            List<int> ListOfPeopleWhoCanDoThatStep = new List<int>();
            for (int i = 0; i < ListOfStepsWithUnassignedSticks.Count; i++)
            {
                int Counter = 0;
                foreach (Person personal in UnassignedPeople)
                {
                    if (personal.IsRested(ListOfStepsWithUnassignedSticks[i].StartStepTime, ListOfStepsWithUnassignedSticks[i].EndStepTime))
                    {
                        //Debug.Log(personal.Name);
                        Counter++;
                    }
                }
                //Debug.Log(ListOfStepsWithUnassignedSticks[i].StartStepTime + " has " + NumberOfSticksUnassigned[i] + " sticks and " + Counter + " people who can do it.");
                if (Counter >= NumberOfSticksUnassigned[i])
                {
                    //Debug.Log(ListOfStepsWithUnassignedSticks[i].StartStepTime + " has no problem");
                }
                else
                {
                    //Debug.Log(ListOfStepsWithUnassignedSticks[i].StartStepTime + " has problem " + NumberOfSticksUnassigned[i] + " - " + Counter);
                    return false;
                }
            }
            return true;
        }
        else
        {
            Debug.Log("Smth wrong!!");
            return false;
        }
    }

    public void GenerateUnassignedStepsAndSticks(ref List<StepClass> ListOfStepsWithUnassignedSticks, ref List<int> NumberOfSticksUnassigned)
    {
        for (int i = 0; i < base.Steps.Count; i++)
        {
            int counter = 0;
            for (int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
            {
                Stick tempData = base.Steps[i].ListOfSticks[j];
                if (tempData.Assigned == false && tempData.Unique == true)
                {
                    counter++;
                }
            }
            if (counter > 0)
            {
                ListOfStepsWithUnassignedSticks.Add(base.Steps[i]);
                NumberOfSticksUnassigned.Add(counter);
            }
        }
    }

	public void GenerateUnassignedSticks(ref List<Stick> ListOfUnassignedSticks)
	{
        for (int i = 0; i < base.Steps.Count; i++)
        {
            for (int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
            {
                Stick tempData = base.Steps[i].ListOfSticks[j];
                if (tempData.Assigned == false && tempData.Unique == true)
                {
					ListOfUnassignedSticks.Add(tempData);
                }
            }
        }
	}

    public void GenerateUnassignedPersonels(ref List<Person> UnassignedPeople)
    {
        // Now, I will find out which personal has not been assigned.
        foreach (Batch batch in base.Batches)
        {
            foreach (Person personal in batch.ListOfPeople)
            {
                if (personal.NoOfSticks > 0)
                {
                    UnassignedPeople.Add(personal);
                }
            }
        }
    }

    void Assign(Stick stickData, int NoToAssign)
    {
        //Debug.Log(stickData.Parent.NameOfEmplacement);
        // So now I have created the list. I need to find the right guy for it.
        // The first check finds the people who are avaliable to do the stick selected.
        List<Batch> FinalizedBatchList = new List<Batch>();
        foreach (Batch batch in base.Batches)
        {
            if (stickData.Parent.Easy == true)
            {
                if (batch.DoEasy == true)
                {
                    Batch TempBatch = gameObject.AddComponent<Batch>();
                    TempBatch.BatchName = batch.BatchName;
                    TempBatch.BatchNo = batch.BatchNo;
                    TempBatch.ClassData = batch.ClassData;
                    TempBatch.ListOfPeople = new List<Person>();
                    foreach (Person personal in batch.ListOfPeople)
                    {
                        //Debug.Log(personal.Name + " - " + stickData.TimeStart + " - " + personal.lastStickEndTiming + " - " + (stickData.TimeStart - personal.lastStickEndTiming).Hours + " - " + personal.IsRested(stickData.TimeStart, stickData.TimeEnd).ToString() + " - " + personal.ListOfRoles.Contains(stickData.Parent.CurrentRole).ToString() + " - " + (personal.NoOfSticks - NoToAssign >= 0).ToString());
                        if (personal.IsRested(stickData.TimeStart, stickData.TimeEnd) &&
                            personal.ListOfRoles.Contains(stickData.Parent.CurrentRole) &&
                            personal.NoOfSticks - NoToAssign >= 0)
                        {
                            TempBatch.AddPersonal(personal);
                        }
                    }
                    if (TempBatch.ListOfPeople.Count > 0)
                    {
                        FinalizedBatchList.Add(TempBatch);
                    }
                    Destroy(TempBatch);
                }
            }
            else
            {
                Batch TempBatch = gameObject.AddComponent<Batch>();
                TempBatch.BatchName = batch.BatchName;
                TempBatch.BatchNo = batch.BatchNo;
                TempBatch.ClassData = batch.ClassData;
                TempBatch.ListOfPeople = new List<Person>();
                foreach (Person personal in batch.ListOfPeople)
                {
                    //Debug.Log (personal.Name + " - " + stickData.TimeStart + " - " + personal.lastStickEndTiming + " - " + (stickData.TimeStart - personal.lastStickEndTiming).Hours + " - " + personal.IsRested (stickData.TimeStart,stickData.TimeEnd).ToString() + " - " + personal.ListOfRoles.Contains (stickData.Parent.CurrentRole).ToString() + " - " + (personal.NoOfSticks - NoToAssign >= 0).ToString());
                    if (personal.IsRested(stickData.TimeStart, stickData.TimeEnd) &&
                        personal.ListOfRoles.Contains(stickData.Parent.CurrentRole) &&
                        personal.NoOfSticks - NoToAssign >= 0
                        )
                    {
                        TempBatch.AddPersonal(personal);
                    }
                }
                if (TempBatch.ListOfPeople.Count > 0)
                {
                    FinalizedBatchList.Add(TempBatch);
                }
                Destroy(TempBatch);
            }
        }

        // This simple sort handles the ordering of people based off the number of sticks. 
        foreach (Batch TempBatch in FinalizedBatchList)
        {
            for (int i = 0; i < TempBatch.ListOfPeople.Count; i++)
            {
                for (int j = 0; j < TempBatch.ListOfPeople.Count; j++)
                {
                    if (TempBatch.ListOfPeople[i].NoOfSticks > TempBatch.ListOfPeople[j].NoOfSticks)
                    {
                        Person tempPerson = TempBatch.ListOfPeople[i];
                        TempBatch.ListOfPeople[i] = TempBatch.ListOfPeople[j];
                        TempBatch.ListOfPeople[j] = tempPerson;
                    }
                }
            }
        }

        //		// Debug here to check if the sort works
        //		foreach (Batch TempBatch in FinalizedBatchList) 
        //		{
        //			foreach (Person Personal in TempBatch.ListOfPeople) 
        //			{
        //				Debug.Log (TempBatch.BatchName + " - " + Personal.Name + " - " + Personal.NoOfSticks);
        //			}
        //		}

        // Now I need to assign the stick a person.
        // For example, if UVSS is the best stick, we need to assign him first.
        // I need to order the batch first.
        for (int i = 0; i < FinalizedBatchList.Count; i++)
        {
            for (int j = 0; j < FinalizedBatchList.Count; j++)
            {
                if (FinalizedBatchList[i].BatchNo > FinalizedBatchList[j].BatchNo)
                {
                    Batch tBatch = FinalizedBatchList[i];
                    FinalizedBatchList[i] = FinalizedBatchList[j];
                    FinalizedBatchList[j] = tBatch;
                }
            }
        }

        //		if (stickData.Parent.Easy == true) 
        //		{
        //			FinalizedBatchList.Reverse ();
        //		}

        // Now we need to select the guy who the largest pirority.
        List<Person> FinalizedListOfPersonal = new List<Person>();
        foreach (Batch temp in FinalizedBatchList)
        {
            if (temp.ListOfPeople.Count > 0)
            {
                //				Person HighestAmtOfStickInBatch = null;
                //				// Find the largest guy in each batch
                //				List<Person> BatchListOfPersonal = new List<Person> ();
                //				foreach (Person personal in temp.ListOfPeople) 
                //				{
                //					if (HighestAmtOfStickInBatch == null) 
                //					{
                //						HighestAmtOfStickInBatch = personal;
                //						BatchListOfPersonal.Add (HighestAmtOfStickInBatch);
                //					}
                //					else if (HighestAmtOfStickInBatch.NoOfSticks == personal.NoOfSticks)
                //					{
                //						BatchListOfPersonal.Add (personal);
                //					}
                //					else if (personal.NoOfSticks > HighestAmtOfStickInBatch.NoOfSticks) 
                //					{
                //						HighestAmtOfStickInBatch = personal;
                //						if (BatchListOfPersonal.Count > 0) 
                //						{
                //							BatchListOfPersonal = new List<Person> ();
                //							BatchListOfPersonal.Add (HighestAmtOfStickInBatch);
                //						}
                //					}
                //				}
                //				Person tPerson = BatchListOfPersonal [Random.Range (0, BatchListOfPersonal.Count)];
                //				Debug.Log ("Highest In Batch: " + tPerson.Name + " - " + tPerson.Parent.BatchName + " - " + tPerson.NoOfSticks);
                //				FinalizedListOfPersonal.Add (tPerson);

                foreach (Person personal in temp.ListOfPeople)
                {
                    FinalizedListOfPersonal.Add(personal);
                }
            }
        }

        IWeightedRandomizer<string> randomizer = new DynamicWeightedRandomizer<string>();
        foreach (Batch temp in FinalizedBatchList)
        {
            if (temp.ListOfPeople.Count > 0)
            {
                foreach (Person personal in temp.ListOfPeople)
                {
                    //Debug.Log (personal.Name + " Hours since last stick: " + personal.GetHoursSinceLastStick(stickData.TimeStart));
                    randomizer[personal.Name] = (personal.GetHoursSinceLastStick(stickData.TimeStart) * (personal.NoOfSticks + 1) * (personal.OriginNoOfSticks));
                }
                break;
            }
        }

        //		foreach (string val in randomizer) 
        //		{
        //			Debug.Log ("Weight: " + val + " - " + randomizer.GetWeight (val));;
        //		}


        if (randomizer.Count > 0)
        {
            if (NoToAssign == 1)
            {
                //stickData.AssignPerson (temp.ListOfPeople [Random.Range (0, temp.ListOfPeople.Count - 1)]);
                string Name = randomizer.NextWithReplacement();

                Person personalToAssign = (FinalizedListOfPersonal.Find(x => x.Name == Name));
                //Debug.Log("Assigning " + Name + " with weight " + randomizer.GetWeight (Name));
                stickData.AssignPerson(personalToAssign);
            }
            else
            {
                //Person personalToAssign = temp.ListOfPeople [Random.Range (0, temp.ListOfPeople.Count - 1)];
                string Name = randomizer.NextWithReplacement();
                //Debug.Log("Assigning " + Name + " with weight " + randomizer.GetWeight (Name));
                Person personalToAssign = (FinalizedListOfPersonal.Find(x => x.Name == Name));
                stickData.AssignPerson(personalToAssign);
				stickData.Neighbour = stickData.Parent.ListOfSticks[stickData.Parent.ListOfSticks.FindIndex(x => x.TimeStart == stickData.TimeStart) + 1];
                // I need to get the next stick here. How do I get the next stick....
                //Debug.Log("Hello world");
                //Debug.Log("Data: " + stickData.Parent.ListOfSticks.FindIndex(x => x.TimeStart == stickData.TimeStart) + " - " + stickData.Parent.ListOfSticks.Count);
                //Debug.Log (stickData.Parent.ListOfSticks [stickData.Parent.ListOfSticks.FindIndex (x => x.TimeStart == stickData.TimeStart)].Parent.NameOfEmplacement);
                //Debug.Log (stickData.Parent.ListOfSticks [stickData.Parent.ListOfSticks.FindIndex (x => x.TimeStart == stickData.TimeStart) + 1].Parent.NameOfEmplacement);
                stickData.Parent.ListOfSticks[stickData.Parent.ListOfSticks.FindIndex(x => x.TimeStart == stickData.TimeStart) + 1].AssignPerson(personalToAssign);
            }
        }
    }
}
