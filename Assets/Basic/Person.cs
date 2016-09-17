using UnityEngine;
using System;
using System.Collections;

public class Person : MonoBehaviour {

	public string Name;
	public string IC;
	public DateTime DOB;
	public Texture2D Photo;
	private int NoOfSticks;

	public Person (string tName, string tIC, DateTime tDOB)
	{
		Name = tName;
		IC = tIC;
		DOB = tDOB;
	}

	public void Set (string tName, string tIC, DateTime tDOB)
	{
		Name = tName;
		IC = tIC;
		DOB = tDOB;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
