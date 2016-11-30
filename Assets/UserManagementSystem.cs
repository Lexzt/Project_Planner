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
	public GameObject AddBatchPanel;
	public GameObject EditBatchPanel;
	public GameObject UserManagementSystemOutsideButton;
	public GameObject UserManagementSystemUI;

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
	public bool isDragging;

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
			BatchObj.transform.SetParent(BatchUIParent.transform);
			BatchObj.transform.localScale = Vector3.one;
			BatchObj.GetComponent<BatchUI> ().UpdateUI (BatchData);
			BatchObj.name = BatchData.BatchName;

			// Draw the user button here now.
			GameObject UserParent = Instantiate(UserParentUIObject) as GameObject;
			UserParent.transform.SetParent(UserUIParent.transform);
			UserParent.transform.localScale = Vector3.one;
			UserParent.transform.localPosition = Vector3.zero;
			UserParent.name = BatchData.name;
			foreach (Person people in BatchData.ListOfPeople) 
			{
				GameObject UserObject = Instantiate (UserUIObject) as GameObject;
				UserObject.transform.SetParent(UserParent.transform);
				UserObject.transform.localScale = Vector3.one;
				UserObject.name = people.Name;
				UserObject.GetComponent<UserUI> ().UpdateUI(people);
			}
			ListOfUserParents.Add (UserParent);
			UserParent.SetActive (false);

			BatchObj.GetComponent<BatchUI> ().UserParentObject = UserParent;
			BatchObj.GetComponent<Button> ().onClick.AddListener(() => DisableOtherUserParent());
			BatchObj.GetComponent<Button> ().onClick.AddListener(() => BatchObj.GetComponent<BatchUI> ().SwapUserParent ());
			BatchObj.GetComponent<Button> ().onClick.AddListener(() => {if(isDragging == false) AddPersonelButton.SetActive (true);});
			BatchObj.GetComponent<Button> ().onClick.AddListener(() => EditBatch());
		}
	}

	public void DisableOtherUserParent ()
	{
		if(isDragging ==  false)
		{
			foreach (GameObject Obj in ListOfUserParents) 
			{
				Obj.SetActive (false);
			}
		}
	}

	public void AddPersonel ()
	{
		AddUserPanel.SetActive (true);
		AddPersonelButton.SetActive (true);
		EditUserPanel.SetActive (false);
		EditBatchPanel.SetActive (false);

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
			SelectedPersonel.gameObject.name = Name;
			SelectedBatch.AddPersonal(SelectedPersonel);

			GameObject UserObject = Instantiate (UserUIObject) as GameObject;
			Transform parent = UserUIParent.transform.FindChild (SelectedBatch.BatchName);
			UserObject.transform.SetParent(parent);
			UserObject.transform.localScale = Vector3.one;
			UserObject.name = Name;
			UserObject.GetComponent<UserUI> ().UpdateUI(SelectedPersonel);

			List<string> AutoCompleteList = new List<string>();
			foreach(Batch batchData in GetComponent<Base>().Batches)
			{
				foreach(Person personel in batchData.ListOfPeople)
				{
					AutoCompleteList.Add(personel.Name);
				}
			}
			//GetComponent<NamePanel>().FillUpList(AutoCompleteList);
		}
	}

	public void SaveSelectedPersonel ()
	{
		if (SelectedPersonel != null) 
		{
			string Name = EditUserPanel.transform.FindChild("UserDataPanel").FindChild("Name Panel").FindChild("InputField").GetComponent<InputField>().text;
			string IC = EditUserPanel.transform.FindChild("UserDataPanel").FindChild("IC Panel").FindChild("InputField").GetComponent<InputField>().text;
			List<Roles> ListOfNewRoles = new List<Roles>();
			foreach(Transform child in EditUserPanel.transform.FindChild("UserDataPanel").FindChild("Current Roles Panel"))
			{
				ListOfNewRoles.Add(StaticVars.RolesParseJson(child.name));
			}

			if(SelectedPersonel.Name != Name)
			{
				foreach(Transform child in UserUIParent.transform)
				{
					if(child.gameObject.activeInHierarchy == true)
					{
						foreach(Transform Secondchild in child)
						{
							if(Secondchild.name == SelectedPersonel.Name)
							{
								Secondchild.name = Name;
								Secondchild.FindChild("Text").GetComponent<Text>().text = Name;
								break;
							}
						}
						break;
					}
				}
			}

			SelectedPersonel.Name = Name;
			SelectedPersonel.IC = IC;
			SelectedPersonel.ListOfRoles = ListOfNewRoles;
		}
	}

	public void AddPersonelRole ()
	{
		Dropdown rolesDropdown = AddUserPanel.transform.FindChild("AddDataPanel").FindChild ("Roles Panel").FindChild ("Dropdown").GetComponent<Dropdown> ();
		string choice = rolesDropdown.options [rolesDropdown.value].text;
		//Debug.Log (choice);
		List<Roles> ListOfNewRoles = new List<Roles>();
		foreach(Transform child in AddUserPanel.transform.FindChild("AddDataPanel").FindChild("Current Roles Panel"))
		{
			ListOfNewRoles.Add(StaticVars.RolesParseJson(child.name));
		}
		if (ListOfNewRoles.Contains (StaticVars.RolesParseJson (choice)) == false) 
		{
			//SelectedPersonel.ListOfRoles.Add (StaticVars.RolesParseJson (choice));

			GameObject RoleObj = Instantiate (RolesObject) as GameObject;
			RoleObj.name = choice;
			RoleObj.transform.SetParent(AddUserPanel.transform.FindChild("AddDataPanel").FindChild("Current Roles Panel"));
			RoleObj.transform.localScale = Vector3.one;
			RoleObj.transform.FindChild ("Image").FindChild ("Text").GetComponent<Text> ().text = choice;

			RoleObj.transform.FindChild ("Remove Role").GetComponent<Button> ().onClick.AddListener (() => {
				Debug.Log("Removing " + choice + " from " + SelectedPersonel.Name);
				//SelectedPersonel.ListOfRoles.Remove(StaticVars.RolesParseJson (choice));
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
			if (SelectedPersonel.ListOfRoles.Contains (StaticVars.RolesParseJson (choice)) == false) 
			{
				//SelectedPersonel.ListOfRoles.Add (StaticVars.RolesParseJson (choice));

				GameObject RoleObj = Instantiate (RolesObject) as GameObject;
				RoleObj.name = choice;
				RoleObj.transform.SetParent(CurrentRoleParent.transform);
				RoleObj.transform.localScale = Vector3.one;
				RoleObj.transform.FindChild ("Image").FindChild ("Text").GetComponent<Text> ().text = choice;

				RoleObj.transform.FindChild ("Remove Role").GetComponent<Button> ().onClick.AddListener (() => {
					Debug.Log("Removing " + choice + " from " + SelectedPersonel.Name);
					//SelectedPersonel.ListOfRoles.Remove(StaticVars.RolesParseJson (choice));
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
			RoleObj.name = StaticVars.RolesParseJson(role);
			RoleObj.transform.SetParent(CurrentRoleParent.transform);
			RoleObj.transform.localScale = Vector3.one;
			RoleObj.transform.FindChild ("Image").FindChild ("Text").GetComponent<Text> ().text = StaticVars.RolesParseJson (role);

			RoleObj.transform.FindChild ("Remove Role").GetComponent<Button> ().onClick.AddListener (() => {
				Debug.Log("Removing " + role + " from " + SelectedPersonel.Name);
				//SelectedPersonel.ListOfRoles.Remove(role);
				Destroy(RoleObj);
			});
		}
	}

	public void DrawUsers ()
	{
		UserUIObject.SetActive (true);
	}

	public void AddBatch ()
	{
		DisableOtherUserParent();
		AddBatchPanel.SetActive(true);
		AddUserPanel.SetActive (false);
		AddPersonelButton.SetActive (false);
		EditUserPanel.SetActive (false);
		EditBatchPanel.SetActive(false);
	}

	public void AddBatchAndSave()
	{
		GameObject BatchObjUI = Instantiate (BatchUIObject) as GameObject;
		BatchObjUI.transform.SetParent(BatchUIParent.transform);
		BatchObjUI.transform.localScale = Vector3.one;
		
		GameObject UserParent = Instantiate(UserParentUIObject) as GameObject;
		UserParent.transform.SetParent(UserUIParent.transform);
		UserParent.transform.localScale = Vector3.one;
		UserParent.transform.localPosition = Vector3.zero;
		UserParent.SetActive(false);
		ListOfUserParents.Add (UserParent);

		BatchObjUI.GetComponent<BatchUI> ().UserParentObject = UserParent;
		BatchObjUI.GetComponent<Button> ().onClick.AddListener(() => DisableOtherUserParent());
		BatchObjUI.GetComponent<Button> ().onClick.AddListener(() => BatchObjUI.GetComponent<BatchUI> ().SwapUserParent ());
		BatchObjUI.GetComponent<Button> ().onClick.AddListener(() => {AddPersonelButton.SetActive (true);});
		BatchObjUI.GetComponent<Button> ().onClick.AddListener(() => EditBatch());
		
		GameObject BatchObj = new GameObject();
		BatchObj.transform.parent = GameObject.Find("Batches").transform;
		Batch tempBatch = BatchObj.AddComponent<Batch>();
		tempBatch.BatchName = AddBatchPanel.transform.FindChild("AddDataPanel").FindChild ("Batch Name Panel").FindChild("InputField").GetComponent<InputField>().text;
		tempBatch.BatchNo = int.Parse(AddBatchPanel.transform.FindChild("AddDataPanel").FindChild ("Batch Number Panel").FindChild("InputField").GetComponent<InputField>().text);
		tempBatch.DoEasy = AddBatchPanel.transform.FindChild("AddDataPanel").FindChild ("Do Easy Panel").FindChild("Toggle").GetComponent<Toggle>().isOn;
		tempBatch.BaseParent = this.gameObject;
		BatchObj.name = tempBatch.BatchName;

		BatchObjUI.GetComponent<BatchUI> ().UpdateUI (tempBatch);
		UserParent.name = tempBatch.name;
		GetComponent<Base>().Batches.Add(tempBatch);

		DisableOtherUserParent();
		AddBatchPanel.SetActive(false);
	}

	public void EditBatch()
	{
		if(isDragging == false)
		{
			EditBatchPanel.SetActive(true);
			AddBatchPanel.SetActive(false);
			EditUserPanel.SetActive(false);
			AddUserPanel.SetActive(false);
			EditBatchPanel.transform.FindChild("AddDataPanel").FindChild("Batch Name Panel").FindChild("InputField").GetComponent<InputField>().text = SelectedBatch.BatchName;
			EditBatchPanel.transform.FindChild("AddDataPanel").FindChild("Batch Number Panel").FindChild("InputField").GetComponent<InputField>().text = SelectedBatch.BatchNo.ToString();
			EditBatchPanel.transform.FindChild("AddDataPanel").FindChild("Do Easy Panel").FindChild("Toggle").GetComponent<Toggle>().isOn = SelectedBatch.DoEasy;
		}
	}

	public void EditBatchAndSave ()
	{
		string Name = EditBatchPanel.transform.FindChild("AddDataPanel").FindChild("Batch Name Panel").FindChild("InputField").GetComponent<InputField>().text;
		if(SelectedBatch.BatchName != Name)
		{
			foreach(Transform child in BatchUIParent.transform)
			{
				if(child.name == SelectedBatch.BatchName)
				{
					child.FindChild("Text").GetComponent<Text>().text = Name;
					break;
				}
			}
		}
		SelectedBatch.BatchName = Name;
		SelectedBatch.BatchNo = int.Parse(EditBatchPanel.transform.FindChild("AddDataPanel").FindChild("Batch Number Panel").FindChild("InputField").GetComponent<InputField>().text);
		SelectedBatch.DoEasy = EditBatchPanel.transform.FindChild("AddDataPanel").FindChild("Do Easy Panel").FindChild("Toggle").GetComponent<Toggle>().isOn;
	}

	public void EnableUserManagementSystem()
	{
		UserManagementSystemOutsideButton.SetActive(false);
		UserManagementSystemUI.SetActive(true);
		GetComponent<EmplacementManagementSystem>().EmplacementOutsideButton.SetActive(false);
	}

	public void DisableUserManagementSystem()
	{
		UserManagementSystemOutsideButton.SetActive(true);
		UserManagementSystemUI.SetActive(false);
		GetComponent<EmplacementManagementSystem>().EmplacementOutsideButton.SetActive(true);
	}

	public void Dragging (bool tDrag)
	{
		isDragging = tDrag;
	}

	public void OnDragEnd ()
	{
		// int NewPirority = 1;
		// foreach(Transform child in EmplacementSpawnButtonParent.transform)
		// {
		// 	if(child.name != "Fake")
		// 	{
		// 		if(child.GetComponent<EmplacementUI>().MainData.IsSpecialRole() == false)
		// 		{
		// 			child.GetComponent<EmplacementUI>().MainData.Pirority = NewPirority++;
		// 		}
		// 	}
		// }
	}
}
