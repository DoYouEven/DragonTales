using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Tail : MonoBehaviour
{
    public int tailNo;
    public int OwnerID;
	private GameObject player;
	private float speed;

	void Start() 
	{
		// get the speed of the owner
		player = GameObject.FindGameObjectWithTag (OwnerID.ToString ());
		speed = player.GetComponent<MovementController> ().moveSpeed;
	}

	void Update()
	{
		if (transform.parent != null) {
			// calculate the relative angle between each tail segment and its parent
			Quaternion relative = Quaternion.Inverse (transform.parent.rotation) * transform.rotation;
			// if the angles differ enough, move object forward
			// gives better tail movement and control
			if (relative.y > 0.05f || relative.y < -0.05f)
				rigidbody.velocity = transform.forward * speed * 0.9f;
		}
	}

}

