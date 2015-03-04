using UnityEngine;
using System.Collections;

public class BasePowerup : MonoBehaviour {

	// Use this for initialization

    public MoveData move;

	void Start () {

        //TODO
       // move = getMoveFromList(Random[0, 3]);
        MoveData move = new MoveData();
        move.name = "Dash";
        move.Cooldown = 4.0f;
        move.ChargeTime = 4.0f;
        move.ID = 1;
        //moves.Add(Dash);
        //currentMoveMethods[Dash.ID] = DashAttack;
	}

    void onColliderHit(Collision hit)
    {
		if(hit.collider.tag == "Player1" || hit.collider.tag == "Player2")
        {
            GameObject go = hit.gameObject;

            go.GetComponent<DragonBase>().moves.Add(move);

            Destroy(this.gameObject);
             
        }
    }	
	// Update is called once per frame
	void Update () {
	
	}
}
