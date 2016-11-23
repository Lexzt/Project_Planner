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

	private static EmplacementManagementSystem mInstance = null;
	public static EmplacementManagementSystem Instance()
	{
		return mInstance;
	}

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
}
