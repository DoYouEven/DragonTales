using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 public enum ProjectileType
 {
     Fire,
     Ice
}

public class IceBullet : MonoBehaviour
{
   
	public int ownerID;
	private string playerTag;
    public GameObject explosion;
    private bool isCollided;
	public ProjectileType type;// = ProjectileType.Ice;
	void Start() 
	{
        Destroy(this.gameObject, 5f);
		playerTag = "Player" + ownerID.ToString ();
        isCollided = false;
	}

	void Update()
	{
        //rigidbody.AddForce(transform.forward * 25,ForceMode.Impulse);
        if (!isCollided)
            rigidbody.velocity = transform.forward * 25;
        else
            rigidbody.velocity = Vector3.zero;
	}

	void OnCollisionEnter (Collision collision) 
	{
       isCollided = true;
       rigidbody.velocity = Vector3.zero;
       Destroy(gameObject.collider);
       explosion.SetActive(true);

        //if(collision.gameObject.tag != playerTag)
        //{

		DragonBase dragon = null;
		int currentTail = 1;

		if (collision.gameObject.tag == "Tail") {
			dragon = collision.gameObject.GetComponent<Tail>().player.GetComponent<DragonBase>() ;
			currentTail = collision.gameObject.GetComponent<Tail>().tailNo;
		}
		else if (collision.gameObject.tag == playerTag)
			dragon = collision.gameObject.GetComponent<DragonBase>() ;

	    if (dragon != null)
	    {
	        switch(type)
	        {
	        case ProjectileType.Ice :
				Debug.Log ("FreezeMe");
				StartCoroutine(dragon.Frozen());
				break;
	        case ProjectileType.Fire :
				dragon.BreakTail(currentTail);	
				break;  // Break the dragon at the give tail point
	        }
	        Debug.Log("hit dragon " + dragon.tag);
	    }
        //}

		//f (collision.gameObject.tag == playerTag) 
    		//Physics.IgnoreCollision(collision.collider, collider);
        
    	// ToDO: slow down enemy player
    } 

}