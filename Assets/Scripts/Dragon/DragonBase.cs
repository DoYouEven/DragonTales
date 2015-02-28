﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DragonBase : MonoBehaviour
{

    #region DragonStuff
    public string name;
    public int playerID;
    public int initialTailCount;
    private GameObject tailEnd;
    public GameObject tailPrefab;
    public List<Tail> tails;
	public GameObject Prefab;
    
    #endregion


    #region moves
    public List<MoveData> moves = new List<MoveData>();
    private Dictionary<int, Action<MoveData, int>> currentMoveMethods = new Dictionary<int, Action<MoveData, int>>();
    public Powerup p;
    #endregion
    bool inputMouseButton;


	// Use this for initialization
	void Start () {

        //ToDo needs to be loaded from a data loader

        //*************Dash
        MoveData Dash = new MoveData();
        Dash.name = "Dash";
        Dash.Cooldown = 4.0f; 
        Dash.ChargeTime = 0.5f;
        Dash.ID = 0;
        moves.Add(Dash);
        //***********************

        MoveData Bite = new MoveData();
        Bite.name = "Bite";
        Bite.Cooldown = 3.0f;
        Bite.ID = 1;
        moves.Add(Bite);
        currentMoveMethods[0] = DashAttack;
        currentMoveMethods[1] = BiteAttack;



        for (int i = 0; i < initialTailCount; i++)
        {
            ExtendTail();
        }
        //Mapping the moveToAMethod
		var r = UnityEngine.Random.Range(0, 100);
		if (r > 90 && GameObject.FindWithTag("Prefab") == null) {
			var range = 3;
			Instantiate(Prefab, new Vector3(UnityEngine.Random.Range(-range, range), 1, UnityEngine.Random.Range(-range, range)), new Quaternion(0, 0,0,1));
			
		}

	}
	
    void ExtendTail()
    {
        if (tailEnd == null)
            tailEnd = this.gameObject;

        //Add tail and keep track
        GameObject newTail = (GameObject)Instantiate(tailPrefab, tailEnd.transform.position - tailEnd.transform.forward, new Quaternion(0, 0, 0, 0));
        Tail tail =  newTail.GetComponent<Tail>();
        tail.GetComponent<Tail>().OwnerID = playerID;
        tails.Add(tail);
        tail.tailNo = tails.Count;
        newTail.transform.rotation = tailEnd.transform.rotation;
		// new ojects were distorted so I changed this
        newTail.transform.localScale = tailEnd.transform.localScale * 0.9f; //old -> // - new Vector3(0.2f, 0.1f, 0);
        Joint joint = newTail.GetComponent<HingeJoint>();
        if (joint != null)
        {
            newTail.transform.parent = tailEnd.transform;
            joint.connectedBody = tailEnd.rigidbody;
            tailEnd = newTail;
        }
		
        rigidbody.mass++; // make the head weight greater so it can carry it's tail... lol
        //moveSpeed += 0.05f;

        return;
    }

    void BreakTail(int tailNo)
    {
        for(int i = tailNo - 1; i <=tails.Count ; i ++)
        {
             tails.Remove(tails[tailNo]);
            Destroy(tails[tailNo].gameObject);
           
        }
        if(tails.Count == 0)
        {
            tailEnd = null;
        }
  
        tailEnd = tails[tailNo -1].gameObject;
    }
	// Update is called once per frame
	void Update () {

        inputMouseButton = Input.GetMouseButton(0);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ExtendTail();
        }
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
        if (hit.gameObject.tag == "Tail")
        {
            BreakTail(hit.gameObject.GetComponent<Tail>().tailNo);
            ExtendTail();
        }

		if (hit.gameObject.name == "Prefab(Clone)") {

			
			if (UnityEngine.Random.value > 0.5) {
				
				Destroy (hit.gameObject);
				return;
				
			}
		}
    }
    void CastMove(int selectedIndex)
    {

      
    
        /* ToDO
         * MoveList
         * 1) Dash
         * 2) Bite
         * 4) Ice Shot 
         * 5) Fire Shot
         * 6) Sheild  
         * 3) Sprint( x% boost in speed)
         */
        currentMoveMethods[moves[selectedIndex].ID].Invoke(moves[selectedIndex], selectedIndex);

       
    }

     void DashAttack(MoveData moveData, int index)
    {
         
        StartCoroutine(DashAttackCo(moveData));

    }

    void BiteAttack(MoveData moveData, int index)
     {

     }
    void ConstraintRotation(bool constraint)
{
        if (constraint)
        {
            for (int i = 0; i < tails.Count; i++)
            {
                tails[i].rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            }
            gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            for (int i = 0; i < tails.Count; i++)
            {
                tails[i].rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            }
            gameObject.rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        }

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
                     ConstraintRotation(true);
                     rigidbody.AddForce(transform.forward * 1500 + new Vector3(0,0.5f,0), ForceMode.Impulse);
                    //TODO state can eat
                     yield return new WaitForSeconds(0.3f);


                     moveData.ResetCharge();
                     ConstraintRotation(false);
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
