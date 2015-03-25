using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tail : MonoBehaviour
{
	public int tailNo;
	public int OwnerID;
	public bool canEat;
	public GameObject player;
	private float speed;
    public GameObject BasicMesh;
    public GameObject SheildMesh;
    public MovementController movementController;
    public Animator anim;
    public bool isBackLeg;
	void Start() 
	{
		canEat = false;
		// get the owner Tag and speed
		string playerTag = "Player" + OwnerID.ToString ();
		player = GameObject.FindGameObjectWithTag (playerTag);
		// set color
		if (OwnerID == 2) {
			Color colorM = transform.GetChild(0).GetChild(0).renderer.material.color;
			colorM.r = colorM.r * 0.5f;
			colorM.b = colorM.b * 1.5f;
			transform.GetChild(0).GetChild(0).renderer.material.color = colorM;
		}

		Color color = transform.GetChild(0).GetChild(0).renderer.material.color;
		if (tailNo > 4 && tailNo <= 10) 
		{
			color.r = color.r * 0.75f;
			transform.GetChild(0).GetChild(0).renderer.material.color = color;
		}
		else if (tailNo > 10) 
		{
			color.r = color.r * 0.5f;
			color.g = color.g * 0.6f;
			transform.GetChild(0).GetChild(0).renderer.material.color = color;
		}
	}

	void Update()
	{
		if (transform.parent != null) 
		{
			// calculate the relative angle between each tail segment and its parent
			Quaternion relative = Quaternion.Inverse (transform.parent.rotation) * transform.rotation;
			// if the angles differ enough, move object forward
			// gives better tail movement and control
			if ((relative.y > 0.05f || relative.y < -0.05f) && player.GetComponent<MovementController> ().isMoving)
			{
				speed = player.GetComponent<MovementController> ().moveSpeed;
				rigidbody.velocity = transform.forward * speed * 0.9f;
			}
            if(anim!=null)
            {

           
            if (player.GetComponent<MovementController>().isMoving)
                anim.SetFloat("Speed", 1);
            else
                anim.SetFloat("Speed", 0);
            }
            
		}
	}

	void OnCollisionEnter(Collision hit)
	{
		if (hit.gameObject.tag == "Tail")
		{
			int TownerID = hit.gameObject.GetComponent<Tail>().OwnerID;
			// ignore collisions with own tail pieces
			if (TownerID == OwnerID)
				Physics.IgnoreCollision(hit.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
		}
	}

	public void Break()
	{
		// can eat broken pieces for brief perioid
		canEat = true;
		// apply random force to scatter tail segments
		rigidbody.AddForce(new Vector3(Random.Range (-200,200), 0, Random.Range (-200,200)));
		// fade out and destroy
		StartCoroutine (FadeDestroy ());
	}

	IEnumerator FadeDestroy() 
	{
		yield return new WaitForSeconds(4);
		//TODO: Fadeout or particle explosion or whatever lol
		Destroy (this.gameObject);
	}

}

