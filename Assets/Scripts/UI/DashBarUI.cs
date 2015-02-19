using UnityEngine;
using System.Collections;

public class DashBarUI : MonoBehaviour {

	// Use this for initialization
    public DragonBase dragonBase;
    public EnergyBar dashBar;
	void Start () {
        dashBar = GetComponent<EnergyBar>();
	}
	
	// Update is called once per frame
	void Update () {
        dashBar.SetValueCurrentF(dragonBase.moves[0].currentChargeTime);
	
	}
}
