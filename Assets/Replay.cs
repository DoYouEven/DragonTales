﻿using UnityEngine;
using System.Collections;

public class Replay : MonoBehaviour {
	public bool suddenDeath;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetButtonDown("P1Skill1"))
        {
			if (!suddenDeath)
				Application.LoadLevel("BetaSceneV_2"); 
			else
				Application.LoadLevel("SuddenDeathV_1"); 
        }
	}
}
