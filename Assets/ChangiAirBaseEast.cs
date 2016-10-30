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
		BatchStick = new List<int[]> ();
		BatchStick.Add (new int[] { tBatchAmount, tNoOfPeronals });
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
	public List<Stick> ListOfSticks = new List<Stick> ();
	public DateTime StartStepTime;
	public DateTime EndStepTime;
}

public class ChangiAirBaseEast : Base {

	public DateTime StartDate;
	public DateTime EndDate;

	public GameObject BarObject;

	public GameObject ParentObject;
	public GameObject StickGameObject;

	public GameObject SearchInputField;

	private JSONNode root = null; 

	public List<BatchClass> FinalizedList = new List<BatchClass>();

	public string dataFile = "";

	// Use this for initialization
	void Start () 
    {
        Debug.Log("No Of Hours: " + (StaticVars.EndDate - StaticVars.StartDate).TotalHours);
        GameObject BatchParents = new GameObject("Batches");
        GameObject EmplacementParents = new GameObject("Emplacements");
        
		TextAsset Json = Resources.Load(dataFile) as TextAsset;
        root = JSON.Parse(Json.text);
        
        for (int j = 0; j < root["Batches"].Count; j++)
        {
            GameObject BatchObj = new GameObject();
            BatchObj.transform.parent = BatchParents.transform;
            Batch tempBatch = BatchObj.AddComponent<Batch>();
            tempBatch.BatchName = root["Batches"][j]["BatchName"];
			tempBatch.BatchNo = j + 1;
			tempBatch.DoEasy = root["Batches"][j]["DoEasy"].AsBool;
            BatchObj.name = root["Batches"][j]["BatchName"];
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
        }

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

        for (int i = 0; i < root["Emplacements"].Count; i++)
        {
            //Debug.Log(root["Emplacements"][i]["Name"].Value + " - " + root["Emplacements"][i]["Role"].Value);
            GameObject EmpObj = new GameObject();
            EmpObj.transform.parent = EmplacementParents.transform;
            Emplacement Emp1 = EmpObj.AddComponent<Emplacement>();
            Emp1.NameOfEmplacement = root["Emplacements"][i]["Name"];
			Emp1.Easy = root ["Emplacements"] [i] ["Easy"].AsBool;
            EmpObj.name = Emp1.NameOfEmplacement;
			Emp1.GenerateSticks(ParentObject, StickGameObject, StaticVars.RolesParseJson(root["Emplacements"][i]["Role"]),root["Emplacements"][i]["Pirority"].AsInt,i);
			base.Emplacements.Add (Emp1);
        }

		ParentObject.transform.localScale = new Vector3 (0.7f, 0.7f, 0f);
		ParentObject.transform.localPosition = new Vector3 (-115f,0f,0f);

		// Viper 1 Sentry
//		base.Emplacements [0].RemoveStick (new DateTime (2016, 8, 15, 18, 00, 00),new DateTime (2016, 8, 16, 6, 00, 00));
//		base.Emplacements [0].RemoveStick (new DateTime (2016, 8, 16, 9, 00, 00),new DateTime (2016, 8, 16, 15, 00, 00));
//		base.Emplacements [0].RemoveStick (new DateTime (2016, 8, 16, 18, 00, 00),new DateTime (2016, 8, 17, 6, 00, 00));
//		base.Emplacements [0].RemoveStick (new DateTime (2016, 8, 17, 9, 00, 00),new DateTime (2016, 8, 17, 15, 00, 00));
		base.Emplacements [0].RemoveStick (new DateTime (2016, 8, 15, 14, 00, 00),new DateTime (2016, 8, 17, 14, 00, 00));

		// Viper 1 Checker
//		base.Emplacements [1].RemoveStick (new DateTime (2016, 8, 15, 18, 00, 00),new DateTime (2016, 8, 16, 6, 00, 00));
//		base.Emplacements [1].RemoveStick (new DateTime (2016, 8, 16, 9, 00, 00),new DateTime (2016, 8, 16, 15, 00, 00));
//		base.Emplacements [1].RemoveStick (new DateTime (2016, 8, 16, 18, 00, 00),new DateTime (2016, 8, 17, 6, 00, 00));
//		base.Emplacements [1].RemoveStick (new DateTime (2016, 8, 17, 9, 00, 00),new DateTime (2016, 8, 17, 15, 00, 00));
		base.Emplacements [1].RemoveStick (new DateTime (2016, 8, 15, 14, 00, 00),new DateTime (2016, 8, 16, 6, 00, 00));
		base.Emplacements [1].RemoveStick (new DateTime (2016, 8, 16, 9, 00, 00),new DateTime (2016, 8, 16, 15, 00, 00));
		base.Emplacements [1].RemoveStick (new DateTime (2016, 8, 16, 18, 00, 00),new DateTime (2016, 8, 17, 14, 00, 00));

		// UVSS
//		base.Emplacements [6].RemoveStick (new DateTime (2016, 8, 15, 18, 00, 00),new DateTime (2016, 8, 16, 6, 00, 00));
//		base.Emplacements [6].RemoveStick (new DateTime (2016, 8, 16, 9, 00, 00),new DateTime (2016, 8, 16, 15, 00, 00));
//		base.Emplacements [6].RemoveStick (new DateTime (2016, 8, 16, 18, 00, 00),new DateTime (2016, 8, 17, 6, 00, 00));
//		base.Emplacements [6].RemoveStick (new DateTime (2016, 8, 17, 9, 00, 00),new DateTime (2016, 8, 17, 15, 00, 00));
		base.Emplacements [6].RemoveStick (new DateTime (2016, 8, 15, 14, 00, 00),new DateTime (2016, 8, 16, 6, 00, 00));
		base.Emplacements [6].RemoveStick (new DateTime (2016, 8, 16, 9, 00, 00),new DateTime (2016, 8, 16, 15, 00, 00));
		base.Emplacements [6].RemoveStick (new DateTime (2016, 8, 16, 18, 00, 00),new DateTime (2016, 8, 17, 14, 00, 00));

		// Python Sentry
		base.Emplacements [2].RemoveStick (new DateTime (2016, 8, 15, 18, 00, 00),new DateTime (2016, 8, 16, 6, 00, 00));
		base.Emplacements [2].RemoveStick (new DateTime (2016, 8, 16, 18, 00, 00),new DateTime (2016, 8, 17, 6, 00, 00));
		base.Emplacements [2].RemoveStick (new DateTime (2016, 8, 16, 9, 00, 00),new DateTime (2016, 8, 16, 15, 00, 00));
		base.Emplacements [2].RemoveStick (new DateTime (2016, 8, 17, 9, 00, 00),new DateTime (2016, 8, 17, 14, 00, 00));

		base.Emplacements [3].RemoveStick (new DateTime (2016, 8, 15, 18, 00, 00),new DateTime (2016, 8, 16, 6, 00, 00));
		base.Emplacements [3].RemoveStick (new DateTime (2016, 8, 16, 18, 00, 00),new DateTime (2016, 8, 17, 6, 00, 00));

		base.Emplacements [4].RemoveStick (new DateTime (2016, 8, 15, 18, 00, 00),new DateTime (2016, 8, 17, 12, 00, 00));

		GenerateDictionary ();

		#region ICT Handling
		//int TotalNumberOfSticksCoveredByICT = root["ICT"]["NumberOfICT"].AsInt * root["ICT"]["NumberOfStickforICTWeekDay"].AsInt;
		#endregion

		// Parameters for the function
		//int NoOfSticks = 60 - TotalNumberOfSticksCoveredByICT;
		int NoOfSticks = 34;
		bool MaxXZ = true;

		// First, We need to pull out the personal who are not standing sticks.
		// Aka, driver, console. This will be soft coded in the future so that they can choose who to pull.

		// To do this, we create a new json node, so we dont override it by accident.
		JSONNode subNode = JSON.Parse(root.ToString());

		Debug.Log ("Subnode: " + subNode.ToString());
		// Now, we start by removing the drivers and console personal.
		for (int j = 0; j < subNode ["Batches"].Count; j++)
		{
			List<JSONNode> indexToRemove = new List<JSONNode> ();
			for (int i = 0; i < subNode ["Batches"] [j] ["Personnels"].Count; i++) 
			{
				//Debug.Log(subNode ["Batches"] [j] ["Personnels"] [i] ["Name"]+ " - " + subNode["Batches"][j]["Personnels"][i]["Roles"][0].Value);
				if (subNode ["Batches"] [j] ["Personnels"] [i] ["Roles"][0].Value == "Driver") 
				{
					Debug.Log ("Driver detected! " + subNode ["Batches"] [j] ["Personnels"] [i]["Name"]);
					indexToRemove.Add (subNode ["Batches"] [j] ["Personnels"] [i]);
				}
				else if (subNode ["Batches"] [j] ["Personnels"] [i] ["Roles"][0].Value == "Console") 
				{
					Debug.Log ("Console detected! " + subNode ["Batches"] [j] ["Personnels"] [i]["Name"]);
					indexToRemove.Add (subNode ["Batches"] [j] ["Personnels"] [i]);
				}
				else if (subNode ["Batches"] [j] ["Personnels"] [i] ["Roles"][0].Value == "Giro") 
				{
					Debug.Log ("Giro detected! " + subNode ["Batches"] [j] ["Personnels"] [i]["Name"]);
					indexToRemove.Add (subNode ["Batches"] [j] ["Personnels"] [i]);
				}
				else if (subNode ["Batches"] [j] ["Personnels"] [i] ["Roles"][0].Value == "Armorer") 
				{
					Debug.Log ("Armorer detected! " + subNode ["Batches"] [j] ["Personnels"] [i]["Name"]);
					indexToRemove.Add (subNode ["Batches"] [j] ["Personnels"] [i]);
				}
			}

			for (int k = 0; k < indexToRemove.Count; k++) 
			{
				Debug.Log ("Removing " + indexToRemove [k].ToString());
				subNode ["Batches"] [j] ["Personnels"].Remove (indexToRemove [k]);
			}

			if (subNode ["Batches"] [j] ["Personnels"].Count == 0) 
			{
				Debug.Log ("Removing batch!");
				//subNode["Batches"].Remove (subNode ["Batches"] [j]);
			}
		}

		Debug.Log ("Subnode: " + subNode.ToString());
		Debug.Log ("Root: " + root.ToString());

		// Now that I have the subnode, which have the latest information.
		// I know which personal I can use. I can now assign the dictionary of lists.
		// The idea is to have an array of ints, for example
		// In my camp, I have 4 batches that I can assign stick. But it might not be 4. 
		// So, based off the json, I can then think of how many batches I can use.
		int CurrentNoOfBatches = subNode ["Batches"].Count;

		// Currently, I am going to hard code the variable of number of stick for xz.
		int XZBaseStick = StaticVars.MaxNoPerMountWeekDay;

		// Define the stick count list, not the finalize number
		List<BatchClass> StickCount = new List<BatchClass> ();

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
				StickCount.Add (new BatchClass (XZBaseStick, subNode ["Batches"] [i] ["Personnels"].Count, subNode ["Batches"] [i] ["BatchName"], true));
			} 
			else 
			{
				//Debug.Log (XZBaseStick - i + " - "+ subNode ["Batches"] [i] ["Personnels"].Count);
				StickCount.Add (new BatchClass (XZBaseStick - i, subNode ["Batches"] [i] ["Personnels"].Count, subNode ["Batches"] [i] ["BatchName"]));
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
		foreach(BatchClass temp in StickCount)
		{
			int val = temp.BatchStick[0][0] * temp.BatchStick [0][1];
			Debug.Log (temp.BatchName + " - " + val + " - " + temp.NoOfPersonals);
			TotalSticks += val;
		}
		Debug.Log ("Here:" + TotalSticks);

		// Now, I need to calculate the diff in sticks. Which is the most important value.
		// This value determines if we increase or decrease the value. 
		int diff = NoOfSticks - TotalSticks;
		Debug.Log (NoOfSticks + " - " + TotalSticks);
		List<List<int[]>> CurrentConfiguration = new List<List<int[]>> ();
		foreach(BatchClass temp in StickCount)
		{
			Debug.Log (temp.BatchStick [0] [0] * temp.BatchStick [0] [1]);
			CurrentConfiguration.Add (temp.BatchStick);
		}

		bool loop = true;
		int breakpoint = 0;

		Debug.Log ("Diff: " + diff);
		// Here, I do my condition checks. 
		if (diff == 0) 
		{
			// Yay! We dont need to calculate anymore
			Debug.Log ("Finalized");
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
					Debug.Log ("Diff == BatchNo!");
					// We can increase that batch total amount of stick, 
					if (diff > 0) 
					{
						temp.BatchStick [0] [1]++;
						loop = false;
						break;
					}
					else if(diff < 0)
					{
						temp.BatchStick [0] [1]--;
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
				for(int i = StickCount.Count - 1; i > 0; i--)
				{
					BatchClass temp = StickCount [i];
					if (temp.XZ != MaxXZ) 
					{
						// This is for max xz
						Debug.Log(sticksLeft + " - " + temp.BatchName + " - " + temp.NoOfPersonals + " - " + diff);
						if (sticksLeft > temp.NoOfPersonals && temp.NoOfPersonals > 0) 
						{
							// Now, I will minus the amount of sticks left.
							sticksLeft -= temp.NoOfPersonals;
							if (diff < 0) 
							{
								Debug.Log ("Minus");
								// Now, I reconfigure the sticks for when we need less manpower.
								temp.BatchStick [0] [0]--;
							}
							else if (diff > 0) 
							{
								Debug.Log ("Plus");
								// Now, I reconfigure the sticks for when we need more manpower.
								temp.BatchStick [0] [0]++;
							}
						}
						else if(temp.NoOfPersonals > 0)
						{
							Debug.Log ("No of sticks left = " + sticksLeft);
							// This means we do not have enough manpower. 
							// This means we need to uneven stick.
							int PreviousAmt = temp.BatchStick[0][0];
							temp.BatchStick = new List<int[]>();

							if (diff < 0) 
							{
								int FirstAmt = temp.NoOfPersonals - sticksLeft;
								int SecAmt = temp.NoOfPersonals - FirstAmt;
								Debug.Log ("First Second " + FirstAmt + " - " + SecAmt + " - " + PreviousAmt);
								temp.BatchStick.Add (new int[] { PreviousAmt - 1, SecAmt });
								temp.BatchStick.Add (new int[] { PreviousAmt, FirstAmt });
							} 
							else 
							{
								int FirstAmt = temp.NoOfPersonals - sticksLeft;
								int SecAmt = temp.NoOfPersonals - FirstAmt;
								Debug.Log ("First Second " + FirstAmt + " - " + SecAmt + " - " + PreviousAmt);
								temp.BatchStick.Add (new int[] { PreviousAmt + 1, SecAmt });
								temp.BatchStick.Add (new int[] { PreviousAmt, FirstAmt });
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
						Debug.Log ("(" + countStick.BatchName + ")" + val[1] + " People do " + val[0] * val[1] + " - " + val[0] + " each");
						if (val[1] == 0 && val[0] == 0) 
						{
							StickCount.Remove (countStick);
							break;
						}
					}
				}
				Debug.Log (breakpoint + " Times - Count: " + count);
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

			foreach(Batch batchData in Batches)
			{
				if(batchData.BatchName == temp.BatchName)
				{
					temp.BatchPersonalData = batchData;
					batchData.ClassData = temp;
					//Debug.Log(temp.BatchStick.Count + " - " + temp.BatchName);
					if(temp.BatchStick.Count > 1)
					{
						for (int NoOfIndex = 0; NoOfIndex < temp.BatchStick.Count; NoOfIndex++) 
						{
							int NoOfLoops = 0;
							int LoopNo = temp.BatchStick [NoOfIndex] [1];
							while (NoOfLoops != LoopNo) 
							{
								int RandomIndex = Random.Range(0, batchData.ListOfPeople.Count);
								if (!batchData.ListOfPeople [RandomIndex].IsSpecialRole () && batchData.ListOfPeople [RandomIndex].NoOfSticks == 0) 
								{
									if(temp.BatchStick[NoOfIndex][1] > 0)
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
						foreach(Person personal in batchData.ListOfPeople)
						{
							if(!personal.IsSpecialRole())
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
		Debug.Log ("Count: " + finalCount);

		// So now, I have calculated the amount of sticks per batch. 
		// No matter if they are even or uneven. Yay!
		// Now, I need to assign the sticks accordingly. 
		// I will now check the emplacement data first.
//		for (int k = 0; k < subNode ["Emplacements"].Count; k++) 
//		{
//			Debug.Log(root["Emplacements"][k]["Name"].Value + " - " + root["Emplacements"][k]["Role"].Value);
//		}

		base.Steps = new List<StepClass>();
		int TotalAmtOfStick = (int)(StaticVars.EndDate - StaticVars.StartDate).TotalHours / StaticVars.StickInHours;
		for(int i = 0; i < TotalAmtOfStick; i++)
		{
			StepClass tempStep = new StepClass();
			foreach(Emplacement emp in base.Emplacements)
			{
				if(!emp.IsSpecialRole())
				{
					foreach(Stick stick in emp.ListOfSticks)
					{
						if(stick.Assigned == false && stick.StepIndex == i)
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

		foreach(Emplacement emp in base.Emplacements)
		{
			//foreach(Stick stick in emp.ListOfSticks)
			for(int i = 0; i < emp.ListOfSticks.Count; i++)
			{
				Stick stick = emp.ListOfSticks [i];
				if((stick.TimeEnd - stick.TimeStart).Hours % 3 != 0)
				{
					stick.Unique = true;
				}

				if (i + 1 == emp.ListOfSticks.Count) 
				{
					stick.Unique = true;
				}

				if (i + 1 != emp.ListOfSticks.Count && (i - 1 >= 0)) 
				{
					Stick NextStick = emp.ListOfSticks [i + 1];
					Stick PrevStick = emp.ListOfSticks [i - 1];
					if (NextStick.TimeStart != stick.TimeEnd && PrevStick.TimeEnd != stick.TimeStart) 
					{
						stick.Unique = true;
					}
				}

				if (i == 0) 
				{
					if (i + 1 != emp.ListOfSticks.Count) 
					{
						Stick NextStick = emp.ListOfSticks [i + 1];
						if (NextStick.TimeStart != stick.TimeEnd) 
						{
							stick.Unique = true;
						}
					}
				}
			}
		}
		Reset ();
	}

	string[] GenerateDictionary()
	{
		List<string> OutputDictionary = new List<string>();
		for (int i = 0; i < Batches.Count; i++) 
		{
			for (int j = 0; j < Batches [i].ListOfPeople.Count; j++) 
			{
				//Debug.Log ("Batch: " + Batches [i].BatchName + " - " + Batches [i].ListOfPeople [j].Name);
				OutputDictionary.Add (Batches [i].ListOfPeople [j].Name);
			}
		}
		return OutputDictionary.ToArray ();
	}

	public void Reset ()
	{
		foreach (Emplacement emp in base.Emplacements) 
		{
			emp.Reset ();
		}

		foreach (Batch batch in base.Batches) 
		{
			batch.Reset ();
		}
	}

	public void AssignSticks ()
	{
		for(int i = 0; i < base.Steps.Count; i++)
		//for(int i = 0; i < 5; i++)
		{
			// Yay Order works!
			for(int k = 0; k < base.Steps[i].ListOfSticks.Count; k++)
			{
				if(base.Steps[i].ListOfSticks[k].Parent.Pirority != 0)
				{
					for(int l = 0; l < base.Steps[i].ListOfSticks.Count; l++)
					{
						if(base.Steps[i].ListOfSticks[k].Parent.Pirority < base.Steps[i].ListOfSticks[l].Parent.Pirority)
						{
							Stick tempHold = base.Steps[i].ListOfSticks[k];
							base.Steps[i].ListOfSticks[k] = base.Steps[i].ListOfSticks[l];
							base.Steps[i].ListOfSticks[l] = tempHold;
						}
					}
				}
			}

//			for(int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
//			{
//				Debug.Log(base.Steps[i].ListOfSticks[j].Parent.NameOfEmplacement + " - " +base.Steps[i].StartStepTime.ToString() + " - " + base.Steps[i].EndStepTime.ToString());
//				Stick tempData = base.Steps[i].ListOfSticks[j];
//				if(tempData.Assigned == false)
//				{
//					if(tempData.Unique == true)
//					{
//						Debug.Log("Assign 1");
//						Assign(tempData,1);
//					}
//					else
//					{
//						Debug.Log("Assign 2");
//						Assign(tempData,2);
//					}
//				}
//			}

			for(int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
			{
				Debug.Log(base.Steps[i].ListOfSticks[j].Parent.NameOfEmplacement + " - " +base.Steps[i].StartStepTime.ToString() + " - " + base.Steps[i].EndStepTime.ToString());
				Stick tempData = base.Steps[i].ListOfSticks[j];
				if(tempData.Assigned == false && tempData.Unique != true && tempData.Parent.Easy == true)
				{
					Debug.Log("Assign 2");
					Assign(tempData,2);
				}
			}
		}

		for (int i = 0; i < base.Steps.Count; i++)
		{
			for(int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
			{
				Debug.Log(base.Steps[i].ListOfSticks[j].Parent.NameOfEmplacement + " - " +base.Steps[i].StartStepTime.ToString() + " - " + base.Steps[i].EndStepTime.ToString());
				Stick tempData = base.Steps[i].ListOfSticks[j];
				if(tempData.Assigned == false && tempData.Unique == true && tempData.Parent.Easy == true)
				{
					Debug.Log("Assign 1");
					Assign(tempData,1);
				}
			}
		}

		for (int i = 0; i < base.Steps.Count; i++)
		{
			for(int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
			{
				Debug.Log(base.Steps[i].ListOfSticks[j].Parent.NameOfEmplacement + " - " +base.Steps[i].StartStepTime.ToString() + " - " + base.Steps[i].EndStepTime.ToString());
				Stick tempData = base.Steps[i].ListOfSticks[j];
				if(tempData.Assigned == false && tempData.Unique != true)
				{
					Debug.Log("Assign 2");
					Assign(tempData,2);
				}
			}
		}

//		for (int i = 0; i < base.Steps.Count; i++)
//		{
//			for(int j = 0; j < base.Steps[i].ListOfSticks.Count; j++)
//			{
//				Debug.Log(base.Steps[i].ListOfSticks[j].Parent.NameOfEmplacement + " - " +base.Steps[i].StartStepTime.ToString() + " - " + base.Steps[i].EndStepTime.ToString());
//				Stick tempData = base.Steps[i].ListOfSticks[j];
//				if(tempData.Assigned == false && tempData.Unique == true)
//				{
//					Debug.Log("Assign 1");
//					Assign(tempData,1);
//				}
//			}
//		}

		foreach (Emplacement emp in base.Emplacements) 
		{
			emp.SetAllAssigned ();
			if (emp.GetAllAssigned () == false && !emp.IsSpecialRole()) 
			{
				Debug.Log (emp.NameOfEmplacement + " has " + emp.NumberOfSticksUnAssigned () + " sticks");
			}
		}

		foreach (Batch batch in base.Batches) 
		{
			foreach (Person personal in batch.ListOfPeople) 
			{
				if (personal.NoOfSticks > 0) 
				{
					Debug.Log (personal.name + " has " + personal.NoOfSticks + " left");
				}
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			AssignSticks ();
		}

		if (Input.GetKeyDown (KeyCode.R)) 
		{
			Reset ();
		}

		if (Input.GetKeyDown (KeyCode.C)) 
		{
			IWeightedRandomizer<string> randomizer = new DynamicWeightedRandomizer<string>();
			randomizer ["Joe"] = 1;
			randomizer ["Ryan"] = 5;
			randomizer ["Jason"] = 3;

			//Debug.Log ("Before: " + randomizer.GetWeight("Joe") + " - " + randomizer.GetWeight("Ryan") + " - " + randomizer.GetWeight("Jason"));
			string name1 = randomizer.NextWithReplacement ();
			//Debug.Log ("After: " + randomizer.GetWeight("Joe") + " - " + randomizer.GetWeight("Ryan") + " - " + randomizer.GetWeight("Jason"));
			//Debug.Log (name1);

			foreach (string val in randomizer) 
			{
				Debug.Log (val + " - " + randomizer.GetWeight(val));
			}
		}
	}

	void Assign(Stick stickData, int NoToAssign)
	{
		Debug.Log (stickData.Parent.NameOfEmplacement);
		// So now I have created the list. I need to find the right guy for it.
		// The first check finds the people who are avaliable to do the stick selected.
		List<Batch> FinalizedBatchList = new List<Batch> ();
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
					TempBatch.ListOfPeople = new List<Person> ();
					foreach (Person personal in batch.ListOfPeople) 
					{
						Debug.Log (personal.Name + " - " + stickData.TimeStart + " - " + personal.lastStickEndTiming + " - " + (stickData.TimeStart - personal.lastStickEndTiming).Hours + " - " + personal.IsRested (stickData.TimeStart,stickData.TimeEnd).ToString() + " - " + personal.ListOfRoles.Contains (stickData.Parent.CurrentRole).ToString() + " - " + (personal.NoOfSticks - NoToAssign >= 0).ToString());
						if (personal.IsRested (stickData.TimeStart,stickData.TimeEnd) &&
							personal.ListOfRoles.Contains (stickData.Parent.CurrentRole) && 
							personal.NoOfSticks - NoToAssign >= 0) 
						{
							TempBatch.AddPersonal (personal);
						}
					}
					if (TempBatch.ListOfPeople.Count > 0) 
					{
						FinalizedBatchList.Add (TempBatch);
					}
					Destroy (TempBatch);
				}
			}
			else
			{
				Batch TempBatch = gameObject.AddComponent<Batch>();
				TempBatch.BatchName = batch.BatchName;
				TempBatch.BatchNo = batch.BatchNo;
				TempBatch.ClassData = batch.ClassData;
				TempBatch.ListOfPeople = new List<Person> ();
				foreach (Person personal in batch.ListOfPeople) 
				{
					Debug.Log (personal.Name + " - " + stickData.TimeStart + " - " + personal.lastStickEndTiming + " - " + (stickData.TimeStart - personal.lastStickEndTiming).Hours + " - " + personal.IsRested (stickData.TimeStart,stickData.TimeEnd).ToString() + " - " + personal.ListOfRoles.Contains (stickData.Parent.CurrentRole).ToString() + " - " + (personal.NoOfSticks - NoToAssign >= 0).ToString());
					if (personal.IsRested (stickData.TimeStart,stickData.TimeEnd) &&
						personal.ListOfRoles.Contains (stickData.Parent.CurrentRole) && 
						personal.NoOfSticks - NoToAssign >= 0
						) 
					{
						TempBatch.AddPersonal (personal);
					}
				}
				if (TempBatch.ListOfPeople.Count > 0) 
				{
					FinalizedBatchList.Add (TempBatch);
				}
				Destroy (TempBatch);
			}
		}		

		// This simple sort handles the ordering of people based off the number of sticks. 
		foreach (Batch TempBatch in FinalizedBatchList) 
		{
			for(int i = 0; i < TempBatch.ListOfPeople.Count; i++)
			{
				for(int j = 0; j < TempBatch.ListOfPeople.Count; j++)
				{
					if (TempBatch.ListOfPeople [i].NoOfSticks > TempBatch.ListOfPeople [j].NoOfSticks) 
					{
						Person tempPerson = TempBatch.ListOfPeople [i];
						TempBatch.ListOfPeople [i] = TempBatch.ListOfPeople [j];
						TempBatch.ListOfPeople [j] = tempPerson;
					}
				}		
			}
		}

		// Debug here to check if the sort works
		foreach (Batch TempBatch in FinalizedBatchList) 
		{
			foreach (Person Personal in TempBatch.ListOfPeople) 
			{
				Debug.Log (TempBatch.BatchName + " - " + Personal.Name + " - " + Personal.NoOfSticks);
			}
		}

		// Now I need to assign the stick a person.
		// For example, if UVSS is the best stick, we need to assign him first.
		// I need to order the batch first.
		for (int i = 0; i < FinalizedBatchList.Count; i++) 
		{
			for (int j = 0; j < FinalizedBatchList.Count; j++) 
			{
				if (FinalizedBatchList [i].BatchNo > FinalizedBatchList [j].BatchNo) 
				{
					Batch tBatch = FinalizedBatchList [i];
					FinalizedBatchList [i] = FinalizedBatchList [j];
					FinalizedBatchList [j] = tBatch;
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
					FinalizedListOfPersonal.Add (personal);
				}
			}
		}

		IWeightedRandomizer<string> randomizer = new DynamicWeightedRandomizer<string>();
		foreach (Batch temp in FinalizedBatchList) 
		{
			if(temp.ListOfPeople.Count > 0)
			{
				foreach (Person personal in temp.ListOfPeople) 
				{
					Debug.Log (personal.Name + " Hours since last stick: " + personal.GetHoursSinceLastStick(stickData.TimeStart));
					randomizer [personal.Name] = (personal.GetHoursSinceLastStick(stickData.TimeStart) * (personal.NoOfSticks + 1) * (personal.OriginNoOfSticks));
				}
				break;
			}
		}

		foreach (string val in randomizer) 
		{
			Debug.Log ("Weight: " + val + " - " + randomizer.GetWeight (val));;
		}


		if(randomizer.Count > 0)
		{
			if (NoToAssign == 1) 
			{
				//stickData.AssignPerson (temp.ListOfPeople [Random.Range (0, temp.ListOfPeople.Count - 1)]);
				string Name = randomizer.NextWithReplacement();

				Person personalToAssign = (FinalizedListOfPersonal.Find(x => x.Name == Name));
				Debug.Log("Assigning " + Name + " with weight " + randomizer.GetWeight (Name));
				stickData.AssignPerson (personalToAssign);
			}
			else
			{
				//Person personalToAssign = temp.ListOfPeople [Random.Range (0, temp.ListOfPeople.Count - 1)];
				string Name = randomizer.NextWithReplacement();
				Debug.Log("Assigning " + Name + " with weight " + randomizer.GetWeight (Name));
				Person personalToAssign = (FinalizedListOfPersonal.Find(x => x.Name == Name));
				stickData.AssignPerson (personalToAssign);
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
