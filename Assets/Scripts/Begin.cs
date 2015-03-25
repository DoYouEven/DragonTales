using UnityEngine;
using System.Collections;

public class Begin : MonoBehaviour {
	public GameObject menu;
	public GameObject play;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("P1Skill1"))
		{
			menu.SetActive(false);
			play.SetActive(true);
		}
	}
}
