using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ChangiAirBaseEast : Base {

	public DateTime StartDate;
	public DateTime EndDate;

	public GameObject BarObject;

	public GameObject ParentObject;
	public GameObject StickGameObject;

	public GameObject SearchInputField;

	// Use this for initialization
	void Start () 
    {
        Debug.Log("No Of Hours: " + (StaticVars.EndDate - StaticVars.StartDate).TotalHours);
        
        GameObject BatchParents = new GameObject("Batches");
        GameObject EmplacementParents = new GameObject("Emplacements");
        
        TextAsset Json = Resources.Load("data") as TextAsset;
        JSONNode root = JSON.Parse(Json.text);
        
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
	void Update () {
	
	}
}
