using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combi
{
	public List<TempStick> Combination = new List<TempStick> ();
    public int EfficiencyIndex;
    public Combi(List<TempStick> Combinations) 
	{
		foreach (TempStick s in Combinations) 
		{
			Combination.Add (new TempStick(s));
		}
	}
}

public class TempPerson
{
	public List<TempStick> ListOfPossibleSticks = new List<TempStick>();
	public List<TempStick> ListOfAssginedSticks = new List<TempStick>();
	public Person PersonData;
	public TempPerson (TempPerson objToCopy)
	{
		ListOfPossibleSticks = objToCopy.ListOfPossibleSticks;
		ListOfAssginedSticks = objToCopy.ListOfAssginedSticks;
	}
	public TempPerson(Person p) 
	{
		PersonData = p;
	}
	public void Assign (TempStick s)
	{
		PersonData.ListOfSticks.Add (s.StickData);
		ListOfAssginedSticks.Add (s);
	}
	public void Unassign(TempStick s)
	{
		PersonData.ListOfSticks.Remove (s.StickData);
		ListOfAssginedSticks.Remove (s);
	}
	public bool IsAssignable()
	{
		if (PersonData.NoOfSticks > 0)
			return true;
		else
			return false;
	}
}

public class TempStick
{
    public Stick StickData;
    public Person PersonData;
	public TempStick(TempStick objToCpy)
	{
		StickData = objToCpy.StickData;
		PersonData = objToCpy.PersonData;
	}
	public TempStick(Stick s)
	{
		StickData = s;
	}
	public bool IsAssigned()
	{
		return PersonData != null;
	}
	public void SetAssigned (TempPerson p)
	{
		PersonData = p.PersonData;
	}
	public void SetUnassigned()
	{
		PersonData = null;
	}
}

public class CombinationAssign : MonoBehaviour {

    List<TempStick> Sticks;
	List<Combi> Combinations;

    int Progress = 0;
    int RealProgressStep = 0;
    int ProgressStepSize = 0;
	bool stop = true;
    Job myJob;

    void Start()
    {
		ProgressStepSize = (int)Mathf.Pow (2, 31);

        myJob = new Job();
        myJob.InData = new Vector3[10];
        myJob.Start(); // Don't touch any data in the job class after you called Start until IsDone is true.
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

		if (stop == true && Input.GetKeyDown (KeyCode.Y)) 
		{
			Combinations = new List<Combi> ();
			debug = "";
			stop = false;
			List<TempPerson> tp = PrepareAACR ();
			AssignAndCheckR (tp);
			Debug.Log (debug);
			stop = true;

		}

		if (Input.GetKeyDown (KeyCode.T)) 
		{
			if(Combinations != null)
			{
				Debug.Log (Combinations.Count);
				foreach (Emplacement emp in GetComponent<Base>().Emplacements) 
				{
					emp.Reset ();
				}

//				foreach (TempStick s in Combinations[Random.Range(0,Combinations.Count-1)].Combination) 
//				{
//					s.StickData.AssignPerson (s.PersonData);
//				}
				string order = "";
				int index = 0;
				foreach (Combi comb in Combinations) 
				{
					order += index++ + "|\n";
					foreach (TempStick s in comb.Combination) 
					{
						order += s.StickData.TimeStart + " - " + s.StickData.Parent.NameOfEmplacement + " - " + s.PersonData.Name + "\n";
					}
					order += "|\n";
				}
				Debug.Log (order);

				order = "";
			}
		}
    }

	List<TempPerson> PrepareAACR()
     {
		List<TempPerson> c = new List<TempPerson>();
        List<TempStick> s = new List<TempStick>();

        List<Batch> AllBatches = GetComponent<Base>().Batches;
        foreach(Batch b in AllBatches)
        {
            foreach(Person p in b.ListOfPeople)
            {
				c.Add(new TempPerson(p));
            }
        }

        List<Emplacement> AllEmp = GetComponent<Base>().Emplacements;
		foreach (Emplacement emp in AllEmp) 
		{
			if(!emp.IsSpecialRole())
			{
				foreach (Stick stick in emp.ListOfSticks) 
				{
					if (stick.State == StickState.ENABLED) 
					{
						s.Add (new TempStick (stick));
					}
				}
			}
		}
		foreach(TempPerson p in c)
        {
            foreach (TempStick tick in s)
            {
				if (CanDo(p.PersonData,tick.StickData))
					p.ListOfPossibleSticks.Add(tick);
            }
        }

		string debuglog = "\n";
		foreach (TempPerson p in c) 
		{
			debuglog += (p.PersonData.Name + " - " + p.ListOfPossibleSticks.Count + "\n");
			foreach(TempStick st in p.ListOfPossibleSticks)
			{
				debuglog += ("|" + st.StickData.TimeStart + " - " + st.StickData.Parent.NameOfEmplacement + "|\n");
			}
			debuglog += "\n";
		}
		Debug.Log (debuglog);

		Sticks = s;
		return c;
    }
	string debug = "";
	bool AssignAndCheckR(List<TempPerson> C,string trace = "",int depth = 0)
    {
		//foreach (TempPerson c in C)
		// for (int i = 0; i < C.Count; i++)
        // {
        //     TempPerson c = C[i];
        //     if (c.IsAssignable())
        //     {
        //         //foreach (TempStick s in c.ListOfPossibleSticks)
        //         for (int j = 0; j < c.ListOfPossibleSticks.Count; j++)
        //         {
        //             string msg = ("\tDepth: " + depth + ", i: " + i + " ," + c.PersonData.Name + ", j: " + j + " | ");
        //             debug += trace + msg + "\n";
        //             TempStick s = c.ListOfPossibleSticks[j];
        //             if (stop)
        //             {
        //                 throw new UnityException("Force stopped");
        //             }
        //             Progress++;
        //             if (++Progress >= ProgressStepSize)
        //             {
        //                 Progress = 0;
        //                 RealProgressStep++;
        //             }
        //             debug += ("\t|" + !s.IsAssigned() + " - " + CanDo(c.PersonData, s.StickData) + "|\n");
        //             if (!s.IsAssigned() && CanDo(c.PersonData, s.StickData))
        //             {
        //                 debug += ("\t| (Assigned) " + c.PersonData.Name + " | " + s.StickData.TimeStart.ToShortTimeString() + " | " + s.StickData.Parent.NameOfEmplacement + " | \n");
        //                 c.Assign(s);
        //                 s.SetAssigned(c);

        //                 //						if (s.StickData.Unique == false) 
        //                 //						{
        //                 //							c.Assign(c.ListOfPossibleSticks [j + 1]);
        //                 //							c.ListOfPossibleSticks [j + 1].SetAssigned(c);
        //                 //						}

        //                 if (AllAssigned())
        //                 {
        //                     debug += ("\t| (Combination Generated) ");
        //                     foreach (TempStick v in Sticks)
        //                     {
        //                         debug += ("|" + v.PersonData.Name + " - " + v.StickData.TimeStart + " - " + v.StickData.Parent.NameOfEmplacement + "| ");
        //                     }
        //                     debug += "\n";
        //                     Combinations.Add(new Combi(Sticks));
        //                     // c.Unassign (s);
        //                     // s.SetUnassigned ();
        //                     return true;
        //                 }

        //                 AssignAndCheckR(C, trace + msg, depth + 1);
        //                 c.Unassign(s);
        //                 s.SetUnassigned();
        //             }
        //         }
        //     }
        // }
        // return false;

 		List<TempStick> allsticks = new List<TempStick>();
        List<Emplacement> AllEmp = GetComponent<Base>().Emplacements;
		foreach (Emplacement emp in AllEmp) 
		{
			if(!emp.IsSpecialRole())
			{
				foreach (Stick stick in emp.ListOfSticks) 
				{
					if (stick.State == StickState.ENABLED) 
					{
						allsticks.Add (new TempStick (stick));
					}
				}
			}
		}

		for(int j = 0; j < allsticks.Count; j++)
		{
			TempStick s = allsticks[j];
			for (int i = 0; i < C.Count; i++)
        	{
           		TempPerson c = C[i];
                if (c.IsAssignable())
                {
                    string msg = ("\tDepth: " + depth + ", i: " + i + " ," + c.PersonData.Name + ", j: " + j + " | ");
                    debug += trace + msg + "\n";
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
                    debug += ("\t|" + !s.IsAssigned() + " - " + CanDo(c.PersonData, s.StickData) + "|\n");
                    if (!s.IsAssigned() && CanDo(c.PersonData, s.StickData))
                    {
                        debug += ("\t| (Assigned) " + c.PersonData.Name + " | " + s.StickData.TimeStart.ToShortTimeString() + " | " + s.StickData.Parent.NameOfEmplacement + " | \n");
                        c.Assign(s);
                        s.SetAssigned(c);

                        if (AllAssigned())
                        {
                            debug += ("\t| (Combination Generated) ");
                            foreach (TempStick v in Sticks)
                            {
                                debug += ("|" + v.PersonData.Name + " - " + v.StickData.TimeStart + " - " + v.StickData.Parent.NameOfEmplacement + "| ");
                            }
                            debug += "\n";
                            Combinations.Add(new Combi(Sticks));
                            // c.Unassign (s);
                            // s.SetUnassigned ();
                            return true;
                        }

                        AssignAndCheckR(C, trace + msg, depth + 1);
                        c.Unassign(s);
                        s.SetUnassigned();
                    }
                }
			}
		}
		return false;
     }


	public bool CanDo(Person p, Stick s)
	{
		if (p.IsRested (s.TimeStart, s.TimeEnd) &&
		   	p.ListOfRoles.Contains (s.Parent.CurrentRole) &&
			p.NoOfSticks > 0) 
		{
			return true;
		}
		return false;
	}

	public bool AllAssigned ()
	{
		foreach (TempStick s in Sticks) 
		{
			if (!s.IsAssigned ()) 
			{
				return false;
			}
		}
		return true;
	}
}
