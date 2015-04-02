using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public enum CameraState
{
    FirstPerson,
    ThirdPerson
}
public class CameraController : MonoBehaviour
{
    // Feel free to change them to public if they need to be accessed by other classes
    public Transform target;
    public Vector2 heightClamp;
    public float ZFactor = 1;
    public float YFactor = 1;
    public float YSmoothingFactor;
    public Quaternion RotationVector;
    public LayerMask collisionLayers = new LayerMask();
    public float collisionAlpha = 0.15f;
    public float collisionFadeSpeed = 10;
    public GameObject SmoothCamera = null;
    public bool allowRotation = true;
    public bool lockCursor = true;
    public bool invertX = false;
    public bool invertY = false;
    public bool stayBehindTarget = false;

    public Vector2 targetOffset = new Vector2();

    public bool rotateObjects = true;
    public List<Transform> objectsToRotate;


   

    public Vector2 originRotation = new Vector2();
    public bool returnToOrigin = true;
    public float returnSmoothing = 3;
    public Vector3 CollisionSmooth;
    [SerializeField]
    private float
            distance = -4f;
    [SerializeField]
    private float
            minDistance = 5f;
    [SerializeField]
    private float
            maxDistance = 25f;
    public float collisionDistance;
    public Vector2 sensitivity = new Vector2(3, 3);

    public float zoomSpeed = 2.5f;
    public float zoomSmoothing = 7f;

    public float zoomAltDelay = 0.5f;

    private float minAngle = -45;
    private float maxAngle = 45;
    private bool _canRotate = true;
    private List<Material> _faded_mats = new List<Material>();   // I have yet to implement this properly
    private List<Material> _current_faded_mats = new List<Material>(); // I have yet to implement this properly

    private float wantedDistance;
    private Quaternion _rotation;
    private Vector2 inputRotation;
    private bool isFirstPerson = false;       //TDDO need to get read to these bool states.
    private bool isThirdPerson = true;
    private static bool isPlayerControl = true;
    public Transform _transform;
    // addition rotation smooth components
    public float mouseSmoothingFactor = 0.08f;

    //*******Mouse Variables***********
    private float mouseXSmooth = 0f;
    private float mouseXVel;
    private float mouseYSmooth = 0f;
    private float mouseYVel;
    //**********************************
    //******** Camera State************
    public static CameraState currentCameraState = CameraState.ThirdPerson;
    //**************************
    public static bool Controllable
    {            // incase we would be need to modifier our setters later.
        get { return isPlayerControl; }
        set { isPlayerControl = value; }
    }
    public bool CanRotate
    {            // incase we would be need to modifier our setters later.
        get { return _canRotate; }
        set { _canRotate = value; }
    }


    public GameObject player1;
    public GameObject player2;
    void Start()
    {
      
        //ThirdPersonCamera();
        _transform = transform;
        wantedDistance = distance;
        inputRotation = originRotation;

        // If no target set, warn the user
        if (!target)
        {
            Debug.LogWarning("Set Target BITCH");
        }
    }

    void Update()
    {
		if (player1 != null && player2 != null) {
       	 	Debug.DrawLine(player1.transform.position, player2.transform.position);
        	target.position = (player1.transform.position + player2.transform.position) / 2;
			float dist = Mathf.Abs(player1.transform.position.z - player2.transform.position.z);
			YFactor = 0.5f + (dist/160);
			ZFactor = 0.75f + (dist/480);
			this.camera.fieldOfView = 60 + (dist/20);
			//Debug.Log(Mathf.Abs(player1.transform.position.z - player2.transform.position.z));
	        wantedDistance = (player1.transform.position - player2.transform.position).magnitude * ZFactor ;
	        //targetOffset.y = (player1.transform.position - player2.transform.position).magnitude * YFactor;
	        float lerp = Mathf.Lerp(targetOffset.y, (player1.transform.position - player2.transform.position).magnitude * YFactor, YSmoothingFactor);
	        targetOffset.y = Mathf.Clamp(lerp,heightClamp.x,heightClamp.y);
	        RotationVector.x = Mathf.Clamp(lerp, 0.3f, 0.55f);

	        target.transform.localRotation = new Quaternion(0, 1, 0,1);


            if (isPlayerControl)
            {


               
            }
        

        //if (Screen.lockCursor) {
        //Screen.lockCursor = false;
        //}


		}
    }


    void LateUpdate()
    {
        Screen.lockCursor = true;

        if (currentCameraState == CameraState.FirstPerson || currentCameraState == CameraState.ThirdPerson)
            SmoothFollow(target);


    }


    #region Methods
    private void SmoothFollow(Transform target)
    {


        if (target)
        {
            if (isPlayerControl)
            {
                // Zoom control
              
            }

            // Prevent wanted distance from going below or above min and max distance
            wantedDistance = Mathf.Clamp(wantedDistance, minDistance, maxDistance);

            // If user clicks, change position based on drag direction and sensitivity
            // Stop at 90 degrees above / below object
            if (allowRotation)
            { //&& (Input.GetMouseButton (0) || Input.GetMouseButton (1))) {
                if (isPlayerControl)
                {
                    if (invertX)
                    {
                        //inputRotation.x -= Input.GetAxis("Mouse X") * sensitivity.x;
                    }
                    else
                    {
                       // inputRotation.x += Input.GetAxis("Mouse X") * sensitivity.x;

                    }

                    //ClampRotation ();

                    if (invertY)
                    {
                      //  inputRotation.y += Input.GetAxis("Mouse Y") * sensitivity.y;
                    }
                    else
                    {//
                       // inputRotation.y -= Input.GetAxis("Mouse Y") * sensitivity.y;
                    }

                    inputRotation.y = Mathf.Clamp(inputRotation.y, minAngle, maxAngle);
                    mouseXSmooth = Mathf.SmoothDamp(mouseXSmooth, inputRotation.x, ref mouseXVel, mouseSmoothingFactor);
                    mouseYSmooth = Mathf.SmoothDamp(mouseYSmooth, inputRotation.y, ref mouseYVel, mouseSmoothingFactor);
                    _rotation = Quaternion.Euler(mouseYSmooth, mouseXSmooth, 0);

                    // Force the target's y rotation to face forward (if enabled) when right clicking
                    if (rotateObjects)
                    {
                        //if (Input.GetMouseButton (1)) {
                        foreach (Transform o in objectsToRotate)
                        {
                            o.rotation = Quaternion.Euler(0, 90, 0);
                        }
                        //}
                    }

                    // If user is right clicking, set the default position to the current position
                    //if (Input.GetMouseButton (1)) {
                    //originRotation = inputRotation;
                    //ClampRotation ();
                    //}
                }
            }
            else
            {
                // Keeps the camera behind the target when not controlling it (if enabled)
                if (stayBehindTarget)
                {
                    originRotation.x = target.eulerAngles.y;
                    ClampRotation();
                }

                // If Return To Origin, move camera back to the default position
                if (returnToOrigin && Input.GetKeyDown(KeyCode.C))
                {
                    inputRotation = Vector3.Lerp(inputRotation, originRotation, returnSmoothing * Time.deltaTime);
                }

                _rotation = Quaternion.Euler(mouseYSmooth, mouseXSmooth, 0);
            }

            // Lerp from current distance to wanted distance
           
            distance = Mathf.Clamp(Mathf.Lerp(distance, wantedDistance, Time.deltaTime * zoomSmoothing), minDistance, maxDistance);

            // Set wanted position based off rotation and distance
            Vector3 wanted_position = _rotation * new Vector3(targetOffset.x, 0, -wantedDistance - 0.2f) + target.position + new Vector3(0, targetOffset.y, 0);
            Vector3 current_position = _rotation * new Vector3(targetOffset.x, 0, 0) + target.position + new Vector3(0, targetOffset.y, 0);


            // Linecast to test if there are objects between the camera and the target using collision layers
            RaycastHit hit;

            if (Physics.Linecast(current_position, wanted_position, out hit, collisionLayers))
            {
                distance = Vector3.Distance(current_position, hit.point) - 0.2f;
                //Screen.lockCursor = true;
            }



            // Set the position and rotation of the camera
            _transform.position = _rotation * new Vector3(targetOffset.x, 0.0f, -distance) + target.position + new Vector3(0.0f, targetOffset.y, 0);
            _transform.rotation = RotationVector;
        }
    }
    private void ClampRotation()
    {
        if (originRotation.x < -180)
        {
            originRotation.x += 360;
        }
        else if (originRotation.x > 180)
        {
            originRotation.x -= 360;
        }

        if (inputRotation.x - originRotation.x < -180)
        {
            inputRotation.x += 360;
        }
        else if (inputRotation.x - originRotation.x > 180)
        {
            inputRotation.x -= 360;
        }
    }
    private void ThirdPersonCamera()
    {
        minDistance = -5f;
       // targetOffset.y = 0.9f;
    }
    private void FirstPersonCamera()
    {
        isFirstPerson = true;
        isThirdPerson = false;
        targetOffset.y = 2;

    }




    public void DisableCameraRotation()
    {
        rotateObjects = false;
    }
    public void EnableCameraRotation()
    {
        rotateObjects = true;
    }
    #endregion
  
}