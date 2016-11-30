using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EmplacementManagementSystem : MonoBehaviour 
{
	[Header("[Prefabs]")]
	public GameObject EmplacementButton;

	[Header("[Parents]")]
	public GameObject EmplacementParentUI;
	public GameObject EmplacementOutsideButton;
	public GameObject EmplacementSpawnButtonParent;
	public GameObject EmplacementEditUI;
	public GameObject EmplacementAddUI;

	[Header("[Variables]")]
	public bool isDragging;

	private static EmplacementManagementSystem mInstance = null;
	public static EmplacementManagementSystem Instance()
	{
		return mInstance;
	}

	public Emplacement SelectedEmplacement;

	void Awake ()
	{
		if(!mInstance) mInstance = this;
	}

	void Start () 
	{
		EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Role Panel").FindChild ("Dropdown").GetComponent<Dropdown> ().value = 1;
	}
	
	void Update () 
	{
		
	}

	public void SwapActive ()
	{
		EmplacementParentUI.SetActive (!EmplacementParentUI.activeInHierarchy);
		EmplacementButton.SetActive (!EmplacementButton.activeInHierarchy);
	}

	public void DrawUI ()
	{
		foreach (Emplacement emp in GetComponent<Base>().Emplacements) 
		{
			GameObject EmpObj = Instantiate (EmplacementButton) as GameObject;
			EmpObj.transform.SetParent (EmplacementSpawnButtonParent.transform, false);
			EmpObj.transform.FindChild ("Text").GetComponent<Text> ().text = emp.NameOfEmplacement;
			EmpObj.name = emp.NameOfEmplacement;
			EmpObj.GetComponent<EmplacementUI> ().UpdateUI (emp);
		}
	}

	public void Dragging (bool tDrag)
	{
		isDragging = tDrag;
	}

	public void AddEmplacement ()
	{
		EmplacementAddUI.SetActive(true);
		EmplacementEditUI.SetActive(false);
	}

	public void AddEmplacementAndSave()
	{
		string EmplacementName = EmplacementAddUI.transform.FindChild("AddDataPanel").FindChild("Emplacement Name Panel").FindChild("InputField").GetComponent<InputField>().text;
		Roles CurrentRole = ((Roles) EmplacementAddUI.transform.FindChild("AddDataPanel").FindChild("Role Panel").FindChild("Dropdown").GetComponent<Dropdown>().value);
		bool DoEasy = EmplacementAddUI.transform.FindChild("AddDataPanel").FindChild("Do Easy Panel").FindChild("Toggle").GetComponent<Toggle>().isOn;

		Emplacement NewEmp = GetComponent<ChangiAirBaseEast>().AddEmplacement(EmplacementName,CurrentRole,DoEasy,0);

		GameObject EmpObj = Instantiate (EmplacementButton) as GameObject;
		EmpObj.transform.SetParent (EmplacementSpawnButtonParent.transform, false);
		EmpObj.transform.FindChild ("Text").GetComponent<Text> ().text = NewEmp.NameOfEmplacement;
		EmpObj.name = NewEmp.NameOfEmplacement;
		EmpObj.GetComponent<EmplacementUI> ().UpdateUI (NewEmp);

		EmplacementAddUI.SetActive(false);
	}

	public void EditEmplacementAndSave()
	{
		if(SelectedEmplacement != null)
		{
			string EmplacementName = EmplacementEditUI.transform.FindChild("AddDataPanel").FindChild("Emplacement Name Panel").FindChild("InputField").GetComponent<InputField>().text;
			Roles CurrentRole = ((Roles) EmplacementEditUI.transform.FindChild("AddDataPanel").FindChild("Role Panel").FindChild("Dropdown").GetComponent<Dropdown>().value);
			bool DoEasy = EmplacementEditUI.transform.FindChild("AddDataPanel").FindChild("Do Easy Panel").FindChild("Toggle").GetComponent<Toggle>().isOn;

			string OldEmpName = SelectedEmplacement.NameOfEmplacement;

			foreach(Transform child in GameObject.Find("Emplacements").transform)
			{
				if(child.name == SelectedEmplacement.NameOfEmplacement)
				{
					child.GetComponent<Emplacement>().NameOfEmplacement = EmplacementName;
					child.GetComponent<Emplacement>().CurrentRole = CurrentRole;
					child.GetComponent<Emplacement>().Easy = DoEasy;
					child.gameObject.name = EmplacementName; 
					break;
				}
			}

			foreach(Transform child in EmplacementSpawnButtonParent.transform)
			{
				if(child.name == OldEmpName)
				{
					child.FindChild("Text").GetComponent<Text>().text = EmplacementName;
					child.gameObject.name = EmplacementName; 
					break;
				}
			}
		}
	}

	public void EnableEmplacementManagementSystem ()
	{
		EmplacementParentUI.SetActive(true);
		EmplacementOutsideButton.SetActive(false);
		GetComponent<UserManagementSystem>().UserManagementSystemOutsideButton.SetActive(false);
	}

	public void DisableEmplacementManagementSystem ()
	{
		EmplacementParentUI.SetActive(false);
		EmplacementOutsideButton.SetActive(true);
		GetComponent<UserManagementSystem>().UserManagementSystemOutsideButton.SetActive(true);
	}

	public void OnDragEnd ()
	{
//		int NewPirority = 1;
//		foreach(Transform child in EmplacementSpawnButtonParent.transform)
//		{
//			if(child.name != "Fake")
//			{
//				if(child.GetComponent<EmplacementUI>().MainData.IsSpecialRole() == false)
//				{
//					child.GetComponent<EmplacementUI>().MainData.Pirority = NewPirority++;
//				}
//			}
//		}
	}
}
