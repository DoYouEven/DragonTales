using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {

	// Bring panel to front
	public void GoToLevelSelect() {
		GetComponent<UIPanel> ().depth = 3;
	}

	// Back to start menu
	public void GoBack() {
		GetComponent<UIPanel> ().depth = 1;
	}

	public void LoadLevelOne() {
		Application.LoadLevel ("MVPALPHA2"); 
	}
}
