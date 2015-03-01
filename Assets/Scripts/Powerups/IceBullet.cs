using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class IceBullet : MonoBehaviour
{

	public int ownerID;
	private string playerTag;

	void Start() 
	{
		playerTag = "Player" + ownerID.ToString ();
	}

	void Update()
	{
		rigidbody.velocity = transform.forward * 25;
	}

	void OnCollisionEnter (Collision collision) 
	{
		if (collision.gameObject.tag == playerTag) 
    		Physics.IgnoreCollision(collision.collider, collider);
    	// ToDO: slow down enemy player
    } 

}