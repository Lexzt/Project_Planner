using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[System.Serializable]
public class Combi
{
	public List<int> combiIndex = null;
	public List<TempStick> Combination = new List<TempStick> ();
	//	public List<TempPerson> Combination = new List<TempPerson>();
	public Dictionary<Person,List<TempStick>> starters = new Dictionary<Person, List<TempStick>>();
	public Dictionary <Person, List<TempStick>> enders = new Dictionary<Person, List<TempStick>>();

	public int EfficiencyIndex;
	int smallestOrder = 1<<30, largestOrder = 0;

	void GenerateStarters(){
		Dictionary<Person,List<TempStick>> dict = starters;
		dict.Clear ();
		for (int i = 0; i < Combination.Count; ++i) {
			TempStick s = Combination [i];
			if (s.order < (smallestOrder + StaticVars.RestAfterSticks)) { 
				if(!dict.ContainsKey(s.PersonData))
					dict.Add (s.PersonData,new List<TempStick>());
				dict [s.PersonData].Add (s);
			} else if (s.order < (smallestOrder + StaticVars.MaxNoOfDutyInARow)) {
				if (dict.ContainsKey (s.PersonData))
					dict [s.PersonData].Add(s);
			}else{
				break;
			}
		}
	}

	void GenerateEnders(){
		Dictionary<Person,List<TempStick>> dict = enders;
		dict.Clear ();
		for(int i =Combination.Count-1;i>=0;--i){
			TempStick s = Combination [i];
			if (s.order > (largestOrder - StaticVars.RestAfterSticks)) {
				if(!dict.ContainsKey(s.PersonData))
					dict.Add (s.PersonData,new List<TempStick>());
				dict [s.PersonData].Add (s);
			} else if (s.order > (largestOrder - StaticVars.MaxNoOfDutyInARow)) {
				if (dict.ContainsKey (s.PersonData))
					dict [s.PersonData].Add(s);
			} else {
				break;
			}
		}
	}
	public Combi(List<TempStick> Combinations, int index = -1)
	{
		combiIndex = new List<int> ();
		combiIndex.Add(index);
		if (Combinations != null) {
			foreach (TempStick s in Combinations) {
				if (s.IsAssigned ()) {
					if (smallestOrder > s.order)
						smallestOrder = s.order;
					if (largestOrder < s.order)
						largestOrder = s.order;
					Combination.Add (new TempStick (s));
				}
			}
			Combination.Sort (new TStickOrderComparer ());
			GenerateEnders ();
			GenerateStarters ();
		}
	}
	bool SufficientRest(List<TempStick> sticks1, List<TempStick> sticks2){
		return (sticks1.Count + sticks2.Count) <= StaticVars.MaxNoOfDutyInARow && sticks1 [sticks1.Count - 1].StickData.Parent == sticks2 [0].StickData.Parent;
	}
	public bool CanConnect(Combi nextCombi){
		Dictionary<Person,List<TempStick>> otherDict = null, myDict = null;
		if (nextCombi.smallestOrder == largestOrder+1) {
			otherDict = nextCombi.starters;
			myDict = enders;
		}
		if (smallestOrder == nextCombi.largestOrder+1) {
			otherDict= nextCombi.enders;
			myDict = starters;
		}
		int otherP = -1;
		try{
			foreach (KeyValuePair<Person,List<TempStick>> kvp in myDict) {
				Person p = kvp.Key;
				//if(p.name == "XZ 11")
				//	Debug.Log(kvp.Value + ((otherDict.ContainsKey(p))?otherDict[p]:0));
				//if ((otherP = otherDict.IndexOf (p)) > -1) {
				if(otherDict.ContainsKey(p)){
					if (!SufficientRest (kvp.Value,otherDict[p] ))
						return  false;
				}
			}
		}catch(Exception e){
			throw (e);
		}
		return true;
	}
	//Warning, this will store references.
	public Combi AddToCombi(Combi c2, int index =-1){
		combiIndex.Insert (0, index);
		Combi c1 = this;
		smallestOrder = Math.Min (c1.smallestOrder, c2.smallestOrder);
		largestOrder = Math.Max (c1.largestOrder, c2.largestOrder);

		foreach (TempStick s in c2.Combination) {
			Combination.Add (s);
		}
		return this;
		//No need to re-sort because both combinations are expected to be sorted and non-overlapping in order
	}
	//Warning, this remove by references.
	public void RemoveFromCombi(Combi c2){
		foreach (TempStick s in c2.Combination) {
			Combination.Remove(s);
		}
		//No need to re-sort because both combinations are expected to be sorted.
		if (c2.largestOrder == largestOrder) {
			//Recalc largest order
			largestOrder = Combination[Combination.Count-1].order;
		}
		if (c2.smallestOrder == smallestOrder) {
			//recalc smallest order
			smallestOrder = Combination[0].order;
		}


	}
	public Combi(Combi c1, Combi c2){
		smallestOrder = Math.Min (c1.smallestOrder, c2.smallestOrder);
		largestOrder = Math.Max (c1.largestOrder, c2.largestOrder);
		foreach (TempStick s in c1.Combination) {
			Combination.Add (new TempStick (s));
		}
		foreach (TempStick s in c2.Combination) {
			Combination.Add (new TempStick (s));
		}
		Combination.Sort (new TStickOrderComparer ());
		GenerateEnders ();
		GenerateStarters ();
	}

	public static void ReadCombiPath(string combiPath, List<int> oCombiPath){
		int firstVal = 0;
		int lastVal = combiPath.IndexOf (' ',firstVal);
		string sCount = combiPath.Substring (firstVal, lastVal);
		firstVal = lastVal;
		int count = int.Parse (sCount);
		int i = 0;
		while( (lastVal = combiPath.IndexOf(' ', firstVal)) > firstVal && i++ <= count){
			oCombiPath.Add(int.Parse (combiPath.Substring (firstVal, lastVal)));
			firstVal = lastVal;
		}
	}
	public string CombiPath(){
		string combiStr = combiIndex.Count.ToString()+' ';
		foreach (int i in combiIndex) {
			combiStr += i.ToString() + ' ';
		}
		return combiStr;
	}

}

[System.Serializable]
public class TempPerson
{
	public List<TempStick> ListOfPossibleSticks = new List<TempStick>();
	public List<TempStick> ListOfAssginedSticks = new List<TempStick>();
	public Person PersonData;
	public int NoOfSticks = 0;
	public TempPerson(TempPerson objToCopy)
	{
		ListOfPossibleSticks = objToCopy.ListOfPossibleSticks;
		ListOfAssginedSticks = objToCopy.ListOfAssginedSticks;
		NoOfSticks = objToCopy.NoOfSticks;
	}
	public TempPerson(Person p)
	{
		PersonData = p;
		NoOfSticks = p.NoOfSticks;
	}
	public void Assign(TempStick s)
	{
		if (PersonData.ListOfSticks.Contains (s.StickData) || ListOfAssginedSticks.Contains(s))
			return;
		PersonData.ListOfSticks.Add(s.StickData);
		ListOfAssginedSticks.Add(s);
	}
	public void UnassignAll(){
		foreach (TempStick s in ListOfAssginedSticks) {
			PersonData.ListOfSticks.Remove (s.StickData);
		}
		ListOfAssginedSticks.Clear ();
	}
	public void Unassign(TempStick s)
	{
		PersonData.ListOfSticks.Remove(s.StickData);
		ListOfAssginedSticks.Remove(s);
	}
	public bool IsAssignable()
	{
		if (NoOfSticks > ListOfAssginedSticks.Count) return true;
		else return false;
	}
}
public class TStickOrderComparer : IComparer<TempStick>{
	public int Compare(TempStick ts, TempStick ts2){
		return ts.order - ts2.order;
	}
}
[System.Serializable]
public class TempStick
{
	public string role;
	public Stick StickData;
	public Person PersonData;
	public string nameOfEmplacement;
	[SerializeField]
	int _order;
	public int order{
		get{
			return _order;
		}
	}
	public TempStick(TempStick objToCpy)
	{
		StickData = objToCpy.StickData;
		PersonData = objToCpy.PersonData;
		_order = objToCpy._order;
	}
	public TempStick(Stick s)
	{
		StickData = s;
		role = s.Parent.CurrentRole.ToString();
		_order = (int)(StickData.TimeStart - StaticVars.StartDate).TotalHours / StaticVars.StickInHours;
		nameOfEmplacement = s.Parent.NameOfEmplacement;
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
public class CombiExplorer {
	public long pathsQuota = 0;
	public List < int > fullCombiPath = null;

	//Check if the list of temppersons is still properly rested.
	public bool ValidatePersons() {
		//Cycle through all the persons and check if they have enough rest.
		foreach(KeyValuePair < Person, List < int >> kvp in assignedPersons) {
			if (kvp.Value.Count > kvp.Key.OriginNoOfSticks) {
				return false;
			}

			//Can skip all these because of the additional check when doing CanConnect.
			List < int > listOfAssignedSticks = kvp.Value;
			int span = 0;
			int prevOrder = 0;
			//p.ListOfAssginedSticks.Sort (new TStickOrderComparer ());

			foreach(int tso in listOfAssignedSticks) {
				//Not connected to the previous stick

				if (tso > 1 + prevOrder) {
					span = 0;
					//     if (tso > 3 || prevOrder > 3) {
					//      Debug.Log (tso + " " + prevOrder);
					//      if (!((tso - prevOrder) > StaticVars.RestAfterSticks)) {
					//       Debug.Log ("Return false");
					//      }
					//     }
					if (!((tso - prevOrder) > StaticVars.RestAfterSticks)) {
						return false; //Not enough rest
					}
				} else {
					//Connected to the previous stick.
					if (++span > StaticVars.MaxNoOfDutyInARow)
						return false; //Too many sticks connected
				}
				prevOrder = tso;
				/*
    if ( ((tso - prevOrder) <= StaticVars.RestAfterSticks)) {
     Debug.Log ("IDK WHY MAN");
     return false;//Not enough rest
    }*/
			}
		}
		return true;
	}

	Dictionary < Person, List < int >> assignedPersons = new Dictionary < Person, List < int >> ();
	public void UpdateListFromCombi(Combi c) {

		foreach(TempStick ts in c.Combination) {
			if (assignedPersons.ContainsKey(ts.PersonData)) {
				assignedPersons[ts.PersonData].Add(ts.order);
			} else {
				List < int > newList = new List < int > ();
				newList.Add(ts.order);
				assignedPersons.Add(ts.PersonData, newList);
			}
		}
		foreach(KeyValuePair < Person, List < int >> kvp in assignedPersons) {
			kvp.Value.Sort();
		}
	}
	public void RevertListFromCombi(Combi c) {
		foreach(TempStick ts in c.Combination) {
			if (assignedPersons.ContainsKey(ts.PersonData)) {
				assignedPersons[ts.PersonData].Remove(ts.order);
			}
		}
	}
	int stepsTaken = 0;
	public List < int > GetFullCombi(List < List < Combi >> combiSteps, Combi prev = null, int index = 0) {
		List < Combi > combis = combiSteps[index];
		int i = 0;

		if (fullCombiPath != null) {
			if (index == combiSteps.Count - 1)
			if (--pathsQuota <= 0) return null; //Tracks and limits the combis
			if (index < fullCombiPath.Count)
				i = fullCombiPath[index];
		}

		for (; i < combis.Count; ++i) {
			stepsTaken++;
			if (stepsTaken > pathsQuota)
				return null;
			Combi c = combis[i];
			UpdateListFromCombi(c);
			if ((prev == null || prev.CanConnect(c)) &&
				ValidatePersons()) {
				if (index + 1 < combiSteps.Count) {
					List < int > ret = GetFullCombi(combiSteps, c, index + 1);
					if (ret != null) {
						ret.Insert(0, combis.IndexOf(c)); //ret.AddToCombi (c,combis.IndexOf(c));
						fullCombiPath[index] = i;
						return ret;
					}
				} else {
					List < int > result = new List < int > ();
					result.Add(combis.IndexOf(c));
					fullCombiPath[index] = i;
					return result; //new Combi (c.Combination, combis.IndexOf (c));
				}
			}

			RevertListFromCombi(c);
			//if(prev != null)
			//RevertListFromCombi (prev, persons);
		}
		if (fullCombiPath != null)
			fullCombiPath[index] = 0;
		return null;
	}
	void IncrementCombiPath(List < int > FullCombiPath, List < List < Combi >> combinationSteps, long incr = 1, int combiStepIndex = -2) {
		if (combiStepIndex == -2) {
			combiStepIndex = combinationSteps.Count - 1;
		}
		//Debug.Log ("CombiStepIndex: "+combiStepIndex);
		if (combiStepIndex < 0 || FullCombiPath == null)
			return;
		//Debug.Log ("Did not return ");
		//Debug.Log ("FCP: "+FullCombiPath.Count);
		//Debug.Log ("FCP: "+FullCombiPath [combiStepIndex]);

		long num = FullCombiPath[combiStepIndex] + incr;
		//Debug.Log ("num: " + num);
		int maxCombiStep = combinationSteps[combiStepIndex].Count;
		// Debug.Log ("maxCombiStep: " + maxCombiStep);
		//if (maxCombiStep >= num) {
		FullCombiPath[combiStepIndex] = (int)(num % maxCombiStep);
		// Debug.Log ("maxCombiStep: " + FullCombiPath[combiStepIndex]);
		IncrementCombiPath(FullCombiPath, combinationSteps, num / maxCombiStep, combiStepIndex - 1);
		// Debug.Log ("RecursiveShit" + maxCombiStep);

		//}
	}
	public CombiExplorer(List < int > startingCombiPath, List < List < Combi >> combinationSteps, List < TempPerson > persons, long pathOffset, long combiQuota) {
		//Debug.Log ("New Explorer");
		if (startingCombiPath != null && startingCombiPath.Count == combinationSteps.Count)
			fullCombiPath = new List < int > (startingCombiPath);
		else {
			fullCombiPath = new List < int > (combinationSteps.Count);
			int diff = 0;
			if (fullCombiPath != null && startingCombiPath != null)
				diff = fullCombiPath.Count - startingCombiPath.Count;

			for (int i = 0; i < combinationSteps.Count; ++i) {
				fullCombiPath.Add(0);
			}
		}
		//Debug.Log ("New List");
		IncrementCombiPath(fullCombiPath, combinationSteps, pathOffset);
		//Debug.Log ("Incr Path to offset");
		foreach(TempPerson tp in persons) {
			List < int > newList = new List < int > ();
			assignedPersons.Add(tp.PersonData, newList);
			foreach(TempStick ts in tp.ListOfAssginedSticks) {
				assignedPersons[tp.PersonData].Add(ts.order);
			}
		}
		//Debug.Log ("Assigned peeps");

		pathsQuota = combiQuota;
	}
}
public class CombinationAssign : MonoBehaviour
{
	List<TempPerson> Persons;
	[SerializeField]
	List<TempStick> Sticks;
	//[SerializeField]
	List<Combi> Combinations;
	int Progress = 0;
	int RealProgressStep = 0;
	int ProgressStepSize = 0;
	bool stop = true;
	Job myJob;
	StringBuilder debuglog = new StringBuilder();
	List<List<int>> ListOfCombinations = new List<List<int>>();
	public List<Emplacement> Emplacements;
	public List<Batch> Batches;

	void Start()
	{
		ProgressStepSize = (int)Mathf.Pow(2, 31);
		myJob = new Job();
		myJob.InData = new Vector3[10];

		Batches = GetComponent<Base> ().Batches;
		Emplacements = GetComponent<Base> ().Emplacements;
	}
	public int OrderIndex(int order){
		for (int i = 0; i < Sticks.Count; ++i) {
			if (Sticks [i].order == order)
				return i;
		}
		return -1;
	}
	public int startOrder = 0, endOrder = 2, sampleSize = 3;
	public string CurrCombiPath = "";
	List<List<Combi>> CombinationSteps = new List<List<Combi>>();
	void Update()
	{
		if (myJob != null)
		{
			if (myJob.Update())
			{ // Alternative to the OnFinished callback

				myJob = null;
			}
		}
		if (stop == true && Input.GetKeyDown(KeyCode.Y))
		{
			Task t = new Task (() => {
				Debug.Log("Start Calc");
				Combinations = new List<Combi>();
				debug = new StringBuilder();
				stop = false;
				int combIndex = 0;
				List<TempPerson> tp = Persons = new List<TempPerson> ();
				Sticks = new List<TempStick>();
				int depth = 0, sOrder = 0, eOrder =0;
				do{
					sOrder = startOrder + combIndex * sampleSize;
					eOrder = startOrder + (combIndex+1) * sampleSize;
					PrepareAACR(tp,Sticks, sOrder,eOrder);
					Combinations = CombinationSteps[combIndex];
					AssignAndCheckR(tp, OrderIndex(sOrder), sOrder,eOrder);
					//depth = Sticks.IndexOf(Combinations[0].Combination[Combinations[0].Combination.Count-1])+1;
				}while(++combIndex < CombinationSteps.Count);
				//Combinations = CombinationSteps[0];
				Debug.Log ("Thread Done!");
				//System.IO.File.WriteAllText(Application.dataPath + "/" + "debug.txt", debug);
				stop = true;
			});
			t.Start ();
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			//Combi.ReadCombiPath (CurrCombiPath,fullCombiPath);
			Debug.Log(CombinationSteps.Count);
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			Debug.Log (ListOfCombinations.Count);
			foreach(List<int> combi in ListOfCombinations)
			{
				string fullStringPath = "";
				foreach (int val in combi) 
				{
					fullStringPath += (val + " ");
				}
				Debug.Log (fullStringPath);
			}
//			for (int i = 0; i < 8; i++) 
//			{
//				List<int> vals = ListOfCombinations[UnityEngine.Random.Range(0,ListOfCombinations.Count-1)];
////				CombinationSteps[i][vals[i]]
//			}

//			foreach (List<Combi> combiStep in CombinationSteps) 
//			{
//				combiStep[0].
//			}
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			if (Combinations != null) {
				//StringBuilder sb = new StringBuilder ();
				//sb.Append ("No Of Combinations: " + Combinations.Count + "\n");
//				Debug.Log (Combinations.Count);
//				Debug.Log ("Persons: ");
//				for(int i=0;i<Persons.Count;++i)
//				Debug.Log (Persons[i].ListOfAssginedSticks.Count);
//				Debug.Log (": End ");
				//Debug.Break ();

				long TotalCombiSteps = 1;
				int threadCount = 20;
				int counter = 0;

				foreach (List<Combi> combiStep in CombinationSteps) 
				{
					TotalCombiSteps *= combiStep.Count;
				}
				Debug.Log ("Total Steps: " + TotalCombiSteps);
				Debug.Log ("Quota Per step: " + TotalCombiSteps / (threadCount - 1));

				ParallelOptions po = new ParallelOptions ();
				po.TaskScheduler = UnityTaskScheduler.Default;
				po.MaxDegreeOfParallelism = 2;

				Parallel.For (0, threadCount, (f) => {
					Task t = new Task (() => {
						int ThreadNo = -1;
						int LoopAmount = 3;
						long threadQuota = -1;

						ThreadNo = f;

						// Previous 
						threadQuota = 1000;

						// Current
//						threadQuota = TotalCombiSteps / (threadCount - 1);
//						if(ThreadNo == threadCount -1)
//							threadQuota = TotalCombiSteps % (threadCount - 1);

						//IncrementCombiPath (fullCombiPath, CombinationSteps,threadQuota*ThreadNo);
						for(int i = 1; i < LoopAmount; i++)
						{
							CombiExplorer combiExplorer = new CombiExplorer(fullCombiPath,CombinationSteps,Persons,threadQuota*ThreadNo + i,threadQuota);
							Debug.Log (f + ": Done First Func (" + i + ") times");
							List<int> showCombi = combiExplorer.GetFullCombi (CombinationSteps);
							Debug.Log (f + ": Done Second Func (" + i + ") times");
							lock (ListOfCombinations) {
								ListOfCombinations.Add (showCombi);
								Debug.Log (f + ": Added Combination " + ThreadNo);
							}
						}
						Debug.Log (f + ": Done with thread " + ThreadNo);
					});
					t.Start ();
				});

				//IncrementCombiPath (fullCombiPath, CombinationSteps);
				//Combi showCombi = GetFullCombi (Persons, CombinationSteps);//Combinations[0];
				//fullCombiPath = showCombi.combiIndex;
				//CurrCombiPath = showCombi.CombiPath();
				foreach (Emplacement emp in GetComponent<Base>().Emplacements)
				{
					emp.Reset();
				}
//				foreach (TempStick s in showCombi.Combination)
//				{
//					if(s.IsAssigned())
//						s.StickData.AssignPerson(s.PersonData);
//				}
				string order = "";
				int index = 0;
				/*foreach (Combi comb in Combinations)
                { 
                    //order += index++ + "|\n";
                    sb.Append(index++ + "|\n");
                    foreach (TempStick s in comb.Combination)
                    {
                        if(s.order >= 0 && s.order < 3)
                        {
                            sb.Append(s.StickData.TimeStart + " - " + s.StickData.Parent.NameOfEmplacement + " - " + s.PersonData.Name + "\n"); //order += s.StickData.TimeStart + " - " + s.StickData.Parent.NameOfEmplacement + " - " + s.PersonData.Name + "\n";
                        }
                    }
                    sb.Append("|\n"); 
                    //order += "|\n";
                } */
				//Debug.Log(order);
				//System.IO.File.WriteAllText(Application.dataPath + "/" + "order.txt", sb.ToString());
				order = "";
			}
		}
	}
	public void UpdateListFromCombi(Combi c, List<TempPerson> persons){
		foreach (TempStick ts in c.Combination) {
			foreach (TempPerson tp in persons) {
				if (tp.PersonData == ts.PersonData)
					tp.Assign (ts);
			}
		}
		foreach (TempPerson tp in persons) {
			tp.ListOfAssginedSticks.Sort (new TStickOrderComparer ());
		}
	}
	public void RevertListFromCombi(Combi c, List<TempPerson> persons){
		foreach (TempStick ts in c.Combination) {
			foreach (TempPerson tp in persons) {
				if (tp.PersonData == ts.PersonData)
					tp.Unassign (ts);
			}
		}
	}

	//Check if the list of temppersons is still properly rested.
	public bool ValidatePersons(List<TempPerson> persons){
		//Cycle through all the persons and check if they have enough rest.
		foreach (TempPerson p in persons) {
			if (p.ListOfAssginedSticks.Count > p.NoOfSticks) {
				return false;
			}
		}
		//Can skip all these because of the additional check when doing CanConnect.
		foreach (TempPerson p in persons) {
			int span = 0;
			int prevOrder = 0;
			//p.ListOfAssginedSticks.Sort (new TStickOrderComparer ());
			if (p.ListOfAssginedSticks.Count > 3 && p.PersonData.Name == "XZ 12" && p.ListOfAssginedSticks[3].order == 8)
				Debug.Log ("Previous stuffs are accounted for");
			foreach (TempStick ts in p.ListOfAssginedSticks) {
				//Not connected to the previous stick

				if (ts.order > 1 + prevOrder) {
					span = 0;
					if (ts.order > 3 || prevOrder > 3) {
						Debug.Log (ts.order + " " + prevOrder);
						if (!((ts.order - prevOrder) > StaticVars.RestAfterSticks)) {
							Debug.Log ("Return false");
						}
					}
					if ( ! ((ts.order - prevOrder) > StaticVars.RestAfterSticks)) {
						return false;//Not enough rest
					}
				} else {
					//Connected to the previous stick.
					if (++span > StaticVars.MaxNoOfDutyInARow)
						return false;//Too many sticks connected
				}
				prevOrder = ts.order;
				/*
				if ( ((ts.order - prevOrder) <= StaticVars.RestAfterSticks)) {
					Debug.Log ("IDK WHY MAN");
					return false;//Not enough rest
				}*/
			}	
		}
		return true;
	}
	void IncrementCombiPath(List<int> FullCombiPath,List<List<Combi>> combinationSteps, int incr=1,int combiStepIndex = -2){
		if (combiStepIndex == -2) {
			combiStepIndex = combinationSteps.Count-1;
		}

		if(combiStepIndex <0 || FullCombiPath ==null)
			return;
		int num = FullCombiPath [combiStepIndex] + incr;
		int maxCombiStep = combinationSteps [combiStepIndex].Count ;
		if (maxCombiStep >= num) {
			FullCombiPath[combiStepIndex] = num % maxCombiStep  ;
			IncrementCombiPath(FullCombiPath,combinationSteps,num/maxCombiStep,combiStepIndex -1);
		}
	}
	List<int> fullCombiPath;
	//To generate the final combi.
	public Combi GetFullCombi(List<TempPerson> persons, List<List<Combi>> combiSteps, Combi prev = null, int index =0){
		List<Combi> combis = combiSteps [index];

		//if(prev != null)
		//	UpdateListFromCombi (prev, persons);
		int i=0;
		if (fullCombiPath != null && index < fullCombiPath.Count)
			i = fullCombiPath [index];
		for (; i< combis.Count;++i) {
			Combi c = combis [i];
			UpdateListFromCombi (c, persons);
			if ( (prev == null || prev.CanConnect(c)) &&  
				ValidatePersons (persons)) {
				if (index + 1 < combiSteps.Count) {
					Combi ret = GetFullCombi (persons, combiSteps,c, index + 1);
					if (ret != null) {
						return ret.AddToCombi (c,combis.IndexOf(c));
					}
				} else {
					return new Combi (c.Combination, combis.IndexOf (c));
				}
			}

			RevertListFromCombi (c, persons);
			//if(prev != null)
			//RevertListFromCombi (prev, persons);
		}
		if(fullCombiPath != null)
			fullCombiPath [index] = 0;
		return null;
	}
	//public List<TempStick> possibleSticks = new List<TempStick>(), allPossibleSticks = new List<TempStick>();
	List<TempPerson> PrepareAACR(List<TempPerson> c, List<TempStick> s, int StartOrder, int EndOrder)
	{
		if (CombinationSteps.Count == 0) {
			List<Batch> AllBatches = Batches;
			foreach (Batch b in AllBatches) {
				foreach (Person p in b.ListOfPeople) {
					c.Add (new TempPerson (p));
				}
			}
			List<Emplacement> AllEmp = Emplacements;
			int curOrder = 0;
			foreach (Emplacement emp in AllEmp) {
				if (!emp.IsSpecialRole ()) {
					foreach (Stick stick in emp.ListOfSticks) {
						if (stick.State == StickState.ENABLED) {
							TempStick tick = new TempStick (stick);
							s.Add (tick);
							if (tick.order >= curOrder) {
								CombinationSteps.Add (new List<Combi> ());
								curOrder += sampleSize;
							}
							/*	if (tick.order >= StartOrder && tick.order < EndOrder)
							allPossibleSticks.Add(tick);*/
						}
					}
				}
			}
			s.Sort (new TStickOrderComparer ());
		}
		foreach (TempPerson p in c)
		{
			p.ListOfPossibleSticks.Clear ();
			//p.UnassignAll ();
			foreach (TempStick tick in s)
			{
				if(tick.order >= StartOrder && tick.order < EndOrder)
				{
					if (CanDo(p, tick)) 
					{
						p.ListOfPossibleSticks.Add(tick);
						/*if(!possibleSticks.Contains(tick))
							possibleSticks.Add (tick);*/
					}
				}
			}
		}
		/*string debuglog = "";
        foreach (TempPerson p in c)
        {
            debuglog += (p.PersonData.Name + " - " + p.ListOfPossibleSticks.Count + " - " + p.NoOfSticks + "\n");
            foreach (TempStick st in p.ListOfPossibleSticks)
            {
                debuglog += ("|" + st.StickData.TimeStart + " - " + st.StickData.Parent.NameOfEmplacement + "|\n");
            }
            debuglog += "\n";
        }
        Debug.Log(debuglog);*/
		//Sticks = s;
		return c;
	}
	StringBuilder debug = new StringBuilder();
	int count = 0;
	bool AssignAndCheckR(List<TempPerson> C, StringBuilder trace, int depth = 0, int StartOrder = -1, int EndOrder = -1)
	{
		for (int i = 0; i < C.Count; i++)
		{
			TempPerson c = C[i];
			if (c.IsAssignable())
			{
				string msg = ("\tDepth: " + depth + ", i: " + i + " ," + c.PersonData.Name + ", ListOfPossibleSticks.Count: " + c.ListOfPossibleSticks.Count + " | ");
				debug.Append(trace.ToString() + msg);
				debug.Append(Environment.NewLine);
				TempStick s = Sticks[depth];
				if (!c.ListOfPossibleSticks.Contains(s))
				{
					continue;
				}
				if (stop)
				{
					throw new UnityException("Force stopped");
				}
				Progress++;
				if (++Progress >= ProgressStepSize)
				{
					Progress = 0;
					RealProgressStep++;
				}
				debug.Append("\t|" + !s.IsAssigned() + " - " + CanDo(c, s));
				debug.Append(Environment.NewLine);
				if (!s.IsAssigned() && CanDo(c, s))
				{
					debug.Append("\t| (Assigned) " + c.PersonData.Name + " | " + s.StickData.TimeStart.ToShortTimeString() + " | " + s.StickData.Parent.NameOfEmplacement + " | ");
					debug.Append(Environment.NewLine);
					c.Assign(s);
					s.SetAssigned(c);
					if (AllAssigned(StartOrder,EndOrder))
					{
						debug.Append("\t| (Combination Generated) ");
						foreach (TempStick v in Sticks)
						{
							if(v.order >= StartOrder && v.order < EndOrder)
							{
								debug.Append("|" + v.PersonData.Name + " - " + v.StickData.TimeStart + " - " + v.StickData.Parent.NameOfEmplacement + "| ");
							}
						}
						debug.Append(Environment.NewLine);
						Combinations.Add(new Combi(Sticks));
						c.Unassign(s);
						s.SetUnassigned(); // return true;

						continue;
					}
					//StringBuilder tempbuilder = new StringBuilder(debug.ToString() + msg);
					System.IO.File.WriteAllText(Application.dataPath + "/" + "debug2.txt", debug.ToString());
					AssignAndCheckR(C, debug, depth + 1);
					c.Unassign(s);
					s.SetUnassigned();
				}
			}
		}
		return false;
	}
	bool AssignAndCheckR(List<TempPerson> C, int depth, int StartOrder = -1, int EndOrder = -1)
	{
		if (depth >= Sticks.Count) return false;
		for (int i = 0; i < C.Count; i++)
		{
			TempPerson c = C[i];
			if (c.IsAssignable())
			{
				TempStick s = Sticks[depth];
				if (!c.ListOfPossibleSticks.Contains(s))
				{
					continue;
				}
				if (stop)
				{
					throw new UnityException("Force stopped");
				}
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
					if (AllAssigned(StartOrder,EndOrder))
					{ //System.IO.File.WriteAllText(Application.dataPath + "/" + "debug3.txt", debuglog.ToString());

						Combinations.Add(new Combi(Sticks));
						c.Unassign(s);
						s.SetUnassigned(); //throw new UnityException ("First combination Generated!");

						continue;
					}
					if( depth+1 < Sticks.Count && !(( Sticks[depth+1].order) > EndOrder) )
						AssignAndCheckR(C, depth + 1, StartOrder, EndOrder);
					c.Unassign(s);
					s.SetUnassigned();
				}
			}
		}
		return false;
	}
	public bool CanDo(TempPerson p, TempStick s)
	{
		if (p.PersonData.ListOfRoles.Contains(s.StickData.Parent.CurrentRole) && p.ListOfAssginedSticks.Count < p.NoOfSticks)
		{
			if (p.PersonData.IsRested(s.StickData.TimeStart, s.StickData.TimeEnd) || CheckContiguous(s, p))
			{
				return true;
			}
		}
		return false;
	}
	public bool AllAssigned(int StartOrder, int EndOrder)
	{
		foreach (TempStick s in Sticks)
		{
			if(s.order >= StartOrder && s.order < EndOrder)
			{
				if (!s.IsAssigned())
				{
					return false;
				}
			}
		}
		return true;
	}
	public bool CheckContiguous(TempStick s, TempPerson p)
	{
		List<TempStick> contiguousSticks = new List<TempStick>();
		foreach (TempStick assignedStick in p.ListOfAssginedSticks)
		{
			if (IsConnected(assignedStick, s))
			{
				contiguousSticks.Add(assignedStick);
			}
			else if (!SingleIsRested(assignedStick, s))
			{ //return false;
			}
		}
		if (contiguousSticks.Count < 2)
		{
			if (contiguousSticks.Count == 0)
			{
				return false;
			}
			foreach (TempStick tempAssignedStick in p.ListOfAssginedSticks)
			{
				if (IsConnected(tempAssignedStick, contiguousSticks[0]))
				{
					return false;
				}
			}
			return true;
		}
		else
		{
			return false;
		}
	}
	public bool IsConnected(TempStick s1, TempStick s2)
	{
		if (s1.StickData.Parent == s2.StickData.Parent)
		{
			TimeSpan ts = s1.StickData.TimeEnd - s2.StickData.TimeStart;
			if (ts.TotalHours > 0)
			{
				ts = s2.StickData.TimeEnd - s1.StickData.TimeStart;
			}
			if (Mathf.FloorToInt((float)ts.TotalHours) == 0)
			{
				return true;
			}
		}
		return false;
	}
	public bool SingleIsRested(TempStick s1, TempStick s2)
	{
		int HoursDiff = 0;
		int FirstStartDiff = (int)(s1.StickData.TimeStart - s2.StickData.TimeStart).TotalHours; // The new stick starts AFTER the old stick starts.
		//Debug.Log ("0: " + StickStartTiming.ToString() + " - " + lastStick.TimeEnd.ToString());

		if (FirstStartDiff > 0)
		{ // The new stick is starting after the old stick starts. 

			int EndDiff = (int)(s1.StickData.TimeStart - s2.StickData.TimeEnd).TotalHours; //Debug.Log ("1: " + EndDiff + " - " + StickStartTiming.ToString() + " - " + lastStick.TimeEnd.ToString());

			if (EndDiff > 0)
			{ // This means the old stick, ends before your new stick starts. 

				HoursDiff = Mathf.Abs(EndDiff);
			}
		}
		else if (FirstStartDiff < 0)
		{
			int EndDiff = (int)(s1.StickData.TimeEnd - s2.StickData.TimeStart).TotalHours; //Debug.Log ("2: " + EndDiff + " - " + StickStartTiming.ToString() + lastStick.TimeEnd.ToString());

			if (EndDiff < 0)
			{ // This means my new sticks, ends before my old stick starts.

				HoursDiff = Mathf.Abs(EndDiff);
			}
		}
		if (HoursDiff < StaticVars.RestAfterSticks * StaticVars.StickInHours)
		{ //debuglog.AppendLine ("(" + s1.StickData.Parent.NameOfEmplacement + ")(" + s2.StickData.Parent.NameOfEmplacement + ") HoursDiff: " + HoursDiff + " | "); 
			//debuglog.AppendLine ("s1: " + s1.StickData.TimeStart + " - " + s1.StickData.TimeEnd + " | ");
			//debuglog.AppendLine ("s2: " + s2.StickData.TimeStart + " - " + s2.StickData.TimeEnd + " | ");

			return false;
		}
		HoursDiff = 0;
		return true;
	}
}