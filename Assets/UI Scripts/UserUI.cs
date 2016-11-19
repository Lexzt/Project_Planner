using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserUI : MonoBehaviour {

	public Person MainData;
	public GameObject RemoveButton;

	void Start () 
	{
	
	}
	
	void Update () 
	{
	
	}

	public void UpdateUI (Person Data)
	{
		MainData = Data;
		transform.GetChild (0).GetComponent<Text> ().text = Data.Name;
		GetComponent<Button> ().onClick.AddListener (() => UpdateUserPanel (Data));
//		Debug.Log ("Clicked");
	}

	public void UpdateUserPanel(Person PersonData)
	{
//		if (UserManagementSystem.Instance ().UserValuesPanel.activeInHierarchy == false) 
//		{
			UserManagementSystem.Instance ().SelectedPersonel = MainData;
			UserManagementSystem.Instance ().DrawRoles (PersonData.ListOfRoles);
			UserManagementSystem.Instance ().AddUserPanel.SetActive (false);
			UserManagementSystem.Instance ().AddUserPanel.SetActive (false);
			UserManagementSystem.Instance ().EditBatchPanel.SetActive (false);

			GameObject UserPanel = UserManagementSystem.Instance ().EditUserPanel.transform.FindChild("UserDataPanel").gameObject;
			UserManagementSystem.Instance ().EditUserPanel.SetActive (true);
			UserPanel.transform.FindChild ("Name Panel").FindChild ("InputField").GetComponent<InputField> ().text = PersonData.Name;
			UserPanel.transform.FindChild ("IC Panel").FindChild ("InputField").GetComponent<InputField> ().text = PersonData.IC;
			string ListOfRoles = "";
			for(int i = 0; i < PersonData.ListOfRoles.Count; i++)
			{
				ListOfRoles += StaticVars.RolesParseJson (PersonData.ListOfRoles[i]);
				if(i + 1 != PersonData.ListOfRoles.Count)
					ListOfRoles += ",";
			}
			//UserPanel.transform.FindChild ("Current Roles Panel").FindChild ("Dropdown").FindChild("Label").GetComponent<Text> ().text = ListOfRoles;
//		}
//		else
//		{
//			UserManagementSystem.Instance ().UserValuesPanel.SetActive (false);
//		}
	}

	public void RemoveUser()
	{
		MainData.RemoveMe();
		Destroy(this.gameObject);
	}
}
