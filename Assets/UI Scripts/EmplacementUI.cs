using UnityEngine;
using UnityEngine.UI;
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
		GameObject EmplacementEditUI = EmplacementManagementSystem.Instance ().EmplacementEditUI;
		EmplacementEditUI.SetActive (true);
		EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Emplacement Name Panel").FindChild ("InputField").GetComponent<InputField> ().text = Data.NameOfEmplacement;
		Debug.Log ((int)Data.CurrentRole + " - " + EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Role Panel").FindChild ("Dropdown").GetComponent<Dropdown> ().value);
		EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Role Panel").FindChild ("Dropdown").GetComponent<Dropdown> ().value = (int)Data.CurrentRole;
		Debug.Log ((int)Data.CurrentRole + " - " + EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Role Panel").FindChild ("Dropdown").GetComponent<Dropdown> ().value);
		EmplacementEditUI.transform.FindChild ("AddDataPanel").FindChild ("Do Easy Panel").FindChild ("Toggle").GetComponent<Toggle> ().isOn = Data.Easy;
	}
}
