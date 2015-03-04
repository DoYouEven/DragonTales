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
	}

    void OnCollisionEnter(Collision hit)
    {
		if(hit.collider.tag == "Player1" || hit.collider.tag == "Player2")
        {
            GameObject go = hit.gameObject;

            go.GetComponent<DragonBase>().AddMoveByID(moveID);

            Destroy(this.gameObject);
             
        }
    }	
	// Update is called once per frame
	void Update () {
	
	}
}
