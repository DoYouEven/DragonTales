using UnityEngine;
using System.Collections;

public class Replay : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetButton("P1Skill1"))
        {
			Application.LoadLevel("BetaSceneV_1"); 
        }
	}
}
