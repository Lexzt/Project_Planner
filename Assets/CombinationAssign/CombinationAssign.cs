using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Combi
{
	public List<TempStick> Combination = new List<TempStick>();
	public int EfficiencyIndex;
	public Combi(List<TempStick> Combinations)
	{
		foreach (TempStick s in Combinations)
		{
			Combination.Add(new TempStick(s));
		}
	}
}
[System.Serializable]
public class TempPerson
{
	public string ListOfRoles;//List<Roles> ListOfRoles = new List<Roles>();
	//  public List<TempStick> ListOfPossibleSticks = new List<TempStick>();
	public SortedList<int,TempStick> ListOfAssginedSticks = new SortedList<int,TempStick>();
	public Person PersonData;
	public int NoOfSticks = 0;
	public TempPerson(TempPerson objToCopy)
	{
		//ListOfPossibleSticks = objToCopy.ListOfPossibleSticks;
		ListOfAssginedSticks = objToCopy.ListOfAssginedSticks;
		NoOfSticks = objToCopy.NoOfSticks;
		ListOfRoles = objToCopy.ListOfRoles;
	}
	void UpdateRoles(){
		ListOfRoles = "";
		foreach (Roles role in PersonData.ListOfRoles) {
			ListOfRoles += "["+role.ToString ()+"]";
		}
	}
	public TempPerson(Person p)
	{
		PersonData = p;
		UpdateRoles ();
		NoOfSticks = p.NoOfSticks;
	}
	public void Assign(TempStick s)
	{
		PersonData.ListOfSticks.Add(s.StickData);
		try{
			ListOfAssginedSticks.Add(s.order,s);
		}catch(Exception e){
			return;
		}
	}
	public void Unassign(TempStick s)
	{
		PersonData.ListOfSticks.Remove(s.StickData);
		ListOfAssginedSticks.Remove(s.order);//Because it does not overlap, we can remove by key.
	}
	public bool IsAssignable()
	{
		if (NoOfSticks > ListOfAssginedSticks.Count)
			return true;
		else
			return false;
	}
}
[System.Serializable]
public class TempStick
{
	public string role;
	public Stick StickData;
	public Person PersonData;
	public List<TempPerson> person_;
	public List<TempPerson> persons{
		get{
			return person_;
		}
		set{
			person_ = value;
		}
	}
	[SerializeField]
	int _order;
	public int order{
		get{
			return _order;
		}
	}
	public TempStick(TempStick objToCpy)
	{
		role = objToCpy.role;
		StickData = objToCpy.StickData;
		PersonData = objToCpy.PersonData;
		persons = objToCpy.persons;
		_order = objToCpy._order;
	}
	public TempStick(Stick s)
	{
		StickData = s;
		role = s.Parent.CurrentRole.ToString();
		_order = (int)(StickData.TimeStart - StaticVars.StartDate).TotalHours / StaticVars.StickInHours;
		persons = new List<TempPerson> ();
	}
	public bool IsAssigned()
	{
		return PersonData != null;
	}
	public void SetAssigned(TempPerson p)
	{
		PersonData = p.PersonData;
	}
	public void SetUnassigned()
	{
		PersonData = null;
	}
}

[System.Serializable]
public class TempGroup
{
	public List<TempPerson> perons;
	public List<TempStick> stickPool;

	public Dictionary<Roles,TempStick> stickByRoles;
};

public class CombinationAssign : MonoBehaviour
{
	[SerializeField]
	List<TempStick> Sticks;
	[SerializeField]
	List<Combi> Combinations;
	[SerializeField]
	List<TempPerson> Persons;

	int Progress = 0;
	int RealProgressStep = 0;
	int ProgressStepSize = 0;
	bool stop = true;
	Job myJob;

	StringBuilder debuglog = new StringBuilder();

	void Start()
	{
		ProgressStepSize = 1 << 30;

		myJob = new Job();
		myJob.InData = new Vector3[10];
		//myJob.Start(); // Don't touch any data in the job class after you called Start until IsDone is true.
	}
	List<int> mSeedPath = new List<int>();
	List<int> GenerateRandomSeed(List<TempPerson> candidates, List<TempStick> sticks){

		List<int> startSeed = new List<int>();
		System.Random r = new System.Random ();
		int max = candidates.Count;
		foreach (TempStick s in sticks) {
			startSeed.Add (r.Next(/**/0));/*
				max));/**/
		}
		return startSeed;
	}
	int IndexOfTempPersonFromTempStick(List<TempPerson> Candidates,TempStick ts){
		for (int i = 0; i < Candidates.Count; ++i) {
			if (ts.PersonData == Candidates [i].PersonData) {
				return i;
			}
		}
		return -1;
	}
	//Can be used to get path from an existing combination.
	//Maybe use intelligent generation/placment first.
	//Then use this to get the path
	//And brute force the placement from there.
	void GetPathFromCombi(List<TempPerson> Candidates, Combi combi, List<int> path){
		path.Clear ();
		foreach (TempStick ts in combi.Combination) {
			path.Add (IndexOfTempPersonFromTempStick (Candidates, ts));
		}
	}
	int IndexOfTempPersonFromStick(List<TempPerson> Candidates, Stick stick){
		for (int i = 0; i < Candidates.Count; ++i) {
			if (stick.DutyPersonal == Candidates [i].PersonData) {
				return i;
			}
		}
		return -1;
	}
	//Takes in a non-null list of TempPersons.
	//Persons must never be null nor empty.
	//Will populate the list of ouput only if it is empty
	public Combi GetCombiFromListOfSticks(List<Person> persons, List<Stick> sticks, List<TempPerson> output){
		return null;
	}
	public Combi GetCombiFromListOfSticks(List<Stick> sticks, List<TempPerson> listOfTempPersons){
		return null;
	}
	//TODO Encapsulate path traversal
	//Things to include:
	//List<TempPerson>
	//List<TempStick>
	//List<int> seedPath
	//List<Combi>
	//
	//Features include:
	//Generating a random seedPath
	//Getting a valid combi from the seedPath
	//Storing the current seedPath
	//Resuming from the current seedPath
	void GetNextValidCombi(List<TempPerson> tp, List<int> seedPath){
		stop = false;
		seedPath [seedPath.Count - 1]++;
		AssignAndCheckR (tp, seedPath);
	}
	void Update()
	{
		if (myJob != null)
		{
			if (myJob.Update())
			{
				// Alternative to the OnFinished callback
				myJob = null;
			}
		}

		if (stop == true && Input.GetKeyDown(KeyCode.Y))
		{
			Combinations = new List<Combi>();
			debug = new StringBuilder();
			stop = false;
			Persons = PrepareAACR();
			/*StringBuilder sb = new StringBuilder ();
			foreach (TempPerson p in Persons) 
			{
				sb.Append (p.PersonData.Name + "\n");
				foreach (TempStick s in p.ListOfPossibleSticks) 
				{
					sb.Append ("\t" + s.StickData.TimeStart + " - " + s.StickData.Parent.NameOfEmplacement + "\n");
				}
			}
			System.IO.File.WriteAllText(Application.dataPath + "/" + "debug1.txt", sb.ToString());*/
			//	sb = new StringBuilder ();
			//AssignAndCheckR(tp,sb,0);
			mSeedPath = GenerateRandomSeed (Persons,Sticks);
			Debug.Log ("Seed ");
			Debug.Log ("Path: ");
			string sP = "|";
			foreach (int seed in mSeedPath) {
				sP+= (seed + "|");
			}
			DateTime startTime = DateTime.Now;
			Debug.Log ("Start Time: " + startTime);
			Debug.Log (sP);
			try{
				//try{
				AssignAndCheckR(Persons,mSeedPath,0);
				//}catch(Exception e){
				//	if (e.Message != "Done")
				//		throw e;
				//}
			}catch(UnityException e){
				Debug.LogError (e);
			}
			Debug.Log("Start Time: " + startTime + " End Time: "+DateTime.Now+ " Time Elapsed: "+(startTime - DateTime.Now).TotalMinutes);
			Debug.Log ("Final ");
			Debug.Log ("Path: ");
			sP = "|";
			foreach (int seed in mSeedPath) {
				sP+= (seed + "|");
			}
			Debug.Log (sP);
			Debug.Log (irPrint.ToString ());
			//Debug.Log(debug);
			//System.IO.File.WriteAllText(Application.dataPath + "/" + "debug.txt", debug);
			stop = true;
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			if (Combinations != null)
			{
				StringBuilder sb = new StringBuilder ();
				sb.Append ("No Of Combinations: " + Combinations.Count + "\n");
				Debug.Log(Combinations.Count);
				foreach (Emplacement emp in GetComponent<Base>().Emplacements)
				{
					emp.Reset();
				}

				foreach (TempStick s in Combinations[0].Combination) 
				{
					s.StickData.AssignPerson (s.PersonData);
				}
				return;
				string order = "";
				int index = 0;
				foreach (Combi comb in Combinations)
				{
					//order += index++ + "|\n";
					sb.Append (index++ + "|\n");
					foreach (TempStick s in comb.Combination)
					{
						sb.Append (s.StickData.TimeStart + " - " + s.StickData.Parent.NameOfEmplacement + " - " + s.PersonData.Name + "\n");
						//order += s.StickData.TimeStart + " - " + s.StickData.Parent.NameOfEmplacement + " - " + s.PersonData.Name + "\n";
					}
					sb.Append ("|\n");
					//order += "|\n";
				}
				//Debug.Log(order);
				System.IO.File.WriteAllText(Application.dataPath + "/" + "order.txt", sb.ToString());

				order = "";
			}
		}

		//if(Input.GetKeyDown(Keycode.
	}

	List<TempPerson> PrepareAACR()
	{
		List<TempPerson> c = new List<TempPerson>();
		List<TempStick> s = new List<TempStick>();

		List<Batch> AllBatches = GetComponent<Base>().Batches;
		foreach (Batch b in AllBatches)
		{
			foreach (Person p in b.ListOfPeople)
			{
				c.Add(new TempPerson(p));
			}
		}

		List<Emplacement> AllEmp = GetComponent<Base>().Emplacements;
		foreach (Emplacement emp in AllEmp)
		{
			if (!emp.IsSpecialRole())
			{
				foreach (Stick stick in emp.ListOfSticks)
				{
					if (stick.State == StickState.ENABLED)
					{
						s.Add(new TempStick(stick));
					}
				}
			}
		}
		foreach (TempPerson p in c)
		{
			foreach (TempStick tick in s)
			{
				if (CanDo (p, tick)) {
					//p.ListOfPossibleSticks.Add (tick);
					tick.persons.Add (p);
				}

			}
		}
		/*
        string debuglog = "\n";
        foreach (TempPerson p in c)
        {
			debuglog += (p.PersonData.Name + " - " + p.ListOfPossibleSticks.Count + " - " + p.NoOfSticks + "\n");
            foreach (TempStick st in p.ListOfPossibleSticks)
            {
                debuglog += ("|" + st.StickData.TimeStart + " - " + st.StickData.Parent.NameOfEmplacement + "|\n");
            }
            debuglog += "\n";
        }
        Debug.Log(debuglog);
*/
		Sticks = s;
		return c;
	}
	StringBuilder debug = new StringBuilder();
	int count = 0;
	bool AssignAndCheckR(List<TempPerson> C, StringBuilder trace, int depth = 0)
	{
		for (int i = 0; i < C.Count; i++)
		{
			if (stop)
			{
				throw new UnityException("Force stopped");
			}
			TempStick s = Sticks[depth];
			TempPerson c = C[i];
			if (c.IsAssignable())// && c.ListOfPossibleSticks.Contains(s))
			{
				string msg = ("\tDepth: " + depth + ", i: " + i + " ," + c.PersonData.Name + ", ListOfPossibleSticks.Count: " /*+ c.ListOfPossibleSticks.Count*/ + " | ");
				//debug.Append (trace.ToString() + msg);
				//debug.Append (Environment.NewLine);
				Progress++;
				if (++Progress >= ProgressStepSize)
				{
					Progress = 0;
					RealProgressStep++;
				}
				//debug.Append ("\t|" + !s.IsAssigned () + " - " + CanDo (c, s));
				//debug.Append (Environment.NewLine);
				if (!s.IsAssigned() && CanDo(c, s))
				{
					//	debug.Append("\t| (Assigned) " + c.PersonData.Name + " | " + s.StickData.TimeStart.ToShortTimeString() + " | " + s.StickData.Parent.NameOfEmplacement + " | ");
					//	debug.Append (Environment.NewLine);
					c.Assign(s);
					s.SetAssigned(c);

					if (AllAssigned())
					{
						//		debug.Append("\t| (Combination Generated) ");
						foreach (TempStick v in Sticks)
						{
							//			debug.Append("|" + v.PersonData.Name + " - " + v.StickData.TimeStart + " - " + v.StickData.Parent.NameOfEmplacement + "| ");
						}
						//		debug.Append(Environment.NewLine);
						Combinations.Add(new Combi(Sticks));
						c.Unassign(s);
						///*
						s.SetUnassigned();/*/
						/*/return true;/*/
						continue;/*/
						/*/throw new Exception("boom");/**/
					}
					StringBuilder tempbuilder = new StringBuilder (trace.ToString() + msg);
					//System.IO.File.WriteAllText(Application.dataPath + "/" + "debug2.txt", tempbuilder.ToString());
					bool ret = AssignAndCheckR(C, tempbuilder, depth + 1);
					c.Unassign(s);
					s.SetUnassigned();
					if (ret)
						return true;
				}
			}
		}
		return false;
	}
	bool AssignAndCheckR(List<TempPerson> C,List<int> seed,int depth = 0)    {
		if (depth >= Sticks.Count)
			return false;
		/*int seedVal = 0;
		if (seed != null && seed.Count > 0) {
			seedVal = seed [0];
			seed.RemoveAt (0);
		}*/
		TempStick s = Sticks[depth];
		int maxCount = /**/s.persons.Count;/*
		C.Count;/**/
		List<TempPerson> persons = /**/s.persons;/*
		C;/**/
		for (int i = seed[depth]%maxCount; i < maxCount; i++)
		{
			seed [depth] = i;
			TempPerson c = persons[i];
			if (stop)
			{
				throw new UnityException("Force stopped");
			}
			if (c.IsAssignable())//&& c.ListOfPossibleSticks.Contains (s))
			{
				Progress++;
				if (++Progress >= ProgressStepSize)
				{
					Progress = 0;
					RealProgressStep++;
				}
				if (!s.IsAssigned() && CanDo(c, s))
				{
					c.Assign(s);
					s.SetAssigned(c);

					if (AllAssigned ()) {
						//System.IO.File.WriteAllText(Application.dataPath + "/" + "debug3.txt", debuglog.ToString());
						Combi newCombi =new Combi (Sticks);
						Combinations.Add (newCombi);
						c.Unassign (s);
						s.SetUnassigned ();
						//If this combination is satisfactory
						//if( EvaulateCombi(newCombi) )
						//throw new Exception ("Done");
						return true;
						//throw new UnityException ("First combination Generated!");
					} else {
						bool result = AssignAndCheckR (C, seed, depth + 1);
						c.Unassign (s);
						s.SetUnassigned ();
						if (result) {
							return true;
						}
					}
				}
			}
		}
		seed [depth] = 0;
		return false;
	}
	StringBuilder irPrint = new StringBuilder ();
	bool IsRested(TempPerson tp, TempStick ts){
		foreach (KeyValuePair <int,TempStick> kvp in tp.ListOfAssginedSticks) {
			int diff = kvp.Value.order - ts.order;
			/*irPrint.Append (ts.StickData.name);
			irPrint.Append (ts.order);
			irPrint.Append (kvp.Value.StickData.name);
			irPrint.Append (kvp.Value.order);
			irPrint.Append ('\n');*/
			if (diff*diff <= 2 * 2) {
				return false;
			}
			//If this kvp is more than 2 sticks past the stick's order, the rest of the sticks in this list will be too.
			if (diff > 2) {
				//return true;
				break;
			}
		}
		return true;
	}
	public bool CanDo(TempPerson p, TempStick s)
	{
		if (
			p.PersonData.ListOfRoles.Contains(s.StickData.Parent.CurrentRole) &&
			p.ListOfAssginedSticks.Count < p.NoOfSticks)
		{
			if (
				//p.PersonData.IsRested (s.StickData.TimeStart, s.StickData.TimeEnd) /*
				IsRested(p,s)/**/
				|| CheckContiguous (s, p)) 
			{
				return true;
			}
		}
		return false;
	}

	public bool AllAssigned()
	{
		foreach (TempStick s in Sticks)
		{
			if (!s.IsAssigned())
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckContiguous(TempStick s, TempPerson p)
	{
		List<TempStick> contiguousSticks = new List<TempStick> ();
		TempStick cS = null;
		//for (int i=0; i<p.ListOfAssginedSticks.Count;++i)
		foreach(KeyValuePair<int,TempStick> kvp in p.ListOfAssginedSticks)
		{
			TempStick assignedStick = kvp.Value;
			if (IsConnected(assignedStick, s))
			{
				if (cS != null)
					return false;
				cS = assignedStick;
				//contiguousSticks.Add(assignedStick);
			}
			else if(!SingleIsRested(s, assignedStick))
			{
				return false;
			}
			//If we have iterated past the relevant sticks
			if (kvp.Key - s.order > 1) {
				break;
			}
		}
		//if (contiguousSticks.Count < 2)
		//  {
		//if (contiguousSticks.Count == 0)
		if(cS == null)
		{
			return false;
		}
		foreach(KeyValuePair<int,TempStick> kvp in p.ListOfAssginedSticks)
		{
			TempStick tempAssignedStick = kvp.Value;
			if (cS.StickData != tempAssignedStick.StickData && IsConnected(tempAssignedStick, cS))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsConnected(TempStick s1, TempStick s2)
	{
		if(s1.StickData.Parent == s2.StickData.Parent)
		{
			int gap = (s1.order - s2.order);
			if (gap * gap == 1) {
				return true;
			}
			/*TimeSpan ts = s1.StickData.TimeEnd - s2.StickData.TimeStart;
			if (ts.TotalHours > 0) 
			{
				ts = s2.StickData.TimeEnd - s1.StickData.TimeStart;
			}
			if (Mathf.FloorToInt ((float)ts.TotalHours) == 0) 
			{
				return true;	
			}*/
		}
		return false;
	}

	public bool SingleIsRested (TempStick s1, TempStick s2)
	{
		int HoursDiff = s1.order - s2.order;
		if ((HoursDiff * HoursDiff) > (2*2) && s1 != s2) {
			return true;
		} else {
			return false;
		}
		int FirstStartDiff = (int)(s1.StickData.TimeStart - s2.StickData.TimeStart).TotalHours;
		// The new stick starts AFTER the old stick starts.
		//Debug.Log ("0: " + StickStartTiming.ToString() + " - " + lastStick.TimeEnd.ToString());
		if (FirstStartDiff > 0) 
		{
			// The new stick is starting after the old stick starts. 
			int EndDiff = (int)(s1.StickData.TimeStart - s2.StickData.TimeEnd).TotalHours;
			//Debug.Log ("1: " + EndDiff + " - " + StickStartTiming.ToString() + " - " + lastStick.TimeEnd.ToString());
			if (EndDiff > 0) 
			{
				// This means the old stick, ends before your new stick starts. 
				HoursDiff = Mathf.Abs(EndDiff);
			}
		}
		else if(FirstStartDiff < 0)
		{
			int EndDiff = (int)(s1.StickData.TimeEnd - s2.StickData.TimeStart).TotalHours;
			//Debug.Log ("2: " + EndDiff + " - " + StickStartTiming.ToString() + lastStick.TimeEnd.ToString());
			if (EndDiff < 0) 
			{
				// This means my new sticks, ends before my old stick starts.
				HoursDiff = Mathf.Abs(EndDiff);
			}
		}
		if (HoursDiff < StaticVars.RestAfterSticks * StaticVars.StickInHours) 
		{
			//debuglog.AppendLine ("(" + s1.StickData.Parent.NameOfEmplacement + ")(" + s2.StickData.Parent.NameOfEmplacement + ") HoursDiff: " + HoursDiff + " | "); 
			//debuglog.AppendLine ("s1: " + s1.StickData.TimeStart + " - " + s1.StickData.TimeEnd + " | ");
			//	debuglog.AppendLine ("s2: " + s2.StickData.TimeStart + " - " + s2.StickData.TimeEnd + " | ");
			return false;
		}
		HoursDiff = 0;
		return true;
	}
	public class SticksAndRoles{
		public List<TempStick> sticks;
		public List<TempPerson> persons;
		public SticksAndRoles (){
			sticks = new List<TempStick>();
			persons = new List<TempPerson>();
		}
	}
	public List<TempGroup> SplitGroups (List<TempPerson> p, List<TempStick> s)
	{
		Dictionary<Roles,SticksAndRoles> sortByRoles = new Dictionary<Roles, SticksAndRoles>();
		List<TempPerson> multiRoles = new List<TempPerson>();

		foreach (TempPerson temp in p) 
		{
			foreach (Roles r in temp.ListOfRoles) 
			{
				if (!sortByRoles.ContainsKey (r)) 
					sortByRoles.Add (r, new SticksAndRoles ());

			}
			if (temp.PersonData.ListOfRoles.Count == 1)
				sortByRoles [temp.PersonData.ListOfRoles [0]].persons.Add (temp);//(()?sortByRoles [r].persons.Count-1:0)temp);
			else {
				multiRoles.Add (temp);
			}
		}

		foreach (TempStick temp in s) 
		{
			if (!sortByRoles.ContainsKey (temp.StickData.Parent.CurrentRole)) 
				sortByRoles.Add (temp.StickData.Parent.CurrentRole, new SticksAndRoles ());

			sortByRoles [temp.StickData.Parent.CurrentRole].sticks.Add (temp);

		}

		List<TempGroup> returnList = new List<TempGroup>();
		int NoOfGroups = 2;
		int j = 0;
		for (int i = 0; i < NoOfGroups; i++) 
		{
			TempGroup tg = new TempGroup ();
			returnList.Add (tg);
		}
		foreach (KeyValuePair<Roles,SticksAndRoles> kvp in sortByRoles) 
		{
			j = 0;
			for (int k =0; k<kvp.Value.sticks.Count;++k,j++) {
				returnList [j % NoOfGroups].stickByRoles.Add (kvp.Key, kvp.Value.sticks[k]);
				if (!kvp.Value.sticks [k].StickData.Unique) {
					j--;
				}
			}
			//for(int i=0;i<
		}


		return returnList;
	}
}
