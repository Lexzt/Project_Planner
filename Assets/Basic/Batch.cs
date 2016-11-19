using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Batch : MonoBehaviour {

	public List<Person> ListOfPeople = new List<Person> ();
	public string BatchName;
	public int BatchNo;
	public BatchClass ClassData = null;
	public bool DoEasy = false;
	public bool ICT = false;
	public DateTime ORD;
	public GameObject BaseParent;

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

	public void Reset ()
	{
		foreach (Person personal in ListOfPeople) 
		{
			personal.Reset ();
		}
	}

	public JSONNode ToJSON ()
	{
		JSONNode node = new JSONClass ();
		node ["DoEasy"].AsBool = DoEasy;
		node ["BatchName"] = BatchName;
		node ["ORD Month And Year"] = ORD.ToString ();

		JSONNode personelArray = new JSONArray ();
		int i = 0;
		if(ListOfPeople != null)
		{
			foreach (Person personel in ListOfPeople) 
			{
				JSONNode data = personel.ToJSON ();
				personelArray [i++] = data;
			}
		}
		node ["Personnels"] = personelArray;
		return node;
	}

	public void AllReset ()
	{
		ClassData = null;
		foreach (Person personal in ListOfPeople) 
		{
			personal.AllReset ();
		}
	}

	public void RemoveMe()
	{
		// foreach(Person personal in ListOfPeople)
		// {
		// 	ListOfPeople.Remove(personal);
		// 	Destroy(this.gameObject);
		// }
		BaseParent.GetComponent<Base>().Batches.Remove(this);
		Destroy(this.gameObject);
	}
}
