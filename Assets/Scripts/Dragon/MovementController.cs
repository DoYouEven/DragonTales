using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour
{

    // Use this for initialization
    private GameObject tailEnd;
    public GameObject tailPrefab;
    public bool alwaysMove = true;
    public bool isDashing;
    public bool isMoving = false;
    public float moveSpeed;
    public int initialTailCount;
    public KeyCode MoveForward;
    public string MoveForwardAxis;
    private float dx = 0;
    public float dy = 0;
    void Start()
    {
     

    }

    // Update is called once per frame

    void Update()
    {
        
        // temp player 2 controls
        if (tag == "Player1")
            dx =  Mathf.Lerp(dx,Input.GetAxis("Horizontal"),0.5f);// *10f + dx;
        else if (tag == "Player2")
            dx = Mathf.Lerp(dx, Input.GetAxis("HorizontalPlayer2"), 0.5f);// *10f + dx;
        //dx = Mathf.Clamp(dx, -50, 50);
       
        // forward movement
        if (alwaysMove || isDashing) {
            rigidbody.velocity = transform.forward * moveSpeed;
            isMoving = true;
        }
        else if (Input.GetButton(MoveForwardAxis))
        {
                rigidbody.velocity = transform.forward * moveSpeed ;
                isMoving = true;
            
        } else 
            isMoving = false;
		

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
                //rigidbody.AddTorque(transform.up * turnSpeed * dx, ForceMode.VelocityChange); //add more variables
                // smoother turning
                Quaternion deltaRotation = Quaternion.Euler((transform.up * turnSpeed * 50 * dx) * Time.deltaTime);
                rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
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