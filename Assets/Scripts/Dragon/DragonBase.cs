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
	public  GameObject DashTrail;
	public  GameObject FireTrail;
	public bool suddenDeath;
	private Color oldColor;
	public List<Texture> mat;
	
	//****New protoType
	public GameObject bodyPrefab;
	public GameObject finalTail;
	private MoveAssetDatabase moveAssetDatabase;
	private Texture thismat;
	private int matnum;
	#endregion

    public List<string> Inputs;
    #region controls
    public KeyCode Skill1;
    public KeyCode Skill2;
    public KeyCode Skill3;
    public KeyCode Skill4;
    public KeyCode Skill5;
    public KeyCode Skill6;
	#endregion

    #region mesh
    public GameObject BasicMesh;
    public GameObject SheildMesh;
	public GameObject FrostMesh;
    #endregion
    #region VFX
    public GameObject collisionVFX;
    #endregion
    bool inputMouseButton, inputDashButton;
		
	#region DragonStates
	private int dashState = 0;   // 0=no dash, 1=weak, 2=medium, 3=strong
	private bool hasIce;
	#endregion


	public AudioClip smash;
	public AudioClip bite;
	public AudioClip dash;
	public AudioClip shield;
	public AudioClip shootingbeam;
	public AudioClip fire;

	#region events
	public  delegate void EventPowerup(MoveData moveData);
    public delegate void EventPowerupUse(int index);
	public event EventPowerup onPowerup = new EventPowerup((MoveData moveData) => { });  //UI event broadcasting
    public event EventPowerupUse onPowerupUse = new EventPowerupUse((int index) => { }); 
	#endregion
    #region Moves
    public List<MoveData> moves = new List<MoveData>();
    public PlayerUI playerUI;
	public bool shielded;
    private Dictionary<int, Action<MoveData, int>> currentMoveMethods = new Dictionary<int, Action<MoveData, int>>();
    public Powerup p;
    public bool hasBitePowerup;
    public void AddMoveByID(int id)
    {
        MoveAssetData Data = moveAssetDatabase.GetByID(id);
        MoveData Dash = new MoveData();
        Dash.name = Data.Name;
        Dash.Cooldown = Data.Cooldown;
        Dash.ChargeTime = Data.ChargeTime;
        Dash.clip = Data.audioClipName;
        Dash.ID = Data.ID;
        Dash.VFXPrefab = Data.VFXPrefab;
        Dash.icon = Data.icon;
		if (moves.Count > 0)
			playerUI.moveSlots[0].Unassign();
        moves.Add(Dash);
        if(id!=0)
        onPowerup(Dash);
    }
    public void RemoveMove(int index)
    {

    }
    public void RemoveMove(MoveData move)
    {
        //OnPowerUpUse(
       if(moves.Contains(move))
       {
           moves.Remove(move);
       }
        //onPowerup(Dash);
    }
    #endregion
    // Use this for initialization
	void Start () {
		if (suddenDeath)
			SuddenDeathSprint ();
		moveAssetDatabase = GameController.instance.moveAssetDatabase;
		//ToDo needs to be loaded from a data loader
		DashTrail = transform.Find ("DashTrail").gameObject;
		FireTrail = transform.Find ("FireTrail").gameObject;
		oldColor = DashTrail.transform.Find ("Trail").particleSystem.startColor;
		smash = Resources.Load ("Sound/smash") as AudioClip;
		bite = Resources.Load ("Sound/bite") as AudioClip;
		shootingbeam = Resources.Load ("Sound/shootingbeam") as AudioClip;
		shield = Resources.Load ("Sound/shield") as AudioClip;
		dash = Resources.Load ("Sound/dash2") as AudioClip;
		fire = Resources.Load ("Sound/fire") as AudioClip;


        /*for (int i = 0; i < 6; i++)
        {
           AddMoveByID(i);
        }*/
		AddMoveByID(0);
            /*
            //*************Dash
            MoveData Dash = new MoveData();
            Dash.name = "Dash";
            Dash.Cooldown = 4.0f; 
            Dash.ChargeTime = 0.5f;
            Dash.ID = 0;
            Dash.clip = dash;
            Dash.icon = moveAssetDatabase.GetByID(Dash.ID).icon;
            moves.Add(Dash);
            onPowerup(Dash);
            //***********************
		
            //*************Bite
            MoveData Bite = new MoveData();
            Bite.name = "Bite";
            Bite.Cooldown = 4.0f;
            Bite.ID = 1;
            Bite.clip = bite;
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
            Ice.name = "Ice";
            Ice.Cooldown = 4.0f;
            Ice.ID = 3;
            Ice.VFXPrefab = moveAssetDatabase.GetByID(Ice.ID).VFXPrefab;
            moves.Add(Ice);
        
            //***********************

            //*************Fire
            MoveData Fire = new MoveData();
            Fire.name = "Fire";
            Fire.Cooldown = 4.0f;
            Fire.ID = 4;
            Fire.VFXPrefab = moveAssetDatabase.GetByID(Fire.ID).VFXPrefab;
            moves.Add(Fire);

             */
            //***********************
            currentMoveMethods[0] = DashAttack;
		currentMoveMethods[1] = BiteAttack;
		currentMoveMethods[2] = SprintAttack;
		currentMoveMethods[3] = IceAttack;
        currentMoveMethods[4] = FireAttack;
        currentMoveMethods[5] = Sheild;
		moveSpeed = this.GetComponent<MovementController> ().moveSpeed;
		normalSpeed = moveSpeed;
		sprintSpeed = moveSpeed + 15;
		frozenSpeed = moveSpeed - 8;

		// set color
		matnum = UnityEngine.Random.Range (0, 3);
		thismat = mat [matnum];
		transform.GetChild (3).GetChild (0).GetChild (0).renderer.material.SetTexture("_MainTex", thismat);
		if (playerID == 2) {
			Color colorM = transform.GetChild(3).GetChild(0).GetChild(0).renderer.material.color;
			//Debug.Log (transform.GetChild(3).GetChild(0).GetChild(0).name);
			colorM.r = colorM.r * 0.5f;
			colorM.b = colorM.b * 1.5f;
			transform.GetChild(3).GetChild(0).GetChild(0).renderer.material.color = colorM;
		}

		for (int i = 0; i < initialTailCount; i++)
		{
			//ExtendTail();
			//ExtendTailProtoType();
			ExtendTail2();
		}

//		var r = UnityEngine.Random.Range (0, 4);
//		
//		if ((GameObject.FindWithTag ("IcePowerUp") == null) || (GameObject.FindWithTag("BitPowerUp") == null))
//		{
//			Instantiate(moveAssetDatabase.GetByID(r).PowerUpPrefab, new Vector3(180, 1, 150), Quaternion.identity);
//			Debug.Log("here");
//			Debug.Log (moveAssetDatabase.GetByID(r).Name);
//		
//		}


		//Mapping the moveToAMethod
		/*if (GameObject.FindWithTag ("Prefab") == null)
		{
			Instantiate(Prefab, new Vector3(UnityEngine.Random.Range(-6, 8), 1, UnityEngine.Random.Range(-10, 3)), new Quaternion(0, 0,0,1));
		}
		var r = UnityEngine.Random.Range(0, 100);
		if (r > 90 && GameObject.FindWithTag("Prefab") == null) {
			var range = 3;
			Instantiate(Prefab, new Vector3(UnityEngine.Random.Range(-range, range), 1, UnityEngine.Random.Range(-range, range)), new Quaternion(0, 0,0,1));
			
		}*/
		
	}
	// public abstract void ExtendTail();
#region tailProtocols
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
        {
            newTail = (GameObject)Instantiate(legsPrefab, tailEnd.transform.position - tailEnd.transform.forward, new Quaternion(0, 0, 0, 0));
            newTail.GetComponent<Tail>().movementController = GetComponent<MovementController>();
            newTail.GetComponent<Tail>().isBackLeg = true;
        }
        else
            newTail = (GameObject)Instantiate(tailPrefab, tailEnd.transform.position - tailEnd.transform.forward, new Quaternion(0, 0, 0, 0));

		Tail tail =  newTail.GetComponent<Tail>();
		tail.GetComponent<Tail>().OwnerID = playerID;
		tail.GetComponent<Tail> ().matnum = matnum;
		tails.Add(tail);
		tail.tailNo = tails.Count - 1;

		// scale each segment smaller as the tail grows
		if (tail.tailNo > 2) {
			// tail segment scale
			Vector3 newScale = new Vector3(newTail.transform.Find ("Mid").transform.localScale.x * Mathf.Pow (0.97f, tails.Count),
			                               newTail.transform.Find ("Mid").transform.localScale.y * Mathf.Pow (0.97f, tails.Count),
			                               newTail.transform.Find ("Mid").transform.localScale.z);

			newTail.transform.Find ("Mid").transform.localScale = newScale; //old -> // - new Vector3(0.2f, 0.1f, 0);
			// adjust sphere scale
			Vector3 newScale2 = new Vector3(newTail.transform.Find ("Mid/Joint").transform.localScale.x * Mathf.Pow (0.97f, tails.Count),
			                                newTail.transform.Find ("Mid/Joint").transform.localScale.y * Mathf.Pow (0.97f, tails.Count),
			                                newTail.transform.Find ("Mid/Joint").transform.localScale.z * Mathf.Pow (0.97f, tails.Count));
			newTail.transform.Find ("Mid/Joint").transform.localScale = newScale2;
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
#endregion
    // Update is called once per frame
	void Update () {
		if (!suddenDeath) {
						//inputMouseButton = Input.GetMouseButton(0);
						inputMouseButton = Input.GetButton (Inputs [0]);

						if (Input.GetKeyDown (KeyCode.Space)) {
								ExtendTail2 ();
						}
						if (Input.GetButtonDown (Inputs [0])) {
								if (canDash)
										DashAttack (moves [0], 0);
						}

						if ((Input.GetButtonDown ("UsePower")) || Input.GetButton (Inputs [1])) {
								CastMove (0);
						}
						if ((Input.GetButtonDown ("UsePower")) || Input.GetButton (Inputs [2])) {
								CastMove (1);
						}
						if ((Input.GetButtonDown ("UsePower")) || Input.GetButton (Inputs [3])) {
								CastMove (2);
								//shielded = true;
						}
						if ((Input.GetButtonDown ("UsePower")) || Input.GetButton (Inputs [4])) {
								CastMove (3);
								//shielded = true;
						}
						if ((Input.GetButtonDown ("UsePower")) || Input.GetButton (Inputs [5])) {
								CastMove (4);
								//shielded = true;
						}
				}
		// increase speed during sudden death
		else if (rigidbody.velocity != Vector3.zero)
			this.GetComponent<MovementController> ().moveSpeed += 0.01f;
		
	}
	
	void OnCollisionEnter(Collision hit)
	{
		//**********SUDDEN DEATH
		/*if (suddenDeath) {
			if (hit.gameObject.tag == "Obstacle") {
				GameObject otherPlayer;
				if (playerID == 1)
				{
					otherPlayer = GameObject.FindGameObjectWithTag ("Player2");
					GameObject.Find ("Game").GetComponent<GameController>().SuddenDeathWinner(2);
				}
				else
				{
					otherPlayer = GameObject.FindGameObjectWithTag ("Player1");
					GameObject.Find ("Game").GetComponent<GameController>().SuddenDeathWinner(1);
				}
				AudioSource.PlayClipAtPoint(smash,gameObject.transform.position);
				otherPlayer.rigidbody.velocity = Vector3.zero;
				otherPlayer.GetComponent<MovementController>().moveSpeed = 0;
				Destroy (this.gameObject);
			}
		}*/

		//**********Tail collisions
		if (hit.gameObject.tag == "Tail")
		{
			int currentTail = hit.gameObject.GetComponent<Tail>().tailNo;
			string playerTag = "Player" + hit.gameObject.GetComponent<Tail>().OwnerID.ToString ();
			int ownerID = hit.gameObject.GetComponent<Tail>().OwnerID;

			// sudden death
			if (suddenDeath && ownerID != playerID) 
			{
				if (playerID == 1)
					GameObject.Find ("Game").GetComponent<GameController>().SuddenDeathWinner(1);
				else
					GameObject.Find ("Game").GetComponent<GameController>().SuddenDeathWinner(2);
				GameObject otherPlayer = GameObject.FindGameObjectWithTag (playerTag);
				AudioSource.PlayClipAtPoint(smash,gameObject.transform.position);
				Destroy(otherPlayer);
				rigidbody.velocity = Vector3.zero;
				GetComponent<MovementController>().moveSpeed = 0;
			}
			
			// Collision with OWN tail
			if (ownerID == playerID && tails.Count > 1 && !isBreaking) {
				AudioSource.PlayClipAtPoint(smash,gameObject.transform.position);
				Physics.IgnoreCollision(hit.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
				//isBreaking = true;
               // Instantiate(collisionVFX, hit.collider.transform.position, Quaternion.identity);
				//StartCoroutine(BreakingTail());
				//BreakTail(currentTail);
			} 
			
			// Bite powerup
			if (hasBitePowerup && ownerID != 0 && ownerID != playerID) {
				GameObject otherPlayer = GameObject.FindGameObjectWithTag (playerTag);
				otherPlayer.GetComponent<DragonBase> ().BreakTail(currentTail);
				AudioSource.PlayClipAtPoint(bite,gameObject.transform.position, 0.4f);
				Destroy (hit.gameObject);
				ExtendTail2();
			} 
			
			// Eat a broken tail piece
			if (hit.gameObject.GetComponent<Tail>().canEat && !isBreaking) {
				AudioSource.PlayClipAtPoint(bite,gameObject.transform.position, 0.4f);
				Destroy (hit.gameObject);
				ExtendTail2();
			} 
			// collision with enemy tail while not dashing
			if (ownerID != playerID && !hit.gameObject.GetComponent<Tail>().canEat && dashState == 0) 
			{
				AudioSource.PlayClipAtPoint(smash,gameObject.transform.position);
				Quaternion relative = Quaternion.Inverse (hit.gameObject.transform.rotation) * transform.rotation;
				//Deflect(relative);
				if (!hasBitePowerup){
					if (relative.y > 0)
						rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * 1000)));
					else
						rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * -1000)));
				}
			}
			
			// Collision with enemy tail while Dashing
			if (ownerID != playerID && !isBreaking && ownerID != 0) {
				if (dashState > 0 && !GameObject.FindWithTag(playerTag).GetComponent<DragonBase>().shielded) 
				{
					Instantiate(collisionVFX, hit.collider.transform.position, Quaternion.identity);
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
				else if (dashState > 0 && GameObject.FindWithTag(playerTag).GetComponent<DragonBase>().shielded) 
				{
					Quaternion relative = Quaternion.Inverse (hit.gameObject.transform.rotation) * transform.rotation;
					if (relative.y > 0)
						rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * 1000)));
					else
						rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * -1000)));
				}
			} 
		}
		//****************************
		if (hit.gameObject.tag == "food")
		{
			AudioSource.PlayClipAtPoint(bite,gameObject.transform.position, 0.4f);
			Destroy (hit.gameObject);
			ExtendTail2();
			Spawn.currentTailCount--;
		}
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
			CastMove(3);
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
			AudioSource.PlayClipAtPoint(smash, gameObject.transform.position);
			Deflect(relative);

		}
		
		// collision with obstacle
		else if (hit.gameObject.tag == "Obstacle") {
			Quaternion relative = Quaternion.Inverse (hit.gameObject.transform.rotation) * transform.rotation;
			AudioSource.PlayClipAtPoint(smash ,gameObject.transform.position);
			Deflect(relative);
			//BreakTail(1);
		}

		if ((hit.gameObject.tag == "Player1" || hit.gameObject.tag == "Player2") && hit.gameObject.tag != gameObject.tag )
		{ 
			if (hit.gameObject.GetComponent<DragonBase>().tails.Count == 0 && dashState == 3)
			{
				Destroy (hit.gameObject);
				GameObject.Find ("Game").GetComponent<GameController>().SuddenDeathWinner(playerID);
			}
		}
		
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
		// simple fix (for now)
		rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * 180)));
		//if (relative.y > 0)
		//	rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * 1000)));
		//else
		//	rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler((transform.up * -1000)));

	}
	
	void CastMove(int selectedIndex)
	{
		
		/* ToDO.
         * MoveList
         * 1) DONE - Dash
         * 2) DONE - Bite
         * 3) Sprint( x% boost in speed)
         * 4) Ice Shot 
         * 5) Fire Shot
         * 6) Sheild  
         */
        if (playerUI.moveSlots[selectedIndex].isAssigned)
        { 
        MoveData data = playerUI.moveSlots[selectedIndex].moveInfo;

        currentMoveMethods[data.ID].Invoke(data, selectedIndex);
        }

		//currentMoveMethods[moves[selectedIndex].ID].Invoke(moves[selectedIndex], selectedIndex);
	}
	
	void DashAttack(MoveData moveData, int index)
	{ 
		StartCoroutine(DashAttackCo(moveData));
	}
	
	void BiteAttack(MoveData moveData, int index)
	{
		StartCoroutine(BiteAttackCo(moveData,index));
	}
	
	void SprintAttack(MoveData moveData, int index)
	{
        StartCoroutine(SprintAttackCo(moveData, index));
	}
	
    void Sheild(MoveData moveData, int index)
    {
        StartCoroutine(SheildCo(moveData,index));
       BasicMesh.SetActive(false);
       SheildMesh.SetActive(true);

    }
    IEnumerator SheildCo(MoveData moveData,int index)
    {
		shielded = true;
        GetComponent<AudioSource>().PlayOneShot(moveData.clip);
        playerUI.moveSlots[index].Unassign();
        BasicMesh.SetActive(false);
		AudioSource.PlayClipAtPoint(shield ,gameObject.transform.position);
        SheildMesh.SetActive(true);
        for (int i = 0; i < tails.Count; i++ )
        {
           	tails[i].BasicMesh.SetActive(false);
            tails[i].SheildMesh.SetActive(true);
        }
            yield return new WaitForSeconds(3.0f);
		shielded = false;
        BasicMesh.SetActive(true);
        SheildMesh.SetActive(false);
        for (int i = 0; i < tails.Count; i++)
        {
            tails[i].BasicMesh.SetActive(true);
            tails[i].SheildMesh.SetActive(false);
        }
        yield return null;

    }
	void IceAttack(MoveData moveData, int index)
	{
        GameObject ice = (GameObject)Instantiate(moveData.VFXPrefab, transform.position + transform.forward * 1.7f +transform.up*1.5f, transform.rotation);

        GetComponent<AudioSource>().PlayOneShot(moveData.clip);
		AudioSource.PlayClipAtPoint(shootingbeam,gameObject.transform.position);
        //ice.GetComponent<EffectSettings>().Target.transform.position = ice.transform.position + transform.forward*10;
		ice.GetComponent<IceBullet>().ownerID = playerID;
		ice.GetComponent<IceBullet> ().type = ProjectileType.Ice;
        ice.GetComponent<IceBullet>().tag = gameObject.tag;
		hasIce = false;
        playerUI.moveSlots[index].Unassign();
	}
    void FireAttack(MoveData moveData, int index)
    {
        GameObject Fire = (GameObject)Instantiate(moveData.VFXPrefab, transform.position + transform.forward * 1.7f + transform.up * 1.5f, transform.rotation);
		AudioSource.PlayClipAtPoint(fire,gameObject.transform.position);
        GetComponent<AudioSource>().PlayOneShot(moveData.clip);
        //ice.GetComponent<EffectSettings>().Target.transform.position = ice.transform.position + transform.forward*10;
        Fire.GetComponent<IceBullet>().ownerID = playerID;
		Fire.GetComponent<IceBullet> ().type = ProjectileType.Fire;
        Fire.GetComponent<IceBullet>().tag = gameObject.tag;
        hasIce = false;
        playerUI.moveSlots[index].Unassign();
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
			DashTrail.SetActive (true);
			FireTrail.SetActive (false);
			DashTrail.transform.Find ("Trail").particleSystem.startSize = 4;
			DashTrail.transform.Find ("Trail").particleSystem.startColor = new Color(1,0,0);

			if (inputMouseButton)
			{
				moveData.currentChargeTime += 0.02f;

				// slow down while charging
				if (this.GetComponent<MovementController>().moveSpeed > 0)
					this.GetComponent<MovementController>().moveSpeed -= 0.1f;

				// change golor to green when dash is charged enough
				if (moveData.currentChargeTime >= moveData.ChargeTime)
				{
					Color newColor = new Color(0,1,0);
					DashTrail.transform.Find ("Trail").particleSystem.startColor = newColor;
					DashTrail.transform.Find ("Trail").particleSystem.startSize = 5.5f;
				}

				// Hold Dash for too long
				if (moveData.currentChargeTime > 6)
				{
					ResetSpeed();
					moveData.ResetCharge();
					DashTrail.SetActive(false);
					FireTrail.SetActive(true);
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
				/*if (moveData.currentChargeTime >= moveData.ChargeTime/3 && moveData.currentChargeTime < (moveData.ChargeTime * 2)/3)
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
				}*/

				// Perform Dash on Release
				if (moveData.currentChargeTime > moveData.ChargeTime)
				{
					StartCoroutine(PerformDash(3, 15));
					moveData.ResetCharge();

					break;
				}
				else{
					ResetSpeed();
					moveData.ResetCharge();
					DashTrail.SetActive(false);
					FireTrail.SetActive(true);
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
		canDash = false;
		// move the player if they are stopped
		GetComponent<MovementController>().isDashing = true;
		Color newColor;
		// different colors for each dash power level
		switch (state)
		{
		case 1:
			DashTrail.transform.Find ("Trail").particleSystem.startSize = 5;
			newColor = new Color(1,0,0);
			DashTrail.transform.Find ("Trail").particleSystem.startColor = newColor;
			break;
		case 2:
			DashTrail.transform.Find ("Trail").particleSystem.startSize = 6;
			newColor = new Color(0,1,0);
			DashTrail.transform.Find ("Trail").particleSystem.startColor = newColor;
			break;
		case 3:
			DashTrail.transform.Find ("Trail").particleSystem.startSize = 7;
			DashTrail.transform.Find ("Trail").particleSystem.startColor = oldColor;
			break;
		default:
			DashTrail.transform.Find ("Trail").particleSystem.startSize = 4;
			DashTrail.transform.Find ("Trail").particleSystem.startColor = oldColor;
			break;
		}
		IncreaseSpeed(speed);
		AudioSource.PlayClipAtPoint(dash ,gameObject.transform.position, 0.5f);
		Vector3 vel = rigidbody.velocity;
		yield return new WaitForSeconds(0.6f);
		ResetSpeed();
		rigidbody.velocity = vel;

		dashState = 0;
		canDash = true;
		GetComponent<MovementController>().isDashing = false;
		DashTrail.transform.Find ("Trail").particleSystem.startColor = oldColor;
		DashTrail.SetActive (false);
		FireTrail.SetActive (true);
	}
	
	void IncreaseSpeed(int speed) 
	{
		if (Mathf.Pow(0.98f, tails.Count) * moveSpeed > minSpeed)
			this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed + speed;
		else
			this.GetComponent<MovementController>().moveSpeed = minSpeed + speed;
	}

	public IEnumerator Frozen() 
	{
		canDash = false;
		this.GetComponent<MovementController> ().moveSpeed = minSpeed - 3;

		BasicMesh.SetActive(false);
		FrostMesh.SetActive(true);
		for (int i = 0; i < tails.Count; i++ )
		{
			tails[i].BasicMesh.SetActive(false);
			tails[i].FrostMesh.SetActive(true);
		}

		yield return new WaitForSeconds(4);

		BasicMesh.SetActive(true);
		FrostMesh.SetActive(false);
		for (int i = 0; i < tails.Count; i++)
		{
			tails[i].BasicMesh.SetActive(true);
			tails[i].FrostMesh.SetActive(false);
		}

		canDash = true;
		ResetSpeed ();
	}
	
	void ResetSpeed() 
	{
		if (Mathf.Pow(0.98f, tails.Count) * moveSpeed > minSpeed)
			this.GetComponent<MovementController>().moveSpeed = Mathf.Pow(0.98f, tails.Count) * moveSpeed;
		else
			this.GetComponent<MovementController>().moveSpeed = minSpeed;
	}
	
	IEnumerator BiteAttackCo(MoveData moveData,int index)
	{
        GetComponent<AudioSource>().PlayOneShot(moveData.clip);
        playerUI.moveSlots[index].Unassign();
		hasBitePowerup = true;
		canDash = false;
		Color oldcolor = transform.GetChild(3).GetChild(0).GetChild(0).renderer.material.color;
		Color newcolor = oldcolor;
		newcolor = new Color(1, 0.8f, 0);
        GameObject GO =(GameObject)Instantiate(moveData.VFXPrefab, transform.position + transform.forward * 1.7f + transform.up * 1.5f, transform.rotation);
        Destroy(GO, 2.0f);
		transform.GetChild(3).GetChild(0).GetChild(0).renderer.material.color = newcolor;
		transform.GetChild(3).GetChild(0).GetChild(0).renderer.material.SetTexture("_MainTex", mat[4]);
		yield return new WaitForSeconds(moveData.Cooldown);
		transform.GetChild(3).GetChild(0).GetChild(0).renderer.material.color = oldcolor;
		transform.GetChild(3).GetChild(0).GetChild(0).renderer.material.SetTexture("_MainTex", mat[matnum]);
		hasBitePowerup = false;
		canDash = true;
	}
	
	IEnumerator SprintAttackCo(MoveData moveData,int index)
	{
        playerUI.moveSlots[index].Unassign();
		canDash = false;
		GetComponent<MovementController>().isDashing = true;
        GetComponent<AudioSource>().PlayOneShot(moveData.clip);
		dashState = 3;

		IncreaseSpeed(6); 

		// set particle effects
		DashTrail.SetActive (true);
		FireTrail.SetActive (false);
		DashTrail.transform.Find ("Trail").particleSystem.startSize = 4;

		yield return new WaitForSeconds(moveData.Cooldown);
		DashTrail.SetActive (false);
		FireTrail.SetActive (true);
		ResetSpeed();

		GetComponent<MovementController>().isDashing = false;
		canDash = true;
		dashState = 0;
	}

	void SuddenDeathSprint() 
	{
		canDash = false;
		GetComponent<MovementController>().isDashing = true;
		dashState = 3;
		
		IncreaseSpeed(6); 
		
		// set particle effects
		DashTrail.SetActive (true);
		FireTrail.SetActive (false);
		DashTrail.transform.Find ("Trail").particleSystem.startSize = 4;
	}
}