using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Batch : MonoBehaviour {

	public List<Person> ListOfPeople = new List<Person> ();
	public string BatchName;
	public int BatchNo;
	public BatchClass ClassData;

	public Batch(Batch tBatch)
	{
		ListOfPeople = new List<Person> ();
		BatchName = tBatch.BatchName;
		BatchNo = tBatch.BatchNo;
		ClassData = tBatch.ClassData;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddPersonal(Person Data)
	{
		if (ListOfPeople == null) 
		{
			ListOfPeople = new List<Person> ();
		}
		ListOfPeople.Add (Data);
	}
}
