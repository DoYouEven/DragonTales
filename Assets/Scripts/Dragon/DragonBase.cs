using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DragonBase : MonoBehaviour
{

    #region DragonStuff
    public string name;
    public int playerID;
    public int initialTailCount;
    public float minSpeed;
    private GameObject tailEnd;
    public GameObject tailPrefab;
    public List<Tail> tails;
    public GameObject Prefab;
    public bool canDash = true;
    private float moveSpeed;
    private float normalSpeed;
    private float sprintSpeed;
    private float frozenSpeed;
    private bool isBreaking;
    #endregion

    #region moves
    public List<MoveData> moves = new List<MoveData>();
    private Dictionary<int, Action<MoveData, int>> currentMoveMethods = new Dictionary<int, Action<MoveData, int>>();
    public Powerup p;
    #endregion
    bool inputMouseButton, inputDashButton;

    #region DragonStates
    private int dashState = 0;   // 0=no dash, 1=weak, 2=medium, 3=strong
    #endregion


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

		moveSpeed = this.GetComponent<MovementController> ().moveSpeed;
        normalSpeed = moveSpeed;
        sprintSpeed = moveSpeed + 15;
        frozenSpeed = moveSpeed - 8;

        for (int i = 0; i < initialTailCount; i++)
        {
            ExtendTail();
        }

        //Mapping the moveToAMethod
        if (GameObject.FindWithTag ("Prefab") == null)
        {
            Instantiate(Prefab, new Vector3(UnityEngine.Random.Range(-6, 8), 1, UnityEngine.Random.Range(-10, 3)), new Quaternion(0, 0,0,1));
		}
        /*var r = UnityEngine.Random.Range(0, 100);
		if (r > 90 && GameObject.FindWithTag("Prefab") == null) {
			var range = 3;
			Instantiate(Prefab, new Vector3(UnityEngine.Random.Range(-range, range), 1, UnityEngine.Random.Range(-range, range)), new Quaternion(0, 0,0,1));
			
		}*/

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
        tail.tailNo = tails.Count - 1;
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
        if (this.GetComponent<MovementController>().moveSpeed > minSpeed)
            // speed depends of tail length (longer = slower)
            this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;
        return;
    }

    public void BreakTail(int tailNo)
    {
        // remove parent and hingjoint from remainig segments
        for(int i=tailNo; i<tails.Count ; i++)
        {
            tails [i].gameObject.transform.parent = null;
            tails [i].gameObject.GetComponent<Tail>().OwnerID = 0;
            Component.Destroy(tails [i].gameObject.GetComponent<HingeJoint> ());
            tails [i].gameObject.GetComponent<Tail>().Break();
        }
        // update tail list
        tails = tails.GetRange (0, tailNo);
        // check if speed needs to be updated
        if (Mathf.Pow(0.98f, tails.Count) * moveSpeed > minSpeed)
            this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;

        // set new tail end
        if(tails.Count == 0)
            tailEnd = null;
        else
            tailEnd = tails[tailNo-1].gameObject;
    }

    // breaking cooldown
    IEnumerator BreakingTail() 
    {
        yield return new WaitForSeconds(0.5f);
        isBreaking = false;
    }

	// Update is called once per frame
	void Update () {

        inputMouseButton = Input.GetMouseButton(0);
        inputDashButton = Input.GetKey("s");

        if(Input.GetKeyDown(KeyCode.Space))
        {
            ExtendTail();
        }
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown("s"))
        {
            if (canDash)
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
            // Collision with OWN tail
            if (hit.gameObject.GetComponent<Tail>().OwnerID == playerID && tails.Count > 1 && !isBreaking) {
                isBreaking = true;
                StartCoroutine(BreakingTail());
                BreakTail(hit.gameObject.GetComponent<Tail>().tailNo);
            } 

            // Eat a broken tail piece
            if (hit.gameObject.GetComponent<Tail>().canEat && !isBreaking) {
                Destroy (hit.gameObject);
                ExtendTail();
            } 
            // collision with enemy tail while not dashing
            if (hit.gameObject.GetComponent<Tail>().OwnerID != playerID && !hit.gameObject.GetComponent<Tail>().canEat && dashState == 0) 
            {
                Quaternion relative = Quaternion.Inverse (hit.gameObject.transform.rotation) * transform.rotation;
                if (relative.y > 0)
                    rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * 1000)));
                else
                    rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * -1000)));
            }

            // Collision with enemy tail while Dashing
            if (hit.gameObject.GetComponent<Tail>().OwnerID != playerID && !isBreaking) {
                if (dashState > 0) 
                {
                    isBreaking = true;
                    int currentTail = hit.gameObject.GetComponent<Tail>().tailNo;
                    string playerTag = "Player" + hit.gameObject.GetComponent<Tail>().OwnerID.ToString ();
                    GameObject otherPlayer = GameObject.FindGameObjectWithTag (playerTag);
                    StartCoroutine(BreakingTail());
                    if (dashState == 1) 
                    {
                        if (currentTail > 10) 
                        {
                            otherPlayer.GetComponent<DragonBase> ().BreakTail(hit.gameObject.GetComponent<Tail>().tailNo);
                        }
                    }
                    else if (dashState == 2) 
                    {
                        if (currentTail > 4) 
                        {
                            otherPlayer.GetComponent<DragonBase> ().BreakTail(hit.gameObject.GetComponent<Tail>().tailNo);
                        }
                    }
                    else if (dashState == 3) 
                    {
                        otherPlayer.GetComponent<DragonBase> ().BreakTail(hit.gameObject.GetComponent<Tail>().tailNo);
                    }
                }
            } 
        }

        // collision with other player's head
        else if (hit.gameObject.tag == "Player1" || hit.gameObject.tag == "Player2")
        { 
            Quaternion relative = Quaternion.Inverse (hit.gameObject.transform.rotation) * transform.rotation;
            if (relative.y > 0)
                rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * 1000)));
            else
                rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * -1000)));
        }

        // collision with obstacle
        else if (hit.gameObject.tag == "Obstacle")
            Destroy(this.gameObject);

        ////
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

                // slow down while charging
                if (this.GetComponent<MovementController>().moveSpeed > 0)
                    this.GetComponent<MovementController>().moveSpeed -= 0.15f;

                // Strong Dash
                if (moveData.currentChargeTime > moveData.ChargeTime)
                {
                    dashState = 3;
                    // move the player if they are stopped
                    GetComponent<MovementController>().isDashing = true;

                    // increase the speed
                    this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed + 13;

                    yield return new WaitForSeconds(0.5f);

                    // set speed back to normal
                    this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;
                    dashState = 0;
                    GetComponent<MovementController>().isDashing = false;

                    moveData.ResetCharge();
                    break;
                }
                else
                {
                    yield return null;
                }
            }
            if (!inputMouseButton)
            {
                // Weak dash
                if (moveData.currentChargeTime >= moveData.ChargeTime/3 && moveData.currentChargeTime < (moveData.ChargeTime * 2)/3)
                {
                    dashState = 1;
                    // move the player if they are stopped
                    GetComponent<MovementController>().isDashing = true;
                    // increase the speed
                    this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed + 7;

                    yield return new WaitForSeconds(0.5f);

                    // set speed back to normal
                    this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;
                    dashState = 0;
                    GetComponent<MovementController>().isDashing = false;

                    moveData.ResetCharge();
                    break;
                }
                // Medium dash
                else if (moveData.currentChargeTime >= (moveData.ChargeTime * 2)/3 && moveData.currentChargeTime < moveData.ChargeTime)
                {
                    dashState = 2;
                    // move the player if they are stopped
                    GetComponent<MovementController>().isDashing = true;
                    // increase the speed
                    this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed + 10;

                    yield return new WaitForSeconds(0.5f);

                    // set speed back to normal
                    this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;
                    dashState = 0;
                    GetComponent<MovementController>().isDashing = false;

                    moveData.ResetCharge();
                    break;
                }
                else  
                {
                    this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;
                    moveData.ResetCharge();
                    break;
                }
            }

            /* OLD DASH
            if (inputMouseButton)
            {
                moveData.currentChargeTime += 0.01f;
                // slow down while charging
                if (this.GetComponent<MovementController>().moveSpeed > 0)
                    this.GetComponent<MovementController>().moveSpeed -= 0.1f;
                if (moveData.currentChargeTime > moveData.ChargeTime)
                {
                    ConstraintRotation(true);
                    rigidbody.AddForce(transform.forward * 1500 + new Vector3(0,0.5f,0), ForceMode.Impulse);

                    yield return new WaitForSeconds(0.5f);

                    // set speed back to normal

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
            }*/
        }
        yield return null;
    }
}
