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
		BatchStick.Add (new int[] { tNoOfPeronals, tBatchAmount });
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
	public bool UnEven = false;
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
                Debug.Log(root["Batches"][j]["Personnels"][i]);
                GameObject Person = new GameObject();
                Person.transform.parent = BatchObj.transform;
                Person tempPerson = Person.AddComponent<Person>();
                tempPerson.Set(root["Batches"][j]["Personnels"][i]["Name"], root["Batches"][j]["Personnels"][i]["IC"], new DateTime(root["Batches"][j]["Personnels"][i]["DOB"].AsInt));
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
            Emp1.GenerateSticks(ParentObject, StickGameObject,RolesParseJson(root["Emplacements"][i]["Role"]) ,i);
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

    Roles RolesParseJson(string Json)
    {
        switch (Json)
        {
            case "Checker":
                return Roles.eCHECKER;
                break;
            case "Sentry":
                return Roles.eSENTRY;
                break;
            case "Pass Office":
                return Roles.ePASS_OFFICE;
                break;
            case "Console":
                return Roles.eCONSOLE;
                break;
            case "Driver":
                return Roles.eDRIVER;
                break;
            default:
                return Roles.eNONE;
                break;
        }
    }

	// Update is called once per frame
	void Update () 
	{
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
			int XZBaseStick = 6;

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
			while (loop && breakpoint != 10) 
			{
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
					if (diff > 0) 
					{
						// Okay, This means we need the value is positive. 
						// This means we have to Increase the amount of sticks
						// So we can match the amount of stick.
						foreach (BatchClass temp in StickCount) 
						{
							if (temp.XZ != MaxXZ) 
							{
								// This is for max xz

							}
							else
							{
								// This is for not max xz

							}
						}
					} 
					else if (diff < 0) 
					{
						// Okay, This means we need the value is negative. 
						// This means we have to too much stick with our current configuration.
						// So we have to decrease our previous configuration. 
					}
				}
				breakpoint++;
			}

			foreach (BatchClass temp in StickCount) 
			{
				Debug.Log (temp.BatchName + " - " + temp.BatchStick [0] [0] + " - " + temp.BatchStick [0] [1]);
			}
		}
	}

	void CalculateStick (int[] BatchList, int TotalAmountOfSticks)
	{
		
	}

	bool RecursionCalculate (int[] Current, int[] Previous, bool IncreaseOrDecrease)
	{
		return true;
	}
}
