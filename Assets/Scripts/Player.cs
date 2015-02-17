using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    // Use this for initialization
    private GameObject tailEnd;
    public GameObject tailPrefab;
    public float moveSpeed;
    public int initialTailCount;
    public KeyCode MoveForward;

    void Start()
    {
        for (int i = 0; i < initialTailCount; i++)
        {
            AddTail();
        }

    }

    // Update is called once per frame

    void Update()
    {


        var dx = Input.GetAxis("Horizontal");
        var dy = Input.GetAxis("Vertical");

        // forward move

        //var fwd = transform.forward*moveSpeed*dy;
        // rigidbody.AddForce(fwd, ForceMode.Impulse);
        //rigidbody.AddForce(transform.forward*moveSpeed, ForceMode.Force);
        if (Input.GetKeyDown(KeyCode.Space))
            AddTail();

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


    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.tag == "Tail")
        {
            Destroy(hit.gameObject);
            AddTail();
        }
    }
    public void AddTail()
    {
        if (tailEnd == null)
            tailEnd = this.gameObject;


        GameObject newTail = (GameObject)Instantiate(tailPrefab, tailEnd.transform.position - tailEnd.transform.forward , new Quaternion(0, 0, 0, 0));
        newTail.transform.rotation = tailEnd.transform.rotation;
        Joint joint = newTail.GetComponent<HingeJoint>();
        if (joint != null)
        {
            joint.connectedBody = tailEnd.rigidbody;
            tailEnd = newTail;
        }


        rigidbody.mass++; // make the head weight greater so it can carry it's tail... lol
        moveSpeed += 0.05f;

        return;

    }


    void onCollisionEnter(Collision c)
    {
        if (c.gameObject == null)
            return;

        // restart on wall hit 
        if (c.gameObject.CompareTag("Respawn"))
        {
            Application.LoadLevel("test1");
        }

        if (c.rigidbody == null)
            return;

        if (c.gameObject.name == "Apple(Clone)")
        {

            Destroy(c.gameObject);
            AddTail();
            return;
        }
    }
}



