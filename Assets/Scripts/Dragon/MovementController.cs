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

    void Start()
    {
     

    }

    // Update is called once per frame

    void Update()
    {


        var dx = Input.GetAxis("Horizontal");
        var dy = Input.GetAxis("Vertical");

        // forward move


        if (Input.GetKey(MoveForward))
            rigidbody.velocity = transform.forward * moveSpeed;

        // turning
        var turnSpeed = 5;
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
    }


    
}



