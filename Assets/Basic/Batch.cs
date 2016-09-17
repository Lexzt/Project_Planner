using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Batch : MonoBehaviour {

	public List<Person> ListOfPeople = new List<Person> ();
	public string BatchName;
	public int BatchNo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddPersonal(Person Data)
	{
		ListOfPeople.Add (Data);
	}
}
