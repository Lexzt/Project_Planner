using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChangiAirBaseEast : Base {

	public DateTime StartDate;
	public DateTime EndDate;

	public GameObject BarObject;

	public GameObject ParentObject;
	public GameObject StickGameObject;

	public GameObject SearchInputField;

	// Use this for initialization
	void Start () {
		Debug.Log ("No Of Hours: " + (StaticVars.EndDate - StaticVars.StartDate).TotalHours);


//		Person Keith = new Person("Keith","S9434749A",new DateTime(1994,9,16));
//		Batch Batch1 = gameObject.AddComponent<Batch> ();
//		Batch1.BatchName = "Team Brian";
//		Batch1.AddPersonal (Keith);
//
//		Person Minghan = new Person("Minghan","S938283C",new DateTime(1993,4,09));
//		Batch Batch2 = gameObject.AddComponent<Batch>();
//		Batch2.BatchName = "2 Man Solo";
//		Batch2.AddPersonal (Minghan);
//
//		Person YC = new Person("YC","S9651234N",new DateTime(1996,6,23));
//		Batch Batch3 = gameObject.AddComponent<Batch>();
//		Batch3.BatchName = "IDontevenKnow";
//		Batch3.AddPersonal (YC);
//
//		base.Batches.Add (Batch1);
//		base.Batches.Add (Batch2);
//		base.Batches.Add (Batch3);

		GameObject PersonObjects = new GameObject ("People");
		GameObject Person = new GameObject ();
		Person.transform.parent = PersonObjects.transform;
		Person Keith = Person.AddComponent<Person> ();
		Keith.Set("Keith","S9434749A",new DateTime(1994,9,16));
		Person.name = Keith.Name;

		GameObject BatchParents = new GameObject ("Batches");
		GameObject BatchObj = new GameObject ();
		BatchObj.transform.parent = BatchParents.transform;

		Batch Batch1 = BatchObj.AddComponent<Batch> ();
		Batch1.BatchName = "Team Brian";
		Batch1.AddPersonal (Keith);
		base.Batches.Add (Batch1);

		GameObject EmplacementParents = new GameObject ("Emplacements");

		GameObject EmpObj = new GameObject ();
		EmpObj.transform.parent = EmplacementParents.transform;
		Emplacement Emp1 = EmpObj.AddComponent<Emplacement>();
		Emp1.NameOfEmplacement = "Viper 1 Checker";
		EmpObj.name = Emp1.NameOfEmplacement;
		Emp1.GenerateSticks (ParentObject,StickGameObject,0);
		Emp1.RemoveStick (new DateTime (2016, 8, 15, 18, 00, 00));
		Emp1.RemoveStick (new DateTime (2016, 8, 16, 22, 00, 00));


		EmpObj = new GameObject ();
		EmpObj.transform.parent = EmplacementParents.transform;
		Emplacement Emp2 = EmpObj.AddComponent<Emplacement>();
		Emp2.NameOfEmplacement = "Viper 1 Sentry";
		EmpObj.name = Emp2.NameOfEmplacement;
		Emp2.GenerateSticks (ParentObject,StickGameObject,1);

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
	void Update () {
	
	}
}
