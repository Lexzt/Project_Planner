using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WordSearch : MonoBehaviour {

	public string[] dictionary;

	public InputField inputField;
	public bool caseSensetive;
	public bool restrictUserInput;
	ArrayList possibleWords;

	// Use this for initialization
	void Start () {
		inputField.onValueChanged.AddListener (delegate {
			ValueChangeCheck (); 
		});
		possibleWords = new ArrayList(dictionary.Length);
	}

	void ValueChangeCheck()
	{
		possibleWords.Clear ();
		for (int i = 0; i < dictionary.Length; i++) {
			if (dictionary [i].StartsWith (inputField.text, !caseSensetive, null))
				possibleWords.Add (i);
		}

		if (possibleWords.Count == 0 && restrictUserInput)
			inputField.text = inputField.text.Remove (inputField.text.Length - 1, 1);

		for (int i = 0; i < possibleWords.Count; i++) {
			Debug.Log (dictionary [(int)possibleWords [i]]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
