using UnityEngine;
using System.Collections;

public class BasePowerup : MonoBehaviour {

	// Use this for initialization

    public MoveData move;
    public int moveID;
	void Start () {

        //TODO
       // move = getMoveFromList(Random[0, 3]);
        //moves.Add(Dash);
        //currentMoveMethods[Dash.ID] = DashAttack;
       
        Destroy(gameObject, 10);
        
	}

    void OnTriggerEnter(Collider hit)
    {
		if(hit.collider.tag == "Player1" || hit.collider.tag == "Player2")
        {
            Destroy(this.collider);
            GameObject go = hit.gameObject;
            Spawn.currentSpawnCount--;
            go.GetComponent<DragonBase>().AddMoveByID(moveID);

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
