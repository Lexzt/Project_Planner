using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class UserManagementSystem : MonoBehaviour 
{
	[Header("[Prefabs]")]
	public GameObject BatchUIObject;
	public GameObject UserUIObject;
	public GameObject UserParentUIObject;
	public GameObject RolesObject;

	[Header("[Parents]")]
	public GameObject BatchUIParent;
	public GameObject UserUIParent;
	public GameObject CurrentRoleParent;
	public GameObject AddUserPanel;
	public GameObject EditUserPanel;

	[Header("[List Datas]")]
	public List<GameObject> ListOfBatchData = new List<GameObject>();
	private List<GameObject> ListOfUserParents = new List<GameObject>();

	[Header("[Buttons]")]
	public GameObject EditPersonelButton;
	public GameObject AddPersonelButton;
	public GameObject AddPersonelInGUIButton;

	[Header("[Runtime Values]")]
	public Batch SelectedBatch = null;
	public Person SelectedPersonel = null;

	private static UserManagementSystem mInstance = null;
	public static UserManagementSystem Instance()
	{
		return mInstance;
	}
	void Awake ()
	{
		if(!mInstance) mInstance = this;
	}

	void Start () 
	{
	
	}
	
	void Update () 
	{
//		if (Input.GetKeyDown (KeyCode.U)) 
//		{
//			Save ();
//		}
	}

	public void Save()
	{
		JSONData tempJson = new JSONData ("Hello World");
		JSONNode Parent = new JSONClass ();
		Parent["Name"] = "Keith";
		Debug.Log ("Parent: " + Parent.ToString() + " - " + Parent["Name"]);
		Debug.Log ("Parent: " + tempJson.Value);
	}

	public void AddBatchData (GameObject BatchObj)
	{
		ListOfBatchData.Add (BatchObj);
	}

	public void DrawUI ()
	{
		for (int j = 0; j < ListOfBatchData.Count; j++) 
		{
			// Draw the batch button first
			Batch BatchData = ListOfBatchData [j].GetComponent<Batch> ();
			GameObject BatchObj = Instantiate (BatchUIObject) as GameObject;
			BatchObj.transform.parent = BatchUIParent.transform;
			BatchObj.transform.localScale = Vector3.one;
			BatchObj.GetComponent<BatchUI> ().UpdateUI (BatchData);

			// Draw the user button here now.
			GameObject UserParent = Instantiate(UserParentUIObject) as GameObject;
			UserParent.transform.parent = UserUIParent.transform;
			UserParent.transform.localScale = Vector3.one;
			UserParent.transform.localPosition = Vector3.zero;
			UserParent.name = BatchData.name;
			foreach (Person people in BatchData.ListOfPeople) 
			{
				GameObject UserObject = Instantiate (UserUIObject) as GameObject;
				UserObject.transform.parent = UserParent.transform;
				UserObject.transform.localScale = Vector3.one;
				UserObject.name = people.Name;
				UserObject.GetComponent<UserUI> ().UpdateUI(people);
			}
			ListOfUserParents.Add (UserParent);
			UserParent.SetActive (false);

			BatchObj.GetComponent<BatchUI> ().UserParentObject = UserParent;
			BatchObj.GetComponent<Button> ().onClick.AddListener(() => DisableOtherUserParent());
			BatchObj.GetComponent<Button> ().onClick.AddListener(() => BatchObj.GetComponent<BatchUI> ().SwapUserParent ());
			BatchObj.GetComponent<Button> ().onClick.AddListener(() => {AddPersonelButton.SetActive (true);});
		}
	}

	public void DisableOtherUserParent ()
	{
		foreach (GameObject Obj in ListOfUserParents) 
		{
			Obj.SetActive (false);
		}
	}

	public void AddPersonel ()
	{
		AddUserPanel.SetActive (true);
		AddPersonelButton.SetActive (true);
		EditUserPanel.SetActive (false);

		AddUserPanel.transform.FindChild("AddDataPanel").FindChild ("Name Panel").FindChild ("InputField").GetComponent<InputField> ().text = "";
		AddUserPanel.transform.FindChild("AddDataPanel").FindChild ("IC Panel").FindChild ("InputField").GetComponent<InputField> ().text = "";
		foreach (Transform child in AddUserPanel.transform.FindChild("AddDataPanel").FindChild("Current Roles Panel")) 
		{
			Destroy (child.gameObject);
		}

		GameObject Person = new GameObject();
		Person.transform.parent = SelectedBatch.transform;
		SelectedPersonel = Person.AddComponent<Person>();
		SelectedPersonel.ListOfRoles = new List<Roles> ();
	}

	public void AddPersonelAndSave ()
	{
		if (SelectedBatch != null) 
		{
			string Name = AddUserPanel.transform.FindChild("AddDataPanel").FindChild ("Name Panel").FindChild ("InputField").GetComponent<InputField> ().text;
			string IC = AddUserPanel.transform.FindChild("AddDataPanel").FindChild ("IC Panel").FindChild ("InputField").GetComponent<InputField> ().text;

			JSONArray NodeArray = new JSONArray ();
			foreach (Transform child in AddUserPanel.transform.FindChild("AddDataPanel").FindChild("Current Roles Panel")) 
			{
				JSONData node = new JSONData (child.FindChild ("Image").FindChild ("Text").GetComponent<Text> ().text);
				NodeArray.Add (node);
			}

			SelectedPersonel.Set(Name,
				IC,
				new System.DateTime(),
				NodeArray);
			SelectedPersonel.Parent = SelectedBatch;
			SelectedBatch.AddPersonal(SelectedPersonel);

			GameObject UserObject = Instantiate (UserUIObject) as GameObject;
			Transform parent = UserUIParent.transform.FindChild (SelectedBatch.BatchName);
			UserObject.transform.parent = parent;
			UserObject.transform.localScale = Vector3.one;
			UserObject.name = Name;
			UserObject.GetComponent<UserUI> ().UpdateUI(SelectedPersonel);
		}
	}

	public void AddPersonelRole ()
	{
		Dropdown rolesDropdown = AddUserPanel.transform.FindChild("AddDataPanel").FindChild ("Roles Panel").FindChild ("Dropdown").GetComponent<Dropdown> ();
		string choice = rolesDropdown.options [rolesDropdown.value].text;
		//Debug.Log (choice);
		if (SelectedPersonel.ListOfRoles.Contains (StaticVars.RolesParseJson (choice)) == false) 
		{
			//SelectedPersonel.ListOfRoles.Add (StaticVars.RolesParseJson (choice));

			GameObject RoleObj = Instantiate (RolesObject) as GameObject;
			RoleObj.transform.parent = AddUserPanel.transform.FindChild("AddDataPanel").FindChild("Current Roles Panel");
			RoleObj.transform.localScale = Vector3.one;
			RoleObj.transform.FindChild ("Image").FindChild ("Text").GetComponent<Text> ().text = choice;

			RoleObj.transform.FindChild ("Remove Role").GetComponent<Button> ().onClick.AddListener (() => {
				Debug.Log("Removing " + choice + " from " + SelectedPersonel.Name);
				SelectedPersonel.ListOfRoles.Remove(StaticVars.RolesParseJson (choice));
				Destroy(RoleObj);
			});
		}
	}

	public void AddRoleToPersonel ()
	{
		if (SelectedPersonel != null) 
		{
			Dropdown rolesDropdown = EditUserPanel.transform.FindChild("UserDataPanel").FindChild ("Roles Panel").FindChild ("Dropdown").GetComponent<Dropdown> ();
			string choice = rolesDropdown.options [rolesDropdown.value].text;
			Debug.Log (choice);
			if (SelectedPersonel.ListOfRoles.Contains (StaticVars.RolesParseJson (choice)) == false) 
			{
				SelectedPersonel.ListOfRoles.Add (StaticVars.RolesParseJson (choice));

				GameObject RoleObj = Instantiate (RolesObject) as GameObject;
				RoleObj.transform.parent = CurrentRoleParent.transform;
				RoleObj.transform.localScale = Vector3.one;
				RoleObj.transform.FindChild ("Image").FindChild ("Text").GetComponent<Text> ().text = choice;

				RoleObj.transform.FindChild ("Remove Role").GetComponent<Button> ().onClick.AddListener (() => {
					Debug.Log("Removing " + choice + " from " + SelectedPersonel.Name);
					SelectedPersonel.ListOfRoles.Remove(StaticVars.RolesParseJson (choice));
					Destroy(RoleObj);
				});
			}
		}
	}

	public void DrawRoles(List<Roles> RolesToDraw)
	{
		foreach (Transform child in CurrentRoleParent.transform) 
		{
			Destroy (child.gameObject);
		}

		foreach(Roles role in RolesToDraw)
		{
			GameObject RoleObj = Instantiate (RolesObject) as GameObject;
			RoleObj.transform.parent = CurrentRoleParent.transform;
			RoleObj.transform.localScale = Vector3.one;
			RoleObj.transform.FindChild ("Image").FindChild ("Text").GetComponent<Text> ().text = StaticVars.RolesParseJson (role);

			RoleObj.transform.FindChild ("Remove Role").GetComponent<Button> ().onClick.AddListener (() => {
				Debug.Log("Removing " + role + " from " + SelectedPersonel.Name);
				SelectedPersonel.ListOfRoles.Remove(role);
				Destroy(RoleObj);
			});
		}
	}

	public void DrawUsers ()
	{
		UserUIObject.SetActive (true);
	}
}
