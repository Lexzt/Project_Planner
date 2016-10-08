using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

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

	// Use this for initialization
	void Start () 
    {
        Debug.Log("No Of Hours: " + (StaticVars.EndDate - StaticVars.StartDate).TotalHours);
        
        GameObject BatchParents = new GameObject("Batches");
        GameObject EmplacementParents = new GameObject("Emplacements");
        
        TextAsset Json = Resources.Load("data") as TextAsset;
        root = JSON.Parse(Json.text);
        
        for (int j = 0; j < root["Batches"].Count; j++)
        {
            GameObject BatchObj = new GameObject();
            BatchObj.transform.parent = BatchParents.transform;
            Batch tempBatch = BatchObj.AddComponent<Batch>();
            tempBatch.BatchName = root["Batches"][j]["BatchName"];
            BatchObj.name = root["Batches"][j]["BatchName"];
            base.Batches.Add(tempBatch);

            for (int i = 0; i < root["Batches"][j]["Personnels"].Count; i++)
            {
				Debug.Log("Array: " + root["Batches"][j]["Personnels"][i]["Roles"]);
                GameObject Person = new GameObject();
                Person.transform.parent = BatchObj.transform;
                Person tempPerson = Person.AddComponent<Person>();
                tempPerson.Set(root["Batches"][j]["Personnels"][i]["Name"], 
					root["Batches"][j]["Personnels"][i]["IC"], 
					new DateTime(root["Batches"][j]["Personnels"][i]["DOB"].AsInt),
					root["Batches"][j]["Personnels"][i]["Roles"]);
                Person.name = tempPerson.Name;
                tempBatch.AddPersonal(tempPerson);
            }
        }

        for (int i = 0; i < root["Emplacements"].Count; i++)
        {
            Debug.Log(root["Emplacements"][i]["Name"].Value + " - " + root["Emplacements"][i]["Role"].Value);
            GameObject EmpObj = new GameObject();
            EmpObj.transform.parent = EmplacementParents.transform;
            Emplacement Emp1 = EmpObj.AddComponent<Emplacement>();
            Emp1.NameOfEmplacement = root["Emplacements"][i]["Name"];
            EmpObj.name = Emp1.NameOfEmplacement;
			Emp1.GenerateSticks(ParentObject, StickGameObject,StaticVars.RolesParseJson(root["Emplacements"][i]["Role"]),root["Emplacements"][i]["Pirority"].AsInt,i);
			base.Emplacements.Add (Emp1);
        }

        //GameObject EmpObj = new GameObject ();
        //EmpObj.transform.parent = EmplacementParents.transform;
        //Emplacement Emp1 = EmpObj.AddComponent<Emplacement>();
        //Emp1.NameOfEmplacement = "Viper 1 Checker";
        //EmpObj.name = Emp1.NameOfEmplacement;
        //Emp1.GenerateSticks (ParentObject,StickGameObject,0);
        //Emp1.RemoveStick (new DateTime (2016, 8, 15, 18, 00, 00));
        //Emp1.RemoveStick (new DateTime (2016, 8, 16, 22, 00, 00));

        //EmpObj = new GameObject ();
        //EmpObj.transform.parent = EmplacementParents.transform;
        //Emplacement Emp2 = EmpObj.AddComponent<Emplacement>();
        //Emp2.NameOfEmplacement = "Viper 1 Sentry";
        //EmpObj.name = Emp2.NameOfEmplacement;
        //Emp2.GenerateSticks (ParentObject,StickGameObject,1);

		GenerateDictionary ();
	}

	string[] GenerateDictionary()
	{
		List<string> OutputDictionary = new List<string>();
		for (int i = 0; i < Batches.Count; i++) 
		{
			for (int j = 0; j < Batches [i].ListOfPeople.Count; j++) 
			{
				Debug.Log ("Batch: " + Batches [i].BatchName + " - " + Batches [i].ListOfPeople [j].Name);
				OutputDictionary.Add (Batches [i].ListOfPeople [j].Name);
			}
		}
		return OutputDictionary.ToArray ();
	}

    

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			// The goal is to give the emplacement to the latest people.
			// However, I will give from top down, cause the latest people can fill up the rest easily.
			foreach (Emplacement tEmplacement in Emplacements) 
			{
				Debug.Log ("Doing " + tEmplacement.NameOfEmplacement);
				if(tEmplacement.Pirority != 0)
				{
					// This is one here to ignore the first stick cause its odd number.
					// We will cast it to the static var class in the future.
					for(int j = 1; j < (tEmplacement.ListOfSticks.Count - 1); j ++)
					{
						if(tEmplacement.ListOfSticks[j].Assigned == false)
						{
							for(int k = 0; k < Batches.Count; k++)
							{
								Batch tBatch = Batches [k];
								Debug.Log (tBatch.BatchName);
								foreach(Person tPersonal in tBatch.ListOfPeople)
								{
									Debug.Log (tPersonal.NoOfSticks + " - " + tPersonal.Name);
									if (tPersonal.NoOfSticks > 0)
									{
										TimeSpan TimePassed = tEmplacement.ListOfSticks[j].TimeStart - tPersonal.lastStickEndTiming;
										if (TimePassed.TotalHours >= 6) 
										{
											Debug.Log ("Assigning " + tPersonal.Name + " " + tEmplacement.name);
											tEmplacement.ListOfSticks[j].AssignPerson (tPersonal);
											break;
										}
										else
										{
											Debug.Log ("Did not set cause " + tPersonal.lastStickEndTiming + " did stick recently! " + tEmplacement.ListOfSticks [j].TimeStart);
											continue;
										}
									}
								}
							}
						}
					}
				}
			}
//			Emplacements [0].ListOfSticks [1].AssignPerson (Batches [0].ListOfPeople [0]);
//			Emplacements [0].ListOfSticks [2].AssignPerson (Batches [0].ListOfPeople [0]);
//			TimeSpan TimePassed = Emplacements [0].ListOfSticks [2].TimeEnd - Batches [0].ListOfPeople [0].lastStickEndTiming;
//			Debug.Log (TimePassed.TotalHours + " - " + Emplacements [0].ListOfSticks [2].TimeEnd + " - " + Batches [0].ListOfPeople [0].lastStickEndTiming);
//			foreach (Emplacement val in ListOfEmplacements) 
//			{
//				// If pirority == 0, means they require a certain skillset to do the emplacement duty.
//				// So, we cant assign them now like how we assign the rest cause we use people with the proper skillset only.
//				if (val.Pirority != 0) 
//				{
//					Debug.Log (val.Pirority);
//				}
//				else
//				{
//					Debug.Log ("Ignoring cause " + val.NameOfEmplacement);
//				}
//			}
		}

		#region CalculateSticks
		if(Input.GetKeyDown(KeyCode.Space))
		{
			// Parameters for the function
			int NoOfSticks = 59;
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
					Debug.Log(subNode ["Batches"] [j] ["Personnels"] [i] ["Name"]+ " - " + subNode["Batches"][j]["Personnels"][i]["Roles"][0].Value);
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
			int XZBaseStick = 8;

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
				//Debug.Log (temp.BatchName + " - " + val);
				TotalSticks += val;
			}
			//Debug.Log (TotalSticks);

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
						batchData.ClassData = temp;
						Debug.Log(temp.BatchStick.Count + " - " + temp.BatchName);
						if(temp.BatchStick.Count > 1)
						{
							int RandomVal = Random.Range(0, temp.NoOfPersonals);
							// This means uneven, So we need random.
							for(int n = 0; n < temp.NoOfPersonals; n++)
							{
								bool RandomDone = true;
								while(RandomDone)
								{
									int RandomIndex = Random.Range(0, temp.BatchStick.Count);
									if(temp.BatchStick[RandomIndex][1] > 0)
									{
										batchData.ListOfPeople[n].NoOfSticks = temp.BatchStick[RandomIndex][0];
										temp.BatchStick[RandomIndex][1]--;
										RandomDone = false;
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
									Debug.Log("Setting " + personal.Name + " to " + temp.BatchStick[0][0]);
									personal.NoOfSticks = temp.BatchStick[0][0];
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
			for (int k = 0; k < subNode ["Emplacements"].Count; k++) 
			{
				Debug.Log(root["Emplacements"][k]["Name"].Value + " - " + root["Emplacements"][k]["Role"].Value);
			}
		}
		#endregion
	}

	void CalculateStick (int[] BatchList, int TotalAmountOfSticks)
	{
		
	}

	bool RecursionCalculate (int[] Current, int[] Previous, bool IncreaseOrDecrease)
	{
		return true;
	}
}
