using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DragonBase : MonoBehaviour
{

    #region DragonStuff
    public string name;
    public int initialTailCount;
    private GameObject tailEnd;
    public GameObject tailPrefab;
    public List<Tail> tails;
    
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
        MoveData Dash = new MoveData();
        Dash.name = "Dash";
        Dash.Cooldown = 4.0f; 
        Dash.ChargeTime = 0.5f;
        Dash.ID = 0;
        moves.Add(Dash);
        currentMoveMethods[0] = DashAttack;
        currentMoveMethods[1] = DashAttack;



        for (int i = 0; i < initialTailCount; i++)
        {
            ExtendTail();
        }
        //Mapping the moveToAMethod
	}
	
    void ExtendTail()
    {
        if (tailEnd == null)
            tailEnd = this.gameObject;

        //Add tail and keep track
        GameObject newTail = (GameObject)Instantiate(tailPrefab, tailEnd.transform.position - tailEnd.transform.forward, new Quaternion(0, 0, 0, 0));
       Tail tail =  newTail.GetComponent<Tail>();
      
        tails.Add(tail);
        tail.tailNo = tails.Count;
        newTail.transform.rotation = tailEnd.transform.rotation;
        newTail.transform.localScale = tailEnd.transform.localScale - new Vector3(0.2f, 0.1f, 0);
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
                     for (int i = 0; i < tails.Count; i++)
                     {
                         tails[i].rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
   
                     }
                     gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                     rigidbody.AddForce(transform.forward *400, ForceMode.Impulse);
                     moveData.ResetCharge();
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
