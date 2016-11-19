using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatchUI : MonoBehaviour {

	public GameObject UserParentObject;
	private Batch BatchData;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateUI (Batch tBatchData)
	{
		transform.GetChild (0).GetComponent<Text> ().text = tBatchData.BatchName;
		BatchData = tBatchData;
	}

	public void EnableUserParent ()
	{
		UserParentObject.SetActive (true);
	}

	public void DisableUserParent()
	{
		UserParentObject.SetActive (false);
	}

	public void SwapUserParent ()
	{
		UserParentObject.SetActive (!UserParentObject.activeInHierarchy);
		if (UserParentObject.activeInHierarchy == true) 
		{
			UserManagementSystem.Instance ().SelectedBatch = BatchData;
		}
	}

	public void RemoveBatch()
	{
		BatchData.RemoveMe();
		Destroy(this.gameObject);
	}
}
