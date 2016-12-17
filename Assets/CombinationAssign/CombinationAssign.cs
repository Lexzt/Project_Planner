﻿using UnityEngine;
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
        PersonData.ListOfSticks.Add(s.StickData);
        ListOfAssginedSticks.Add(s);
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

[System.Serializable]
public class TempStick
{
	public string role;
    public Stick StickData;
    public Person PersonData;

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

public class CombinationAssign : MonoBehaviour
{
	[SerializeField]
    List<TempStick> Sticks;
	[SerializeField]
    List<Combi> Combinations;
    int Progress = 0;
    int RealProgressStep = 0;
    int ProgressStepSize = 0;
    bool stop = true;
    Job myJob;
    StringBuilder debuglog = new StringBuilder();

    void Start()
    {
        ProgressStepSize = (int)Mathf.Pow(2, 31);
        myJob = new Job();
        myJob.InData = new Vector3[10]; //myJob.Start(); // Don't touch any data in the job class after you called Start until IsDone is true.

    }
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
            Combinations = new List<Combi>();
            debug = new StringBuilder();
            stop = false;
            List<TempPerson> tp = PrepareAACR(0,3);
            StringBuilder sb = new StringBuilder();
            foreach (TempPerson p in tp)
            {
                sb.Append(p.PersonData.Name + "\n");
                foreach (TempStick s in p.ListOfPossibleSticks)
                {
                    sb.Append("\t" + s.StickData.TimeStart + " - " + s.StickData.Parent.NameOfEmplacement + "\n");
                }
            }
            System.IO.File.WriteAllText(Application.dataPath + "/" + "debug1.txt", sb.ToString());
            sb = new StringBuilder(); 
			//AssignAndCheckR(tp,sb,0, 0,3);

            AssignAndCheckR(tp, 0, 0, 3); 
			//Debug.Log(debug);
			//System.IO.File.WriteAllText(Application.dataPath + "/" + "debug.txt", debug);

            stop = true;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Combinations != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("No Of Combinations: " + Combinations.Count + "\n");
                Debug.Log(Combinations.Count);
                // foreach (Emplacement emp in GetComponent<Base>().Emplacements)
                // {
                //     emp.Reset();
                // }
                // foreach (TempStick s in Combinations[0].Combination)
                // {
                //     s.StickData.AssignPerson(s.PersonData);
                // }
                string order = "";
                int index = 0;
                foreach (Combi comb in Combinations)
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
                } 
                //Debug.Log(order);
                System.IO.File.WriteAllText(Application.dataPath + "/" + "order.txt", sb.ToString());
                order = "";
            }
        }
    }
    List<TempPerson> PrepareAACR(int StartOrder, int EndOrder)
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
				if(tick.order >= StartOrder && tick.order < EndOrder)
				{
					if (CanDo(p, tick)) 
					{
						p.ListOfPossibleSticks.Add(tick);
					}
				}
            }
        }
        string debuglog = "";
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
        Sticks = s;
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
    bool AssignAndCheckR(List<TempPerson> C, int depth = 0, int StartOrder = -1, int EndOrder = -1)
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