using UnityEngine;
using System.Collections;

public class Options : MonoBehaviour {

	// Bring panel to front
	public void GoToOptions() {
		GetComponent<UIPanel> ().depth = 3;
	}
	
	// Back to start menu
	public void GoBack() {
		GetComponent<UIPanel> ().depth = 1;
	}
}
