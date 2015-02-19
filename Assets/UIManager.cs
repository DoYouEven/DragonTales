using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	// Use this for initialization
   
    public static UIManager instance;

    public EnergyBar gameTimer;
    public GameController gameController;
	void Start () {
        instance = this;
        if (gameTimer == null)
            Debug.Log("Game Timer is not attached to the UI manager");
        else
        {
            gameTimer.SetValueMax((int)gameController.MaxGameTime);
        }
        
        
	}
	
	// Update is called once per frame
	void Update () {
        gameTimer.SetValueCurrentF(gameController.currentTime);
	}
}
