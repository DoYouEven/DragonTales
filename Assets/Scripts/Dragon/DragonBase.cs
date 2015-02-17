using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DragonBase : MonoBehaviour {


    public string name;

    public List<MoveData> moves = new List<MoveData>();
    private Dictionary<int, Action<MoveData, int>> currentMoveMethods = new Dictionary<int, Action<MoveData, int>>();
    public Powerup p;

    bool inputMouseButton;


	// Use this for initialization
	void Start () {

        //ToDo needs to be loaded from a data loader
        MoveData Dash = new MoveData();
        Dash.name = "Dash";
        Dash.Cooldown = 4.0f; 
        Dash.ChargeTime = 4.0f;
        Dash.ID = 0;
        moves.Add(Dash);
        currentMoveMethods[0] = DashAttack;
        currentMoveMethods[1] = DashAttack;
        //Mapping the moveToAMethod
	}
	
	// Update is called once per frame
	void Update () {

        inputMouseButton = Input.GetMouseButton(0);

        if(Input.GetMouseButtonDown(0))
        {
            CastMove(0);
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            CastMove(1);

        }
	
	}

    void OnCollisionEnter(Collision hit)
    {

    }
    void CastMove(int selectedIndex)
    {
        currentMoveMethods[moves[selectedIndex].ID].Invoke(moves[selectedIndex], selectedIndex);
    }

     void DashAttack(MoveData moveData, int index)
    {
         
        StartCoroutine(DashAttackCo(moveData));

    }

    IEnumerator DashAttackCo(MoveData moveData)
     {
         while (true)
         {
             
             if (inputMouseButton)
             {
                 moveData.currentChargeTime += 0.01f;
                 if (moveData.currentChargeTime > moveData.ChargeTime)
                 {
                     break;
                     //
                     //TODO Do something by design
                     //AddForceInCurrentDirection[of head]
                     //
                 }
                 else
                 {
                     Debug.Log("Charging Up" + moveData.currentChargeTime);
                     yield return null;
                 }
             }
             if (!inputMouseButton)
             {
                 if(moveData.currentChargeTime >= 0)
                 {
                     moveData.currentChargeTime -= 0.01f;
                     Debug.Log("Charging down " + moveData.currentChargeTime);
                     yield return null;
                    
                 }
                 else break;
             }
         }
         yield return null;
     }

    }
