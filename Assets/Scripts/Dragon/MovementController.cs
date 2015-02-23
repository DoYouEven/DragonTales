using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour
{

    // Use this for initialization
    private GameObject tailEnd;
    public GameObject tailPrefab;
    public float moveSpeed;
    public int initialTailCount;
    public KeyCode MoveForward;
	float dx = 0;

    void Start()
    {
     

    }

    // Update is called once per frame

    void Update()
    {


        dx = Input.GetAxis("Horizontal");// *10f + dx;
		//dx = Mathf.Clamp(dx, -50, 50);


        // forward move


        if (Input.GetKey(MoveForward))
            rigidbody.velocity = transform.forward * moveSpeed;

        // turning
        var turnSpeed = 4;

        if (Input.acceleration.sqrMagnitude != 0)
        {
            turnSpeed = 500;
            //rigidbody.AddTorque(-transform.up*turnSpeed*counter*Input.acceleration.y*0.5, ForceMode.Impulse);
            transform.rotation *= Quaternion.Euler(0, -Input.acceleration.y * Time.deltaTime * turnSpeed, 0);
        }
        else
        { // pc
            if (dx != 0)
            {
                rigidbody.AddTorque(transform.up * turnSpeed * dx, ForceMode.Impulse); //add more variables
            }
        }
        /*
        if (Input.acceleration.sqrMagnitude != 0)
        {
            turnSpeed = 500;
            //rigidbody.AddTorque(-transform.up*turnSpeed*counter*Input.acceleration.y*0.5, ForceMode.Impulse);
            //transform.rotation *= Quaternion.Euler(0, -Input.acceleration.y * Time.deltaTime * turnSpeed, 0);
        }
        else
        { // pc
            if (dx != 0)
            {
				transform.rotation = Quaternion.Euler(0, dx, 0);
            }
        }
         * */
    }


    
}



