using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combi
{
    List<TempStick> Combination;
    int EfficiencyIndex;
    Combi(List<TempStick> Combinations) {Combination = Combinations;}
}

public class TempStick
{
    Stick StickData;
    Person PersonData;
}

public class CombinationAssign : MonoBehaviour {

    List<TempStick> Sticks;
    List<Combi> Combinations;

    int Progress = 0;
    int RealProgressStep = 0;
    int ProgressStepSize = 2^31;
    Job myJob;

    void Start()
    {
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
    }

    // void PrepareAACR()
    // {
    //     List<Person> c = new List<Person>();
    //     List<TempStick> s = new List<TempStick>();

    //     List<Batch> AllBatches = GetComponent<Base>().Batches;
    //     foreach(Batch b in AllBatches)
    //     {
    //         foreach(Person p in b.ListOfPeople)
    //         {
    //             c.Add(p);
    //         }
    //     }

    //     List<Emplacement> AllEmp = GetComponent<Base>().Emplacements;
    //     foreach(Person p in c)
    //     {
    //         foreach (TempStick tick in s)
    //         {
    //             if (c.CanDo(s))
    //                 c.AddToPossibleSticks(s);
    //         }
    //     }
    // }

    // bool AssignAndCheckR(Person C)
    // {
    //     foreach (Person c in C)
    //     {
    //         if (c.IsAssignable())
    //         {
    //             foreach (Stick s in c.possibleSticks)
    //             {
    //                 if (Combinations.Count > CombiLimit || stop)
    //                 {
    //                     return false;
    //                 }
    //                 Progress++;
    //                 if (++Progress >= ProgressStepSize)
    //                 {
    //                     Progress = 0;
    //                     RealProgressStep++;
    //                 }
    //                 if (!s.IsAssigned() && c.CanBeAssigned(s))
    //                 {
    //                     c.Assign(s);
    //                     s.SetAssigned();

    //                     if (AllAssigned())
    //                     {
    //                         Combinations.Add(new Combi(sticks));
    //                         return true;
    //                     }
    //                     AssignAndCheckR(C);
    //                 }
    //             }
    //         }
    //         return false;
    //     }
    // }
}
