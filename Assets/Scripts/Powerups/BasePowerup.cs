using UnityEngine;
using System.Collections;

public class BasePowerup : MonoBehaviour {

	// Use this for initialization

    public MoveData move;
    public int moveID; 
	public AudioClip collect;
	void Start () {

        //TODO
       // move = getMoveFromList(Random[0, 3]);
        //moves.Add(Dash);
        //currentMoveMethods[Dash.ID] = DashAttack;
       
		collect = Resources.Load ("Sound/collect") as AudioClip;
		Destroy(gameObject, 25);
        
	}

    void OnTriggerEnter(Collider hit)
    {
		if(hit.collider.tag == "Player1" || hit.collider.tag == "Player2")
        {
            Destroy(this.collider);
			GameObject go = hit.gameObject;
            Spawn.currentSpawnCount--;
            go.GetComponent<DragonBase>().AddMoveByID(moveID);
			Debug.Log("basepowerup:i got it");
			AudioSource.PlayClipAtPoint(collect,gameObject.transform.position);
            Destroy(this.gameObject);
		
        }
    }	
    void OnDestroy()
    {
        Spawn.currentSpawnCount--;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
