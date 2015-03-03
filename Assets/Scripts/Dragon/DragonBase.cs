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
	public GameObject legsPrefab;
	public List<Tail> tails;
	public GameObject Prefab;
	public GameObject IceBullet;
	public bool canDash = true;
	private float moveSpeed;
	private float normalSpeed;
	private float sprintSpeed;
	private float frozenSpeed;
	private bool isBreaking;
	
	//****New protoType
	public GameObject bodyPrefab;
	public GameObject finalTail;
	private MoveAssetDatabase moveAssetDatabase;
	#endregion

	#region controls
	public KeyCode DashAttackKey;
	#endregion
	
	#region moves
	public List<MoveData> moves = new List<MoveData>();
	private Dictionary<int, Action<MoveData, int>> currentMoveMethods = new Dictionary<int, Action<MoveData, int>>();
	public Powerup p;
	public bool hasBitePowerup;
	#endregion
	bool inputMouseButton, inputDashButton;
	
	#region DragonStates
	private int dashState = 0;   // 0=no dash, 1=weak, 2=medium, 3=strong
	private bool hasIce;
	#endregion
	
	#region events
	public  delegate void EventPowerup(MoveData moveData);
	public event EventPowerup onPowerup = new EventPowerup((MoveData moveData) => { });  //UI event broadcasting
	#endregion
	
	
	
	// Use this for initialization
	void Start () {
		moveAssetDatabase = GameController.instance.moveAssetDatabase;
		//ToDo needs to be loaded from a data loader
		
		//*************Dash
		MoveData Dash = new MoveData();
		Dash.name = "Dash";
		Dash.Cooldown = 4.0f; 
		Dash.ChargeTime = 0.5f;
		Dash.ID = 0;
		Dash.icon = moveAssetDatabase.GetByID(Dash.ID).icon;
		moves.Add(Dash);
		onPowerup(Dash);
		//***********************
		
		//*************Bite
		MoveData Bite = new MoveData();
		Bite.name = "Bite";
		Bite.Cooldown = 4.0f;
		Bite.ID = 1;
		moves.Add(Bite);
		//***********************
		
		//*************Sprint
		MoveData Sprint = new MoveData();
		Sprint.name = "Sprint";
		Sprint.Cooldown = 4.0f;
		Sprint.ID = 2;
		moves.Add(Sprint);
		//***********************
		
		//*************Ice
		MoveData Ice = new MoveData();
		Ice.name = "Sprint";
		Ice.Cooldown = 4.0f;
		Ice.ID = 3;
		moves.Add(Ice);
		//***********************
		
		currentMoveMethods[0] = DashAttack;
		currentMoveMethods[1] = BiteAttack;
		currentMoveMethods[2] = SprintAttack;
		currentMoveMethods[3] = IceAttack;
		
		moveSpeed = this.GetComponent<MovementController> ().moveSpeed;
		normalSpeed = moveSpeed;
		sprintSpeed = moveSpeed + 15;
		frozenSpeed = moveSpeed - 8;
		
		for (int i = 0; i < initialTailCount; i++)
		{
			//ExtendTail();
			//ExtendTailProtoType();
			ExtendTail2();
		}

		// set color
		if (playerID == 2) {
			Color colorM = transform.GetChild(0).GetChild(0).renderer.material.color;
			colorM.r = colorM.r * 0.5f;
			colorM.b = colorM.b * 1.5f;
			transform.GetChild(0).GetChild(0).renderer.material.color = colorM;
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
	// public abstract void ExtendTail();
	public void ExtendTailProtoType()
	{
		if (tailEnd == null)
			tailEnd = this.gameObject;
		
		//Add tail and keep track
		GameObject newBodyPrefab = (GameObject)Instantiate(bodyPrefab, tailEnd.transform.position - tailEnd.transform.forward, new Quaternion(0, 0, 0, 0));
		Tail tail = newBodyPrefab.GetComponent<Tail>();
		tail.GetComponent<Tail>().OwnerID = playerID;
		tails.Add(tail);
		tail.tailNo = tails.Count - 1;
		newBodyPrefab.transform.rotation = newBodyPrefab.transform.rotation;
		// new ojects were distorted so I changed this
		//newTail.transform.localScale = tailEnd.transform.localScale * 0.9f; //old -> // - new Vector3(0.2f, 0.1f, 0);
		Joint joint = newBodyPrefab.GetComponent<HingeJoint>();
		if (joint != null)
		{
			newBodyPrefab.transform.parent = tailEnd.transform;
			joint.connectedBody = tailEnd.rigidbody;
			tailEnd = newBodyPrefab;
			
			//Final Tail Readjustment
			finalTail.GetComponent<HingeJoint>().connectedBody = tailEnd.rigidbody;
			finalTail.transform.position = tailEnd.transform.position - tailEnd.transform.forward;
		}
		
		rigidbody.mass++; // make the head weight greater so it can carry it's tail... lol
		//moveSpeed += 0.05f;
		if (this.GetComponent<MovementController>().moveSpeed > minSpeed)
			// speed depends of tail length (longer = slower)
			this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;
		return;
	}
	public void ExtendTail()
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
		//newTail.transform.localScale = tailEnd.transform.localScale * 0.9f; //old -> // - new Vector3(0.2f, 0.1f, 0);
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
	public void ExtendTail2()
	{
		if (tailEnd == null)
			tailEnd = this.gameObject;
		
		//Add tail and keep track
		GameObject newTail;
		if (tails.Count == 2)
			newTail = (GameObject)Instantiate(legsPrefab, tailEnd.transform.position - tailEnd.transform.forward, new Quaternion(0, 0, 0, 0));
		else
			newTail = (GameObject)Instantiate(tailPrefab, tailEnd.transform.position - tailEnd.transform.forward, new Quaternion(0, 0, 0, 0));

		Tail tail =  newTail.GetComponent<Tail>();
		tail.GetComponent<Tail>().OwnerID = playerID;
		tails.Add(tail);
		tail.tailNo = tails.Count - 1;

		// scale each segment smaller as the tail grows
		if (tail.tailNo > 2) {
			Vector3 newScale = new Vector3(newTail.transform.GetChild(0).transform.localScale.x * Mathf.Pow (0.97f, tails.Count),
			                               newTail.transform.GetChild(0).transform.localScale.y * Mathf.Pow (0.97f, tails.Count),
			                               newTail.transform.GetChild(0).transform.localScale.z);

			newTail.transform.GetChild(0).transform.localScale = newScale; //old -> // - new Vector3(0.2f, 0.1f, 0);

			Vector3 newScale2 = new Vector3(newTail.transform.GetChild(0).GetChild(1).transform.localScale.x * Mathf.Pow (0.97f, tails.Count),
			                                newTail.transform.GetChild(0).GetChild(1).transform.localScale.y * Mathf.Pow (0.97f, tails.Count),
			                                newTail.transform.GetChild(0).GetChild(1).transform.localScale.z * Mathf.Pow (0.97f, tails.Count));
			newTail.transform.GetChild(0).GetChild(1).transform.localScale = newScale2;
		}

		newTail.transform.rotation = tailEnd.transform.rotation;
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
		
		//inputMouseButton = Input.GetMouseButton(0);
		inputMouseButton = Input.GetKey (DashAttackKey);
		
		if(Input.GetKeyDown(KeyCode.Space))
		{
			ExtendTail2();
		}
		if(Input.GetKeyDown(DashAttackKey))
		{
			if (canDash)
				CastMove(0);
		}
		if (Input.GetButtonDown("UsePower") && hasIce)
		{
			CastMove(3);
		}
		
	}
	
	void OnCollisionEnter(Collision hit)
	{
		//**********Tail collisions
		if (hit.gameObject.tag == "Tail")
		{
			int currentTail = hit.gameObject.GetComponent<Tail>().tailNo;
			string playerTag = "Player" + hit.gameObject.GetComponent<Tail>().OwnerID.ToString ();
			int ownerID = hit.gameObject.GetComponent<Tail>().OwnerID;
			
			// Collision with OWN tail
			if (ownerID == playerID && tails.Count > 1 && !isBreaking) {
				isBreaking = true;
				StartCoroutine(BreakingTail());
				BreakTail(currentTail);
			} 
			
			// Bite powerup
			if (hasBitePowerup && ownerID != 0) {
				GameObject otherPlayer = GameObject.FindGameObjectWithTag (playerTag);
				otherPlayer.GetComponent<DragonBase> ().BreakTail(currentTail);
				Destroy (hit.gameObject);
				ExtendTail2();
			} 
			
			// Eat a broken tail piece
			if (hit.gameObject.GetComponent<Tail>().canEat && !isBreaking) {
				Destroy (hit.gameObject);
				ExtendTail2();
			} 
			// collision with enemy tail while not dashing
			if (ownerID != playerID && !hit.gameObject.GetComponent<Tail>().canEat && dashState == 0) 
			{
				Quaternion relative = Quaternion.Inverse (hit.gameObject.transform.rotation) * transform.rotation;
				Deflect(relative);
			}
			
			// Collision with enemy tail while Dashing
			if (ownerID != playerID && !isBreaking && ownerID != 0) {
				if (dashState > 0) 
				{
					isBreaking = true;
					GameObject otherPlayer = GameObject.FindGameObjectWithTag (playerTag);
					Quaternion relative = Quaternion.Inverse (hit.gameObject.transform.rotation) * transform.rotation;
					StartCoroutine(BreakingTail());
					if (dashState == 1) 
					{
						if (currentTail > 10) 
							otherPlayer.GetComponent<DragonBase> ().BreakTail(currentTail);
						else
							Deflect(relative);
					}
					else if (dashState == 2) 
					{
						if (currentTail > 4) 
							otherPlayer.GetComponent<DragonBase> ().BreakTail(currentTail);
						else
							Deflect(relative);
					}
					else if (dashState == 3) 
					{
						otherPlayer.GetComponent<DragonBase> ().BreakTail(currentTail);
					}
				}
			} 
		}
		//****************************
		
		//**********Power up collisions***********
		// BITE
		else if (hit.gameObject.tag == "BitePowerUp")
		{
			Destroy(hit.gameObject);
			CastMove(1);
		}
		// SPRINT
		else if (hit.gameObject.tag == "SprintPowerUp")
		{
			Destroy(hit.gameObject);
			CastMove(2);
		}
		// ICE
		else if (hit.gameObject.tag == "IcePowerUp")
		{
			Destroy(hit.gameObject);
			hasIce = true;
		}
		//****************************************
		
		// collision with other player's head
		else if (hit.gameObject.tag == "Player1" || hit.gameObject.tag == "Player2")
		{ 
			Quaternion relative = Quaternion.Inverse (hit.gameObject.transform.rotation) * transform.rotation;
			Deflect(relative);
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
	
	void Deflect (Quaternion relative)
	{
		if (relative.y > 0)
			rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * 1000)));
		else
			rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * -1000)));
	}
	
	void CastMove(int selectedIndex)
	{
		
		/* ToDO
         * MoveList
         * 1) DONE - Dash
         * 2) DONE - Bite
         * 3) Sprint( x% boost in speed)
         * 4) Ice Shot 
         * 5) Fire Shot
         * 6) Sheild  
         */
		currentMoveMethods[moves[selectedIndex].ID].Invoke(moves[selectedIndex], selectedIndex);
	}
	
	void DashAttack(MoveData moveData, int index)
	{ 
		StartCoroutine(DashAttackCo(moveData));
	}
	
	void BiteAttack(MoveData moveData, int index)
	{
		StartCoroutine(BiteAttackCo(moveData));
	}
	
	void SprintAttack(MoveData moveData, int index)
	{
		StartCoroutine(SprintAttackCo(moveData));
	}
	
	void IceAttack(MoveData moveData, int index)
	{
		GameObject ice = (GameObject)Instantiate(IceBullet, transform.position, transform.rotation);
		ice.GetComponent<IceBullet>().ownerID = playerID;
		hasIce = false;
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
					this.GetComponent<MovementController>().moveSpeed -= 0.1f;
				
				// Strong Dash
				if (moveData.currentChargeTime > moveData.ChargeTime)
				{
					StartCoroutine(PerformDash(3, 13));
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
					StartCoroutine(PerformDash(1, 7));
					moveData.ResetCharge();
					break;
				}
				// Medium dash
				else if (moveData.currentChargeTime >= (moveData.ChargeTime * 2)/3 && moveData.currentChargeTime < moveData.ChargeTime)
				{
					StartCoroutine(PerformDash(2, 10));
					moveData.ResetCharge();
					break;
				}
				else  
				{
					ResetSpeed();
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
	
	IEnumerator PerformDash(int state, int speed)
	{
		dashState = state;
		// move the player if they are stopped
		GetComponent<MovementController>().isDashing = true;
		
		IncreaseSpeed(speed);
		yield return new WaitForSeconds(0.5f);
		ResetSpeed();
		
		dashState = 0;
		GetComponent<MovementController>().isDashing = false;
	}
	
	void IncreaseSpeed(int speed) 
	{
		if (Mathf.Pow(0.98f, tails.Count) * moveSpeed > minSpeed)
			this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed + speed;
		else
			this.GetComponent<MovementController>().moveSpeed = minSpeed + speed;
	}
	
	void ResetSpeed() 
	{
		if (Mathf.Pow(0.98f, tails.Count) * moveSpeed > minSpeed)
			this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;
		else
			this.GetComponent<MovementController>().moveSpeed = minSpeed;
	}
	
	IEnumerator BiteAttackCo(MoveData moveData)
	{
		hasBitePowerup = true;
		canDash = false;
		Color oldcolor = transform.GetChild(0).GetChild(0).renderer.material.color;
		Color newcolor = oldcolor;
		newcolor = new Color(1, 0.8f, 0);
		transform.GetChild(0).GetChild(0).renderer.material.color = newcolor;
		yield return new WaitForSeconds(moveData.Cooldown);
		transform.GetChild(0).GetChild(0).renderer.material.color = oldcolor;
		hasBitePowerup = false;
		canDash = true;
	}
	
	IEnumerator SprintAttackCo(MoveData moveData)
	{
		canDash = false;
		GetComponent<MovementController>().isDashing = true;
		dashState = 3;
		Color oldcolor = transform.GetChild(0).GetChild(0).renderer.material.color;
		Color newcolor = oldcolor;
		newcolor = new Color(0.1f, 0.1f, 0.1f);
		transform.GetChild(0).GetChild(0).renderer.material.color = newcolor;

		IncreaseSpeed(6); 
		yield return new WaitForSeconds(moveData.Cooldown);
		ResetSpeed();

		GetComponent<MovementController>().isDashing = false;
		transform.GetChild(0).GetChild(0).renderer.material.color = oldcolor;
		canDash = true;
		dashState = 0;
	}
}