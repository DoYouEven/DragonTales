using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{

    // Use this for initialization
    public DragonBase dragonBase;
    public EnergyBar dashBar;
    public EnergyBar scoreBar;
    public List<MoveSlot> moveSlots;
    void Awake()
    {
        if(dragonBase == null)
        {
            Debug.Log("Please Attached Dragon to the UI");
        }

        dragonBase.onPowerup += AssignPowerUp;
        
    }


    void AssignPowerUp(MoveData moveData)
    {
        for(int i=0 ;i< moveSlots.Count; i++)
        {
            if(moveSlots[i].IsAssigned() == false)
            {
                moveSlots[i].Assign(moveData.icon);
                break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        dashBar.SetValueCurrentF(dragonBase.moves[0].currentChargeTime);

    }
}
