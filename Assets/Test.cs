using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DoTaskWork ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void DoTaskWork()
	{
		Task t = new Task(TaskWork);
		t.Start();

		Task t2 = new Task(() => { Debug.Log("Task2"); });
		t2.Start();    
	}

	private void TaskWork()
	{
		Debug.Log ("Task1");
	}

}
