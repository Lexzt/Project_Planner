using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class NamePanel : MonoBehaviour {

 	private static NamePanel m_instance;
	public GameObject AutoCompleteBox;
	public Stick SelectedStick;
 
    public static NamePanel Instance
    {
        get
        {
            return m_instance;
        }
    }
 	void Awake()
    {
        m_instance = this;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			AutoCompleteBox.SetActive(false);
		}
	}
	public void OnItemClicked(string nameOfSelected)
	{
		Debug.Log(nameOfSelected);
		foreach(Batch batchData in GetComponent<Base>().Batches)
		{
			foreach(Person personel in batchData.ListOfPeople)
			{
				if(nameOfSelected == personel.Name)
				{
					SelectedStick.AssignPersonWithoutChecks(personel);
					SelectedStick = null;
					AutoCompleteBox.SetActive(false);
					break;
				}
			}
		}
	}

	public void FillUpList (List<string> ListOfChoices)
	{
		AutoCompleteBox.transform.FindChild("Name Panel").GetComponent<AutoCompleteComboBox>().Initialize();
		AutoCompleteBox.transform.FindChild("Name Panel").GetComponent<AutoCompleteComboBox>().SetItemsAndRedraw(ListOfChoices);
	}
}
