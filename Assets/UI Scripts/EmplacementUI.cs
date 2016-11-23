using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections;

public class EmplacementUI : MonoBehaviour {

	public Emplacement MainData;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateUI (Emplacement Data)
	{
		MainData = Data;
		GetComponent<Button> ().onClick.AddListener (() => UpdateEmplacementPanel (Data));
	}

	public void UpdateEmplacementPanel(Emplacement Data)
	{
		if(EmplacementManagementSystem.Instance().isDragging == false)
		{
			GameObject EmplacementEditUI = EmplacementManagementSystem.Instance ().EmplacementEditUI;
			EmplacementEditUI.SetActive (true);
			EmplacementManagementSystem.Instance ().EmplacementAddUI.SetActive(false);
			EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Emplacement Name Panel").FindChild ("InputField").GetComponent<InputField> ().text = Data.NameOfEmplacement;
			EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Role Panel").FindChild ("Dropdown").GetComponent<Dropdown> ().value = (int)Data.CurrentRole;
			EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Do Easy Panel").FindChild ("Toggle").GetComponent<Toggle> ().isOn = Data.Easy;
			EmplacementManagementSystem.Instance().SelectedEmplacement = MainData;
		}
	}
}
