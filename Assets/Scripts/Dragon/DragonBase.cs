using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DragonBase : MonoBehaviour {


    public string name;

    public List<MoveData> moves = new List<MoveData>();
    private Dictionary<int, Action<MoveData, int>> currentMoveMethods = new Dictionary<int, Action<MoveData, int>>();
    public Powerup p;
	// Use this for initialization
	void Start () {

        //ToDo needs to be loaded from a data loader
        MoveData Dash = new MoveData();
        Dash.name = "Dash";
        Dash.cooldown = 4.0f; 
        Dash.chargeTime = 4.0f;
        Dash.ID = 0;
        moves.Add(Dash);
        currentMoveMethods[Dash.ID] = DashAttack;

        //Mapping the moveToAMethod
	}
	
	// Update is called once per frame
	void Update () {

       

        if(Input.GetMouseButtonDown(0))
        {
            CastMove(0);
            
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
         if (Input.GetMouseButton(0))
         {
             moveData.currentChargeTime += 0.01f;
             if (moveData.currentChargeTime > moveData.chargeTime)
             {
                 Debug.Log()
                 //
                 //TODO Do something by design
                 //AddForceInCurrentDirection[of head]
                 //
             }
             Debug.Log(moveData.chargeTime);
         }
        if(!Input.GetMouseButton(0))
        {
            while(moveData.currentChargeTime<=0)
            {
                moveData.currentChargeTime -= 0.01f;
            }
        }
         yield return null;
     }

    }
